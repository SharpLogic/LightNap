import { AppRoute, RoleNames } from "@core";
import { AppLayoutComponent } from "@core/features/layout/components/layouts/app-layout/app-layout.component";
import { PublicLayoutComponent } from "@core/features/layout/components/layouts/public-layout/public-layout.component";
import { editPageGuard } from "@core/guards/edit-page.guard";
import { permissionsGuard } from "@core/guards/permissions.guard";
import { readPageGuard } from "@core/guards/read-page.guard";

export const Routes: AppRoute[] = [
  {
    path: "",
    component: AppLayoutComponent,
    data: { breadcrumb: "Content" },
    children: [
      {
        path: "",
        canActivate: [permissionsGuard([RoleNames.Administrator, RoleNames.ContentEditor], [])],
        data: { alias: "manage-content", breadcrumb: "" },
        title: "Manage Content",
        loadComponent: () => import("./manage/manage.component").then(m => m.ManageComponent),
      },
      {
        path: "edit",
        children: [
          {
            path: ":key",
            data: { alias: "edit-content", breadcrumb: "Edit" },
            canActivate: [editPageGuard],
            title: "Edit Content",
            loadComponent: () => import("./edit/edit.component").then(m => m.EditComponent),
          },
          {
            path: "edit/:key/:languageCode",
            data: { alias: "edit-language", breadcrumb: "Edit Language" },
            canActivate: [editPageGuard],
            title: "Edit Language",
            loadComponent: () => import("./edit-language/edit-language.component").then(m => m.EditLanguageComponent),
          },
        ],
      },
    ],
  },
  {
    path: "",
    component: PublicLayoutComponent,
    children: [
      {
        path: ":key",
        canActivate: [readPageGuard],
        data: { alias: "view-content" },
        title: "View Content",
        loadComponent: () => import("./page/page.component").then(m => m.PageComponent),
      },
    ],
  },
];
