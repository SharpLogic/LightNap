import { StaticContentUserVisibilities } from "@core/backend-api/static-content-user-visibilities";
import { PublishedStaticContentDto } from "./published-static-content-dto";

/**
 * The result of published static content request.
 */
export interface PublishedStaticContentResultDto {
    /**
     * The visibility for the current user.
     */
    visibility: StaticContentUserVisibilities;

    /**
     * The content.
     */
    content?: PublishedStaticContentDto;
}
