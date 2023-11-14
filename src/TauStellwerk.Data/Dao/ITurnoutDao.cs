// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using FluentResults;
using TauStellwerk.Base.Dto;
using TauStellwerk.Data.Model;

namespace TauStellwerk.Data.Dao;

public interface ITurnoutDao
{
    public Task<Result<Turnout>> GetTurnoutById(int id);

    public Task<IReadOnlyList<Turnout>> GetTurnouts(int page);

    public Task<Result> AddOrUpdate(TurnoutDto dto);

    public Task<Result> Delete(TurnoutDto dto);
}
