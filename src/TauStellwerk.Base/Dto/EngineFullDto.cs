// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using TauStellwerk.Base.Model;

namespace TauStellwerk.Base.Dto;

public class EngineFullDto : EngineOverviewDto
{
    public List<FunctionDto> Functions { get; set; } = new();

    public ushort Address { get; set; }

    public int TopSpeed { get; set; }

    public int Throttle { get; set; }

    public Direction Direction { get; set; }
}
