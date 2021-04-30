// <copyright file="CounterDictionary.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PiStellwerk.Util
{
    /// <summary>
    /// Dictionary for counting how often a key occured.
    /// </summary>
    public class CounterDictionary : IEnumerable<KeyValuePair<int, long>>
    {
        private readonly Dictionary<int, long> _dictionary = new();

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<int, long>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<int, long>>)_dictionary).GetEnumerator();
        }

        /// <summary>
        /// Calculate the average of all values with respect to how often it occured.
        /// Returns null if no value was counted yet.
        /// </summary>
        /// <returns>The average of all values.</returns>
        public double? Average()
        {
            ulong totalNumber = 0;
            double movingAverage = 0;

            if (!this.Any())
            {
                return null;
            }

            foreach (var kvp in _dictionary)
            {
                for (long u = 0; u < kvp.Value; u++)
                {
                    movingAverage += (kvp.Key - movingAverage) / ++totalNumber;
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
        /// Get the 90th Percentile.
        /// </summary>
        /// <returns>Value representing the 90th Percentile.</returns>
        public int? Get90ThPercentile()
        {
            return GetPercentile(0.9);
        }

        /// <summary>
        /// Get the 95th Percentile.
        /// </summary>
        /// <returns>Value representing the 95th Percentile.</returns>
        public int? Get95ThPercentile()
        {
            return GetPercentile(0.95);
        }

        /// <summary>
        /// Get the 99th Percentile.
        /// </summary>
        /// <returns>Value representing the 99th Percentile.</returns>
        public int? Get99ThPercentile()
        {
            return GetPercentile(0.99);
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
        /// Get a value by it's key. If key doesn't exist, 0 is returned instead.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The value.</returns>
        public long GetByKey(int key)
        {
            _dictionary.TryGetValue(key, out long value);
            return value;
        }

        /// <summary>
        /// Returns the total number of occurrences this CounterDictionary recorded.
        /// </summary>
        /// <returns>Counted number of occurrences.</returns>
        public long Count()
        {
            long result = 0;
            foreach (var kv in _dictionary)
            {
                result += kv.Value;
            }

            return result;
        }

        private void Add(int key, long value)
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

        private int? GetPercentile(double k)
        {
            var index = (long)Math.Round(Count() * k);
            foreach (var (key, value) in _dictionary.OrderBy(kvp => kvp.Key))
            {
                if (value < index)
                {
                    index -= value;
                }
                else
                {
                    return key;
                }
            }

            return null;
        }
    }
}
