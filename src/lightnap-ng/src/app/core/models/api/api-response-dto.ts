import { ApiResponseType } from "./api-response-type";

/**
 * Represents a generic API response.
 *
 * @template T - The type of the result.
 */
export interface ApiResponseDto<T> {
    /**
     * The result of the API call.
     */
    result?: T;

    /**
     * The type of the API response.
     */
    type: ApiResponseType;

    /**
     * An array of error messages, if any.
     */
    errorMessages?: Array<string>;
}
