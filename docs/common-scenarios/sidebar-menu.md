---
title: Updating the Sidebar Menu
layout: home
parent: Common Scenarios
nav_order: 700
---

# {{ page.title }}

The sidebar menu system provides a dynamic, role-based navigation structure that automatically updates based on the current user's authentication status and permissions. The contents of the sidebar menu are managed in the `MenuService` located at `app/core/features/layout/services/menu.service.ts`.

## How It Works

The `MenuService` uses Angular signals to reactively monitor user state and automatically refresh the menu when authentication or role status changes. The service watches three key states:

1. **Login Status** - Whether the user is authenticated
2. **Content Editor Status** - Whether the user has the `Administrator` or `ContentEditor` role
3. **Admin Status** - Whether the user has the `Administrator` role

These states are converted to signals using `toSignal()` and combined using a `computed()` signal that rebuilds the menu tree whenever any state changes.

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
readonly #loggedInMenuItems: MenuItem[] = [
  {
    label: "Profile",
    expanded: true,
    items: [
      { label: "Profile", icon: "pi pi-fw pi-user", routerLink: this.#routeAlias.getRoute("profile"), routerLinkActiveOptions: { exact: true } },
      { label: "Devices", icon: "pi pi-fw pi-mobile", routerLink: this.#routeAlias.getRoute("devices") },
      { label: "Change Password", icon: "pi pi-fw pi-lock", routerLink: this.#routeAlias.getRoute("change-password") },
      // Add your new item here
      { label: "Settings", icon: "pi pi-fw pi-cog", routerLink: this.#routeAlias.getRoute("user-settings") },
    ],
  },
];
```

{: .note}
Menu items use [PrimeNG's MenuItem](https://primeng.org/menu) model. Icons follow PrimeNG's icon conventions (see [PrimeIcons](https://primeng.org/icons) for available options). The `expanded: true` property controls whether the menu section is expanded by default.

### Adding New Menu Sections

To add a completely new menu section (e.g., for a "Moderator" role):

1. **Define the menu items array** as a private readonly field:

    ```typescript
    readonly #moderatorMenuItems: MenuItem[] = [
      {
        label: "Moderation",
        expanded: true,
        items: [
          { label: "Reports", icon: "pi pi-fw pi-flag", routerLink: this.#routeAlias.getRoute("moderator-reports") },
          { label: "Review Queue", icon: "pi pi-fw pi-list", routerLink: this.#routeAlias.getRoute("moderator-queue") },
        ],
      },
    ];
    ```

2. **Add a signal to track the role**:

    ```typescript
    readonly #isModeratorLoggedIn = toSignal(
      this.#identityService.watchUserRole$(RoleNames.Moderator),
      { initialValue: false }
    );
    ```

3. **Include the section in the `menuItems` computed signal**:

    ```typescript
    readonly menuItems = computed(() => {
      const items: MenuItem[] = [];

      // Always include default items
      items.push(...this.#defaultMenuItems);

      // Add role-based items
      if (this.#isLoggedIn()) {
        items.push(...this.#loggedInMenuItems);
      }

      if (this.#isContentEditorLoggedIn()) {
        items.push(...this.#contentMenuItems);
      }

      if (this.#isModeratorLoggedIn()) {
        items.push(...this.#moderatorMenuItems);
      }

      if (this.#isAdminLoggedIn()) {
        items.push(...this.#adminMenuItems);
      }

      return items;
    });
    ```

### Using Route Aliases

Menu items should use the `RouteAliasService` to resolve routes rather than hardcoding paths. This ensures links remain valid even if route paths change:

```typescript
{ label: "My Page", icon: "pi pi-fw pi-home", routerLink: this.#routeAlias.getRoute("my-page-alias") }
```

See [Working With Angular Routes](./using-route-alias) for more information on route aliases.

### Router Link Active Options

For menu items that should only be highlighted when the exact route is active (not child routes), use the `routerLinkActiveOptions` property:

```typescript
{
  label: "Home",
  icon: "pi pi-fw pi-home",
  routerLink: this.#routeAlias.getRoute("user-home"),
  routerLinkActiveOptions: { exact: true }
}
```

Without this option, the menu item will be highlighted when any child route is active.

### Checking Multiple Roles

The `IdentityService` provides two methods for role checking:

- **`watchUserRole$(roleName)`** - Watches for a single specific role
- **`watchAnyUserRole$([role1, role2, ...])`** - Watches for any role in the provided array

For example, to show a menu section to users with either `Administrator` or `ContentEditor` roles:

```typescript
readonly #isContentEditorLoggedIn = toSignal(
  this.#identityService.watchAnyUserRole$([RoleNames.Administrator, RoleNames.ContentEditor]),
  { initialValue: false }
);
```

{: .note }
A similar technique can be used to determine the visibility of menu items based on user claims.

## Advanced Scenarios

### Conditional Menu Items

To show/hide specific items within a section based on additional conditions, you can create separate signals and use them in the computed signal:

```typescript
readonly #userHasPassword = toSignal(
  this.#identityService.watchUserHasPassword$(),
  { initialValue: false }
);

readonly menuItems = computed(() => {
  const items: MenuItem[] = [];

  items.push(...this.#defaultMenuItems);

  if (this.#isLoggedIn()) {
    const profileItems = [
      { label: "Profile", icon: "pi pi-fw pi-user", routerLink: this.#routeAlias.getRoute("profile"), routerLinkActiveOptions: { exact: true } },
      { label: "Devices", icon: "pi pi-fw pi-mobile", routerLink: this.#routeAlias.getRoute("devices") },
    ];

    // Conditionally add password change option
    if (this.#userHasPassword()) {
      profileItems.push({
        label: "Change Password",
        icon: "pi pi-fw pi-lock",
        routerLink: this.#routeAlias.getRoute("change-password")
      });
    }

    items.push({ label: "Profile", expanded: true, items: profileItems });
  }

  // ... rest of menu building logic

  return items;
});
```

### Navigation-Based Menu Items

You can create signals based on router events to show menu sections based on the current route. Inject the `Router` and use `toSignal()` with router events:

```typescript
readonly #currentRoute = toSignal(
  this.#router.events.pipe(
    filter(event => event instanceof NavigationEnd),
    map(() => this.#router.url)
  ),
  { initialValue: this.#router.url }
);

readonly menuItems = computed(() => {
  const items: MenuItem[] = [];

  // Use this.#currentRoute() to conditionally include menu sections

  return items;
});
```

## Menu Component Structure

The menu is rendered using PrimeNG's `PanelMenu` component in the `AppSidebarComponent`. The component simply binds to the `menuItems` signal from the `MenuService`:

```typescript
@Component({
  selector: "ln-app-sidebar",
  templateUrl: "./app-sidebar.component.html",
  imports: [PanelMenuModule],
})
export class AppSidebarComponent {
  readonly #menuService = inject(MenuService);
  readonly menuItems = this.#menuService.menuItems;
}
```

The template uses the PrimeNG `p-panelMenu` component with the `[multiple]="true"` option to allow multiple sections to be expanded simultaneously:

```html
<div class="layout-sidebar menu-container">
  <p-panelMenu [model]="menuItems()" [multiple]="true" />
</div>
```

## See Also

- [Working With Angular Routes](./using-route-alias) - Learn about route aliases
- [Working with Roles](./working-with-roles) - Understand the role system