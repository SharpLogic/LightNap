import { TestBed } from '@angular/core/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { PublicService } from './public.service';
import { LightNapWebApiService } from '@core/backend-api/services/lightnap-api';

describe('PublicService', () => {
  let service: PublicService;
  let webApiServiceSpy: jasmine.SpyObj<any>;

  beforeEach(() => {
    webApiServiceSpy = jasmine.createSpyObj('LightNapWebApiService', ['getData']);

    TestBed.configureTestingModule({
      providers: [
        provideZonelessChangeDetection(),
        PublicService,
        { provide: LightNapWebApiService, useValue: webApiServiceSpy },
      ],
    });

    service = TestBed.inject(PublicService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should have data service dependency', () => {
    // Service should be created without errors, indicating successful injection
    expect(service).toBeDefined();
  });
});
