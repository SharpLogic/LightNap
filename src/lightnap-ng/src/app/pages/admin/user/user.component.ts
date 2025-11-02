import { CommonModule } from "@angular/common";
import { Component, inject, input, OnChanges, signal } from "@angular/core";
import { ReactiveFormsModule } from "@angular/forms";
import { AdminUserDto, AdminUsersService, RoleDto, RouteAliasService, setApiErrors, TypeHelpers } from "@core";
import { ApiResponseComponent } from "@core/components/api-response/api-response.component";
import { ConfirmPopupComponent } from "@core/components/confirm-popup/confirm-popup.component";
import { ErrorListComponent } from "@core/components/error-list/error-list.component";
import { ToastService } from "@core/services/toast.service";
import { ConfirmationService } from "primeng/api";
import { ButtonModule } from "primeng/button";
import { PanelModule } from "primeng/panel";
import { TabsModule } from "primeng/tabs";
import { TagModule } from "primeng/tag";
import { Observable, tap } from "rxjs";
import { UserClaimsComponent } from "./user-claims/user-claims.component";
import { UserProfileComponent } from "./user-profile/user-profile.component";
import { UserRolesComponent } from "./user-roles/user-roles.component";

@Component({
  standalone: true,
  templateUrl: "./user.component.html",
  imports: [
    CommonModule,
    ReactiveFormsModule,
    PanelModule,
    ButtonModule,
    ErrorListComponent,
    ApiResponseComponent,
    ConfirmPopupComponent,
    TagModule,
    TabsModule,
    UserClaimsComponent,
    UserRolesComponent,
    UserProfileComponent,
  ],
})
export class UserComponent implements OnChanges {
  readonly adminService = inject(AdminUsersService);
  readonly #confirmationService = inject(ConfirmationService);
  readonly #toast = inject(ToastService);
  readonly #routeAlias = inject(RouteAliasService);

  readonly userName = input.required<string>();

  readonly errors = signal(new Array<string>());

  readonly user$ = signal<Observable<AdminUserDto>>(new Observable<AdminUserDto>());
  readonly userRoles$ = signal<Observable<Array<RoleDto>>>(new Observable<Array<RoleDto>>());

  #userId = "";

  readonly asUser = TypeHelpers.cast<AdminUserDto>;
  readonly asUserRoles = TypeHelpers.cast<Array<RoleDto>>;

  ngOnChanges() {
    this.#refreshUser();
  }

  #refreshUser() {
    this.user$.set(
      this.adminService.getUserByUserName(this.userName()).pipe(
        tap(user => {
          if (!user) return;

          this.#userId = user.id;
          this.#refreshRoles();
        })
      )
    );
  }

  #refreshRoles() {
    this.userRoles$.set(this.adminService.getUserRoles(this.#userId));
  }

  lockUserAccount(event: any) {
    this.errors.set([]);

    this.#confirmationService.confirm({
      header: "Confirm Lock Account",
      message: `Are you sure that you want to lock this user account?`,
      target: event.target,
      key: "lock",
      accept: () => {
        this.adminService.lockUserAccount(this.#userId).subscribe({
          next: () => this.#refreshUser(),
          error: setApiErrors(this.errors),
        });
      },
    });
  }

  unlockUserAccount(event: any) {
    this.errors.set([]);

    this.#confirmationService.confirm({
      header: "Confirm Unlock Account",
      message: `Are you sure that you want to unlock this user account?`,
      target: event.target,
      key: "unlock",
      accept: () => {
        this.adminService.unlockUserAccount(this.#userId).subscribe({
          next: () => this.#refreshUser(),
          error: setApiErrors(this.errors),
        });
      },
    });
  }

  deleteUser(event: any) {
    this.errors.set([]);

    this.#confirmationService.confirm({
      header: "Confirm Delete User",
      message: `Are you sure that you want to delete this user?`,
      target: event.target,
      key: "delete",
      accept: () => {
        this.adminService.deleteUser(this.#userId).subscribe({
          next: () => {
            this.#toast.success("User deleted successfully.");
            this.#routeAlias.navigate("admin-users");
          },
          error: setApiErrors(this.errors),
        });
      },
    });
  }

  removeRole(role: string) {
    this.errors.set([]);

    this.adminService.removeUserFromRole(this.#userId, role).subscribe({
      next: () => this.#refreshRoles(),
      error: setApiErrors(this.errors),
    });
  }

  addRole(role: string) {
    this.errors.set([]);

    this.adminService.addUserToRole(this.#userId, role).subscribe({
      next: () => this.#refreshRoles(),
      error: setApiErrors(this.errors),
    });
  }
}
