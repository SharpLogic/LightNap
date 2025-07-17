import { PrivilegedSearchUsersRequestDto } from "./privileged-search-users-request-dto";

/**
 * Interface representing a request to search for admin users.
 * Extends the PrivilegedSearchUsersRequestDto interface to add additionally supported properties.
 */
export interface AdminSearchUsersRequestDto extends PrivilegedSearchUsersRequestDto {}
