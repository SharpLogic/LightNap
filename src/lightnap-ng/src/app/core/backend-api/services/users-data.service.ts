import { HttpClient } from "@angular/common/http";
import { Injectable, inject } from "@angular/core";
import { API_URL_ROOT } from "@core/helpers";
import {
    AdminSearchUsersRequestDto,
    AdminUpdateUserRequestDto,
    AdminUserDto,
    ClaimDto,
    PagedResponseDto,
    RoleDto,
    SearchClaimsRequestDto,
    SearchUserClaimsRequestDto,
    SetUserSettingRequestDto,
    UserClaimDto,
    UserSettingDto,
} from "../dtos";
import { tap } from "rxjs";
import { UserSettingHelper } from "../helpers/user-setting.helper";

@Injectable({
  providedIn: "root",
})
export class UsersDataService {
  #http = inject(HttpClient);
  #apiUrlRoot = `${inject(API_URL_ROOT)}users/`;

  getUser(userId: string) {
    return this.#http.get<AdminUserDto>(`${this.#apiUrlRoot}${userId}`);
  }

  getUserByUserName(userName: string) {
    return this.#http.get<AdminUserDto>(`${this.#apiUrlRoot}user-name/${userName}`);
  }

  getUsersById(userIds: Array<string>) {
    return this.#http.post<Array<AdminUserDto>>(`${this.#apiUrlRoot}get-by-ids`, userIds);
  }

  updateUser(userId: string, updateAdminUser: AdminUpdateUserRequestDto) {
    return this.#http.put<AdminUserDto>(`${this.#apiUrlRoot}${userId}`, updateAdminUser);
  }

  deleteUser(userId: string) {
    return this.#http.delete<boolean>(`${this.#apiUrlRoot}${userId}`);
  }

  searchUsers(searchAdminUsers: AdminSearchUsersRequestDto) {
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

  searchClaims(searchClaimsRequestDto: SearchClaimsRequestDto) {
    return this.#http.post<PagedResponseDto<ClaimDto>>(`${this.#apiUrlRoot}claims/search`, searchClaimsRequestDto);
  }

  searchUserClaims(searchUserClaimsRequestDto: SearchUserClaimsRequestDto) {
    return this.#http.post<PagedResponseDto<UserClaimDto>>(`${this.#apiUrlRoot}user-claims/search`, searchUserClaimsRequestDto);
  }

  addUserClaim(userId: string, claim: ClaimDto) {
    return this.#http.post<boolean>(`${this.#apiUrlRoot}${userId}/claims`, claim);
  }

  removeUserClaim(userId: string, claim: ClaimDto) {
    return this.#http.request<boolean>("delete", `${this.#apiUrlRoot}${userId}/claims`, { body: claim });
  }

  lockUserAccount(userId: string) {
    return this.#http.post<boolean>(`${this.#apiUrlRoot}${userId}/lock`, null);
  }

  unlockUserAccount(userId: string) {
    return this.#http.post<boolean>(`${this.#apiUrlRoot}${userId}/unlock`, null);
  }

  getSettings(userId: string) {
    return this.#http.get<Array<UserSettingDto>>(`${this.#apiUrlRoot}${userId}/settings`).pipe(tap(settings => settings.forEach(UserSettingHelper.rehydrate)));
  }

  updateSetting(userId: string, setUserSetting: SetUserSettingRequestDto) {
    return this.#http.patch<UserSettingDto>(`${this.#apiUrlRoot}${userId}/settings`, setUserSetting).pipe(tap(UserSettingHelper.rehydrate));
  }
}
