// <copyright file="User.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Web;

namespace PiStellwerk.Data
{
    /// <summary>
    /// A user of the PiStellwerk software. Does not necessarily correspond to actual people.
    /// </summary>
    public class User
    {
        private string _name;
        private string _userAgent;

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        /// <param name="name">Username.</param>
        /// <param name="userAgent">User-Agent of the user.</param>
        public User(string name, string userAgent)
        {
            Name = name;
            UserAgent = userAgent;
        }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        public string Name
        {
            get => _name;
            set => _name = HttpUtility.HtmlEncode(value);
        }

        /// <summary>
        /// Gets or sets the user-agent string of the client the user uses.
        /// </summary>
        public string UserAgent
        {
            get => _userAgent;
            set => _userAgent = HttpUtility.HtmlEncode(value);
        }

        /// <summary>
        /// Gets or sets the timestamp of the last heartbeat received by the application from the user.
        /// </summary>
        public DateTime LastContact { get; set; }
    }
}
