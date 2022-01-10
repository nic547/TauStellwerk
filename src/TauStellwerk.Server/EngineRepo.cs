// <copyright file="EngineRepo.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TauStellwerk.Base.Model;
using TauStellwerk.Database;
using TauStellwerk.Database.Model;

namespace TauStellwerk;

public class EngineRepo
{
    private const int ResultsPerPage = 20;

    private readonly StwDbContext _dbContext;
    private readonly ILogger<EngineRepo> _logger;

    public EngineRepo(StwDbContext dbContext, ILogger<EngineRepo> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<EngineFullDto>> GetEngineFullDto(int id)
    {
        var engine = await _dbContext.Engines
            .AsNoTracking()
            .AsSingleQuery()
            .Include(x => x.Functions)
            .Include(x => x.Images)
            .Include(x => x.Tags)
            .SingleOrDefaultAsync(x => x.Id == id)
            .ContinueWith(x => x.Result?.ToEngineFullDto());

        if (engine == null)
        {
            return Result.Fail("Engine not found");
        }

        return Result.Ok(engine);
    }

    public async Task<Result<Engine>> GetEngine(int id)
    {
        var engine = await _dbContext.Engines
            .AsNoTracking()
            .AsSingleQuery()
            .Include(x => x.Functions)
            .Include(x => x.Images)
            .Include(x => x.Tags)
            .Include(e => e.ECoSEngineData)
            .SingleOrDefaultAsync(x => x.Id == id);

        if (engine == null)
        {
            return Result.Fail("Engine not found");
        }

        return Result.Ok(engine);
    }

    public async Task<IList<EngineDto>> GetEngineList(int page = 0, bool showHiddenEngines = false, SortEnginesBy sortBy = SortEnginesBy.LastUsed, bool sortDescending = true)
    {
        var query = _dbContext.Engines
            .AsNoTracking()
            .AsSingleQuery(); // TODO: Why does AsSplitQuery break this?

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
            .Take(ResultsPerPage)
            .Include(e => e.Images)
            .Include(x => x.Tags);

        var result = await query.ToListAsync();
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
                .Include(x => x.Tags)
                .SingleOrDefaultAsync(x => x.Id == engineDto.Id);

            if (engine == null)
            {
                return Result.Fail("Engine was not found");
            }
        }

        await engine.UpdateWith(engineDto, _dbContext);

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError($"Exception while updating engine: {e.GetType()}");
            return Result.Fail("Could not update engine");
        }

        engineDto.Id = engine.Id;

        return Result.Ok(engineDto);
    }

    public async Task UpdateLastUsed(int id)
    {
        var engine = await _dbContext.Engines.SingleAsync(e => e.Id == id);
        engine.LastUsed = DateTime.Now;
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Result> Delete(int id)
    {
        var engine = await _dbContext.Engines
            .Include(e => e.Images)
            .Include(e => e.Functions)
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
