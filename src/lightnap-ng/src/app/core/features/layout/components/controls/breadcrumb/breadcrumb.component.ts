import { CommonModule } from "@angular/common";
import { Component, inject } from "@angular/core";
import { toSignal } from "@angular/core/rxjs-interop";
import { RouterLink } from "@angular/router";
import { RouteAliasService } from "@core/features/routing/services/route-alias-service";
import { TypeHelpers } from "@core/helpers";
import { MenuItem } from "primeng/api";
import { Breadcrumb } from "primeng/breadcrumb";
import { BreadcrumbService } from "../../../services/breadcrumb.service";

@Component({
  selector: "ln-breadcrumb",
  templateUrl: "./breadcrumb.component.html",
  imports: [CommonModule, RouterLink, Breadcrumb],
})
export class BreadcrumbComponent {
  readonly #breadcrumbService = inject(BreadcrumbService);
  readonly #routeAlias = inject(RouteAliasService);

  readonly homeItem: MenuItem = { icon: "pi pi-home", routerLink: this.#routeAlias.getRoute("user-home") };
  readonly breadcrumbItems = toSignal(this.#breadcrumbService.breadcrumbs$);

  asMenuItem = TypeHelpers.cast<MenuItem>;
}
