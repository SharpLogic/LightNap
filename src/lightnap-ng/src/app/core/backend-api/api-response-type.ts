/**
 * Represents the type of response received from an API call.
 */
export type ApiResponseType =
  /**
   * Indicates that the API call was successful.
   */
  | "Success"
  /**
   * Indicates that the API call encountered an expected error with user-friendly error messages.
   */
  | "Error"
  /**
   * Indicates that the API call encountered an unexpected error.
   */
  | "UnexpectedError";

export const ApiResponseTypes = {
  Success: "Success",
  Error: "Error",
  UnexpectedError: "UnexpectedError",
} as const;
