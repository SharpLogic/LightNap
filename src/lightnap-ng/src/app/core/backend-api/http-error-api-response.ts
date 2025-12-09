import { HttpErrorResponse } from "@angular/common/http";
import { environment } from "src/environments/environment";
import { ApiResponseDto } from ".";
import { ApiResponseType } from "./models";

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
  type: ApiResponseType = ApiResponseType.UnexpectedError;

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
    this.errorMessages = [];

    switch (response.status) {
      case 0:
        this.errorMessages.push("We were unable to connect to the service.");
        if (!environment.production) {
          this.errorMessages.push(`DEBUG: The backend is probably not running or the API URL is incorrect.`);
        }
        break;

      case 400:
        this.errorMessages.push("The request was invalid.");
        if (!environment.production) {
          this.errorMessages.push(`DEBUG: Check the body of the request to make sure it meets the expectations of the backend endpoint.`);
        }
        break;

      case 401:
        this.errorMessages.push("You must log in to perform this action.");
        if (!environment.production) {
          this.errorMessages.push(`DEBUG: The backend endpoint requires the user to be logged in to make this request.`);
        }
        break;

      case 403:
        this.errorMessages.push("You do not have permission to perform this action.");
        if (!environment.production) {
          this.errorMessages.push(`DEBUG: The backend authorization requires a role and/or claim the logged in user does not have.`);
        }
        break;

      case 404:
        this.errorMessages.push("The requested resource was not found.");
        if (!environment.production) {
          this.errorMessages.push(`DEBUG: There is probably a typo in the URL this request was sent to: ${response.url}`);
        }
        break;

      case 405:
        this.errorMessages.push("The requested method is not allowed.");
        if (!environment.production) {
          this.errorMessages.push(`DEBUG: There may be a typo in the URL this request was sent to: ${response.url}`);
          this.errorMessages.push(`DEBUG: Ensure you're using the right HTTP verb for this endpoint (e.g. GET, POST, PUT, DELETE).`);
        }
        break;

      case 429:
        this.errorMessages.push("You have sent too many requests too quickly.");
        if (!environment.production) {
          this.errorMessages.push(`DEBUG: The backend endpoint uses rate limiting and the request count has reached the threshold for this policy.`);
        }
        break;

        case 500:
        this.errorMessages.push("An internal server error occurred.");
        if (!environment.production) {
          this.errorMessages.push(`DEBUG: The backend threw an unexpected exception.`);
        }
        break;

      default:
        this.errorMessages.push("An unexpected error occurred");
        break;
    }

    if (!environment.production) {
      const errors = response.error?.errors;
      if (errors) {
        if (Array.isArray(errors)) {
          this.errorMessages.push(
            ...errors.map((error: any) => {
              try {
                return `DEBUG: ${JSON.stringify(error)}`;
              } catch {
                return `DEBUG: [Error: Unable to stringify]`;
              }
            })
          );
        } else if (typeof errors === "object" && errors !== null) {
          this.errorMessages.push(...Object.values(errors).map((value: any) => String(value)));
        } else {
          this.errorMessages.push("DEBUG: Unexpected error format");
        }
      }

      console.debug("Full response:", response);
      this.errorMessages.push(`DEBUG (Full response): ${JSON.stringify(response, null, 2)}`);
    }
  }
}
