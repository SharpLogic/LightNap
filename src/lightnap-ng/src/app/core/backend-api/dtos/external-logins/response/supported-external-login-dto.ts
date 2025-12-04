/**
 * DTO representing a supported external login option.
 */
export interface SupportedExternalLoginDto {
    /** The unique name of the external login provider. */
    providerName: string;
    /** The display name of the external login provider. */
    displayName: string;
    /** The URL to initiate the external login process. */
    loginUrl: string;
}
