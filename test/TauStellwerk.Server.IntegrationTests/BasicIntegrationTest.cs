// <copyright file="BasicIntegrationTest.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using NUnit.Framework;
using TauStellwerk.Base.Model;
using TauStellwerk.Client.Services;

namespace TauStellwerk.Server.IntegrationTests
{
    public class BasicIntegrationTest : IDisposable
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public BasicIntegrationTest()
        {
            _factory = new CustomWebApplicationFactory<Startup>();
        }

        [Test]
        [Category("long-running")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType()
        {
            var client = _factory.CreateClient();
            var mock = new Mock<IHttpClientService>(MockBehavior.Strict);
            mock.Setup(m => m.GetHttpClient()).ReturnsAsync(client);
            var engineService = new EngineService(mock.Object);

            // TODO: Add Add-Functionality to TauStellwerk.Client and use it in tests
            var engines = await engineService.GetEngines(1, SortEnginesBy.Name, true);

            engines.Should().BeEmpty();
        }

        public void Dispose()
        {
            _factory.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}