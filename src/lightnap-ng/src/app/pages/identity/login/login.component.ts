import { Component, inject, signal } from "@angular/core";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";
import { FormBuilder, ReactiveFormsModule, Validators } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { LoginSuccessTypes, RoutePipe, setApiErrors } from "@core";
import { BrandedCardComponent } from "@core/components/branded-card/branded-card.component";
import { ErrorListComponent } from "@core/components/error-list/error-list.component";
import { RouteAliasService } from "@core/features/routing/services/route-alias-service";
import { BlockUiService } from "@core/services/block-ui.service";
import { IdentityService } from "@core/services/identity.service";
import { ButtonModule } from "primeng/button";
import { CheckboxModule } from "primeng/checkbox";
import { InputGroupModule } from "primeng/inputgroup";
import { InputGroupAddonModule } from "primeng/inputgroupaddon";
import { InputTextModule } from "primeng/inputtext";
import { PasswordModule } from "primeng/password";
import { finalize } from "rxjs";

@Component({
  templateUrl: "./login.component.html",
  imports: [
    ReactiveFormsModule,
    RouterModule,
    ButtonModule,
    InputTextModule,
    CheckboxModule,
    RoutePipe,
    PasswordModule,
    BrandedCardComponent,
    ErrorListComponent,
    InputGroupModule,
    InputGroupAddonModule,
  ],
})
export class LoginComponent {
  #identityService = inject(IdentityService);
  #blockUi = inject(BlockUiService);
  #fb = inject(FormBuilder);
  #routeAlias = inject(RouteAliasService);

  form = this.#fb.nonNullable.group({
    login: this.#fb.control("", [Validators.required]),
    password: this.#fb.control("", [Validators.required]),
    rememberMe: this.#fb.control(true),
  });

  showMagicLink = this.#fb.control("", [Validators.required, Validators.email]);

  errors = signal(new Array<string>());

  constructor() {
    this.form.controls.login.valueChanges.pipe(takeUntilDestroyed()).subscribe({
      next: login => this.showMagicLink.setValue(login),
    });

    this.form.markAsTouched();
  }

  logIn() {
    this.#blockUi.show({ message: "Logging in..." });
    this.#identityService
      .logIn({
        login: this.form.value.login!,
        password: this.form.value.password!,
        rememberMe: this.form.value.rememberMe!,
        deviceDetails: navigator.userAgent,
      })
      .pipe(finalize(() => this.#blockUi.hide()))
      .subscribe({
        next: result => {
          switch (result.type) {
            case LoginSuccessTypes.TwoFactorRequired:
              this.#routeAlias.navigate("verify-code", this.form.value.login);
              break;
            case LoginSuccessTypes.AccessToken:
              this.#identityService.redirectLoggedInUser();
              break;
            case LoginSuccessTypes.EmailVerificationRequired:
              this.#routeAlias.navigate("email-verification-required");
              break;
            default:
              throw new Error(`Unexpected LoginSuccessResult.type: '${result.type}'`);
          }
        },
        error: setApiErrors(this.errors),
      });
  }

  sendMagicLink() {
    this.#blockUi.show({ message: "Sending magic link..." });

    this.#identityService
      .requestMagicLinkEmail({
        email: this.form.value.login!,
      })
      .pipe(finalize(() => this.#blockUi.hide()))
      .subscribe({
        next: () => this.#routeAlias.navigate("magic-link-sent"),
        error: setApiErrors(this.errors),
      });
  }
}
