import { ApiResponseDto } from "./api-response-dto";
import { ApiResponseType } from "./api-response-type";

/**
 * Represents a successful API response. This is useful when a method needs to return a successful API response,
 * such as when the final result of an operation is not the result of the API call itself.
 * @template T - The type of the result.
 */
export class SuccessApiResponse<T> implements ApiResponseDto<T> {
    /**
     * The type of the API response.
     * @default "Success"
     */
    type: ApiResponseType = "Success";

    /**
     * An array of error messages. This will be empty for a successful response.
     * @default []
     */
    errorMessages = [];

    /**
     * Indicates whether the API call succeeded.
     * @default true
     */
    succeeded = true;

    /**
     * Creates an instance of SuccessApiResponse.
     * @param result - The result of the API call.
     */
    constructor(public result: T) {}
}
