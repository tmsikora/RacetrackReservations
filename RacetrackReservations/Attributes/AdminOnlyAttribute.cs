using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Identity;
using RacetrackReservations.Models;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class AdminOnlyAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        if (user.Identity.IsAuthenticated)
        {
            // Check the IsAdmin property of the user
            var userManager = context.HttpContext.RequestServices.GetService<UserManager<User>>();
            var userWithRoles = userManager.GetUserAsync(user).Result;

            if (userWithRoles != null && userWithRoles.IsAdmin)
            {
                return; // User is admin, proceed with the request
            }
        }

        // User is not authorized, redirect to the homepage
        context.Result = new RedirectToActionResult("Index", "Home", null);
    }
}
