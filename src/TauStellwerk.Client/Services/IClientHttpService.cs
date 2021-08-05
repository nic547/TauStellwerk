// <copyright file="IClientHttpService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Net.Http;
using System.Threading.Tasks;

namespace TauStellwerk.Client.Services
{
    public interface IClientHttpService
    {
        Task<HttpClient> GetHttpClient();
    }
}