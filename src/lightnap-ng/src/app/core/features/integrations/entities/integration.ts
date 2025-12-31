import { IntegrationDefinition, IntegrationDto } from "@core/backend-api";

export interface Integration {
  integration: IntegrationDto;
  definition: IntegrationDefinition;
}
