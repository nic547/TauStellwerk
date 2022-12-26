// <copyright file="CopyEngineTests.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.ObjectModel;
using FluentAssertions;
using NUnit.Framework;
using TauStellwerk.Client.Model;

namespace TauStellwerk.Client.Tests;

public class CopyEngineTests
{
    [Test]
    public void CopiedNameGetsAmendedTest()
    {
        var engine = new EngineFull
        {
            Name = "Test engine",
        };

        var copy = engine.CreateCopy();

        copy.Name.Should().Be("Test engine (Copy)");
    }

    [Test]
    public void TagsShouldBeCopiedTest()
    {
        var engine = new EngineFull()
        {
            Tags = new(new[] { "Tag1", "Tag2" }),
        };

        var copy = engine.CreateCopy();

        copy.Tags.Should().NotBeSameAs(engine.Tags);
        copy.Tags.Should().BeEquivalentTo(engine.Tags);
    }

    [Test]
    public void TopSpeedShouldBeCopiedTest()
    {
        var engine = new EngineFull()
        {
            TopSpeed = 200,
        };

        var copy = engine.CreateCopy();

        copy.TopSpeed.Should().Be(200);
    }

    [Test]
    public void FunctionsShouldBeCopiedTest()
    {
        var engine = new EngineFull()
        {
            Functions = new ObservableCollection<Function>(new[]
            {
                new Function(0, "Light", 0),
                new Function(1, "Sound", 0),
                new Function(2, "High beams", 0),
                new Function(3, "Horn (low, short)", 500),
                new Function(4, "Horn (high, short)", 500),
            }),
        };

        var copy = engine.CreateCopy();

        copy.Functions.Should().NotBeSameAs(engine.Functions);
        copy.Functions.Should().BeEquivalentTo(engine.Functions);
        copy.Functions.First().Should().NotBeNull();
    }

    [Test]
    public void CopiedIdShouldBeZero()
    {
        var engine = new EngineFull { Id = 1337 };

        var copy = engine.CreateCopy();

        copy.Id.Should().Be(0);
    }
}