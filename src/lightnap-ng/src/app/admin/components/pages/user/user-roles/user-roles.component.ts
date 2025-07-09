import { UsersService } from "@core/services/users.service";
import { CommonModule } from "@angular/common";
import { Component, inject, input, output } from "@angular/core";
import { FormBuilder, ReactiveFormsModule, Validators } from "@angular/forms";
import { RouterLink } from "@angular/router";
import { ConfirmPopupComponent, RoleDto } from "@core";
import { ApiResponseComponent } from "@core/components/controls/api-response/api-response.component";
import { RoutePipe } from "@routing";
import { ConfirmationService } from "primeng/api";
import { ButtonModule } from "primeng/button";
import { SelectModule } from "primeng/select";
import { TableModule } from "primeng/table";

@Component({
  standalone: true,
  selector: "user-roles",
  templateUrl: "./user-roles.component.html",
  imports: [
    CommonModule,
    ReactiveFormsModule,
    TableModule,
    ButtonModule,
    RouterLink,
    RoutePipe,
    ApiResponseComponent,
    SelectModule,
    ConfirmPopupComponent,
  ],
})
export class UserRolesComponent {
  #adminService = inject(UsersService);
  #confirmationService = inject(ConfirmationService);
  #fb = inject(FormBuilder);

  userRoles = input.required<Array<RoleDto>>();
  addRole = output<string>();
  removeRole = output<string>();

  addUserToRoleForm = this.#fb.group({
    role: this.#fb.control("", [Validators.required]),
  });

  roles$ = this.#adminService.getRoles();

  removeRoleClicked(event: any, role: string) {
    this.#confirmationService.confirm({
      header: "Confirm Role Removal",
      message: `Are you sure that you want to remove this role membership?`,
      target: event.target,
      key: role,
      accept: () => this.removeRole.emit(role),
    });
  }
}
