// <copyright file="Engine.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using PiStellwerk.Base.Model;

namespace PiStellwerk.Database.Model
{
    /// <summary>
    /// A choo-choo, in this context generally understood to be smaller than real-live-sized.
    /// </summary>
    public class Engine
    {
        /// <summary>
        /// Gets or sets the name of the choo-choo.
        /// </summary>
        public string Name { get; set; } = string.Empty;

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
        /// Gets a list of functions a decoder offers.
        /// </summary>
        public List<DccFunction> Functions { get; init; } = new List<DccFunction>();

        /// <summary>
        /// Gets a list of strings that describe an engine. These might be alternative names, manufacturers, the owner etc, basically
        /// everything one might search for if the exact name is unknown.
        /// TODO: HTMLEncode these before actually displaying them anywhere.
        /// </summary>
        public List<string> Tags { get; init; } = new List<string>();

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ECoSEngineData? ECoSEngineData { get; init; }

        public List<EngineImage> Image { get; init; } = new();

        public DateTime LastUsed { get; set; }

        /// <summary>
        /// Create a deep copy of this object.
        /// </summary>
        /// <returns>The copy.</returns>
        public Engine DeepClone()
        {
            // Probably not the fastest way for a deep clone, but very simple.
            var json = JsonSerializer.Serialize(this);
            var clonedObject = JsonSerializer.Deserialize<Engine>(json);
            return clonedObject ?? throw new InvalidDataException("Serialized object could not be deserialized.");
        }

        public override string ToString()
        {
            return Name;
        }

        public EngineDto ToEngineDto()
        {
            return new EngineDto()
            {
                Id = Id,
                Name = Name,
                Images = Image.Select(i => i.ToImageDto()).ToList(),
                Tags = Tags,
                LastUsed = LastUsed,
            };
        }

        public EngineFullDto ToEngineFullDto()
        {
            return new()
            {
                Id = Id,
                Name = Name,
                Images = Image.Select(i => i.ToImageDto()).ToList(),
                Tags = Tags,
                LastUsed = LastUsed,
                TopSpeed = TopSpeed,
                Address = Address,
                Functions = Functions.Select(f => f.ToFunctionDto()).ToList(),
            };
        }

        public void UpdateWith(EngineFullDto engineDto)
        {
            Name = HttpUtility.HtmlEncode(engineDto.Name);
            TopSpeed = engineDto.TopSpeed;
            Address = engineDto.Address;

            Tags.Clear();
            Tags.AddRange(engineDto.Tags.Select(t => HttpUtility.HtmlEncode(t)));
        }
    }
}
