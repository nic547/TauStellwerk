// <copyright file="JsonCommandTests.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Text.Json;
using NUnit.Framework;
using PiStellwerk.Data.Commands;

namespace PiStellwerk.Data.Test.Commands
{
    /// <summary>
    /// Tests related to <see cref="CommandBase"/> and <see cref="JsonCommand"/>.
    /// </summary>
    public class JsonCommandTests
    {
        /// <summary>
        /// Test that serializing and deserializing an object leads to an equal object.
        /// </summary>
        [Test]
        public void CommandSerializationTests()
        {
            var command = new JsonCommand
            {
                Type = CommandType.FunctionOn,
                Data = 128,
            };

            var json = JsonSerializer.Serialize(command);
            var command2 = JsonSerializer.Deserialize<JsonCommand>(json);

            Assert.AreEqual(command, command2);
        }

        /// <summary>
        /// Test that a JsonCommand can be turned into a command.
        /// </summary>
        [Test]
        public void JsonCommandToCommandTest()
        {
            var jsonCommand = new JsonCommand
            {
                Type = CommandType.Speed,
                Data = 38,
            };

            var expectedResult = new Command(138, 128, 38, CommandType.Speed);

            var result = jsonCommand.ToCommand(138, 128);

            Assert.AreEqual(expectedResult, result);
        }

        /// <summary>
        /// Test that two JsonCommands that only differ in their data property do not equal each other.
        /// </summary>
        [Test]
        public void DifferentDataDoesNotEqualTest()
        {
            var command = new JsonCommand
            {
                Type = CommandType.Speed,
                Data = 120,
            };

            var command2 = new JsonCommand
            {
                Type = CommandType.Speed,
                Data = 140,
            };

            Assert.AreNotEqual(command, command2);
        }

        /// <summary>
        /// Test that two JsonCommands that only differ in their data property do not equal each other.
        /// </summary>
        [Test]
        public void DifferentTypeDoesNotEqualTest()
        {
            var command = new JsonCommand
            {
                Type = CommandType.FunctionToggleOff,
                Data = 0,
            };

            var command2 = new JsonCommand
            {
                Type = CommandType.FunctionToggleOn,
                Data = 0,
            };

            Assert.AreNotEqual(command, command2);
        }
    }
}
