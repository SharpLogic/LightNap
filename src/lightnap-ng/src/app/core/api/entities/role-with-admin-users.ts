import { RoleDto, AdminUserDto } from "../dtos";

export interface RoleWithAdminUsers {
    role: RoleDto;
    users: Array<AdminUserDto>;
}
