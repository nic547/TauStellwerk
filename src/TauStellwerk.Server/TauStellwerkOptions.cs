// <copyright file="TauStellwerkOptions.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace TauStellwerk;

public class TauStellwerkOptions
{
    public bool ResetEnginesWithoutState { get; init; } = true;

    public bool StopOnLastUserDisconnect { get; init; } = true;
}