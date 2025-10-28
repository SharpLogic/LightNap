import { StaticContentFormats } from "@core/backend-api/static-content-formats";

/**
 * Static content for rendering.
 */
export interface PublishedStaticContentDto {
    /**
     * The content.
     */
    content: string;

    /**
     * The format of the content.
     */
    format: StaticContentFormats;
}
