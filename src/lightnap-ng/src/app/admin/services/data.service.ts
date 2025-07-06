import { AdminUserDto, RoleDto, SearchAdminUsersRequestDto, UpdateAdminUserRequestDto } from "@admin/models";
import { HttpClient } from "@angular/common/http";
import { Injectable, inject } from "@angular/core";
import { API_URL_ROOT, ApiResponseDto, PagedResponseDto } from "@core";
import { ClaimDto } from "@identity";

@Injectable({
  providedIn: "root",
})
export class DataService {
  #http = inject(HttpClient);
  #apiUrlRoot = `${inject(API_URL_ROOT)}administrator/`;

  getUser(userId: string) {
    return this.#http.get<AdminUserDto>(`${this.#apiUrlRoot}users/${userId}`);
  }

  updateUser(userId: string, updateAdminUser: UpdateAdminUserRequestDto) {
    return this.#http.put<AdminUserDto>(`${this.#apiUrlRoot}users/${userId}`, updateAdminUser);
  }

  deleteUser(userId: string) {
    return this.#http.delete<boolean>(`${this.#apiUrlRoot}users/${userId}`);
  }

  searchUsers(searchAdminUsers: SearchAdminUsersRequestDto) {
    return this.#http.post<PagedResponseDto<AdminUserDto>>(`${this.#apiUrlRoot}users/search`, searchAdminUsers);
  }

  getRoles() {
    return this.#http.get<Array<RoleDto>>(`${this.#apiUrlRoot}roles`);
  }

  getUserRoles(userId: string) {
    return this.#http.get<Array<string>>(`${this.#apiUrlRoot}users/${userId}/roles`);
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

  getUserClaims(userId: string) {
    return this.#http.get<Array<ClaimDto>>(`${this.#apiUrlRoot}users/${userId}/claims`);
  }

  getUsersForClaim(claim: ClaimDto) {
    return this.#http.get<Array<AdminUserDto>>(`${this.#apiUrlRoot}claims/${claim.type}/${claim.value}`);
  }

  addClaimToUser(userId: string, claim: ClaimDto) {
    return this.#http.post<boolean>(`${this.#apiUrlRoot}users/${userId}/claims/${claim.type}/${claim.value}`, null);
  }

  removeClaimFromUser(userId: string, claim: ClaimDto) {
    return this.#http.delete<boolean>(`${this.#apiUrlRoot}users/${userId}/claims/${claim.type}/${claim.value}`);
  }

  lockUserAccount(userId: string) {
    return this.#http.post<boolean>(`${this.#apiUrlRoot}users/${userId}/lock`, null);
  }

  unlockUserAccount(userId: string) {
    return this.#http.post<boolean>(`${this.#apiUrlRoot}users/${userId}/unlock`, null);
  }
}
