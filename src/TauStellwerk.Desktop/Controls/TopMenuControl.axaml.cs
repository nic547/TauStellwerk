// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using Avalonia;
using Avalonia.Controls;
using TauStellwerk.Desktop.ViewModels;

namespace TauStellwerk.Desktop.Controls;

public partial class TopMenuControl : UserControl
{

    private readonly TopMenuViewModel _vm;

    public TopMenuControl()
    {
        InitializeComponent();
        _vm = new TopMenuViewModel();
        DataContext = _vm;
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _vm.UpdateWindowType(TryGetContainingWindowType());
        base.OnAttachedToVisualTree(e);
    }

    private string TryGetContainingWindowType()
    {
        return VisualRoot?.GetType().ToString() ?? string.Empty;
    }
}
