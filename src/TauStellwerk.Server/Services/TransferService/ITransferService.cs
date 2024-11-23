// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

namespace TauStellwerk.Server.Services.TransferService;

public interface ITransferService
{
    Task ExportEverything();

    Task ImportEverything(string path);

    Task DeleteBackup(string filename);
}
