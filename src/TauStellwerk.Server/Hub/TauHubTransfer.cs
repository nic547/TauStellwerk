// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TauStellwerk.Server.Services.TransferService;

namespace TauStellwerk.Server.Hub;

public partial class TauHub
{
    public Task StartBackup([FromServices] ITransferService transferService)
    {
        _ = Task.Run(transferService.ExportEverything);
        return Task.CompletedTask;
    }

    public List<string> GetBackups([FromServices] IOptions<TauStellwerkOptions> options)
    {
        return Directory.EnumerateFiles(options.Value.DataTransferDirectory, $"*.zip")
            .Select(Path.GetFileName)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList()!; // Absolutly shouldn't be null since we filter out null or empty strings
    }
}
