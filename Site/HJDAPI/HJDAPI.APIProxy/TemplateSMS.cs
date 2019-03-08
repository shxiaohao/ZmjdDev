using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HJD.ADServices.Contract;
using HJD.CouponService.Contracts.Entity;
using HJDAPI.Common.Helpers;
using HJDAPI.Controllers.Adapter;
using HJDAPI.Models.JiGuangSMS;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HJDAPI.APIProxy
{
    public class TemplateSMS : BaseProxy
    {

        /// <summary>
        /// 单条发送极光短信
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static TemplateSendResult SendSMSByTemplate(TemplateMessage p)
        {
                string url = APISiteUrl + "api/JiGuangSMS/SendTemplateMessageAsync";
                CookieContainer cc = new CookieContainer();
                string json = HttpRequestHelper.PostJson(url, p, ref cc);
            

                ////JObject.FromObject(json);

                ////将json转换为JObject
                //JObject jObj = JObject.Parse(json);
                TemplateSendResult result = Newtonsoft.Json.JsonConvert.DeserializeObject<TemplateSendResult>(json);
                return result;
        }



    }





}
