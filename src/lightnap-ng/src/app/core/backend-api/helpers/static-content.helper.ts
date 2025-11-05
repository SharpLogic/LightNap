import { StaticContentDto } from "../dtos";

export class StaticContentHelper {
  public static rehydrate(staticContent: StaticContentDto | null) {
    if (staticContent?.createdDate) {
      staticContent.createdDate = new Date(staticContent.createdDate);
    }
    if (staticContent?.lastModifiedDate) {
      staticContent.lastModifiedDate = new Date(staticContent.lastModifiedDate);
    }
    if (staticContent?.statusChangedDate) {
      staticContent.statusChangedDate = new Date(staticContent.statusChangedDate);
    }
  }
}
