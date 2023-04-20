// <copyright file="BlazorSettingsService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Text.Json;
using Microsoft.JSInterop;
using TauStellwerk.Client.Model;
using TauStellwerk.Client.Services;

namespace TauStellwerk.WebClient;

public class BlazorSettingsService : ISettingsService
{
    private const string SettingsKey = "TauStellwerk_Settings";

    private readonly string _baseAddress;
    private readonly IJSRuntime _runtime;

    private bool _hasLoadBeenAttempted;

    private MutableSettings _settings;
    private ImmutableSettings _immutableSettings;

    public BlazorSettingsService(string baseAddress, IJSRuntime runtime)
    {
        _baseAddress = baseAddress;
        _runtime = runtime;

        _settings = new MutableSettings
        {
            ServerAddress = baseAddress,
        };
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
        mutableSettings.ServerAddress = _baseAddress;
        _settings = mutableSettings;

        _immutableSettings = _settings.GetImmutableCopy();

        await SaveSettings(mutableSettings);

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
            var potentialSettings = await TryLoadSettings();
            if (potentialSettings != null)
            {
                potentialSettings.ServerAddress = _baseAddress;
                _settings = potentialSettings;
                _immutableSettings = _settings.GetImmutableCopy();
                SettingsChanged?.Invoke(_immutableSettings);
            }
            else
            {
                var prefersDarkMode = await _runtime.InvokeAsync<bool>("isDarkModePreferred");
                _settings.Theme = prefersDarkMode ? "dark" : "light";
                _immutableSettings = _settings.GetImmutableCopy();
                SettingsChanged?.Invoke(_immutableSettings);
            }
        }
        catch (Exception)
        {
            // ignore exception, key-value pair probably just doesn't exists yet.
        }

        _hasLoadBeenAttempted = true;
    }

    private async Task<MutableSettings?> TryLoadSettings()
    {
        var json = await _runtime.InvokeAsync<string?>("getItem", SettingsKey);
        if (json is null)
        {
            return null;
        }

        return JsonSerializer.Deserialize(json, Client.SettingsJsonContext.Default.MutableSettings);
    }

    private async Task SaveSettings(MutableSettings settings)
    {
        var json = JsonSerializer.Serialize(settings, Client.SettingsJsonContext.Default.MutableSettings);
        await _runtime.InvokeVoidAsync("setItem", SettingsKey, json);
    }
}