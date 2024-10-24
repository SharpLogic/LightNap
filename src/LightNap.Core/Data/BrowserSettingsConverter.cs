﻿using LightNap.Core.Profile;
using LightNap.Core.Profile.Dto.Response;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace LightNap.Core.Data
{
    internal class BrowserSettingsConverter : ValueConverter<BrowserSettingsDto, string>
    {
        public BrowserSettingsConverter() : base(
            settings => JsonSerializer.Serialize(settings, null as JsonSerializerOptions),
            json => BrowserSettingsMigration.Migrate(JsonSerializer.Deserialize<BrowserSettingsDto>(json, null as JsonSerializerOptions) ?? new BrowserSettingsDto()))
        {
        }
    }

}
