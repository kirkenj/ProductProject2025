using Microsoft.AspNetCore.Mvc;

namespace NotificationService.Api.NotificationApi.Controllers
{
    [ApiController]
    [Route("NotificationApi/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Hello world");
        }
    }
}
