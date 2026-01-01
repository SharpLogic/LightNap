import { inject, Injectable } from "@angular/core";
import {
    CreateIntegrationRequestDto,
    IntegrationCategory,
    IntegrationFeature,
    PagedResponseDto,
    SearchIntegrationsRequestDto,
    SupportedIntegrationsDto,
    UpdateIntegrationRequestDto,
} from "@core/backend-api";
import { LightNapWebApiService } from "@core/backend-api/services/lightnap-api";
import { forkJoin, map, Observable, shareReplay } from "rxjs";
import { AdminIntegration, Integration } from "../entities";

/**
 * Service for public functionality any user can access, even if they're not logged in.
 */
@Injectable({
  providedIn: "root",
})
export class IntegrationsService {
  #webApiService = inject(LightNapWebApiService);
  #supportedIntegrations$: Observable<SupportedIntegrationsDto> | null = null;

  getSupportedIntegrations() {
    if (!this.#supportedIntegrations$) {
      this.#supportedIntegrations$ = this.#webApiService.getSupportedIntegrations().pipe(
        map(integrations => integrations || []),
        shareReplay({ bufferSize: 1, refCount: false })
      );
    }
    return this.#supportedIntegrations$;
  }

  getSupportedCategories() {
    return this.getSupportedIntegrations().pipe(map(integrations => integrations.categories));
  }

  getSupportedFeatures() {
    return this.getSupportedIntegrations().pipe(map(integrations => integrations.features));
  }

  getSupportedProviders() {
    return this.getSupportedIntegrations().pipe(map(integrations => integrations.providers));
  }

  getSupportedIntegrationsByFeature(feature: IntegrationFeature) {
    return this.getSupportedIntegrations().pipe(
      map(supported => supported.providers.filter(integration => integration.features.find(f => f === feature)))
    );
  }

  getSupportedIntegrationsByCategory(category: IntegrationCategory) {
    return this.getSupportedIntegrations().pipe(
      map(supported => {
        const categoryDefinition = supported.categories.find(c => c.category === category);
        if (!categoryDefinition) {
          return [];
        }
        return supported.providers.filter(integration => integration.features.some(f => categoryDefinition.features.includes(f)));
      })
    );
  }

  getMyIntegrations() {
    return forkJoin([this.#webApiService.getMyIntegrations(), this.getSupportedIntegrations()]).pipe(
      map(([integrationDtos, supported]) => {
        return integrationDtos!.map(
          integrationDto =>
            <Integration>{
              integration: integrationDto,
              definition: supported.providers.find(definition => definition.provider === integrationDto.provider)!,
            }
        );
      })
    );
  }

  searchIntegrations(searchIntegrationsRequestDto: SearchIntegrationsRequestDto) {
    return forkJoin([this.#webApiService.searchIntegrations(searchIntegrationsRequestDto), this.getSupportedIntegrations()]).pipe(
      map(
        ([results, supported]) =>
          <PagedResponseDto<AdminIntegration>>{
            ...results!,
            data: results!.data.map(
              adminIntegrationDto =>
                <AdminIntegration>{
                  integration: adminIntegrationDto,
                  definition: supported.providers.find(definition => definition.provider === adminIntegrationDto.provider)!,
                }
            ),
          }
      )
    );
  }

  createMyIntegration(createIntegrationRequestDto: CreateIntegrationRequestDto) {
    return this.#webApiService.createMyIntegration(createIntegrationRequestDto);
  }

  updateMyIntegration(integrationId: number, updateIntegrationRequestDto: UpdateIntegrationRequestDto) {
    return this.#webApiService.updateMyIntegration(integrationId, updateIntegrationRequestDto);
  }

  deleteIntegration(integrationId: number) {
    return this.#webApiService.deleteIntegration(integrationId);
  }

  deleteMyIntegration(integrationId: number) {
    return this.#webApiService.deleteMyIntegration(integrationId);
  }
}
