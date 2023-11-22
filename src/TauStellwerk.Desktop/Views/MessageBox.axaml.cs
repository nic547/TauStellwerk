// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TauStellwerk.Desktop.Views;

// Intentionally not inheriting from BaseWindow, as this dialog isn't really a window like others
public partial class MessageBox : Window
{
    public MessageBox()
    {
        AvaloniaXamlLoader.Load(this);

#if DEBUG
        this.AttachDevTools();
#endif
    }
}
