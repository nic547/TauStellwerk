// <copyright file="DefaultConfiguration.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace TauStellwerk;

public class DefaultConfiguration
{
    public static SortedDictionary<string, string> Values { get; } = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["connectionstrings:database"] = "Filename=StwDatabase.db;cache=shared",
        ["originalImageDirectory"] = "./originalImages",
        ["generatedImageDirectory"] = "./generatedImages",
    };
}