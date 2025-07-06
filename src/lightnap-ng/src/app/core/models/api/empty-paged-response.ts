import { PagedResponseDto } from "./paged-response-dto";

/**
 * Represents an empty paged response. This is useful when a component requires an initial response to kickstart.
 *
 * @template T - The type of the data items.
 */
export class EmptyPagedResponse<T> implements PagedResponseDto<T> {
    /**
     * The data items in the response.
     *
     * @type {T[]}
     */
    data = [];

    /**
     * The current page number.
     *
     * @type {number}
     */
    pageNumber = 1;

    /**
     * The number of items per page.
     *
     * @type {number}
     */
    pageSize = 0;

    /**
     * The total number of items.
     *
     * @type {number}
     */
    totalCount = 0;

    /**
     * The total number of pages.
     *
     * @type {number}
     */
    totalPages = 0;
}
