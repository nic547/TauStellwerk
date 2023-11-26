// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

namespace TauStellwerk.Base.Dto;

public class EngineOverviewDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public List<string> Tags { get; set; } = [];

    public List<ImageDto> Images { get; set; } = [];

    // Intentionally not a long, wanna keep the number short, collisions should be unlikely anyway
    public int ImageTimestamp { get; set; }

    public DateTime LastUsed { get; set; }

    public DateTime Created { get; set; }

    public bool IsHidden { get; set; }
}
