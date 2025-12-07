import { ExternalLoginDto } from "./external-login-dto";

/**
 * DTO representing an external login associated with a specific user in an admin context.
 */
export interface AdminExternalLoginDto extends ExternalLoginDto {
    /** The unique identifier of the user associated with the external login. */
    userId: string;
}

