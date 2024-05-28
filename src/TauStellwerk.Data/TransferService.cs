// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using System.IO.Compression;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TauStellwerk.Data.Model;

namespace TauStellwerk.Data;
public class TransferService
{
    private readonly StwDbContext _dbContext;
    private readonly ILogger<TransferService> _logger;

    public TransferService(StwDbContext dbContext, ILogger<TransferService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task ExportEverything()
    {
        await ExportEngines();
        await ExportTurnouts();

        using var zip = new ZipArchive(File.Create("./transfer/transfer.zip"), ZipArchiveMode.Create);
        zip.CreateEntryFromFile("./transfer/engines.ndjson", "engines.ndjson");
        zip.CreateEntryFromFile("./transfer/turnouts.ndjson", "turnouts.ndjson");
    }

    public async Task ImportEverything()
    {
        await ImportEngines();
        await ImportTurnouts();
    }

    public async Task ImportTurnouts()
    {
        throw new NotImplementedException();
    }

    public async Task ImportEngines()
    {
        using var reader = File.OpenText("./transfer/engines.ndjson");
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            var engine = JsonSerializer.Deserialize<Engine>(line ?? string.Empty);
            if (engine != null)
            {
                _dbContext.Engines.Add(engine);
            }
        }
    }

    public async Task ExportTurnouts()
    {
        Directory.CreateDirectory("./transfer");

        var turnoutEnumereable = _dbContext.Turnouts.AsEnumerable();
        using var writer = File.CreateText("./transfer/turnouts.ndjson");
        {
            foreach (var turnout in turnoutEnumereable)
            {
                await writer.WriteLineAsync(JsonSerializer.Serialize(turnout, new JsonSerializerOptions() { }));
            }
        }
    }

    public async Task ExportEngines()
    {
        Directory.CreateDirectory("./transfer");

        var engineEnumereable = _dbContext.Engines.Include(x => x.Functions).Include(e => e.ECoSEngineData).AsEnumerable();
        using var writer = File.CreateText("./transfer/engines.ndjson");
        {
            foreach (var engine in engineEnumereable)
            {
                await writer.WriteLineAsync(JsonSerializer.Serialize(engine, new JsonSerializerOptions() { }));
            }
        }
    }
}
