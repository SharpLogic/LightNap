import { CommonModule } from "@angular/common";
import { Component, inject, input, OnInit, signal } from "@angular/core";
import { ReactiveFormsModule } from "@angular/forms";
import { ErrorListComponent } from "@core";
import { BlockUiService, RouteAliasService } from "@core";
import { ProfileService } from "@core/services/profile.service";
import { ButtonModule } from "primeng/button";
import { InputTextModule } from "primeng/inputtext";
import { PanelModule } from "primeng/panel";
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
        next: () => this.#routeAlias.navigateWithExtras("profile", undefined, { replaceUrl: true }),
        error: response => this.errors.set(response.errorMessages),
      });
  }
}
