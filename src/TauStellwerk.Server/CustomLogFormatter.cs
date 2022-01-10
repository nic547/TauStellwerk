using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

namespace TauStellwerk;

public sealed class CustomLogFormatter : ConsoleFormatter
{
    private const string Red = "\x1B[1m\x1B[31m";
    private const string DarkYellow = "\x1B[33m";
    private const string Green = "\x1B[1m\x1B[32m";
    private const string Reset = "\x1B[39m\x1B[22m";

    public CustomLogFormatter(SimpleConsoleFormatterOptions? options = null)
        : base(nameof(CustomLogFormatter))
    {
    }

    public override void Write<TState>(
        in LogEntry<TState> logEntry,
        IExternalScopeProvider scopeProvider,
        TextWriter textWriter)
    {
        var message = logEntry.Formatter?.Invoke(logEntry.State, logEntry.Exception);

        if (message is null)
        {
            return;
        }

        var logLevel = GetShortLevelNameColored(logEntry.LogLevel);

        textWriter.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} {logLevel} > {message}");
    }

    private static string GetShortLevelNameColored(LogLevel level)
    {
        return level switch
        {
            LogLevel.None => "NONE ",
            LogLevel.Trace => "TRACE",
            LogLevel.Debug => "DEBUG",
            LogLevel.Information => $"{Green}INFO {Reset}",
            LogLevel.Warning => $"{DarkYellow}WARN {Reset}",
            LogLevel.Error => "ERROR",
            LogLevel.Critical => "CRIT ",
            _ => throw new NotImplementedException(),
        };
    }
}

public static class CustomLoggerExtension
{
    public static void LogInformationHighlighted(this ILogger logger, string text)
    {
        logger.LogInformation($"\x1B[1m\x1B[32m{text}\x1B[39m\x1B[22m");
    }
}