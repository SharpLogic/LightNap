import { CommonModule } from "@angular/common";
import { Component, inject, signal } from "@angular/core";
import { FormBuilder, ReactiveFormsModule, Validators } from "@angular/forms";
import { setApiErrors } from "@core";
import { ErrorListComponent } from "@core/components/error-list/error-list.component";
import { RouteAliasService } from "@core/routing/services/route-alias-service";
import { BlockUiService } from "@core/services/block-ui.service";
import { IdentityService } from "@core/services/identity.service";
import { ButtonModule } from "primeng/button";
import { InputTextModule } from "primeng/inputtext";
import { PanelModule } from "primeng/panel";
import { finalize } from "rxjs";

@Component({
  standalone: true,
  templateUrl: "./change-email.component.html",
  imports: [CommonModule, ButtonModule, ErrorListComponent, InputTextModule, ReactiveFormsModule, PanelModule],
})
export class ChangeEmailComponent {
  readonly #identityService = inject(IdentityService);
  readonly #blockUi = inject(BlockUiService);
  readonly #routeAlias = inject(RouteAliasService);
  readonly #fb = inject(FormBuilder);

  errors = signal(new Array<string>());

  form = this.#fb.nonNullable.group({
    newEmail: this.#fb.control("", [Validators.required, Validators.email]),
  });

  changeEmail() {
    this.#blockUi.show({ message: "Changing email..." });
    this.#identityService
      .changeEmail({
        newEmail: this.form.value.newEmail!,
      })
      .pipe(finalize(() => this.#blockUi.hide()))
      .subscribe({
        next: () => this.#routeAlias.navigate("change-email-requested"),
        error: setApiErrors(this.errors),
      });
  }
}
