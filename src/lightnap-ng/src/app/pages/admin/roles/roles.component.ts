import { UsersService } from "@core/services/users.service";
import { CommonModule } from "@angular/common";
import { Component, inject } from "@angular/core";
import { RouterLink } from "@angular/router";
import { ApiResponseComponent } from "@core/components/controls/api-response/api-response.component";
import { RoutePipe } from "@pages";
import { PanelModule } from 'primeng/panel';
import { TableModule } from "primeng/table";

@Component({
  standalone: true,
  templateUrl: "./roles.component.html",
  imports: [CommonModule, PanelModule, RouterLink, RoutePipe, ApiResponseComponent, TableModule],
})
export class RolesComponent {
  readonly #adminService = inject(UsersService);

  readonly roles$ = this.#adminService.getRoles();
}
