// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

namespace TauStellwerk.Server;

public class TauStellwerkOptions
{
    public bool ResetEnginesWithoutState { get; init; } = true;

    public bool StopOnLastUserDisconnect { get; init; } = true;

    public string OriginalImageDirectory { get; init; } = "./originalImages";

    public string GeneratedImageDirectory { get; init; } = "./generatedImages";

    public string DataTransferDirectory { get; init; } = "./transfer";

    public DatabaseOptions Database { get; init; } = new();

    public class DatabaseOptions
    {
        public string ConnectionString { get; init; } = "Filename=StwDatabase.db;cache=shared";
    }
}
