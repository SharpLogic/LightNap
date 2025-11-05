import { StaticContentLanguageDto } from "../dtos";

export class StaticContentLanguageHelper {
  public static rehydrate(staticContentLanguage: StaticContentLanguageDto | null) {
    if (staticContentLanguage?.createdDate) {
      staticContentLanguage.createdDate = new Date(staticContentLanguage.createdDate);
    }
    if (staticContentLanguage?.lastModifiedDate) {
      staticContentLanguage.lastModifiedDate = new Date(staticContentLanguage.lastModifiedDate);
    }
  }
}
