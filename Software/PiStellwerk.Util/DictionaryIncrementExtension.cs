// <copyright file="DictionaryIncrementExtension.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;

namespace PiStellwerk.Util
{
    /// <summary>
    /// Extension methods for dictionaries with numeric values. The value is increased by 1, if no K/V Pair exists yet, a new one is created.
    /// </summary>
    public static class DictionaryIncrementExtension
    {
        /// <summary>
        /// Increases the value by one. If no key exists yet, a new one is added.
        /// </summary>
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <param name="dictionary">The dictionary this operation should be performed on.</param>
        /// <param name="key">The Key whose value should be incremented.</param>
        public static void IncrementValue<TKey>(this Dictionary<TKey, long> dictionary, TKey key)
        {
            if (dictionary.TryGetValue(key, out _))
            {
                dictionary[key]++;
            }
            else
            {
                dictionary.Add(key, 1);
            }
        }

        /// <inheritdoc cref="IncrementValue{TKey}(Dictionary{TKey,long},TKey)"/>
        public static void IncrementValue<TKey>(this Dictionary<TKey, ulong> dictionary, TKey key)
        {
            if (dictionary.TryGetValue(key, out _))
            {
                dictionary[key]++;
            }
            else
            {
                dictionary.Add(key, 1);
            }
        }

        /// <inheritdoc cref="IncrementValue{TKey}(Dictionary{TKey,long},TKey)"/>
        public static void IncrementValue<TKey>(this Dictionary<TKey, int> dictionary, TKey key)
        {
            if (dictionary.TryGetValue(key, out _))
            {
                dictionary[key]++;
            }
            else
            {
                dictionary.Add(key, 1);
            }
        }

        /// <inheritdoc cref="IncrementValue{TKey}(Dictionary{TKey,long},TKey)"/>
        public static void IncrementValue<TKey>(this Dictionary<TKey, uint> dictionary, TKey key)
        {
            if (dictionary.TryGetValue(key, out _))
            {
                dictionary[key]++;
            }
            else
            {
                dictionary.Add(key, 1);
            }
        }
    }
}
