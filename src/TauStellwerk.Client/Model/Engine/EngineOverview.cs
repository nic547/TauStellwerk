// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using TauStellwerk.Base.Dto;
using TauStellwerk.Client.Model.Engine;

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
        ImageTimestamp = dto.ImageTimestamp;
        LastUsed = dto.LastUsed;
        Created = dto.Created;
    }

    public EngineOverview()
    {
        _name = string.Empty;
        Tags = [];
        Images = [];
        LastUsed = DateTime.UtcNow;
        Created = DateTime.UtcNow;
    }

    public int Id { get; init; }

    public ObservableCollection<string> Tags { get; init; }

    public ImmutableList<EngineImage> Images { get; init; }

    public int ImageTimestamp { get; init; }

    public DateTime LastUsed { get; init; }

    public DateTime Created { get; }
}
