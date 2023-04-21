// <copyright file="SessionServiceTests.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using FluentAssertions;
using FluentResults.Extensions.FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using TauStellwerk.Server.Services;

namespace TauStellwerk.Test.Services;

public class SessionServiceTests
{
    [Test]
    public void SessionIsCreatedOnConnecting()
    {
        var sessionService = new SessionService(GetLoggerMock().Object);
        sessionService.HandleConnected("Foo", "Alice");

        var session = sessionService.TryGetSession("Foo");

        session.Should().BeSuccess();
        session.Value.UserName.Should().Be("Alice");
        session.Value.ConnectionId.Should().Be("Foo");
    }

    [Test]
    public void SessionIsRemovedOnDisconnecting()
    {
        var sessionService = new SessionService(GetLoggerMock().Object);
        sessionService.HandleConnected("Foo", "Alice");
        sessionService.HandleDisconnected("Foo", null);

        var session = sessionService.TryGetSession("Foo");

        session.Should().BeFailure();
    }

    [Test]
    public void CreatingSessionIsLogged()
    {
        var loggerMock = GetLoggerMock();
        SessionService sessionService = new(loggerMock.Object);

        sessionService.HandleConnected("Foo", "Bob");

        VerifyLoggerWasUsed(loggerMock, "Bob");
    }

    [Test]
    public void DisconnectingWithExceptionIsLogged()
    {
        var loggerMock = GetLoggerMock();
        SessionService sessionService = new(loggerMock.Object);

        sessionService.HandleConnected("Foo", "Bob");
        sessionService.HandleDisconnected("Foo", new InvalidOperationException("Test"));

        VerifyLoggerWasUsed(loggerMock, "InvalidOperationException");
    }

    private static Mock<ILogger<SessionService>> GetLoggerMock()
    {
        return new Mock<ILogger<SessionService>>();
    }

    private static void VerifyLoggerWasUsed(Mock<ILogger<SessionService>> mock, string? shouldContain = null)
    {
        mock.Verify(
            x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains(shouldContain ?? string.Empty)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }
}
