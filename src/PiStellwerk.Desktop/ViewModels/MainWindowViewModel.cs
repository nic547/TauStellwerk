// <copyright file="MainWindowViewModel.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using JetBrains.Annotations;
using PiStellwerk.Desktop.Views;

namespace PiStellwerk.Desktop.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public static string Greeting => "Welcome to Avalonia!";

        // ReSharper disable once UnusedMember.Local
        private static void OpenEngineList()
        {
            var engineWindow = new EngineWindow();
            engineWindow.Show();
        }

        [UsedImplicitly]
        private static void OpenSettings()
        {
            new SettingsWindow().Show();
        }
    }
}