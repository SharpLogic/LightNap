---
title: Working With Angular Routes
layout: home
parent: Common Scenarios
nav_order: 600
---

# {{ page.title }}

LightNap relies on Angular's built-in routing support with some extensions and abstractions designed to make application routing more convenient and scalable for developers.

## Route Definitions

The root route definition file is at `app/pages/routes.ts`. This file contains the array of top-level routes for the areas of the app, such as `identity`, `profile`, `admin`, etc. Each of those top-level routes then references a child routes file within its area folder, such as `app/pages/identity/routes.ts`. This organization allows the path structure of each area to be kept close to the pages it references and easier to manage.

## Route Aliases

Route aliases are shortcodes that reference exactly one route configuration in the app. The list of supported route aliases is defined in `app/core/routing/models/route-alias.ts`.

{: .note}
Using route aliases is optional. Developers can still hardcode inline routes if preferred.

By using a route alias you can tag an application route, such as:

``` typescript
export const Routes: AppRoute[] = [
  {
    path: "login",
    data: {
      // This alias needs to be defined in route-alias.ts and used only once across all routes.
      alias: "login"
    },
    loadComponent: () => import("./login/login.component").then(m => m.LoginComponent) },
    ...
```

and then reference it in markup using the `RoutePipe` like this:

``` html
<a [routerLink]="'login' | route" class="ml-auto">Return to login</a>
```

or in code using the `RouteAliasService` like this:

``` typescript
this.#routeAlias.navigate("login");
```

Now if the path to the login page ever changes it can be automatically picked up across the whole app without having to change any of the links or navigation calls.

## Route Aliases with Parameters

Routes that expect parameters can be provide them as an argument (for single replacements) or an array of arguments in the order they need to be replaced. For example, this route:

``` typescript
{
  path: "roles/:role",
  data: {
     alias: "admin-role"
  },
  ...
```

can be bound to a link like this:

``` html
<a [routerLink]="'admin-role' | route : role.name"> {{ role.displayName }}</a>
<a [routerLink]="'admin-role' | route : [role.name]"> {{ role.displayName }}</a>
```

or from code:

``` typescript
this.#routeAlias.navigate("admin-role", role);
this.#routeAlias.navigate("admin-role", [role]);
```

## Extending Route Data

Any additional route-specific fields can be easily attached to the `RouteData` class in the same way the `RouteAlias` field is managed. This provides strong typing support for a better developer experience.

## Loading Routes Synchronously vs. Asynchronously

`RouteAliasService` iterates the route tree upon creation to build a map of all route aliases. As a result, any route to be included must be loaded synchronously. To support this, paths that have children with aliases should use `children` and not `loadChildren` all the way down. You can still load the actual components however you want (like `loadComponent`), but any routes that are not in the tree when `RouteAliasService` loads won't be in the map and will throw errors at runtime.
