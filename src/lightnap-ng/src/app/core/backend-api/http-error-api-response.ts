import { HttpErrorResponse } from "@angular/common/http";
import { ApiResponseType } from "./api-response-type";
import { environment } from "src/environments/environment";
import { ApiResponseDto } from ".";

/**
 * Represents an HTTP error response from an API.
 *
 * @template T - The type of the result expected from the API response.
 */
export class HttpErrorApiResponse<T> implements ApiResponseDto<T> {
  /**
   * The result of the API response, if any.
   */
  result?: T;

  /**
   * The type of the API response.
   * Defaults to "UnexpectedError".
   */
  type: ApiResponseType = "UnexpectedError";

  /**
   * A list of error messages associated with the API response.
   */
  errorMessages: Array<string>;

  /**
   * Constructs an instance of `HttpErrorApiResponse`.
   *
   * @param response - The HTTP error response received from the API.
   */
  constructor(response: HttpErrorResponse) {
    switch (response.status) {
      case 0:
        this.errorMessages = ["We were unable to connect to the service."];
        break;

      default:
        this.errorMessages = ["An unexpected error occurred"];
        break;
    }

    if (!environment.production) {
      if (response.error?.errors) {
        if (Array.isArray(response.error.errors)) {
          this.errorMessages = response.error.errors.map((error: any) => `DEBUG: ${JSON.stringify(error)}`);
        } else {
          this.errorMessages = Object.values(response.error.errors);
        }
      }

      this.errorMessages.push(`DEBUG (Full response): ${JSON.stringify(response)}`);
    }
  }
}
