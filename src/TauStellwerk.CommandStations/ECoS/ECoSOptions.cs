// <copyright file="ECoSOptions.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace TauStellwerk.CommandStations.ECoS;

public class ECoSOptions
{
    public string? IPAddress { get; }

    public int Port { get; } = 5432;
}
