// <copyright file="SettingsService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Text.Json;
using TauStellwerk.Client.Model;

namespace TauStellwerk.Client.Services;

public class SettingsService : ISettingsService
{
    private const string FileName = "settings.json";

    private bool _hasLoadBeenAttempted;

    private MutableSettings _settings;
    private ImmutableSettings _immutableSettings;

    public SettingsService()
    {
        _settings = new MutableSettings();
        _immutableSettings = _settings.GetImmutableCopy();
    }

    public event ISettingsService.SettingsChange? SettingsChanged;

    public async Task<ImmutableSettings> GetSettings()
    {
        await EnsureSettingsWereLoaded();
        return _immutableSettings;
    }

    public async Task<MutableSettings> GetMutableSettingsCopy()
    {
        await EnsureSettingsWereLoaded();
        return _settings.GetMutableCopy();
    }

    public async Task SetSettings(MutableSettings mutableSettings)
    {
        _settings = mutableSettings;
        _immutableSettings = _settings.GetImmutableCopy();

        await using var stream = File.Open(FileName, FileMode.Create);
        await JsonSerializer.SerializeAsync(stream, mutableSettings, SettingsJsonContext.Default.MutableSettings);

        SettingsChanged?.Invoke(_immutableSettings);
    }

    /// <summary>
    /// Ensures that there was an attempt at loading the settings file.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private async Task EnsureSettingsWereLoaded()
    {
        if (_hasLoadBeenAttempted)
        {
            return;
        }

        try
        {
            await using var stream = File.OpenRead(FileName);
            var potentialSettings = await JsonSerializer.DeserializeAsync(stream, SettingsJsonContext.Default.MutableSettings);
            if (potentialSettings != null)
            {
                _settings = potentialSettings;
                _immutableSettings = _settings.GetImmutableCopy();
                SettingsChanged?.Invoke(_immutableSettings);
            }
        }
        catch (Exception)
        {
            // ignore exception, file probably just doesn't exists yet.
        }

        _hasLoadBeenAttempted = true;
    }
}