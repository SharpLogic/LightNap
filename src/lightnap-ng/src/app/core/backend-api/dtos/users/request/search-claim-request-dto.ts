import { PagedRequestDto } from "../../paged-request-dto";

/**
 * Represents a request to search for claims with optional filtering and pagination.
 *
 * @extends PagedRequestDto
 *
 * @property {string} [type] - The username substring to search for.
 * @property {string} [value] - The value to search for in the claims.
 */
export interface SearchClaimRequestDto extends PagedRequestDto {
  /**
   * The exact type to filter claims by.
   * @type {string}
   */
  type: string;

  /**
   * The exact value to filter claims by.
   * @type {string}
   */
  value: string;
}
