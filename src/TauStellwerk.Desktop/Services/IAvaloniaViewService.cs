// <copyright file="IAvaloniaViewService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using Avalonia.Platform.Storage;
using TauStellwerk.Client.Model;
using TauStellwerk.Client.Services;

namespace TauStellwerk.Desktop.Services;

public interface IAvaloniaViewService : IViewService
{
    void ShowTurnoutsWindow(object? source = null);

    void ShowTurnoutEditWindow(Turnout turnout, object? source = null);

    Task<IStorageFile?> ShowFilePicker(object source, FilePickerOpenOptions? filePickerOpenOptions = null);
}