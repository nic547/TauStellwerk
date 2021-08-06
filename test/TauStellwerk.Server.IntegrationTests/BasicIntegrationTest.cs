// <copyright file="BasicIntegrationTest.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            var enginesToInsert = Tools.CreateTestDb.EngineDtoGenerator.GetEngineFullDtos(100);
            foreach (var engine in enginesToInsert)
            {
                await engineService.AddOrUpdateEngine(engine);
            }

            List<EngineDto> engines = new();

            foreach (var i in Enumerable.Range(0, 6))
            {
                engines.AddRange(await engineService.GetEngines(i, SortEnginesBy.Name, true));
            }

            engines.Should().HaveCount(100);
        }

        public void Dispose()
        {
            File.Delete("StwDatabase.db");
            _factory.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}