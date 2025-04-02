using Microsoft.AspNetCore.Mvc;
using NotificationService.Core.Application.Contracts.Persistence;

namespace NotificationService.Api.NotificationApi.Controllers
{
    [ApiController]
    [Route("NotificationApi/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly INotificationRepository _notificationRepository;

        public WeatherForecastController(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        //[Authorize]
        [HttpGet("list")]
        public async Task<IActionResult> Get2()
        {
            var result = await _notificationRepository.GetPageContent(new());
            return Ok(result);
        }
    }
}
