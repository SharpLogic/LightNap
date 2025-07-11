import { Injectable, inject } from "@angular/core";
import { RoleDto, ErrorApiResponse, ClaimDto, PrivilegedUserDto } from "@core/api";
import { PrivilegedSearchUsersRequestDto } from "@core/api/dtos/users/request/privileged-search-users-request-dto";
import { Observable, of, tap, map, forkJoin, switchMap, throwError } from "rxjs";
import { UsersService } from "./users.service";

/**
 * Service for privileged endpoint access.
 */
@Injectable({
  providedIn: "root",
})
export class PrivilegedService {
  #usersService = inject(UsersService);

  /**
   * Gets a user by their ID.
   * @param {string} userId - The ID of the user to retrieve.
   * @returns {Observable<PrivilegedUserDto>} An observable containing the user data.
   */
  getUser(userId: string) {
    return this.#usersService.getUser(userId);
  }

  /**
   * Searches for users based on the search criteria.
   * @param {PrivilegedSearchUsersRequestDto} privilegedSearchUsersRequest - The search criteria.
   * @returns {Observable<Array<PrivilegedUserDto>>} An observable containing the search results.
   */
  searchUsers(privilegedSearchUsersRequest: PrivilegedSearchUsersRequestDto) {
    return this.#usersService.searchUsers(privilegedSearchUsersRequest);
  }

  /**
     * Gets users by their IDs.
     * @param {Array<string>} userIds - The IDs of the users to retrieve.
     * @returns {Observable<Array<PrivilegedUserDto>>} An observable containing the users.
     */
    getUsersById(userIds: Array<string>) {
      if (!userIds || userIds.length === 0) return of([]);
      return this.#usersService.getUsersById(userIds) as Observable<Array<PrivilegedUserDto>>;
    }
}
