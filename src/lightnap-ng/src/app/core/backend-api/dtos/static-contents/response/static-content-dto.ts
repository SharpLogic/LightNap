import { StaticContentReadAccesses } from "@core/backend-api/static-content-read-accesses";
import { StaticContentStatuses } from "@core/backend-api/static-content-statuses";
import { StaticContentTypes } from "@core/backend-api/static-content-types";

/**
 * Static content details.
 */
export interface StaticContentDto {
  id: number;
  type: StaticContentTypes;
  key: string;
  status: StaticContentStatuses;
  readAccess: StaticContentReadAccesses;
  editorRoles?: string;
  viewerRoles?: string;
  createdDate: Date;
  createdByUserId?: string;
  statusChangedDate?: Date;
  statusChangedByUserId?: string;
  lastModifiedDate?: Date;
  lastModifiedByUserId?: string;
}
