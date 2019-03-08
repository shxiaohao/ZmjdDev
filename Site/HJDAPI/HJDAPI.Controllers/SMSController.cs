using HJD.Framework.WCF;
using HJD.HotelManagementCenter.Domain;
using HJD.HotelManagementCenter.IServices;
using HJDAPI.Controllers.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using HJDAPI.Models;

namespace HJDAPI.Controllers
{
    public class SMServiceController:BaseApiController
    {
     
        [HttpGet]
        public List<SMSEntity> GetWaitSendSMSList4App()
        {
            return SMSAdapter.GetWaitSendSMSList4App();
        }

        [HttpPost]
        public static int UpdateSMSSendState4App(int ID, string mtstat, string mterrcode)
        {
            return SMSAdapter.UpdateSMSSendState4App(ID, mtstat, mterrcode);
        }

        public static bool SendSMS(string phone, string sms)
        {
            if (sms.Trim().Length > 0)
            {
                //Log.WriteLog(string.Format("API SMServiceController SendSMS:{0} {1} {2}", phone, sms, HttpContext.Current.Request.UserHostAddress));
                return SMSAdapter.SendSMS(phone, sms);
            }
            else
            {
                return false;
            }
        }

        public static bool SendNotice(int noticId, string userList, string msg)
        {
            return SMSAdapter.SendNotice(noticId, userList, msg);
        }

        public static bool SendSMS1(string phone, string sms,string orderid)
        {
             return SMSAdapter.SendSMS1( phone,  sms, orderid);
        }
        /// <summary>
        /// 短信上行-推送方式
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        /// 
        [HttpPost]
        public string YZXSMSsx1 ()
        {
            var sState = "{\"code\":\"" + "0" + "\"}";//成功

            Log.WriteLog("YZXSMSsx:Start");
            try
            {
                var inputStream = HttpContext.Current.Request.InputStream;
                var strLen = Convert.ToInt32(inputStream.Length);

                if (strLen == 0)
                    return null;

                var strArr = new byte[strLen];
                inputStream.Read(strArr, 0, strLen);
                var requestMes = Encoding.UTF8.GetString(strArr);

                sState += requestMes;


            }
            catch (Exception err)
            {
                sState += err.Message;
            }
            Log.WriteLog("YZXSMSsx:" + sState);
            return sState;
            //var sState = "{\"code\":\"" + "0" + "\"}";//成功

            //var fState = "{\"code\":\"" + "403" + "\""//失败
            //           + ",\"errmsg\":\"" + "ip limit" + "\""
            //           + "}";

            //try
            //{
            //    var inputStream = HttpContext.Current.Request.InputStream;
            //    var strLen = Convert.ToInt32(inputStream.Length);//获取长度
            //    if (strLen == 0)
            //        Log.WriteLog("YZXSMSsx:没有取到推送数据");

            //    var strArr = new byte[strLen];
            //    inputStream.Read(strArr, 0, strLen);
            //    string requestMes = Encoding.UTF8.GetString(strArr);
            //    requestMes = HttpContext.Current.Server.UrlDecode(requestMes);


            //    return SMSAdapter.YZXSMSsx(requestMes) ? sState : fState;

            //}
            //catch (Exception e)
            //{

            //    Log.WriteLog("YZXSMSsx:" + e);
            //    return fState;

            //}

        }
        /// <summary>
        /// 云之讯上行短信接收
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public string YZXSMSsx(YZXSMSRequestEntity param)
        {
            var sState = "{\"code\":\"" + "0" + "\"}";//成功

            var fState = "{\"code\":\"" + "403" + "\""//失败
                       + ",\"errmsg\":\"" + "ip limit" + "\""
                       + "}";
            try {

                YZXSMSRequestEntity ydata = new YZXSMSRequestEntity();
                ydata.moid = param.moid;
                ydata.mobile = param.mobile;
                ydata.content = param.content;
                ydata.reply_time = param.reply_time;
                return SMSAdapter.YZXSMSsx(ydata) ? sState : fState;
            }
            catch (Exception e) {

                Log.WriteLog("YZXSMSsx:" + e);
                return fState;
            }
        }
        /// <summary>
        /// 营销上行
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public string YZXYXSMSsx(YZXSMSRequestEntity param)
        {
            var sState = "{\"code\":\"" + "0" + "\"}";//成功

            var fState = "{\"code\":\"" + "403" + "\""//失败
                       + ",\"errmsg\":\"" + "ip limit" + "\""
                       + "}";
            try
            {

                YZXSMSRequestEntity ydata = new YZXSMSRequestEntity();
                ydata.moid = param.moid;
                ydata.mobile = param.mobile;
                ydata.content = param.content;
                ydata.reply_time = param.reply_time;
                return SMSAdapter.YZXYXSMSsx(ydata) ? sState : fState;
            }
            catch (Exception e)
            {

                Log.WriteLog("YZXYXSMSsx:" + e);
                return fState;
            }
        }



        /// <summary>
        /// 云之讯状态接收
        /// </summary>
        [HttpPost]
        public string YZXSMSstate(List<YZXStateEntity> paramList)
        {

            var sState = "{\"code\":\"" + "0" + "\"}";//成功

            var fState = "{\"code\":\"" + "403" + "\""//失败
                       + ",\"errmsg\":\"" + "ip limit" + "\""
                       + "}";

            try
            {
                return SMSAdapter.YZXSMSstate(paramList) ? sState : fState;
            }
            catch (Exception e)
            {

                Log.WriteLog("云之讯状态本次未写入:" + e);
                return fState;
            }

    
        }

        [HttpPost]
        public string YZXYXSMSstate(List<YZXStateEntity> paramList)
        {

            var sState = "{\"code\":\"" + "0" + "\"}";//成功

            var fState = "{\"code\":\"" + "403" + "\""//失败
                       + ",\"errmsg\":\"" + "ip limit" + "\""
                       + "}";

            try
            {
                return SMSAdapter.YZXYXSMSstate(paramList) ? sState : fState;
            }
            catch (Exception e)
            {

                Log.WriteLog("云之讯营销状态本次未写入:" + e);
                return fState;
            }


        }







    }
}