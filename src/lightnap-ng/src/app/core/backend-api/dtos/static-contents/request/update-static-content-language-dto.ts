import { StaticContentFormats } from "@core/backend-api/static-content-formats";

/**
 * Represents a request to update the content for a specific language.
 */
export interface UpdateStaticContentLanguageDto {
    content: string;
    format: StaticContentFormats;
}
