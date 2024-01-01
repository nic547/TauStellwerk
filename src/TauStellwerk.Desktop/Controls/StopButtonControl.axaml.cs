// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using Avalonia.Controls;
using Splat;
using TauStellwerk.Desktop.ViewModels;

namespace TauStellwerk.Desktop.Controls;

public partial class StopButtonControl : UserControl
{
    public StopButtonControl()
    {
        InitializeComponent();

        DataContext = Locator.Current.GetRequiredService<StopButtonControlViewModel>();
    }
}
