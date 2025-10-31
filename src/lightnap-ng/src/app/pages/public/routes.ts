import { AppRoute } from "@core";

export const Routes: AppRoute[] = [
  { path: "", title: "Welcome!", data: { alias: "landing" }, loadComponent: () => import("./index/index.component").then(m => m.IndexComponent) },
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
];
