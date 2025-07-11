import { Component, inject, signal } from "@angular/core";
import { FormBuilder, ReactiveFormsModule, Validators } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { BlockUiService, ErrorListComponent } from "@core";
import { IdentityService } from "@core/services/identity.service";
import { RouteAliasService, RoutePipe } from "@pages";
import { ButtonModule } from "primeng/button";
import { InputTextModule } from "primeng/inputtext";
import { PasswordModule } from "primeng/password";
import { finalize } from "rxjs";
import { IdentityCardComponent } from "../../controls/identity-card/identity-card.component";

@Component({
  standalone: true,
  templateUrl: './request-verification-email.component.html',
  imports: [ReactiveFormsModule, RouterModule, ButtonModule, PasswordModule, InputTextModule, RoutePipe, IdentityCardComponent, ErrorListComponent],
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
        error: response => this.errors.set(response.errorMessages),
      });
  }
}
