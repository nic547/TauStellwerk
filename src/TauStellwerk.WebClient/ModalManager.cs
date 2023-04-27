// <copyright file="ModalManager.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

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