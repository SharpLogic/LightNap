/**
 * Indicates the type of login field used in a login request.
 */
export type LoginTypes = "Email" | "UserName" | "MagicLink" | "Unknown";

export const LoginType = {
    Email: "Email",
    UserName: "UserName",
    MagicLink: "MagicLink",
    Unknown: "Unknown",
} as const;
