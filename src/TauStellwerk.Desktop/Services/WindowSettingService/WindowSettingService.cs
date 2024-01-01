// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using System.Text.Json;
using Avalonia;

namespace TauStellwerk.Desktop.Services.WindowSettingService;

public class WindowSettingService : IWindowSettingService
{
    private const string Filename = "./windowSettings.json";
    private readonly Dictionary<string, WindowSetting> _windowSettings;

    public event EventHandler<(string WindowType, bool UseLargeButton)>? UseLargeButtonChanged;

    public WindowSettingService()
    {
        if (File.Exists(Filename))
        {
            var json = File.ReadAllText(Filename);
            var windowSizes = JsonSerializer.Deserialize<Dictionary<string, WindowSetting>>(json);
            _windowSettings = windowSizes ?? [];
            return;
        }
        _windowSettings = [];

        // TODO: Find a smarter way to set default values for the large/small button switch
        // default window sizes are just part of each window

        _windowSettings["TauStellwerk.Desktop.Views.EngineControlView"] = new WindowSetting(null, null, true);
    }

    public void SaveSize(string windowType, Size size)
    {
        var existingSettings = _windowSettings[windowType];

        if (existingSettings is not null)
        {
            _windowSettings[windowType] = new WindowSetting(size.Width, size.Height, existingSettings.UseLargeButton);
            WriteDictionaryToFile();
            return;
        }
        _windowSettings[windowType] = new WindowSetting(size.Width, size.Height, null);
        WriteDictionaryToFile();
    }

    public Size? LoadSize(string windowType)
    {
        _windowSettings.TryGetValue(windowType, out var settings);
        if (settings is null || settings.Width is null || settings.Height is null)
        {
            return null;
        }
        return new Size((double)settings.Width, (double)settings.Height);
    }

    public void SaveUseLargeButton(string windowType, bool useLargeButton)
    {
        UseLargeButtonChanged?.Invoke(this, (windowType, useLargeButton));

        var existingSettings = _windowSettings[windowType];

        if (existingSettings is not null)
        {
            _windowSettings[windowType] = new WindowSetting(existingSettings.Width, existingSettings.Height, useLargeButton);
            WriteDictionaryToFile();
            return;
        }
        _windowSettings[windowType] = new WindowSetting(null, null, useLargeButton);
        WriteDictionaryToFile();
    }

    public bool? LoadUseLargeButton(string windowType)
    {
        _windowSettings.TryGetValue(windowType, out var settings);
        if (settings is null || settings.UseLargeButton is null)
        {
            return null;
        }
        return settings.UseLargeButton;
    }

    private void WriteDictionaryToFile()
    {
        var json = JsonSerializer.Serialize(_windowSettings);
        File.WriteAllText(Filename, json);
    }

    public record WindowSetting(double? Width, double? Height, bool? UseLargeButton);
}
