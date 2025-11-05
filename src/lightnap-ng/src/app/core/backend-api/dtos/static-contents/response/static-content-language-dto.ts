import { PublishedStaticContentDto } from "./published-static-content-dto";

/**
 * Static content details for a specific language.
 */
export interface StaticContentLanguageDto extends PublishedStaticContentDto {
  staticContentId: number;
  languageCode: string;
  createdDate: Date;
  createdByUserId?: string;
  lastModifiedDate?: Date;
  lastModifiedByUserId?: string;
}
