// <copyright file="EngineControllerCRUDTests.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using PiStellwerk.Base.Model;
using PiStellwerk.Controllers;

namespace PiStellwerk.Test.ControllerTests.EngineControllerTests
{
    /// <summary>
    /// Tests related to <see cref="EngineController"/>.
    /// </summary>
    public class EngineControllerCrudTests : EngineControllerTestsBase
    {
        [Test]
        public async Task CanLoadList()
        {
            var list = await GetController().GetEngines();
            list.Should().NotBeEmpty();
        }

        [Test]
        public async Task CanAddEngine()
        {
            var engineToAdd = new EngineFullDto
            {
                Address = 392,
                Name = "Re 620 088-5 (xrail)",
                TopSpeed = 140,
                Tags = new List<string>
                {
                    "Freight",
                    "SLM",
                    "Märklin",
                },
            };

            var returnedEngine = (await GetController().UpdateOrAdd(engineToAdd)).Value;
            var loadedEngine = await GetController().GetEngine(returnedEngine.Id);
            var list = await GetController().GetEngines();

            engineToAdd.Id = returnedEngine.Id;
            engineToAdd.Should().BeEquivalentTo(returnedEngine);
            returnedEngine.Should().BeEquivalentTo(loadedEngine);
            list.Should().HaveCount(2);
        }

        [Test]
        public async Task CannotAddEngineWithId()
        {
            var engine = new EngineFullDto
            {
                Id = int.MaxValue,
            };

            var result = await GetController().UpdateOrAdd(engine);
            result.Result.Should().BeAssignableTo<NotFoundResult>();
        }

        [Test]
        public async Task CanRemoveTag()
        {
            var engine = new EngineFullDto
            {
                Id = 0,
                Tags = new List<string> { "TEST", "TEST2" },
            };

            var result = await GetController().UpdateOrAdd(engine);
            result.Value.Tags.Clear();
            await GetController().UpdateOrAdd(result.Value);

            var finalEngine = await GetController().GetEngine(result.Value.Id);

            finalEngine.Functions.Should().BeEmpty();
        }

        [Test]
        public async Task SameTagIsOnlySavedOnce()
        {
            var engine1 = new EngineFullDto()
            {
                Tags = new List<string> { "TestTag", "nic547" },
            };

            var engine2 = new EngineFullDto()
            {
                Tags = new List<string> { "TestTag", "nic547" },
            };

            await GetController().UpdateOrAdd(engine1);
            await GetController().UpdateOrAdd(engine2);

            var context = GetContext();

            var dbTags = await context.Tags.ToListAsync();
            dbTags.Should().HaveCount(2);
        }

        [Test]
        public async Task CanDeleteEngine()
        {
            var result = await GetController().Delete(1);
            var list = await GetController().GetEngines();

            result.Should().BeAssignableTo<OkResult>();
            list.Should().BeEmpty();
        }
    }
}
