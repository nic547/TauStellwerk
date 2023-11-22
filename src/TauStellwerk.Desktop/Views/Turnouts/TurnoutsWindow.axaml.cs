// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Markup.Xaml;
using JetBrains.Annotations;
using TauStellwerk.Desktop.Controls;
using TauStellwerk.Desktop.ViewModels;

namespace TauStellwerk.Desktop.Views;

public partial class TurnoutsWindow : BaseWindow
{
    public TurnoutsWindow(TurnoutsViewModel vm)
    {
        DataContext = vm;
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    [UsedImplicitly]
    [Obsolete("Use constructor with ViewModel parameter", true)]
    public TurnoutsWindow()
    {
        // https://github.com/AvaloniaUI/Avalonia/issues/2593
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
