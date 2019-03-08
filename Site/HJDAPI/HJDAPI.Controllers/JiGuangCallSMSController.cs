using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using HJDAPI.Controllers.Common;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Web;
using System.Web.Http;
using System.Collections.Specialized;
using HJDAPI.Models.JiGuangSMS;
using HJD.Framework.WCF;
using HJDAPI.Controllers.Adapter;

namespace HJDAPI.Controllers
{
   public class JiGuangCallSMSController: BaseApiController
    {


        #region  校验部分
        /// <summary>
        /// 上行消息内容回调
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public HttpResponseMessage UpSMSCall(string echostr)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.StatusCode = HttpStatusCode.OK;
            response.Content = new StringContent(echostr);    // 响应内容
            return response;

        }



        /// <summary>
        /// 下行消息送达状态回调
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public HttpResponseMessage DownSMSCall(string echostr)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.StatusCode = HttpStatusCode.OK;
            response.Content = new StringContent(echostr);    // 响应内容
            return response;
        }


        /// <summary>
        /// 模板
        /// </summary>
        /// <param name="echostr"></param>
        /// <returns></returns>
        /// 
        [System.Web.Http.HttpGet]
        public HttpResponseMessage TemplateSMSCall(string echostr)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.StatusCode = HttpStatusCode.OK;
            response.Content = new StringContent(echostr);    // 响应内容
            return response;
        }

        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="echostr"></param>
        /// <returns></returns>
        /// 
        [System.Web.Http.HttpGet]
        public HttpResponseMessage SignSMSCall(string echostr)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.StatusCode = HttpStatusCode.OK;
            response.Content = new StringContent(echostr);    // 响应内容
            return response;
        }

        #endregion




        /// <summary>
        /// 上行消息内容回调
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public string  UpSMSCall()
        {

            Request.Content.ReadAsStreamAsync().Result.Seek(0, System.IO.SeekOrigin.Begin);
            var content = Request.Content.ReadAsStringAsync().Result;

            var result=  HttpUtility.UrlDecode(content);


            NameValueCollection resultList= HttpUtility.ParseQueryString(result);

            //Dictionary<string, string> dict = new Dictionary<string, string>();
            string nonce = "";

            foreach (var k in resultList.AllKeys)
            {

                if (k == "nonce") {

                     nonce = resultList[k].Replace("\\", "");//去掉转义符

                }

                if (k == "data") {

                    var str = resultList[k].Replace("\\", "");//去掉转义符

                    UpParam param= JsonConvert.DeserializeObject<UpParam>(str);
                    Log.WriteLog("上行消息"+param.content+param.replyTime+param.phone);

                    TemplateAdapter.UpdateThirdTemplateReply(nonce, param.content, param.replyTime, param.phone);



                }

                //dict.Add(k, resultList[k]);
            }

            return result;

        }



        /// <summary>
        /// 下行消息送达状态回调
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public string DownSMSCall()
        {

            Request.Content.ReadAsStreamAsync().Result.Seek(0, System.IO.SeekOrigin.Begin);
            var content = Request.Content.ReadAsStringAsync().Result;

            var result = HttpUtility.UrlDecode(content);


            NameValueCollection resultList = HttpUtility.ParseQueryString(result);

            Dictionary<string, string> dict = new Dictionary<string, string>();

            foreach (var k in resultList.AllKeys)
            {

                if (k == "data")
                {

                    var str = resultList[k].Replace("\\", "");//去掉转义符

                    DownParam param = JsonConvert.DeserializeObject<DownParam>(str);
                    Log.WriteLog("下行消息" + param.msgId + param.phone + param.receiveTime+param.status);

                    TemplateAdapter.UpdateThirdTemplateSMSCallMessage(param.msgId, param.status.ToString());







                }

                dict.Add(k, resultList[k]);
            }

            return result;

        }


        /// <summary>
        /// 模板
        /// </summary>
        /// <param name="echostr"></param>
        /// <returns></returns>
        /// 
        [System.Web.Http.HttpPost]
        public void TemplateSMSCall()
        {
            Request.Content.ReadAsStreamAsync().Result.Seek(0, System.IO.SeekOrigin.Begin);
            var content = Request.Content.ReadAsStringAsync().Result;

            var result = HttpUtility.UrlDecode(content);

            Log.WriteLog("模板审核报文结果" + result);

            NameValueCollection resultList = HttpUtility.ParseQueryString(result);

            //Dictionary<string, string> dict = new Dictionary<string, string>();
            Log.WriteLog("模板审核开始处理结果");

            foreach (var k in resultList.AllKeys)
            {
                Log.WriteLog("模板审核开始处理结果数据");
                if (k == "data")
                {

                    var str = resultList[k].Replace("\\", "");//去掉转义符
                    Log.WriteLog("模板审核开始处理结果："+ str);

                    TemplateAudit param = JsonConvert.DeserializeObject<TemplateAudit>(str);

                    Log.WriteLog("模板审核反序列化后参数结果：" + param.refuseReason + param.status + param.tempId);
                    try {

                        TemplateAdapter.IsThirdPartSMSTemplatePass(param.tempId, param.status, param.refuseReason);//更新模板审核状态

                    } catch (Exception e) {

                        Log.WriteLog("更新模板审核状态报错："+e.Message);

                    }
                    


                    Log.WriteLog("模板审核" + param.refuseReason + param.status + param.tempId );

                }

                //dict.Add(k, resultList[k]);
            }
        }

        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="echostr"></param>
        /// <returns></returns>
        /// 
        [System.Web.Http.HttpPost]
        public string SignSMSCall()
        {
            Request.Content.ReadAsStreamAsync().Result.Seek(0, System.IO.SeekOrigin.Begin);
            var content = Request.Content.ReadAsStringAsync().Result;

            var result = HttpUtility.UrlDecode(content);
            NameValueCollection resultList = HttpUtility.ParseQueryString(result);

            Dictionary<string, string> dict = new Dictionary<string, string>();

            foreach (var k in resultList.AllKeys)
            {

                if (k == "data")
                {

                    var str = resultList[k].Replace("\\", "");//去掉转义符
                    SignAudit param = JsonConvert.DeserializeObject<SignAudit>(str);
                    Log.WriteLog("签名审核" + param.refuseReason + param.signId + param.status );

                }

                dict.Add(k, resultList[k]);
            }

            return result;
        }







        //public static IDictionary<string, string> ToDictionary(this NameValueCollection col)
        //{
        //    IDictionary<string, string> dict = new Dictionary<string, string>();
        //    foreach (var k in col.AllKeys)
        //    {
        //        dict.Add(k, col[k]);
        //    }
        //    return dict;
        //}






    }






}
