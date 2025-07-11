import { AdminUserDto, ClaimDto } from "../dtos";

export interface ClaimWithAdminUsers {
    claim: ClaimDto;
    users: Array<AdminUserDto>;
}
