import { Injectable, inject } from "@angular/core";
import { AdminUserWithRoles, ErrorApiResponse, RoleWithAdminUsers } from "@core/api";
import {
  RoleDto,
  UpdateAdminUserRequestDto,
  AdminSearchUsersRequestDto,
  ClaimDto,
  PublicUserDto,
  PublicSearchUsersRequestDto,
  PagedResponseDto,
} from "@core/api/dtos";
import { UsersDataService } from "@core/api/services/users-data.service";
import { Observable, of, tap, map, forkJoin, switchMap, throwError } from "rxjs";
import { UsersService } from "./users.service";

/**
 * Service for public functionality any user can access, even if they're not logged in.
 */
@Injectable({
  providedIn: "root",
})
export class PublicService {
  #usersService = inject(UsersService);

  /**
   * Gets a user by their ID.
   * @param {string} userId - The ID of the user to retrieve.
   * @returns {Observable<PublicUserDto>} An observable containing the user data.
   */
  getUser(userId: string) {
    return this.#usersService.getUser(userId) as Observable<PublicUserDto>;
  }

  /**
   * Searches for users based on the search criteria.
   * @param {PublicSearchUsersRequestDto} publicSearchUsers - The search criteria.
   * @returns {Observable<Array<PublicUserDto>>} An observable containing the search results.
   */
  searchUsers(publicSearchUsers: PublicSearchUsersRequestDto) {
    return this.#usersService.searchUsers(publicSearchUsers) as Observable<PagedResponseDto<PublicUserDto>>;
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
