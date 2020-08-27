// <copyright file="CounterDictionaryTests.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using NUnit.Framework;

namespace PiStellwerk.Util.Tests
{
    /// <summary>
    /// Tests for <see cref="CounterDictionary"/>.
    /// </summary>
    public class CounterDictionaryTests
    {
        /// <summary>
        /// Just a simple test testing Average().
        /// </summary>
        [Test]
        public void AverageTest()
        {
            var ct = SimpleTestCounterDictionary();
            Assert.AreEqual(1.5d, ct.Average());
        }

        /// <summary>
        /// Test that totals are calculated correctly.
        /// </summary>
        [Test]
        public void TotalTest()
        {
            var ct = SimpleTestCounterDictionary();
            Assert.AreEqual(6, ct.Total());

            var ctEmpty = new CounterDictionary();
            Assert.AreEqual(0, ctEmpty.Total());
        }

        /// <summary>
        /// Test that percentiles are calculated correctly.
        /// </summary>
        [Test]
        public void PercentileTest()
        {
            var ct = new CounterDictionary();
            for (int i = 1; i <= 100_000; i++)
            {
                ct.Increment(i);
            }

            Assert.AreEqual(90_000, ct.Get90ThPercentile());
            Assert.AreEqual(95_000, ct.Get95ThPercentile());
            Assert.AreEqual(99_000, ct.Get99ThPercentile());
        }

        private static CounterDictionary SimpleTestCounterDictionary()
        {
            var ct = new CounterDictionary();
            ct.Increment(1);
            ct.Increment(1);
            ct.Increment(1);
            ct.Increment(2);
            ct.Increment(2);
            ct.Increment(2);

            return ct;
        }
    }
}