import { AppRoute, RoleName } from "@core";
import { editPageGuard } from "@core/guards/edit-page.guard";
import { permissionsGuard } from "@core/guards/permissions.guard";
import { readPageGuard } from "@core/guards/read-page.guard";

export const Routes: AppRoute[] = [
  {
    path: "",
    canActivate: [permissionsGuard([RoleName.Administrator, RoleName.ContentEditor], [])],
    data: { alias: "manage-content" },
    title: "Manage Content",
    loadComponent: () => import("./manage/manage.component").then(m => m.ManageComponent),
  },
  {
    path: "edit/:key",
    data: { alias: "edit-content" },
    canActivate: [editPageGuard],
    title: "Edit Content",
    loadComponent: () => import("./edit/edit.component").then(m => m.EditComponent),
  },
  {
    path: ":key",
    canActivate: [readPageGuard],
    data: { alias: "view-content" },
    title: "View Content",
    loadComponent: () => import("./page/page.component").then(m => m.PageComponent),
  },
];
