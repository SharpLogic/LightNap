---
title: Configuring JSON Web Tokens (JWT)
layout: home
parent: Getting Started
nav_order: 600
---

# {{ page.title }}

Some JSON Web Token (JWT) parameters need to be [configured in `appsettings.json`](./application-configuration) or your deployment host. It will work out of the box as configured, but it's critical that the `Key` value be changed for at least production environments.

- `Jwt.Key`: The secret key used for JWT token generation. This can be any 32+ character string, such as a randomly generated GUID. It should be different across different environments (development vs. production and so on).

{: .important }
`Jwt.Key` must be at least 32 characters. The backend enforces this and will throw an error on startup if not met (see `src/LightNap.Core/Identity/Services/TokenService.cs`).

- `Jwt.Issuer`: The issuer of the JWT token. For typical scenarios this can be the site URL, such as `https://www.yourdomain.com`.
- `Jwt.Audience`: The audience of the JWT token. For typical scenarios this can be the site URL, such as `https://www.yourdomain.com`.
- `Jwt.ExpirationMinutes`: The expiration time of the JWT token in minutes. By default this is `120` minutes.
{: .note }

{: .important }
`Jwt:ExpirationMinutes` must be a valid integer of at least `5`. The backend validates this setting during startup and will fail with a clear configuration error if it is invalid.

## Frontend token refresh behavior

The front-end periodically checks the token lifetime and automatically requests a new access token shortly before expiration. Implementation details:

- The Angular `IdentityService` checks token expiry every **60 seconds**.
- The frontend will request a refresh when the token is set to expire in less than **5 minutes**.

This is controlled by `IdentityService.TokenRefreshCheckMillis` and `IdentityService.TokenExpirationWindowMillis` in `src/lightnap-ng/src/app/core/services/identity.service.ts`.

Recommended pattern: use short-lived access tokens (e.g., 10–30 minutes) combined with a refresh token for long-lived sessions. This keeps access tokens secure while allowing continuous user sessions.

{: .note }
If you set the access token expiration to 5 minutes on the backend, then the frontend will request a new token every minute since each loop check will detect that the token expires in less than 5 minutes.
