/**
 * Represents an option with a value, label, and description.
 */
export interface OptionDto {
  /**
   * The unique value for the option.
   */
  value: string;

  /**
   * The display label of the option.
   */
  label: string;

  /**
   * A brief description of the option.
   */
  description: string;
}
