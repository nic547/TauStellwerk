// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using FluentAssertions;
using NUnit.Framework;
using TauStellwerk.Base.Model;
using TauStellwerk.Client.Model;
using TauStellwerk.Client.Services;

namespace TauStellwerk.Server.IntegrationTests;

public class EngineIntegrationTests : IntegrationTestBase
{
    [Test]
    [Category("long-running")]
    public async Task CanCreateAndLoadEngines()
    {
        var engineService = new EngineService(CreateConnectionService());

        var enginesToInsert = Tools.CreateTestDb.EngineDtoGenerator.GetEngineFullDtos(100);
        foreach (var engine in enginesToInsert)
        {
            await engineService.AddOrUpdateEngine(new EngineFull(engine));
        }

        List<EngineOverview> engines = [];

        foreach (var i in Enumerable.Range(0, 6))
        {
            engines.AddRange(await engineService.GetEngines(string.Empty, i, SortEnginesBy.Name));
        }

        engines.Should().HaveCount(100);
    }

    [Test]
    [Category("long-running")]
    public async Task CanCreateAndControlEngine()
    {
        var engineService = new EngineService(CreateConnectionService());

        var engineToInsert = Tools.CreateTestDb.EngineDtoGenerator.GetEngineDto();
        var engine = await engineService.AddOrUpdateEngine(new EngineFull(engineToInsert));

        await engineService.AcquireEngine(engine.Id);
        await engineService.SetSpeed(engine.Id, 20, Direction.Forwards);
        await engineService.SetEStop(engine.Id);
        await engineService.SetFunction(engine.Id, 1, State.On);
        await engineService.ReleaseEngine(engine.Id);

        Assert.Pass();
    }
}
