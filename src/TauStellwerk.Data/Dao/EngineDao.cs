// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Globalization;
using FluentResults;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TauStellwerk.Base.Dto;
using TauStellwerk.Base.Model;
using TauStellwerk.Data.Model;

namespace TauStellwerk.Data.Dao;

public class EngineDao
{
    private const int ResultsPerPage = 20;

    private readonly StwDbContext _dbContext;
    private readonly ILogger<EngineDao> _logger;

    public EngineDao(StwDbContext dbContext, ILogger<EngineDao> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<Engine>> GetEngine(int id)
    {
        var engine = await _dbContext.Engines
            .AsSingleQuery()
            .Include(x => x.Functions)
            .Include(e => e.ECoSEngineData)
            .SingleOrDefaultAsync(x => x.Id == id);

        if (engine == null)
        {
            return Result.Fail("Engine not found");
        }

        engine.LastUsed = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        return Result.Ok(engine);
    }

    public async Task<IList<EngineOverviewDto>> GetEngineList(
        string searchTerm,
        int page,
        bool showHiddenEngines,
        SortEnginesBy sortBy,
        bool sortDescending)
    {
        var stopwatch = Stopwatch.StartNew();

        searchTerm = $"%{searchTerm}%";
        SqliteParameter searchParameter = new("searchTerm", $"%{searchTerm}%");

        var query = _dbContext.Engines.FromSqlRaw(
            $"SELECT * FROM Engines " +
            $"WHERE (tags LIKE $searchTerm OR name LIKE $searchTerm) " +
            $"{(showHiddenEngines ? string.Empty : "AND isHidden = FALSE ")}" +
            $"ORDER BY {sortBy} {(sortDescending ? "DESC" : "ASC")} " +
            $"LIMIT {ResultsPerPage} OFFSET {page * ResultsPerPage};",
            searchParameter);

        var result = await query.ToListAsync();
        _logger.LogDebug("EngineList page {page} was queried in {time}ms", page, stopwatch.Elapsed.TotalMilliseconds.ToString("F2", CultureInfo.CurrentCulture));
        return result.Select(e => e.ToEngineDto()).ToList();
    }

    public async Task<List<EngineOverviewDto>> GetEnginesWithAddress(int address)
    {
        var stopwatch = Stopwatch.StartNew();

        var result = await _dbContext.Engines
            .AsSingleQuery()
            .Where(e => e.Address == address)
            .Take(6)
            .Select(e => e.ToEngineDto())
            .ToListAsync();

        _logger.LogDebug("Engines with address {address} were queried in {time}ms", address, stopwatch.Elapsed.TotalMilliseconds.ToString("F2", CultureInfo.CurrentCulture));
        return result;
    }

    public async Task<Result<EngineFullDto>> UpdateOrAdd(EngineFullDto engineDto)
    {
        Engine? engine;
        if (engineDto.Id == 0)
        {
            engine = new Engine();
            _dbContext.Engines.Add(engine);
        }
        else
        {
            engine = await _dbContext.Engines
                .Include(x => x.Functions)
                .AsSingleQuery()
                .SingleOrDefaultAsync(x => x.Id == engineDto.Id);

            if (engine == null)
            {
                return Result.Fail("Engine was not found");
            }
        }

        engine.UpdateWith(engineDto);

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError("Exception while updating engineOverview: {exception}", e);
            return Result.Fail("Could not update engineOverview");
        }

        engineDto.Id = engine.Id;

        return Result.Ok(engineDto);
    }

    public async Task<Result> Delete(int id)
    {
        var engine = await _dbContext.Engines
            .Include(e => e.Functions)
            .AsSingleQuery()
            .SingleOrDefaultAsync(e => e.Id == id);
        if (engine == null)
        {
            return Result.Fail("Engine was not found");
        }

        _dbContext.Engines.Remove(engine);
        await _dbContext.SaveChangesAsync();
        return Result.Ok();
    }
}
