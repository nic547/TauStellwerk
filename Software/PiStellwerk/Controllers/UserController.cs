using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PiStellwerk.BackgroundServices;
using PiStellwerk.Data;

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
