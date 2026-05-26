---
title: Idempotency-Key Filter
layout: home
parent: Concepts
nav_order: 1100
---

# {{ page.title }}

Network drops between client and server cause duplicate POSTs when clients retry. Without an idempotency mechanism, a double-click on submit double-counts and a flaky mobile network creates duplicate records. The `[Idempotent]` filter implements the Stripe-style `Idempotency-Key` convention: the first call's response is cached and replayed for subsequent calls with the same key.

## Using the filter

Decorate any mutating action:

```csharp
[HttpPost("payments")]
[Idempotent]
public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentDto dto) { ... }
```

The client supplies a unique value per logical operation in the `Idempotency-Key` request header (commonly a UUID generated at the moment of form submission and retained across retries).

## Behavior

| Scenario                                                          | Result                                              |
|-------------------------------------------------------------------|-----------------------------------------------------|
| Header missing                                                    | Pass through. No caching, no replay.                |
| Key not seen before                                               | Action runs. If response is 2xx, body is cached.    |
| Same key, same method+path, within TTL                            | Cached body is replayed as `200 OK application/json`.|
| Same key, different method or path                                | Cache miss — action runs.                           |
| Action returned a non-2xx (4xx / 5xx) or threw                    | Not cached. Retries re-execute the action.          |

The cache key composes method, path, and the client-supplied key, so a client recycling a key against an unrelated endpoint cannot poison the entry.

## Configuration

```jsonc
"Idempotency": {
  "Ttl": "01:00:00"
}
```

Default is one hour. Pick the TTL based on how long a client might reasonably retry the same logical operation — long enough to cover retry-after-delay flows, short enough that an unrelated future request with the same UUID (vanishingly unlikely with a fresh UUID, but possible if clients reuse keys) does not hit a stale entry.

## Storage

The filter uses the existing LightNap `HybridCache`. In single-instance mode, that's an in-process L1 cache. In distributed mode (Redis enabled), the cache is shared across instances, so a key generated on one node is honored by another. Replays survive a single-pod restart only when distributed mode is configured.

## When not to use it

- **GET endpoints**: GETs should be safe to repeat by definition. Cache them with normal HTTP cache headers instead.
- **Idempotent-by-design POSTs**: If your action is idempotent at the data layer (upserts by a natural key), the filter is redundant. Add it only when re-execution would be observably wrong.
- **Long-running operations**: If the cached response only makes sense in tandem with side effects that you wouldn't want to skip on retry, prefer a job-status pattern (`202 Accepted` plus a poll endpoint) over idempotency caching.

## Notes for callers

A 4xx response is not cached — retries will re-execute the action. Clients should treat the second response as authoritative when a first attempt's response was lost or unparseable. If you need the original error to be replayed verbatim, expand the cache predicate; the default keeps the cache small by storing only successful results.
