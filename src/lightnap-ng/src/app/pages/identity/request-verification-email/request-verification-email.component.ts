import { Component, inject, signal } from "@angular/core";
import { FormBuilder, ReactiveFormsModule, Validators } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { BlockUiService, ErrorListComponent, setApiErrors } from "@core";
import { IdentityService } from "@core/services/identity.service";
import { RouteAliasService, RoutePipe } from "@core";
import { ButtonModule } from "primeng/button";
import { InputTextModule } from "primeng/inputtext";
import { PasswordModule } from "primeng/password";
import { finalize } from "rxjs";
import { BrandedCardComponent } from "@core";

@Component({
  standalone: true,
  templateUrl: './request-verification-email.component.html',
  imports: [ReactiveFormsModule, RouterModule, ButtonModule, PasswordModule, InputTextModule, RoutePipe, BrandedCardComponent, ErrorListComponent],
})
export class RequestVerificationEmailComponent {
  #identityService = inject(IdentityService);
  #blockUi = inject(BlockUiService);
  #fb = inject(FormBuilder);
  #routeAlias = inject(RouteAliasService);

  form = this.#fb.nonNullable.group({
    email: this.#fb.control("", [Validators.required, Validators.email]),
  });

  errors = signal(new Array<string>());

  resendVerificationEmail() {
    this.#blockUi.show({ message: "Resending verification email..." });
    this.#identityService
      .requestVerificationEmail({ email: this.form.value.email! })
      .pipe(finalize(() => this.#blockUi.hide()))
      .subscribe({
        next: () => this.#routeAlias.navigate("email-verification-required"),
        error: setApiErrors(this.errors),
      });
  }
}
