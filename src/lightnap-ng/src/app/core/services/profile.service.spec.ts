import { provideZonelessChangeDetection } from "@angular/core";
import { TestBed } from "@angular/core/testing";
import { UpdateProfileRequestDto, LayoutConfigDto } from "@core/backend-api";
import { ProfileDataService } from "@core/backend-api/services/profile-data.service";
import { of } from "rxjs";
import { IdentityService } from "./identity.service";
import { ProfileService } from "./profile.service";
import { TimerService } from "./timer.service";

describe("ProfileService", () => {
  let service: ProfileService;
  let dataServiceSpy: jasmine.SpyObj<ProfileDataService>;
  let timerServiceSpy: jasmine.SpyObj<TimerService>;
  let identityServiceSpy: jasmine.SpyObj<IdentityService>;

  beforeEach(() => {
    const dataSpy = jasmine.createSpyObj("ProfileDataService", [
      "getProfile",
      "updateProfile",
      "getSettings",
      "setSetting",
    ]);
    const identitySpy = jasmine.createSpyObj("IdentityService", ["watchLoggedIn$"]);
    const timerSpy = jasmine.createSpyObj("TimerService", ["watchTimer$"]);

    TestBed.configureTestingModule({
      providers: [
        provideZonelessChangeDetection(),
        ProfileService,
        { provide: ProfileDataService, useValue: dataSpy },
        { provide: IdentityService, useValue: identitySpy },
        { provide: TimerService, useValue: timerSpy },
      ],
    });

    timerServiceSpy = TestBed.inject(TimerService) as jasmine.SpyObj<TimerService>;
    timerServiceSpy.watchTimer$.and.returnValue(of(0));

    identityServiceSpy = TestBed.inject(IdentityService) as jasmine.SpyObj<IdentityService>;
    identityServiceSpy.watchLoggedIn$.and.returnValue(of(true));

    dataServiceSpy = TestBed.inject(ProfileDataService) as jasmine.SpyObj<ProfileDataService>;
    service = TestBed.inject(ProfileService);
  });

  it("should be created", () => {
    expect(service).toBeTruthy();
  });

  it("should get profile", () => {
    dataServiceSpy.getProfile.and.returnValue(of({} as any));

    service.getProfile().subscribe();

    expect(dataServiceSpy.getProfile).toHaveBeenCalled();
  });

  it("should update profile", () => {
    const updateProfileRequest: UpdateProfileRequestDto = {} as any;
    dataServiceSpy.updateProfile.and.returnValue(of({} as any));

    service.updateProfile(updateProfileRequest).subscribe();

    expect(dataServiceSpy.updateProfile).toHaveBeenCalledWith(updateProfileRequest);
  });

  it("should get settings", () => {
    dataServiceSpy.getSettings.and.returnValue(of([{}] as any));

    service.getSettings().subscribe();

    expect(dataServiceSpy.getSettings).toHaveBeenCalled();
    expect(service.hasLoadedStyleSettings()).toBeTrue();
  });

  it("should update settings", () => {
    const browserSettings: LayoutConfigDto = {} as any;
    dataServiceSpy.setSetting.and.returnValue(of({} as any));

    service.setSetting("BrowserSettings", browserSettings).subscribe();

    expect(dataServiceSpy.setSetting).toHaveBeenCalledWith({ key: "BrowserSettings", value: JSON.stringify(browserSettings) });
  });

  it("should update style settings", () => {
    const clientSettings: LayoutConfigDto = { source: "server" } as any;
    const serverSettings: LayoutConfigDto = { source: "client" } as any;
    dataServiceSpy.getSettings.and.returnValue(of([{ key: "BrowserSettings", value: JSON.stringify(serverSettings) }] as any));
    dataServiceSpy.setSetting.and.returnValue(of({} as any));

    service.updateStyleSettings(clientSettings).subscribe();

    expect(dataServiceSpy.getSettings).toHaveBeenCalled();
    expect(dataServiceSpy.setSetting).toHaveBeenCalled();
  });

  it("should get default style settings", () => {
    const defaultStyleSettings = service.getDefaultStyleSettings();
    expect(defaultStyleSettings).toBeTruthy();
  });
});
