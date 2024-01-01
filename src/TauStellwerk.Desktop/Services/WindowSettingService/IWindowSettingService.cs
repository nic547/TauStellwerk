// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using Avalonia;

namespace TauStellwerk.Desktop.Services.WindowSettingService;

public interface IWindowSettingService
{
    event EventHandler<(string WindowType, bool UseLargeButton)>? UseLargeButtonChanged;

    public void SaveSize(string windowType, Size size);

    public Size? LoadSize(string windowType);
    bool? LoadUseLargeButton(string windowType);
    void SaveUseLargeButton(string windowType, bool useLargeButton);
}
