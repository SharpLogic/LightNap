import { inject, Injectable } from "@angular/core";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";
import { LayoutConfigDto, SetUserSettingRequestDto, UpdateProfileRequestDto, UserSettingDto, UserSettingKeys } from "@core/backend-api";
import { ProfileDataService } from "@core/backend-api/services/profile-data.service";
import { filter, map, Observable, of, shareReplay, switchMap, tap } from "rxjs";
import { IdentityService } from "./identity.service";

@Injectable({
  providedIn: "root",
})
/**
 * @class ProfileService
 * @description
 * The ProfileService class provides methods to manage user profiles and application settings.
 */
export class ProfileService {
  #dataService = inject(ProfileDataService);
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
    return this.#dataService.updateProfile(updateProfileRequest);
  }

  /**
   * @method getSettings
   * @description Fetches the browser settings. If settings are already loaded, returns them from memory.
   * @returns {Observable<Array<UserSettingDto>>} An observable containing the application settings.
   */
  getSettings() {
    if (this.#settings.length) return of(this.#settings);
    if (!this.#settings$) {
      this.#settings$ = this.#dataService.getSettings().pipe(
        shareReplay(1),
        tap(settings => (this.#settings = settings))
      );
    }

    return this.#settings$;
  }

  getSetting<T>(key: UserSettingKeys, defaultValue?: T) {
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
   * @description Updates an application setting.
   * @param {ApplicationSettingsDto} applicationSettings - The new application setting to be updated.
   * @returns {Observable<boolean>} An observable containing true if successful.
   */
  setSetting<T>(key: UserSettingKeys, value: T) {
    return this.#dataService
      .setSetting(<SetUserSettingRequestDto>{
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
    return this.getSetting<LayoutConfigDto>("BrowserSettings", this.#defaultBrowserSettings);
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
        return this.setSetting("BrowserSettings", layoutConfig);
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
}
