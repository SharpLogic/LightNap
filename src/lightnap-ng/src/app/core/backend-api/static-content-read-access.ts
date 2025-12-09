import { ListItem } from "@core/models/list-item";
import { StaticContentReadAccess } from "./models";

export const StaticContentReadAccessListItems = [
  new ListItem<StaticContentReadAccess>(StaticContentReadAccess.Public, "Public", "Visible to anyone."),
  new ListItem<StaticContentReadAccess>(StaticContentReadAccess.Authenticated, "Authenticated", "Visible to authenticated users."),
  new ListItem<StaticContentReadAccess>(StaticContentReadAccess.Explicit, "Explicit", "Visible to explicitly granted roles and users."),
];
