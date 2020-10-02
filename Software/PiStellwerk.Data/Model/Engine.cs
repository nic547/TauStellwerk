// <copyright file="Engine.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Web;

namespace PiStellwerk.Data
{
    /// <summary>
    /// A choo-choo, in this context generally understood to be smaller than real-live-sized.
    /// </summary>
    public class Engine : IEquatable<Engine>
    {
        private string _name;

        /// <summary>
        /// Gets or sets the name of the choo-choo.
        /// </summary>
        public string Name
        {
            get => _name;
            set => _name = HttpUtility.HtmlEncode(value);
        }

        /// <summary>
        /// Gets or sets the Id of the Engine in the database system.
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the DCC Address of an Engine. Is not necessarily unique.
        /// </summary>
        public ushort Address { get; set; }

        /// <summary>
        /// Gets or sets the amount of speed steps the decoder supports.
        /// </summary>
        public byte SpeedSteps { get; set; }

        /// <summary>
        /// Gets or sets the top speed of the real thing.
        /// </summary>
        public int TopSpeed { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="SpeedDisplayType"/>, indicating how the speed of the model should be displayed.
        /// </summary>
        public SpeedDisplayType SpeedDisplayType { get; set; }

        /// <summary>
        /// Gets or sets the functions a decoder offers.
        /// </summary>
        public List<DccFunction> Functions { get; set; }

        /// <summary>
        /// Gets or sets a list of strings that describe an engine. These might be alternative names, manufacturers, the owner etc, basically
        /// everything one might search for if the exact name is unknown.
        /// TODO: HTMLEncode these before actually displaying them anywhere.
        /// </summary>
        public List<string> Tags { get; set; }

        /// <inheritdoc/>
        public bool Equals(Engine other)
        {
            if (other == null)
            {
                return false;
            }

            return
                Name.Equals(other.Name) &&
                Id.Equals(other.Id) &&
                Address.Equals(other.Address) &&
                SpeedSteps.Equals(other.SpeedSteps) &&
                TopSpeed.Equals(other.TopSpeed) &&
                SpeedDisplayType.Equals(other.SpeedDisplayType) &&
                Functions.Equals(other.Functions) &&
                Tags.Equals(other.Tags);
        }

        /// <summary>
        /// Create a deep copy of this object.
        /// </summary>
        /// <returns>The copy.</returns>
        public Engine DeepClone()
        {
            // Probably not the fastest way for a deep clone, but very simple.
            var json = JsonSerializer.Serialize(this);
            return JsonSerializer.Deserialize<Engine>(json);
        }
    }
}
