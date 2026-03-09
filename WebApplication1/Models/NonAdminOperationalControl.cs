using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace IncidentProject.Models
{
    public class NonAdminOperationalControl : ActionFilterAttribute
    {
        public static readonly IConfiguration _config;
        public static IServiceProvider ServiceProvider { get; }
        static NonAdminOperationalControl()
        {
            var builder = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("appsettings.json", false, true)
          .AddEnvironmentVariables();
            _config = builder.Build();
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {

            var loginData = context.HttpContext.Session.GetString("Userdetails");
            if (loginData != null)
            {

                string[] adminList = _config.GetSection("AdminRights").GetSection("admins").Value.Split(';');

                var adminemail = "";
                bool access = false;

                foreach (string admin in adminList)
                {
                    adminemail = admin;

                    if (loginData == adminemail)//_accessLevel.Admin)
                    {
                        access = true;
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }

                if (access == false)
                {
                    var redirectTarget = new RouteValueDictionary { { "Controller", "Logins" }, { "Action", "AccessDenied" } };
                    context.Result = new RedirectToRouteResult(redirectTarget);
                }
            }

        }
    }
}
