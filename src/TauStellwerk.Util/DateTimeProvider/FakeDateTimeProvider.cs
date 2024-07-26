// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

namespace TauStellwerk.Util.DateTimeProvider;

public class FakeDateTimeProvider : IDateTimeProvider
{
    public DateTime Value { get; set; } = DateTime.MinValue;

    public DateTime GetUtcNow()
    {
        return Value;
    }

    public DateTime GetLocalNow()
    {
        return Value;
    }
}
