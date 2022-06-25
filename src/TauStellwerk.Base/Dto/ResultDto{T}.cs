// <copyright file="ResultDto{T}.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Linq;
using FluentResults;

namespace TauStellwerk.Base.Dto;

public class ResultDto<T>
{
    public ResultDto(T? value, bool success, string? error)
    {
        Value = value;
        Success = success;
        Error = error;
    }

    public string? Error { get; }

    public bool Success { get; }

    public T? Value { get; }

    public static implicit operator ResultDto<T>(Result<T> result)
    {
        return new ResultDto<T>(result.ValueOrDefault, result.IsSuccess, string.Join(' ', result.Errors.Select(e => e.Message)));
    }

    public static implicit operator ResultDto<T>(Result result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException();
        }

        return new ResultDto<T>(default, false, string.Join(' ', result.Errors.Select(e => e.Message)));
    }
}
