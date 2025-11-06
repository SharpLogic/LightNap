import { StaticContentReadAccess, StaticContentStatus, StaticContentType } from "@core";

/**
 * Static content details.
 */
export interface StaticContentDto {
  id: number;
  type: StaticContentType;
  key: string;
  status: StaticContentStatus;
  readAccess: StaticContentReadAccess;
  editorRoles?: string;
  readerRoles?: string;
  createdDate: Date;
  createdByUserId?: string;
  statusChangedDate?: Date;
  statusChangedByUserId?: string;
  lastModifiedDate?: Date;
  lastModifiedByUserId?: string;
}
