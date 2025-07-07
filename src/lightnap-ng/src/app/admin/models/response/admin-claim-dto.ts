import { ClaimDto } from "@identity";

/**
 * Extends the ClaimDto to include additional properties for administrative claims.
 */
export interface AdminClaimDto extends ClaimDto {
    /**
     * The user ID the claim belongs to.
     */
    userId: string;
}
