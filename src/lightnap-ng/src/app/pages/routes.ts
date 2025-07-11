import { loggedInGuard } from "@core/guards/logged-in.guard";
import { roleGuard } from "@core/guards/role.guard";
import { AppLayoutComponent } from "@core/layout/components/layouts/app-layout/app-layout.component";
import { PublicLayoutComponent } from "@core/layout/components/layouts/public-layout/public-layout.component";
import { Routes as AdminRoutes } from "./admin/routes";
import { AppRoute } from "./api/models/app-route";
import { Routes as IdentityRoutes } from "./identity/routes";
import { Routes as ProfileRoutes } from "./profile/routes";
import { Routes as PublicRoutes } from "./public/routes";
import { Routes as HomeRoutes } from "./home/routes";

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
    canActivate: [loggedInGuard, roleGuard("Administrator")],
    children: [{ path: "", data: { breadcrumb: "Admin" }, children: AdminRoutes }],
  },
  { path: "identity", data: { breadcrumb: "Identity" }, children: IdentityRoutes },
  { path: "**", redirectTo: "/not-found" },
];
