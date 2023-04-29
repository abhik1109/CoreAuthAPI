using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreAuthAPI.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class LoginController : ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok("This is Login Index.");
        }
    }
}
