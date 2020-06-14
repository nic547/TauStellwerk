﻿#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using PiStellwerk.Data;

namespace PiStellwerk.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class EngineController: Controller
    {
        private StwDbContext _dbContext;
        public EngineController (StwDbContext dbContext)
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
            else { return null; }
        }

        [HttpPost("command/{id}")]
        public void EngineAction(int id)
        {
            // TODO: Remove testing sleep
            Thread.Sleep(1);
        }
    }
}
