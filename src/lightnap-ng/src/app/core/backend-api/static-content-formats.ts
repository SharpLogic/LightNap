import { ListItem } from "@core/models";

/*
 * Static content formats used in the application.
 */
export type StaticContentFormats = "Html" | "Markdown" | "PlainText";

export const StaticContentFormat = {
  Html: "Html",
  Markdown: "Markdown",
  PlainText: "PlainText",
} as const;

export const StaticContentFormatListItems: Array<ListItem<StaticContentFormats>> = [
  new ListItem<StaticContentFormats>(StaticContentFormat.Html, "HTML", "Render as HTML."),
  new ListItem<StaticContentFormats>(StaticContentFormat.Markdown, "Markdown", "Render as Markdown."),
  new ListItem<StaticContentFormats>(StaticContentFormat.PlainText, "Plain Text", "Render as plain text."),
];
