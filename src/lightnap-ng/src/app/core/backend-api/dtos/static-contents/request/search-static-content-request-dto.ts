import { PagedRequestDto, StaticContentSortBys } from "@core/backend-api";
import { StaticContentStatuses } from "@core/backend-api/static-content-statuses";
import { StaticContentTypes } from "@core/backend-api/static-content-types";

/**
 * Represents a request to search for static contents with specific criteria.
 */
export interface SearchStaticContentRequestDto extends PagedRequestDto {
  keyContains?: string;
  status?: StaticContentStatuses;
  type?: StaticContentTypes;
  sortBy: StaticContentSortBys;
  reverseSort: boolean;
}
