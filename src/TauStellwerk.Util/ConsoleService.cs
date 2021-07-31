// <copyright file="ConsoleService.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Web;

#nullable enable

namespace TauStellwerk.Util
{
    public static class ConsoleService
    {
        public static void PrintMessage(string message)
        {
            Console.WriteLine($"{DateTime.Now} > {HttpUtility.HtmlDecode(message)}");
        }

        public static void PrintWarning(string warning)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"{DateTime.Now} > {HttpUtility.HtmlDecode(warning)}");
            Console.ResetColor();
        }

        public static void PrintError(string error)
        {
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{DateTime.Now} > {HttpUtility.HtmlDecode(error)}");
            Console.ResetColor();
        }

        public static void PrintHighlightedMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{DateTime.Now} > {HttpUtility.HtmlDecode(message)}");
            Console.ResetColor();
        }
    }
}
