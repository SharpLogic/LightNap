import { StaticContentUserVisibility } from "@core/backend-api/static-content-user-visibility";
import { PublishedStaticContentDto } from "./published-static-content-dto";

/**
 * The result of published static content request.
 */
export interface PublishedStaticContentResultDto {
    /**
     * The visibility for the current user.
     */
    visibility: StaticContentUserVisibility;

    /**
     * The content.
     */
    content?: PublishedStaticContentDto;
}
