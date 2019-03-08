using HJD.Framework.WCF;
using HJDAPI.Controllers.Common;
using HJDAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace HJDAPI.Controllers
{
    public class ThirdPartyController : BaseApiController
    {
        public static HJD.HotelManagementCenter.IServices.IThirdPartyService SupplierService = ServiceProxyFactory.Create<HJD.HotelManagementCenter.IServices.IThirdPartyService>("IThirdPartyService");

        public static string SUCESSRESULT= "<?xml version=\"1.0\" encoding=\"utf-8\"?><root><result>true</result><msg>接收成功</msg></root>";

        public static string custId = "2058732";
        public static string apikey = "E2F32F61A2D5D9054B5C1ED5C35E2765";


        [HttpPost]
        public HttpResponseMessage LYTicketRefund(ThirdPartyLyRequestParam Context)
        {
            LYResponseResultEntity responseModel = new LYResponseResultEntity();
            bool res = true;
            if (Context != null)
            {
                try
                {
                    string txt = " \r\n本地ID:" + Context.requestBody.orderBillId.ToString() + " 手续费:" + Context.requestBody.poundageAmount.ToString() + " 退款金额:" + Context.requestBody.refundAmount.ToString() + " 退款票数:" + Context.requestBody.refundTicketsNum.ToString() + " 退款时间:" + Context.requestBody.refundTime.ToString() + " 退款类型:" + Context.requestBody.RefundType.ToString() + " 请求编码:" + Context.requestBody.returnCode.ToString() + " 响应信息:" + Context.requestBody.returnMsg.ToString() + " 订单号:" + Context.requestBody.serialId.ToString();
                    Log.WriteLog("TestThirdPartyLY: LYTicketRefund " + Context.requestHead.digitalSign + Context.requestHead.agentAccount + txt);
                }
                catch
                {
                    res = false;
                }
            }
            else
            {
                res = false;

            }
            responseModel.respTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if (res)
            {
                responseModel.respCode = 1;
                responseModel.respMsg = "回调数据更新成功";
            }
            else
            {
                responseModel.respCode = 0;
                responseModel.respMsg = "回调数据更新失败";
            }
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string str = serializer.Serialize(responseModel);
            HttpResponseMessage result = new HttpResponseMessage { Content = new StringContent(str, Encoding.GetEncoding("UTF-8"), "application/json") };
            return result;
        }


        /// <summary>
        /// 确认通知
        /// </summary>
        /// <param name="Params"></param>
        /// <returns></returns>

        [HttpPost]
        public string ZwyOrderConfirm(object o) {

            Request.Content.ReadAsStreamAsync().Result.Seek(0, System.IO.SeekOrigin.Begin);
            var content = Request.Content.ReadAsStringAsync().Result;
            //var content1 = Request.Content.ReadAsStringAsync().Result.ToList();
            //var stk = "apikey=E2F32F61A2D5D9054B5C1ED5C35E2765&cust_id=2058732&order_id=81504211&ticket_num=1&order_money=0.1&sale_money=500";
            Log.WriteLog("自我游推送结果：" + content);
            Dictionary<string, string> Params = new Dictionary<string, string>();
            string[] array = content.Split('&');

            foreach (var item in array)
            {
                string[] arr = item.Split('=');
                Params.Add(arr[0], arr[1]);

            }

            var s = Params;

            foreach (var obj in Params)
            {

                Log.WriteLog("订单确认：" + obj.Key + ":" + obj.Value + "/n");

            }

            Log.WriteLog("【订单确认】");
            return SUCESSRESULT;


        }

        /// <summary>
        /// 订单取消
        /// </summary>
        /// <param name="Params"></param>
        /// <returns></returns>

        [HttpPost]
        public string ZwyOrderCancel(object o)
        {
            Request.Content.ReadAsStreamAsync().Result.Seek(0, System.IO.SeekOrigin.Begin);
            var content = Request.Content.ReadAsStringAsync().Result;
            //var content1 = Request.Content.ReadAsStringAsync().Result.ToList();
            //var stk = "apikey=E2F32F61A2D5D9054B5C1ED5C35E2765&cust_id=2058732&order_id=81504211&ticket_num=1&order_money=0.1&sale_money=500";
            Log.WriteLog("自我游推送结果：" + content);
            Dictionary<string, string> Params = new Dictionary<string, string>();
            string[] array = content.Split('&');

            foreach (var item in array)
            {
                string[] arr = item.Split('=');
                Params.Add(arr[0], arr[1]);

            }

            var s = Params;

            foreach (var obj in Params)
            {

                Log.WriteLog("订单取消：" + obj.Key + ":" + obj.Value + "/n");

            }

            Log.WriteLog("【订单取消】");
            return SUCESSRESULT;


        }



        /// <summary>
        /// 订单核销
        /// </summary>
        /// <param name="Params"></param>
        /// <returns></returns>
        [HttpPost]
        public string ZwyOrderHeXiao(object o)
        {


            Request.Content.ReadAsStreamAsync().Result.Seek(0, System.IO.SeekOrigin.Begin);
            var content = Request.Content.ReadAsStringAsync().Result;
            //var content1 = Request.Content.ReadAsStringAsync().Result.ToList();
            //var stk = "apikey=E2F32F61A2D5D9054B5C1ED5C35E2765&cust_id=2058732&order_id=81504211&ticket_num=1&order_money=0.1&sale_money=500";
            Log.WriteLog("自我游推送结果：" + content);
            Dictionary<string, string> Params = new Dictionary<string, string>();
            string[] array = content.Split('&');

            foreach (var item in array)
            {
                string[] arr = item.Split('=');
                Params.Add(arr[0], arr[1]);

            }

            var s = Params;

            foreach (var obj in Params)
            {

                Log.WriteLog("订单核销：" + obj.Key + ":" + obj.Value + "/n");

            }

            Log.WriteLog("【订单核销】");
            return SUCESSRESULT;


        }


        /// <summary>
        /// 订单安排
        /// </summary>
        /// <param name="Params"></param>
        /// <returns></returns>

        [HttpPost]
        public string ZwyOrderPlan(object o)
        {

            Request.Content.ReadAsStreamAsync().Result.Seek(0, System.IO.SeekOrigin.Begin);
            var content = Request.Content.ReadAsStringAsync().Result;
            //var content1 = Request.Content.ReadAsStringAsync().Result.ToList();
            //var stk = "apikey=E2F32F61A2D5D9054B5C1ED5C35E2765&cust_id=2058732&order_id=81504211&ticket_num=1&order_money=0.1&sale_money=500";
            Log.WriteLog("自我游推送结果：" + content);
            Dictionary<string, string> Params = new Dictionary<string, string>();
            string[] array = content.Split('&');

            foreach (var item in array)
            {
                string[] arr = item.Split('=');
                Params.Add(arr[0], arr[1]);

            }

            var s = Params;

            foreach (var obj in Params)
            {

                Log.WriteLog("订单安排：" + obj.Key + ":" + obj.Value + "/n");

            }

            Log.WriteLog("【订单安排】");
            return SUCESSRESULT;


        }




        /// <summary>
        /// 退改申请
        /// </summary>
        /// <param name="Params"></param>
        /// <returns></returns>
        [HttpPost]
        public string ZwyOrderRefund (object o)
        {

            Request.Content.ReadAsStreamAsync().Result.Seek(0, System.IO.SeekOrigin.Begin);
            var content = Request.Content.ReadAsStringAsync().Result;
            //var content1 = Request.Content.ReadAsStringAsync().Result.ToList();
            //var stk = "apikey=E2F32F61A2D5D9054B5C1ED5C35E2765&cust_id=2058732&order_id=81504211&ticket_num=1&order_money=0.1&sale_money=500";
            Log.WriteLog("自我游推送结果：" + content);
            Dictionary<string, string> Params = new Dictionary<string, string>();
            string[] array = content.Split('&');

            foreach (var item in array)
            {
                string[] arr = item.Split('=');
                Params.Add(arr[0], arr[1]);

            }

            var s = Params;

            foreach (var obj in Params)
            {

                Log.WriteLog("订单申请：" + obj.Key + ":" + obj.Value + "/n");

            }

            Log.WriteLog("【订单申请】");
            return SUCESSRESULT;


        }





        /// <summary>
        /// 订单支付
        /// </summary>
        /// <param name="Params"></param>
        /// <returns></returns>
        [HttpPost]
        public string ZwyOrderPay(object o)
        {

            Request.Content.ReadAsStreamAsync().Result.Seek(0, System.IO.SeekOrigin.Begin);
            var content = Request.Content.ReadAsStringAsync().Result;
            //var content1 = Request.Content.ReadAsStringAsync().Result.ToList();
            //var stk = "apikey=E2F32F61A2D5D9054B5C1ED5C35E2765&cust_id=2058732&order_id=81504211&ticket_num=1&order_money=0.1&sale_money=500";
            Log.WriteLog("自我游推送结果：" + content);
            Dictionary<string, string> Params = new Dictionary<string, string>();
            string[] array = content.Split('&');

            foreach (var item in array)
            {
                string[] arr = item.Split('=');
                Params.Add(arr[0], arr[1]);

            }

            var s = Params;

            foreach (var obj in Params)
            {

                Log.WriteLog("订单支付：" + obj.Key + ":" + obj.Value + "/n");

            }

            Log.WriteLog("【订单支付】");
            return SUCESSRESULT;


        }


        /// <summary>
        /// 锁单通知
        /// </summary>
        /// <param name="Params"></param>
        /// <returns></returns>

        [HttpPost]
        public string ZwyOrderLocking(object o)
        {


            Request.Content.ReadAsStreamAsync().Result.Seek(0, System.IO.SeekOrigin.Begin);
            var content = Request.Content.ReadAsStringAsync().Result;
            //var content1 = Request.Content.ReadAsStringAsync().Result.ToList();
            //var stk = "apikey=E2F32F61A2D5D9054B5C1ED5C35E2765&cust_id=2058732&order_id=81504211&ticket_num=1&order_money=0.1&sale_money=500";
            Log.WriteLog("自我游推送结果：" + content);
            Dictionary<string, string> Params = new Dictionary<string, string>();
            string[] array = content.Split('&');

            foreach (var item in array)
            {
                string[] arr = item.Split('=');
                Params.Add(arr[0], arr[1]);

            }

            var s = Params;

            foreach (var obj in Params)
            {

                Log.WriteLog("锁单通知：" + obj.Key + ":" + obj.Value + "/n");

            }

            Log.WriteLog("锁单通知");
            return SUCESSRESULT;


        }


        /// <summary>
        /// 产品变更通知
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        [HttpPost]
        public string ZwyProductChanage(object o)
        {


            Request.Content.ReadAsStreamAsync().Result.Seek(0, System.IO.SeekOrigin.Begin);
            var content = Request.Content.ReadAsStringAsync().Result;
            //var content1 = Request.Content.ReadAsStringAsync().Result.ToList();
            //var stk = "apikey=E2F32F61A2D5D9054B5C1ED5C35E2765&cust_id=2058732&order_id=81504211&ticket_num=1&order_money=0.1&sale_money=500";
            Log.WriteLog("自我游推送结果：" + content);

            Dictionary<string, string> Params = new Dictionary<string, string>();
            string[] array = content.Split('&');

            foreach (var item in array)
            {
                string[] arr = item.Split('=');
                Params.Add(arr[0], arr[1]);

            }

            var s = Params;

            foreach (var obj in Params)
            {

                Log.WriteLog("产品变更：" + obj.Key + ":" + obj.Value + "/n");

            }

            Log.WriteLog("产品变更");
            return SUCESSRESULT;


        }


    }
}
