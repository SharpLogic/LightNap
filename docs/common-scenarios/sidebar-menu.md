---
title: Updating the Sidebar Menu
layout: home
parent: Common Scenarios
nav_order: 700
---

# {{ page.title }}

The sidebar menu system provides a dynamic, role-based navigation structure that automatically updates based on the current user's authentication status and permissions. The contents of the sidebar menu are managed in the `MenuService` located at `app/core/features/layout/services/menu.service.ts`.

## How It Works

The `MenuService` uses reactive programming with RxJS to monitor user state and automatically refresh the menu when authentication or role status changes. The service watches three key states:

1. **Login Status** - Whether the user is authenticated
2. **Content Editor Status** - Whether the user has the `Administrator` or `ContentEditor` role
3. **Admin Status** - Whether the user has the `Administrator` role

These states are combined using `combineLatest` with a 100ms debounce to efficiently handle multiple simultaneous state changes. When any state changes, the `#refreshMenuItems` method rebuilds the menu tree.

## Default Menu Structure

Out of the box, LightNap provides four menu sections:

### Home Section

Always visible to all users (including anonymous users):

- **Home** - Links to the user home page

{: .note}
While logged-in users typically see the sidebar menu, the Home section is included for all users to illustrate how to provide menu items for anonymous users if you add publicly accessible pages that show the sidebar menu.

### Profile Section

Visible to logged-in users:

- **Profile** - User profile management
- **Devices** - Manage registered devices
- **Change Password** - Password change interface

### Content Section

Visible to users with `Administrator` or `ContentEditor` roles:

- **Manage** - Content management interface

### Admin Section

Visible only to users with the `Administrator` role:

- **Home** - Admin dashboard
- **Users** - User management
- **Roles** - Role management
- **Claims** - Claims management

## Adding Menu Items

### Adding Items to Existing Sections

To add a new menu item to an existing section, update the appropriate menu items array in `menu.service.ts`:

```typescript
#loggedInMenuItems = new Array<MenuItem>({
  label: "Profile",
  items: [
    { label: "Profile", icon: "pi pi-fw pi-user", routerLink: this.#routeAlias.getRoute("profile") },
    { label: "Devices", icon: "pi pi-fw pi-mobile", routerLink: this.#routeAlias.getRoute("devices") },
    { label: "Change Password", icon: "pi pi-fw pi-lock", routerLink: this.#routeAlias.getRoute("change-password") },
    // Add your new item here
    { label: "Settings", icon: "pi pi-fw pi-cog", routerLink: this.#routeAlias.getRoute("user-settings") },
  ],
});
```

{: .note}
Menu items use [PrimeNG's MenuItem](https://primeng.org/menu) model. Icons follow PrimeNG's icon conventions (see [PrimeIcons](https://primeng.org/icons) for available options).

### Adding New Menu Sections

To add a completely new menu section (e.g., for a "Moderator" role):

1. **Define the menu items array** as a private field:

    ```typescript
    #moderatorMenuItems = new Array<MenuItem>({
      label: "Moderation",
      items: [
        { label: "Reports", icon: "pi pi-fw pi-flag", routerLink: this.#routeAlias.getRoute("moderator-reports") },
        { label: "Review Queue", icon: "pi pi-fw pi-list", routerLink: this.#routeAlias.getRoute("moderator-queue") },
      ],
    });
    ```

2. **Add a state tracking field**:

    ```typescript
    #isModeratorLoggedIn = false;
    ```

3. **Watch the role in the constructor**:

    ```typescript
    combineLatest([
      this.#identityService.watchLoggedIn$().pipe(tap(isLoggedIn => (this.#isLoggedIn = isLoggedIn))),
      this.#identityService
        .watchAnyUserRole$([RoleNames.Administrator, RoleNames.ContentEditor])
        .pipe(tap(isContentEditorLoggedIn => (this.#isContentEditorLoggedIn = isContentEditorLoggedIn))),
      this.#identityService.watchUserRole$(RoleNames.Administrator).pipe(tap(isAdminLoggedIn => (this.#isAdminLoggedIn = isAdminLoggedIn))),
      // Add your new role watch
      this.#identityService.watchUserRole$(RoleNames.Moderator).pipe(tap(isModeratorLoggedIn => (this.#isModeratorLoggedIn = isModeratorLoggedIn))),
    ])
      .pipe(takeUntilDestroyed(), debounceTime(100))
      .subscribe({ next: () => this.#refreshMenuItems() });
    ```

4. **Include the section in `#refreshMenuItems`**:

    ```typescript
    #refreshMenuItems() {
      var menuItems = [...this.#defaultMenuItems];

      if (this.#isLoggedIn) {
        menuItems.push(...this.#loggedInMenuItems);
      }

      if (this.#isContentEditorLoggedIn) {
        menuItems.push(...this.#contentMenuItems);
      }

      if (this.#isModeratorLoggedIn) {
        menuItems.push(...this.#moderatorMenuItems);
      }

      if (this.#isAdminLoggedIn) {
        menuItems.push(...this.#adminMenuItems);
      }

      this.#menuItemSubject.next(menuItems);
    }
    ```

### Using Route Aliases

Menu items should use the `RouteAliasService` to resolve routes rather than hardcoding paths. This ensures links remain valid even if route paths change:

```typescript
{ label: "My Page", icon: "pi pi-fw pi-home", routerLink: this.#routeAlias.getRoute("my-page-alias") }
```

See [Working With Angular Routes](./using-route-alias) for more information on route aliases.

### Checking Multiple Roles

The `IdentityService` provides two methods for role checking:

- **`watchUserRole$(roleName)`** - Watches for a single specific role
- **`watchAnyUserRole$([role1, role2, ...])`** - Watches for any role in the provided array

For example, to show a menu section to users with either `Administrator` or `ContentEditor` roles:

```typescript
this.#identityService
  .watchAnyUserRole$([RoleNames.Administrator, RoleNames.ContentEditor])
  .pipe(tap(hasRole => (this.#hasContentRole = hasRole)))
```

## Advanced Scenarios

### Conditional Menu Items

To show/hide specific items within a section based on additional conditions, you can dynamically build the items array in `#refreshMenuItems`:

```typescript
#refreshMenuItems() {
  var menuItems = [...this.#defaultMenuItems];

  if (this.#isLoggedIn) {
    const profileItems = [
      { label: "Profile", icon: "pi pi-fw pi-user", routerLink: this.#routeAlias.getRoute("profile") },
      { label: "Devices", icon: "pi pi-fw pi-mobile", routerLink: this.#routeAlias.getRoute("devices") },
    ];

    // Conditionally add password change option
    if (this.#userHasPassword) {
      profileItems.push({ label: "Change Password", icon: "pi pi-fw pi-lock", routerLink: this.#routeAlias.getRoute("change-password") });
    }

    menuItems.push({ label: "Profile", items: profileItems });
  }

  // ... rest of menu building logic
}
```

### Navigation-Based Menu Items

You can subscribe to router events to show menu sections based on the current route. Inject the `Router` and watch for navigation events in the constructor:

```typescript
this.#router.events
  .pipe(
    filter(event => event instanceof NavigationEnd),
    takeUntilDestroyed()
  )
  .subscribe(() => {
    // Check current route and update menu accordingly
  });
```

## See Also

- [Working With Angular Routes](./using-route-alias) - Learn about route aliases
- [Working with Roles](./working-with-roles) - Understand the role system
