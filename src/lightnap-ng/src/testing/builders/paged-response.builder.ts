/**
 * Builder for paged response DTOs
 * 
 * Provides convenient methods for creating paged test data
 */

export class PagedResponseBuilder {
  /**
   * Create a paged response
   */
  static create<T>(data: T[], overrides?: Partial<any>): any {
    return {
      data,
      page: 1,
      pageSize: data.length,
      totalCount: data.length,
      totalPages: 1,
      ...overrides,
    };
  }

  /**
   * Create an empty paged response
   */
  static createEmpty<T>(): any {
    return {
      data: [],
      page: 1,
      pageSize: 10,
      totalCount: 0,
      totalPages: 0,
    };
  }

  /**
   * Create a multi-page response
   */
  static createMultiPage<T>(
    data: T[],
    page: number,
    pageSize: number,
    totalCount: number
  ): any {
    return {
      data,
      page,
      pageSize,
      totalCount,
      totalPages: Math.ceil(totalCount / pageSize),
    };
  }
}
