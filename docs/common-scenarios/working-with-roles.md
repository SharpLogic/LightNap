---
title: Working With Roles
layout: home
parent: Common Scenarios
nav_order: 300
---

# {{ page.title }}

LightNap uses ASP.NET Roles to manage application authorization. By default there is only one role, `Administrator`. However, you can extend the application to introduce new roles to support a variety of authorization needs.

## Adding a Role

Roles are defined in `Configuration/ApplicationRoles.cs` in the `LightNap.Core` project. To add a new role, simply use the existing `Administrator` role as a reference example. Roles use the custom `ApplicationRole` instance that can be extended with additional properties, if required.

New roles must also be added to the `ApplicationRoles.All` collection so that they can be managed on startup. The app automatically references this collection to add any roles that don't yet exist and delete those that don't exist anymore. It's also the list that's returned to the front-end for managing user membership in roles.

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

Learn more about role-based authorization in ASP.NET [here](https://learn.microsoft.com/aspnet/core/security/authorization/roles).

## Roles and JSON Web Tokens (JWTs)

Roles are automatically embedded in access tokens by the back-end to authorize future requests. As a result, tokens issued before a role assignment has changed will not reflect the latest role behavior until they are replaced. The maximum theoretical time for this is based on how long the access token is [configured to expire in the application settings](../getting-started/configuring-jwt). Users can speed this up by logging out and in again.

## Using Roles on the Front-End

Roles are automatically extracted from JWTs on the front-end, so there's no additional work required to get them.

### Accessing the Logged-In User's Roles

The best way to access their roles is via `IdentityService.watchLoggedInToRole$()` or `IdentityService.watchLoggedInToAnyRole$()`. These return hot observables that publish every time the user's roles have been updated from a new JWT. They're backed by `ReplaySubject`, so the response will be immediate if the roles have been set at least once.

### Applying the Logged-In User's Roles

The main benefit to using roles on the front-end is to show components available to a user belonging to a given role. There are some good patterns in place that can be referenced when applying behavior for new roles.

#### Route Guards

Route guards make it easy to determine whether a user can see a given route based on their role. A reference example for this is at `app/core/guards/admin.guard.ts`. This guard watches for changes in roles and only allows the guarded section to be visible if the user is logged in as an `Administrator`.

To protect a route, add the guard to its `canActivate` collection. A reference example for this is in `routing\routes.ts` where the `admin` branch of the route tree is protected by the `adminGuard`.

{: .important }
Front-end work to restrict access to functionality is superficial. While it provides a nicer user experience to show or hide components, the key security considerations must be taken care of on the back-end. Never rely on front-end security for anything meaningful because insecure back-end APIs can be easily exploited.

#### Role Directives

Route directives make it easy to show or hide elements based on the logged-in user's role membership.

The `showByRoles` directive only shows the element if the user is logged in and belongs to at least one of the roles listed.

``` html
<p-button showByRoles roles="Administrator" ...
```

or

``` html
<p-button showByRoles [roles]="['Administrator', 'Moderator']" ...
```

Similarly, the `hideByRoles` directive only hides the element if the user is logged in and belongs to at least one of the roles listed.

``` html
<p-button hideByRoles roles="Administrator" ...
```

or

``` html
<p-button hideByRoles [roles]="['Administrator', 'Moderator']" ...
```

#### Menus

You can also [change the sidebar menu](./sidebar-menu) based on the roles of the logged-in user.

### Managing User Roles

Most administrative tasks should be covered by the built-in administrative functionality. By default, any `Administrator` can add and remove users from the roles registered on the back-end. Changes to roles are picked up automatically, so running the application with the changes above will now show the new `Moderator` role.
