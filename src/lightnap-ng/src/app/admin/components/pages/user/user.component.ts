import { AdminUser, Role } from "@admin/models";
import { AdminService } from "@admin/services/admin.service";
import { CommonModule } from "@angular/common";
import { Component, inject, input, OnChanges, signal } from "@angular/core";
import { ReactiveFormsModule } from "@angular/forms";
import { ConfirmPopupComponent, ToastService } from "@core";
import { ApiResponseComponent } from "@core/components/controls/api-response/api-response.component";
import { ErrorListComponent } from "@core/components/controls/error-list/error-list.component";
import { Claim } from "@identity";
import { RouteAliasService } from "@routing";
import { ConfirmationService } from "primeng/api";
import { ButtonModule } from "primeng/button";
import { PanelModule } from "primeng/panel";
import { TabsModule } from "primeng/tabs";
import { TagModule } from "primeng/tag";
import { Observable } from "rxjs";
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
  adminService = inject(AdminService);
  #confirmationService = inject(ConfirmationService);
  #toast = inject(ToastService);
  #routeAlias = inject(RouteAliasService);

  userId = input.required<string>();

  errors = signal(new Array<string>());

  user$ = signal<Observable<AdminUser>>(new Observable<AdminUser>());
  userClaims$ = signal<Observable<Array<Claim>>>(new Observable<Array<Claim>>());
  userRoles$ = signal<Observable<Array<Role>>>(new Observable<Array<Role>>());

  ngOnChanges() {
    this.#refreshUser();
    this.#refreshRoles();
    this.#refreshClaims();
  }

  #refreshUser() {
    this.user$.set(this.adminService.getUser(this.userId()));
  }

  #refreshRoles() {
    this.userRoles$.set(this.adminService.getUserRoles(this.userId()));
  }

  #refreshClaims() {
    this.userClaims$.set(this.adminService.getUserClaims(this.userId()));
  }

  lockUserAccount(event: any) {
    this.errors.set([]);

    this.#confirmationService.confirm({
      header: "Confirm Lock Account",
      message: `Are you sure that you want to lock this user account?`,
      target: event.target,
      key: "lock",
      accept: () => {
        this.adminService.lockUserAccount(this.userId()).subscribe({
          next: () => this.#refreshUser(),
          error: response => this.errors.set(response.errorMessages),
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
        this.adminService.unlockUserAccount(this.userId()).subscribe({
          next: () => this.#refreshUser(),
          error: response => this.errors.set(response.errorMessages),
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
        this.adminService.deleteUser(this.userId()).subscribe({
          next: () => {
            this.#toast.success("User deleted successfully.");
            this.#routeAlias.navigate("admin-users");
          },
          error: response => this.errors.set(response.errorMessages),
        });
      },
    });
  }

  removeRole(role: string) {
    this.errors.set([]);

    this.adminService.removeUserFromRole(this.userId(), role).subscribe({
      next: () => this.#refreshRoles(),
      error: response => this.errors.set(response.errorMessages),
    });
  }

  addRole(role: string) {
    this.errors.set([]);

    this.adminService.addUserToRole(this.userId(), role).subscribe({
      next: () => this.#refreshRoles(),
      error: response => this.errors.set(response.errorMessages),
    });
  }

  removeClaim(claim: Claim) {
    this.errors.set([]);

    this.adminService.removeClaimFromUser(this.userId(), claim).subscribe({
      next: () => this.#refreshClaims(),
      error: response => this.errors.set(response.errorMessages),
    });
  }

  addClaim(claim: Claim) {
    this.errors.set([]);

    this.adminService.addClaimToUser(this.userId(), claim).subscribe({
      next: () => this.#refreshClaims(),
      error: response => this.errors.set(response.errorMessages),
    });
  }
}
