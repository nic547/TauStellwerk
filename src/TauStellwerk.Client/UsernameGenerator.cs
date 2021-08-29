// <copyright file="UsernameGenerator.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using TauStellwerk.Util;

namespace TauStellwerk.Client
{
    public static class UsernameGenerator
    {
        // VGA Colors
        private static readonly List<string> _color = new() { "Black", "Maroon", "Green", "Olive", "Navy", "Purple", "Teal", "Gray", "Silver", "Red", "Lime", "Yellow", "Blue", "Fuchsia", "Aqua", "White" };

        private static readonly List<string> _animals = new() { "Axolotl", "Bat", "Bear", "Bee", "Cat", "Chicken", "Cod", "Cow", "Dog", "Dolphin", "Donkey", "Fox", "Goat", "Horse", "Llama", "Mule", "Ocelot", "Panda", "Parrot", "Pig", "Rabbit", "Salmon", "Sheep", "Spider", "Squid", "Turtle", "Wolf" };

        public static string GetRandomUsername()
        {
            var random = new Random();
            return $"Random {_color.TakeRandom(random)} {_animals.TakeRandom(random)}";
        }
    }
}
