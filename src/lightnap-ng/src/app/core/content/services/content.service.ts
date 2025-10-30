import { Injectable, inject } from "@angular/core";
import {
  CreateStaticContentDto,
  CreateStaticContentLanguageDto,
  SearchStaticContentRequestDto,
  UpdateStaticContentDto,
  UpdateStaticContentLanguageDto,
} from "@core/backend-api/dtos/static-contents";
import { ContentDataService } from "@core/backend-api/services/content-data.service";
import { map, switchMap, take } from "rxjs";
import { PublishedContent } from "../entities";
import { IdentityService } from "@core";

@Injectable({
  providedIn: "root",
})
export class ContentService {
  #dataService = inject(ContentDataService);
  #identityService = inject(IdentityService);

  getPublishedStaticContent(key: string, languageCode: string) {
    // We need to ensure the user's identity status has been established before making requests for content.
    return this.#identityService.watchLoggedIn$().pipe(
      take(1),
      switchMap(_ =>
        this.#dataService.getPublishedStaticContent(key, languageCode).pipe(map(result => (result ? new PublishedContent(result) : null)))
      )
    );
  }

  getSupportedLanguages() {
    return this.#dataService.getSupportedLanguages();
  }

  createStaticContent(createDto: CreateStaticContentDto) {
    return this.#dataService.createStaticContent(createDto);
  }

  getStaticContent(key: string) {
    return this.#dataService.getStaticContent(key);
  }

  searchStaticContent(searchDto: SearchStaticContentRequestDto) {
    return this.#dataService.searchStaticContent(searchDto);
  }

  updateStaticContent(key: string, updateDto: UpdateStaticContentDto) {
    return this.#dataService.updateStaticContent(key, updateDto);
  }

  deleteStaticContent(key: string) {
    return this.#dataService.deleteStaticContent(key);
  }

  getStaticContentLanguage(key: string, languageCode: string) {
    return this.#dataService.getStaticContentLanguage(key, languageCode);
  }

  getStaticContentLanguages(key: string) {
    return this.#dataService.getStaticContentLanguages(key);
  }

  createStaticContentLanguage(key: string, languageCode: string, createDto: CreateStaticContentLanguageDto) {
    return this.#dataService.createStaticContentLanguage(key, languageCode, createDto);
  }

  updateStaticContentLanguage(key: string, languageCode: string, updateDto: UpdateStaticContentLanguageDto) {
    return this.#dataService.updateStaticContentLanguage(key, languageCode, updateDto);
  }

  deleteStaticContentLanguage(key: string, languageCode: string) {
    return this.#dataService.deleteStaticContentLanguage(key, languageCode);
  }
}
