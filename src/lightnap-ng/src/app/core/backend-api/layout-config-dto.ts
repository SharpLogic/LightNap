/**
 * Represents the style settings for the application.
 */
export interface LayoutConfigDto {
  preset: string;
  primary: string;
  surface: string | null | undefined;
  darkTheme: boolean;
  menuMode: string;
}
