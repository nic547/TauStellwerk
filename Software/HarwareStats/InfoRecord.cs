// <copyright file="InfoRecord.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace HardwareInfo
{
    /// <summary>
    /// A specific information about a hardware thing.
    /// Examples: CPU Temperature, used Memory etc.
    /// </summary>
    public class InfoRecord
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InfoRecord"/> class.
        /// </summary>
        public InfoRecord()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InfoRecord"/> class.
        /// </summary>
        /// <param name="type"><see cref="InfoType"/></param>
        /// <param name="name"><see cref="Name"/></param>
        /// <param name="value"><see cref="Value"/></param>
        public InfoRecord(InfoType type, string name, string value)
        {
            Type = type;
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="InfoType"/>.
        /// </summary>
        public InfoType Type { get; set; }

        /// <summary>
        /// Gets or sets the name of the stat.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value of the stat.
        /// </summary>
        public string Value { get; set; }
    }
}
