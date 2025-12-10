import { TestBed } from '@angular/core/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { SincePipe } from './since.pipe';
import { MockTimerService } from '@testing';
import { TimerService } from '@core/services/timer.service';
import { firstValueFrom } from 'rxjs';

describe('SincePipe', () => {
  let pipe: SincePipe;
  let mockTimer: MockTimerService;

  beforeEach(() => {
    mockTimer = new MockTimerService();

    TestBed.configureTestingModule({
      providers: [
        provideZonelessChangeDetection(),
        SincePipe,
        { provide: TimerService, useValue: mockTimer },
      ],
    });

    pipe = TestBed.inject(SincePipe);
  });

  it('should create', () => {
    expect(pipe).toBeTruthy();
  });

  describe('null and undefined handling', () => {
    it('should return "--" for null or undefined date', async () => {
      const result = await firstValueFrom(pipe.transform(null as any));
      expect(result).toBe('--');
    });
  });

  describe('time format conversions', () => {
    it('should return "just now" for very recent dates', async () => {
      const now = new Date();
      const result = await firstValueFrom(pipe.transform(now));
      expect(result).toBe('just now');
    });

    it('should return minutes ago for dates within an hour', async () => {
      const date = new Date(Date.now() - 5 * 60 * 1000); // 5 minutes ago
      const result = await firstValueFrom(pipe.transform(date));
      expect(result).toBe('5m ago');
    });

    it('should return hours ago for dates within a day', async () => {
      const date = new Date(Date.now() - 3 * 60 * 60 * 1000); // 3 hours ago
      const result = await firstValueFrom(pipe.transform(date));
      expect(result).toBe('3h ago');
    });

    it('should return days ago for dates within a week', async () => {
      const date = new Date(Date.now() - 4 * 24 * 60 * 60 * 1000); // 4 days ago
      const result = await firstValueFrom(pipe.transform(date));
      expect(result).toBe('4d ago');
    });

    it('should return weeks ago for dates older than a week', async () => {
      const date = new Date(Date.now() - 2 * 7 * 24 * 60 * 60 * 1000); // 2 weeks ago
      const result = await firstValueFrom(pipe.transform(date));
      expect(result).toBe('2w ago');
    });

    it('should accept timestamp numbers', async () => {
      const timestamp = Date.now() - 10 * 60 * 1000; // 10 minutes ago
      const result = await firstValueFrom(pipe.transform(timestamp));
      expect(result).toBe('10m ago');
    });
  });

  describe('edge cases', () => {
    it('should handle boundary conditions correctly', async () => {
      // Exactly 1 minute
      const oneMinuteAgo = new Date(Date.now() - 60 * 1000);
      let result = await firstValueFrom(pipe.transform(oneMinuteAgo));
      expect(result).toBe('1m ago');

      // Exactly 1 hour
      const oneHourAgo = new Date(Date.now() - 60 * 60 * 1000);
      result = await firstValueFrom(pipe.transform(oneHourAgo));
      expect(result).toBe('1h ago');

      // Exactly 1 day
      const oneDayAgo = new Date(Date.now() - 24 * 60 * 60 * 1000);
      result = await firstValueFrom(pipe.transform(oneDayAgo));
      expect(result).toBe('1d ago');
    });
  });
});
