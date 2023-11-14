// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using TauStellwerk.Client.Model;

namespace TauStellwerk.Client.Services;

public interface IViewService
{
    public void ShowEngineSelectionView(object? source = null);

    public void ShowEngineEditView(EngineFull engine, object? source = null);

    public void ShowEngineControlView(EngineFull engine, object? source = null);

    public void ShowSettingsView(object? source);

    public void ShowMessageBox(string title, string message, object source);
}
