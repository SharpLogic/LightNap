/*
 * Static content read accesses used in the application.
 */
export type StaticContentReadAccesses = "Public" | "Authenticated" | "Explicit";

export const StaticContentReadAccess = {
  Public: "Public",
  Authenticated: "Authenticated",
  Explicit: "Explicit",
} as const;
