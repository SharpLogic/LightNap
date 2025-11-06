import { StaticContentReadAccess } from "@core/backend-api/static-content-read-access";
import { StaticContentStatus } from "@core/backend-api/static-content-status";
import { StaticContentType } from "@core/backend-api/static-content-type";

/**
 * Represents a request to update a static content.
 */
export interface UpdateStaticContentDto {
    key: string;
    status: StaticContentStatus;
    type: StaticContentType;
    readAccess: StaticContentReadAccess;
    editorRoles?: string;
    readerRoles?: string;
}
