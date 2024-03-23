// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Hosting.Systemd;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

namespace TauStellwerk.Server;

public sealed class CustomLogFormatter : ConsoleFormatter
{
    private const string Red = "\x1B[1m\x1B[31m";
    private const string DarkYellow = "\x1B[33m";
    private const string Green = "\x1B[1m\x1B[32m";
    private const string Reset = "\x1B[39m\x1B[22m";

    private readonly bool _isService;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required here")]
    public CustomLogFormatter(SimpleConsoleFormatterOptions? options = null)
        : base(nameof(CustomLogFormatter))
    {
        _isService = SystemdHelpers.IsSystemdService();
    }

    public override void Write<TState>(
        in LogEntry<TState> logEntry,
        IExternalScopeProvider? scopeProvider,
        TextWriter textWriter)
    {
        var message = logEntry.Formatter?.Invoke(logEntry.State, logEntry.Exception);

        if (message is null)
        {
            return;
        }

        var logLevel = GetShortLevelNameColored(logEntry.LogLevel);

        if (_isService)
        {
            var systemdLogLevel = GetSystemdLevel(logEntry.LogLevel);
            textWriter.WriteLine($"{systemdLogLevel}{logLevel}> {message}");
        }
        else
        {
            textWriter.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {logLevel}> {message}");
        }
    }

    private static string GetShortLevelNameColored(LogLevel level)
    {
        return level switch
        {
            LogLevel.Trace => "TRACE",
            LogLevel.Debug => "DEBUG",
            LogLevel.Information => $"{Green}INFO {Reset}",
            LogLevel.Warning => $"{DarkYellow}WARN {Reset}",
            LogLevel.Error => $"{Red}ERROR{Reset}",
            LogLevel.Critical => $"{Red}CRIT {Reset}",
            LogLevel.None => "NONE ",
            _ => throw new ArgumentOutOfRangeException(nameof(level)),
        };
    }

    private static string GetSystemdLevel(LogLevel level)
    {
        return level switch
        {
            LogLevel.Trace => "<7>",
            LogLevel.Debug => "<7>",
            LogLevel.Information => "<6>",
            LogLevel.Warning => "<4>",
            LogLevel.Error => "<3>",
            LogLevel.Critical => "<2>",
            LogLevel.None => "<7>",
            _ => throw new ArgumentOutOfRangeException(nameof(level)),
        };
    }
}
