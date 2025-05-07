using Microsoft.AspNetCore.Mvc;

namespace Hangfire.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController(
        IConfiguration config
        ) : ControllerBase
    {
        public readonly IConfiguration _config = config;
    }
}
