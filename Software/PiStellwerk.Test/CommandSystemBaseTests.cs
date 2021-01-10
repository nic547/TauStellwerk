// <copyright file="CommandSystemBaseTests.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using NUnit.Framework;
using PiStellwerk.Commands;
using PiStellwerk.Data;
using PiStellwerk.Data.Commands;

namespace PiStellwerk.Test
{
    /// <summary>
    /// Contains tests related to <see cref="CommandSystemBase"/>.
    /// </summary>
    public class CommandSystemBaseTests
    {
        private CommandSystemBase _commandSystem;

        /// <summary>
        /// Setup for the test.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _commandSystem = new CommandSystemBase();
        }

        /// <summary>
        /// Test that a command that is sent is returned.
        /// </summary>
        [Test]
        public void SingleCommandTest()
        {
            var jsonCommand = new JsonCommand
            {
                Data = 60,
                Type = CommandType.Speed,
            };
            var engine = GetTestEngine1();
            var command = jsonCommand.ToCommand(engine.Address, engine.SpeedSteps);

            _commandSystem.HandleCommand(jsonCommand, engine);
            Assert.AreEqual(command, _commandSystem.GetNextCommand());
        }

        /// <summary>
        /// Ensure that a replaceable command gets replaced.
        /// </summary>
        [Test]
        public void ReplaceableCommandGetsReplaced()
        {
            var engine = GetTestEngine1();

            var jsonCommand1 = new JsonCommand
            {
                Data = 60,
                Type = CommandType.Speed,
            };

            var jsonCommand2 = new JsonCommand
            {
                Data = 80,
                Type = CommandType.Speed,
            };

            var command2 = jsonCommand2.ToCommand(engine.Address, engine.SpeedSteps);

            _commandSystem.HandleCommand(jsonCommand1, engine);
            _commandSystem.HandleCommand(jsonCommand2, engine);

            Assert.AreEqual(command2, _commandSystem.GetNextCommand());
            Assert.Null(_commandSystem.GetNextCommand());
        }

        private static Engine GetTestEngine1()
        {
            return new()
            {
                Address = 492,
                Name = "Engine1",
                Id = 1,
            };
        }
    }
}