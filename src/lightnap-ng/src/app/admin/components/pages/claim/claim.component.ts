import { AdminService } from "@admin/services/admin.service";
import { CommonModule } from "@angular/common";
import { Component, inject, input, OnInit, signal } from "@angular/core";
import { RouterLink } from "@angular/router";
import { ConfirmPopupComponent } from "@core";
import { ApiResponseComponent } from "@core/components/controls/api-response/api-response.component";
import { ErrorListComponent } from "@core/components/controls/error-list/error-list.component";
import { RoutePipe } from "@routing";
import { ConfirmationService } from "primeng/api";
import { ButtonModule } from "primeng/button";
import { PanelModule } from 'primeng/panel';
import { TableModule } from "primeng/table";
import { Observable, tap } from "rxjs";
import { AdminUser, RoleWithAdminUsers } from "@admin/models";

@Component({
  standalone: true,
  templateUrl: "./claim.component.html",
  imports: [
    CommonModule,
    PanelModule,
    TableModule,
    ButtonModule,
    RouterLink,
    RoutePipe,
    ErrorListComponent,
    ApiResponseComponent,
    ConfirmPopupComponent,
  ],
})
export class ClaimComponent implements OnInit {
  readonly #adminService = inject(AdminService);
  readonly #confirmationService = inject(ConfirmationService);

  readonly type = input.required<string>();
  readonly value = input.required<string>();

  errors = signal(new Array<string>());

  usersForClaim$ = signal(new Observable<Array<AdminUser>>());

  ngOnInit() {
    this.#refreshUsers();
  }

  #refreshUsers() {
    this.usersForClaim$.set(this.#adminService.getUsersForClaim({ type: this.type(), value: this.value() }));
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
}
