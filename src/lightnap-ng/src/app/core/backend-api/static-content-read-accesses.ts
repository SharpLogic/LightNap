import { ListItem } from "@core/models/list-item";

/*
 * Static content read accesses used in the application.
 */
export type StaticContentReadAccesses = "Public" | "Authenticated" | "Explicit";

export const StaticContentReadAccess = {
  Public: "Public",
  Authenticated: "Authenticated",
  Explicit: "Explicit",
} as const;

export const StaticContentReadAccessListItems = [
  new ListItem<StaticContentReadAccesses>(StaticContentReadAccess.Public, "Public", "Visible to anyone."),
  new ListItem<StaticContentReadAccesses>(StaticContentReadAccess.Authenticated, "Authenticated", "Visible to authenticated users."),
  new ListItem<StaticContentReadAccesses>(StaticContentReadAccess.Explicit, "Explicit", "Visible to explicitly granted roles and users."),
];
