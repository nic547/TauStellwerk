#nullable enable

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using PiStellwerk.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PiStellwerk.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class EngineController
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
                return _dbContext.Engines.ToList();
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
