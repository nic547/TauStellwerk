// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

namespace TauStellwerk.CommandStations.ECoS;

public class ECoSOptions
{
    public string? IPAddress { get; }

    public int Port { get; } = 5432;
}
