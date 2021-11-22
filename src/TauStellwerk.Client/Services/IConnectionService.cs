﻿// <copyright file="IHttpClientService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace TauStellwerk.Client.Services;

public interface IConnectionService
{
    Task<HubConnection> GetHubConnection();
}