import { Component, inject, input, signal } from "@angular/core";
import { FormBuilder, ReactiveFormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { LoginSuccessTypes, setApiErrors } from "@core";
import { BrandedCardComponent } from "@core/components/branded-card/branded-card.component";
import { ErrorListComponent } from "@core/components/error-list/error-list.component";
import { RouteAliasService } from "@core/features/routing/services/route-alias-service";
import { BlockUiService } from "@core/services/block-ui.service";
import { IdentityService } from "@core/services/identity.service";
import { ButtonModule } from "primeng/button";
import { CheckboxModule } from "primeng/checkbox";
import { InputTextModule } from "primeng/inputtext";
import { finalize } from "rxjs";

@Component({
  standalone: true,
  templateUrl: "./external-login-complete.component.html",
  imports: [ReactiveFormsModule, RouterModule, ButtonModule, CheckboxModule, ErrorListComponent, BrandedCardComponent],
})
export class ExternalLoginCompleteComponent {
  #identityService = inject(IdentityService);
  #blockUi = inject(BlockUiService);
  #fb = inject(FormBuilder);
  #routeAlias = inject(RouteAliasService);

  token = input.required<string>();

  form = this.#fb.nonNullable.group({
    rememberMe: this.#fb.control(true),
  });

  errors = signal(new Array<string>());

  logIn() {
    this.#blockUi.show({ message: "Completing login..." });

    this.#identityService
      .completeExternalLogin(this.token(), {
        deviceDetails: navigator.userAgent,
        rememberMe: this.form.value.rememberMe!,
      })
      .pipe(finalize(() => this.#blockUi.hide()))
      .subscribe({
        next: loginResult => {
          switch (loginResult.type) {
            case LoginSuccessTypes.TwoFactorRequired:
              alert("Check your email for two-factor verification.");
              break;
            case LoginSuccessTypes.AccessToken:
              this.#identityService.redirectLoggedInUser();
              break;
            case LoginSuccessTypes.EmailVerificationRequired:
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
