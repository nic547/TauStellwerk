// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

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
