import { HttpClient } from "@angular/common/http";
import { Injectable, inject } from "@angular/core";
import { API_URL_ROOT } from "@core/helpers";
import { tap } from "rxjs";
import { ClaimDto, NotificationSearchResultsDto, PagedRequestDto, PagedResponseDto, ProfileDto, SearchNotificationsRequestDto, SetUserSettingRequestDto, UpdateProfileRequestDto, UserSettingDto } from "../dtos";
import { NotificationHelper } from "../helpers/notification.helper";
import { UserSettingHelper } from "../helpers/user-setting.helper";

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

  getMyClaims(pagedRequestDto: PagedRequestDto) {
    return this.#http.post<PagedResponseDto<ClaimDto>>(`${this.#apiUrlRoot}claims`, pagedRequestDto);
  }

  getSetting(key: string) {
    return this.#http.get<string>(`${this.#apiUrlRoot}settings/${key}`);
  }

  getSettings() {
    return this.#http.get<Array<UserSettingDto>>(`${this.#apiUrlRoot}settings`).pipe(tap(settings => settings.forEach(UserSettingHelper.rehydrate)));
  }

  setSetting(setUserSettingRequest: SetUserSettingRequestDto) {
    return this.#http.patch<UserSettingDto>(`${this.#apiUrlRoot}settings`, setUserSettingRequest).pipe(tap(UserSettingHelper.rehydrate));
  }
}
