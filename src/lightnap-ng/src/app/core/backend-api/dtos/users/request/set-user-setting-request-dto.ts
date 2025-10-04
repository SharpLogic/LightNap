import { UserSettingKeys } from "@core/backend-api/user-setting-keys";

/**
 * Request for setting a user setting.
 */
export interface SetUserSettingRequestDto {
  key: UserSettingKeys;
  value: string;
}
