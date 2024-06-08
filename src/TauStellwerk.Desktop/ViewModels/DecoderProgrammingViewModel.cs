// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Splat;
using TauStellwerk.Client.Services;
using TauStellwerk.Client.Services.DecoderProgramming;

namespace TauStellwerk.Desktop.ViewModels;

public partial class DecoderProgrammingViewModel : ViewModelBase
{
    private readonly IDecoderProgrammingService _decoderProgrammingService;
    private readonly EngineService _engineService;

    [ObservableProperty]
    private int? _dccAddress;

    [ObservableProperty]
    private string _message = string.Empty;

    [ObservableProperty]
    private bool _isNotProgramming = true;

    [ObservableProperty]
    private List<string> _enginesWithSameAddress = [];

    [ObservableProperty]
    private bool _hasManyEnginesWithSameAddress;

    public DecoderProgrammingViewModel(IDecoderProgrammingService? programmingService = null, EngineService? engineService = null)
    {
        _decoderProgrammingService = programmingService ?? Locator.Current.GetRequiredService<IDecoderProgrammingService>();
        _engineService = engineService ?? Locator.Current.GetRequiredService<EngineService>();

        PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(DccAddress))
            {
                HandleChangedAddress();
            }
        };
    }

    private async void HandleChangedAddress()
    {
        if (DccAddress is > 0 and < 10240)
        {
            var engines = await _engineService.GetEnginesForAddress((int)DccAddress);
            EnginesWithSameAddress = engines.Take(5).Select(e => e.Name).ToList();
            HasManyEnginesWithSameAddress = engines.Count > 5;
        }
    }

    [RelayCommand]
    private async Task ReadDccAddress()
    {
        IsNotProgramming = false;

        DccAddress = null;
        var result = await _decoderProgrammingService.ReadDccAddress();
        if (result.Success)
        {
            DccAddress = result.Value;
            Message = string.Empty;
        }
        else
        {
            Message = result.Error ?? "Unspecified error";
        }
        IsNotProgramming = true;
    }

    [RelayCommand]
    private async Task WriteDccAddress()
    {
        IsNotProgramming = false;
        if (DccAddress is null)
        {
            Message = "Please enter a valid DCC address (1-10239)";
            IsNotProgramming = true;
            return;
        }
        await _decoderProgrammingService.WriteDccAddress((int)DccAddress);
        IsNotProgramming = true;
    }
}
