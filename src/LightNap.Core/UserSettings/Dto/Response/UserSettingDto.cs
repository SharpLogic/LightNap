namespace LightNap.Core.UserSettings.Dto.Response
{
    public class UserSettingDto
    {
        public required string Key { get; set; }
        public required string Value { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}