// <copyright file="EngineControllerCRUDTests.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using PiStellwerk.Base.Model;
using PiStellwerk.Controllers;
using PiStellwerk.Tools.CreateTestDb;

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
            await InsertEngines(99);
            var list = await GetController().GetEngines();
            list.Should().HaveCount(20);
        }

        [Test]
        public async Task ListIsCompletlySorted()
        {
            await InsertEngines(99);
            List<EngineDto> list = new();

            for (var i = 0; i < 5; i++)
            {
                list.AddRange(await GetController().GetEngines(i));
            }

            list.Should().HaveCount(100);
            list.Select(e => e.LastUsed).Should().BeInDescendingOrder();
        }

        [Test]
        public async Task ListCanBeSortedAscending()
        {
            await InsertEngines(99);
            List<EngineDto> list = new();

            for (var i = 0; i < 5; i++)
            {
                list.AddRange(await GetController().GetEngines(i, false, default, false));
            }

            list.Should().HaveCount(100);
            list.Select(e => e.LastUsed).Should().BeInAscendingOrder();
        }

        [Test]
        public async Task ListCanBeSortedDescendingByCreationDate()
        {
            await InsertEngines(99);
            List<EngineDto> list = new();

            for (var i = 0; i < 5; i++)
            {
                list.AddRange(await GetController().GetEngines(i, false, SortEnginesBy.Created, true));
            }

            list.Should().HaveCount(100);
            list.Select(e => e.Created).Should().BeInDescendingOrder();
        }

        [Test]
        public async Task ListCanBeSortedAscendingByCreationDate()
        {
            await InsertEngines(99);
            List<EngineDto> list = new();

            for (var i = 0; i < 5; i++)
            {
                list.AddRange(await GetController().GetEngines(i, false, SortEnginesBy.Created, false));
            }

            list.Should().HaveCount(100);
            list.Select(e => e.Created).Should().BeInAscendingOrder();
        }

        [Test]
        public async Task ListCanBeSortedDescendingByName()
        {
            await InsertEngines(99);
            List<EngineDto> list = new();

            for (var i = 0; i < 5; i++)
            {
                list.AddRange(await GetController().GetEngines(i, false, SortEnginesBy.Name, true));
            }

            list.Should().HaveCount(100);
            list.Select(e => e.Name).Should().BeInDescendingOrder();
        }

        [Test]
        public async Task ListCanBeSortedAscendingByName()
        {
            await InsertEngines(99);
            List<EngineDto> list = new();

            for (var i = 0; i < 5; i++)
            {
                list.AddRange(await GetController().GetEngines(i, false, SortEnginesBy.Name, false));
            }

            list.Should().HaveCount(100);
            list.Select(e => e.Name).Should().BeInAscendingOrder();
        }

        [Test]
        public async Task CanAddEngine()
        {
            var engineToAdd = EngineDtoGenerator.GetEngineDto();

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

        [Test]
        public async Task CanUpdateFunctions()
        {
            var engine = new EngineFullDto
            {
                Functions = new()
                { new(0, "H"), },
            };

            var returnedEngine = (await GetController().UpdateOrAdd(engine)).Value;
            returnedEngine.Functions[0].Name = "Headlights";
            await GetController().UpdateOrAdd(engine);

            var resultEngine = (await GetController().UpdateOrAdd(engine)).Value;
            resultEngine.Functions.Should().HaveCount(1);
            resultEngine.Functions[0].Name.Should().Be("Headlights");
        }

        public async Task InsertEngines(int number)
        {
            var engines = EngineDtoGenerator.GetEngineFullDtos(number);
            foreach (var engine in engines)
            {
                await GetController().UpdateOrAdd(engine);
            }
        }
    }
}
