import { AppRoute } from "@core";

export const Routes: AppRoute[] = [
  {
    path: "",
    title: "External Logins",
    data: { alias: "external-logins" },
    loadComponent: () => import("./index/index.component").then(m => m.IndexComponent),
  },
  {
    path: "callback",
    title: "External Login Callback",
    data: { alias: "external-login-callback" },
    loadComponent: () => import("./callback/callback.component").then(m => m.CallbackComponent),
  },
  {
    path: "error",
    title: "External Login Error",
    loadComponent: () => import("./error/error.component").then(m => m.ErrorComponent),
  },
  {
    path: "register/:token",
    title: "External Login Registration",
    data: { alias: "external-login-register" },
    loadComponent: () => import("./register/register.component").then(m => m.RegisterComponent),
  },
  {
    path: "complete/:token",
    title: "External Login Completion",
    data: { alias: "external-login-complete" },
    loadComponent: () => import("./complete/complete.component").then(m => m.CompleteComponent),
  },
];
