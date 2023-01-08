// <copyright file="EngineDao.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Diagnostics;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using TauStellwerk.Base;
using TauStellwerk.Server.Database;
using TauStellwerk.Server.Database.Model;

namespace TauStellwerk.Server.Dao;

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

    public async Task<IList<EngineOverviewDto>> GetEngineList(int page = 0, bool showHiddenEngines = false, SortEnginesBy sortBy = SortEnginesBy.LastUsed, bool sortDescending = true)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        var query = _dbContext.Engines
            .AsNoTracking()
            .AsSplitQuery();

        if (!showHiddenEngines)
        {
            query = query.Where(e => !e.IsHidden);
        }

        query = (sortBy, sortDescending) switch
        {
            (SortEnginesBy.Created, false) => query.OrderBy(e => e.Created),
            (SortEnginesBy.Created, true) => query.OrderByDescending(e => e.Created),
            (SortEnginesBy.Name, false) => query.OrderBy(e => e.Name),
            (SortEnginesBy.Name, true) => query.OrderByDescending(e => e.Name),
            (SortEnginesBy.LastUsed, false) => query.OrderBy(e => e.LastUsed),
            (SortEnginesBy.LastUsed, true) => query.OrderByDescending(e => e.LastUsed),
            _ => throw new InvalidOperationException(),
        };

        query = query.Skip(page * ResultsPerPage)
            .Take(ResultsPerPage);

        var result = await query.ToListAsync();
        _logger.LogDebug("EngineList page {page} was queried in {time}ms", page, stopwatch.ElapsedMilliseconds);
        return result.Select(e => e.ToEngineDto()).ToList();
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
