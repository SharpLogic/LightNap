---
title: Anonymous Visitor Tracking
layout: home
parent: Concepts
nav_order: 500
---

# {{ page.title }}

LightNap apps with anonymous-input features (comments, ratings, public submissions, anonymous analytics, A/B test buckets) need persistent identity for visitors who are not logged in. IP-only identification loses identity on every NAT or VPN hop; rolling your own cookie scheme is error-prone. LightNap ships an opt-in middleware that mints and reads a first-party visitor cookie, then exposes the identifier on the current request.

## What the middleware does

On every request, `AnonymousVisitorIdMiddleware`:

1. Looks for the configured cookie (default name `lna_visitor_id`).
2. If a valid GUID is present, copies it to `HttpContext.Items["AnonymousVisitorId"]`.
3. Otherwise mints a new GUID, sets the cookie on the response, and stores the new value on `HttpContext.Items`.

Two consumers read the item:

- `WebUserContext` resolves `IUserContext.Kind` to `UserContextKind.AnonymousVisitor` when the request is unauthenticated but a visitor ID is set. `GetActorId()` then returns the visitor ID, which downstream code can use for audit, last-modified-by, or partition-key purposes.
- The rate-limit partitioner prefers the visitor ID over the remote IP fallback, so unauthenticated users behind shared NATs do not all share a single bucket.

## When to enable it

Turn it on when your app:

- Accepts anonymous user-generated content (comments, votes, public form submissions).
- Correlates anonymous analytics or experiment buckets across requests.
- Wants per-visitor rate limiting that survives IP changes.

If your app has no anonymous input surface, leave it off. The middleware is not registered by default — consumers that don't need it pay nothing.

## Enabling

In `Program.cs`, after the existing `Authentication` settings are loaded:

```csharp
var anonymousVisitorSettings = builder.Configuration
    .GetRequiredSection<AnonymousVisitorSettings>("AnonymousVisitor");
builder.Services.AddLightNapAnonymousVisitorTracking(anonymousVisitorSettings, bootstrapLogger);
```

And in the pipeline, after `UseAuthentication()` and before the endpoints:

```csharp
app.UseLightNapAnonymousVisitorTracking();
```

In `appsettings.json`, add:

```jsonc
"AnonymousVisitor": {
  "CookieName": "lna_visitor_id",
  "Lifetime": "365.00:00:00",
  "SecureOnly": true
}
```

Both ends are commented out in the stock `Program.cs` to make the opt-in explicit.

## Cookie attributes

| Attribute    | Default                  | Why                                                                                            |
|--------------|--------------------------|------------------------------------------------------------------------------------------------|
| `HttpOnly`   | `true`                   | The cookie is server-side only; no script needs to read it.                                    |
| `SameSite`   | `Lax`                    | Sent on same-site and top-level navigation; not on third-party iframes.                        |
| `Secure`     | `true` (via `SecureOnly`)| HTTPS only. Set `false` for local HTTP development.                                            |
| `Expires`    | 1 year                   | Long enough to persist across browser restarts; short enough to limit linkability over time.   |
| `Path`       | `/`                      | Used by the whole app.                                                                         |

## Privacy and retention

The visitor cookie is anonymous — it does not by itself reveal who a person is. It does, however, link a person's actions over time. If your app **persists** the visitor identifier or `IUserContext.GetIpAddress()` on durable rows (audit log, user-generated content), document a retention policy that matches the rest of your privacy posture and prune older rows accordingly. See the [Audit Log](./audit-log) docs for a maintenance-task pattern.

## How `IUserContext.GetActorId()` interacts

Once the middleware is registered, the contract from the [IUserContext](./project-structure) primitive resolves cleanly without branching:

| Request kind                      | `Kind`              | `GetActorId()`           |
|-----------------------------------|---------------------|--------------------------|
| Authenticated                     | `Authenticated`     | User ID                  |
| Unauthenticated, visitor cookie   | `AnonymousVisitor`  | Visitor GUID             |
| Unauthenticated, no cookie        | `Anonymous`         | Throws                   |
| Background job / seeder           | `System`            | `"system"`               |

Callers writing audit rows or partitioning anonymous data just call `GetActorId()`; the framework guarantees the right answer.
