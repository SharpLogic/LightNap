import { ListItem } from "@core/models";
import { StaticContentFormat } from "./models";

export const StaticContentFormatListItems: Array<ListItem<StaticContentFormat>> = [
  new ListItem<StaticContentFormat>(StaticContentFormat.Html, "HTML", "Render as HTML."),
  new ListItem<StaticContentFormat>(StaticContentFormat.Markdown, "Markdown", "Render as Markdown."),
  new ListItem<StaticContentFormat>(StaticContentFormat.PlainText, "Plain Text", "Render as plain text."),
];
