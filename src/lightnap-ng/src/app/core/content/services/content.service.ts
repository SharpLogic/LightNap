import { inject, Injectable } from "@angular/core";
import { ExtendedMap, IdentityService } from "@core";
import {
    CreateStaticContentDto,
    CreateStaticContentLanguageDto,
    SearchStaticContentRequestDto,
    UpdateStaticContentDto,
    UpdateStaticContentLanguageDto,
} from "@core/backend-api/dtos/static-contents";
import { ContentDataService } from "@core/backend-api/services/content-data.service";
import { map, Observable, shareReplay, switchMap, take } from "rxjs";
import { PublishedContent } from "../entities";

@Injectable({
  providedIn: "root",
})
export class ContentService {
  #dataService = inject(ContentDataService);
  #identityService = inject(IdentityService);

  #supportedLanguages$ = this.#dataService.getSupportedLanguages().pipe(shareReplay({ bufferSize: 1, refCount: false }));

  #publishedContentCache = new ExtendedMap<string, Observable<PublishedContent | null>>();

  getPublishedStaticContent(key: string, languageCode: string) {
    const cacheKey = `${key}:${languageCode}`;

    return this.#publishedContentCache.getOrSetDefault(cacheKey, () =>
      this.#identityService.watchLoggedIn$().pipe(
        take(1),
        switchMap(_ =>
          this.#dataService.getPublishedStaticContent(key, languageCode).pipe(map(result => (result ? new PublishedContent(result) : null)))
        ),
        shareReplay({ bufferSize: 1, refCount: false })
      )
    );
  }

  clearCachedPublishedStaticContent(key: string, languageCode: string) {
    const cacheKey = `${key}:${languageCode}`;
    this.#publishedContentCache.delete(cacheKey);
  }

  getSupportedLanguages() {
    return this.#supportedLanguages$;
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
