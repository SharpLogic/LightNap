/**
 * Represents an option with a key, display name, and description.
 */
export interface OptionDto {
  /**
   * The unique key for the option.
   */
  key: string;

  /**
   * The display name of the option.
   */
  displayName: string;

  /**
   * A brief description of the option.
   */
  description: string;
}
