// <copyright file="TurnoutDao.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using FluentResults;
using Microsoft.EntityFrameworkCore;
using TauStellwerk.Base;
using TauStellwerk.Data.Model;

namespace TauStellwerk.Data.Dao;

public class TurnoutDao : ITurnoutDao
{
    private const int TurnoutsPerPage = 20;

    private readonly StwDbContext _dbContext;

    public TurnoutDao(StwDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<Turnout>> GetTurnoutById(int id)
    {
        var turnout = await _dbContext.Turnouts.SingleOrDefaultAsync(t => t.Id == id);
        return turnout is null ? Result.Fail($"Failed to find turnout with id:{id}") : Result.Ok(turnout);
    }

    public async Task<IReadOnlyList<Turnout>> GetTurnouts(int page)
    {
        return await _dbContext.Turnouts
            .OrderBy(t => t.Id)
            .Skip(page * TurnoutsPerPage)
            .Take(TurnoutsPerPage)
            .ToListAsync();
    }

    public async Task<Result> AddOrUpdate(TurnoutDto dto)
    {
        var turnout = Turnout.FromDto(dto);
        return await Result.Try(async Task () =>
            {
                _dbContext.Turnouts.Update(turnout);
                await _dbContext.SaveChangesAsync();
            });
    }

    public async Task<Result> Delete(TurnoutDto dto)
    {
        var turnout = Turnout.FromDto(dto);
        return await Result.Try(async Task () =>
        {
            _dbContext.Turnouts.Remove(turnout);
            await _dbContext.SaveChangesAsync();
        });
    }
}