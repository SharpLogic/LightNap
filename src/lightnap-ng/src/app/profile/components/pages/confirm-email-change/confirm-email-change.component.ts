import { CommonModule } from "@angular/common";
import { Component, inject, input, OnInit, signal } from "@angular/core";
import { FormBuilder, ReactiveFormsModule, Validators } from "@angular/forms";
import { BlockUiService, ErrorListComponent } from "@core";
import { ProfileService } from "@profile/services/profile.service";
import { RouteAliasService } from "@routing";
import { ButtonModule } from "primeng/button";
import { PanelModule } from 'primeng/panel';
import { InputTextModule } from "primeng/inputtext";
import { finalize } from "rxjs";

@Component({
  standalone: true,
  templateUrl: "./confirm-email-change.component.html",
  imports: [CommonModule, ButtonModule, ErrorListComponent, InputTextModule, ReactiveFormsModule, PanelModule],
})
export class ConfirmEmailChangeComponent implements OnInit {
  readonly #profileService = inject(ProfileService);
  readonly #blockUi = inject(BlockUiService);
  readonly #routeAlias = inject(RouteAliasService);
  readonly newEmail = input.required<string>();
  readonly code = input.required<string>();

  errors = signal(new Array<string>());

  ngOnInit() {
    this.#blockUi.show({ message: "Confirming email change..." });
    this.#profileService
      .confirmEmailChange({
        newEmail: this.newEmail(),
        code: this.code(),
      })
      .pipe(finalize(() => this.#blockUi.hide()))
      .subscribe({
        next: () => this.#routeAlias.navigateWithReplace("profile"),
        error: response => this.errors.set(response.errorMessages),
      });
  }
}
