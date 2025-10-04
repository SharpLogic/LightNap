namespace LightNap.Core.UserSettings.Dto.Request
{
    public class SetUserSettingRequestDto
    {
        public required string Key { get; set; }
        public required string Value { get; set; }
    }
}