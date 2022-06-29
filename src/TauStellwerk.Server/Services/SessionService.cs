// <copyright file="SessionService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

#nullable enable

using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using TauStellwerk.Base.Model;
using TauStellwerk.Util;

namespace TauStellwerk.Server.Services;

/// <summary>
/// Service that keeps track of users.
/// </summary>
public class SessionService
{
    private readonly ConcurrentDictionary<string, Session> _sessions = new();
    private readonly ILogger<SessionService> _logger;

    public SessionService(ILogger<SessionService> logger)
    {
        _logger = logger;
    }

    public delegate void SessionTimeoutHandler(Session session);

    public delegate void NoUsersRemainingHandler();

    public event SessionTimeoutHandler? SessionTimeout;

    public event NoUsersRemainingHandler? NoUsersRemaining;

    public Session? TryGetSession(string connectionId)
    {
        _sessions.TryGetValue(connectionId, out var value);
        return value;
    }

    public void HandleConnected(string connectionId, string username)
    {
        var session = new Session(connectionId, username);
        _sessions.TryAdd(connectionId, session);
        _logger.LogInformation("{session} connected.", session);
    }

    public void HandleDisconnected(string connectionId, Exception? exception)
    {
        _sessions.TryRemove(connectionId, out var connection);
        if (connection is null)
        {
            _logger.LogError("Connection:'{connection}' has disconnected, but SessionService had no active session for it.", connectionId.Left(8));
            return;
        }

        if (exception is not null)
        {
            _logger.LogWarning("{connection} disconnected with a {exception}", connection, exception);
        }
        else
        {
            _logger.LogInformation("{connection} disconnected", connection);
        }

        SessionTimeout?.Invoke(connection);

        if (_sessions.IsEmpty)
        {
            NoUsersRemaining?.Invoke();
        }
    }

    public void RenameSessionUser(string sessionId, string newUsername)
    {
        var session = TryGetSession(sessionId);
        if (session == null)
        {
            throw new ArgumentException($"No user with user id {sessionId} found. Requested username: {newUsername}");
        }

        _logger.LogInformation("{session} has been renamed to {username}", session, newUsername);
        session.UserName = newUsername;
    }
}