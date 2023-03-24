// <copyright file="MomentaryFunctionHandlerTests.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using FluentAssertions;
using NUnit.Framework;
using TauStellwerk.Base;
using TauStellwerk.Server.Database.Model;
using TauStellwerk.Server.Services.EngineService;
using TauStellwerk.Util;
using TauStellwerk.Util.Timer;

namespace TauStellwerk.Test.Services.EngineControlService;

public class MomentaryFunctionHandlerTests
{
    [Test]
    public async Task SingleFunction()
    {
        DateTime testNow = new(2020, 12, 24, 12, 00, 00, 0, DateTimeKind.Utc);
        FakeDateTimeProvider dateTimeProvider = new() { Value = testNow };

        LoggingCommandStation cs = new();
        FakeTimer timer = new();
        MomentaryFunctionHandler handler = new(cs, 100, timer, dateTimeProvider);

        await handler.Add(new Engine(), new DccFunction(1, "Horn", 200));

        timer.Elapse(testNow.AddMilliseconds(100));
        await Task.Delay(10); // The elapsed event handler runs on the thread pool, so we need to wait a bit for it to have completed.
        timer.Elapse(testNow.AddMilliseconds(200));
        await Task.Delay(10);

        cs.EngineFunctionCalls.Should().Contain((1, State.Off));
    }

    [Test]
    public async Task MultipleFunctionsSameDuration()
    {
        DateTime testNow = new(2020, 12, 24, 12, 00, 00, 0, DateTimeKind.Utc);
        FakeDateTimeProvider dateTimeProvider = new() { Value = testNow };

        LoggingCommandStation cs = new();
        FakeTimer timer = new();
        MomentaryFunctionHandler handler = new(cs, 100, timer, dateTimeProvider);

        await handler.Add(new Engine(), new DccFunction(1, "Horn", 200));
        await handler.Add(new Engine(), new DccFunction(2, "Whistle", 200));

        timer.Elapse(testNow.AddMilliseconds(100));
        await Task.Delay(10); // The elapsed event handler runs on the thread pool, so we need to wait a bit for it to have completed.
        timer.Elapse(testNow.AddMilliseconds(200));
        await Task.Delay(10);

        cs.EngineFunctionCalls.Should().HaveCount(2);
    }
}