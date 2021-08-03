// <copyright file="ClampExtension.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace TauStellwerk.Util
{
    public static class ClampExtension
    {
        public static int Clamp(this int value, int lower, int upper)
        {
            if (value < lower)
            {
                value = lower;
            }

            if (value > upper)
            {
                value = upper;
            }

            return value;
        }

        /// <summary>
        /// Clamp a value to a value greater then zero and the upper limit.
        /// </summary>
        /// <param name="value">The value to clamp.</param>
        /// <param name="upper">The inclusive upper limit.</param>
        /// <returns>A positive integer no larger then upper.</returns>
        public static int Clamp(this int value, int upper)
        {
            return value.Clamp(0, upper);
        }

        /// <summary>
        /// Clamp a value to a value greater than zero.
        /// </summary>
        /// <param name="value">The value to clamped.</param>
        /// <returns>A positive integer.</returns>
        public static int Clamp(this int value)
        {
            return value.Clamp(0, int.MaxValue);
        }
    }
}
