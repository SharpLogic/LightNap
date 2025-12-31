import { CommonModule } from "@angular/common";
import { Component, inject, signal } from "@angular/core";
import { Integration, setApiErrors, TypeHelpers } from "@core";
import { ApiResponseComponent } from "@core/components/api-response/api-response.component";
import { ConfirmDialogComponent } from "@core/components/confirm-dialog/confirm-dialog.component";
import { ErrorListComponent } from "@core/components/error-list/error-list.component";
import { IntegrationsService } from "@core/features/integrations/services/integrations.service";
import { ConfirmationService } from "primeng/api";
import { ButtonModule } from "primeng/button";
import { PanelModule } from "primeng/panel";
import { TableModule } from "primeng/table";

@Component({
  templateUrl: "./integrations.component.html",
  imports: [CommonModule, TableModule, ButtonModule, ErrorListComponent, PanelModule, ApiResponseComponent, ConfirmDialogComponent],
})
export class IntegrationsComponent {
  readonly #integrationsService = inject(IntegrationsService);
  readonly #confirmationService = inject(ConfirmationService);

  readonly integrations$ = signal(this.#integrationsService.getMyIntegrations());

  readonly errors = signal(new Array<string>());

  readonly asIntegrations = TypeHelpers.cast<Array<Integration>>;
  readonly asIntegration = TypeHelpers.cast<Integration>;

  removeIntegration(event: any, integrationId: number) {
    this.#confirmationService.confirm({
      header: "Confirm Revoke",
      message: `Are you sure that you want to revoke this integration?`,
      target: event.target,
      key: integrationId,
      accept: () => {
        this.#integrationsService.deleteMyIntegration(integrationId).subscribe({
          next: () => this.integrations$.set(this.#integrationsService.getMyIntegrations()),
          error: setApiErrors(this.errors),
        });
      },
    });
  }
}
