import { UserSettingDto } from "../dtos";

export class UserSettingHelper {
  public static rehydrate(userSetting: UserSettingDto) {
    if (userSetting?.createdDate) {
      userSetting.createdDate = new Date(userSetting.createdDate);
    }

    if (userSetting?.lastModifiedDate) {
      userSetting.lastModifiedDate = new Date(userSetting.lastModifiedDate);
    }
  }
}
