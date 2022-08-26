// <copyright file="TurnoutStateChangedEventArgs.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using TauStellwerk.Base;

namespace TauStellwerk.Client.Services;

public record TurnoutStateChangedEventArgs(int Address, State State);