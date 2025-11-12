---
title: Configuring Breadcrumbs
layout: home
parent: Common Scenarios
av_order: 750
---

# {{ page.title }}

This guide shows you how to configure breadcrumb navigation for your routes. Breadcrumbs help users understand their location within the application and provide quick navigation back to parent pages.

- TOC
{:toc}

## Prerequisites

Before configuring breadcrumbs, you should understand:

- [Breadcrumb Navigation concepts](../concepts/breadcrumb-navigation)
- [Working with Angular Routes](./using-route-alias)

## Basic Configuration

### Adding a Static Breadcrumb

The simplest way to add a breadcrumb is with a static string in the route's `data` property:

```typescript
import { AppRoute } from '@core';

export const Routes: AppRoute[] = [
  {
    path: 'settings',
    title: 'Settings',
    data: { 
      alias: 'settings',
      breadcrumb: 'Settings'  // Static breadcrumb label
    },
    loadComponent: () => import('./settings/settings.component').then(m => m.SettingsComponent)
  }
];
```

This creates a breadcrumb that displays "Settings" when the user navigates to this route.

### Hiding Breadcrumbs

To prevent a route from adding a breadcrumb, use an empty string:

```typescript
{
  path: '',
  data: { 
    alias: 'home',
    breadcrumb: ''  // No breadcrumb for this route
  },
  loadComponent: () => import('./home/home.component').then(m => m.HomeComponent)
}
```

This is commonly used for:
- Index routes that would create duplicate breadcrumbs
- Wrapper routes that only provide structure
- Routes where the parent breadcrumb is sufficient

## Dynamic Breadcrumbs

### Using Route Parameters

For routes with parameters, use a function to generate dynamic labels:

```typescript
{
  path: 'products/:productId',
  data: {
    breadcrumb: (route) => {
      const productId = route.params['productId'];
      return `Product ${productId}`;
    }
  },
  loadComponent: () => import('./product/product.component').then(m => m.ProductComponent)
}
```

The function receives an `ActivatedRouteSnapshot` with access to:
- `route.params` - Route parameters
- `route.queryParams` - Query parameters
- `route.data` - Route data

### Providing Fallbacks

Always provide fallback text in case parameters are missing:

```typescript
{
  path: 'users/:userName',
  data: {
    breadcrumb: (route) => route.params['userName'] || 'User Details'
  },
  loadComponent: () => import('./user/user.component').then(m => m.UserComponent)
}
```

### Complex Parameter Formatting

You can format parameters for better display:

```typescript
{
  path: 'claims/:type/:value',
  data: {
    breadcrumb: (route) => {
      const type = route.params['type'];
      const value = route.params['value'];
      return type && value ? `${type}: ${value}` : 'Claim Details';
    }
  },
  loadComponent: () => import('./claim/claim.component').then(m => m.ClaimComponent)
}
```

## Hierarchical Breadcrumbs

### Creating Multi-Level Navigation

Breadcrumbs automatically reflect route hierarchy. Each level can contribute to the trail:

```typescript
export const Routes: AppRoute[] = [
  {
    path: 'admin',
    data: { breadcrumb: 'Admin' },
    children: [
      {
        path: 'users',
        data: { breadcrumb: 'Users' },
        children: [
          {
            path: '',
            data: { breadcrumb: '' },  // Don't add extra breadcrumb
            loadComponent: () => import('./users-list.component').then(m => m.UsersListComponent)
          },
          {
            path: ':userName',
            data: { breadcrumb: (route) => route.params['userName'] },
            loadComponent: () => import('./user-detail.component').then(m => m.UserDetailComponent)
          }
        ]
      }
    ]
  }
];
```

This creates breadcrumb trails like:
- `/admin/users` → **Home > Admin > Users**
- `/admin/users/john.doe` → **Home > Admin > Users > john.doe**

### Grouping Related Routes

Use parent routes to group related pages under a common breadcrumb:

```typescript
{
  path: 'content',
  data: { breadcrumb: 'Content' },
  children: [
    {
      path: '',
      data: { breadcrumb: '' },
      loadComponent: () => import('./manage.component').then(m => m.ManageComponent)
    },
    {
      path: 'edit/:key',
      data: { breadcrumb: (route) => route.params['key'] },
      children: [
        {
          path: '',
          data: { breadcrumb: '' },
          loadComponent: () => import('./edit.component').then(m => m.EditComponent)
        },
        {
          path: ':languageCode',
          data: { breadcrumb: (route) => route.params['languageCode'] },
          loadComponent: () => import('./edit-language.component').then(m => m.EditLanguageComponent)
        }
      ]
    }
  ]
}
```

