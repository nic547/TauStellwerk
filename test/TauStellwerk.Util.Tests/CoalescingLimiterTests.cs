// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using FluentAssertions;
using NUnit.Framework;
using TauStellwerk.Util.RateLimiter;
using TauStellwerk.Util.Timer;

namespace TauStellwerk.Util.Tests;

public class CoalescingLimiterTests
{
    [Test]
    public async Task FunctionIsExecuted()
    {
        var helper = new LimiterTestHelper<int>();
        var limiter = new CoalescingLimiter<int>(helper.Method, 100);

        await limiter.Execute(460);
        helper.MethodCalls.Should().HaveCount(1);
        helper.MethodCalls.First().Value.Should().Be(460);
    }

    [Test]
    public async Task SubsequentExecutionGetsDelayed()
    {
        var helper = new LimiterTestHelper<int>();
        var limiter = new CoalescingLimiter<int>(helper.Method, 100);

        _ = limiter.Execute(460);
        await limiter.Execute(620);

        var timeBetweenCalls = helper.MethodCalls[1].DateTime - helper.MethodCalls[0].DateTime;
        helper.MethodCalls.Should().HaveCount(2);
        timeBetweenCalls.Should().BeGreaterOrEqualTo(TimeSpan.FromMilliseconds(95));
    }

    [Test]
    public async Task SubsequentExecutionsCanCoalesce()
    {
        var helper = new LimiterTestHelper<int>();
        var fakeTimer = new FakeTimer();
        var limiter = new CoalescingLimiter<int>(helper.Method, 100);

        _ = limiter.Execute(460);
        fakeTimer.Elapse();
        _ = limiter.Execute(514);
        var lastTask = limiter.Execute(620);

        fakeTimer.Elapse();
        await lastTask;

        helper.MethodCalls.Should().HaveCount(2);
        helper.MethodCalls[0].Value.Should().Be(460);
        helper.MethodCalls[1].Value.Should().Be(620);
    }

    public class LimiterTestHelper<T>
    {
        public List<(T Value, DateTime DateTime)> MethodCalls { get; } = [];

        public Task Method(T param)
        {
            MethodCalls.Add((param, DateTime.UtcNow));
            return Task.CompletedTask;
        }
    }
}
