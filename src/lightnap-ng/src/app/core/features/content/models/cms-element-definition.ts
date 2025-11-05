import { Type } from "@angular/core";

export interface CmsElementDefinition {
  tagName: string;
  component: Type<any>;
  displayName: string;
  description: string;
  inputs?: Array<{
    name: string;
    type: string;
    description: string;
    required?: boolean;
    default?: string;
  }>;
  example?: string;
}
