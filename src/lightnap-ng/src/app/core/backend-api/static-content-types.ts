/*
 * Static content types used in the application.
 */
export type StaticContentTypes = "Zone" | "Page";

export const StaticContentType = {
  Zone: "Zone",
  Page: "Page",
} as const;
