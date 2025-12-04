import { PagedRequestDto } from "../../paged-request-dto";

/**
 * Interface representing a request to search for external logins with optional filtering criteria.
 * Extends the PagedRequestDto interface to include pagination properties.
 */
export interface SearchExternalLoginsRequestDto extends PagedRequestDto {
    /**
     * Optional filter for the login provider (e.g., Google, Facebook).
     */
    loginProvider?: string;
    /**
     * Optional filter for the user ID associated with the external login.
     */
    userId?: string;
}
