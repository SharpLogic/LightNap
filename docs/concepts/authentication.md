---
title: Authentication & Tokens
layout: home
parent: Concepts
nav_order: 200
---

# {{ page.title }}

LightNap uses JSON Web Tokens (JWTs) to secure access to authenticated APIs. Under most circumstances it shouldn't be necessary for developers to worry about these since all of the functionality is built into LightNap and uses underlying platform functionality where possible to provide a seamless experience. But it's still a good idea for developers to understand what's going on, so this article will provide an overview of how the whole system works from the perspective of the frontend Angular app.

## Getting An Access Token

There are three ways the frontend will attempt retrieve an access token.

### Frontend Loading

When the frontend loads, it requests an access token from the backend. While it doesn't explicitly send any parameters, the browser does implicitly include cookies that were set in previous sessions. When processing the request, the backend will check for an HTTP-only cookie called `refreshToken`.

The backend follows this general workflow when processing token requests:

1. Does the `refreshToken` cookie exist?
2. Is there a `RefreshToken` record for that cookie in the database?
3. Is the `RefreshToken` record unexpired and unrevoked?
4. Is the associated user allowed to log in?

If all of the above are true then the backend returns a freshly-minted access token. It also resets the expiration of the `RefreshToken` record and updates the `refreshToken` cookie in the response.

### User Authentication

If the loading request to retrieve a token fails then the user needs to authenticate. There are multiple `IdentityController` API calls that return an access token:

- When the user registers.
- When the user logs in and their account is not multi-factor.
- When the user submits a multi-factor code after a successful login.
- When the user sets a new password using the link from a "forgot password" email.

{: .note }
If the user does not opt for their device to be remembered, then the refresh token cookie issued by the backend will be set to expire at the end of the current browser session. The next time the browser is opened they will no longer have that cookie available and will need to authenticate.

### Token Refresh

If the `IdentityService` was able to retrieve an initial token it will regularly attempt to refresh it prior to expiration.

## Access Tokens On The Backend

Access tokens are not tracked on the backend. They follow the JSON format and are signed using the configured JWT key, so they (and their claims) are trusted if the signature can be validated. As a result, it's critical to secure this configuration setting since anyone with access to the backend JWT key can generate any tokens they want.

When an authenticated API request is received, the backend automatically validates the token and configures the request context with the associated claims. There is no need to access the token itself since endpoints are secured using the `Authorize` attribute on controllers and/or endpoints. See `ProfileController` for an example of how an entire controller can require users to be logged-in to access whereas `AdministratorController` also requires they be logged in to the `Administrator` role.

It's recommended that API access be secured both at the `LightNap.WebApi` level and within `LightNap.Core` services. Services may be leveraged from other interfaces, so it's a best practice to employ defense in depth.

## Access Tokens On The Frontend

The management of access tokens is built into the frontend platform, so there should never be a need to directly work with them.

### IdentityService

`IdentityService` manages access tokens during the lifetime of the frontend app. It exposes key identity details, such as login status, user ID, email, user name, roles, and claims, via methods and observables that can be used to tailor the user experience as appropriate.

There are several synchronous accessors provided for scenarios where the user is known to be logged in:

- `loggedIn` is `true` if the user is logged in.
- `userId` contains the user's ID.
- `userName` contains the user's user name.
- `email` contains the user's email.
- `roles` contains the list of roles the user belongs to.
- `isUserInRole(role: string)` returns `true` or `false` based on the user's status in the specified role.
- `isUserInAnyRole(roles: Array<string>)` returns `true` if the user is in at least one of the specified roles, otherwise `false`.

When it's unknown whether the user is logged in, such as during frontend loading, asynchronous observables should be used:

- `watchLoggedIn$()` emits `true` or `false` when the login status changes.
- `watchUserRole$(allowedRole: string)` emits `true` or `false` when the user's status in the specified role changes.
- `watchAnyUserRole$(allowedRoles: Array<string>)` emits `true` or `false` when the user's status as being a member of at least one of the specified roles changes.

`IdentityService` also offers synchronous and asynchronous access to custom claims as defined on the backend and then propagated to the frontend via JWT. Depending on the complexity of claims, it may be preferred to wrap claim string construction and management in a `PermissionsService`.

{: .note }
The asynchronous observables will not emit until the initial user access token request has returned. As a result, they can be relied on to determine user status upon the initial load of the frontend. All will emit `false` when the user is determined to not be logged in.

### tokenInterceptor

The frontend is configured with `tokenInterceptor`, which is an HTTP interceptor that automatically inserts an `Authorization` header with the access token. There is no need to worry about this when adding new API requests since they'll be automatically handled.

### Route Guards And Directives

The frontend provides guards and directives to make it easier to tailor the user experience for users based on their authentication/authorization status.

#### Guards

- `loggedInGuard` requires the user be logged in to visit the route.
- `roleGuard` requires the user be logged in as a specific role (like `Administrator`) to visit the route.
- `claimGuard` requires the user be logged in with a specific claim to visit the route.
- `permissionsGuard` requires the user be logged in is a specific role and/or to have a specific claim to visit the route.

#### Directives

- `showLoggedIn` and `hideLoggedIn` will show or hide an element based on the user's logged-in status.
- `showByPermissions` and `showByPermissions` will show or hide an element based on the user's roles and/or claims.

See [working with roles](../common-scenarios/working-with-roles#using-roles-on-the-frontend) or [working with claims](../common-scenarios/custom-claims.md) for more details on using these to evaluate roles or claims.

## Managing Refresh Tokens/Devices

Learn about the relationship between refresh tokens and devices in [this article](./devices).
