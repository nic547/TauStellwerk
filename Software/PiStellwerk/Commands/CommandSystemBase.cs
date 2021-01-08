// <copyright file="CommandSystemBase.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Concurrent;
using System.Threading;
using JetBrains.Annotations;
using PiStellwerk.Data;
using PiStellwerk.Data.Commands;

namespace PiStellwerk.Commands
{
    /// <summary>
    /// A generic CommandSystem. Queue and dequeue commands.
    /// </summary>
    public class CommandSystemBase : ICommandSystem
    {
        /*  TODO: Don't use ConcurrentQueue to be able to remove commands.
            A command can be negated by another command, for example when toggling a function on and off quickly.
            We don't need to send that, but the queue doesn't allow the removal of items "in the middle".
        */
        private readonly ConcurrentQueue<QueuedCommand> _commandQueue = new();

        /// <summary>
        /// Get the next command that should be sent to the command station.
        /// </summary>
        /// <returns><see cref="Command"/>.</returns>
        [CanBeNull]
        public Command GetNextCommand()
        {
            if (!_commandQueue.TryDequeue(out var command))
            {
                return null;
            }

            command.Mutex.WaitOne();
            return command.Command;
        }

        /// <inheritdoc/>
        public void HandleCommand(JsonCommand command, Engine engine)
        {
            AddCommand(command.ToCommand(engine.Address, engine.SpeedSteps));
        }

        /// <summary>
        /// Add a command to the queue.
        /// </summary>
        /// <param name="commandToAdd">The <see cref="Command"/>to add.</param>
        private void AddCommand(Command commandToAdd)
         {
            foreach (var queueCommand in _commandQueue)
            {
                if (commandToAdd == queueCommand.Command)
                {
                    return;
                }

                if (queueCommand.Command.IsReplaceableBy(commandToAdd) && queueCommand.Mutex.WaitOne(0))
                {
                    queueCommand.Command = commandToAdd;
                    queueCommand.Mutex.ReleaseMutex();
                    return;
                }
            }

            _commandQueue.Enqueue(new QueuedCommand(commandToAdd));
        }

        private class QueuedCommand
        {
            public QueuedCommand(Command command)
            {
                Command = command;
            }

            public Command Command { get; set; }

            /// <summary>
            /// Gets a mutex to synchronise access to a queued command. Will not release after being sent.
            /// </summary>
            public Mutex Mutex { get; } = new();
        }
    }
}
