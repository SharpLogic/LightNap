import { UserSettingKeys } from "@core/backend-api/user-setting-keys";

/**
 * Represents a user setting.
 */
export interface UserSettingDto {
  key: UserSettingKeys;
  value: string;
  createdDate?: Date;
  lastModifiedDate?: Date;
}
