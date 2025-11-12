---
title: Breadcrumb Navigation
layout: home
parent: Concepts
nav_order: 350
---

# {{ page.title }}

LightNap includes a breadcrumb navigation system that automatically generates navigation breadcrumbs based on the current route. The breadcrumb trail helps users understand their location within the application hierarchy and provides quick navigation back to parent pages.

- TOC
{:toc}

## Overview

The breadcrumb system consists of three main components:

1. **BreadcrumbService** - Monitors route changes and builds breadcrumb trails from route data
2. **BreadcrumbComponent** - Renders the breadcrumb UI using PrimeNG's Breadcrumb component
3. **Route Configuration** - Routes define their breadcrumb labels through route data

## How It Works

The breadcrumb system automatically generates breadcrumbs by:

1. Listening to Angular router navigation events
2. Walking through the activated route tree
3. Collecting breadcrumb data from each route's `data.breadcrumb` property
4. Building a hierarchical trail of navigation items
5. Rendering the breadcrumbs with proper links and styling

### BreadcrumbService

The `BreadcrumbService` (located at `app/core/features/layout/services/breadcrumb.service.ts`) provides an observable stream of breadcrumb items:

```typescript
readonly breadcrumbs$ = this.#router.events.pipe(
  filter(event => event instanceof NavigationEnd),
  map(() => this.#createBreadcrumbs(this.#router.routerState.snapshot.root))
);
```

The service recursively walks the route tree, collecting breadcrumb labels from route data and building the full navigation path.

### BreadcrumbComponent

The `BreadcrumbComponent` (located at `app/core/features/layout/components/controls/breadcrumb/breadcrumb.component.ts`) subscribes to the breadcrumb stream and renders the navigation trail:

- Displays a home icon that links to the user home page
- Renders each breadcrumb item with optional icons
- Makes intermediate items clickable links
- Displays the current page (last item) as plain text without a link

### Route Configuration

Routes define their breadcrumb labels through the `breadcrumb` property in route data:

```typescript
{
  path: 'profile',
  data: { breadcrumb: 'Profile' },
  children: [...]
}
```

## Breadcrumb Labels

### Static Labels

The simplest approach is to use a static string:

```typescript
{
  path: 'devices',
  data: { breadcrumb: 'Devices' },
  loadComponent: () => import('./devices/devices.component').then(m => m.DevicesComponent)
}
```

### Dynamic Labels

For routes with parameters, use a function that receives the route snapshot:

```typescript
{
  path: 'users/:userName',
  data: {
    breadcrumb: (route) => route.params['userName'] || 'User Details'
  },
  loadComponent: () => import('./user/user.component').then(m => m.UserComponent)
}
```

The function receives an `ActivatedRouteSnapshot` and can access:
- `route.params` - Route parameters
- `route.queryParams` - Query parameters
- `route.data` - Route data
- Any other route snapshot properties

### Empty Labels

Use an empty string to skip adding a breadcrumb for a route:

```typescript
{
  path: '',
  data: { breadcrumb: '' },  // No breadcrumb for this route
  loadComponent: () => import('./index/index.component').then(m => m.IndexComponent)
}
```

This is useful for:
- Index routes that shouldn't add an extra breadcrumb
- Wrapper routes that only provide structure
- Routes where the parent's breadcrumb is sufficient

## Hierarchical Structure

Breadcrumbs automatically reflect the route hierarchy. For nested routes, each level can contribute to the breadcrumb trail:

```typescript
{
  path: 'admin',
  data: { breadcrumb: 'Admin' },
  children: [
    {
      path: 'users',
      data: { breadcrumb: 'Users' },
      children: [
        {
          path: ':userName',
          data: { breadcrumb: (route) => route.params['userName'] }
        }
      ]
    }
  ]
}
```

This creates a breadcrumb trail like: **Home > Admin > Users > john.doe**

## Advanced Scenarios

### Loading Data from APIs

For complex scenarios where breadcrumb labels need to be loaded from an API, consider using an Angular resolver:

```typescript
const userResolver: ResolveFn<User> = (route) => {
  const userService = inject(UserService);
  return userService.getUser(route.params['id']);
};

export const Routes: AppRoute[] = [
  {
    path: 'users/:id',
    resolve: { user: userResolver },
    data: {
      breadcrumb: (route) => route.data['user']?.displayName || 'User'
    },
    loadComponent: () => import('./user/user.component').then(m => m.UserComponent)
  }
];
```

The resolver fetches the data before route activation, making it available to the breadcrumb function.

### Conditional Breadcrumbs

You can conditionally show or hide breadcrumbs based on route data:

```typescript
{
  path: 'edit/:id',
  data: {
    breadcrumb: (route) => {
      const isNew = route.params['id'] === 'new';
      return isNew ? 'Create New' : 'Edit';
    }
  }
}
```

## Styling and Customization

The breadcrumb component uses PrimeNG's Breadcrumb component with custom styling:

- Uses a forward slash (`/`) as the separator
- Displays icons when provided in menu items
- Applies responsive max-width styling
- Integrates with the application's theme

To customize the appearance, modify the component template at `app/core/features/layout/components/controls/breadcrumb/breadcrumb.component.html`.

## Best Practices

1. **Keep Labels Concise** - Breadcrumb labels should be short and descriptive
2. **Use Empty Strings for Index Routes** - Avoid duplicate breadcrumbs for index pages
3. **Provide Fallbacks** - Always provide fallback text in dynamic breadcrumb functions
4. **Consider Mobile** - Keep breadcrumb trails short enough to display on mobile devices
5. **Use Resolvers for API Data** - Don't make API calls directly in breadcrumb functions

## Integration with Layout

The breadcrumb component is integrated into the `AppLayoutComponent` and appears above the main content area:

```html
<div class="layout-main-container">
  <div class="container">
    <ln-breadcrumb />
    <router-outlet />
  </div>
</div>
```

It automatically updates whenever the route changes, providing consistent navigation context throughout the application.