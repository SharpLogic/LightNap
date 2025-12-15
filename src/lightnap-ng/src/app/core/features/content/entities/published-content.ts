import { PublishedStaticContentResultDto, StaticContentFormat, StaticContentUserVisibility } from "@core/backend-api";

export class PublishedContent {
  readonly visibility: StaticContentUserVisibility;
  readonly content?: string;
  readonly format?: StaticContentFormat;
  readonly requiresAuthentication: boolean;
  readonly isRestricted: boolean;
  readonly canView: boolean;
  readonly canEdit: boolean;

  constructor(result: PublishedStaticContentResultDto) {
    this.visibility = result.visibility;
    this.format = result.content?.format;
    this.content = result.content?.content;

    this.requiresAuthentication = this.visibility === StaticContentUserVisibility.RequiresAuthentication;
    this.isRestricted = this.visibility === StaticContentUserVisibility.Restricted;
    this.canEdit = this.visibility === StaticContentUserVisibility.Editor;
    this.canView = this.canEdit || this.visibility === StaticContentUserVisibility.Reader;
  }
}
