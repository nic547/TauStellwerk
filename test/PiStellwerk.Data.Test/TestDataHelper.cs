// <copyright file="TestDataHelper.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace PiStellwerk.Data.Test
{
    /// <summary>
    /// Contains helpers proving sample data for testing stuff.
    /// </summary>
    public static class TestDataHelper
    {
        /// <summary>
        /// Creates an engine with sample data.
        /// </summary>
        /// <returns>The engine.</returns>
        public static Engine CreateTestEngine()
        {
            return new Engine()
            {
                Name = "RE 777",
                TopSpeed = 356,
                Id = 7777,
                Address = 77,
                SpeedSteps = 128,
                SpeedDisplayType = SpeedDisplayType.TopSpeed,
                Tags = { "testtag1", "testtag2" },
                Functions =
                {
                    new DccFunction(0, "Lights"),
                },
                Image =
                {
                    new EngineImage()
                    {
                        Filename = "test.jpg",
                    },
                },
            };
        }
    }
}
