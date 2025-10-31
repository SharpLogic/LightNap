import { AppRoute, RoleName } from "@core";
import { editPageGuard } from "@core/guards/edit-page.guard";
import { permissionsGuard } from "@core/guards/permissions.guard";
import { readPageGuard } from "@core/guards/read-page.guard";

export const Routes: AppRoute[] = [
  { path: "", canActivate: [permissionsGuard([RoleName.Administrator, RoleName.ContentEditor], [])], title: "Manage Content", loadComponent: () => import("./manage/manage.component").then(m => m.ManageComponent) },
  { path: "edit/:key", canActivate: [editPageGuard], title: "Edit Content", loadComponent: () => import("./edit/edit.component").then(m => m.EditComponent) },
  { path: ":key", canActivate: [readPageGuard], title: "Content Page", loadComponent: () => import("./page/page.component").then(m => m.PageComponent) },
];
