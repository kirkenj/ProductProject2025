using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace CentralizedJwtAuthentication
{
    public class CustomJwtBearerEvents : JwtBearerEvents
    {
        private readonly IJwtValidatingService _jwtValidatingService;

        public CustomJwtBearerEvents(IJwtValidatingService jwtValidatingService)
        {
            _jwtValidatingService = jwtValidatingService;
        }

        public override async Task TokenValidated(TokenValidatedContext context)
        {
            var token = context.SecurityToken;
            if (token != null)
            {
                var validationResult = await _jwtValidatingService.IsValid(token.UnsafeToString()!);
                if (!validationResult.Result)
                {
                    context.Fail(validationResult.Message);
                }
            }

            await base.TokenValidated(context);
        }
    }
}
