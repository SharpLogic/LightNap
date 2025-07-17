import { PublicSearchUsersRequestDto } from "./public-search-users-request-dto";

/**
 * Interface representing a request to search for privileged users.
 * Extends the PublicSearchUsersRequestDto interface to add additionally supported properties.
 */
export interface PrivilegedSearchUsersRequestDto extends PublicSearchUsersRequestDto {
    /**
     * The email address substring to search for.
     * @type {string}
     */
    email?: string;
}
