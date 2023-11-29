// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using System.Text.Json;
using Avalonia;

namespace TauStellwerk.Desktop.Services.WindowSettingService;

public class WindowSettingService : IWindowSettingService
{
    private const string Filename = "./windowSettings.json";
    private readonly Dictionary<string, WindowSetting> _windowSettings;

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
    }

    public void SaveSize(string windowType, Size size)
    {
        _windowSettings[windowType] = new WindowSetting(size.Width, size.Height);
        WriteDictionaryToFile();
    }

    public Size? LoadSize(string windowType)
    {
        _windowSettings.TryGetValue(windowType, out var settings);
        if (settings is null)
        {
            return null;
        }
        return new Size(settings.Width, settings.Height);
    }

    private void WriteDictionaryToFile()
    {
        var json = JsonSerializer.Serialize(_windowSettings);
        File.WriteAllText(Filename, json);
    }

    public record WindowSetting(double Width, double Height);
}
