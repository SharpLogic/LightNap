import { ClaimDto } from "./claim-dto";

/**
 * Extends the ClaimDto to include additional properties for administrative claims.
 */
export interface UserClaimDto extends ClaimDto {
    /**
     * The user ID the claim belongs to.
     */
    userId: string;
}
