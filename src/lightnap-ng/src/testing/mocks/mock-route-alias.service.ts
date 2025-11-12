import { Injectable } from '@angular/core';

/**
 * Mock RouteAliasService for testing
 * 
 * Tracks navigation calls without performing actual routing
 */
@Injectable()
export class MockRouteAliasService {
  public lastNavigatedAlias?: string;
  public lastNavigatedPath?: string[];
  public navigationCount = 0;

  navigate(alias: string, ...args: any[]): Promise<boolean> {
    this.lastNavigatedAlias = alias;
    this.navigationCount++;
    return Promise.resolve(true);
  }

  navigateByUrl(path: string[]): Promise<boolean> {
    this.lastNavigatedPath = path;
    this.navigationCount++;
    return Promise.resolve(true);
  }

  getUrl(alias: string): string {
    return `/mock/${alias}`;
  }

  reset(): void {
    this.lastNavigatedAlias = undefined;
    this.lastNavigatedPath = undefined;
    this.navigationCount = 0;
  }
}
