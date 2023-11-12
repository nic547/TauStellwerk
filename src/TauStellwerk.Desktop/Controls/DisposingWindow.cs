// <copyright file="DisposingWindow.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using Avalonia.Controls;

namespace TauStellwerk.Desktop.Controls;

/// <summary>
/// A <see cref="Window"/> that disposed it's DataContext (if it's IDisposable) when closing.
/// </summary>
public class DisposingWindow : Window
{
    protected override void OnClosed(EventArgs e)
    {
        if (DataContext is IDisposable disposable)
        {
            disposable.Dispose();
        }

        base.OnClosed(e);
    }
}