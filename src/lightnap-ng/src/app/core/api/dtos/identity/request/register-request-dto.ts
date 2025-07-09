/**
 * Represents a request to register a new user.
 */
export interface RegisterRequestDto {
    /**
     * The username of the user.
     */
    userName: string;

    /**
     * The email address of the user.
     */
    email: string;

    /**
     * The password chosen by the user.
     */
    password: string;

    /**
     * The confirmation of the password.
     */
    confirmPassword: string;

    /**
     * Indicates whether the user should be remembered on the device.
     */
    rememberMe: boolean;

    /**
     * Details about the device being used for registration.
     */
    deviceDetails: string;
}
