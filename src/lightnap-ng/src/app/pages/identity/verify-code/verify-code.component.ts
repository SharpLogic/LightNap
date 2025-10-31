import { Component, inject, input, signal } from "@angular/core";
import { FormBuilder, ReactiveFormsModule, Validators } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { BlockUiService, ErrorListComponent, BrandedCardComponent, setApiErrors } from "@core";
import { RouteAliasService, RoutePipe } from "@core";
import { ButtonModule } from "primeng/button";
import { CheckboxModule } from "primeng/checkbox";
import { InputOtpModule } from "primeng/inputotp";
import { finalize } from "rxjs";
import { IdentityService } from "@core/services/identity.service";

@Component({
  standalone: true,
  templateUrl: "./verify-code.component.html",
  imports: [ReactiveFormsModule, RouterModule, CheckboxModule, ButtonModule, RoutePipe, InputOtpModule, BrandedCardComponent, ErrorListComponent],
})
export class VerifyCodeComponent {
  #identityService = inject(IdentityService);
  #blockUi = inject(BlockUiService);
  #fb = inject(FormBuilder);
  #routeAlias = inject(RouteAliasService);

  readonly login = input.required<string>();

  form = this.#fb.group({
    code: this.#fb.control("", [Validators.required]),
    rememberMe: this.#fb.control(false),
  });

  errors = signal(new Array<string>());

  onVerifyClicked() {
    const value = this.form.value;

    this.#blockUi.show({ message: "Verifying code..." });
    this.#identityService
      .verifyCode({
        code: value.code!,
        login: this.login(),
        deviceDetails: navigator.userAgent,
        rememberMe: value.rememberMe!,
      })
      .pipe(finalize(() => this.#blockUi.hide()))
      .subscribe({
        next: () => this.#routeAlias.navigate("user-home"),
        error: setApiErrors(this.errors),
      });
  }
}
