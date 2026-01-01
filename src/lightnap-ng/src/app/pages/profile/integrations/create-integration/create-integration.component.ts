import { CommonModule } from "@angular/common";
import { Component, computed, inject, input } from "@angular/core";
import { Router } from "@angular/router";
import { ApiResponseComponent } from "@core/components/api-response/api-response.component";
import { IntegrationsService } from "@core/features/integrations/services/integrations.service";
import { ButtonModule } from "primeng/button";
import { CardModule } from "primeng/card";
import { PanelModule } from "primeng/panel";
import { TagModule } from "primeng/tag";
import { map, tap } from "rxjs";

@Component({
  templateUrl: "./create-integration.component.html",
  imports: [CommonModule, ButtonModule, TagModule, PanelModule, CardModule, ApiResponseComponent],
})
export class CreateIntegrationComponent {
  readonly #integrationsService = inject(IntegrationsService);
  readonly #router = inject(Router);

  readonly provider = input.required<string>();

  readonly providerDefinition$ = computed(() => {
    return this.#integrationsService.getSupportedProviders().pipe(
      map(providers => providers.find(p => p.provider === this.provider())),
      tap(provider => {
        if (!provider) {
          throw new Error(`Unsupported integration provider: ${this.provider()}`);
        }
        if (!provider.isConfiguredManually) {
          this.#router.navigateByUrl(`/api/Integrations/connect/${this.provider()}`, { replaceUrl: true } );
        }
      })
    );
  });
}
