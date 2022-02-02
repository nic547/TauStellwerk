using System;
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
