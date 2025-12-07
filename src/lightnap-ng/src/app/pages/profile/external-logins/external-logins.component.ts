import { CommonModule } from "@angular/common";
import { Component, inject, signal } from "@angular/core";
import { RouterLink } from "@angular/router";
import { ExternalLoginDto, RoutePipe, setApiErrors, TypeHelpers } from "@core";
import { ApiResponseComponent } from "@core/components/api-response/api-response.component";
import { ConfirmDialogComponent } from "@core/components/confirm-dialog/confirm-dialog.component";
import { ErrorListComponent } from "@core/components/error-list/error-list.component";
import { ProfileService } from "@core/services/profile.service";
import { ConfirmationService } from "primeng/api";
import { ButtonModule } from "primeng/button";
import { PanelModule } from "primeng/panel";
import { TableModule } from "primeng/table";

@Component({
  standalone: true,
  templateUrl: "./external-logins.component.html",
  imports: [
    CommonModule,
    TableModule,
    ButtonModule,
    ErrorListComponent,
    PanelModule,
    ApiResponseComponent,
    ConfirmDialogComponent,
    RouterLink,
    RoutePipe,
  ],
})
export class ExternalLoginsComponent {
  readonly #profileService = inject(ProfileService);
  readonly #confirmationService = inject(ConfirmationService);

  readonly externalLogins$ = signal(this.#profileService.getExternalLogins());

  readonly errors = signal(new Array<string>());

  readonly asExternalLogins = TypeHelpers.cast<Array<ExternalLoginDto>>;
  readonly asExternalLogin = TypeHelpers.cast<ExternalLoginDto>;

  removeExternalLogin(event: any, loginProvider: string, providerKey: string) {
    this.#confirmationService.confirm({
      header: "Confirm Removal",
      message: `Are you sure that you want to remove this external login?`,
      target: event.target,
      key: `${loginProvider}:${providerKey}`,
      accept: () => {
        this.#profileService.removeExternalLogin(loginProvider, providerKey).subscribe({
          next: () => this.externalLogins$.set(this.#profileService.getExternalLogins()),
          error: setApiErrors(this.errors),
        });
      },
    });
  }
}
