/*
 * Settings keys used in the application.
 */
export type UserSettingKey = "BrowserSettings" | "PreferredLanguage";

export const UserSettingKeys = {
  BrowserSettings: "BrowserSettings",
  PreferredLanguage: "PreferredLanguage",
} as const;
