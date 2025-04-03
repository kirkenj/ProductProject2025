using Constants;
using Extensions.ClaimsPrincipalExtensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Core.Application.Features.NotificatioinService.GetNotifications;
using NotificationService.Core.Application.Features.NotificatioinService.MarkNotificationsRead;
using NotificationService.Core.Application.Models.Filters;

namespace NotificationService.Api.NotificationApi.Controllers
{
    [ApiController]
    [Route("NotificationApi/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NotificationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]NotificationFilter filter, int? page, int? pageSize)
        {
            if (!User.IsInRole(ApiConstants.ADMIN_AUTH_ROLE_NAME))
            {
                filter.UserIds = [ User.GetUserId()!.ToString()! ];
            }

            var result = await _mediator.Send(new GetNotificationsCommand() { Filter = filter, Page = page, PageSize = pageSize});
            return result.GetActionResult();
        }

        [Authorize]
        [HttpPost("MarkRead")]
        public async Task<IActionResult> MarkRead([FromBody]IEnumerable<string> notificationIds)
        {
            if (!notificationIds.Any())
            {
                return BadRequest("No ids specified");
            }

            var filter = new NotificationFilter { Ids = notificationIds };
            if (!User.IsInRole(ApiConstants.ADMIN_AUTH_ROLE_NAME))
            {
                filter.UserIds = [ User.GetUserId()!.ToString()! ];
            }

            var result = await _mediator.Send(new MarkNotificationsReadCommand() { Filter = filter });
            return result.GetActionResult();
        }
    }
}
