import { inject, Injectable } from "@angular/core";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";
import {
    LayoutConfigDto,
    SetUserSettingRequestDto,
    UpdateProfileRequestDto,
    UserSettingDto,
    UserSettingKey,
    UserSettingKeys,
} from "@core/backend-api";
import { filter, map, Observable, of, shareReplay, switchMap, tap } from "rxjs";
import { IdentityService } from "./identity.service";
import { LightNapWebApiService } from "@core/backend-api/services/lightnap-api";

@Injectable({
  providedIn: "root",
})
/**
 * @class ProfileService
 * @description
 * The ProfileService class provides methods to manage user profiles and application settings.
 */
export class ProfileService {
  #dataService = inject(LightNapWebApiService);
  #identityService = inject(IdentityService);

  #defaultBrowserSettings = <LayoutConfigDto>{
    preset: "Aura",
    primary: "violet",
    surface: null,
    darkTheme: false,
    menuMode: "static",
  };

  #settings$?: Observable<Array<UserSettingDto>>;
  #settings = new Array<UserSettingDto>();

  /**
   * Constructs the ProfileService and sets up the subscription to handle user logout.
   */
  constructor() {
    this.#identityService
      .watchLoggedIn$()
      .pipe(
        takeUntilDestroyed(),
        filter(loggedIn => !loggedIn)
      )
      .subscribe({
        next: _ => {
          this.#settings$ = undefined;
          this.#settings = [];
        },
      });
  }

  /**
   * @method getProfile
   * @description Fetches the user profile.
   * @returns {Observable<Profile>} An observable containing the user profile.
   */
  getProfile() {
    return this.#dataService.getProfile();
  }

  /**
   * @method updateProfile
   * @description Updates the user profile.
   * @param {UpdateProfileRequestDto} updateProfileRequest - The request object containing profile update information.
   * @returns {Observable<Profile>} An observable containing the updated profile.
   */
  updateProfile(updateProfileRequest: UpdateProfileRequestDto) {
    return this.#dataService.updateMyProfile(updateProfileRequest);
  }

  /**
   * @method getSettings
   * @description Fetches the browser settings. If settings are already loaded, returns them from memory.
   * @returns {Observable<Array<UserSettingDto>>} An observable containing the application settings.
   */
  getSettings() {
    if (this.#settings.length) return of(this.#settings);
    if (!this.#settings$) {
      this.#settings$ = this.#dataService.getMyUserSettings().pipe(
        map(settings => settings || []),
        shareReplay(1),
        tap(settings => (this.#settings = settings))
      );
    }

    return this.#settings$;
  }

  getSetting<T>(key: UserSettingKey, defaultValue?: T) {
    return this.getSettings().pipe(
      map(settings => {
        const setting = settings.find(s => s.key === key);
        if (!setting) throw new Error(`Setting with key ${key} not found`);
        if (!setting.value.length) {
          if (defaultValue !== undefined) return defaultValue;
          throw new Error(`Setting with key ${key} has no value`);
        }

        return JSON.parse(setting.value) as T;
      })
    );
  }

  /**
   * @method setSetting
   * @description Sets a user setting.
   * @param {UserSettingKey} key - The key of the setting to be set.
   * @param {T} value - The value to be set for the specified key.
   * @returns {Observable<UserSettingDto>} An observable containing the updated user setting.
   */
  setSetting<T>(key: UserSettingKey, value: T) {
    return this.#dataService
      .setMyUserSetting(<SetUserSettingRequestDto>{
        key,
        value: JSON.stringify(value),
      })
      .pipe(
        tap(setting => {
          this.#settings = this.#settings.filter(s => s.key !== key);
          this.#settings.push(setting);
        })
      );
  }

  getStyleSettings() {
    return this.getSetting<LayoutConfigDto>(UserSettingKeys.BrowserSettings, this.#defaultBrowserSettings);
  }

  /**
   * @method updateStyleSettings
   * @description Updates the style settings of the application.
   * @param {LayoutConfigDto} layoutConfig - The new style settings to be updated.
   * @returns {Observable<boolean>} An observable containing true if successful.
   */
  updateStyleSettings(layoutConfig: LayoutConfigDto) {
    return this.getStyleSettings().pipe(
      switchMap(styleSettings => {
        if (JSON.stringify(styleSettings) === JSON.stringify(layoutConfig)) {
          return of(layoutConfig);
        }
        return this.setSetting(UserSettingKeys.BrowserSettings, layoutConfig);
      })
    );
  }

  /**
   * @method getDefaultStyleSettings
   * @description Retrieves the default style settings.
   * @returns {LayoutConfigDto} The default style settings.
   */
  getDefaultStyleSettings() {
    return JSON.parse(JSON.stringify(this.#defaultBrowserSettings)) as LayoutConfigDto;
  }

  /**
   * @method hasLoadedStyleSettings
   * @description Checks if the style settings have been loaded.
   * @returns {boolean} True if the style settings have been loaded, false otherwise.
   */
  hasLoadedStyleSettings() {
    return !!this.#settings.length;
  }

  /**
   * @method getExternalLogins
   * @description Retrieves the list of external logins associated with the user.
   * @returns {Observable<Array<ExternalLoginDto>>} An observable containing the list of external logins.
   */
  getExternalLogins() {
    return this.#dataService.getMyExternalLogins();
  }

  /**
   * @method removeExternalLogin
   * @description Removes an external login associated with the user.
   * @param {string} loginProvider - The login provider of the external login to be removed.
   * @param {string} providerKey - The provider key of the external login to be removed.
   * @returns {Observable<boolean>} An observable indicating whether the removal was successful.
   */
  removeExternalLogin(loginProvider: string, providerKey: string) {
    return this.#dataService.removeMyExternalLogin(loginProvider, providerKey);
  }
}
