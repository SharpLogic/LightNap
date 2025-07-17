import { RoleDto, AdminUserDto } from "../../backend-api/dtos";

export interface RoleWithAdminUsers {
    role: RoleDto;
    users: Array<AdminUserDto>;
}
