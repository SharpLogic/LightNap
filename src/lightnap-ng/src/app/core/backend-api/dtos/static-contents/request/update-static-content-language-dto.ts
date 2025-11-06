import { StaticContentFormat } from "@core/backend-api/static-content-format";

/**
 * Represents a request to update the content for a specific language.
 */
export interface UpdateStaticContentLanguageDto {
    content: string;
    format: StaticContentFormat;
}
