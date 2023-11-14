// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using TauStellwerk.Client.Model;

namespace TauStellwerk.WebClient;

public partial class AppState : ObservableObject
{
    private readonly ModalManager _modalManager;

    [ObservableProperty]
    private string _messageBoxText = string.Empty;

    [ObservableProperty]
    private EngineFull _selectedEngine = new();

    public AppState(ModalManager modalManager)
    {
        _modalManager = modalManager;
    }

    public ObservableCollection<EngineFull> ActiveEngines { get; } = new();

    public void ShowMessageBox(string text)
    {
        MessageBoxText = text;
        _modalManager.IsMessageBoxVisible = true;
    }
}
