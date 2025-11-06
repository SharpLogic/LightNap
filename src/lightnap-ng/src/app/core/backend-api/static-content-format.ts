import { ListItem } from "@core/models";

/*
 * Static content formats used in the application.
 */
export type StaticContentFormat = "Html" | "Markdown" | "PlainText";

export const StaticContentFormats = {
  Html: "Html",
  Markdown: "Markdown",
  PlainText: "PlainText",
} as const;

export const StaticContentFormatListItems: Array<ListItem<StaticContentFormat>> = [
  new ListItem<StaticContentFormat>(StaticContentFormats.Html, "HTML", "Render as HTML."),
  new ListItem<StaticContentFormat>(StaticContentFormats.Markdown, "Markdown", "Render as Markdown."),
  new ListItem<StaticContentFormat>(StaticContentFormats.PlainText, "Plain Text", "Render as plain text."),
];
