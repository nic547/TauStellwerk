// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

namespace TauStellwerk.Util;

public static class TakeRandomExtension
{
    public static T TakeRandom<T>(this ICollection<T> collection, Random random)
    {
        var index = random.Next(0, collection.Count);
        return collection.ElementAt(index);
    }
}
