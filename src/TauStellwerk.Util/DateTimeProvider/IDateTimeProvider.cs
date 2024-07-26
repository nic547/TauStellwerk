// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

namespace TauStellwerk.Util.DateTimeProvider;

public interface IDateTimeProvider
{
    public DateTime GetUtcNow();

    public DateTime GetLocalNow();
}

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime GetUtcNow()
    {
        return DateTime.UtcNow;
    }

    public DateTime GetLocalNow()
    {
        return DateTime.Now;
    }
}
