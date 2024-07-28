// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TauStellwerk.Base.Dto;
using TauStellwerk.Server.Services.TransferService;

namespace TauStellwerk.Server.Hub;

public partial class TauHub
{
    public Task StartBackup([FromServices] ITransferService transferService)
    {
        _ = Task.Run(transferService.ExportEverything);
        return Task.CompletedTask;
    }

    public List<BackupInfoDto> GetBackups([FromServices] IOptions<TauStellwerkOptions> options)
    {
        return Directory.EnumerateFiles(options.Value.DataTransferDirectory, $"*.zip")
            .Select(path => new BackupInfoDto(Path.GetFileName(path), new FileInfo(path).Length))
            .OrderByDescending(x => x.FileName)
            .ToList();
    }

    public async Task DeleteBackup(string filename, [FromServices] ITransferService transferService)
    {
        await transferService.DeleteBackup(filename);
    }
}
