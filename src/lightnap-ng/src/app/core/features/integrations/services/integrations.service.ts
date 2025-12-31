import { inject, Injectable } from "@angular/core";
import {
    CreateIntegrationRequestDto,
    IntegrationCategory,
    IntegrationCategoryDefinition,
    IntegrationDefinition,
    IntegrationService,
    PagedResponseDto,
    SearchIntegrationsRequestDto,
    UpdateIntegrationRequestDto
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
  #supportedIntegrations$: Observable<Array<IntegrationDefinition>> | null = null;
  #supportedIntegrationCategories$: Observable<Array<IntegrationCategoryDefinition>> | null = null;

  getSupportedIntegrations() {
    if (!this.#supportedIntegrations$) {
      this.#supportedIntegrations$ = this.#webApiService.getSupportedIntegrations().pipe(
        map(integrations => integrations || []),
        shareReplay({ bufferSize: 1, refCount: false })
      );
    }
    return this.#supportedIntegrations$;
  }

  getSupportedIntegrationCategories() {
    if (!this.#supportedIntegrationCategories$) {
      this.#supportedIntegrationCategories$ = this.#webApiService.getSupportedIntegrationCategories().pipe(
        map(categories => categories || []),
        shareReplay({ bufferSize: 1, refCount: false })
      );
    }
    return this.#supportedIntegrationCategories$;
  }

  getSupportedIntegrationsByService(service: IntegrationService) {
    return this.getSupportedIntegrations().pipe(
      map(integrations => integrations.filter(integration => integration.services.find(s => s === service)))
    );
  }

  getSupportedIntegrationsByCategory(category: IntegrationCategory) {
    return forkJoin([this.getSupportedIntegrations(), this.getSupportedIntegrationCategories()]).pipe(
      map(([integrations, categories]) => {
        const categoryDefinition = categories.find(c => c.category === category);
        if (!categoryDefinition) {
          return [];
        }
        return integrations.filter(integration => integration.services.some(s => categoryDefinition.services.includes(s)));
      })
    );
  }

  getMyIntegrations() {
    return forkJoin([this.#webApiService.getMyIntegrations(), this.getSupportedIntegrations()]).pipe(
      map(([integrationDtos, integrationDefinitions]) => {
        return integrationDtos!.map(
          integrationDto =>
            <Integration>{
              integration: integrationDto,
              definition: integrationDefinitions.find(definition => definition.type === integrationDto.provider)!,
            }
        );
      })
    );
  }

  searchIntegrations(searchIntegrationsRequestDto: SearchIntegrationsRequestDto) {
    return forkJoin([this.#webApiService.searchIntegrations(searchIntegrationsRequestDto), this.getSupportedIntegrations()]).pipe(
      map(
        ([results, integrationDefinitions]) =>
          <PagedResponseDto<AdminIntegration>>{
            ...results!,
            data: results!.data.map(
              adminIntegrationDto =>
                <AdminIntegration>{
                  integration: adminIntegrationDto,
                  definition: integrationDefinitions.find(definition => definition.type === adminIntegrationDto.provider)!,
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
