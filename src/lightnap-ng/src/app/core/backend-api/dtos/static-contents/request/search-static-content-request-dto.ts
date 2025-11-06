import { PagedRequestDto, StaticContentReadAccess, StaticContentSortBy, StaticContentStatus, StaticContentType } from "@core/backend-api";

/**
 * Represents a request to search for static contents with specific criteria.
 */
export interface SearchStaticContentRequestDto extends PagedRequestDto {
  keyContains?: string;
  readAccess?: StaticContentReadAccess;
  status?: StaticContentStatus;
  type?: StaticContentType;
  sortBy: StaticContentSortBy;
  reverseSort: boolean;
}
