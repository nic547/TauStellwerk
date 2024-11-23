// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using System.IO.Compression;
using FluentAssertions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using TauStellwerk.Data;
using TauStellwerk.Data.Tests;
using TauStellwerk.Server;
using TauStellwerk.Server.Hub;
using TauStellwerk.Server.Services.TransferService;
using TauStellwerk.Util.DateTimeProvider;

namespace TauStellwerk.Test;
public class TransferServiceTests : ContextTestBase
{
    private readonly TauStellwerkOptions _defaultOptions = new();

    private readonly IDateTimeProvider _dateTimeProvider = new FakeDateTimeProvider();

    [SetUp]
    public void TransferSetup()
    {
        Directory.CreateDirectory(_defaultOptions.OriginalImageDirectory);
        Directory.CreateDirectory("./transfer");
    }

    [TearDown]
    public void TransferTearDown()
    {
        if (Directory.Exists("./transfer"))
        {
            Directory.Delete("./transfer", true);
        }

        if (Directory.Exists(_defaultOptions.OriginalImageDirectory))
        {
            Directory.Delete(_defaultOptions.OriginalImageDirectory, true);
        }
    }

    [Test]
    public async Task ExportedEnginesNumberTest()
    {
        var engines = TestDataHelper.CreateTestEngineList();
        var context = GetContext();
        context.Engines.AddRange(engines);
        await context.SaveChangesAsync();

        var service = new TransferService(CreateFakeContextFactory(), CreateFakeHubContext(), Substitute.For<ILogger<TransferService>>(), _defaultOptions, _dateTimeProvider);
        await service.ExportEngines();

        var lines = await File.ReadAllLinesAsync("./transfer/temp/engines.ndjson");
        lines.Length.Should().Be(engines.Count);
    }

    [Test]
    public async Task RoundtripEnginesTest()
    {
        var engines = TestDataHelper.CreateTestEngineList();
        var context = GetContext();
        context.Engines.AddRange(engines);
        await context.SaveChangesAsync();

        var service = new TransferService(CreateFakeContextFactory(), CreateFakeHubContext(), Substitute.For<ILogger<TransferService>>(), _defaultOptions, _dateTimeProvider);
        await service.ExportEngines();

        var newContext = GetContext();
        var newService = new TransferService(CreateFakeContextFactory(), CreateFakeHubContext(), Substitute.For<ILogger<TransferService>>(), _defaultOptions, _dateTimeProvider);

        await newService.ImportEngines("./transfer/temp/");
        newContext.Engines.Count().Should().Be(engines.Count);
    }

    [Test]
    public async Task ExportCreatesZipFileTest()
    {
        var engines = TestDataHelper.CreateTestEngineList();
        var context = GetContext();
        context.Engines.AddRange(engines);
        await context.SaveChangesAsync();

        var service = new TransferService(CreateFakeContextFactory(), CreateFakeHubContext(), Substitute.For<ILogger<TransferService>>(), _defaultOptions, _dateTimeProvider);
        await service.ExportEverything();


        File.Exists($"./transfer/TauStellwerk-Backup-{_dateTimeProvider.GetLocalNow():yyyy-MM-dd'T'HH_mm}.zip").Should().BeTrue();
    }

    [Test]
    public async Task ExportZipIncludesImageFiles()
    {
        var engines = TestDataHelper.CreateTestEngineList();
        var context = GetContext();
        context.Engines.AddRange(engines);
        await context.SaveChangesAsync();
        await CreateImageFiles(engines.Count);

        var service = new TransferService(CreateFakeContextFactory(), CreateFakeHubContext(), Substitute.For<ILogger<TransferService>>(), _defaultOptions, _dateTimeProvider);
        await service.ExportEverything();

        using var zip = ZipFile.OpenRead($"./transfer/TauStellwerk-Backup-{_dateTimeProvider.GetLocalNow():yyyy-MM-dd'T'HH_mm}.zip");
        zip.Entries.Count.Should().Be(engines.Count + 2,
            "There should be one entry for each engine and two for the ndjson files");
    }

    private async Task CreateImageFiles(int number)
    {
        var imageDirectory = _defaultOptions.OriginalImageDirectory;
        if (Directory.Exists(imageDirectory))
        {
            Directory.Delete(imageDirectory, true);
        }

        Directory.CreateDirectory(imageDirectory);
        for (var i = 1; i <= number; i++)
        {
            await File.WriteAllTextAsync($"{_defaultOptions.OriginalImageDirectory}/{i}.png", "test");
        }
    }

    private IDbContextFactory<StwDbContext> CreateFakeContextFactory()
    {
        var fake = Substitute.For<IDbContextFactory<StwDbContext>>();
        fake.CreateDbContextAsync().Returns(_ => Task.FromResult(GetContext()));
        return fake;
    }

    private static IHubContext<TauHub> CreateFakeHubContext()
    {
        return Substitute.For<IHubContext<TauHub>>();
    }
}
