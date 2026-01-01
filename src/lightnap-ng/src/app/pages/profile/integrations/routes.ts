import { AppRoute } from "@core";

export const Routes: AppRoute[] = [
  {
    path: "",
    title: "Profile | Integrations",
    data: { alias: "my-integrations", breadcrumb: "" },
    loadComponent: () => import("./index/index.component").then(m => m.IndexComponent),
  },
  {
    path: "supported",
    title: "Profile | Supported Integrations",
    data: { alias: "supported-integrations", breadcrumb: "Select An Integration" },
    loadComponent: () => import("./supported/supported.component").then(m => m.SupportedComponent),
  },
  {
    path: "connect/:provider",
    title: "Profile | Connect An Integration",
    data: { alias: "connect-integration", breadcrumb: "Connect" },
    loadComponent: () => import("./connect/connect.component").then(m => m.ConnectComponent),
  },
  {
    path: "manage/:integrationId",
    title: "Profile | Manage Integration",
    data: { alias: "manage-integration", breadcrumb: "Manage" },
    loadComponent: () => import("./manage/manage.component").then(m => m.ManageComponent),
  },
];
