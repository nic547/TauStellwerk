// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using Avalonia;
using JetBrains.Annotations;
using TauStellwerk.Desktop.Controls;
using TauStellwerk.Desktop.ViewModels;

namespace TauStellwerk.Desktop.Views;

public partial class DecoderProgrammingWindow : BaseWindow
{
    protected override Size DefaultSize => new(400, 400);

    [UsedImplicitly]
    [Obsolete("This constructor is for the designer only and should not be used in code.")]
    public DecoderProgrammingWindow()
    {
        InitializeComponent();
    }
    public DecoderProgrammingWindow(DecoderProgrammingViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}
