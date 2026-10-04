using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace _13_DangHoangSon_Assignment01_FontEnd.Filters
{
    public class CustomAuthorizeAttribute : ActionFilterAttribute
    {
        private readonly int[] _roles;

        public CustomAuthorizeAttribute(params int[] roles)
        {
            _roles = roles;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var userId = session.GetInt32("UserId");
            var userRole = session.GetInt32("UserRole");

            if (!userId.HasValue || !userRole.HasValue)
            {
                context.Result = new RedirectToActionResult("Login", "Auth", new { returnUrl = context.HttpContext.Request.Path });
                return;
            }

            if (_roles.Length > 0 && !_roles.Contains(userRole.Value))
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Home", null);
            }
        }
    }
}
