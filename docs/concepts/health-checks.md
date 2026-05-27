---
title: Health Check Endpoints
layout: home
parent: Concepts
nav_order: 800
---

# {{ page.title }}

LightNap exposes two HTTP endpoints intended for container orchestrators, load balancers, and uptime monitors:

| Endpoint        | Purpose   | What it checks                                            |
|-----------------|-----------|-----------------------------------------------------------|
| `/health/live`  | Liveness  | Nothing. Responding is enough — the process is up.        |
| `/health/ready` | Readiness | The database (and Redis when distributed mode is enabled).|

Both endpoints are anonymous and bypass rate limiting, so probes can not be throttled or pushed to the auth flow.

## Why liveness and readiness are different

- **Liveness** answers "should the orchestrator restart this pod?" If the process is responding, it's alive. Liveness probes that check dependencies (database, Redis) cause cascading restarts during downstream outages, which is rarely what you want.
- **Readiness** answers "should we route requests to this instance right now?" An instance whose database is unreachable should still stay up (so a transient blip recovers without churn), but should be removed from the load-balancer pool until the dependency recovers.

Following this distinction is what `MapHealthChecks` with the `"ready"` tag predicate gives you out of the box.

## Extending the readiness probe

Add additional checks with the `"ready"` tag in `AddLightNapHealthChecks` or anywhere downstream:

```csharp
builder.Services
    .AddLightNapHealthChecks(useDistributed, redisConnection, bootstrapLogger);

// Later, in a feature module that adds its own dependency:
builder.Services.AddHealthChecks()
    .AddCheck<MyDownstreamApiHealthCheck>("downstream-api", tags: ["ready"]);
```

The readiness endpoint evaluates every check tagged `"ready"`, so adding to the list does not require touching `Program.cs` again.

## Liveness behavior

`/health/live` uses `Predicate = _ => false`, which means *no* checks run — the endpoint returns `200 Healthy` as long as the routing layer can serve it. That's the right shape for liveness: if you want richer instrument data, expose it through your telemetry pipeline (`ITelemetryClient`), not through liveness.

## Authentication and rate limiting

Both endpoints call `AllowAnonymous()` and `DisableRateLimiting()`. Liveness and readiness probes hit frequently (every few seconds in most orchestrators); applying rate limits to them would create false readiness failures under load. Most orchestrators do not authenticate when probing, so `AllowAnonymous()` is required.

If your security posture requires probes to be authenticated, restrict at the network layer (orchestrator-internal probes only) rather than at the application layer.
