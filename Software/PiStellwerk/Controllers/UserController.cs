using Microsoft.AspNetCore.Mvc;
using PiStellwerk.BackgroundServices;
using RpiStellwerk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PiStellwerk.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class UserController
    {
        [HttpGet]
        public IReadOnlyList<User> Get()
        {
            return UserService.GetUsers();
        }

        [HttpPut]
        public void Put([FromBody] User[] user)
        {
            UserService.RenameUser(user[0],user[1]);
        }

    }
}
