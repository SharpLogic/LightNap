import { ListItem } from "@core/models/list-item";

/*
 * Static content read accesses used in the application.
 */
export type StaticContentReadAccess = "Public" | "Authenticated" | "Explicit";

export const StaticContentReadAccesses = {
  Public: "Public",
  Authenticated: "Authenticated",
  Explicit: "Explicit",
} as const;

export const StaticContentReadAccessListItems = [
  new ListItem<StaticContentReadAccess>(StaticContentReadAccesses.Public, "Public", "Visible to anyone."),
  new ListItem<StaticContentReadAccess>(StaticContentReadAccesses.Authenticated, "Authenticated", "Visible to authenticated users."),
  new ListItem<StaticContentReadAccess>(StaticContentReadAccesses.Explicit, "Explicit", "Visible to explicitly granted roles and users."),
];
