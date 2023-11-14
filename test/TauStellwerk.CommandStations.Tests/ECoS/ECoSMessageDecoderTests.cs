// This file is part of the TauStellwerk project.
//  Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.

using FluentAssertions;
using NUnit.Framework;
using TauStellwerk.Server.CommandStations;

namespace TauStellwerk.CommandStations.Tests.ECoS;

/// <summary>
/// <see cref="ECoSMessageDecoder"/>.
/// </summary>
public class ECoSMessageDecoderTests
{
    [Test]
    public void FuncDescCanHandleCRLF()
    {
        var message = "1000 funcdesc[11,9,moment]\r\n1000 funcdesc[12,2055]\r\n1000 funcdesc[13,11783,moment]\r\n";
        var result = ECoSMessageDecoder.DecodeFuncdescMessage(message).ToList();

        result[0].Number.Should().Be(11);
        result[0].Type.Should().Be(9);
        result[0].IsMomentary.Should().Be(true);

        result[1].Number.Should().Be(12);
        result[1].Type.Should().Be(2055);
        result[1].IsMomentary.Should().Be(false);

        result[2].Number.Should().Be(13);
        result[2].Type.Should().Be(11783);
        result[2].IsMomentary.Should().Be(true);
    }

    [Test]
    public void FuncDescCanHandleLF()
    {
        var message = "1000 funcdesc[11,9,moment]\n1000 funcdesc[12,2055]\n1000 funcdesc[13,11783,moment]\n";
        var result = ECoSMessageDecoder.DecodeFuncdescMessage(message).ToList();

        result[0].Number.Should().Be(11);
        result[0].Type.Should().Be(9);
        result[0].IsMomentary.Should().Be(true);

        result[1].Number.Should().Be(12);
        result[1].Type.Should().Be(2055);
        result[1].IsMomentary.Should().Be(false);

        result[2].Number.Should().Be(13);
        result[2].Type.Should().Be(11783);
        result[2].IsMomentary.Should().Be(true);
    }
}
