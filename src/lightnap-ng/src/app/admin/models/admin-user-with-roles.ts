import { AdminUserDto, RoleDto } from "./response";

/**
 * Interface representing a user with full details including roles for an administrative context.
 */
export interface AdminUserWithRoles {
    /**
     * The timestamp when the user was last modified.
     * @type {AdminUserDto}
     */
    user: AdminUserDto;

    /**
     * The timestamp when the user was created.
     * @type {Array<RoleDto>}
     */
    roles: Array<RoleDto>;
}
