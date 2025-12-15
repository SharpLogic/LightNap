import { ListItem } from "@core/models/list-item";
import { StaticContentType } from "./models";

export const StaticContentTypeListItems = [
  new ListItem<StaticContentType>(StaticContentType.Page, "Page", "Full page content with a URL."),
  new ListItem<StaticContentType>(StaticContentType.Zone, "Zone", "Content for a zone within another page."),
];
