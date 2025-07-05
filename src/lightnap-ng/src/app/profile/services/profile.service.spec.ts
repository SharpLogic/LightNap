import { TestBed } from '@angular/core/testing';
import { SuccessApiResponse, TimerService } from '@core';
import { IdentityService } from '@identity';
import { ApplicationSettings, ChangePasswordRequest, LayoutConfig, UpdateProfileRequest } from '@profile';
import { of } from 'rxjs';
import { DataService } from './data.service';
import { ProfileService } from './profile.service';
import { provideZonelessChangeDetection } from '@angular/core';

describe('ProfileService', () => {
    let service: ProfileService;
    let dataServiceSpy: jasmine.SpyObj<DataService>;
    let timerServiceSpy: jasmine.SpyObj<TimerService>;
    let identityServiceSpy: jasmine.SpyObj<IdentityService>;

    beforeEach(() => {
        const dataSpy = jasmine.createSpyObj('DataService', ['getProfile', 'updateProfile', 'getDevices', 'revokeDevice', 'changePassword', 'getSettings', 'updateSettings']);
        const identitySpy = jasmine.createSpyObj('IdentityService', ['watchLoggedIn$']);
        const timerSpy = jasmine.createSpyObj('TimerService', ['watchTimer$']);

        TestBed.configureTestingModule({
            providers: [
                provideZonelessChangeDetection(),
                ProfileService,
                { provide: DataService, useValue: dataSpy },
                { provide: IdentityService, useValue: identitySpy },
                { provide: TimerService, useValue: timerSpy }
            ]
        });

        timerServiceSpy = TestBed.inject(TimerService) as jasmine.SpyObj<TimerService>;
        timerServiceSpy.watchTimer$.and.returnValue(of(0));

        identityServiceSpy = TestBed.inject(IdentityService) as jasmine.SpyObj<IdentityService>;
        identityServiceSpy.watchLoggedIn$.and.returnValue(of(true));

        dataServiceSpy = TestBed.inject(DataService) as jasmine.SpyObj<DataService>;
        service = TestBed.inject(ProfileService);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });

    it('should get profile', () => {
        dataServiceSpy.getProfile.and.returnValue(of({} as any));

        service.getProfile().subscribe();

        expect(dataServiceSpy.getProfile).toHaveBeenCalled();
    });

    it('should update profile', () => {
        const updateProfileRequest: UpdateProfileRequest = {} as any;
        dataServiceSpy.updateProfile.and.returnValue(of({} as any));

        service.updateProfile(updateProfileRequest).subscribe();

        expect(dataServiceSpy.updateProfile).toHaveBeenCalledWith(updateProfileRequest);
    });

    it('should get devices', () => {
        dataServiceSpy.getDevices.and.returnValue(of({} as any));

        service.getDevices().subscribe();

        expect(dataServiceSpy.getDevices).toHaveBeenCalled();
    });

    it('should revoke device', () => {
        const deviceId = 'device123';
        dataServiceSpy.revokeDevice.and.returnValue(of({} as any));

        service.revokeDevice(deviceId).subscribe();

        expect(dataServiceSpy.revokeDevice).toHaveBeenCalledWith(deviceId);
    });

    it('should change password', () => {
        const changePasswordRequest: ChangePasswordRequest = {} as any;
        dataServiceSpy.changePassword.and.returnValue(of({} as any));

        service.changePassword(changePasswordRequest).subscribe();

        expect(dataServiceSpy.changePassword).toHaveBeenCalledWith(changePasswordRequest);
    });

    it('should get settings', () => {
        dataServiceSpy.getSettings.and.returnValue(of({} as any));

        service.getSettings().subscribe();

        expect(dataServiceSpy.getSettings).toHaveBeenCalled();
        expect(service.hasLoadedStyleSettings()).toBeTrue();
    });

    it('should update settings', () => {
        const browserSettings: ApplicationSettings = { } as any;
        dataServiceSpy.updateSettings.and.returnValue(of({} as any));

        service.updateSettings(browserSettings).subscribe();

        expect(dataServiceSpy.updateSettings).toHaveBeenCalledWith(browserSettings);
    });

    it('should update style settings', () => {
        const styleSettings: LayoutConfig = {} as any;
        const expectedSettings: ApplicationSettings = {} as any;
        dataServiceSpy.getSettings.and.returnValue(of(expectedSettings));
        dataServiceSpy.updateSettings.and.returnValue(of({} as any));

        service.updateStyleSettings(styleSettings).subscribe();

        expect(dataServiceSpy.getSettings).toHaveBeenCalled();
        expect(dataServiceSpy.updateSettings).toHaveBeenCalled();
    });

    it('should get default style settings', () => {
        const defaultStyleSettings = service.getDefaultStyleSettings();
        expect(defaultStyleSettings).toBeTruthy();
    });
});
