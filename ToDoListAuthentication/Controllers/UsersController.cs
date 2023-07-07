using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoListAuthentication.Models;
using ToDoListAuthentication.Repository;

namespace ToDoListAuthentication.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class UsersController : Controller
    {
        private readonly IJWTManagerRepository _jWTManager;

        public UsersController(IJWTManagerRepository jWTManager)
        {
            this._jWTManager = jWTManager;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("authenticate")]
        public IActionResult Authenticate(Users usersdata)
        {
            var token = _jWTManager.Authenticate(usersdata);

            if (token == null)
            {
                return Unauthorized();
            }

            return Ok(token);
        }
    }
}
