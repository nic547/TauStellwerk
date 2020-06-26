// <copyright file="UserService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using PiStellwerk.Data;

namespace PiStellwerk.BackgroundServices
{
    /// <summary>
    /// Service that keeps track of users.
    /// </summary>
    public class UserService : BackgroundService
    {
        private const int _timeoutWarning = 10;
        private const int _timeoutRemoval = 30;
        private static readonly List<User> _users = new List<User>();

        /// <summary>
        /// Updates the last heartbeat of a user, also adds new users to the user list.
        /// </summary>
        /// <param name="user">The user.</param>
        public static void UpdateUser(User user)
        {
            user.LastContact = DateTime.Now;
            User foundUser = _users.Find(x => x.Name == user.Name && x.UserAgent == user.UserAgent);
            if (foundUser == null)
            {
                _users.Add(user);
                Console.WriteLine($"New user \"{user.Name}\" added");
            }
            else
            {
                foundUser.LastContact = user.LastContact;
            }
        }

        /// <summary>
        /// Handles a changed username.
        /// </summary>
        /// <param name="oldUser">The old username.</param>
        /// <param name="newUser">The new username.</param>
        public static void RenameUser(User oldUser, User newUser)
        {
            var userToRename = _users.SingleOrDefault(x => x.Name == oldUser.Name && x.UserAgent == oldUser.UserAgent);

            if (userToRename == null)
            {
                Console.WriteLine($"User {newUser.Name} tried to rename himself, but the prior username was not found");
                UpdateUser(newUser);
            }
            else
            {
                userToRename.Name = newUser.Name;
                Console.WriteLine($"User \"{oldUser.Name}\" has been renamed to \"{newUser.Name}\" ");
                userToRename.LastContact = DateTime.Now;
            }
        }

        /// <summary>
        /// Get a list of all active users.
        /// </summary>
        /// <returns>The list of active users.</returns>
        public static IReadOnlyList<User> GetUsers()
        {
            return _users;
        }

        /// <inheritdoc/>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("UserService is starting.");

            stoppingToken.Register(() => Console.WriteLine("UserService is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var user in _users.Reverse<User>())
                {
                    var idle = (DateTime.Now - user.LastContact).TotalSeconds;
                    if (idle < _timeoutRemoval && idle > _timeoutWarning)
                    {
                        Console.WriteLine($"User {user.Name} has been inactive for {Math.Round(idle)} seconds");
                    }

                    if (idle > _timeoutRemoval)
                    {
                        _users.Remove(user);
                        Console.WriteLine($"User {user.Name} has been removed from the active user list");
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }

            Console.WriteLine("UserService background task is stopping.");
        }
    }
}
