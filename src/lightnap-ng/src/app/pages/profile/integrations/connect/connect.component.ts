import { CommonModule } from "@angular/common";
import { Component, computed, inject, input } from "@angular/core";
import { FormBuilder, ReactiveFormsModule, Validators } from "@angular/forms";
import { Router } from "@angular/router";
import { ErrorApiResponse, IntegrationProvider, IntegrationProviderDefinition, TypeHelpers } from "@core";
import { ApiResponseComponent } from "@core/components/api-response/api-response.component";
import { IntegrationsService } from "@core/features/integrations/services/integrations.service";
import { RouteAliasService } from "@core/features/routing/services/route-alias-service";
import { ButtonModule } from "primeng/button";
import { CardModule } from "primeng/card";
import { CheckboxModule } from "primeng/checkbox";
import { InputTextModule } from "primeng/inputtext";
import { PanelModule } from "primeng/panel";
import { TagModule } from "primeng/tag";
import { TextareaModule } from "primeng/textarea";
import { map, tap } from "rxjs";

@Component({
  templateUrl: "./connect.component.html",
  imports: [
    CommonModule,
    ReactiveFormsModule,
    InputTextModule,
    CheckboxModule,
    TextareaModule,
    ButtonModule,
    TagModule,
    PanelModule,
    CardModule,
    ApiResponseComponent,
  ],
})
export class ConnectComponent {
  readonly #integrationsService = inject(IntegrationsService);
  readonly #router = inject(Router);
  readonly #fb = inject(FormBuilder);
  readonly #routeAlias = inject(RouteAliasService);

  readonly provider = input.required<string>();

  readonly form = this.#fb.nonNullable.group({
    friendlyName: this.#fb.nonNullable.control<string>("New Integration", [Validators.required]),
    credentials: this.#fb.nonNullable.control<string>("", [Validators.required]),
    shareWithClient: this.#fb.nonNullable.control<boolean>(false),
  });

  readonly providerDefinition$ = computed(() => {
    return this.#integrationsService.getSupportedProviders().pipe(
      map(providers => providers.find(p => p.provider === this.provider())),
      tap(provider => {
        if (!provider) {
          throw new ErrorApiResponse([`Unsupported integration provider: ${this.provider()}`]);
        }
        this.form.controls.friendlyName.setValue(provider.displayName);
        if (!provider.isConfiguredManually) {
          this.#router.navigateByUrl(`/api/Integrations/connect/${this.provider()}`, { replaceUrl: true });
        }
      })
    );
  });

  readonly asProvider = TypeHelpers.cast<IntegrationProviderDefinition>;

  createIntegration() {
    this.#integrationsService
      .createMyIntegration({
        ...this.form.getRawValue(),
        providerKey: this.provider(),
      })
      .subscribe(integration => {
        if (!integration) {
          throw new Error("Failed to create integration");
        }
        this.#routeAlias.navigate("my-integrations");
      });
  }
}
