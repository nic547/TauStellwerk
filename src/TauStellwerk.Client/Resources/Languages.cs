// <copyright file="Languages.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Globalization;

namespace TauStellwerk.Client.Resources;

public static class Languages
{
    public static List<string> LanguageNames { get; } = new() { "English", "Deutsch", "Schwyzerdütsch" };

    public static void SetUILanguage(string language)
    {
        string languageCode = language switch
        {
            "Deutsch" => "de",
            "Schwyzerdütsch" => "gsw",
            _ => "en",
        };

        Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(languageCode);
    }
}
