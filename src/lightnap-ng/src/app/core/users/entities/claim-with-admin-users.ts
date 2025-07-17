import { AdminUserDto, ClaimDto } from "../../backend-api/dtos";

export interface ClaimWithAdminUsers {
    claim: ClaimDto;
    users: Array<AdminUserDto>;
}
