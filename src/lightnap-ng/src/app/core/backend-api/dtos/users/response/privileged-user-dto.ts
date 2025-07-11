import { PublicUserDto } from "./public-user-dto";

/**
 * Interface representing a user with expanded details for a privileged context.
 */
export interface PrivilegedUserDto extends PublicUserDto {
    /**
     * The email address of the user.
     * @type {string}
     */
    email: string;
}
