/*
 * Role names used in the application. These need to be kept in sync with the backend.
 */
export type RoleNames = "Administrator" | "ContentEditor";

export const RoleName = {
  Administrator: "Administrator",
  ContentEditor: "ContentEditor",
} as const;
