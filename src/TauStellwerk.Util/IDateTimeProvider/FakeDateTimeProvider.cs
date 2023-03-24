// <copyright file="FakeDateTimeProvider.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace TauStellwerk.Util;

public class FakeDateTimeProvider : IDateTimeProvider
{
    public DateTime Value { get; set; } = DateTime.MinValue;

    public DateTime GetUtcNow()
    {
        return Value;
    }
}