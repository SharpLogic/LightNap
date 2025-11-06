import { UserSettingKey } from "@core";

/**
 * Request for setting a user setting.
 */
export interface SetUserSettingRequestDto {
  key: UserSettingKey;
  value: string;
}
