---
title: Working With Roles
layout: home
parent: Security & Authorization
nav_order: 10
---

# {{ page.title }}

LightNap uses ASP.NET Roles to manage application authorization. By default there is only one role, `Administrator`. However, you can extend the application to introduce new roles to support a variety of authorization needs.

## Adding a Role

Roles are defined in `Configuration/ApplicationRoles.cs` in the `LightNap.Core` project. To add a new role, simply use the existing `Administrator` role as a reference example. Roles use the custom `ApplicationRole` instance that can be extended with additional properties, if required.

New roles must also be added to the `ApplicationRoles.All` collection so that they can be managed on startup. The app automatically references this collection to add any roles that don't yet exist and delete those that don't exist anymore. It's also the list that's returned to the frontend for managing user membership in roles.

Here's an example of how a new `Moderator` role can be added:

``` csharp
public static class ApplicationRoles
{
    public static readonly ApplicationRole Administrator = new(Constants.Roles.Administrator, "Administrator", "Access to all administrative features");
    public static readonly ApplicationRole Moderator = new("Moderator", "Moderator", "Moderates content");

    public static IReadOnlyList<ApplicationRole> All =>
    [
        Administrator,
        Moderator
    ];
}
```

{: .note}
The "Moderator" name is defined in line here for simplicity, but you may prefer to add it to `Constants.Roles` as a constant string in `Configuration/Constants.cs` in the `LightNap.Core` project.

## Applying a Role Authorization

Suppose you wanted to update the application so that `Moderator` users were able to hide comments via a hypothetical `ChatController.HideComment()` endpoint. You can do this by specifying the authorized roles via the `Authorize` attribute.

``` csharp
public class ChatController(IChatController chatService) : ControllerBase
{
  ...
  [HttpPost("chat/{commentId}/hide")]
  [Authorize(Roles = "Moderator")]
  public async Task<ApiResponseDto<bool>> HideComment(string commentId)
  {
    ...
```

That's it. The role has been added and the REST endpoint now allows `Moderator` access.

### Allowing Any Of A List Of Roles

Sometimes you'll want to allow users who are a member of _any_ specified role to access an endpoint. This can be done by providing a list of roles in a single `Authorize` attribute, such as:

```csharp
[Authorize(Roles = "Administrator,Moderator")]
```

This will allow any user who is an `Administrator` or `Moderator` (or both) to access the endpoint.

### Requiring All Of A List Of Roles

If your endpoint needs a user to be a member of _all_ roles, add each as its own attribute, such as:

``` csharp
[Authorize(Roles = "Administrator")]
[Authorize(Roles = "Moderator")]
```

This will require a user to be both an `Administrator` and a `Moderator` to access the endpoint.

Learn more about [role-based authorization in ASP.NET](https://learn.microsoft.com/aspnet/core/security/authorization/roles).

## Seeding Role Users

Learn about [seeding users in a role](../getting-started/seeding-users).

## Roles and JSON Web Tokens (JWTs)

Roles are automatically embedded in access tokens by the backend to authorize future requests. As a result, tokens issued before a role assignment has changed will not reflect the latest role behavior until they are replaced. The maximum theoretical time for this is based on how long the access token is [configured to expire in the application settings](../getting-started/configuring-jwt). Users can speed this up by logging out and in again.

## Using Roles on the Frontend

Roles are automatically extracted from JWTs on the frontend, so there's no additional work required to get them.

### Accessing the Logged-In User's Roles

The best way to access their roles is via `IdentityService.watchUserRole$()` or `IdentityService.watchAnyUserRole$()`. These return hot observables that publish every time the user's roles have been updated from a new JWT. They're backed by `ReplaySubject`, so the response will be immediate if the roles have been set at least once.

### Applying the Logged-In User's Roles

The main benefit to using roles on the frontend is to show components available to a user belonging to a given role. There are some good patterns in place that can be referenced when applying behavior for new roles.

#### Route Guards

Route guards make it easy to determine whether a user can see a given route based on their role. A reference example for this is at `app/core/guards/role.guard.ts`. This guard watches for changes in roles and only allows the guarded section to be visible if the user is logged in as one of the specified roles.

To protect a route, add the guard to its `canActivate` collection. A reference example for this is in `routing\routes.ts` where the `admin` branch of the route tree is protected by the `roleGuard`.

{: .important }
Frontend work to restrict access to functionality is superficial. While it provides a nicer user experience to show or hide components, the key security considerations must be taken care of on the backend. Never rely on frontend security for anything meaningful because insecure backend APIs can be easily exploited.

#### Role Directives

Route directives make it easy to show or hide elements based on the logged-in user's role membership.

The `showByPermissions` directive only shows the element if the user is logged in and belongs to at least one of the roles listed.

``` html
<p-button showByPermissions roles="Administrator" ...
```

or

``` html
<p-button showByPermissions [roles]="['Administrator', 'Moderator']" ...
```

Similarly, the `hideByPermissions` directive only hides the element if the user is logged in and belongs to at least one of the roles listed.

``` html
<p-button hideByPermissions roles="Administrator" ...
```

or

``` html
<p-button hideByPermissions [roles]="['Administrator', 'Moderator']" ...
```

#### Menus

You can also [change the sidebar menu](./sidebar-menu) based on the roles of the logged-in user.

### Managing User Roles

Most administrative tasks should be covered by the built-in administrative functionality. By default, any `Administrator` can add and remove users from the roles registered on the backend. Changes to roles are picked up automatically, so running the application with the changes above will now show the new `Moderator` role.
