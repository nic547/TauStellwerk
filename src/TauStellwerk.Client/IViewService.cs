// <copyright file="IViewService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using TauStellwerk.Client.Model;

namespace TauStellwerk.Client
{
    public interface IViewService
    {
        public void ShowEngineSelectionView(object? source = null);

        public void ShowEngineEditView(EngineFull engine, object? source = null);

        public void ShowEngineControlView(EngineFull engine, object? source = null);

        public void ShowSettingsView(object? source);

        public void ShowMessageBox(string title, string message, object? source = null);
    }
}
