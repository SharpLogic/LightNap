import { AppRoute } from "@core";

export const Routes: AppRoute[] = [
  {
    path: "",
    title: "Admin | Home",
    data: { alias: "admin-home", breadcrumb: "Dashboard" },
    loadComponent: () => import("./index/index.component").then(m => m.IndexComponent),
  },
  {
    path: "users",
    data: { breadcrumb: "Users" },
    children: [
      {
        path: "",
        title: "Admin | Users",
        data: { alias: "admin-users", breadcrumb: "" },
        loadComponent: () => import("./users/users.component").then(m => m.UsersComponent),
      },
      {
        path: "users/:userName",
        title: "Admin | User",
        data: { alias: "admin-user", breadcrumb: "User Details" },
        loadComponent: () => import("./user/user.component").then(m => m.UserComponent),
      },
    ],
  },
  {
    path: "roles",
    data: { breadcrumb: "Roles" },
    children: [
      {
        path: "",
        title: "Admin | Roles",
        data: { alias: "admin-roles", breadcrumb: "" },
        loadComponent: () => import("./roles/roles.component").then(m => m.RolesComponent),
      },
      {
        path: "roles/:role",
        title: "Admin | Role",
        data: { alias: "admin-role", breadcrumb: "Role Details" },
        loadComponent: () => import("./role/role.component").then(m => m.RoleComponent),
      },
    ],
  },
  {
    path: "claims",
    data: { breadcrumb: "Claims" },
    children: [
      {
        path: "",
        title: "Admin | Claims",
        data: { alias: "admin-claims", breadcrumb: "" },
        loadComponent: () => import("./claims/claims.component").then(m => m.ClaimsComponent),
      },
      {
        path: "claims/:type/:value",
        title: "Admin | Claim",
        data: { alias: "admin-claim", breadcrumb: "Claim Details" },
        loadComponent: () => import("./claim/claim.component").then(m => m.ClaimComponent),
      },
    ],
  },
];
