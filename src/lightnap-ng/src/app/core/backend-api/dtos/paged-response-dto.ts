/**
 * Represents a paginated response from an API.
 *
 * @template T - The type of the items in the paginated response.
 */
export interface PagedResponseDto<T> {
    /**
     * The array of items for the current page.
     */
    data: Array<T>;

    /**
     * The current page number.
     */
    pageNumber: number;

    /**
     * The number of items per page.
     */
    pageSize: number;

    /**
     * The total number of items across all pages.
     */
    totalCount: number;

    /**
     * The total number of pages.
     */
    totalPages: number;
}
