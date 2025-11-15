import { NotificationHelper } from './notification.helper';
import { NotificationDto } from '../dtos';

describe('NotificationHelper', () => {
  describe('rehydrate', () => {
    it('should convert timestamp string to Date when present', () => {
      const notification: NotificationDto = {
        id: 1,
        type: 'AdministratorNewUserRegistration',
        status: 'Unread',
        data: {},
        timestamp: '2023-01-01T00:00:00Z' as any
      };

      NotificationHelper.rehydrate(notification);

      expect(notification.timestamp).toBeInstanceOf(Date);
      expect((notification.timestamp as Date).toISOString()).toBe('2023-01-01T00:00:00.000Z');
    });

    it('should not modify timestamp when null', () => {
      const notification: NotificationDto = {
        id: 1,
        type: 'AdministratorNewUserRegistration',
        status: 'Unread',
        data: {},
        timestamp: null as any
      };

      NotificationHelper.rehydrate(notification);

      expect(notification.timestamp).toBeNull();
    });

    it('should not modify timestamp when undefined', () => {
      const notification: Partial<NotificationDto> = {
        id: 1,
        type: 'AdministratorNewUserRegistration',
        status: 'Unread',
        data: {}
      };

      NotificationHelper.rehydrate(notification as NotificationDto);

      expect(notification.timestamp).toBeUndefined();
    });

    it('should handle null notification', () => {
      expect(() => NotificationHelper.rehydrate(null as any)).not.toThrow();
    });
  });
});