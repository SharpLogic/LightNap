/*
 * Static content formats used in the application.
 */
export type StaticContentFormats = "Html" | "Markdown" | "PlainText";

export const StaticContentFormat = {
  Html: "Html",
  Markdown: "Markdown",
  PlainText: "PlainText",
} as const;
