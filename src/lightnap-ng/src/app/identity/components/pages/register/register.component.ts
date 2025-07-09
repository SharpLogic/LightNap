import { Component, inject, signal } from "@angular/core";
import { FormBuilder, ReactiveFormsModule, Validators } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { BlockUiService } from "@core";
import { ErrorListComponent } from "@core/components/controls/error-list/error-list.component";
import { confirmPasswordValidator } from "@core/helpers/form-helpers";
import { IdentityCardComponent } from "@identity/components/controls/identity-card/identity-card.component";
import { RouteAliasService, RoutePipe } from "@routing";
import { ButtonModule } from "primeng/button";
import { CheckboxModule } from "primeng/checkbox";
import { InputTextModule } from "primeng/inputtext";
import { PasswordModule } from "primeng/password";
import { finalize } from "rxjs";
import { IdentityService } from "@core/services/identity.service";

@Component({
  standalone: true,
  templateUrl: "./register.component.html",
  imports: [
    ReactiveFormsModule,
    RouterModule,
    InputTextModule,
    ButtonModule,
    PasswordModule,
    CheckboxModule,
    RoutePipe,
    ErrorListComponent,
    IdentityCardComponent,
  ],
})
export class RegisterComponent {
  #identityService = inject(IdentityService);
  #blockUi = inject(BlockUiService);
  #fb = inject(FormBuilder);
  #routeAlias = inject(RouteAliasService);

  form = this.#fb.nonNullable.group(
    {
      email: this.#fb.control("", [Validators.required, Validators.email]),
      password: this.#fb.control("", [Validators.required]),
      confirmPassword: this.#fb.control("", [Validators.required]),
      userName: this.#fb.control("", [Validators.required]),
      agreedToTerms: this.#fb.control(false, [Validators.requiredTrue]),
      rememberMe: this.#fb.control(true),
    },
    { validators: [confirmPasswordValidator("password", "confirmPassword")] }
  );

  errors = signal(new Array<string>());

  register() {
    this.#blockUi.show({ message: "Registering..." });

    this.#identityService
      .register({
        email: this.form.value.email!,
        password: this.form.value.password!,
        confirmPassword: this.form.value.confirmPassword!,
        deviceDetails: navigator.userAgent,
        rememberMe: this.form.value.rememberMe!,
        userName: this.form.value.userName!,
      })
      .pipe(finalize(() => this.#blockUi.hide()))
      .subscribe({
        next: loginResult => {
          switch (loginResult.type) {
            case "TwoFactorRequired":
              this.#routeAlias.navigate("verify-code", this.form.value.email);
              break;
            case "AccessToken":
                this.#identityService.redirectLoggedInUser();
              break;
            case "EmailVerificationRequired":
              this.#routeAlias.navigate("email-verification-required");
              break;
            default:
              throw new Error(`Unexpected LoginSuccessResult.type: '${loginResult.type}'`);
          }
        },
        error: response => this.errors.set(response.errorMessages),
      });
  }
}
