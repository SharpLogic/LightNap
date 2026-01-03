import { Component, computed, inject, input } from "@angular/core";
import { ApiResponseComponent } from "@core/components/api-response/api-response.component";
import { IntegrationsService } from "@core/features/integrations/services/integrations.service";
import { RouteAliasService } from "@core/features/routing/services/route-alias-service";
import { tap } from "rxjs";

@Component({
  templateUrl: "./confirm.component.html",
  imports: [ApiResponseComponent],
})
export class ConfirmComponent {
  readonly #integrationsService = inject(IntegrationsService);
  readonly #routeAlias = inject(RouteAliasService);

  readonly confirmationToken = input.required<string>();

  readonly confirm$ = computed(() => {
    return this.#integrationsService.confirmIntegration({ confirmationToken: this.confirmationToken() }).pipe(
      tap(integration => {
        if (integration) {
          this.#routeAlias.navigate("manage-integration", integration!.id);
        }
      })
    );
  });
}
