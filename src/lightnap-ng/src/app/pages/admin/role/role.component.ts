import { CommonModule } from "@angular/common";
import { Component, inject, input, OnInit, signal } from "@angular/core";
import { FormBuilder, ReactiveFormsModule, Validators } from "@angular/forms";
import { RouterLink } from "@angular/router";
import { ConfirmPopupComponent, PeoplePickerComponent, RoleWithAdminUsers } from "@core";
import { ApiResponseComponent } from "@core/components/controls/api-response/api-response.component";
import { ErrorListComponent } from "@core/components/controls/error-list/error-list.component";
import { UsersService } from "@core/services/users.service";
import { RoutePipe } from "@pages";
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
  readonly #adminService = inject(UsersService);
  readonly #confirmationService = inject(ConfirmationService);
  readonly #fb = inject(FormBuilder);

  readonly role = input.required<string>();

  readonly form = this.#fb.group({
    userId: this.#fb.control<string | null>(null, [Validators.required]),
  });

  errors = signal(new Array<string>());

  roleWithUsers$ = signal(new Observable<RoleWithAdminUsers>());

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
