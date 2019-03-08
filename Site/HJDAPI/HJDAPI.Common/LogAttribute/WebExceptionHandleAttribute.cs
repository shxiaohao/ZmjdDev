using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http;
using System.Web.Http.Filters;
using System.Threading;
using System.Configuration;
using System.Web.Http.Controllers;

namespace HJDAPI.Common.LogAttribute
{
    /// <summary>
    /// 异常持久化类
    /// </summary>
    public class GlobalExceptionAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            var exception = actionExecutedContext.Exception;
            var request = actionExecutedContext.Request;
            var headers = request.Headers;
            var appVer = "";
            try
            {
                appVer = headers.GetValues("apptype").First() + headers.GetValues("appver").First();
            }
            catch (Exception ex)
            {
                appVer = "Web";
            }

            string message = string.Format(@"消息内容:{0}|请求IP:{5}|终端:{6}|请求类型:{1}|请求URL:{2}|请求参数:{3}|堆栈信息:{4}"
                , exception.Message
                , request.Method
                , request.RequestUri
                , JsonConvert.SerializeObject(actionExecutedContext.ActionContext.ActionArguments)
                , exception.StackTrace
                , HJDAPI.Common.Helpers.HttpRequestHelper.GetClientIp(request)
                , appVer);

            string url = string.Format("{0}/Home/SendLogInfo", System.Configuration.ConfigurationManager.AppSettings["GlobalExceptionLogSite"]);
            CookieContainer cc = new CookieContainer();
            string json = HJDAPI.Common.Helpers.HttpRequestHelper.PostJson(url, new { Message = message, Project = "API" }, ref cc);
            
            base.OnException(actionExecutedContext);
        }

        public Task ExecuteExceptionFilterAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            return new Task(() => { return; });
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class BroweringRecordAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Action方法执行之后执行
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext) { }

        /// <summary>
        /// Action方法执行前执行
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(HttpActionContext actionContext) { }
    }
}