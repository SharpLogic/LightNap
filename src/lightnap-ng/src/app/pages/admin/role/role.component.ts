import { CommonModule } from "@angular/common";
import { Component, inject, input, OnInit, signal } from "@angular/core";
import { FormBuilder, ReactiveFormsModule, Validators } from "@angular/forms";
import { RouterLink } from "@angular/router";
import { AdminUserDto, AdminUsersService, ApiResponseComponent, ConfirmPopupComponent, ErrorListComponent, PeoplePickerComponent, RoleWithAdminUsers, RoutePipe, TypeHelpers } from "@core";
import { ConfirmationService } from "primeng/api";
import { ButtonModule } from "primeng/button";
import { PanelModule } from "primeng/panel";
import { TableModule } from "primeng/table";
import { Observable } from "rxjs";

@Component({
  standalone: true,
  templateUrl: "./role.component.html",
  imports: [
    CommonModule,
    ReactiveFormsModule,
    PanelModule,
    TableModule,
    ButtonModule,
    RouterLink,
    RoutePipe,
    ErrorListComponent,
    ApiResponseComponent,
    ConfirmPopupComponent,
    PeoplePickerComponent,
  ],
})
export class RoleComponent implements OnInit {
  readonly #adminService = inject(AdminUsersService);
  readonly #confirmationService = inject(ConfirmationService);
  readonly #fb = inject(FormBuilder);

  readonly role = input.required<string>();

  readonly form = this.#fb.group({
    userId: this.#fb.control<string | null>(null, [Validators.required]),
  });

  readonly errors = signal(new Array<string>());

  readonly roleWithUsers$ = signal(new Observable<RoleWithAdminUsers>());

  readonly asRoleWithUsers = TypeHelpers.cast<RoleWithAdminUsers>;
  readonly asAdminUser = TypeHelpers.cast<AdminUserDto>;

  ngOnInit() {
    this.#refreshRole();
  }

  #refreshRole() {
    this.roleWithUsers$.set(this.#adminService.getRoleWithUsers(this.role()));
  }

  addUserToRole() {
    this.#adminService.addUserToRole(this.form.value.userId!, this.role()).subscribe({
      next: () => {
        this.form.reset();
        this.#refreshRole();
      },
      error: response => this.errors.set(response.errorMessages),
    });
  }

  removeUserFromRole(event: any, userId: string) {
    this.errors.set([]);

    this.#confirmationService.confirm({
      header: "Confirm Role Removal",
      message: `Are you sure that you want to remove this role membership?`,
      target: event.target,
      key: userId,
      accept: () => {
        this.#adminService.removeUserFromRole(userId, this.role()).subscribe({
          next: () => this.#refreshRole(),
          error: response => this.errors.set(response.errorMessages),
        });
      },
    });
  }
}
