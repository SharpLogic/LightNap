---
title: HTTP Resilience
layout: home
parent: Concepts
nav_order: 700
---

# {{ page.title }}

Outbound HTTP calls to services you don't control fail in inconvenient ways: transient 503s, slow responses that pile up into thread exhaustion, and downstream outages that need to be cut off rather than retried indefinitely. LightNap ships a single extension method that wires `HttpClient` with the .NET standard resilience handler so every outbound client gets sensible defaults from the same convention.

## When to use it

Use `AddLightNapResilientHttpClient<...>()` for any outbound HTTP call to something LightNap doesn't own:

- Third-party API integrations.
- OAuth and identity providers.
- Webhook delivery to consumer callbacks.
- CAPTCHA verification.
- Payment gateways.

Skip it for purely in-process or in-cluster traffic where the platform layer already provides retry semantics (service-to-service Kubernetes mesh, for instance).

## What you get

The `AddStandardResilienceHandler()` policy from `Microsoft.Extensions.Http.Resilience` provides:

- **Retry** with exponential backoff and jitter for transient failures (5xx, 408, network errors).
- **Total request timeout** and per-attempt timeout, so a single slow downstream call can't pin a thread indefinitely.
- **Circuit breaker** to short-circuit a stream of failing calls before they pile up.
- **Concurrency limiter** to prevent a runaway caller from saturating the connection pool.

All defaults come from Microsoft's recommended configuration and are appropriate for typical HTTP-to-API workloads.

## Usage

```csharp
// Typed client
services.AddLightNapResilientHttpClient<IMyApiClient, MyApiClient>();

// Typed client, single class form
services.AddLightNapResilientHttpClient<MyApiClient>();

// Named client
services.AddLightNapResilientHttpClient("my-api");
```

The returned `IHttpClientBuilder` supports the standard `ConfigureHttpClient`, `ConfigurePrimaryHttpMessageHandler`, etc. fluent calls if you need to layer additional configuration.

## Per-client overrides

To override the standard handler's policy (longer total timeout, custom retry count) for a single client, fall through to the underlying handler call:

```csharp
services.AddHttpClient<IMyApiClient, MyApiClient>()
    .AddStandardResilienceHandler(options =>
    {
        options.TotalRequestTimeout.Timeout = TimeSpan.FromMinutes(2);
        options.Retry.MaxRetryAttempts = 5;
    });
```

Mixing styles is fine — `AddLightNapResilientHttpClient` is just a one-line shortcut for the most common case.

## Idempotency caveat

The standard resilience handler retries 5xx responses by default for all verbs. The Microsoft default is conservative about which verbs are retried; if your downstream service treats POSTs as non-idempotent, supply an idempotency key (see the [Idempotency-Key Filter](../development-guide/features/idempotency) docs) or constrain retries in the per-client configuration.
