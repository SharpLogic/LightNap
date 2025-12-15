import { Component, inject, input, signal } from "@angular/core";
import { FormBuilder, ReactiveFormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { LoginSuccessType, setApiErrors } from "@core";
import { BrandedCardComponent } from "@core/components/branded-card/branded-card.component";
import { ErrorListComponent } from "@core/components/error-list/error-list.component";
import { RouteAliasService } from "@core/features/routing/services/route-alias-service";
import { BlockUiService } from "@core/services/block-ui.service";
import { ExternalLoginService } from "@core/services/external-login.service";
import { IdentityService } from "@core/services/identity.service";
import { ButtonModule } from "primeng/button";
import { CheckboxModule } from "primeng/checkbox";
import { finalize } from "rxjs";

@Component({
  templateUrl: "./complete.component.html",
  imports: [ReactiveFormsModule, RouterModule, ButtonModule, CheckboxModule, ErrorListComponent, BrandedCardComponent],
})
export class CompleteComponent {
  readonly #identityService = inject(IdentityService);
  readonly #externalLoginService = inject(ExternalLoginService);
  readonly #blockUi = inject(BlockUiService);
  readonly #fb = inject(FormBuilder);
  readonly #routeAlias = inject(RouteAliasService);

  readonly token = input.required<string>();

  readonly form = this.#fb.nonNullable.group({
    rememberMe: this.#fb.control(true),
  });

  readonly errors = signal(new Array<string>());

  logIn() {
    this.#blockUi.show({ message: "Completing login..." });

    this.#externalLoginService
      .completeExternalLogin(this.token(), {
        deviceDetails: navigator.userAgent,
        rememberMe: this.form.value.rememberMe!,
      })
      .pipe(finalize(() => this.#blockUi.hide()))
      .subscribe({
        next: loginResult => {
          switch (loginResult.type) {
            case LoginSuccessType.TwoFactorRequired:
              alert("Check your email for two-factor verification.");
              break;
            case LoginSuccessType.AccessToken:
              this.#identityService.redirectLoggedInUser();
              break;
            case LoginSuccessType.EmailVerificationRequired:
              this.#routeAlias.navigate("email-verification-required");
              break;
            default:
              throw new Error(`Unexpected LoginSuccessResult.type: '${loginResult.type}'`);
          }
        },
        error: setApiErrors(this.errors),
      });
  }
}
