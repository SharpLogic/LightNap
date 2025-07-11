import { LoginSuccessType } from "./login-success-type";

/**
 * Represents the result of a login attempt.
 */
export interface LoginSuccessResultDto {
    /**
     * Whether the response includes a token or if further steps are required
     */
    type: LoginSuccessType;

    /**
     * The bearer token received upon successful login, if two-factor authentication is not required.
     */
    accessToken?: string;
}
