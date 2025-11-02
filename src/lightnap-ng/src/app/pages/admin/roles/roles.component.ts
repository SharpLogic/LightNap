import { CommonModule } from "@angular/common";
import { Component, inject } from "@angular/core";
import { RouterLink } from "@angular/router";
import { RoleDto, RoutePipe, TypeHelpers } from "@core";
import { ApiResponseComponent } from "@core/components/api-response/api-response.component";
import { AdminUsersService } from "@core/users/services/admin-users.service";
import { PanelModule } from "primeng/panel";
import { TableModule } from "primeng/table";

@Component({
  standalone: true,
  templateUrl: "./roles.component.html",
  imports: [CommonModule, PanelModule, RouterLink, RoutePipe, ApiResponseComponent, TableModule],
})
export class RolesComponent {
  readonly #adminService = inject(AdminUsersService);

  readonly roles$ = this.#adminService.getRoles();

  readonly asRoles = TypeHelpers.cast<Array<RoleDto>>;
  readonly asRole = TypeHelpers.cast<RoleDto>;
}
