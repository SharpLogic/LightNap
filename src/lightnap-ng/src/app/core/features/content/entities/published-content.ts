import {
    PublishedStaticContentResultDto,
    StaticContentFormat,
    StaticContentUserVisibilities,
    StaticContentUserVisibility
} from "@core/backend-api";

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

    this.requiresAuthentication = this.visibility === StaticContentUserVisibilities.RequiresAuthentication;
    this.isRestricted = this.visibility === StaticContentUserVisibilities.Restricted;
    this.canEdit = this.visibility === StaticContentUserVisibilities.Editor;
    this.canView = this.canEdit || this.visibility === StaticContentUserVisibilities.Reader;
  }
}
