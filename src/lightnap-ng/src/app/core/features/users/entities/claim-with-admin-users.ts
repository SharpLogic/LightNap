import { AdminUserDto, ClaimDto } from "@core/backend-api";

export interface ClaimWithAdminUsers {
    claim: ClaimDto;
    users: Array<AdminUserDto>;
}
