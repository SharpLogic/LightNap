/*
 * Role names used in the application. These should be kept in sync with the backend to make frontend
 * development easier. However, it's important to note that the backend is the source of truth for role
 * names and permissions. For example, the admin UI loads the roles from the backend when providing options.
 */
export type RoleNames = "Administrator" | "ContentEditor";

export const RoleName = {
  Administrator: "Administrator",
  ContentEditor: "ContentEditor",
} as const;
