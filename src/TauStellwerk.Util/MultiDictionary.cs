// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.


using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace TauStellwerk.Util;

/// <summary>
/// <see cref="Dictionary{TKey,TValue}"/> that can take multiple values per Key.
/// </summary>
/// <typeparam name="TKey">Type of Key.</typeparam>
/// <typeparam name="TValue">Type of Value.</typeparam>
public class MultiDictionary<TKey, TValue>
    where TKey : notnull
{
    private readonly Dictionary<TKey, List<TValue>> _dict = new();

    /// <summary>
    /// Add a value to the MultiDictionary for the specified key.
    /// </summary>
    /// <param name="key">Key for which the value should be added.</param>
    /// <param name="value">Value to add.</param>
    public void Add(TKey key, TValue value)
    {
        lock (_dict)
        {
            _dict.TryGetValue(key, out var existingList);
            if (existingList == null)
            {
                _dict.Add(key, new List<TValue> { value });
            }
            else
            {
                existingList.Add(value);
            }
        }
    }

    /// <summary>
    /// Remove and return the first value associated with a specific key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The removed value, null if no value was removed.</param>
    /// <returns>True if a value has been removed successfully, false if not.</returns>
    public bool TryRemoveFirst(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        lock (_dict)
        {
            _dict.TryGetValue(key, out var list);
            if (list == null)
            {
                value = default;
                return false;
            }
            else
            {
                value = list[0];
                list.RemoveAt(0);

                if (!list.Any())
                {
                    _dict.Remove(key);
                }

                return true;
            }
        }
    }

    /// <summary>
    /// Get all values associated with a specific key.
    /// </summary>
    /// <param name="key">The key to look for.</param>
    /// <param name="list">List of found values, null if none were found.</param>
    /// <returns>True if there were values associated with the specific key, false if not.</returns>
    public bool TryGetAllValues(TKey key, out IImmutableList<TValue>? list)
    {
        lock (_dict)
        {
            _dict.TryGetValue(key, out var foundList);
            if (foundList == null)
            {
                list = null;
                return false;
            }
            else
            {
                list = foundList.ToImmutableList();
                return true;
            }
        }
    }
}
