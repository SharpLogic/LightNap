import { Injectable, inject } from "@angular/core";
import { Observable, of } from "rxjs";
import { AdminUsersService } from "./admin-users.service";
import { PrivilegedSearchUsersRequestDto, PrivilegedUserDto, RoleDto } from "@core/backend-api";

/**
 * Service for privileged endpoint access.
 */
@Injectable({
  providedIn: "root",
})
export class PrivilegedUsersService {
  #usersService = inject(AdminUsersService);

  /**
   * Gets a user by their ID.
   * @param {string} userId - The ID of the user to retrieve.
   * @returns {Observable<PrivilegedUserDto>} An observable containing the user data.
   */
  getUser(userId: string) {
    return this.#usersService.getUser(userId);
  }

  /**
   * Gets a user by their username.
   * @param {string} userName - The username of the user to retrieve.
   * @returns {Observable<PublicUserDto>} An observable containing the user data.
   */
  getUserByUserName(userName: string) {
    return this.#usersService.getUserByUserName(userName) as Observable<PrivilegedUserDto>;
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
    return this.#usersService.getUsersById(userIds) as Observable<Array<PrivilegedUserDto>>;
  }

  /**
   * Gets the list of roles.
   * @returns {Observable<Array<RoleDto>>} An observable containing the roles.
   */
  getRoles(): Observable<Array<RoleDto>> {
    return this.#usersService.getRoles();
  }
}
