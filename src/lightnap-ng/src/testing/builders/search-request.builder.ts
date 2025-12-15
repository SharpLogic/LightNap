/**
 * Builder for search request DTOs
 *
 * Provides fluent API for creating test search request data
 */
import { SearchClaimRequestDto } from "@core/backend-api";
import { faker } from "@faker-js/faker";

export class SearchRequestBuilder {
  /**
   * Create a SearchClaimRequestDto with sensible defaults
   */
  static createSearchClaimRequest(overrides?: Partial<SearchClaimRequestDto>): SearchClaimRequestDto {
    return {
      type: faker.string.alpha({ length: { min: 5, max: 15 } }),
      value: faker.string.alpha({ length: { min: 5, max: 15 } }),
      pageNumber: 1,
      pageSize: 10,
      ...overrides,
    };
  }

  /**
   * Create a SearchClaimRequestDto with specific type and value
   */
  static createSearchClaimRequestWithValues(type: string, value: string, pageNumber = 1, pageSize = 10): SearchClaimRequestDto {
    return {
      type,
      value,
      pageNumber,
      pageSize,
    };
  }
}
