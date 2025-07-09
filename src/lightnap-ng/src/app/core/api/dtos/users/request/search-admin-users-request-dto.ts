import { SearchAdminUsersSortBy } from "@core/api/search-admin-users-sort-by";
import { PagedRequestDto } from "../../paged-request-dto";

/**
 * Interface representing a request to search for admin users.
 * Extends the PaginationRequest interface to include pagination properties.
 */
export interface SearchAdminUsersRequestDto extends PagedRequestDto {
    /**
     * The email address substring to search for.
     * @type {string}
     */
    email?: string;

    /**
     * The username substring to search for.
     * @type {string}
     */
    userName?: string;

    /**
     * The field to sort the results by.
     * @type {SearchAdminUsersSortBy}
     */
    sortBy: SearchAdminUsersSortBy;

    /**
     * Whether to reverse the sort order.
     * @type {boolean}
     */
    reverseSort: boolean;
}
