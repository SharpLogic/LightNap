import { AdminIntegrationDto, IntegrationProviderDefinition } from "@core/backend-api";

export interface AdminIntegration {
  integration: AdminIntegrationDto;
  definition: IntegrationProviderDefinition;
}
