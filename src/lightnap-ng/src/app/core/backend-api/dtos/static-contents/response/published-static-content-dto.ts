import { StaticContentFormat } from "@core/backend-api/static-content-format";

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
    format: StaticContentFormat;
}
