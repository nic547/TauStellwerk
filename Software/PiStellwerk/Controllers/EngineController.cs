using Microsoft.AspNetCore.Mvc;
using PiStellwerk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PiStellwerk.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class EngineController
    {
        [HttpGet("List")]
        public IReadOnlyList<Engine> GetEngines()
        {
            return TestDataService.GetEngines();
        }

        [HttpPost("command/{id}")]
        public void EngineAction(int id)
        {

        }
    }
}
