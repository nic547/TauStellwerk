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
using System.Web;
using Microsoft.Extensions.Hosting;
using PiStellwerk.Data;

namespace PiStellwerk.Services
{
    /// <summary>
    /// Service that keeps track of users.
    /// </summary>
    public class SessionService : BackgroundService
    {
        private const int _timeoutWarning = 15;
        private const int _timeoutInactive = 60;
        private const int _timeoutDeletion = 3600;

        private static readonly ConcurrentDictionary<string, Session> _sessions = new();

        public delegate void SessionTimeoutHandler(Session session);

        public static event SessionTimeoutHandler? SessionTimeout;

        public static Session CreateSession(string username, string userAgent)
        {
            var session = new Session
            {
                UserAgent = userAgent,
                UserName = username,
                LastContact = DateTime.Now,
            };
            _sessions.TryAdd(session.SessionId, session);
            return session;
        }

        public static void UpdateSessionLastContact(string sessionId)
        {
            var session = TryGetSession(sessionId);
            if (session == null)
            {
                throw new ArgumentException($"Reported contact with sessionId {sessionId}, but no such session was found.");
            }

            session.LastContact = DateTime.Now;
        }

        public static void RenameSessionUser(string sessionId, string newUsername)
        {
            var session = TryGetSession(sessionId);
            if (session == null)
            {
                throw new ArgumentException($"No user with user id {sessionId} found. Requested username: {newUsername}");
            }

            Console.WriteLine($"Renaming {HttpUtility.HtmlDecode(session.UserName)} to {newUsername}");
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

        /// <inheritdoc/>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("SessionService is starting.");

            stoppingToken.Register(() => Console.WriteLine("SessionService is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var session in _sessions.Values)
                {
                    var idle = (DateTime.Now - session.LastContact).TotalSeconds;
                    switch (idle)
                    {
                        case < _timeoutInactive and > _timeoutWarning:
                            Console.WriteLine($"{session.ShortSessionId}:{HttpUtility.HtmlDecode(session.UserName)} has been inactive for {Math.Round(idle)} seconds");
                            break;
                        case > _timeoutInactive when session.IsActive:
                            session.IsActive = false;
                            SessionTimeout?.Invoke(session);
                            Console.WriteLine($"{session.ShortSessionId}:{HttpUtility.HtmlDecode(session.UserName)} has been marked as inactive.");
                            break;
                        case > _timeoutDeletion:
                            _sessions.TryRemove(session.SessionId, out _);
                            Console.WriteLine($"{session.ShortSessionId}:{HttpUtility.HtmlDecode(session.UserName)} has been deleted after {Math.Round(idle)} seconds");
                            break;
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }

            Console.WriteLine("SessionService background task is stopping.");
        }
    }
}
