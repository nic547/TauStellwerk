// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using System.IO.Compression;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TauStellwerk.Base.Dto;
using TauStellwerk.Data;
using TauStellwerk.Data.Model;
using TauStellwerk.Server.Hub;
using TauStellwerk.Util.DateTimeProvider;

namespace TauStellwerk.Server.Services.TransferService;

public class TransferService(
    IDbContextFactory<StwDbContext> dbContextFactory,
    IHubContext<TauHub> hubContext,
    ILogger<TransferService> logger,
    TauStellwerkOptions options,
    IDateTimeProvider? nowProvider = null)
    : ITransferService
{
    private static readonly string BackupPrefix = "TauStellwerk-Backup-";
    private readonly IDateTimeProvider _nowProvider = nowProvider ?? new DateTimeProvider();
    
    private static readonly JsonSerializerOptions _serializerOptions = new();

    public async Task ExportEverything()
    {
        await ExportEngines();
        await ExportTurnouts();

        // Local time is used intentionally. Format is not quite ISO8601 because windows doesn't like colons in filenames.
        var filename = $"{BackupPrefix}{_nowProvider.GetLocalNow():yyyy-MM-dd'T'HH_mm}.zip";

        using var zip = new ZipArchive(File.Create($"./transfer/{filename}"), ZipArchiveMode.Create);
        zip.CreateEntryFromFile("./transfer/temp/engines.ndjson", "engines.ndjson");
        zip.CreateEntryFromFile("./transfer/temp/turnouts.ndjson", "turnouts.ndjson");

        foreach (var image in Directory.EnumerateFiles(options.OriginalImageDirectory))
        {
            zip.CreateEntryFromFile(image, Path.GetFileName(image));
        }

        logger.LogInformation("Backup created: {filename}", filename);
        await hubContext.Clients.All.SendAsync("BackupCreated", new BackupInfoDto(filename, new FileInfo("./transfer/" + filename).Length));
    }

    public async Task ImportEverything(string filename)
    {
        var context = await dbContextFactory.CreateDbContextAsync();
        var engineCount = await context.Engines.CountAsync();
        if (engineCount != 0)
        {
            return;
        }
        
        var zip = new ZipArchive(File.OpenRead(filename));
        var destinationDirectory = Path.Combine(options.DataTransferDirectory, "import");
        zip.ExtractToDirectory(destinationDirectory);
        
        await ImportEngines(destinationDirectory);
        await ImportTurnouts(destinationDirectory);
    }

    public async Task DeleteBackup(string filename)
    {
        File.Delete(Path.Combine(options.DataTransferDirectory, filename));
        logger.LogInformation("Backup deleted: {filename}", filename);

        await hubContext.Clients.All.SendAsync("BackupDeleted", filename);
    }

    public Task ImportTurnouts(string directory)
    {
        // Do Nuthin for now
        return Task.CompletedTask;
    }

    public async Task ImportEngines(string directory)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        using var reader = File.OpenText(Path.Combine(directory, "engines.ndjson"));
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            var engine = JsonSerializer.Deserialize<Engine>(line ?? string.Empty);
            if (engine != null)
            {
                dbContext.Engines.Add(engine);
            }
        }
    }

    public async Task ExportTurnouts()
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        Directory.CreateDirectory("./transfer/temp");

        var turnoutEnumereable = dbContext.Turnouts.AsEnumerable();
        using var writer = File.CreateText("./transfer/temp/turnouts.ndjson");
        {
            foreach (var turnout in turnoutEnumereable)
            {
                await writer.WriteLineAsync(JsonSerializer.Serialize(turnout, _serializerOptions));
            }
        }
    }

    public async Task ExportEngines()
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        Directory.CreateDirectory("./transfer/temp");

        var engineEnumereable = dbContext.Engines.Include(x => x.Functions).Include(e => e.ECoSEngineData).AsEnumerable();
        using var writer = File.CreateText("./transfer/temp/engines.ndjson");
        {
            foreach (var engine in engineEnumereable)
            {
                await writer.WriteLineAsync(JsonSerializer.Serialize(engine, _serializerOptions));
            }
        }
    }
}
