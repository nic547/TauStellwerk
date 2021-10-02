// <copyright file="ModalManager.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.ComponentModel;

namespace TauStellwerk.WebClient
{
    /// <summary>
    /// Small helper class for hiding/unhiding modals.
    /// There might be a cleaner solution, the main Problem is that the "activators" are in different components.
    /// </summary>
    public class ModalManager : INotifyPropertyChanged
    {
        private bool _isEngineSelectionVisible;
        private bool _isSettingsModalVisible;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string EngineSelectionClass => _isEngineSelectionVisible ? "active-modal" : "inactive-modal";

        public bool IsEngineSelectionVisible
        {
            get => _isEngineSelectionVisible;
            set
            {
                _isEngineSelectionVisible = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EngineSelectionClass)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEngineSelectionVisible)));
            }
        }

        public bool IsSettingsModalVisible
        {
            get => _isSettingsModalVisible;
            set
            {
                _isSettingsModalVisible = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSettingsModalVisible)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SettingsModalClass)));
            }
        }

        public string SettingsModalClass => _isSettingsModalVisible ? "active-modal" : "inactive-modal";
    }
}
