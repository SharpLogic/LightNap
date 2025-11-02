import { inject, Injectable } from "@angular/core";
import { ExtendedMap } from "@core";
import {
    CreateStaticContentDto,
    CreateStaticContentLanguageDto,
    SearchStaticContentRequestDto,
    UpdateStaticContentDto,
    UpdateStaticContentLanguageDto,
} from "@core/backend-api/dtos/static-contents";
import { ContentDataService } from "@core/backend-api/services/content-data.service";
import { IdentityService } from "@core/services/identity.service";
import { PrivilegedUsersService } from "@core/users/services/privileged-users.service";
import { map, Observable, shareReplay, switchMap, take, tap } from "rxjs";
import { PublishedContent } from "../entities";

@Injectable({
  providedIn: "root",
})
export class ContentService {
  #dataService = inject(ContentDataService);
  #identityService = inject(IdentityService);
  #usersService = inject(PrivilegedUsersService);

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
    return this.#dataService.updateStaticContent(key, updateDto).pipe(
      switchMap(staticContent =>
        this.getSupportedLanguages().pipe(
          take(1),
          tap(languages => languages.forEach(language => this.clearCachedPublishedStaticContent(key, language.languageCode))),
          map(_ => staticContent)
        )
      )
    );
  }

  deleteStaticContent(key: string) {
    return this.#dataService.deleteStaticContent(key);
  }

  addReader(userId: string, key: string) {
    return this.#usersService.addUserClaim(userId, { type: "Content:Reader", value: key });
  }

  removeReader(userId: string, key: string) {
    return this.#usersService.removeUserClaim(userId, { type: "Content:Reader", value: key });
  }

  addEditor(userId: string, key: string) {
    return this.#usersService.addUserClaim(userId, { type: "Content:Editor", value: key });
  }

  removeEditor(userId: string, key: string) {
    return this.#usersService.removeUserClaim(userId, { type: "Content:Editor", value: key });
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
    return this.#dataService.updateStaticContentLanguage(key, languageCode, updateDto).pipe(
      tap(_ => {
        this.clearCachedPublishedStaticContent(key, languageCode);
      })
    );
  }

  deleteStaticContentLanguage(key: string, languageCode: string) {
    return this.#dataService.deleteStaticContentLanguage(key, languageCode);
  }
}
