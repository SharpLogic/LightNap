/*
 * Static content visibilities for the current user's status.
 */
export type StaticContentUserVisibility = "RequiresAuthentication" | "Restricted" | "Reader" | "Editor";

export const StaticContentUserVisibilities = {
  RequiresAuthentication: "RequiresAuthentication",
  Restricted: "Restricted",
  Reader: "Reader",
  Editor: "Editor",
} as const;
