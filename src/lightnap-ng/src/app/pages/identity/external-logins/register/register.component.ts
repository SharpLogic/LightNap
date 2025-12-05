import { Component, inject, input, OnInit, signal } from "@angular/core";
import { FormBuilder, ReactiveFormsModule, Validators } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { ExternalLoginSuccessTypes, LoginSuccessTypes, RoutePipe, setApiErrors } from "@core";
import { BrandedCardComponent } from "@core/components/branded-card/branded-card.component";
import { ErrorListComponent } from "@core/components/error-list/error-list.component";
import { RouteAliasService } from "@core/features/routing/services/route-alias-service";
import { BlockUiService } from "@core/services/block-ui.service";
import { ExternalLoginService } from "@core/services/external-login.service";
import { IdentityService } from "@core/services/identity.service";
import { ButtonModule } from "primeng/button";
import { CheckboxModule } from "primeng/checkbox";
import { InputTextModule } from "primeng/inputtext";
import { finalize } from "rxjs";

@Component({
  standalone: true,
  templateUrl: "./register.component.html",
  imports: [ReactiveFormsModule, RouterModule, InputTextModule, ButtonModule, CheckboxModule, RoutePipe, ErrorListComponent, BrandedCardComponent],
})
export class RegisterComponent implements OnInit {
  readonly #identityService = inject(IdentityService);
  readonly #externalLoginService = inject(ExternalLoginService);
  readonly #blockUi = inject(BlockUiService);
  readonly #fb = inject(FormBuilder);
  readonly #routeAlias = inject(RouteAliasService);

  readonly token = input.required<string>();

  readonly form = this.#fb.nonNullable.group({
    email: this.#fb.control("", [Validators.required, Validators.email]),
    userName: this.#fb.control("", [Validators.required]),
    agreedToTerms: this.#fb.control(false, [Validators.requiredTrue]),
    rememberMe: this.#fb.control(true),
  });

  readonly errors = signal(new Array<string>());

  ngOnInit() {
    this.#blockUi.show({ message: "Confirming external login..." });

    this.#externalLoginService
      .getExternalLoginResult(this.token())
      .pipe(finalize(() => this.#blockUi.hide()))
      .subscribe({
        next: loginResult => {
          switch (loginResult.type) {
            case ExternalLoginSuccessTypes.RequiresRegistration:
              this.form.patchValue({
                email: loginResult.email ?? "",
                userName: loginResult.userName ?? "",
              });
              break;
            default:
                this.#routeAlias.navigateWithExtras("external-login-callback", this.token(), { replaceUrl: true });
              break;
          }
        },
        error: setApiErrors(this.errors),
      });
  }

  register() {
    this.#blockUi.show({ message: "Registering..." });

    this.#externalLoginService
      .completeExternalLoginRegistration(this.token(), {
        email: this.form.value.email!,
        deviceDetails: navigator.userAgent,
        rememberMe: this.form.value.rememberMe!,
        userName: this.form.value.userName!,
      })
      .pipe(finalize(() => this.#blockUi.hide()))
      .subscribe({
        next: loginResult => {
          switch (loginResult.type) {
            case LoginSuccessTypes.TwoFactorRequired:
              this.#routeAlias.navigate("verify-code", this.form.value.email);
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
