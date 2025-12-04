import { ExternalLoginSuccessType } from "./external-login-success-type";

/**
 * Represents the result of an external login attempt.
 */
export interface ExternalLoginSuccessResultDto {
    /**
     * Next steps for the user.
     */
    type: ExternalLoginSuccessType;

    /**
     * The email address associated with the external login, if provided.
     */
    email?: string;

    /**
     * The user name associated with the external login, if provided.
     */
    userName?: string;
}
