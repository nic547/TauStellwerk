// <copyright file="CoalescingLimiterTests.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace TauStellwerk.Util.Tests
{
    public class CoalescingLimiterTests
    {
        [Test]
        public async Task FunctionIsExecuted()
        {
            var helper = new LimiterTestHelper<int>();
            var limiter = new CoalescingLimiter<int>(x => helper.Method(x), 100);

            await limiter.Execute(460);
            helper.MethodCalls.Should().HaveCount(1);
            helper.MethodCalls.First().Value.Should().Be(460);
        }

        [Test]
        public async Task SubsequentExecutionGetsDelayed()
        {
            var helper = new LimiterTestHelper<int>();
            var limiter = new CoalescingLimiter<int>(x => helper.Method(x), 100);

            _ = limiter.Execute(460);
            await limiter.Execute(620);

            var timeBetweenCalls = helper.MethodCalls[1].DateTime - helper.MethodCalls[0].DateTime;
            helper.MethodCalls.Should().HaveCount(2);
            timeBetweenCalls.Should().BeGreaterOrEqualTo(TimeSpan.FromMilliseconds(100));
        }

        [Test]
        public async Task SubsequentExecutionsCanCoalesce()
        {
            var helper = new LimiterTestHelper<int>();
            var limiter = new CoalescingLimiter<int>(x => helper.Method(x), 100);

            _ = limiter.Execute(460);
            _ = limiter.Execute(514);
            await limiter.Execute(620);

            helper.MethodCalls.Should().HaveCount(2);
            helper.MethodCalls[1].Value.Should().Be(620);
        }

        public class LimiterTestHelper<T>
        {
            public List<(T Value, DateTime DateTime)> MethodCalls { get; } = new();

            public Task Method(T param)
            {
                MethodCalls.Add((param, DateTime.Now));
                return Task.CompletedTask;
            }
        }
    }
}
