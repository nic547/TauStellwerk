// <copyright file="CounterDictionary.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections;
using System.Collections.Generic;

namespace PiStellwerk.Util
{
    /// <summary>
    /// Dictionary for counting how often a key occured.
    /// </summary>
    /// <typeparam name="TKey">Type of Key.</typeparam>
    public class CounterDictionary<TKey> : IEnumerable<KeyValuePair<TKey, ulong>>
    {
        private readonly Dictionary<TKey, ulong> _dictionary = new Dictionary<TKey, ulong>();

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<TKey, ulong>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<TKey, ulong>>)_dictionary).GetEnumerator();
        }

        /// <summary>
        /// Combine a CounterDictionary with this CounterDictionary.
        /// </summary>
        /// <param name="dict2">The other CounterDictionary.</param>
        public void Combine(CounterDictionary<TKey> dict2)
        {
            foreach (var kv in dict2)
            {
                Add(kv.Key, kv.Value);
            }
        }

        /// <summary>
        /// Increment the value of a key by one.
        /// </summary>
        /// <param name="key">The key.</param>
        public void Increment(TKey key)
        {
            Add(key, 1);
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_dictionary).GetEnumerator();
        }

        private void Add(TKey key, ulong value)
        {
            if (_dictionary.TryGetValue(key, out _))
            {
                _dictionary[key] += value;
            }
            else
            {
                _dictionary.Add(key, value);
            }
        }
    }
}
