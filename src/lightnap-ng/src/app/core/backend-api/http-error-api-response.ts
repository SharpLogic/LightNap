import { HttpErrorResponse } from "@angular/common/http";
import { ApiResponseType, ApiResponseTypes } from "./api-response-type";
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
  type: ApiResponseType = ApiResponseTypes.UnexpectedError;

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
        if (!environment.production) {
          this.errorMessages.push(`DEBUG: The backend is probably not running or the API URL is incorrect.`);
        }
        break;

      case 400:
        this.errorMessages = ["The request was invalid."];
        if (!environment.production) {
          this.errorMessages.push(`DEBUG: Check the body of the request to make sure it meets the expectations of the backend endpoint.`);
        }
        break;

      case 401:
        this.errorMessages = ["You must log in to perform this action."];
        if (!environment.production) {
          this.errorMessages.push(`DEBUG: The backend endpoint requires the user to be logged in to make this request.`);
        }
        break;

      case 403:
        this.errorMessages = ["You do not have permission to perform this action."];
        if (!environment.production) {
          this.errorMessages.push(`DEBUG: The backend authorization requires a role and/or claim the logged in user does not have.`);
        }
        break;

      case 404:
        this.errorMessages = ["The requested resource was not found."];
        if (!environment.production) {
          this.errorMessages.push(`DEBUG: There is probably a typo in the URL this request was sent to: ${response.url}`);
        }
        break;

      case 405:
        this.errorMessages = ["The requested method is not allowed."];
        if (!environment.production) {
          this.errorMessages.push(`DEBUG: There may be a typo in the URL this request was sent to: ${response.url}`);
          this.errorMessages.push(`DEBUG: Ensure you're using the right HTTP verb for this endpoint (e.g. GET, POST, PUT, DELETE).`);
        }
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
