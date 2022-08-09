// <copyright file="ECoSMessageDecoderTests.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using NUnit.Framework;
using TauStellwerk.Server.CommandStations;

#nullable enable

namespace TauStellwerk.Test.ECoS;

/// <summary>
/// <see cref="ECoSMessageDecoder"/>.
/// </summary>
public class ECoSMessageDecoderTests
{
    [Test]
    public void FuncDescCanHandleCRLF()
    {
        string message = "1000 funcdesc[11,9,moment]\r\n1000 funcdesc[12,2055]\r\n1000 funcdesc[13,11783,moment]\r\n";
        var result = ECoSMessageDecoder.DecodeFuncdescMessage(message).ToList();

        Assert.AreEqual(11, result[0].Number);
        Assert.AreEqual(9, result[0].Type);
        Assert.AreEqual(true, result[0].IsMomentary);

        Assert.AreEqual(12, result[1].Number);
        Assert.AreEqual(2055, result[1].Type);
        Assert.AreEqual(false, result[1].IsMomentary);

        Assert.AreEqual(13, result[2].Number);
        Assert.AreEqual(11783, result[2].Type);
        Assert.AreEqual(true, result[2].IsMomentary);
    }

    [Test]
    public void FuncDescCanHandleLF()
    {
        string message = "1000 funcdesc[11,9,moment]\n1000 funcdesc[12,2055]\n1000 funcdesc[13,11783,moment]\n";
        var result = ECoSMessageDecoder.DecodeFuncdescMessage(message).ToList();

        Assert.AreEqual(11, result[0].Number);
        Assert.AreEqual(9, result[0].Type);
        Assert.AreEqual(true, result[0].IsMomentary);

        Assert.AreEqual(12, result[1].Number);
        Assert.AreEqual(2055, result[1].Type);
        Assert.AreEqual(false, result[1].IsMomentary);

        Assert.AreEqual(13, result[2].Number);
        Assert.AreEqual(11783, result[2].Type);
        Assert.AreEqual(true, result[2].IsMomentary);
    }
}
