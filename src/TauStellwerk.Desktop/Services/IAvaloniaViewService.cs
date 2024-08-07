﻿// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using Avalonia.Platform.Storage;
using TauStellwerk.Client.Model;
using TauStellwerk.Client.Services;

namespace TauStellwerk.Desktop.Services;

public interface IAvaloniaViewService : IViewService
{
    void ShowProgrammingWindow();

    void ShowTurnoutsWindow(object? source = null);

    void ShowTurnoutEditWindow(Turnout turnout, object? source = null);

    void ShowDataTransferWindow();

    Task<IStorageFile?> ShowFilePicker(object source, FilePickerOpenOptions? filePickerOpenOptions = null);

    Task<IStorageFile?> ShowSaveFilePicker(object source, FilePickerSaveOptions? filePickerSaveOptions = null);
}
