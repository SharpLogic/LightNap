import { CommonModule } from "@angular/common";
import { Component, inject, input, output } from "@angular/core";
import { FormBuilder, ReactiveFormsModule, Validators } from "@angular/forms";
import { RouterLink } from "@angular/router";
import { RoleDto, RoutePipe } from "@core";
import { ConfirmPopupComponent } from "@core/components/confirm-popup/confirm-popup.component";
import { RolePickerComponent } from "@core/components/role-picker/role-picker.component";
import { ConfirmationService } from "primeng/api";
import { ButtonModule } from "primeng/button";
import { TableModule } from "primeng/table";

@Component({
  standalone: true,
  selector: "user-roles",
  templateUrl: "./user-roles.component.html",
  imports: [CommonModule, TableModule, ButtonModule, RouterLink, RoutePipe, ConfirmPopupComponent, ReactiveFormsModule, RolePickerComponent],
})
export class UserRolesComponent {
  #confirmationService = inject(ConfirmationService);
  #fb = inject(FormBuilder);

  userRoles = input.required<Array<RoleDto>>();
  addRole = output<string>();
  removeRole = output<string>();

  addUserToRoleForm = this.#fb.group({
    role: this.#fb.control("", [Validators.required]),
  });

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
