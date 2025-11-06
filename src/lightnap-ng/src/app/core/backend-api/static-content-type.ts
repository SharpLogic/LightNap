import { ListItem } from "@core/models/list-item";

/*
 * Static content types used in the application.
 */
export type StaticContentType = "Zone" | "Page";

export const StaticContentTypes = {
  Zone: "Zone",
  Page: "Page",
} as const;

export const StaticContentTypeListItems = [
  new ListItem<StaticContentType>(StaticContentTypes.Page, "Page", "Full page content with a URL."),
  new ListItem<StaticContentType>(StaticContentTypes.Zone, "Zone", "Content for a zone within another page."),
];
