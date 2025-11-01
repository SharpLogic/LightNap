import { ListItem } from "@core/models/list-item";

/*
 * Static content statuses used in the application.
 */
export type StaticContentStatuses = "Draft" | "Published" | "Archived";

export const StaticContentStatus = {
  Draft: "Draft",
  Published: "Published",
  Archived: "Archived",
} as const;

export const StaticContentStatusListItems = [
  new ListItem<StaticContentStatuses>(StaticContentStatus.Draft, "Draft", "Content is not visible to end users."),
  new ListItem<StaticContentStatuses>(StaticContentStatus.Published, "Published", "Published content visible to allowed users."),
  new ListItem<StaticContentStatuses>(StaticContentStatus.Archived, "Archived", "Archived and not visible to end users."),
];
