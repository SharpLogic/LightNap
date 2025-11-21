---
title: Rate Limiting
layout: home
parent: Security & Authorization
nav_order: 100
---

# {{ page.title }}

Rate limiting is a security feature that helps protect your API from abuse by limiting the number of requests a user or IP address can make within a given time window. LightNap implements rate limiting using ASP.NET Core's built-in rate limiting middleware with fixed window limiters.

## How It Works

Rate limiting in LightNap uses the following approach:

- **Partitioning**: Requests are partitioned by authenticated user ID (if available) or client IP address. This ensures that legitimate users aren't penalized by shared IP addresses, while unauthenticated requests are limited by IP.
- **Fixed Window**: Uses a fixed time window of 1 minute for all limits.
- **Policies**: Different endpoints have different rate limits based on their sensitivity and expected usage patterns.

## Rate Limiting Policies

LightNap defines several rate limiting policies:

### Global Policy

- **Applied to**: All API endpoints
- **Limit**: Configurable via `RateLimiting:GlobalPermitLimit` (default: 100 requests/minute)
- **Purpose**: Provides baseline protection against excessive requests

### Auth Policy

- **Applied to**: Authentication endpoints (login, token refresh, etc.)
- **Limit**: Configurable via `RateLimiting:AuthPermitLimit` (default: 50 requests/minute)
- **Purpose**: Protects against brute force attacks on authentication

### Content Policy

- **Applied to**: Content management endpoints
- **Limit**: Configurable via `RateLimiting:ContentPermitLimit` (default: 200 requests/minute)
- **Purpose**: Allows higher limits for content-heavy operations while still providing protection

### Registration Policy

- **Applied to**: User registration endpoint
- **Limit**: Configurable via `RateLimiting:RegistrationPermitLimit` (default: 10 requests/minute)
- **Purpose**: Prevents spam account creation

## Configuration

Rate limiting settings are configured in `appsettings.json` under the `RateLimiting` section:

```json
{
  "RateLimiting": {
    "GlobalPermitLimit": 100,
    "AuthPermitLimit": 50,
    "ContentPermitLimit": 200,
    "RegistrationPermitLimit": 10
  }
}
```

See [Configuring Rate Limiting](../../getting-started/configuring-rate-limiting) for more details on these settings.

## Behavior

When a rate limit is exceeded:

- The API returns HTTP status code **429 (Too Many Requests)**
- The response body contains the message: "Too many requests. Please try again later."
- The client should wait until the next time window before retrying

## Implementation Details

Rate limiting is implemented using:

- `PartitionedRateLimiter` for per-user/IP limiting
- `FixedWindowRateLimiter` with 1-minute windows
- Automatic partitioning based on user identity or IP address
- Custom rejection handler for consistent error responses

The rate limiter is added to the service collection in `ApplicationServiceExtensions.AddRateLimitingServices()` and applied to controllers using the `[EnableRateLimiting]` attribute.
