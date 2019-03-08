using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace WHotelSite.Filters
{
    public class GlobalExceptionAttribute : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
#if DEBUG
            return;
#endif
            // 添加记录日志代码
            HttpRequestBase request = filterContext.HttpContext.Request;
            string requestType = request.HttpMethod;
            string requestParams = "{";
            if (requestType == "GET")
            {
                string[] keys = request.QueryString.AllKeys;
                if (keys != null && keys.Length > 0)
                {
                    foreach (var key in keys)
                    {
                        var ss = request.QueryString.Get(key);
                        requestParams += string.Format("\"{0}\":\"{1}\",", key, ss);
                    }
                }
            }
            else
            {
                string[] keys = request.Form.AllKeys;
                if (keys != null && keys.Length > 0)
                {
                    foreach (var key in keys)
                    {
                        var ss = request.Form.Get(key);
                        requestParams += string.Format("\"{0}\":\"{1}\",", key, ss);
                    }
                }
            }
            requestParams = requestParams.Trim(",".ToCharArray());
            requestParams += "}";

            string message = string.Format(@"消息内容:{0}|请求类型:{1}|请求URL:{2}|路由参数:{3}|请求参数:{4}|堆栈信息:{5}"
                , filterContext.Exception.Message
                , requestType
                , request.Url.AbsoluteUri
                , Newtonsoft.Json.JsonConvert.SerializeObject(filterContext.RouteData.Values)
                , requestParams
                , filterContext.Exception.StackTrace);

            //string url = string.Format("{0}/Home/SendLogInfo", System.Configuration.ConfigurationManager.AppSettings["GlobalExceptionLogSite"]);
            //System.Net.CookieContainer cc = new System.Net.CookieContainer();
            //HJDAPI.Common.Helpers.HttpRequestHelper.PostJson(url, new { Message = message, Project = "WHotelSite" }, ref cc);
        }
    }
}