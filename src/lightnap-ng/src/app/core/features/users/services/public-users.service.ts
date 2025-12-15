import { Injectable, inject } from "@angular/core";
import { PagedResponseDto, AdminSearchUsersRequestDto, PublicUserDto } from "@core/backend-api";
import { Observable, of } from "rxjs";
import { PrivilegedUsersService } from "./privileged-users.service";

/**
 * Service for public functionality any user can access, even if they're not logged in.
 */
@Injectable({
  providedIn: "root",
})
export class PublicUsersService {
  #usersService = inject(PrivilegedUsersService);

  /**
   * Gets a user by their ID.
   * @param {string} userId - The ID of the user to retrieve.
   * @returns {Observable<PublicUserDto>} An observable containing the user data.
   */
  getUser(userId: string) {
    return this.#usersService.getUser(userId) as Observable<PublicUserDto>;
  }

  /**
   * Gets a user by their username.
   * @param {string} userName - The username of the user to retrieve.
   * @returns {Observable<PublicUserDto>} An observable containing the user data.
   */
  getUserByUserName(userName: string) {
    return this.#usersService.getUserByUserName(userName) as Observable<PublicUserDto>;
  }

  /**
   * Searches for users based on the search criteria.
   * @param {AdminSearchUsersRequestDto} publicSearchUsersRequest - The search criteria.
   * @returns {Observable<Array<PublicUserDto>>} An observable containing the search results.
   */
  searchUsers(publicSearchUsersRequest: AdminSearchUsersRequestDto) {
    return this.#usersService.searchUsers(publicSearchUsersRequest) as Observable<PagedResponseDto<PublicUserDto>>;
  }

  /**
   * Gets users by their IDs.
   * @param {Array<string>} userIds - The IDs of the users to retrieve.
   * @returns {Observable<Array<PublicUserDto>>} An observable containing the users.
   */
  getUsersById(userIds: Array<string>) {
    if (!userIds || userIds.length === 0) return of([]);
    return this.#usersService.getUsersById(userIds) as Observable<Array<PublicUserDto>>;
  }
}
