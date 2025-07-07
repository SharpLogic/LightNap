import { PaginationRequestDto } from "@core";

/**
 * Represents a request to search for admin claims with optional filtering and pagination.
 *
 * @extends PaginationRequestDto
 *
 * @property {string} [userId] - The email address substring to search for.
 * @property {string} [type] - The username substring to search for.
 * @property {string} [value] - The value to search for in the claims.
 */
export interface SearchAdminClaimsRequestDto extends PaginationRequestDto {
    /**
     * The user ID to filter claims by.
     * @type {string}
     */
    userId?: string;

    /**
     * The type to filter claims by.
     * @type {string}
     */
    type?: string;

    /**
     * The value to filter claims by.
     * @type {string}
     */
    value?: string;
}
