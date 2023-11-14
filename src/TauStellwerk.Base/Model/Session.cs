// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.


using TauStellwerk.Util;

namespace TauStellwerk.Base.Model;

/// <summary>
/// A user of the TauStellwerk software. Does not necessarily correspond to actual people.
/// </summary>
public class Session
{
    public Session(string connectionId, string username)
    {
        ConnectionId = connectionId;
        UserName = username;
    }

    public string UserName { get; set; }

    public string ConnectionId { get; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"'{ConnectionId.Left(8)}':'{UserName}'";
    }
}
