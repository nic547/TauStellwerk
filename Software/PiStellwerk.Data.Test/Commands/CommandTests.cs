// <copyright file="CommandTests.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using NUnit.Framework;
using PiStellwerk.Data.Commands;

namespace PiStellwerk.Data.Test.Commands
{
    /// <summary>
    /// Tests related to <see cref="CommandBase"/> and <see cref="Command"/>.
    /// </summary>
    public class CommandTests
    {
        /// <summary>
        /// Test that two Commands that only differ in their Type property do not equal each other.
        /// </summary>
        [Test]
        public void DifferentDataDoesNotEqualTest()
        {
            var command = new Command(256, 128, 120, CommandType.Speed);
            var command2 = new Command(256, 128, 130, CommandType.Speed);

            Assert.AreNotEqual(command, command2);
        }

        /// <summary>
        /// Test that two Commands that only differ in their data property do not equal each other.
        /// </summary>
        [Test]
        public void DifferentTypeDoesNotEqualTest()
        {
            var command = new Command(256, 128, 1, CommandType.FunctionToggleOff);
            var command2 = new Command(256, 128, 1, CommandType.FunctionToggleOn);

            Assert.AreNotEqual(command, command2);

            Assert.AreNotEqual(command, command2);
        }

        /// <summary>
        /// Test that two Commands that only differ in their Address property do not equal each other.
        /// </summary>
        [Test]
        public void DifferentAddressDoesNotEqualTest()
        {
            var command = new Command(256, 128, 120, CommandType.Speed);
            var command2 = new Command(255, 128, 120, CommandType.Speed);

            Assert.AreNotEqual(command, command2);

            Assert.AreNotEqual(command, command2);
        }

        /// <summary>
        /// Test that two Commands that only differ in their speedSteps property do not equal each other.
        /// </summary>
        [Test]
        public void DifferentSpeedStepsDoesNotEqualTest()
        {
            var command = new Command(256, 128, 120, CommandType.Speed);
            var command2 = new Command(256, 28, 120, CommandType.Speed);

            Assert.AreNotEqual(command, command2);

            Assert.AreNotEqual(command, command2);
        }

        /// <summary>
        /// Test that a speed command "IsReplaceableBy" a second speed command for the same address.
        /// </summary>
        [Test]
        public void SecondSpeedReplacesFirst()
        {
            var command = new Command(1337, 128, 100, CommandType.Speed);
            var command2 = new Command(1337, 128, 120, CommandType.Speed);

            Assert.AreNotEqual(command, command2);
            Assert.True(command.IsReplaceableBy(command2));
        }

        /// <summary>
        /// Test that two different SpeedCommands for different Addresses are not replaceable by each-other.
        /// </summary>
        [Test]
        public void SpeedCommandDifferentAddressNotReplaceable()
        {
            var command = new Command(1337, 128, 100, CommandType.Speed);
            var command2 = new Command(420, 128, 120, CommandType.Speed);

            Assert.False(command.IsReplaceableBy(command2));
            Assert.False(command2.IsReplaceableBy(command));
        }
    }
}
