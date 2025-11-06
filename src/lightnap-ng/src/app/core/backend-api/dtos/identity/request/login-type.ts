/**
 * Indicates the type of login field used in a login request.
 */
export type LoginType = "Email" | "UserName" | "MagicLink" | "Unknown";

export const LoginTypes = {
    Email: "Email",
    UserName: "UserName",
    MagicLink: "MagicLink",
    Unknown: "Unknown",
} as const;
