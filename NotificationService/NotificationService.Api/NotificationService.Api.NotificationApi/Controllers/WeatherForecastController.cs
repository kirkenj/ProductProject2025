using Extensions.ClaimsPrincipalExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Api.NotificationApi.Contracts;
using NotificationService.Api.NotificationApi.Models;

namespace NotificationService.Api.NotificationApi.Controllers
{
    [ApiController]
    [Route("NotificationApi/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ISignalRNotificationService _signalRNotificationService;

        public WeatherForecastController(ISignalRNotificationService signalRNotificationService)
        {
            _signalRNotificationService = signalRNotificationService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Hello world");
        }

        [Authorize]
        [HttpPost]
        public async Task Callback()
        {
            var message = new SignalRNotification()
            {
                Body = "Hello world",
            };
            await _signalRNotificationService.Send(message, User.GetUserId() ?? throw new Exception());
        }
    }
}
