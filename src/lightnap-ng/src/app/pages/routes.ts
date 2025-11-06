import { AppRoute, RoleNames } from "@core";
import { AppLayoutComponent } from "@core/features/layout/components/layouts/app-layout/app-layout.component";
import { PublicLayoutComponent } from "@core/features/layout/components/layouts/public-layout/public-layout.component";
import { loggedInGuard } from "@core/guards/logged-in.guard";
import { roleGuard } from "@core/guards/role.guard";
import { Routes as AdminRoutes } from "./admin/routes";
import { Routes as ContentRoutes } from "./content/routes";
import { Routes as HomeRoutes } from "./home/routes";
import { Routes as IdentityRoutes } from "./identity/routes";
import { Routes as ProfileRoutes } from "./profile/routes";
import { Routes as PublicRoutes } from "./public/routes";

export const Routes: AppRoute[] = [
  { path: "", component: PublicLayoutComponent, children: PublicRoutes },
  {
    path: "",
    component: AppLayoutComponent,
    canActivate: [loggedInGuard],
    children: [
      { path: "home", data: { breadcrumb: "Home" }, children: HomeRoutes },
      { path: "profile", data: { breadcrumb: "Profile" }, children: ProfileRoutes },
    ],
  },
  {
    path: "admin",
    component: AppLayoutComponent,
    canActivate: [loggedInGuard, roleGuard(RoleNames.Administrator)],
    children: [{ path: "", data: { breadcrumb: "Admin" }, children: AdminRoutes }],
  },
  { path: "content", data: { breadcrumb: "Content" }, children: ContentRoutes },
  { path: "identity", component: PublicLayoutComponent, data: { breadcrumb: "Identity" }, children: IdentityRoutes },
  { path: "**", redirectTo: "/not-found" },
];
