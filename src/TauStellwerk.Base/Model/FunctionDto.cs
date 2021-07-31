// <copyright file="FunctionDto.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace TauStellwerk.Base.Model
{
    public class FunctionDto
    {
        public FunctionDto(byte number, string name)
        {
            Number = number;
            Name = name;
        }

        /// <summary>
        /// Gets or sets the dcc function number this function has (F0 or F18 for example).
        /// </summary>
        public byte Number { get; set; }

        /// <summary>
        /// Gets or sets the name that this function should have.
        /// </summary>
        public string Name { get; set; }

        public override string ToString() => $"F{Number} - {Name}";
    }
}
