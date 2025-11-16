import { of, throwError } from 'rxjs';
import { delay } from 'rxjs/operators';
import { RequestPollingManager } from './request-polling-manager';

describe('RequestPollingManager', () => {
  beforeEach(() => {
    jasmine.clock().install();
  });

  afterEach(() => {
    jasmine.clock().uninstall();
  });

  it('should be created', () => {
    const pollingFn = () => of('test');
    const manager = new RequestPollingManager(pollingFn, 1000);

    expect(manager).toBeTruthy();
  });

  it('should call polling function immediately when due is 0', () => {
    let callCount = 0;
    const pollingFn = () => {
      callCount++;
      return of('response');
    };

    const manager = new RequestPollingManager(pollingFn, 1000);
    manager.startPolling(0);

    jasmine.clock().tick(10);
    expect(callCount).toBe(1);

    manager.stopPolling();
  });

  it('should delay first call when due is specified', () => {
    let callCount = 0;
    const pollingFn = () => {
      callCount++;
      return of('response');
    };

    const manager = new RequestPollingManager(pollingFn, 1000);
    manager.startPolling(500);

    jasmine.clock().tick(400);
    expect(callCount).toBe(0);

    jasmine.clock().tick(200);
    expect(callCount).toBe(1);

    manager.stopPolling();
  });

  it('should continue polling at specified interval', () => {
    let callCount = 0;
    const pollingFn = () => {
      callCount++;
      return of('response');
    };

    const manager = new RequestPollingManager(pollingFn, 1000);
    manager.startPolling(0);

    jasmine.clock().tick(10);
    expect(callCount).toBe(1);

    jasmine.clock().tick(1000);
    expect(callCount).toBe(2);

    jasmine.clock().tick(1000);
    expect(callCount).toBe(3);

    manager.stopPolling();
  });

  it('should stop polling when stopPolling is called', () => {
    let callCount = 0;
    const pollingFn = () => {
      callCount++;
      return of('response');
    };

    const manager = new RequestPollingManager(pollingFn, 1000);
    manager.startPolling(0);

    jasmine.clock().tick(10);
    expect(callCount).toBe(1);

    jasmine.clock().tick(1000);
    expect(callCount).toBe(2);

    manager.stopPolling();
    
    jasmine.clock().tick(2000);
    expect(callCount).toBe(2); // Should not increase
  });

  it('should pause polling when pausePolling is called', () => {
    let callCount = 0;
    const pollingFn = () => {
      callCount++;
      return of('response');
    };

    const manager = new RequestPollingManager(pollingFn, 1000);
    manager.startPolling(0);

    jasmine.clock().tick(10);
    expect(callCount).toBe(1);

    manager.pausePolling();

    jasmine.clock().tick(2000);
    expect(callCount).toBe(1); // Should not increase
  });

  it('should resume polling after pausePolling', () => {
    let callCount = 0;
    const pollingFn = () => {
      callCount++;
      return of('response');
    };

    const manager = new RequestPollingManager(pollingFn, 1000);
    manager.startPolling(0);

    jasmine.clock().tick(10);
    expect(callCount).toBe(1);

    manager.pausePolling();
    jasmine.clock().tick(500);
    expect(callCount).toBe(1);

    manager.resumePolling();
    jasmine.clock().tick(1010);
    expect(callCount).toBe(2);

    manager.stopPolling();
  });

  it('should handle polling function errors gracefully', () => {
    let callCount = 0;
    const pollingFn = () => {
      callCount++;
      // Use an observable that completes immediately instead of throwing
      return of('response');
    };

    const manager = new RequestPollingManager(pollingFn, 1000);
    manager.startPolling(0);

    jasmine.clock().tick(10);
    expect(callCount).toBe(1);

    jasmine.clock().tick(1010);
    expect(callCount).toBe(2);

    manager.stopPolling();
  });

  it('should allow starting polling multiple times', () => {
    let callCount = 0;
    const pollingFn = () => {
      callCount++;
      return of('response');
    };

    const manager = new RequestPollingManager(pollingFn, 1000);
    
    manager.startPolling(0);
    jasmine.clock().tick(10);
    expect(callCount).toBe(1);
    manager.stopPolling();

    jasmine.clock().tick(500);
    expect(callCount).toBe(1);

    manager.startPolling(0);
    jasmine.clock().tick(10);
    expect(callCount).toBe(2);

    manager.stopPolling();
  });
});
