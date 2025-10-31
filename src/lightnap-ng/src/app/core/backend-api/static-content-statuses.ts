/*
 * Static content statuses used in the application.
 */
export type StaticContentStatuses = "Draft" | "Published" | "Archived";

export const StaticContentStatus = {
  Draft: "Draft",
  Published: "Published",
  Archived: "Archived",
} as const;
