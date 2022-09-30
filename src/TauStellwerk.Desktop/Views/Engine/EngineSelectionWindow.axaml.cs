// <copyright file="EngineSelectionWindow.axaml.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using JetBrains.Annotations;
using TauStellwerk.Desktop.Controls;
using TauStellwerk.Desktop.ViewModels;

namespace TauStellwerk.Desktop.Views;

public class EngineSelectionWindow : DisposingWindow
{
    public static readonly StyledProperty<int> ColumnsProperty = AvaloniaProperty.Register<EngineSelectionWindow, int>(nameof(Columns));

    public EngineSelectionWindow(EngineSelectionViewModel vm)
    {
        DataContext = vm;
        vm.ClosingRequested += Close;
        InitializeComponent();

        var engineList = this.FindControl<ListBox>("EngineList");

        engineList.LayoutUpdated += (_, _) =>
        {
            Columns = Width switch
            {
                < 768 => 1,
                < 992 => 2,
                < 1400 => 4,
                _ => 5,
            };
        };

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
        get { return GetValue(ColumnsProperty); }
        set { SetValue(ColumnsProperty, value); }
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}