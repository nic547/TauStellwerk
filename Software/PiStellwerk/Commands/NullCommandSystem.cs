// <copyright file="NullCommandSystem.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using JetBrains.Annotations;
using PiStellwerk.Data;
using PiStellwerk.Data.Commands;

namespace PiStellwerk.Commands
{
    /// <summary>
    /// Implements a <see cref="ICommandSystem"/> that does nothing.
    /// </summary>
    [UsedImplicitly]
    public class NullCommandSystem : ICommandSystem
    {
        /// <inheritdoc/>
        public void HandleEngineCommand(JsonCommand command, Engine engine)
        {
            // Do Nothing
        }
    }
}
