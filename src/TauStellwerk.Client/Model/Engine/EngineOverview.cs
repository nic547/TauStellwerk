// <copyright file="EngineOverview.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Immutable;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using TauStellwerk.Base.Dto;

namespace TauStellwerk.Client.Model;

public partial class EngineOverview : ObservableObject
{
    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private bool _isHidden;

    public EngineOverview(EngineOverviewDto dto)
    {
        _name = dto.Name;
        IsHidden = dto.IsHidden;
        Id = dto.Id;
        Tags = new ObservableCollection<string>(dto.Tags);
        Images = EngineImage.CreateFromImageDtoList(dto.Images);
        LastUsed = dto.LastUsed;
        Created = dto.Created;
    }

    public EngineOverview()
    {
        _name = string.Empty;
        Tags = new ObservableCollection<string>();
        Images = new List<EngineImage>().ToImmutableList();
        LastUsed = DateTime.UtcNow;
        Created = DateTime.UtcNow;
    }

    public int Id { get; }

    public ObservableCollection<string> Tags { get; }

    public ImmutableList<EngineImage> Images { get; }

    public DateTime LastUsed { get; }

    public DateTime Created { get; }
}
