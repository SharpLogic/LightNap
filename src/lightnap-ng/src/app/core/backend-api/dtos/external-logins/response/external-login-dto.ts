/**
 * DTO representing an external login.
 */
export interface ExternalLoginDto {
    /** The name of the external login provider. */
    loginProvider: string;
    /** The key provided by the external login provider. */
    providerKey: string;
    /** The display name of the external login provider. */
    providerDisplayName: string;
}

