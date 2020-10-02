﻿// <copyright file="DccFunction.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PiStellwerk.Data
{
    /// <summary>
    /// A Function as programmed on the physical dcc decoder in a model engine.
    /// </summary>
    public class DccFunction : IEquatable<DccFunction>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DccFunction"/> class.
        /// </summary>
        public DccFunction()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DccFunction"/> class.
        /// </summary>
        /// <param name="number"><see cref="Number"/>.</param>
        /// <param name="name"><see cref="Name"/>.</param>
        public DccFunction(byte number, string name)
        {
            Number = number;
            Name = name;
        }

        /// <summary>
        /// Gets or sets the Id of the Function as per Database.
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the dcc function number this function has (F0 or F18 for example).
        /// </summary>
        public byte Number { get; set; }

        /// <summary>
        /// Gets or sets the name that this function should have.
        /// </summary>
        public string Name { get; set; }

        /// <inheritdoc/>
        public bool Equals(DccFunction other)
        {
            if (other == null)
            {
                return false;
            }

            return
                Name == other.Name &&
                Number == other.Number &&
                Id == other.Id;
        }
    }
}