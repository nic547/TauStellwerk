// <copyright file="ResultDto.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using FluentResults;

namespace TauStellwerk.Base.Dto;

public class ResultDto
{
    public ResultDto(bool success, string? error)
    {
        Success = success;
        Error = error;
    }

    public string? Error { get; }

    public bool Success { get; }

    public static implicit operator ResultDto(Result result)
    {
        return new ResultDto(result.IsSuccess, string.Join(' ', result.Errors.Select(e => e.Message)));
    }
}