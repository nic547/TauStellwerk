// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

namespace TauStellwerk.Util.Extensions;
public static class DictonaryExtensions
{

    public static TValue? TryGet<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
         where TKey : notnull
    {
        _ = dict.TryGetValue(key, out var value);
        return value;
    }
}
