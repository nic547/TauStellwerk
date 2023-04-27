// <copyright file="OptimalStringAlignment.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace TauStellwerk.Util;

public static class OptimalStringAlignment
{
    /// <summary>
    /// Calculates the optimal string alignment distance for two strings, aka the restricted Damerau-Levenshtein distance.
    /// </summary>
    /// <param name="s1">string 1.</param>
    /// <param name="s2">string 2.</param>
    /// <returns>Distance between the two strings.</returns>
    public static int Calculate(string s1, string s2)
    {
        /* A basic implementation using the Wagner–Fischer algorithm.
        Further optimization is possible, but I don't care about it yet. */

        var length1 = s1.Length;
        var length2 = s2.Length;
        var distanceMatrix = new int[length1 + 1, length2 + 1];
        for (var i = 0; i <= length1; i++)
        {
            distanceMatrix[i, 0] = i;
        }

        for (var j = 0; j <= length2; j++)
        {
            distanceMatrix[0, j] = j;
        }

        for (var i = 1; i <= length1; i++)
        {
            var c1 = s1[i - 1];
            for (var j = 1; j <= length2; j++)
            {
                var c2 = s2[j - 1];
                var cost = c1 == c2 ? 0 : 1;

                distanceMatrix[i, j] = Math.Min(
                    Math.Min(
                        distanceMatrix[i - 1, j] + 1,
                        distanceMatrix[i, j - 1] + 1),
                    distanceMatrix[i - 1, j - 1] + cost);

                if (i > 1 && j > 1 && c1 == s2[j - 2] && c2 == s1[i - 2])
                {
                    distanceMatrix[i, j] = Math.Min(
                        distanceMatrix[i, j],
                        distanceMatrix[i - 2, j - 2] + cost);
                }
            }
        }

        return distanceMatrix[length1, length2];
    }
}