## Advanced Scenarios

### Loading Data from Resolvers

For breadcrumbs that need data from an API, use a resolver:

```typescript
import { inject } from '@angular/core';
import { ResolveFn } from '@angular/router';
import { ProductService } from './product.service';
import { Product } from './product.model';

const productResolver: ResolveFn<Product> = (route) => {
  const productService = inject(ProductService);
  return productService.getProduct(route.params['id']);
};

export const Routes: AppRoute[] = [
  {
    path: 'products/:id',
    resolve: { product: productResolver },
    data: {
      breadcrumb: (route) => {
        const product = route.data['product'];
        return product?.name || 'Product Details';
      }
    },
    loadComponent: () => import('./product.component').then(m => m.ProductComponent)
  }
];
```

The resolver fetches data before route activation, making it available to the breadcrumb function.

### Conditional Breadcrumbs

Show different breadcrumbs based on route state:

```typescript
{
  path: 'edit/:id',
  data: {
    breadcrumb: (route) => {
      const id = route.params['id'];
      return id === 'new' ? 'Create New' : 'Edit';
    }
  },
  loadComponent: () => import('./edit.component').then(m => m.EditComponent)
}
```

### Using Query Parameters

Access query parameters in breadcrumb functions:

```typescript
{
  path: 'search',
  data: {
    breadcrumb: (route) => {
      const query = route.queryParams['q'];
      return query ? `Search: ${query}` : 'Search';
    }
  },
  loadComponent: () => import('./search.component').then(m => m.SearchComponent)
}
```

## Common Patterns

### Profile Section

The profile section demonstrates a typical pattern:

```typescript
export const Routes: AppRoute[] = [
  {
    path: '',
    data: { breadcrumb: '' },  // Profile home doesn't add breadcrumb
    loadComponent: () => import('./index.component').then(m => m.IndexComponent)
  },
  {
    path: 'devices',
    data: { breadcrumb: 'Devices' },
    loadComponent: () => import('./devices.component').then(m => m.DevicesComponent)
  },
  {
    path: 'notifications',
    data: { breadcrumb: 'Notifications' },
    loadComponent: () => import('./notifications.component').then(m => m.NotificationsComponent)
  }
];
```

### Admin Section

The admin section shows hierarchical breadcrumbs with dynamic labels:

```typescript
export const Routes: AppRoute[] = [
  {
    path: 'users',
    data: { breadcrumb: 'Users' },
    children: [
      {
        path: '',
        data: { breadcrumb: '' },
        loadComponent: () => import('./users.component').then(m => m.UsersComponent)
      },
      {
        path: ':userName',
        data: { breadcrumb: (route) => route.params['userName'] || 'User Details' },
        loadComponent: () => import('./user.component').then(m => m.UserComponent)
      }
    ]
  }
];
```

## Best Practices

1. **Keep Labels Short** - Breadcrumb labels should be concise and descriptive
2. **Use Empty Strings for Index Routes** - Prevent duplicate breadcrumbs on index pages
3. **Always Provide Fallbacks** - Include fallback text in dynamic breadcrumb functions
4. **Group Related Routes** - Use parent routes to create logical hierarchies
5. **Use Resolvers for API Data** - Don't make API calls in breadcrumb functions
6. **Test on Mobile** - Ensure breadcrumb trails aren't too long for mobile screens
7. **Be Consistent** - Use similar patterns across your application

## Troubleshooting

### Breadcrumb Not Appearing

- Verify the route has a `breadcrumb` property in its `data`
- Check that the breadcrumb value is not an empty string
- Ensure parent routes also have breadcrumb configuration

### Wrong Breadcrumb Label

- Check the breadcrumb function is returning the expected value
- Verify route parameters are being accessed correctly
- Ensure resolvers are completing before the breadcrumb is generated

### Duplicate Breadcrumbs

- Use empty strings (`breadcrumb: ''`) for index routes
- Review the route hierarchy for unnecessary nesting
- Check that parent and child routes aren't both adding the same label