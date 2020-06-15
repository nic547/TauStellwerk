// <copyright file="EngineController.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PiStellwerk.Data;

namespace PiStellwerk.Controllers
{
    /// <summary>
    /// Controller for everything related to engines.
    /// </summary>
    [ApiController]
    [Route("[Controller]")]
    public class EngineController : Controller
    {
        private readonly StwDbContext _dbContext;

        public EngineController(StwDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("List")]
        public IReadOnlyList<Engine>? GetEngines()
        {
            if (_dbContext.Engines.Any())
            {
                return _dbContext.Engines.Include(e => e.Functions).ToList<Engine>();
            }
            else
            {
                return null;
            }
        }

        [HttpPost("command/{id}")]
        public void EngineAction(int id)
        {
            // TODO: Remove testing sleep
            Thread.Sleep(1);
        }
    }
}
