---
title: Working With Custom Claims
layout: home
parent: Common Scenarios
nav_order: 400
---

# {{ page.title }}

LightNap uses [ASP.NET Claims](https://learn.microsoft.com/en-us/aspnet/core/security/authorization/claims) for custom authorization claims. While [roles](./working-with-roles.md) are recommended for global permission sets, custom claims are ideal for precise permissions, such as edit access to a record.

## Adding a Claim

There are no custom claims in the system by default, but developers are encouraged to use the claims system for fine-grained permissions, if needed. There is no structured definition of claims in ASP.NET. They are simply key-value pairs associated with users and/or roles. It's up to the developer to create a meaningful convention for defining and applying claims throughout the system.

Adding a claim to a user can be done using the `UserManager` class as shown below. In this scenario, the `Owner` claim type could be used to indicate that the specified user has a special set of permissions when it comes to operating on record with ID `1`.

```csharp
await this._userManager.AddClaimAsync(user, new Claim("Owner", "1"));
```

However, this is just one possible scenario. There are a variety of other conventions that could be applied based on the specific needs of a given application.

```csharp
await this._userManager.AddClaimAsync(user, new Claim("Product", "Owner:1"));
await this._userManager.AddClaimAsync(user, new Claim("Product:1", "Owner"));
await this._userManager.AddClaimAsync(user, new Claim("Product:SharpLogic:Software", "Owner"));
```

The selection of a type/value convention should take into account any future needs to query the system for records, such as "all owners of the product with ID 1".

## Enforcing Claims

ASP.NET provides great built-in support for composing security policies from combinations of roles and/or claims that covers most static scenarios where there is a known type or type/value combination used to restrict access to resources. For example, if there is a type `Owner`, then it's very easy to define a policy that requires the user to have a claim for `Owner` to access an endpoint.

``` csharp
options.AddPolicy("OwnersOnly", policy => policy.RequireClaim("Owner"));
```

At that point, an endpoint could be secured with an `Authorize` attribute.

``` csharp
[Authorize(Policy = "OwnersOnly")]
```

Unfortunately, things get more complicated when it comes to applying claims to secure endpoints with dynamic parameters, such as those from the path or query.

## Using the ClaimAuthorize Attribute

To make this scenario easier, LightNap introduces the `ClaimAuthorize` attribute. It extends the `Authorize` attribute to provide a way to enforce a claim using dynamic parameters. For example, the endpoint below is restricted to users who have the claim `Owner` with the value matching the `id` parameter from the path.

``` csharp
[ClaimAuthorize("Owner", "{id}")]
[HttpPost("{id}")]
public ApiResponseDto<ProductDto> UpdateProduct(string id, UpdateProductDto dto)
```

The two required parameters passed to `ClaimAuthorize` are templates for the type and value strings, respectively. This allows for flexibility using the same template processing as ASP.NET route templates, and enables the straightforward implementations below.

``` csharp
[ClaimAuthorize("Product", "Owner:{id}")]
[HttpPost("{id}")]
public ApiResponseDto<ProductDto> UpdateProduct(string id, UpdateProductDto dto)

[ClaimAuthorize("Product:{id}", "Owner")]
[HttpPost("{id}")]
public ApiResponseDto<ProductDto> UpdateProduct(string id, UpdateProductDto dto)

[ClaimAuthorize("{vendor}:{type}:{id}", "Owner")]
[HttpPost("{vendor}/{type}/{id}")]
public ApiResponseDto<ProductDto> UpdateProduct(string vendor, string type, string id, UpdateProductDto dto)
```

### Using the OverrideRoles and Roles Parameters

It's very common for endpoints to support role overrides. For example, it may be preferred that `Administrator` users have access to just about everything regardless of their specific claims. To support this, an optional third parameter offers a way to provide a role name (or comma-separated list of role names) to check for before evaluating specific claims. If the user is a member of any of those roles, then the check succeeds.

``` csharp
[ClaimAuthorize("Owner", "{id}", "Administrator,Auditor")]
[HttpPost("{id}")]
public ApiResponseDto<ProductDto> UpdateProduct(string id, UpdateProductDto dto)
```

This functionality may also be accessed via the `OverrideRoles` named parameter.

``` csharp
[ClaimAuthorize("Owner", "{id}", OverrideRoles = "Administrator,Auditor")]
[HttpPost("{id}")]
public ApiResponseDto<ProductDto> UpdateProduct(string id, UpdateProductDto dto)
```

However, the `Roles` named parameter requires membership in at least one of the specified roles as well as the claim verification to succeed. In the check below, the user must have the required claim and be a `Administrator`, `Auditor`, or both.

``` csharp
[ClaimAuthorize("Owner", "{id}", Roles = "Administrator,Auditor")]
[HttpPost("{id}")]
public ApiResponseDto<ProductDto> UpdateProduct(string id, UpdateProductDto dto)
```

## Checking Claims From Services

To check for roles and/or claims from the core services, use an `IUserContext` service.

``` csharp
  bool IsInRole(string role);
  bool HasClaim(string claimType, string claimValue);
```

## Checking Claims on the Frontend

`IdentityService` provides synchronous and asynchronous methods for checking or tracking claims.

Synchronous methods should only be called after the user's logged-in status has been determined since claims are delivered in the JWT retrieved from the backend.

``` typescript
doesUserHaveClaim(allowedClaim: Claim);
doesUserHaveAnyClaim(allowedClaims: Array<Claim>);
```

Asynchronous methods can be used at any time and will only emit once the logged-in status has been determined.

``` typescript
watchUserClaim$(allowedClaim: Claim);
watchAnyUserClaim$(allowedClaims: Array<Claim>);
```

There is also a synchronous service to determine whether the user meets the requirement of any of the specified claims or roles.

``` typescript
doesUserHavePermission(allowedRoles: Array<string>, allowedClaims: Array<Claim>);
```

As well as the asynchronous version.

``` typescript
watchUserPermission$(allowedRoles: Array<string>, allowedClaims: Array<Claim>)
```

All of the above return either a `boolean` (synchronous) or `Observable<boolean>` (asynchronous). They succeed if the user meets any of the claims or roles.

To implement a check for multiple claims and/or role memberships, calls must be nested or otherwise combined.

These methods can also be used with route guards like `roleGuard`, `claimGuard`, or `permissionsGuard` to restrict access to views based on roles and/or claims.

## Claims Directives

The `showByPermissions` and `hideByPermissions` directives provide an easy way to show or hide elements based on the user's roles and/or claims. Each take an array of `roles` or `claims` that call `IdentityService.watchUserPermission$()` to determine whether to show or hide an element.

``` html
    <!-- Shown if the user has the Owner type claim for value 1 -->
    <div showByPermissions [claims]="{ type: 'Owner', value: 'view:1' }">
      ...
    </div>

    <!-- Shown if the user has the Owner type claim for value 1 OR is an Administrator or Auditor or both. -->
    <div showByPermissions [claims]="{ type: 'Owner', value: 'view:1' }" [roles]="Administrator,Auditor">
      ...
    </div>
```
