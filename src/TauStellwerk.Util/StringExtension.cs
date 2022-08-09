// <copyright file="StringExtension.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Globalization;

namespace TauStellwerk.Util;

public static class StringExtension
{
    /// <summary>
    /// Returns the leftmost characters of the string.
    /// </summary>
    /// <param name="input">The string to take the characters from.</param>
    /// <param name="chars">The maximum number of chars to take - less will be taken if the string is too short.</param>
    /// <returns>The characters.</returns>
    public static string Left(this string input, int chars)
    {
        StringInfo stringInfo = new(input);
        var length = stringInfo.LengthInTextElements;

        if (length <= 0 || chars <= 0)
        {
            return string.Empty;
        }

        return new StringInfo(input).SubstringByTextElements(0, Math.Min(length, chars));
    }
}
