// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using Splat;
using TauStellwerk.Desktop.Services.WindowSettingService;

namespace TauStellwerk.Desktop.Controls;

/// <summary>
/// A <see cref="Window"/> that disposed it's DataContext (if it's IDisposable) when closing.
/// </summary>
public class BaseWindow : Window
{
    protected virtual Size DefaultSize => new(800, 600);

    protected override void OnClosed(EventArgs e)
    {
        if (DataContext is IDisposable disposable)
        {
            disposable.Dispose();
        }

        base.OnClosed(e);
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        var windowSettingService = Locator.Current.GetRequiredService<IWindowSettingService>();
        ClientSize = windowSettingService.LoadSize(GetType().ToString()) ?? DefaultSize;

    }

    protected override void OnResized(WindowResizedEventArgs e)
    {
        base.OnResized(e);
        var windowSettingService = Locator.Current.GetRequiredService<IWindowSettingService>();
        windowSettingService.SaveSize(GetType().ToString(), e.ClientSize);
    }
}
