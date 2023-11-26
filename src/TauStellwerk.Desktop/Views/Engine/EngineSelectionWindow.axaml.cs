// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using JetBrains.Annotations;
using TauStellwerk.Desktop.Controls;
using TauStellwerk.Desktop.ViewModels;

namespace TauStellwerk.Desktop.Views;

public partial class EngineSelectionWindow : BaseWindow
{
    public static readonly StyledProperty<int> ColumnsProperty = AvaloniaProperty.Register<EngineSelectionWindow, int>(nameof(Columns));

    public EngineSelectionWindow(EngineSelectionViewModel vm)
    {
        DataContext = vm;
        vm.ClosingRequested += Close;
        InitializeComponent();

        var engineList = this.FindControl<ItemsControl>("EngineList") ?? throw new Exception("Failed to locate EngineList ListBox");

        engineList.LayoutUpdated += (_, _) =>
        {
            Columns = Width switch
            {
                < 800 => 1,
                < 1300 => 2,
                < 1800 => 4,
                _ => 5,
            };
        };

        var scrollViewer = this.FindControl<ScrollViewer>("ScrollViewer") ?? throw new Exception("Unable to locate ScrollViewer");
        vm.ResetScroll += (_, _) => { scrollViewer.Offset = Vector.Zero; };

#if DEBUG
        this.AttachDevTools();
#endif
    }

    [UsedImplicitly]
    [Obsolete("Use constructor with ViewModel parameter", true)]
    public EngineSelectionWindow()
    {
        // https://github.com/AvaloniaUI/Avalonia/issues/2593
    }

    public int Columns
    {
        get => GetValue(ColumnsProperty);
        set => SetValue(ColumnsProperty, value);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
