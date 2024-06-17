using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WeatherDataAPI.Models;
using WeatherDataAPI.Repository;

namespace WeatherDataAPI.AttributeTags
{
    [AttributeUsage(validOn: AttributeTargets.Method | AttributeTargets.Class)]
    public class ApiKeyAttribute : Attribute, IAsyncActionFilter
    {
        private UserRolesEnum[] _allowedRoles;

        public UserRolesEnum[] AllowedRoles
        {
            get { return _allowedRoles; }
        }

        public ApiKeyAttribute(params UserRolesEnum[] allowedRoles)
        {
            _allowedRoles = allowedRoles;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.HttpContext.Request.Headers.TryGetValue("apiKey", out var key) == false)
            {
                context.Result = new ContentResult
                {
                    StatusCode = 401,
                    Content = "No API key provided."
                };

                return;
            }

            var validKey = key.ToString().Trim('{', '}');
            var userRepo = context.HttpContext.RequestServices.GetRequiredService<IUserDataRepository>();

            if ( userRepo.AuthenticateUser(validKey, AllowedRoles) == null)
            {
                context.Result = new ContentResult
                {
                    StatusCode = 403,
                    Content = "Provided API key is not valid for this operation"
                };
                return;
            }

            userRepo.UpdateLoginTime(validKey);

            await next();
        }


    }
}
