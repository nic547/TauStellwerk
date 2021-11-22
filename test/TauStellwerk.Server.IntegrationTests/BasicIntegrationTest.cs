// <copyright file="BasicIntegrationTest.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.IO;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

namespace TauStellwerk.Server.IntegrationTests;

public class BasicIntegrationTest
{
    private readonly WebApplicationFactory<Startup> _factory;

    public BasicIntegrationTest()
    {
        _factory = new CustomWebApplicationFactory<Startup>();
    }

    [SetUp]
    public void SetUp()
    {
        File.Delete("StwDatabase.db");
    }
}
