import { inject, Injectable } from "@angular/core";
import { CreateStaticContentDto, CreateStaticContentLanguageDto, ExtendedMap, SearchStaticContentRequestDto, UpdateStaticContentDto, UpdateStaticContentLanguageDto } from "@core";
import { UserSettingKeys } from "@core/backend-api/user-setting-key";
import { PublishedContent } from "../entities";
import { PrivilegedUsersService } from "@core/features/users/services/privileged-users.service";
import { IdentityService } from "@core/services/identity.service";
import { ProfileService } from "@core/services/profile.service";
import { shareReplay, Observable, switchMap, of, map, catchError, tap } from "rxjs";
import { LightNapWebApiService } from "@core/backend-api/services/lightnap-api";

@Injectable({
  providedIn: "root",
})
export class ContentService {
  #dataService = inject(LightNapWebApiService);
  #identityService = inject(IdentityService);
  #profileService = inject(ProfileService);
  #usersService = inject(PrivilegedUsersService);

  #supportedLanguages$ = this.#dataService.getSupportedLanguages().pipe(shareReplay({ bufferSize: 1, refCount: false }));

  #publishedContentCache = new ExtendedMap<string, Observable<PublishedContent | null>>();

  /**
   * Gets the browser's default language code.
   * @returns The browser's default language code or "en" if not detectable.
   */
  #getBrowserLanguageCode(): string {
    const browserLang = navigator.language.split("-")[0];
    return browserLang || "en";
  }

  /**
   * Gets the user's preferred language. If empty or auto-detect, returns browser language or default fallback.
   */
  #getPreferredLanguageCode(): Observable<string> {
    return this.#identityService.getLoggedIn$().pipe(
      switchMap(isLoggedIn => {
        if (!isLoggedIn) return of(this.#getBrowserLanguageCode());

        return this.#profileService.getSetting<string>(UserSettingKeys.PreferredLanguage, "").pipe(
          map(preferredLanguage => {
            if (preferredLanguage?.length > 0) return preferredLanguage;
            return this.#getBrowserLanguageCode();
          }),
          catchError(() => of(this.#getBrowserLanguageCode()))
        );
      })
    );
  }

  getPublishedStaticContent(key: string, languageCode: string | null = null) {
    if (languageCode?.length) return this.#getPublishedStaticContentWithLanguage(key, languageCode);

    return this.#getPreferredLanguageCode().pipe(
      switchMap(resolvedLanguageCode => this.#getPublishedStaticContentWithLanguage(key, resolvedLanguageCode))
    );
  }

  #getPublishedStaticContentWithLanguage(key: string, languageCode: string) {
    const cacheKey = `${key}:${languageCode}`;

    return this.#publishedContentCache.getOrSetDefault(cacheKey, () =>
      this.#identityService.getLoggedIn$().pipe(
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
