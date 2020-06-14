using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using PiStellwerk.Data;

namespace PiStellwerk.BackgroundServices
{
    public class UserService : BackgroundService
    {
        private static List<User> _users = new List<User>();

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("UserService is starting.");

            stoppingToken.Register(() => Console.WriteLine("UserService is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var user in _users.Reverse<User>())
                {
                    var idle = (DateTime.Now - user.LastContact).TotalSeconds;
                    if (60 > idle && idle > 30)
                    {
                        Console.WriteLine($"User {user.Name} has been inactive for {Math.Round(idle)} seconds");
                    }
                    if (60 < idle)
                    {
                        _users.Remove(user);
                        Console.WriteLine($"User {user.Name} has been removed from the active user list");
                    }
                }


                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }

            Console.WriteLine("UserService background task is stopping.");
        }

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

        public static void RenameUser(User oldUser, User newUser)
        {
            var userToRename = _users.Find(x => x.Name == oldUser.Name && x.UserAgent == oldUser.UserAgent);
            userToRename.Name = newUser.Name;
            Console.WriteLine($"User \"{oldUser.Name}\" has been renamed to \"{newUser.Name}\" ");
            userToRename.LastContact = DateTime.Now;
            
        }

        public static IReadOnlyList<User> GetUsers()
        {
            return _users;
        }
    }
}
