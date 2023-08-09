// <copyright file="SessionServiceTests.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using FluentAssertions;
using FluentResults.Extensions.FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using TauStellwerk.Server.Services;

namespace TauStellwerk.Test.Services;

public class SessionServiceTests
{
    [Test]
    public void SessionIsCreatedOnConnecting()
    {
        var sessionService = new SessionService(GetTestLogger());
        sessionService.HandleConnected("Foo", "Alice");

        var session = sessionService.TryGetSession("Foo");

        session.Should().BeSuccess();
        session.Value.UserName.Should().Be("Alice");
        session.Value.ConnectionId.Should().Be("Foo");
    }

    [Test]
    public void SessionIsRemovedOnDisconnecting()
    {
        var sessionService = new SessionService(GetTestLogger());
        sessionService.HandleConnected("Foo", "Alice");
        sessionService.HandleDisconnected("Foo", null);

        var session = sessionService.TryGetSession("Foo");

        session.Should().BeFailure();
    }

    [Test]
    public void CreatingSessionIsLogged()
    {
        var loggerMock = GetTestLogger();
        SessionService sessionService = new(loggerMock);

        sessionService.HandleConnected("Foo", "Bob");

        VerifyLoggerWasUsed(loggerMock, "Bob");
    }

    [Test]
    public void DisconnectingWithExceptionIsLogged()
    {
        var loggerMock = GetTestLogger();
        SessionService sessionService = new(loggerMock);

        sessionService.HandleConnected("Foo", "Bob");
        sessionService.HandleDisconnected("Foo", new InvalidOperationException("Test"));

        VerifyLoggerWasUsed(loggerMock, "InvalidOperationException");
    }

    private static MockLogger<SessionService> GetTestLogger()
    {
        return Substitute.For<MockLogger<SessionService>>();
    }

    private static void VerifyLoggerWasUsed(MockLogger<SessionService> mock, string shouldContain)
    {
        mock.Received().Log(
            Arg.Any<LogLevel>(),
            Arg.Is<object>(o => o!.ToString()!.Contains(shouldContain)),
            Arg.Any<Exception>());
    }

#nullable disable

    // ReSharper disable once MemberCanBePrivate.Global
    public abstract class MockLogger<T> : ILogger<T>
    {
        void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            => Log(logLevel, formatter(state, exception), exception);

        public abstract void Log(LogLevel logLevel, object state, Exception exception = null);

        public virtual bool IsEnabled(LogLevel logLevel) => true;

        public abstract IDisposable BeginScope<TState>(TState state);
    }
}
