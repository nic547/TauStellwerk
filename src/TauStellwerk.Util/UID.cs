// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using System.Security.Cryptography;

namespace TauStellwerk.Util;

/// <summary>
/// A class that provides unique identifiers. Based on NanoId, but with a fixed length of 16 characters and a fixed alphabet.
/// </summary>
public class UID
{
    public static UID Global { get; } = new UID();

    // 16 character lenght based on https://zelark.github.io/nano-id-cc/ => 39T Ids needed for a 1% chance of collision
    // With that many things, the DCC Addresses are probably the bigger problem.
    private const int CharLength = 16;

    private static readonly char[] Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ_abcdefghijklmnopqrstuvwxyz-".ToCharArray();

    private readonly RandomNumberGenerator rng = RandomNumberGenerator.Create();

    public string GenerateId()
    {
        var bytes = new byte[CharLength];
        var chars = new char[CharLength];
        rng.GetBytes(bytes);

        for (var i = 0; i < CharLength; i++)
        {
            chars[i] = Alphabet[bytes[i] >>> 2];
        }

        return new string(chars);
    }
}
