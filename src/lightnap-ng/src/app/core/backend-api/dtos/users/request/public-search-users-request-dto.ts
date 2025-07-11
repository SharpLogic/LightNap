import { PagedRequestDto } from "../../paged-request-dto";
import { SearchUsersSortBy } from "./search-users-sort-by";

/**
 * Interface representing a request to search for users.
 * Extends the PaginationRequest interface to include pagination properties.
 */
export interface PublicSearchUsersRequestDto extends PagedRequestDto {
    /**
     * The username substring to search for.
     * @type {string}
     */
    userName?: string;

    /**
     * The field to sort the results by.
     * @type {SearchUsersSortBy}
     */
    sortBy: SearchUsersSortBy;

    /**
     * Whether to reverse the sort order.
     * @type {boolean}
     */
    reverseSort: boolean;
}
