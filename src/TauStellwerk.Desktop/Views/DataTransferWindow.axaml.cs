// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using Avalonia.Controls;
using TauStellwerk.Desktop.ViewModels;

namespace TauStellwerk.Desktop.Views;

public partial class DataTransferWindow : Window
{
    public DataTransferWindow()
    {
        DataContext = new DataTransferViewModel();
        InitializeComponent();
    }
}

