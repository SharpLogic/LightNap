import { TestBed } from '@angular/core/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { PublicService } from './public.service';
import { PublicDataService } from '@core/backend-api/services/public-data.service';

describe('PublicService', () => {
  let service: PublicService;
  let dataServiceSpy: jasmine.SpyObj<PublicDataService>;

  beforeEach(() => {
    dataServiceSpy = jasmine.createSpyObj('PublicDataService', ['getData']);

    TestBed.configureTestingModule({
      providers: [
        provideZonelessChangeDetection(),
        PublicService,
        { provide: PublicDataService, useValue: dataServiceSpy },
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
