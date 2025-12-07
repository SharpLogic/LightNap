import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { BrandedCardComponent } from './branded-card.component';
import { LayoutService } from '@core/features/layout/services/layout.service';
import { Component, signal } from '@angular/core';

describe('BrandedCardComponent', () => {
  let component: BrandedCardComponent;
  let fixture: ComponentFixture<BrandedCardComponent>;
  let mockLayoutService: jasmine.SpyObj<LayoutService>;

  beforeEach(() => {
    mockLayoutService = jasmine.createSpyObj<LayoutService>('LayoutService', [], {
      isDarkTheme: signal(false),
      appName: 'TestApp',
    });

    TestBed.configureTestingModule({
      imports: [BrandedCardComponent],
      providers: [
        provideZonelessChangeDetection(),
        { provide: LayoutService, useValue: mockLayoutService },
      ],
    });

    fixture = TestBed.createComponent(BrandedCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('Logo Display', () => {
    it('should inject LayoutService', () => {
      expect(component.layoutService).toBe(mockLayoutService);
    });

    it('should display light theme logo when isDarkTheme is false', () => {
      const compiled = fixture.nativeElement as HTMLElement;
      const img = compiled.querySelector('img');

      expect(img?.src).toContain('logo-light.png');
    });

    it('should display dark theme logo when isDarkTheme is true', () => {
      // Create new mock service with dark theme
      const darkThemeMock = jasmine.createSpyObj<LayoutService>('LayoutService', [], {
        isDarkTheme: signal(true),
        appName: 'TestApp',
      });

      // Reset TestBed for this specific test
      TestBed.resetTestingModule();
      TestBed.configureTestingModule({
        imports: [BrandedCardComponent],
        providers: [
          provideZonelessChangeDetection(),
          { provide: LayoutService, useValue: darkThemeMock },
        ],
      });

      fixture = TestBed.createComponent(BrandedCardComponent);
      fixture.detectChanges();

      const compiled = fixture.nativeElement as HTMLElement;
      const img = compiled.querySelector('img');

      expect(img?.src).toContain('logo-dark.png');
    });

    it('should display alt text with app name', () => {
      const compiled = fixture.nativeElement as HTMLElement;
      const img = compiled.querySelector('img');

      expect(img?.alt).toBe('TestApp logo');
    });
  });

  describe('Content Projection', () => {
    @Component({
      selector: 'ln-test-host',
      template: `
        <ln-branded-card>
          <div title>Test Title</div>
          <div subtitle>Test Subtitle</div>
          <div>Test Content</div>
        </ln-branded-card>
      `,
      imports: [BrandedCardComponent],
    })
    class TestHostComponent {}

    it('should project title content', () => {
      const hostFixture = TestBed.createComponent(TestHostComponent);
      hostFixture.detectChanges();

      const compiled = hostFixture.nativeElement as HTMLElement;
      const titleDiv = compiled.querySelector('.text-3xl');

      expect(titleDiv?.textContent?.trim()).toBe('Test Title');
    });

    it('should project subtitle content', () => {
      const hostFixture = TestBed.createComponent(TestHostComponent);
      hostFixture.detectChanges();

      const compiled = hostFixture.nativeElement as HTMLElement;
      const spans = compiled.querySelectorAll('span');
      let subtitleFound = false;

      spans.forEach((span) => {
        if (span.textContent?.trim() === 'Test Subtitle') {
          subtitleFound = true;
        }
      });

      expect(subtitleFound).toBe(true);
    });

    it('should project default content', () => {
      const hostFixture = TestBed.createComponent(TestHostComponent);
      hostFixture.detectChanges();

      const compiled = hostFixture.nativeElement as HTMLElement;
      const cardContent = compiled.textContent || '';

      expect(cardContent).toContain('Test Content');
    });

    it('should render card with all three content areas', () => {
      const hostFixture = TestBed.createComponent(TestHostComponent);
      hostFixture.detectChanges();

      const compiled = hostFixture.nativeElement as HTMLElement;
      const cardContent = compiled.textContent || '';

      expect(cardContent).toContain('Test Title');
      expect(cardContent).toContain('Test Subtitle');
      expect(cardContent).toContain('Test Content');
    });
  });

  describe('Structure and Styling', () => {
    it('should have the gradient wrapper div', () => {
      const compiled = fixture.nativeElement as HTMLElement;
      const gradientDiv = compiled.querySelector('[style*="linear-gradient"]');

      expect(gradientDiv).toBeTruthy();
    });

    it('should have the surface background div', () => {
      const compiled = fixture.nativeElement as HTMLElement;
      const surfaceDiv = compiled.querySelector('.bg-surface-0');

      expect(surfaceDiv).toBeTruthy();
    });

    it('should have centered content container', () => {
      const compiled = fixture.nativeElement as HTMLElement;
      const centerDiv = compiled.querySelector('.text-center');

      expect(centerDiv).toBeTruthy();
    });
  });
});
