import { PrivilegedUserDto } from "./privileged-user-dto";

/**
 * Interface representing a user with full details for an administrative context.
 */
export interface AdminUserDto extends PrivilegedUserDto {
    /**
     * The timestamp when the user was last modified.
     * @type {number}
     */
    lastModifiedDate: number;

    /**
     * The timestamp when the user lockout ends. If the user is not locked out, this value is undefined.
     * @type {number}
     */
    lockoutEnd: number;
}
