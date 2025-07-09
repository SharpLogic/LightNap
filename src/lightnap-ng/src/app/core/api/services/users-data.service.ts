import { HttpClient } from "@angular/common/http";
import { Injectable, inject } from "@angular/core";
import { API_URL_ROOT } from "@core/helpers";
import { AdminUserDto, UpdateAdminUserRequestDto, SearchAdminUsersRequestDto, PagedResponseDto, RoleDto, SearchAdminClaimsRequestDto, UserClaimDto, ClaimDto } from "../dtos";

@Injectable({
  providedIn: "root",
})
export class UsersDataService {
  #http = inject(HttpClient);
  #apiUrlRoot = `${inject(API_URL_ROOT)}users/`;

  getUser(userId: string) {
    return this.#http.get<AdminUserDto>(`${this.#apiUrlRoot}${userId}`);
  }

  updateUser(userId: string, updateAdminUser: UpdateAdminUserRequestDto) {
    return this.#http.put<AdminUserDto>(`${this.#apiUrlRoot}${userId}`, updateAdminUser);
  }

  deleteUser(userId: string) {
    return this.#http.delete<boolean>(`${this.#apiUrlRoot}${userId}`);
  }

  searchUsers(searchAdminUsers: SearchAdminUsersRequestDto) {
    return this.#http.post<PagedResponseDto<AdminUserDto>>(`${this.#apiUrlRoot}search`, searchAdminUsers);
  }

  getRoles() {
    return this.#http.get<Array<RoleDto>>(`${this.#apiUrlRoot}roles`);
  }

  getUserRoles(userId: string) {
    return this.#http.get<Array<string>>(`${this.#apiUrlRoot}${userId}/roles`);
  }

  getUsersInRole(role: string) {
    return this.#http.get<Array<AdminUserDto>>(`${this.#apiUrlRoot}roles/${role}`);
  }

  addUserToRole(userId: string, role: string) {
    return this.#http.post<boolean>(`${this.#apiUrlRoot}roles/${role}/${userId}`, null);
  }

  removeUserFromRole(userId: string, role: string) {
    return this.#http.delete<boolean>(`${this.#apiUrlRoot}roles/${role}/${userId}`);
  }

  searchClaims(searchAdminClaimsRequestDto: SearchAdminClaimsRequestDto) {
    return this.#http.post<PagedResponseDto<UserClaimDto>>(`${this.#apiUrlRoot}claims/search`, searchAdminClaimsRequestDto);
  }

  addClaimToUser(userId: string, claim: ClaimDto) {
    return this.#http.post<boolean>(`${this.#apiUrlRoot}${userId}/claims`, claim);
  }

  removeClaimFromUser(userId: string, claim: ClaimDto) {
    return this.#http.request<boolean>('delete', `${this.#apiUrlRoot}${userId}/claims`, { body: claim });
  }

  lockUserAccount(userId: string) {
    return this.#http.post<boolean>(`${this.#apiUrlRoot}${userId}/lock`, null);
  }

  unlockUserAccount(userId: string) {
    return this.#http.post<boolean>(`${this.#apiUrlRoot}${userId}/unlock`, null);
  }
}
