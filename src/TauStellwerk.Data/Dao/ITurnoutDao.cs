// <copyright file="ITurnoutDao.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using FluentResults;
using TauStellwerk.Base;
using TauStellwerk.Data.Model;

namespace TauStellwerk.Data.Dao;

public interface ITurnoutDao
{
    public Task<Result<Turnout>> GetTurnoutById(int id);

    public Task<IReadOnlyList<Turnout>> GetTurnouts(int page);

    public Task<Result> AddOrUpdate(TurnoutDto dto);

    public Task<Result> Delete(TurnoutDto dto);
}