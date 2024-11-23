// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TauStellwerk.Data;
using TauStellwerk.Data.ImageService;
using TauStellwerk.Server.Services.TransferService;

namespace TauStellwerk.Server.Controllers;

[ApiController]
public class BackupUploadController : ControllerBase
{
    private readonly TauStellwerkOptions _options;
    private readonly TransferService _transferService;
    private readonly StwDbContext _context;

    public BackupUploadController(IOptions<TauStellwerkOptions> options, TransferService transferService, StwDbContext context)
    {
        _options = options.Value;

        _transferService = transferService;
        _context = context;
    }

    [HttpPost]
    [Route("/upload/backup")]
    public Task UploadBackup(IFormFile file)
    {
        var filename = _options.DataTransferDirectory + "/import.zip";
        using var localFile = System.IO.File.Create(filename);
        file.CopyTo(localFile);
        _ = _transferService.ImportEverything(filename);
        return Task.CompletedTask;
    }
}
