﻿// <copyright file="EngineDto.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace PiStellwerk.Base.Model
{
    public class EngineDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public List<string> Tags { get; init; } = new List<string>();

        public List<ImageDto> Images { get; init; } = new();

        public DateTime LastUsed { get; set; }

        public DateTime Created { get; set; }

        public bool IsHidden { get; set; }
    }
}