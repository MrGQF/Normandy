using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Normandy.Identity.UserInfo.Api.Controllers
{
    [Route("v1/[controller]/[action]")]
    [ApiController]
    public class ValuesController : Controller
    {
        [Authorize]
        [HttpGet]
        public IEnumerable<string> Get()
        {

            var claims = HttpContext.User.Claims;

            return new string[] { "value1", "value2" };
        }
    }
}
