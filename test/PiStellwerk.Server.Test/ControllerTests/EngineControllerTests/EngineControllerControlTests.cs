// <copyright file="EngineControllerControlTests.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace PiStellwerk.Test.ControllerTests.EngineControllerTests
{
    public class EngineControllerControlTests : EngineControllerTestsBase
    {
        [Test]
        public async Task CannotDeleteNonExistentEngine()
        {
            var result = await GetController().Delete(int.MaxValue);
            result.Should().BeAssignableTo<NotFoundResult>();
        }

        [Test]
        public async Task SpeedFailureReturnsBadRequest()
        {
            var result = await GetController(GetMock(false)).SetEngineSpeed(SessionId, EngineId, 100, null);
            result.Should().BeAssignableTo<BadRequestResult>();
        }

        [Test]
        public async Task SpeedInvalidSessionReturns403()
        {
            var result = await GetController().SetEngineSpeed("InvalidSession", 1, 120, false);
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(403);
        }

        [Test]
        public async Task SpeedSuccessCase()
        {
            var result = await GetController(GetMock(true)).SetEngineSpeed(SessionId, 1, 80, false);
            result.Should().BeAssignableTo<OkResult>();
        }

        [Test]
        public async Task FunctionInvalidSessionReturns403()
        {
            var result = await GetController().EngineFunction("invalidSession", 1, 1, "on");
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(403);
        }

        [Test]
        public async Task FunctionFailureReturnsBadRequest()
        {
            var result = await GetController(GetMock(false)).EngineFunction(SessionId, 1, 1, "on");
            result.Should().BeAssignableTo<BadRequestResult>();
        }

        [Test]
        public async Task FunctionSuccessCase()
        {
            var result = await GetController(GetMock(true)).EngineFunction(SessionId, 1, 1, "on");
            result.Should().BeAssignableTo<OkResult>();
        }

        [Test]
        public async Task AcquireInvalidSessionReturns403()
        {
            var result = await GetController().AcquireEngine(1, "InvalidSession");
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(403);
        }

        [Test]
        public async Task AcquireNonExistentEngineReturnsNotFound()
        {
            var result = await GetController().AcquireEngine(int.MaxValue, SessionId);
            result.Should().BeAssignableTo<NotFoundResult>();
        }

        [Test]
        public async Task AcquireFailureReturnsLocked()
        {
            var result = await GetController(GetMock(false)).AcquireEngine(1, SessionId);
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Which.StatusCode.Should().Be(423);
        }

        [Test]
        public async Task AcquireSuccessCase()
        {
            var result = await GetController(GetMock(true)).AcquireEngine(1, SessionId);
            result.Should().BeAssignableTo<OkResult>();
        }

        [Test]
        public async Task ReleaseInvalidSessionReturns403()
        {
            var result = await GetController().ReleaseEngine(1, "InvalidSession");
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Subject.StatusCode.Should().Be(403);
        }

        [Test]
        public async Task ReleaseFailureReturnsBadRequest()
        {
            var result = await GetController(GetMock(false)).ReleaseEngine(1, SessionId);
            result.Should().BeAssignableTo<BadRequestResult>();
        }

        [Test]
        public async Task ReleaseSuccessCase()
        {
            var result = await GetController(GetMock(true)).ReleaseEngine(1, SessionId);
            result.Should().BeAssignableTo<OkResult>();
        }
    }
}