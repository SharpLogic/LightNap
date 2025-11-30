/**
 * Represents the possible outcomes of a successful external login attempt.
 *
 * @typedef {("AlreadyLinked" | "NewAccountLink" | "RequiresRegistration")} ExternalLoginSuccessType
 *
 * @property {"AlreadyLinked"} AlreadyLinked - Indicates that the external account is already linked to an existing user account, so this was just a plain login.
 * @property {"NewAccountLink"} NewAccountLink - Indicates that the existing user account was linked to the external account, so there is already a user logged in.
 * @property {"RequiresRegistration"} RequiresRegistration - Indicates that additional registration steps are required before linking the external account.
 */
export type ExternalLoginSuccessType = "AlreadyLinked" | "NewAccountLink" | "RequiresRegistration";

export const ExternalLoginSuccessTypes = {
    AlreadyLinked: "AlreadyLinked",
    NewAccountLink: "NewAccountLink",
    RequiresRegistration: "RequiresRegistration",
} as const;
