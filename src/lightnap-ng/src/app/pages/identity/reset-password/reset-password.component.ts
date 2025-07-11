import { Component, inject, signal } from "@angular/core";
import { FormBuilder, ReactiveFormsModule, Validators } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { BlockUiService, ErrorListComponent } from "@core";
import { RouteAliasService, RoutePipe } from "@pages";
import { ButtonModule } from "primeng/button";
import { InputTextModule } from "primeng/inputtext";
import { PasswordModule } from "primeng/password";
import { finalize } from "rxjs";
import { IdentityService } from "@core/services/identity.service";
import { IdentityCardComponent } from "../../controls/identity-card/identity-card.component";

@Component({
  standalone: true,
  templateUrl: "./reset-password.component.html",
  imports: [ReactiveFormsModule, RouterModule, ButtonModule, PasswordModule, InputTextModule, RoutePipe, IdentityCardComponent, ErrorListComponent],
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
        error: response => this.errors.set(response.errorMessages),
      });
  }
}
