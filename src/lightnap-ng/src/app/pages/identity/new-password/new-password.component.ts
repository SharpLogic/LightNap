import { Component, inject, input, signal } from "@angular/core";
import { FormBuilder, ReactiveFormsModule, Validators } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { BlockUiService, LoginSuccessTypes } from "@core";
import { ErrorListComponent } from "@core/components/error-list/error-list.component";
import { confirmPasswordValidator } from "@core/helpers/form-helpers";
import { RouteAliasService, RoutePipe } from "@core";
import { ButtonModule } from "primeng/button";
import { CheckboxModule } from "primeng/checkbox";
import { PasswordModule } from "primeng/password";
import { finalize } from "rxjs";
import { IdentityService } from "@core/services/identity.service";
import { BrandedCardComponent } from "@core";

@Component({
  standalone: true,
  templateUrl: "./new-password.component.html",
  imports: [ReactiveFormsModule, RouterModule, ButtonModule, PasswordModule, CheckboxModule, RoutePipe, ErrorListComponent, BrandedCardComponent],
})
export class NewPasswordComponent {
  #identityService = inject(IdentityService);
  #blockUi = inject(BlockUiService);
  #fb = inject(FormBuilder);
  #routeAlias = inject(RouteAliasService);

  readonly email = input("");
  readonly token = input("");

  errors = signal(new Array<string>());

  form = this.#fb.nonNullable.group(
    {
      password: this.#fb.control("", [Validators.required]),
      confirmPassword: this.#fb.control("", [Validators.required]),
      rememberMe: this.#fb.control(false),
    },
    { validators: [confirmPasswordValidator("password", "confirmPassword")] }
  );

  newPassword() {
    this.#blockUi.show({ message: "Setting new password..." });
    this.#identityService
      .newPassword({
        email: this.email(),
        password: this.form.value.password!,
        token: this.token(),
        deviceDetails: navigator.userAgent,
        rememberMe: this.form.value.rememberMe!,
      })
      .pipe(finalize(() => this.#blockUi.hide()))
      .subscribe({
        next: result => {
          switch (result.type) {
            case LoginSuccessTypes.AccessToken:
                this.#identityService.redirectLoggedInUser();
              break;
            case LoginSuccessTypes.TwoFactorRequired:
              this.#routeAlias.navigate("verify-code", this.email());
              break;
            case LoginSuccessTypes.EmailVerificationRequired:
              throw new Error("Email verification should not be required since email was used to set a new password.");
            default:
              throw new Error(`Unexpected LoginSuccessResult.type: '${result.type}'`);
          }
        },
        error: response => this.errors.set(response.errorMessages),
      });
  }
}
