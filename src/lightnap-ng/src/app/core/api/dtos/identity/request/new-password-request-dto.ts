/**
 * Represents a request to set a new password after receiving a reset link.
 */
export interface NewPasswordRequestDto {
    /**
     * The email address associated with the account.
     */
    email: string;

    /**
     * The new password to be set.
     */
    password: string;

    /**
     * The token used to authorize the password change.
     */
    token: string;

    /**
     * Indicates whether the user should be remembered on the device.
     */
    rememberMe: boolean;

    /**
     * Details about the device making the request.
     */
    deviceDetails: string;
}
