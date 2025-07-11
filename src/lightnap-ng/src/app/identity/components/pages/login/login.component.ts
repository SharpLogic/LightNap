import { Component, inject, signal } from "@angular/core";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";
import { FormBuilder, ReactiveFormsModule, Validators } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { BlockUiService, ErrorListComponent } from "@core";
import { IdentityService } from "@core/services/identity.service";
import { RouteAliasService, RoutePipe } from "@routing";
import { ButtonModule } from "primeng/button";
import { CheckboxModule } from "primeng/checkbox";
import { InputGroupModule } from "primeng/inputgroup";
import { InputGroupAddonModule } from "primeng/inputgroupaddon";
import { InputTextModule } from "primeng/inputtext";
import { PasswordModule } from "primeng/password";
import { finalize } from "rxjs";
import { IdentityCardComponent } from "../../controls/identity-card/identity-card.component";

@Component({
  standalone: true,
  templateUrl: "./login.component.html",
  imports: [
    ReactiveFormsModule,
    RouterModule,
    ButtonModule,
    InputTextModule,
    CheckboxModule,
    RoutePipe,
    PasswordModule,
    IdentityCardComponent,
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

  showMagicLink = this.#fb.control("", [Validators.email]);

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
            case "TwoFactorRequired":
              this.#routeAlias.navigate("verify-code", this.form.value.login);
              break;
            case "AccessToken":
              this.#identityService.redirectLoggedInUser();
              break;
            case "EmailVerificationRequired":
              this.#routeAlias.navigate("email-verification-required");
              break;
            default:
              throw new Error(`Unexpected LoginSuccessResult.type: '${result.type}'`);
          }
        },
        error: response => this.errors.set(response.errorMessages),
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
        error: response => this.errors.set(response.errorMessages),
      });
  }
}
