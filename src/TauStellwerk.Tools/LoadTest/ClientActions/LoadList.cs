// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using JetBrains.Annotations;
using TauStellwerk.Base.Model;
using TauStellwerk.Client.Services;

namespace TauStellwerk.Tools.LoadTest.ClientActions;

[UsedImplicitly]
public class LoadList : ClientActionBase
{
    /// <inheritdoc/>
    public override int Interval => Random.Next(450, 550);

    /// <inheritdoc/>
    public override async Task Initialize(EngineService engineService, LoadTestOptions options, int id, Random random)
    {
        await base.Initialize(engineService, options, id, random);
    }

    /// <inheritdoc/>
    public override async Task<int> PerformRequest()
    {
        var startTime = DateTime.UtcNow;

        await EngineService.GetEngines(string.Empty, 0, SortEnginesBy.Name, true, true);
        return (int)Math.Round((DateTime.UtcNow - startTime).TotalMilliseconds);
    }
}
