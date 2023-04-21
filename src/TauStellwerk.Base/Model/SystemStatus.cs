// <copyright file="SystemStatus.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace TauStellwerk.Base;

/// <summary>
/// SystemStatus of the dcc output.
/// </summary>
public class SystemStatus
{
    /// <summary>
    /// Gets or sets a value indicating whether the dcc output hardware is running.
    /// </summary>
    public State State { get; set; }

    /// <summary>
    /// Gets or sets the name of the Person who is responsible for the last status change.
    /// SYSTEM is the software itself, for example because of the over current protection kicking in.
    /// </summary>
    public string LastActionUsername { get; set; } = string.Empty;
}