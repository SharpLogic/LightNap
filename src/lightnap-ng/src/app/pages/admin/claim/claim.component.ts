import { CommonModule } from "@angular/common";
import { Component, inject, input, signal } from "@angular/core";
import { FormBuilder, ReactiveFormsModule, Validators } from "@angular/forms";
import { RouterLink } from "@angular/router";
import { AdminUserDto, AdminUsersService, ConfirmPopupComponent, PeoplePickerComponent, RoutePipe } from "@core";
import { ApiResponseComponent } from "@core/components/api-response/api-response.component";
import { ErrorListComponent } from "@core/components/error-list/error-list.component";
import { ConfirmationService } from "primeng/api";
import { ButtonModule } from "primeng/button";
import { InputTextModule } from "primeng/inputtext";
import { PanelModule } from "primeng/panel";
import { TableModule } from "primeng/table";
import { Observable } from "rxjs";

@Component({
  standalone: true,
  templateUrl: "./claim.component.html",
  imports: [
    CommonModule,
    ReactiveFormsModule,
    PanelModule,
    TableModule,
    InputTextModule,
    ButtonModule,
    RouterLink,
    RoutePipe,
    ErrorListComponent,
    ApiResponseComponent,
    ConfirmPopupComponent,
    PeoplePickerComponent,
  ],
})
export class ClaimComponent {
  readonly #adminService = inject(AdminUsersService);
  readonly #confirmationService = inject(ConfirmationService);

  readonly type = input.required<string>();
  readonly value = input.required<string>();

  readonly #fb = inject(FormBuilder);

  readonly form = this.#fb.group({
    userId: this.#fb.control<string | null>(null, [Validators.required]),
  });

  errors = signal(new Array<string>());

  usersForClaim$ = signal(new Observable<Array<AdminUserDto>>());

  ngOnChanges() {
    this.#refreshUsers();
  }

  #refreshUsers() {
    this.usersForClaim$.set(this.#adminService.getUsersWithClaim({ type: this.type(), value: this.value() }));
  }

  addUserClaim() {
    this.errors.set([]);

    this.#adminService.addUserClaim(this.form.value.userId!, { type: this.type(), value: this.value() }).subscribe({
      next: () => {
        this.form.reset();
        this.#refreshUsers();
      },
      error: response => this.errors.set(response.errorMessages),
    });
  }

  removeUserClaim(event: any, userId: string) {
    this.errors.set([]);

    this.#confirmationService.confirm({
      header: "Confirm User Claim Removal",
      message: `Are you sure that you want to remove this user claim?`,
      target: event.target,
      key: userId,
      accept: () => {
        this.#adminService.removeUserClaim(userId, { type: this.type(), value: this.value() }).subscribe({
          next: () => this.#refreshUsers(),
          error: response => this.errors.set(response.errorMessages),
        });
      },
    });
  }
}
