using System.Collections.Generic;
using System.Linq;

namespace TauStellwerk.Commands.ECoS;

public record ECoSMessage
{
    public ECoSMessage(List<string> message)
    {
        var firstLine = message.First().Split(' ');
        var lastLine = message.Last().Split(' ');
        var messageLines = message.Skip(1).SkipLast(1);
        Content = string.Join(string.Empty, messageLines);

        Type = firstLine[0].TrimStart('<');
        Command = string.Join(string.Empty, firstLine[1..]).TrimEnd('>', '\r', '\n');

        ErrorCode = int.Parse(lastLine[1]);
        ErrorMessage = string.Join(string.Empty, lastLine[2..]).TrimEnd('>', '\r', '\n');
    }

    public string Type { get; }

    public string Command { get; }

    public string Content { get; }

    public int ErrorCode { get; }

    public string ErrorMessage { get; }
}