import { ClaimDto } from "@identity";
import { AdminUserDto } from "./response";

export interface ClaimWithAdminUsers {
    claim: ClaimDto;
    users: Array<AdminUserDto>;
}
