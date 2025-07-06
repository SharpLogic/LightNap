import { AdminUserDto, RoleDto } from "./response";

export interface RoleWithAdminUsers {
    role: RoleDto;
    users: Array<AdminUserDto>;
}
