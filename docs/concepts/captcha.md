---
title: CAPTCHA Verification
layout: home
parent: Concepts
nav_order: 700
---

# {{ page.title }}

Public-write surfaces — signup, password reset, contact forms, anonymous submissions — need bot mitigation. LightNap provides an `ICaptchaService` abstraction with four implementations and a `[ValidateCaptcha]` action filter for declarative per-endpoint protection.

## Providers

| Provider           | When to pick it                                                                   |
|--------------------|-----------------------------------------------------------------------------------|
| `None` (default)   | Local development, automated tests. NoOp implementation always succeeds.          |
| `Turnstile`        | Privacy-conscious deployments. Free tier, no CAPTCHA puzzle.                      |
| `RecaptchaV2`      | Existing Google Cloud accounts; binary success/failure with a visible challenge.  |
| `RecaptchaV3`      | Score-based; no UI; you set a minimum score threshold.                            |

Selection is via `Captcha:Provider` in configuration. Switching providers does not require code changes — only the matching settings sub-section.

## Configuration

```jsonc
"Captcha": {
  "Provider": "Turnstile",
  "RejectOnProviderError": true,
  "Turnstile": {
    "SiteKey": "1x00000000000000000000AA",
    "SecretKey": "1x0000000000000000000000000000000AA"
  }
}
```

For `RecaptchaV2` / `RecaptchaV3`, supply the matching sub-section and (for v3) a `MinScore`.

### Fail-closed vs fail-open

`RejectOnProviderError` controls behavior when the upstream provider is unreachable:

- `true` (default, fail-closed): the request is rejected. Safer; prioritizes bot mitigation over availability.
- `false` (fail-open): the request is accepted. Prioritizes availability over strict bot mitigation. Pick this when an outage of the CAPTCHA provider would unacceptably break user signup.

The underlying `HttpClient` for each provider is wired through [LightNap's resilience helper](./http-resilience), so the standard retry / circuit-breaker / timeout policies apply before this flag ever kicks in.

## Protecting an endpoint

Decorate any controller action with `[ValidateCaptcha]`:

```csharp
[HttpPost("contact")]
[ValidateCaptcha]
public async Task<IActionResult> SubmitContactForm([FromBody] ContactDto body)
{
    // Reach here only if the X-Captcha-Token header validated.
}
```

The filter:

1. Reads the token from the `X-Captcha-Token` request header.
2. If the header is missing, returns `400` with `{ "error": "captcha_required" }`.
3. Calls `ICaptchaService.ValidateAsync`. If the result is unsuccessful, returns `400` with `{ "error": "captcha_invalid", "errorCodes": [...] }`.
4. Otherwise calls the next handler.

## Client-side wiring

The widget render and token submission happen in the SPA. Expose the `SiteKey` to the browser through your existing client-config endpoint (LightNap already has a `PublicService` for this kind of data), and have the SPA send the resulting token in the `X-Captcha-Token` header on submission.
