// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

namespace TauStellwerk.CommandStations.DccEx;

public class DccExSerialOptions
{
    public string? SerialPort { get; set; }

    public int BaudRate { get; set; } = 115200;

    public bool UseJoinMode { get; set; }
}
