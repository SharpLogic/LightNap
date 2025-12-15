import { Component, inject, input, OnInit, signal } from "@angular/core";
import { ReactiveFormsModule } from "@angular/forms";
import { setApiErrors } from "@core";
import { ErrorListComponent } from "@core/components/error-list/error-list.component";
import { RouteAliasService } from "@core/features/routing/services/route-alias-service";
import { BlockUiService } from "@core/services/block-ui.service";
import { IdentityService } from "@core/services/identity.service";
import { ButtonModule } from "primeng/button";
import { InputTextModule } from "primeng/inputtext";
import { PanelModule } from "primeng/panel";
import { finalize } from "rxjs";

@Component({
  templateUrl: "./confirm-email-change.component.html",
  imports: [ButtonModule, ErrorListComponent, InputTextModule, ReactiveFormsModule, PanelModule],
})
export class ConfirmEmailChangeComponent implements OnInit {
  readonly #identityService = inject(IdentityService);
  readonly #blockUi = inject(BlockUiService);
  readonly #routeAlias = inject(RouteAliasService);
  readonly newEmail = input.required<string>();
  readonly code = input.required<string>();

  errors = signal(new Array<string>());

  ngOnInit() {
    this.#blockUi.show({ message: "Confirming email change..." });
    this.#identityService
      .confirmEmailChange({
        newEmail: this.newEmail(),
        code: this.code(),
      })
      .pipe(finalize(() => this.#blockUi.hide()))
      .subscribe({
        next: () => this.#routeAlias.navigateWithExtras("profile", undefined, { replaceUrl: true }),
        error: setApiErrors(this.errors),
      });
  }
}
