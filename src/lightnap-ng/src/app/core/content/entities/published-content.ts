import { PublishedStaticContentResultDto, StaticContentFormats, StaticContentUserVisibilities } from "@core/backend-api";

export class PublishedContent {
  readonly visibility: StaticContentUserVisibilities;
  readonly content?: string;
  readonly format?: StaticContentFormats;
  readonly requiresAuthentication: boolean;
  readonly isRestricted: boolean;
  readonly canView: boolean;
  readonly canEdit: boolean;

  constructor(result: PublishedStaticContentResultDto) {
    this.visibility = result.visibility;
    this.format = result.content?.format;
    this.content = result.content?.content;

    this.requiresAuthentication = this.visibility === "RequiresAuthentication";
    this.isRestricted = this.visibility === "Restricted";
    this.canEdit = this.visibility === "Editor";
    this.canView = this.canEdit || this.visibility === "Reader";
  }
}
