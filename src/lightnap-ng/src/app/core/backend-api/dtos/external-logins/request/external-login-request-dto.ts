
/**
 * Represents a request to complete an external login to the application.
 */
export interface ExternalLoginRequestDto {
    /**
     * Indicates whether the user should be remembered on the device.
     */
    rememberMe: boolean;

    /**
     * Details about the device from which the login request is made, such as the user agent,
     * so that the user can recognize it later on if they want to revoke the associated token.
     */
    deviceDetails: string;
}
