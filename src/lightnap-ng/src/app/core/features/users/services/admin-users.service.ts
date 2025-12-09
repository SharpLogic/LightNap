import { Injectable, inject } from "@angular/core";
import {
    AdminSearchUsersRequestDto,
    AdminUpdateUserRequestDto,
    AdminUserDto,
    ClaimDto,
    ErrorApiResponse,
    PagedResponseDto,
    RoleDto,
    SearchClaimRequestDto,
    SearchClaimsRequestDto,
    SearchUserClaimsRequestDto,
} from "@core/backend-api";
import { LightNapWebApiService } from "@core/backend-api/services/lightnap-api";
import { Observable, forkJoin, map, of, shareReplay, switchMap, throwError } from "rxjs";
import { AdminUserWithRoles, RoleWithAdminUsers } from "../entities";

/**
 * Service for managing users and roles in the application. This service provides full access and should only be used in the context of
 * and admin user. PrivilegedUsersService and PublicUsersService are provided as a convenience services to be used in the context of lower-privileged
 * users. They limit the exposed endpoints and return types based on what the backend allows for their relative access levels.
 */
@Injectable({
  providedIn: "root",
})
export class AdminUsersService {
  #webApiService = inject(LightNapWebApiService);

  #roles$ = this.#webApiService
    .getRoles()
    .pipe(map(roles => roles || []))
    .pipe(shareReplay({ bufferSize: 1, refCount: false }));

  /**
   * Gets a user by their ID.
   * @param {string} userId - The ID of the user to retrieve.
   * @returns {Observable<AdminUserDto>} An observable containing the user data.
   */
  getUser(userId: string) {
    return this.#webApiService.getUser(userId);
  }

  /**
   * Gets a user by their username.
   * @param {string} userName - The username of the user to retrieve.
   * @returns {Observable<AdminUserDto>} An observable containing the user data.
   */
  getUserByUserName(userName: string) {
    return this.#webApiService.getUserByUserName(userName) as Observable<AdminUserDto>;
  }

  /**
   * Updates a user by their ID.
   * @param {string} userId - The ID of the user to update.
   * @param {AdminUpdateUserRequestDto} updateAdminUserRequest - The update request object.
   * @returns {Observable<AdminUserDto>} An observable with the updated user.
   */
  updateUser(userId: string, updateAdminUserRequest: AdminUpdateUserRequestDto) {
    return this.#webApiService.updateUser(userId, updateAdminUserRequest);
  }

  /**
   * Deletes a user by their ID.
   * @param {string} userId - The ID of the user to delete.
   * @returns {Observable<boolean>} An observable indicating the deletion result.
   */
  deleteUser(userId: string) {
    return this.#webApiService.deleteUser(userId);
  }

  /**
   * Searches for users based on the search criteria.
   * @param {AdminSearchUsersRequestDto} adminSearchUsersRequest - The search criteria.
   * @returns {Observable<Array<AdminUserDto>>} An observable containing the search results.
   */
  searchUsers(adminSearchUsersRequest: AdminSearchUsersRequestDto) {
    return this.#webApiService.searchUsers(adminSearchUsersRequest) as Observable<PagedResponseDto<AdminUserDto>>;
  }

  /**
   * Gets users by their IDs.
   * @param {Array<string>} userIds - The IDs of the users to retrieve.
   * @returns {Observable<Array<AdminUserDto>>} An observable containing the users.
   */
  getUsersById(userIds: Array<string>): Observable<Array<AdminUserDto>> {
    if (!userIds || userIds.length === 0) return of([]);
    return this.#webApiService.getUsersByIds(userIds);
  }

  /**
   * Gets the list of roles.
   * @returns {Observable<Array<RoleDto>>} An observable containing the roles.
   */
  getRoles(): Observable<Array<RoleDto>> {
    return this.#roles$;
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
    return forkJoin([this.getRoles(), this.#webApiService.getRolesForUser(userId).pipe(map(userRoles => userRoles || []))]).pipe(
      map(([roles, userRoles]) => userRoles.map(userRole => roles.find(role => role.name === userRole)!))
    );
  }

  /**
   * Gets users in the specified role.
   * @param {string} role - The role.
   * @returns {Observable<Array<AdminUserDto>>} An observable containing the members.
   */
  getUsersInRole(role: string) {
    return this.#webApiService.getUsersInRole(role);
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
    return this.#webApiService.addUserToRole(role, userId);
  }

  /**
   * Removes a user from a role.
   * @param {string} userId - The user to remove from the role.
   * @param {string} role - The role.
   * @returns {Observable<RoleDto>} An observable with a result of true if successful.
   */
  removeUserFromRole(userId: string, role: string) {
    return this.#webApiService.removeUserFromRole(role, userId);
  }

  /**
   * Searches for claims based on the search criteria.
   * @param {SearchClaimsRequestDto} searchClaims - The search criteria.
   * @returns {Observable<PagedResponseDto<ClaimDto>>} An observable containing the search results.
   */
  searchClaims(searchClaims: SearchClaimsRequestDto) {
    return this.#webApiService.searchClaims(searchClaims);
  }

  /**
   * Searches for claims based on the search criteria.
   * @param {SearchUserClaimsRequestDto} searchUserClaimsRequestDto - The search criteria.
   * @returns {Observable<PagedResponseDto<ClaimDto>>} An observable containing the search results.
   */
  getUserClaims(searchUserClaimsRequestDto: SearchUserClaimsRequestDto) {
    return this.#webApiService
      .searchUserClaims(searchUserClaimsRequestDto)
      .pipe(map(results => <PagedResponseDto<ClaimDto>>{ ...results, data: results.data }));
  }

  /**
   * Gets users who have the specified claim.
   * @param {SearchClaimRequestDto} searchClaimRequestDto - The search criteria.
   * @returns {Observable<PagedResponseDto<AdminUserDto>>} An observable containing the users.
   */
  getUsersWithClaim(searchClaimRequestDto: SearchClaimRequestDto) {
    return this.#webApiService
      .getUsersWithClaim(searchClaimRequestDto)
      .pipe(
        switchMap(results =>
          this.getUsersById(results.data || new Array<string>()).pipe(
            map(users => <PagedResponseDto<AdminUserDto>>{ totalCount: results.totalCount, data: users })
          )
        )
      );
  }

  /**
   * Adds a claim to a user.
   * @param {string} userId - The user to add the claim to.
   * @param {ClaimDto} claim - The claim to add.
   * @returns {Observable<boolean>} An observable with a result of true if successful.
   */
  addUserClaim(userId: string, claim: ClaimDto) {
    return this.#webApiService.addUserClaim(userId, claim);
  }

  /**
   * Removes a claim from a user.
   * @param {string} userId - The user to remove the claim from.
   * @param {ClaimDto} claim - The claim to remove.
   * @returns {Observable<boolean>} An observable with a result of true if successful.
   */
  removeUserClaim(userId: string, claim: ClaimDto) {
    return this.#webApiService.removeUserClaim(userId, claim);
  }

  /**
   * Locks a user account.
   * @param {string} userId - The user to lock.
   * @returns {Observable<boolean>} An observable with a result of true if successful.
   */
  lockUserAccount(userId: string) {
    return this.#webApiService.lockUserAccount(userId);
  }

  /**
   * Unlocks a user account.
   * @param {string} userId - The user to lock.
   * @returns {Observable<boolean>} An observable with a result of true if successful.
   */
  unlockUserAccount(userId: string) {
    return this.#webApiService.unlockUserAccount(userId);
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
