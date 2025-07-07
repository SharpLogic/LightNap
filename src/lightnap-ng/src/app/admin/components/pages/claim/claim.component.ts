import { AdminUserDto } from "@admin/models";
import { AdminService } from "@admin/services/admin.service";
import { CommonModule } from "@angular/common";
import { Component, inject, input, signal } from "@angular/core";
import { FormBuilder, ReactiveFormsModule, Validators } from "@angular/forms";
import { RouterLink } from "@angular/router";
import { ConfirmPopupComponent } from "@core";
import { ApiResponseComponent } from "@core/components/controls/api-response/api-response.component";
import { ErrorListComponent } from "@core/components/controls/error-list/error-list.component";
import { RouteAliasService, RoutePipe } from "@routing";
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
  ],
})
export class ClaimComponent {
  readonly #adminService = inject(AdminService);
  readonly #confirmationService = inject(ConfirmationService);
  readonly #routeAlias = inject(RouteAliasService);

  readonly type = input.required<string>();
  readonly value = input.required<string>();

  readonly #fb = inject(FormBuilder);

  readonly form = this.#fb.group({
    type: this.#fb.control("", [Validators.required]),
    value: this.#fb.control("", [Validators.required]),
  });
  errors = signal(new Array<string>());

  usersForClaim$ = signal(new Observable<Array<AdminUserDto>>());

  ngOnChanges() {
    this.#refreshUsers();
    this.form.setValue({
      type: this.type(),
      value: this.value(),
    });
  }

  #refreshUsers() {
    this.usersForClaim$.set(this.#adminService.getUsersWithClaim({ type: this.type(), value: this.value() }));
  }

  removeUserClaim(event: any, userId: string) {
    this.errors.set([]);

    this.#confirmationService.confirm({
      header: "Confirm User Claim Removal",
      message: `Are you sure that you want to remove this user claim?`,
      target: event.target,
      key: userId,
      accept: () => {
        this.#adminService.removeClaimFromUser(userId, { type: this.type(), value: this.value() }).subscribe({
          next: () => this.#refreshUsers(),
          error: response => this.errors.set(response.errorMessages),
        });
      },
    });
  }

  updateClaim() {
    this.#routeAlias.navigate("admin-claim", [this.form.value.type, this.form.value.value]);
  }
}
