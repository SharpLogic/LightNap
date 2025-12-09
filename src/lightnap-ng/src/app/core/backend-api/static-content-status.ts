import { ListItem } from "@core/models/list-item";
import { StaticContentStatus } from "./models";

export const StaticContentStatusListItems = [
  new ListItem<StaticContentStatus>(StaticContentStatus.Draft, "Draft", "Content is not visible to end users."),
  new ListItem<StaticContentStatus>(StaticContentStatus.Published, "Published", "Published content visible to allowed users."),
  new ListItem<StaticContentStatus>(StaticContentStatus.Archived, "Archived", "Archived and not visible to end users."),
];
