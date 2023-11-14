// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using Avalonia.Controls;

namespace TauStellwerk.Desktop.Controls;

/// <summary>
/// A <see cref="Window"/> that disposed it's DataContext (if it's IDisposable) when closing.
/// </summary>
public class DisposingWindow : Window
{
    protected override void OnClosed(EventArgs e)
    {
        if (DataContext is IDisposable disposable)
        {
            disposable.Dispose();
        }

        base.OnClosed(e);
    }
}
