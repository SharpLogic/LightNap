import { IntegrationProviderDefinition, IntegrationDto } from "@core/backend-api";

export interface Integration {
  integration: IntegrationDto;
  definition: IntegrationProviderDefinition;
}
