// <copyright file="HardwareInfoController.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using HardwareInfo;
using Microsoft.AspNetCore.Mvc;

namespace PiStellwerk.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class HardwareInfoController : Controller
    {
        [HttpGet]
        public IReadOnlyList<InfoRecord> Get()
        {
            return HardwareInfoGatherer.Get();
        }
    }
}