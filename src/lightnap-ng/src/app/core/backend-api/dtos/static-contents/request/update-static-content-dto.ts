import { StaticContentReadAccesses, StaticContentStatuses } from "@core/backend-api";

/**
 * Represents a request to update a static content.
 */
export interface UpdateStaticContentDto {
    key: string;
    status: StaticContentStatuses;
    readAccess: StaticContentReadAccesses;
    editorRoles?: string;
    viewerRoles?: string;
}
