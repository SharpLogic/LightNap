import { CommonModule } from "@angular/common";
import { Component, inject, signal } from "@angular/core";
import { RouterLink } from "@angular/router";
import { IntegrationProviderDefinition, RoutePipe, TypeHelpers } from "@core";
import { ApiResponseComponent } from "@core/components/api-response/api-response.component";
import { IntegrationsService } from "@core/features/integrations/services/integrations.service";
import { ButtonModule } from "primeng/button";
import { CardModule } from "primeng/card";
import { PanelModule } from "primeng/panel";
import { TagModule } from "primeng/tag";

@Component({
  templateUrl: "./supported-integrations.component.html",
  imports: [CommonModule, RouterLink, RoutePipe, ButtonModule, TagModule, PanelModule, CardModule, ApiResponseComponent],
})
export class SupportedIntegrationsComponent {
  readonly #integrationsService = inject(IntegrationsService);

  readonly providers$ = signal(this.#integrationsService.getSupportedProviders());

  readonly asProviders = TypeHelpers.cast<Array<IntegrationProviderDefinition>>;
}
