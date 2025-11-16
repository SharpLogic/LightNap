import { Injectable } from '@angular/core';

/**
 * Mock ToastService for testing
 * 
 * Captures toast messages for verification in tests
 */
@Injectable()
export class MockToastService {
  public messages: Array<{ severity: string; summary: string; detail?: string }> = [];

  success(summary: string, detail?: string): void {
    this.messages.push({ severity: 'success', summary, detail });
  }

  info(summary: string, detail?: string): void {
    this.messages.push({ severity: 'info', summary, detail });
  }

  warn(summary: string, detail?: string): void {
    this.messages.push({ severity: 'warn', summary, detail });
  }

  error(summary: string, detail?: string): void {
    this.messages.push({ severity: 'error', summary, detail });
  }

  clear(): void {
    this.messages = [];
  }

  getLastMessage() {
    return this.messages.length > 0 ? this.messages[this.messages.length - 1] : null;
  }

  hasMessage(severity: string, summaryMatch?: string): boolean {
    return this.messages.some(
      msg => msg.severity === severity && (!summaryMatch || msg.summary.includes(summaryMatch))
    );
  }
}
