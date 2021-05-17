// <copyright file="HardwareInfoController.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using HardwareInfo;
using Microsoft.AspNetCore.Mvc;

namespace PiStellwerk.Controllers
{
    /// <summary>
    /// HTTP Controller related to information related to the hardware. CPU temps/usage, memory usage etc.
    /// </summary>
    [ApiController]
    [Route("hardwareinfo")]
    public class HardwareInfoController : Controller
    {
        /// <summary>
        /// HTTP Get.
        /// </summary>
        /// <returns>A list of records about certain hardware stats.</returns>
        [HttpGet]
        public IReadOnlyList<InfoRecord> Get()
        {
            return HardwareInfoGatherer.Get();
        }
    }
}