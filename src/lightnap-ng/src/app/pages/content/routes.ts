import { AppRoute, RoleName } from "@core";
import { editPageGuard } from "@core/guards/edit-page.guard";
import { permissionsGuard } from "@core/guards/permissions.guard";
import { readPageGuard } from "@core/guards/read-page.guard";
import { AppLayoutComponent } from "@core/layout/components/layouts/app-layout/app-layout.component";
import { PublicLayoutComponent } from "@core/layout/components/layouts/public-layout/public-layout.component";

export const Routes: AppRoute[] = [
  {
    path: "",
    component: AppLayoutComponent,
    children: [
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
        path: "edit/:key/:languageCode",
        data: { alias: "edit-language" },
        canActivate: [editPageGuard],
        title: "Edit Language",
        loadComponent: () => import("./edit-language/edit-language.component").then(m => m.EditLanguageComponent),
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
