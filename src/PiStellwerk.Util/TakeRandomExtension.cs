// <copyright file="TakeRandomExtension.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace PiStellwerk.Util
{
    public static class TakeRandomExtension
    {
        public static T TakeRandom<T>(this ICollection<T> collection, Random random)
        {
            var index = random.Next(0, collection.Count);
            return collection.ElementAt(index);
        }
    }
}