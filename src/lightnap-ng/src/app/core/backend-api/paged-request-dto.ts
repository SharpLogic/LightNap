/**
 * Represents a request for paginated data.
 */
export interface PagedRequestDto {
  /**
   * The page number to retrieve.
   * @remarks
   * This is an optional parameter. If not provided, a default value may be used.
   */
  pageNumber?: number;

  /**
   * The number of items per page.
   * @remarks
   * This is an optional parameter. If not provided, a default value may be used.
   */
  pageSize?: number;
}
