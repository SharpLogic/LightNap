import { AppRoute } from "@core";
import { pageGuard } from "@core/guards/page.guard";

export const Routes: AppRoute[] = [
  { path: "", title: "Welcome!", data: { alias: "landing" }, loadComponent: () => import("./index/index.component").then(m => m.IndexComponent) },
  { path: "pages/:key", canActivate: [pageGuard], title: "Page", loadComponent: () => import("./page/page.component").then(m => m.PageComponent) },
  {
    path: "access-denied",
    title: "Access Denied",
    data: { alias: "access-denied" },
    loadComponent: () => import("../support/access-denied/access-denied.component").then(m => m.AccessDeniedComponent),
  },
  {
    path: "error",
    title: "An Error Occurred",
    data: { alias: "error" },
    loadComponent: () => import("../support/error/error.component").then(m => m.ErrorComponent),
  },
  {
    path: "not-found",
    title: "Page Not Found",
    data: { alias: "not-found" },
    loadComponent: () => import("../support/not-found/not-found.component").then(m => m.NotFoundComponent),
  },
  {
    path: "terms-and-conditions",
    title: "Terms & Conditions",
    data: { alias: "terms-and-conditions" },
    loadComponent: () => import("../support/terms-and-conditions/terms-and-conditions.component").then(m => m.TermsAndConditionsComponent),
  },
  {
    path: "privacy-policy",
    title: "Privacy Policy",
    data: { alias: "privacy-policy" },
    loadComponent: () => import("../support/privacy-policy/privacy-policy.component").then(m => m.PrivacyPolicyComponent),
  },
];
