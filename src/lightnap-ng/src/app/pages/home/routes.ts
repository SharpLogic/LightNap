import { AppRoute } from "@core";

export const Routes: AppRoute[] = [
  {
    path: "",
    title: "User | Home",
    data: { alias: "user-home", breadcrumb: "" },
    loadComponent: () => import("./index/index.component").then(m => m.IndexComponent),
  },
];
