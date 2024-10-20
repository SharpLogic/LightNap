import { AdminUser, SearchAdminUsersRequest, UpdateAdminUserRequest } from "@admin/models";
import { HttpClient } from "@angular/common/http";
import { Injectable, inject } from "@angular/core";
import { API_URL_ROOT, ApiResponse } from "@core";

@Injectable({
  providedIn: "root",
})
export class DataService {
  #http = inject(HttpClient);
  #apiUrl = inject(API_URL_ROOT);

  getUser(userId: string) {
    return this.#http.get<ApiResponse<AdminUser>>(`${this.#apiUrl}administrator/users/${userId}`);
  }

  updateUser(userId: string, updateAdminUser: UpdateAdminUserRequest) {
    return this.#http.put<ApiResponse<AdminUser>>(`${this.#apiUrl}administrator/users/${userId}`, updateAdminUser);
  }

  deleteUser(userId: string) {
    return this.#http.delete<ApiResponse<AdminUser>>(`${this.#apiUrl}administrator/users/${userId}`);
  }

  searchUsers(searchAdminUsers: SearchAdminUsersRequest) {
    return this.#http.post<ApiResponse<Array<AdminUser>>>(`${this.#apiUrl}administrator/users/search`, searchAdminUsers);
  }

  getUserRoles(userId: string) {
    return this.#http.get<ApiResponse<Array<string>>>(`${this.#apiUrl}administrator/users/${userId}/roles`);
  }

  getUsersInRole(role: string) {
    return this.#http.get<ApiResponse<Array<AdminUser>>>(`${this.#apiUrl}administrator/roles/${role}`);
  }

  addUserToRole(userId: string, role: string) {
    return this.#http.post<ApiResponse<boolean>>(`${this.#apiUrl}administrator/roles/${role}/${userId}`, null);
  }

  removeUserFromRole(userId: string, role: string) {
    return this.#http.delete<ApiResponse<boolean>>(`${this.#apiUrl}administrator/roles/${role}/${userId}`);
  }
}
