// <copyright file="User.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace PiStellwerk.Data
{
    /// <summary>
    /// A user of the PiStellwerk software. Does not necessarily correspond to actual people.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the user-agent string of the client the user uses.
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the last heartbeat received by the application from the user.
        /// </summary>
        public DateTime LastContact { get; set; }
    }
}
