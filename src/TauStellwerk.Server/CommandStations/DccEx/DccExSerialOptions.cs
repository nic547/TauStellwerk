// <copyright file="DccExSerialOptions.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace TauStellwerk.Server.CommandStations;

public class DccExSerialOptions
{
    public string? SerialPort { get; set; }

    public int BaudRate { get; set; } = 115200;

    public bool UseJoinMode { get; set; }
}
