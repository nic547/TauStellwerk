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
using PiStellwerk.Data;
using PiStellwerk.Util;

namespace PiStellwerk.Services
{
    /// <summary>
    /// Service that keeps track of users.
    /// </summary>
    public class SessionService : BackgroundService
    {
        private const int _timeoutInactive = 60;
        private const int _timeoutDeletion = 3600;

        private static readonly ConcurrentDictionary<string, Session> _sessions = new();

        public delegate void SessionTimeoutHandler(Session session);

        public static event SessionTimeoutHandler? SessionTimeout;

        public static Session CreateSession(string username, string? userAgent)
        {
            var session = new Session
            {
                UserAgent = userAgent ?? string.Empty,
                UserName = username,
                LastContact = DateTime.Now,
            };
            _sessions.TryAdd(session.SessionId, session);
            ConsoleService.PrintMessage($"{session} created new session");
            return session;
        }

        public static bool TryUpdateSessionLastContact(string sessionId)
        {
            var session = TryGetSession(sessionId);
            if (session == null)
            {
                Console.WriteLine($"Update for non-existant session:{sessionId}");
                return false;
            }

            session.LastContact = DateTime.Now;
            return true;
        }

        public static void RenameSessionUser(string sessionId, string newUsername)
        {
            var session = TryGetSession(sessionId);
            if (session == null)
            {
                throw new ArgumentException($"No user with user id {sessionId} found. Requested username: {newUsername}");
            }

            ConsoleService.PrintMessage($"{session} renamed to {newUsername}");
            session.UserName = newUsername;
        }

        public static Session? TryGetSession(string sessionId)
        {
            _sessions.TryGetValue(sessionId, out var value);
            return value;
        }

        /// <summary>
        /// Get a list of all active users.
        /// </summary>
        /// <returns>The list of active users.</returns>
        public static IReadOnlyList<Session> GetSessions()
        {
            return _sessions.Values.ToList();
        }

        public static void CleanSessions()
        {
            // TODO: Turn into non-static class
            _sessions.Clear();
        }

        /// <inheritdoc/>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            ConsoleService.PrintMessage("SessionService is starting.");

            stoppingToken.Register(() => Console.WriteLine("SessionService is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var session in _sessions.Values)
                {
                    var idle = (DateTime.Now - session.LastContact).TotalSeconds;
                    switch (idle)
                    {
                        case > _timeoutInactive when session.IsActive:
                            session.IsActive = false;
                            SessionTimeout?.Invoke(session);
                            ConsoleService.PrintMessage($"{session} has been marked as inactive.");
                            break;
                        case > _timeoutDeletion:
                            _sessions.TryRemove(session.SessionId, out _);
                            ConsoleService.PrintMessage($"{session} has been deleted after {Math.Round(idle)} seconds");
                            break;
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }

            ConsoleService.PrintMessage("SessionService background task is stopping.");
        }
    }
}
