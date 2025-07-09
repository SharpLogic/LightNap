import { CommonModule } from "@angular/common";
import { Component, inject, signal } from "@angular/core";
import { FormBuilder, ReactiveFormsModule, Validators } from "@angular/forms";
import { BlockUiService, ErrorListComponent } from "@core";
import { ProfileService } from "@core/services/profile.service";
import { RouteAliasService } from "@routing";
import { ButtonModule } from "primeng/button";
import { PanelModule } from 'primeng/panel';
import { InputTextModule } from "primeng/inputtext";
import { finalize } from "rxjs";

@Component({
  standalone: true,
  templateUrl: './change-email.component.html',
  imports: [CommonModule, ButtonModule, ErrorListComponent, InputTextModule, ReactiveFormsModule, PanelModule],
})
export class ChangeEmailComponent {
  readonly #profileService = inject(ProfileService);
  readonly #blockUi = inject(BlockUiService);
  readonly #routeAlias = inject(RouteAliasService);
  readonly #fb = inject(FormBuilder);

  errors = signal(new Array<string>());

  form = this.#fb.nonNullable.group(
    {
      newEmail: this.#fb.control("", [Validators.required, Validators.email]),
    }
  );

  changeEmail() {
    this.#blockUi.show({ message: "Changing email..." });
    this.#profileService
      .changeEmail({
        newEmail: this.form.value.newEmail!,
      })
      .pipe(finalize(() => this.#blockUi.hide()))
      .subscribe({
        next: () => this.#routeAlias.navigate("change-email-requested"),
        error: response => this.errors.set(response.errorMessages),
      });
  }
}
