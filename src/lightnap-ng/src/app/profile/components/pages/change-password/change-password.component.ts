import { CommonModule } from "@angular/common";
import { Component, inject, signal } from "@angular/core";
import { FormBuilder, ReactiveFormsModule, Validators } from "@angular/forms";
import { BlockUiService } from "@core";
import { ErrorListComponent } from "@core/components/controls/error-list/error-list.component";
import { confirmPasswordValidator } from "@core/helpers/form-helpers";
import { ToastService } from "@core/services/toast.service";
import { ProfileService } from "@profile/services/profile.service";
import { ButtonModule } from "primeng/button";
import { PanelModule } from 'primeng/panel';
import { InputGroupModule } from "primeng/inputgroup";
import { InputGroupAddonModule } from 'primeng/inputgroupaddon';
import { PasswordModule } from "primeng/password";
import { finalize } from "rxjs";

@Component({
  standalone: true,
  templateUrl: "./change-password.component.html",
  imports: [CommonModule, ButtonModule, ErrorListComponent, PasswordModule, ReactiveFormsModule, PanelModule, InputGroupModule, InputGroupAddonModule],
})
export class ChangePasswordComponent {
  readonly #profileService = inject(ProfileService);
  readonly #blockUi = inject(BlockUiService);
  readonly #toast = inject(ToastService);
  readonly #fb = inject(FormBuilder);

  errors = signal(new Array<string>());

  form = this.#fb.nonNullable.group(
    {
      currentPassword: this.#fb.control("", [Validators.required]),
      newPassword: this.#fb.control("", [Validators.required]),
      confirmNewPassword: this.#fb.control("", [Validators.required]),
    },
    { validators: [confirmPasswordValidator("newPassword", "confirmNewPassword")] }
  );

  changePassword() {
    this.errors.set([]);
    this.#blockUi.show({ message: "Changing password..." });
    this.#profileService
      .changePassword({
        confirmNewPassword: this.form.value.confirmNewPassword!,
        currentPassword: this.form.value.currentPassword!,
        newPassword: this.form.value.newPassword!,
      })
      .pipe(finalize(() => this.#blockUi.hide()))
      .subscribe({
        next: () => {
          this.#toast.success("Password changed successfully.");
          this.form.reset();
        },
        error: response => this.errors.set(response.errorMessages),
      });
  }
}
