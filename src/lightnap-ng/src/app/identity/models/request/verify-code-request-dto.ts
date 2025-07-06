/**
 * Represents a request to verify a two-factor code.
 */
export interface VerifyCodeRequestDto {
    /**
     * The email address or user name associated with the verification request.
     */
    login: string;

    /**
     * The verification code to be validated.
     */
    code: string;

    /**
     * Indicates whether the user should be remembered on the device.
     */
    rememberMe: boolean;

    /**
     * Details about the device from which the request is made.
     */
    deviceDetails: string;
}
