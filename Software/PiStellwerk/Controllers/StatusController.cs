using System;
using Microsoft.AspNetCore.Mvc;
using PiStellwerk.BackgroundServices;
using PiStellwerk.Controllers;
using PiStellwerk.Data;

namespace PiStellwerk.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class StatusController : Controller
    {
        private static Status _status = new Status() { IsRunning = true, LastActionUsername = "SYSTEM" };

        [HttpPut]
        public Status Put([FromBody] User user)
        {
            UserService.UpdateUser(user);
            return _status;
        }

        [HttpPost]
        public void Post([FromBody] Status status)
        {
            _status = status;
            Console.WriteLine($"{DateTime.Now} {status.LastActionUsername} has {(status.IsRunning ? "started" : "stopped")} the PiStellwerk");
        }
    }
}