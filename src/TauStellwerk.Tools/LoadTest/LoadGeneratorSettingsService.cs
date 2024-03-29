﻿// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using TauStellwerk.Client.Model.Settings;
using TauStellwerk.Client.Services;

namespace TauStellwerk.Tools.LoadTest;

public class LoadGeneratorSettingsService : ISettingsService
{
    private readonly ImmutableSettings _settings;

    public LoadGeneratorSettingsService(LoadTestOptions options, Random random)
    {
        _settings = new ImmutableSettings(
            $"Random User {random.Next(999_999)}",
            options.Uri,
            "ThisIsNotATheme",
            false,
            "en");
    }

    public event ISettingsService.SettingsChange SettingsChanged
    {
        add { }
        remove { }
    }

    public Task<ImmutableSettings> GetSettings()
    {
        return Task.FromResult(_settings);
    }

    public Task<MutableSettings> GetMutableSettingsCopy()
    {
        throw new InvalidOperationException("Cannot edit settings for the load generator");
    }

    public Task SetSettings(MutableSettings mutableSettings)
    {
        throw new InvalidOperationException("Cannot edit settings for the load generator");
    }
}
