import { ListItem } from "@core/models/list-item";

/*
 * Static content statuses used in the application.
 */
export type StaticContentStatus = "Draft" | "Published" | "Archived";

export const StaticContentStatuses = {
  Draft: "Draft",
  Published: "Published",
  Archived: "Archived",
} as const;

export const StaticContentStatusListItems = [
  new ListItem<StaticContentStatus>(StaticContentStatuses.Draft, "Draft", "Content is not visible to end users."),
  new ListItem<StaticContentStatus>(StaticContentStatuses.Published, "Published", "Published content visible to allowed users."),
  new ListItem<StaticContentStatus>(StaticContentStatuses.Archived, "Archived", "Archived and not visible to end users."),
];
