// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using System.Numerics;

namespace TauStellwerk.Util;

public static class ClampExtension
{
    public static T Clamp<T>(this T value, T lower, T upper)
        where T : INumber<T>
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
    /// <typeparam name="T">An INumber.</typeparam>
    /// <returns>A positive integer no larger then upper.</returns>
    public static T Clamp<T>(this T value, T upper)
    where T : INumber<T>
    {
        return value.Clamp(T.Zero, upper);
    }

    /// <summary>
    /// Clamp a value to a value greater than zero.
    /// </summary>
    /// <param name="value">The value to clamped.</param>
    /// <typeparam name="T">An INumber.</typeparam>
    /// <returns>A positive integer.</returns>
    public static T Clamp<T>(this T value)
    where T : INumber<T>
    {
        return value < T.Zero ? T.Zero : value;
    }
}
