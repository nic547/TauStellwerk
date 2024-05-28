// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using System.IO.Compression;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TauStellwerk.Data;
using TauStellwerk.Data.Model;

namespace TauStellwerk.Server.Services.TransferService;

public class TransferService(
    StwDbContext dbContext,
    ILogger<TransferService> logger,
    IOptions<TauStellwerkOptions> options)
    : ITransferService
{
    private static readonly string BackupPrefix = "TauStellwerk-Backup-";

    public async Task ExportEverything()
    {
        await ExportEngines();
        await ExportTurnouts();

        using var zip = new ZipArchive(File.Create($"./transfer/{BackupPrefix}{DateTime.UtcNow:yyyy-MM-dd}.zip"), ZipArchiveMode.Create);
        zip.CreateEntryFromFile("./transfer/temp/engines.ndjson", "engines.ndjson");
        zip.CreateEntryFromFile("./transfer/temp/turnouts.ndjson", "turnouts.ndjson");

        foreach (var image in Directory.EnumerateFiles(options.Value.OriginalImageDirectory))
        {
            zip.CreateEntryFromFile(image, Path.GetFileName(image));
        }
    }

    public async Task ImportEverything()
    {
        await ImportEngines();
        await ImportTurnouts();
    }

    public Task ImportTurnouts()
    {
        throw new NotImplementedException();
    }

    public async Task ImportEngines()
    {
        using var reader = File.OpenText("./transfer/temp/engines.ndjson");
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
        Directory.CreateDirectory("./transfer/temp");

        var turnoutEnumereable = dbContext.Turnouts.AsEnumerable();
        using var writer = File.CreateText("./transfer/temp/turnouts.ndjson");
        {
            foreach (var turnout in turnoutEnumereable)
            {
                await writer.WriteLineAsync(JsonSerializer.Serialize(turnout, new JsonSerializerOptions() { }));
            }
        }
    }

    public async Task ExportEngines()
    {
        Directory.CreateDirectory("./transfer/temp");

        var engineEnumereable = dbContext.Engines.Include(x => x.Functions).Include(e => e.ECoSEngineData).AsEnumerable();
        using var writer = File.CreateText("./transfer/temp/engines.ndjson");
        {
            foreach (var engine in engineEnumereable)
            {
                await writer.WriteLineAsync(JsonSerializer.Serialize(engine, new JsonSerializerOptions() { }));
            }
        }
    }
}
