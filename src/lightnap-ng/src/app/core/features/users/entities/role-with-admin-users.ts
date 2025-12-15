import { RoleDto, AdminUserDto } from "@core/backend-api";

export interface RoleWithAdminUsers {
  role: RoleDto;
  users: Array<AdminUserDto>;
}
