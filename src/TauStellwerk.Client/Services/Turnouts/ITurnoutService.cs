// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using FluentResults;
using TauStellwerk.Base.Dto;
using TauStellwerk.Client.Model;

namespace TauStellwerk.Client.Services.Turnouts;

public interface ITurnoutService
{
    public event EventHandler<TurnoutStateChangedEventArgs> TurnoutStateChanged;

    public Task<Result> ToggleState(Turnout turnout);

    public Task<IImmutableList<Turnout>> GetList(int page = 0);

    public Task<ResultDto> AddOrUpdate(Turnout turnout);

    public Task<ResultDto> Delete(Turnout turnout);
}
