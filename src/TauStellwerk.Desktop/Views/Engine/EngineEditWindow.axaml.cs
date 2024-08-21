// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
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
    }

    [UsedImplicitly]
    [Obsolete("Use constructor with ViewModel parameter", true)]
    public EngineEditWindow()
    {
        _vm = null!;
        // https://github.com/AvaloniaUI/Avalonia/issues/2593
    }

    private void TagInput_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            _vm.AddTagCommand.Execute(null);
        }
    }

    private void Functions_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            _vm.AddFunctionCommand.Execute(null);
            FunctionItemsControl.LayoutUpdated += SetFocusToLastFunctionField;
        }
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        FunctionItemsControl.LayoutUpdated += SetFocusToLastFunctionField;
    }

    private void SetFocusToLastFunctionField(object? obj, EventArgs? args)
    {
        // hideous thing - we need to subscribe to the ContentPresenter to know when it acutally loaded the UI elements and isn't empty
        // We can only do that after the ContentPresenter actually exists, and we need to clean up the subscriptions so the focus is only set once.
        var lastContentPresenter = FunctionItemsControl.GetLogicalChildren().Last() as ContentPresenter ?? throw new NullReferenceException("Failed to get content presenter");
        lastContentPresenter.Loaded += ContentPresenterOnLoaded;
        FunctionItemsControl.LayoutUpdated -= SetFocusToLastFunctionField;
    }

    private void ContentPresenterOnLoaded(object? sender, RoutedEventArgs e)
    {
        var contentPresenter = (ContentPresenter)sender!;
        (contentPresenter.Child as StackPanel)?.Children.OfType<TextBox>().Single().Focus();
        contentPresenter.Loaded -= ContentPresenterOnLoaded;
    }


}
