using HJDAPI.Controllers.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace HJDAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApiEx",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Filters.Add(new HJDAPI.Common.LogAttribute.GlobalExceptionAttribute());

            AutoMapHelper.InitAutoMapper();
        }
    }
}