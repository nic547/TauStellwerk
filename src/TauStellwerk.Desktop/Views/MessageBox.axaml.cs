// <copyright file="MessageBox.axaml.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace TauStellwerk.Desktop.Views;

public partial class MessageBox : Window
{
    public MessageBox()
    {
        AvaloniaXamlLoader.Load(this);

#if DEBUG
        this.AttachDevTools();
#endif
    }
}