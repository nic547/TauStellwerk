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
    public class CounterDictionary : IEnumerable<KeyValuePair<int, ulong>>
    {
        private readonly Dictionary<int, ulong> _dictionary = new Dictionary<int, ulong>();

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<int, ulong>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<int, ulong>>)_dictionary).GetEnumerator();
        }

        /// <summary>
        /// Calculate the average of all values with respect to how often it occured.
        /// </summary>
        /// <returns>The average of all values.</returns>
        public double Average()
        {
            var totalNumber = 0ul;
            var movingAverage = 1d;
            foreach (var kvp in _dictionary)
            {
                for (ulong u = 0; u < kvp.Value; u++)
                {
                    movingAverage += ((double)kvp.Key - movingAverage) / ++totalNumber;
                }
            }

            return movingAverage;
        }

        /// <summary>
        /// Combine a CounterDictionary with this CounterDictionary.
        /// </summary>
        /// <param name="dict2">The other CounterDictionary.</param>
        public void Combine(CounterDictionary dict2)
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
        public void Increment(int key)
        {
            Add(key, 1);
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_dictionary).GetEnumerator();
        }

        /// <summary>
        /// Returns the total number of occurrences this CounterDictionary recorded.
        /// </summary>
        /// <returns>Total number of occurrences.</returns>
        public ulong Total()
        {
            ulong result = 0;
            foreach (var kv in _dictionary)
            {
                result += kv.Value;
            }

            return result;
        }

        private void Add(int key, ulong value)
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
