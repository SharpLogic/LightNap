import { Component, inject, signal } from "@angular/core";
import { FormBuilder, ReactiveFormsModule, Validators } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { RouteAliasService, RoutePipe, setApiErrors } from "@core";
import { BrandedCardComponent } from "@core/components/branded-card/branded-card.component";
import { ErrorListComponent } from "@core/components/error-list/error-list.component";
import { BlockUiService } from "@core/services/block-ui.service";
import { IdentityService } from "@core/services/identity.service";
import { ButtonModule } from "primeng/button";
import { InputTextModule } from "primeng/inputtext";
import { PasswordModule } from "primeng/password";
import { finalize } from "rxjs";

@Component({
  standalone: true,
  templateUrl: "./reset-password.component.html",
  imports: [ReactiveFormsModule, RouterModule, ButtonModule, PasswordModule, InputTextModule, RoutePipe, BrandedCardComponent, ErrorListComponent],
})
export class ResetPasswordComponent {
  #identityService = inject(IdentityService);
  #blockUi = inject(BlockUiService);
  #fb = inject(FormBuilder);
  #routeAlias = inject(RouteAliasService);

  form = this.#fb.nonNullable.group({
    email: this.#fb.control("", [Validators.required, Validators.email]),
  });

  errors = signal(new Array<string>());

  resetPassword() {
    this.#blockUi.show({ message: "Resetting password..." });
    this.#identityService
      .resetPassword({ email: this.form.value.email! })
      .pipe(finalize(() => this.#blockUi.hide()))
      .subscribe({
        next: () => this.#routeAlias.navigate("reset-instructions-sent"),
        error: setApiErrors(this.errors),
      });
  }
}
