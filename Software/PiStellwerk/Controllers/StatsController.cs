using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using HardwareInfo;

namespace PiStellwerk.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class StatsController : Controller
    {
        [HttpGet]
        public IReadOnlyList<Stat> Get()
        {
            return HardwareInfoGatherer.Get();
        }
    }
}