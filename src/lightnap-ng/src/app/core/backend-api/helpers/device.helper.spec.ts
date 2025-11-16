import { DeviceHelper } from './device.helper';
import { DeviceDto } from '../dtos';

describe('DeviceHelper', () => {
  describe('rehydrate', () => {
    it('should convert lastSeen string to Date when present', () => {
      const device: DeviceDto = {
        id: '1',
        ipAddress: '192.168.1.1',
        details: 'Chrome',
        lastSeen: '2023-01-01T00:00:00Z' as any
      };

      DeviceHelper.rehydrate(device);

      expect(device.lastSeen).toBeInstanceOf(Date);
      expect((device.lastSeen as Date).toISOString()).toBe('2023-01-01T00:00:00.000Z');
    });

    it('should not modify lastSeen when null', () => {
      const device: DeviceDto = {
        id: '1',
        ipAddress: '192.168.1.1',
        details: 'Chrome',
        lastSeen: null as any
      };

      DeviceHelper.rehydrate(device);

      expect(device.lastSeen).toBeNull();
    });

    it('should not modify lastSeen when undefined', () => {
      const device: Partial<DeviceDto> = {
        id: '1',
        ipAddress: '192.168.1.1',
        details: 'Chrome'
      };

      DeviceHelper.rehydrate(device as DeviceDto);

      expect(device.lastSeen).toBeUndefined();
    });

    it('should handle null device', () => {
      expect(() => DeviceHelper.rehydrate(null as any)).not.toThrow();
    });
  });
});
