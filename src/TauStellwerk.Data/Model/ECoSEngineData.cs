// <copyright file="ECoSEngineData.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Web;

namespace TauStellwerk.Data.Model;

/// <summary>
/// Data used by the ECoS-related components.
/// </summary>
public record ECoSEngineData
{
    private readonly string _name = string.Empty;

    /// <summary>
    /// Gets the Name of the choo-choo as in the ECoS.
    /// </summary>
    public string Name
    {
        get => _name;
        init => _name = HttpUtility.HtmlEncode(value);
    }

    /// <summary>
    /// Gets the Id of an engine in the ECoS.
    /// </summary>
    public int Id { get; init; }
}