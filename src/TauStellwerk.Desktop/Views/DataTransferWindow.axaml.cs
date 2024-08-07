// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using Avalonia.Controls;
using Avalonia.Input;
using TauStellwerk.Desktop.Controls;
using TauStellwerk.Desktop.ViewModels;

namespace TauStellwerk.Desktop.Views;

public partial class DataTransferWindow : BaseWindow
{
    private readonly DataTransferViewModel _vm = new();
    public DataTransferWindow()
    {
        DataContext = _vm;
        InitializeComponent();
    }

    private void FileLabel_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var label = sender as TextBlock;
        var fileName = label?.Text;
        _vm.DownloadBackupCommand.Execute(fileName);
    }
}

