// <copyright file="AppState.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.ObjectModel;
using PiStellwerk.Data;

namespace PiStellwerk.WebClient
{
    public class AppState
    {
        public ObservableCollection<Engine> ActiveEngines { get; } = new();
    }
}
