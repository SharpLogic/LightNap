import { StaticContentReadAccesses, StaticContentStatuses, StaticContentTypes } from "@core/backend-api";

/**
 * Represents a request to update a static content.
 */
export interface UpdateStaticContentDto {
    key: string;
    status: StaticContentStatuses;
    type: StaticContentTypes;
    readAccess: StaticContentReadAccesses;
    editorRoles?: string;
    viewerRoles?: string;
}
