import type { MockedObject } from "vitest";
import { provideZonelessChangeDetection } from "@angular/core";
import { TestBed } from "@angular/core/testing";
import { UpdateProfileRequestDto, LayoutConfigDto } from "@core/backend-api";
import { getGetProfileResponseMock, getGetMyUserSettingsResponseMock, getSetMyUserSettingResponseMock, LightNapWebApiService, } from "@core/backend-api/services/lightnap-api";
import { createLightNapWebApiServiceSpy } from "@testing/helpers";
import { of } from "rxjs";
import { IdentityService } from "./identity.service";
import { ProfileService } from "./profile.service";
import { TimerService } from "./timer.service";

describe("ProfileService", () => {
    let service: ProfileService;
    let webApiServiceSpy: MockedObject<LightNapWebApiService>;
    let timerServiceSpy: MockedObject<TimerService>;
    let identityServiceSpy: MockedObject<IdentityService>;

    beforeEach(() => {
        const timerSpy = {
            watchTimer$: vi.fn().mockName("TimerService.watchTimer$")
        };
        const identitySpy = {
            watchLoggedIn$: vi.fn().mockName("IdentityService.watchLoggedIn$")
        };
        const webApiSpy = createLightNapWebApiServiceSpy();

        TestBed.configureTestingModule({
            providers: [
                provideZonelessChangeDetection(),
                ProfileService,
                { provide: LightNapWebApiService, useValue: webApiSpy },
                { provide: IdentityService, useValue: identitySpy },
                { provide: TimerService, useValue: timerSpy },
            ],
        });

        timerServiceSpy = TestBed.inject(TimerService) as MockedObject<TimerService>;
        timerServiceSpy.watchTimer$.mockReturnValue(of(0));

        identityServiceSpy = TestBed.inject(IdentityService) as MockedObject<IdentityService>;
        identityServiceSpy.watchLoggedIn$.mockReturnValue(of(true));

        webApiServiceSpy = TestBed.inject(LightNapWebApiService) as MockedObject<LightNapWebApiService>;
        service = TestBed.inject(ProfileService);
    });

    it("should be created", () => {
        expect(service).toBeTruthy();
    });

    describe("profile management", () => {
        it("should get profile", () => {
            const profileResponse = getGetProfileResponseMock();
            webApiServiceSpy.getProfile.mockReturnValue(of(profileResponse) as any);

            service.getProfile().subscribe();

            expect(webApiServiceSpy.getProfile).toHaveBeenCalled();
        });

        it("should update profile", () => {
            const updateProfileRequest: UpdateProfileRequestDto = {} as any;
            const profileResponse = getGetProfileResponseMock();
            webApiServiceSpy.updateMyProfile.mockReturnValue(of(profileResponse) as any);

            service.updateProfile(updateProfileRequest).subscribe();

            expect(webApiServiceSpy.updateMyProfile).toHaveBeenCalledWith(updateProfileRequest);
        });
    });

    describe("settings management", () => {
        it("should get settings", () => {
            const settingsResponse = getGetMyUserSettingsResponseMock();
            webApiServiceSpy.getMyUserSettings.mockReturnValue(of(settingsResponse) as any);

            service.getSettings().subscribe();

            expect(webApiServiceSpy.getMyUserSettings).toHaveBeenCalled();
            expect(service.hasLoadedStyleSettings()).toBe(true);
        });

        it("should update settings", () => {
            const browserSettings: LayoutConfigDto = {} as any;
            const settingResponse = getSetMyUserSettingResponseMock();
            webApiServiceSpy.setMyUserSetting.mockReturnValue(of(settingResponse) as any);

            service.setSetting("BrowserSettings", browserSettings).subscribe();

            expect(webApiServiceSpy.setMyUserSetting).toHaveBeenCalledWith({ key: "BrowserSettings", value: JSON.stringify(browserSettings) });
        });

        it("should update style settings", () => {
            const clientSettings: LayoutConfigDto = { source: "server" } as any;
            const serverSettings: LayoutConfigDto = { source: "client" } as any;
            const settingsResponse = [
                { key: "BrowserSettings", value: JSON.stringify(serverSettings) },
            ] as any;
            const settingResponse = getSetMyUserSettingResponseMock();

            webApiServiceSpy.getMyUserSettings.mockReturnValue(of(settingsResponse) as any);
            webApiServiceSpy.setMyUserSetting.mockReturnValue(of(settingResponse) as any);

            service.updateStyleSettings(clientSettings).subscribe();

            expect(webApiServiceSpy.getMyUserSettings).toHaveBeenCalled();
            expect(webApiServiceSpy.setMyUserSetting).toHaveBeenCalled();
        });

        it("should get default style settings", () => {
            const defaultStyleSettings = service.getDefaultStyleSettings();
            expect(defaultStyleSettings).toBeTruthy();
        });
    });
});
