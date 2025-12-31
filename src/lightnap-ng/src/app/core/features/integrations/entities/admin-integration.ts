import { AdminIntegrationDto, IntegrationDefinition } from "@core/backend-api";

export interface AdminIntegration {
  integration: AdminIntegrationDto;
  definition: IntegrationDefinition;
}
