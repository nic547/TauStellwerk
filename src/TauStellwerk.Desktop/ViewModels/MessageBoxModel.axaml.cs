// <copyright file="MessageBoxModel.axaml.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using Avalonia.Controls;
using ReactiveUI;

namespace TauStellwerk.Desktop.ViewModels
{
    public class MessageBoxModel : ViewModelBase
    {
        private string _title = string.Empty;
        private string _message = string.Empty;

        public string Title
        {
            get => _title;
            set => this.RaiseAndSetIfChanged(ref _title, value);
        }

        public string Message
        {
            get => _message;
            set => this.RaiseAndSetIfChanged(ref _message, value);
        }

        public static void Close(Window window)
        {
            window.Close();
        }
    }
}
