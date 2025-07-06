import { LoginType as LoginTypeDto } from "./login-type";

/**
 * Represents a request to log in to the application.
 */
export interface LoginRequest {
    /**
     * The type of login field used in the request.
     */
    type?: LoginTypeDto;

    /**
     * The login field for the user attempting to log in.
     */
    login: string;

    /**
     * The password of the user attempting to log in.
     */
    password: string;

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
