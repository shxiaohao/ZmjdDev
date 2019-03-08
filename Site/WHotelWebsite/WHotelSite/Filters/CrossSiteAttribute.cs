using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WHotelSite.Common;

namespace WHotelSite.Filters
{
    public class CrossSiteAttribute : FilterAttribute, IActionFilter //System.Web.Mvc.FilterAttribute  //.Http.Filters.ActionFilterAttribute
    {
        private const string Origin = "Origin";
        /// <summary>
        /// Access-Control-Allow-Origin是HTML5中定义的一种服务器端返回Response header，用来解决资源（比如字体）的跨域权限问题。
        /// </summary>
        private const string AccessControlAllowOrigin = "Access-Control-Allow-Origin";
        /// <summary>
        ///  originHeaderdefault的值可以使 URL 或 *，如果是 URL 则只会允许来自该 URL 的请求，* 则允许任何域的请求
        /// </summary>
      //  private const string originHeaderdefault = "*";//  "https://www.zmjiudian.com" ; //"https://www.dev.jiudian.corp" ; //

        public void OnActionExecuting(ActionExecutingContext filterContext)
        { }
        /// <summary>
        /// 该方法允许api支持跨域调用
        /// </summary>
        /// <param name="actionExecutedContext"> 初始化 System.Web.Http.Filters.HttpActionExecutedContext 类的新实例。</param>
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            filterContext.HttpContext.Response.Headers.Remove(AccessControlAllowOrigin);
            filterContext.HttpContext.Response.Headers.Add(AccessControlAllowOrigin, Config.Access_Control_Allow_Origin_URL);
        }
    }
}