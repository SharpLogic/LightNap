import { SearchClaimsRequestDto } from "./search-claims-request-dto";


/**
 * Represents a request to search for user claims with optional filtering and pagination.
 *
 * @extends SearchClaimsRequestDto
 *
 * @property {string} [userId] - The email address substring to search for.
 */
export interface SearchUserClaimsRequestDto extends SearchClaimsRequestDto {
   /**
     * The value to filter claims by.
     * @type {string}
     */
    userId?: string;
}
