// <copyright file="ITurnoutService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Immutable;
using FluentResults;
using TauStellwerk.Base;
using TauStellwerk.Client.Model;

namespace TauStellwerk.Client.Services;

public interface ITurnoutService
{
    public Task<Result> ToggleState(Turnout turnout);

    public Task<IImmutableList<Turnout>> GetList(int page = 0);

    public Task<ResultDto> AddOrUpdate(Turnout turnout);

    public Task<ResultDto> Delete(Turnout turnout);
}