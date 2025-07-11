import { Injectable, inject } from "@angular/core";
import {
    AdminSearchUsersRequestDto,
    AdminUpdateUserRequestDto,
    AdminUserDto,
    AdminUserWithRoles,
    ClaimDto,
    ErrorApiResponse,
    RoleDto,
    RoleWithAdminUsers,
    SearchClaimsRequestDto,
} from "@core/backend-api";
import { UsersDataService } from "@core/backend-api/services/users-data.service";
import { Observable, forkJoin, map, of, switchMap, tap, throwError } from "rxjs";

/**
 * Service for managing users and roles in the application. Note that this service should not be used directly, but rather accessed
 * via the AdminService, PrivilegedService, PublicService or other services that provide the necessary access control.
 */
@Injectable({
  providedIn: "root",
})
export class UsersService {
  #dataService = inject(UsersDataService);
  #rolesResponse?: Array<RoleDto>;

  /**
   * Gets a user by their ID.
   * @param {string} userId - The ID of the user to retrieve.
   * @returns {Observable<AdminUserDto>} An observable containing the user data.
   */
  getUser(userId: string) {
    return this.#dataService.getUser(userId);
  }

  /**
   * Updates a user by their ID.
   * @param {string} userId - The ID of the user to update.
   * @param {AdminUpdateUserRequestDto} updateAdminUserRequest - The update request object.
   * @returns {Observable<AdminUserDto>} An observable with the updated user.
   */
  updateUser(userId: string, updateAdminUserRequest: AdminUpdateUserRequestDto) {
    return this.#dataService.updateUser(userId, updateAdminUserRequest);
  }

  /**
   * Deletes a user by their ID.
   * @param {string} userId - The ID of the user to delete.
   * @returns {Observable<boolean>} An observable indicating the deletion result.
   */
  deleteUser(userId: string) {
    return this.#dataService.deleteUser(userId);
  }

  /**
   * Searches for users based on the search criteria.
   * @param {AdminSearchUsersRequestDto} adminSearchUsersRequest - The search criteria.
   * @returns {Observable<Array<AdminUserDto>>} An observable containing the search results.
   */
  searchUsers(adminSearchUsersRequest: AdminSearchUsersRequestDto) {
    return this.#dataService.searchUsers(adminSearchUsersRequest);
  }

  /**
   * Gets users by their IDs.
   * @param {Array<string>} userIds - The IDs of the users to retrieve.
   * @returns {Observable<Array<AdminUserDto>>} An observable containing the users.
   */
  getUsersById(userIds: Array<string>): Observable<Array<AdminUserDto>> {
    if (!userIds || userIds.length === 0) return of([]);
    return this.#dataService.getUsersById(userIds);
  }

