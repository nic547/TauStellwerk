// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using JetBrains.Annotations;
using TauStellwerk.Desktop.Controls;
using TauStellwerk.Desktop.ViewModels;

namespace TauStellwerk.Desktop.Views;

public partial class EngineEditWindow : BaseWindow
{
    private readonly EngineEditViewModel _vm;

    public EngineEditWindow(EngineEditViewModel vm)
    {
        _vm = vm;
        DataContext = vm;
        Closing += vm.HandleWindowClosing;
        vm.ClosingRequested += Close;

        InitializeComponent();

#if DEBUG
        this.AttachDevTools();
#endif
    }

    [UsedImplicitly]
    [Obsolete("Use constructor with ViewModel parameter", true)]
    public EngineEditWindow()
    {
        _vm = null!;
        // https://github.com/AvaloniaUI/Avalonia/issues/2593
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void TagInput_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            _vm.AddTagCommand.Execute(null);
        }
    }
}
