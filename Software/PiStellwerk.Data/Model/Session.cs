// <copyright file="Session.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

#nullable enable

using System;
using System.Web;

namespace PiStellwerk.Data
{
    /// <summary>
    /// A user of the PiStellwerk software. Does not necessarily correspond to actual people.
    /// </summary>
    public class Session
    {
        private readonly string? _userAgent;
        private readonly string? _sessionId;
        private string? _userName;

        public string UserName
        {
            get => _userName ?? string.Empty;
            set => _userName = HttpUtility.HtmlEncode(value);
        }

        public string UserAgent
        {
            get => _userAgent ?? string.Empty;
            init => _userAgent = HttpUtility.HtmlEncode(value);
        }

        public DateTime LastContact { get; set; }

        public string SessionId
        {
            get => _sessionId ?? string.Empty;
            init => _sessionId = value;
        }
    }
}
