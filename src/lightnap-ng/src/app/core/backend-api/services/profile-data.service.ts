import { HttpClient } from "@angular/common/http";
import { Injectable, inject } from "@angular/core";
import { API_URL_ROOT } from "@core/helpers";
import { tap } from "rxjs";
import { ApplicationSettingsDto, NotificationSearchResultsDto, ProfileDto, SearchNotificationsRequestDto, UpdateProfileRequestDto } from "../dtos";
import { NotificationHelper } from "../helpers/notification.helper";

@Injectable({
  providedIn: "root",
})
export class ProfileDataService {
  #http = inject(HttpClient);
  #apiUrlRoot = `${inject(API_URL_ROOT)}users/me/`;

  getProfile() {
    return this.#http.get<ProfileDto>(`${this.#apiUrlRoot}profile`);
  }

  updateProfile(updateProfile: UpdateProfileRequestDto) {
    return this.#http.put<ProfileDto>(`${this.#apiUrlRoot}profile`, updateProfile);
  }

  getSettings() {
    return this.#http.get<ApplicationSettingsDto>(`${this.#apiUrlRoot}settings`);
  }

  updateSettings(browserSettings: ApplicationSettingsDto) {
    return this.#http.put<boolean>(`${this.#apiUrlRoot}settings`, browserSettings);
  }

  searchNotifications(searchNotificationsRequest: SearchNotificationsRequestDto) {
    return this.#http
      .post<NotificationSearchResultsDto>(`${this.#apiUrlRoot}notifications`, searchNotificationsRequest)
      .pipe(tap(results => results.data.forEach(NotificationHelper.rehydrate)));
  }

  markAllNotificationsAsRead() {
    return this.#http.put<boolean>(`${this.#apiUrlRoot}notifications/mark-all-as-read`, undefined);
  }

  markNotificationAsRead(id: number) {
    return this.#http.put<boolean>(`${this.#apiUrlRoot}notifications/${id}/mark-as-read`, undefined);
  }
}
