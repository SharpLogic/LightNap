import { UserSettingHelper } from './user-setting.helper';
import { UserSettingDto } from '../dtos';

describe('UserSettingHelper', () => {
  describe('rehydrate', () => {
    it('should convert createdDate and lastModifiedDate strings to Date when present', () => {
      const userSetting: UserSettingDto = {
        key: 'BrowserSettings',
        value: 'value',
        createdDate: '2023-01-01T00:00:00Z' as any,
        lastModifiedDate: '2023-01-02T00:00:00Z' as any
      };

      UserSettingHelper.rehydrate(userSetting);

      expect(userSetting.createdDate).toBeInstanceOf(Date);
      expect((userSetting.createdDate as Date).toISOString()).toBe('2023-01-01T00:00:00.000Z');
      expect(userSetting.lastModifiedDate).toBeInstanceOf(Date);
      expect((userSetting.lastModifiedDate as Date).toISOString()).toBe('2023-01-02T00:00:00.000Z');
    });

    it('should convert only createdDate when lastModifiedDate is null', () => {
      const userSetting: UserSettingDto = {
        key: 'BrowserSettings',
        value: 'value',
        createdDate: '2023-01-01T00:00:00Z' as any,
        lastModifiedDate: null as any
      };

      UserSettingHelper.rehydrate(userSetting);

      expect(userSetting.createdDate).toBeInstanceOf(Date);
      expect(userSetting.lastModifiedDate).toBeNull();
    });

    it('should convert only lastModifiedDate when createdDate is null', () => {
      const userSetting: UserSettingDto = {
        key: 'BrowserSettings',
        value: 'value',
        createdDate: null as any,
        lastModifiedDate: '2023-01-02T00:00:00Z' as any
      };

      UserSettingHelper.rehydrate(userSetting);

      expect(userSetting.createdDate).toBeNull();
      expect(userSetting.lastModifiedDate).toBeInstanceOf(Date);
    });

    it('should not modify dates when both are null', () => {
      const userSetting: UserSettingDto = {
        key: 'BrowserSettings',
        value: 'value',
        createdDate: null as any,
        lastModifiedDate: null as any
      };

      UserSettingHelper.rehydrate(userSetting);

      expect(userSetting.createdDate).toBeNull();
      expect(userSetting.lastModifiedDate).toBeNull();
    });

    it('should not modify dates when undefined', () => {
      const userSetting: UserSettingDto = {
        key: 'BrowserSettings',
        value: 'value'
      };

      UserSettingHelper.rehydrate(userSetting);

      expect(userSetting.createdDate).toBeUndefined();
      expect(userSetting.lastModifiedDate).toBeUndefined();
    });

    it('should handle null userSetting', () => {
      expect(() => UserSettingHelper.rehydrate(null as any)).not.toThrow();
    });
  });
});