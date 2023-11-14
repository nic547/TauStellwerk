// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using CommunityToolkit.Mvvm.ComponentModel;

namespace TauStellwerk.WebClient;

/// <summary>
/// Small helper class for hiding/unhiding modals.
/// There might be a cleaner solution, the main problem is that the "activators" are in different components.
/// </summary>
public partial class ModalManager : ObservableObject
{
    [ObservableProperty]
    private bool _isEngineSelectionVisible;

    [ObservableProperty]
    private bool _isSettingsModalVisible;

    [ObservableProperty]
    private bool _isMessageBoxVisible;

    [ObservableProperty]
    private bool _isEngineEditModalVisible;
}
