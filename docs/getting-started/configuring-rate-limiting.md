---
title: Configuring Rate Limiting
layout: home
parent: Application Configuration
nav_order: 350
---

# {{ page.title }}

Rate limiting is a critical security feature that protects your API from abuse by limiting the number of requests a user or IP address can make within a given time window. LightNap implements rate limiting using ASP.NET Core's built-in rate limiting middleware with fixed window limiters.

## How Rate Limiting Works

Rate limiting in LightNap uses the following approach:

- **Partitioning**: Requests are partitioned by authenticated user ID (if available) or client IP address. This ensures that legitimate users aren't penalized by shared IP addresses, while unauthenticated requests are limited by IP.
- **Fixed Window**: Uses a fixed time window of 1 minute for all limits.
- **Policies**: Different endpoints have different rate limits based on their sensitivity and expected usage patterns.

## Rate Limiting Policies

LightNap defines several rate limiting policies, each applied to different types of endpoints:

### Global Policy

- **Applied to**: All API endpoints
- **Limit**: Configurable via `RateLimiting:GlobalPermitLimit` (default: 100 requests/minute)
- **Purpose**: Provides baseline protection against excessive requests from any source

### Auth Policy

- **Applied to**: Authentication endpoints (login, token refresh, password reset, etc.)
- **Limit**: Configurable via `RateLimiting:AuthPermitLimit` (default: 50 requests/minute)
- **Purpose**: Protects against brute force attacks on authentication systems

### Content Policy

- **Applied to**: Content management endpoints
- **Limit**: Configurable via `RateLimiting:ContentPermitLimit` (default: 200 requests/minute)
- **Purpose**: Allows higher limits for content-heavy operations while still providing protection

### Registration Policy
- **Applied to**: User registration endpoint
- **Limit**: Configurable via `RateLimiting:RegistrationPermitLimit` (default: 10 requests/minute)
- **Purpose**: Prevents spam account creation with stricter limits

## Configuration Settings

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

| Setting                          | Purpose                                                                                                                                                                                                                                                                                           |
| -------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `RateLimiting:GlobalPermitLimit` | The global rate limit in requests per minute per user/IP. Applied to all endpoints. Default: 100.                                                                                                                                                                                                |
| `RateLimiting:AuthPermitLimit`   | The rate limit for authentication endpoints (login, refresh, etc.) in requests per minute per user/IP. Default: 50.                                                                                                                                       |
| `RateLimiting:ContentPermitLimit`| The rate limit for content endpoints in requests per minute per user/IP. Default: 200.                                                                                                                                                                                                           |
| `RateLimiting:RegistrationPermitLimit` | The rate limit for the registration endpoint in requests per minute per user/IP. Default: 10.                                                                                                                                                                                                    |

## Behavior When Limits Are Exceeded

When a rate limit is exceeded:

- The API returns HTTP status code **429 (Too Many Requests)**
- The response body contains the message: "Too many requests. Please try again later."
- The client should wait until the next time window before retrying

{: .important }
Rate limits are enforced per partition (user or IP). Authenticated users get individual limits, while unauthenticated traffic is limited by IP address.

## Frontend Considerations

The Angular frontend includes built-in handling for rate limiting errors:

- **Error Display**: 429 responses are displayed to users with appropriate messaging
- **Automatic Handling**: The frontend's HTTP error interceptor catches 429 responses
- **User Feedback**: Clear error messages guide users on what to do

Rate limiting primarily affects API calls, but consider the user experience when setting limits, especially for content-heavy applications.

## Implementation Details

Rate limiting is implemented using:

- `PartitionedRateLimiter` for per-user/IP limiting
- `FixedWindowRateLimiter` with 1-minute windows
- Automatic partitioning based on user identity or IP address
- Custom rejection handler for consistent error responses

The rate limiter is added to the service collection in `ApplicationServiceExtensions.AddRateLimitingServices()` and applied to controllers using the `[EnableRateLimiting]` attribute.

## Best Practices

### Setting Appropriate Limits

- **Start Conservative**: Begin with lower limits and increase based on usage patterns
- **Monitor Usage**: Use application logs to understand normal traffic patterns
- **Consider User Experience**: Balance security with usability
- **Account for Burst Traffic**: Some features may legitimately need higher limits

### Environment-Specific Configuration

```json
// Development - More permissive for testing
{
  "RateLimiting": {
    "GlobalPermitLimit": 500,
    "AuthPermitLimit": 200,
    "ContentPermitLimit": 1000,
    "RegistrationPermitLimit": 50
  }
}

// Production - Stricter limits for security
{
  "RateLimiting": {
    "GlobalPermitLimit": 100,
    "AuthPermitLimit": 30,
    "ContentPermitLimit": 200,
    "RegistrationPermitLimit": 5
  }
}
```

### Monitoring and Alerts

- **Log Rate Limit Hits**: Monitor for unusual patterns that might indicate attacks
- **Set Up Alerts**: Configure alerts for sustained high rates of 429 responses
- **Review Limits Regularly**: Adjust limits based on legitimate usage growth

## Troubleshooting

### Common Issues

**Users Getting Limited Unexpectedly**
- Check if multiple users share the same IP (corporate networks, NAT)
- Verify user authentication status (authenticated users get individual limits)
- Review recent activity that might have triggered limits

**429 Errors in Development**
- Development environments may have lower limits than expected
- Check if multiple developers are testing simultaneously
- Consider increasing limits for development environments

**Inconsistent Limiting**
- Verify that all instances in a distributed setup use the same Redis instance
- Check that rate limiting middleware is properly configured
- Ensure time synchronization across servers

### Testing Rate Limits

To test rate limiting behavior:

1. Use tools like `curl` or Postman to make rapid requests
2. Monitor response codes for 429 status
3. Check application logs for rate limiting events
4. Verify that limits reset after the time window

### Performance Considerations

- Rate limiting adds minimal overhead but monitor for impact on high-traffic applications
- Consider using Redis-backed rate limiting for distributed deployments
- The in-memory limiter is suitable for single-instance deployments

## Security Considerations

Rate limiting is one layer of your security strategy:

- **Not a Complete Solution**: Combine with other security measures
- **Monitor for Bypass Attempts**: Attackers may try to circumvent limits
- **Regular Review**: Periodically assess and update limits based on threats
- **Logging**: Log rate limit violations for security analysis

## Related Topics

- [Application Configuration](../index) - Overview of all configuration options
- [Configuring Application Settings](./configuring-application-settings) - Other core application settings
- [Security & Authorization](../../development-guide/security-authorization) - Authentication and authorization features
 
 