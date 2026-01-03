import { inject, Injectable } from "@angular/core";
import {
  ConfirmIntegrationRequestDto,
  CreateIntegrationRequestDto,
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

  getSupportedProviders() {
    return this.getSupportedIntegrations().pipe(map(integrations => integrations.providers));
  }

  getMyIntegrations() {
    return forkJoin([this.#webApiService.getMyIntegrations(), this.getSupportedIntegrations()]).pipe(
      map(([integrationDtos, supported]) => {
        return integrationDtos!.map(
          integrationDto =>
            <Integration>{
              integration: integrationDto,
              definition: supported.providers.find(definition => definition.provider === integrationDto.providerKey)!,
            }
        );
      })
    );
  }

  getMyIntegration(integrationId: number) {
    return this.getMyIntegrations().pipe(map(integrations => integrations.find(i => i.integration.id === integrationId) || null));
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
                  definition: supported.providers.find(definition => definition.provider === adminIntegrationDto.providerKey)!,
                }
            ),
          }
      )
    );
  }

  createMyIntegration(createIntegrationRequestDto: CreateIntegrationRequestDto) {
    return this.#webApiService.createMyIntegration(createIntegrationRequestDto);
  }

  confirmIntegration(confirmIntegrationRequestDto: ConfirmIntegrationRequestDto) {
    return this.#webApiService.confirmIntegration(confirmIntegrationRequestDto);
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
