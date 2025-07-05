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
import { RoleWithAdminUsers } from "@admin/models";

@Component({
  standalone: true,
  templateUrl: "./role.component.html",
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
export class RoleComponent implements OnInit {
  readonly #adminService = inject(AdminService);
  readonly #confirmationService = inject(ConfirmationService);

  readonly role = input.required<string>();

  errors = signal(new Array<string>());

  roleWithUsers$ = signal(new Observable<RoleWithAdminUsers>());

  ngOnInit() {
    this.#refreshRole();
  }

  #refreshRole() {
    this.roleWithUsers$.set(this.#adminService.getRoleWithUsers(this.role()));
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
