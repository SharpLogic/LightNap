import { HttpClient } from "@angular/common/http";
import { Injectable, inject } from "@angular/core";
import { API_URL_ROOT } from "@core/helpers";
import { tap } from "rxjs";
import { DeviceHelper } from "../helpers/device.helper";
import { NotificationHelper } from "../helpers/notification.helper";
import { ChangePasswordRequestDto, ChangeEmailRequestDto, ConfirmChangeEmailRequestDto, ProfileDto, UpdateProfileRequestDto, DeviceDto, ApplicationSettingsDto, SearchNotificationsRequestDto, NotificationSearchResultsDto } from "../dtos";

@Injectable({
  providedIn: "root",
})
export class ProfileDataService {
  #http = inject(HttpClient);
  #apiUrlRoot = `${inject(API_URL_ROOT)}users/me/`;

  changePassword(changePasswordRequest: ChangePasswordRequestDto) {
    return this.#http.post<boolean>(`${this.#apiUrlRoot}change-password`, changePasswordRequest);
  }

  changeEmail(changeEmailRequest: ChangeEmailRequestDto) {
    return this.#http.post<boolean>(`${this.#apiUrlRoot}change-email`, changeEmailRequest);
  }

  confirmEmailChange(confirmChangeEmailRequest: ConfirmChangeEmailRequestDto) {
    return this.#http.post<boolean>(`${this.#apiUrlRoot}confirm-email-change`, confirmChangeEmailRequest);
  }

  getProfile() {
    return this.#http.get<ProfileDto>(`${this.#apiUrlRoot}profile`);
  }

  updateProfile(updateProfile: UpdateProfileRequestDto) {
    return this.#http.put<ProfileDto>(`${this.#apiUrlRoot}profile`, updateProfile);
  }

  getDevices() {
    return this.#http
      .get<Array<DeviceDto>>(`${this.#apiUrlRoot}devices`)
      .pipe(tap(devices => devices.forEach(device => DeviceHelper.rehydrate(device))));
  }

  revokeDevice(deviceId: string) {
    return this.#http.delete<boolean>(`${this.#apiUrlRoot}devices/${deviceId}`);
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
