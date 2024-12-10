---
title: Authentication & Tokens
layout: home
parent: Concepts
nav_order: 200
---

# {{ page.title }}

LightNap uses JSON Web Tokens (JWTs) to secure access to authenticated APIs. Under most circumstances it shouldn't be necessary for developers to worry about these since all of the functionality is built into LightNap and uses underlying platform functionality where possible to provide a seamless experience. But it's still a good idea for developers to understand what's going on, so this article will provide an overview of how the whole system works from the perspective of the front-end Angular app.

## Getting An Access Token

There are three ways the front-end will attempt retrieve an access token.

### Front-End Loading

When the front-end loads, it requests an access token from the back-end. While it doesn't explicitly send any parameters, the browser does implicitly include cookies that were set in previous sessions. When processing the request, the back-end will check for an HTTP-only cookie called `refreshToken`.

The back-end follows this general workflow when processing token requests:

1. Does the `refreshToken` cookie exist?
2. Is there a `RefreshToken` record for that cookie in the database?
3. Is the `RefreshToken` record unexpired and unrevoked?
4. Is the associated user allowed to log in?

If all of the above are true then the back-end returns a freshly-minted access token. It also resets the expiration of the `RefreshToken` record and updates the `refreshToken` cookie in the response.

### User Authentication

If the loading request to retrieve a token fails then the user needs to authenticate. There are multiple `IdentityController` API calls that return an access token:

- When the user registers.
- When the user logs in and their account is not multi-factor.
- When the user submits a multi-factor code after a successful login.
- When the user sets a new password using the link from a "forgot password" email.

{: .note }
If the user does not opt for their device to be remembered, then the refresh token cookie issued by the back-end will be set to expire at the end of the current browser session. The next time the browser is opened they will no longer have that cookie available and will need to authenticate.

### Token Refresh

If the `IdentityService` was able to retrieve an initial token it will regularly attempt to refresh it prior to expiration.

## Access Tokens On The Back-End

Access tokens are not tracked on the back-end. They follow the JSON format and are signed using the configured JWT key, so they (and their claims) are trusted if the signature can be validated. As a result, it's critical to secure this configuration setting since anyone with access to the back-end JWT key can generate any tokens they want.

When an authenticated API request is received, the back-end automatically validates the token and configures the request context with the associated claims. There is no need to access the token itself since endpoints are secured using the `Authorize` attribute on controllers and/or endpoints. See `ProfileController` for an example of how an entire controller can require users to be logged-in to access whereas `AdministratorController` also requires they be logged in to the `Administrator` role via the `RequireAdministratorRole` policy.

It's recommended that API access be secured at the `LightNap.WebApi` level in most scenarios since the `LightNap.Core` services will trust that incoming requests have been validated for generic authentication and authorization. However, it's still necessary to validate business rules within the core services, such as not allowing an `Administrator` to demote themselves.

## Access Tokens On The Front-End

The management of access tokens is built into the front-end platform, so there should never be a need to directly work with them.

### IdentityService

`IdentityService` manages access tokens during the lifetime of the front-end app. It exposes key identity details, such as login status, user ID, email, user name, and roles, via methods and observables that can be used to tailor the user experience as appropriate.

There are several synchronous accessors provided for scenarios where the user is known to be logged in:

- `loggedIn` is `true` if the user is logged in.
- `userId` contains the user's ID.
- `userName` contains the user's user name.
- `email` contains the user's email.
- `roles` contains the list of roles the user belongs to.
- `isUserInRole(role: string)` returns `true` or `false` based on the user's status in the specified role.
- `isUserInAnyRole(roles: Array<string>)` returns `true` if the user is in at least one of the specified roles, otherwise `false`.

When it's unknown whether the user is logged in, such as during front-end loading, asynchronous observables should be used:

- `watchLoggedIn$()` emits `true` or `false` when the login status changes.
- `watchLoggedInToRole$(allowedRole: string)` emits `true` or `false` when the user's status in the specified role changes.
- `watchLoggedInToAnyRole$(allowedRoles: Array<string>)` emits `true` or `false` when the user's status as being a member of at least one of the specified roles changes.

{: .note }
The asynchronous observables will not emit until the initial user access token request has returned. As a result, they can be relied on to determine user status upon the initial load of the front-end. All will emit `false` when the user is determined to not be logged in.

### tokenInterceptor

The front-end is configured with `tokenInterceptor`, which is an HTTP interceptor that automatically inserts an `Authorization` header with the access token. There is no need to worry about this when adding new API requests since they'll be automatically handled.

### Route Guards And Directives

The front-end provides guards and directives to make it easier to tailor the user experience for users based on their authentication/authorization status.

#### Guards

- `authGuard` requires the user be logged in to visit the route.
- `adminGuard` requires the user be logged in as an `Administrator` to visit the route.

#### Directives

- `showLoggedIn` and `hideLoggedIn` will show or hide an element based on the user's logged-in status.
- `showByRoles` and `hideByRoles` will show or hide an element based on the user's roles.

See [this article](../common-scenarios/working-with-roles#using-roles-on-the-front-end) for more details on using these to evaluate roles.

## Managing Refresh Tokens/Devices

Learn about the relationship between refresh tokens and devices in [this article](./devices).