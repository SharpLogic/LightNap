import { ExternalLoginRequestDto } from "./external-login-request-dto";

/**
 * Represents a request to register a new user.
 */
export interface ExternalLoginRegisterRequestDto extends ExternalLoginRequestDto {
    /**
     * The username of the user.
     */
    userName: string;

    /**
     * The email address of the user.
     */
    email: string;
}
