// <copyright file="SessionService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

#nullable enable

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TauStellwerk.Base.Model;
using TauStellwerk.Util;

namespace TauStellwerk.Services;

/// <summary>
/// Service that keeps track of users.
/// </summary>
public class SessionService : BackgroundService
{
    private const int TimeoutInactive = 60;
    private const int TimeoutDeletion = 3600;

    private readonly ConcurrentDictionary<string, Session> _sessions = new();
    private readonly INowProvider _now;
    private readonly ILogger<SessionService> _logger;

    public SessionService(ILogger<SessionService> logger, INowProvider? now = null)
    {
        _logger = logger;
        _now = now ?? new NowProvider();
    }

    public delegate void SessionTimeoutHandler(Session session);

    public event SessionTimeoutHandler? SessionTimeout;

    public Session CreateSession(string username, string? userAgent, string sessionId)
    {
        var session = new Session
        {
            UserAgent = userAgent ?? string.Empty,
            UserName = username,
            LastContact = _now.GetUtcNow(),
            SessionId = sessionId,
        };
        _sessions.TryAdd(session.SessionId, session);
        _logger.LogDebug($"{session} created new session");
        return session;
    }

    public bool TryUpdateSessionLastContact(string sessionId)
    {
        var session = TryGetSession(sessionId);
        if (session == null)
        {
            _logger.LogWarning($"Update for non-existant session:{sessionId}");
            return false;
        }

        session.LastContact = _now.GetUtcNow();
        return true;
    }

    public void RenameSessionUser(string sessionId, string newUsername)
    {
        var session = TryGetSession(sessionId);
        if (session == null)
        {
            throw new ArgumentException($"No user with user id {sessionId} found. Requested username: {newUsername}");
        }

        _logger.LogDebug($"{session} renamed to {newUsername}");
        session.UserName = newUsername;
    }

    public Session? TryGetSession(string sessionId)
    {
        _sessions.TryGetValue(sessionId, out var value);
        return value;
    }

    /// <summary>
    /// Get a list of all active users.
    /// </summary>
    /// <returns>The list of active users.</returns>
    public IReadOnlyList<Session> GetSessions()
    {
        return _sessions.Values.ToList();
    }

    public void CheckLastContacts()
    {
        foreach (var session in _sessions.Values)
        {
            var idle = (_now.GetUtcNow() - session.LastContact).TotalSeconds;
            switch (idle)
            {
                case > TimeoutInactive when session.IsActive:
                    session.IsActive = false;
                    SessionTimeout?.Invoke(session);
                    _logger.LogDebug($"{session} has been marked as inactive.");
                    break;

                case < TimeoutInactive when !session.IsActive:
                    session.IsActive = true;
                    _logger.LogDebug($"{session} has been reactivated.");
                    break;

                case > TimeoutDeletion:
                    _sessions.TryRemove(session.SessionId, out _);
                    _logger.LogInformation($"{session} has been deleted after {Math.Round(idle)} seconds");
                    break;
            }
        }
    }

    /// <inheritdoc/>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogDebug("SessionService is starting.");

        stoppingToken.Register(() => System.Console.WriteLine("SessionService is stopping."));

        while (!stoppingToken.IsCancellationRequested)
        {
            CheckLastContacts();

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }

        _logger.LogDebug("SessionService background task is stopping.");
    }
}