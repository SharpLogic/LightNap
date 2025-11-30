import { Component, inject, input, OnInit, signal } from "@angular/core";
import { FormBuilder, ReactiveFormsModule, Validators } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { ExternalLoginSuccessTypes, LoginSuccessTypes, RoutePipe, setApiErrors } from "@core";
import { BrandedCardComponent } from "@core/components/branded-card/branded-card.component";
import { ErrorListComponent } from "@core/components/error-list/error-list.component";
import { RouteAliasService } from "@core/features/routing/services/route-alias-service";
import { BlockUiService } from "@core/services/block-ui.service";
import { IdentityService } from "@core/services/identity.service";
import { ButtonModule } from "primeng/button";
import { CheckboxModule } from "primeng/checkbox";
import { InputTextModule } from "primeng/inputtext";
import { finalize, switchMap, take } from "rxjs";

@Component({
    standalone: true,
  templateUrl: "./external-login-register.component.html",
  imports: [ReactiveFormsModule, RouterModule, InputTextModule, ButtonModule, CheckboxModule, RoutePipe, ErrorListComponent, BrandedCardComponent],
})
export class ExternalLoginRegisterComponent implements OnInit {
  #identityService = inject(IdentityService);
  #blockUi = inject(BlockUiService);
  #fb = inject(FormBuilder);
  #routeAlias = inject(RouteAliasService);

  token = input.required<string>();

  form = this.#fb.nonNullable.group({
    email: this.#fb.control("", [Validators.required, Validators.email]),
    userName: this.#fb.control("", [Validators.required]),
    agreedToTerms: this.#fb.control(false, [Validators.requiredTrue]),
    rememberMe: this.#fb.control(true),
  });

  errors = signal(new Array<string>());

  ngOnInit() {
    this.#blockUi.show({ message: "Confirming external login..." });

    this.#identityService
      .watchLoggedIn$()
      .pipe(
        take(1),
        finalize(() => this.#blockUi.hide()),
        switchMap(_ => this.#identityService.getExternalLoginResult(this.token()))
      )
      .subscribe({
        next: loginResult => {
          switch (loginResult.type) {
            case ExternalLoginSuccessTypes.AlreadyLinked:
              this.#routeAlias.navigateWithExtras("external-login-complete", this.token(), { replaceUrl: true });
              break;
            case ExternalLoginSuccessTypes.NewAccountLink:
              this.#routeAlias.navigateWithExtras("user-home", null, { replaceUrl: true });
              break;
            case ExternalLoginSuccessTypes.RequiresRegistration:
              this.form.patchValue({
                email: loginResult.email ?? "",
                userName: loginResult.userName ?? "",
              });
              break;
            default:
              throw new Error(`Unexpected ExternalLoginSuccessResult.type: '${loginResult.type}'`);
          }
        },
        error: setApiErrors(this.errors),
      });
  }

  register() {
    this.#blockUi.show({ message: "Registering..." });

    this.#identityService
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
