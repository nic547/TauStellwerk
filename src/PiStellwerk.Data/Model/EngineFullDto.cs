﻿// <copyright file="EngineFullDto.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;

namespace PiStellwerk.Model.Model
{
    public class EngineFullDto : EngineDto
    {
        public List<FunctionDto> Functions { get; init; } = new();

        public ushort Address { get; set; }

        public int TopSpeed { get; set; }
    }
}
