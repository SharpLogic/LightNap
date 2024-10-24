import { ApiResponse } from "./api-response";
import { ApiResponseType } from "./api-response-type";

export class SuccessApiResponse<T> implements ApiResponse<T> {
  type: ApiResponseType = "Success";
  errorMessages = [];
  succeeded = true;
  requiresReauthorization = false;

  constructor(public result: T) {}
}
