import { ListItem } from "@core/models/list-item";

/*
 * Static content types used in the application.
 */
export type StaticContentTypes = "Zone" | "Page";

export const StaticContentType = {
  Zone: "Zone",
  Page: "Page",
} as const;

export const StaticContentTypeListItems = [
  new ListItem<StaticContentTypes>(StaticContentType.Page, "Page", "Full page content with a URL."),
  new ListItem<StaticContentTypes>(StaticContentType.Zone, "Zone", "Content for a zone within another page."),
];
