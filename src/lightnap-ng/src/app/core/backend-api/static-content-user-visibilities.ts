/*
 * Static content visibilities for the current user's status.
 */
export type StaticContentUserVisibilities = "RequiresAuthentication" | "Restricted" | "Reader" | "Editor";

export const StaticContentUserVisibility = {
  RequiresAuthentication: "RequiresAuthentication",
  Restricted: "Restricted",
  Reader: "Reader",
  Editor: "Editor",
} as const;
