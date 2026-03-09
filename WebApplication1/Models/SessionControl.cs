using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IncidentProject.Models
{
    public class SessionControl : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var loginData = filterContext.HttpContext.Session.Get("Userdetails");
            if (loginData != null)

                return;
            else
            {
                var redirectTarget = new RouteValueDictionary { { "Controller", "Logins" }, { "Action", "Index" } };
                filterContext.Result = new RedirectToRouteResult(redirectTarget);
            }
        }
    }
}