  /**
   * Gets the list of roles.
   * @returns {Observable<Array<RoleDto>>} An observable containing the roles.
   */
  getRoles(): Observable<Array<RoleDto>> {
    if (this.#rolesResponse) return of(this.#rolesResponse);
    return this.#dataService.getRoles().pipe(tap(roles => (this.#rolesResponse = roles)));
  }

  /**
   * Gets a role by its name.
   * @param {string} roleName - The name of the role to retrieve.
   * @returns {Observable<RoleDto>} An observable containing the role data.
   */
  getRole(roleName: string) {
    return this.getRoles().pipe(map(roles => roles.find(role => role.name === roleName)));
  }

  /**
   * Gets the roles a user belongs to.
   * @param {string} userId - The user.
   * @returns {Observable<Array<RoleDto>>} An observable containing the roles.
   */
  getUserRoles(userId: string) {
    return forkJoin([this.getRoles(), this.#dataService.getUserRoles(userId)]).pipe(
      map(([rolesResponse, userRolesResponse]) => userRolesResponse.map(userRole => rolesResponse.find(role => role.name === userRole)!))
    );
  }

  /**
   * Gets users in the specified role.
   * @param {string} role - The role.
   * @returns {Observable<Array<AdminUserDto>>} An observable containing the members.
   */
  getUsersInRole(role: string) {
    return this.#dataService.getUsersInRole(role);
  }

  /**
   * Gets a role with its users.
   * @param {string} roleName - The role.
   * @returns {Observable<RoleWithAdminUsers>} An observable containing the role and users.
   */
  getRoleWithUsers(roleName: string) {
    return this.getRole(roleName).pipe(
      switchMap(role => {
        if (!role) return throwError(() => new ErrorApiResponse([`Role '${roleName}' not found`]));
        return this.getUsersInRole(role.name).pipe(map(users => <RoleWithAdminUsers>{ role, users }));
      })
    );
  }

  /**
   * Adds a user to a role.
   * @param {string} userId - The user to add to the role.
   * @param {string} role - The role.
   * @returns {Observable<boolean>} An observable with a result of true if successful.
   */
  addUserToRole(userId: string, role: string) {
    return this.#dataService.addUserToRole(userId, role);
  }

  /**
   * Removes a user from a role.
   * @param {string} userId - The user to remove from the role.
   * @param {string} role - The role.
   * @returns {Observable<RoleDto>} An observable with a result of true if successful.
   */
  removeUserFromRole(userId: string, role: string) {
    return this.#dataService.removeUserFromRole(userId, role);
  }

  /**
   * Searches for claims based on the search criteria.
   * @param {SearchClaimsRequestDto} searchClaims - The search criteria.
   * @returns {Observable<PagedResponseDto<ClaimDto>>} An observable containing the search results.
   */
  searchClaims(searchClaims: SearchClaimsRequestDto) {
    return this.#dataService.searchClaims(searchClaims);
  }

  /**
   * Searches for claims based on the search criteria.
   * @param {SearchAdminClaimsRequestDto} searchAdminClaims - The search criteria.
   * @returns {Observable<PagedResponseDto<ClaimDto>>} An observable containing the search results.
   */
  getUserClaims(userId: string) {
    return this.#dataService.searchUserClaims({ userId }).pipe(map(response => response.data || new Array<ClaimDto>()));
  }

  /**
   * Searches for claims based on the search criteria.
   * @param {SearchAdminClaimsRequestDto} searchAdminClaims - The search criteria.
   * @returns {Observable<PagedResponseDto<ClaimDto>>} An observable containing the search results.
   */
  getUsersWithClaim(claim: ClaimDto) {
    return this.#dataService.searchUserClaims({ type: claim.type, value: claim.value }).pipe(
      switchMap(response => {
        if (!response.data || response.data.length === 0) {
          return of(new Array<AdminUserDto>());
        }
        return forkJoin(response.data.map(item => this.#dataService.getUser(item.userId)));
      })
    );
  }

  /**
   * Adds a claim to a user.
   * @param {string} userId - The user to add the claim to.
   * @param {ClaimDto} claim - The claim to add.
   * @returns {Observable<boolean>} An observable with a result of true if successful.
   */
  addUserClaim(userId: string, claim: ClaimDto) {
    return this.#dataService.addUserClaim(userId, claim);
  }

  /**
   * Removes a claim from a user.
   * @param {string} userId - The user to remove the claim from.
   * @param {ClaimDto} claim - The claim to remove.
   * @returns {Observable<boolean>} An observable with a result of true if successful.
   */
  removeUserClaim(userId: string, claim: ClaimDto) {
    return this.#dataService.removeUserClaim(userId, claim);
  }

  /**
   * Locks a user account.
   * @param {string} userId - The user to lock.
   * @returns {Observable<boolean>} An observable with a result of true if successful.
   */
  lockUserAccount(userId: string) {
    return this.#dataService.lockUserAccount(userId);
  }

  /**
   * Unlocks a user account.
   * @param {string} userId - The user to lock.
   * @returns {Observable<boolean>} An observable with a result of true if successful.
   */
  unlockUserAccount(userId: string) {
    return this.#dataService.unlockUserAccount(userId);
  }

  /**
   * Gets a user with their roles.
   * @param {string} userId - The user.
   * @returns {Observable<AdminUserWithRoles>} An observable containing the user and roles.
   */
  getUserWithRoles(userId: string) {
    return forkJoin([this.getUser(userId), this.getUserRoles(userId)]).pipe(
      map(([user, roles]) => {
        return <AdminUserWithRoles>{
          user,
          roles,
        };
      })
    );
  }
}
