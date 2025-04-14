using AuthService.Core.Application.Models.DTOs.Role;
using AuthService.Core.Application.Models.DTOs.User;
using ClaimsPrincipalExtensions;
using Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AuthService.API.AuthAPI.ActionFIlters
{
    [AttributeUsage(AttributeTargets.Method)]
    public class GetUserActionFilterAttribute : Attribute, IAsyncResultFilter
    {
        public const string ADDRESS_PLACEHOLDER = "Contact administration to get this information";
        public static readonly RoleDto DEFAULT_ROLE = new() { Id = 2, Name = ApiConstants.REGULAR_AUTH_ROLE_NAME };

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (context.Result is not ObjectResult objectResult)
            {
                context.Cancel = true;
                return;
            }

            if (context.HttpContext.User.IsInRole(ApiConstants.ADMIN_AUTH_ROLE_NAME))
            {
                await next();
                return;
            }

            if (objectResult.Value is IReadOnlyCollection<UserDto> userList)
            {
                SetDefaultValues(context, userList);
            }
            else if (objectResult.Value is UserDto userDto)
            {
                SetDefaultValues(context, userDto);
            }

            await next();
        }

        private static void SetDefaultValues(ResultExecutingContext context, IReadOnlyCollection<UserDto> userList)
        {
            foreach (var item in userList)
            {
                SetDefaultValues(context, item);
            }
        }

        private static UserDto SetDefaultValues(ResultExecutingContext context, UserDto user)
        {
            if ((context.HttpContext.User.GetUserId() ?? Guid.Empty) != user.Id)
            {
                user.Role = DEFAULT_ROLE;
                user.Address = ADDRESS_PLACEHOLDER;
            }
            return user;
        }
    }
}
