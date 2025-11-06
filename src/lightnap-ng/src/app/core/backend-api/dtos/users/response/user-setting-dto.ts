import { UserSettingKey } from "@core/backend-api/user-setting-key";

/**
 * Represents a user setting.
 */
export interface UserSettingDto {
  key: UserSettingKey;
  value: string;
  createdDate?: Date;
  lastModifiedDate?: Date;
}
