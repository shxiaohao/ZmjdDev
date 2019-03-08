using HJD.Framework.WCF;
using HJD.HotelManagementCenter.Domain;
using HJD.HotelManagementCenter.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HJDAPI.Models;

namespace HJDAPI.Controllers
{
     public class SMSAdapter
     {
         static ISMSService SMSService = ServiceProxyFactory.Create<ISMSService>("ISMSService");

         public static List<SMSEntity> GetWaitSendSMSList4App()
         {
             return SMSService.GetWaitSendSMSList4App();
         }

        public static int UpdateSMSSendState4App(int ID, string mtstat, string mterrcode)
         {
             return SMSService.UpdateSMSSendState4App(ID, mtstat, mterrcode);
         }

         public static bool SendSMS(string phone, string sms)
         {
             //Log.WriteLog(string.Format("API SMServiceController SendSMS:{0} {1} {2}", phone, sms, HttpContext.Current.Request.UserHostAddress));
             return SMSService.SendSMS(phone, sms);
         }

         public static bool SendNotice(int noticId, string userList, string msg)
         {
             return SMSService.SendNotice(noticId, userList, msg);
         }

         public static bool SendSMS1(string phone, string sms, string orderid)
         {
             //Log.WriteLog(string.Format("API SMServiceController SendSMS:{0} {1} {2} {3}", phone, sms, orderid, HttpContext.Current.Request.UserHostAddress));
             SMSRequestEntity s = new SMSRequestEntity();
             s.TelPhone = phone;
             s.OrderId = Convert.ToInt64(string.IsNullOrEmpty(orderid) == true ? "0" : orderid);
             s.Msg = sms;
             s.TypeId = 1;
             return SMSService.SendSMS1(s);
         }



         public static bool YZXSMSsx(YZXSMSRequestEntity param)
         {
             return SMSService.YZXReceiveSX(param);

         }
         /// <summary>
         /// 营销
         /// </summary>
         /// <param name="param"></param>
         /// <returns></returns>
         public static bool YZXYXSMSsx(YZXSMSRequestEntity param)
         {
             param.moid = "营销：" + param.moid;
             return SMSService.YZXReceiveSX(param);

         }
         public static bool YZXSMSstate(List<YZXStateEntity> paramList)
         {
             int ret = 0;
             if (paramList.Count > 0) {

                ret= SMSService.YZXStateCode(paramList);
             
             }
             return ret == 1 ? true:false;      
             
         }

         public static bool YZXYXSMSstate(List<YZXStateEntity> paramList)
         {
             int ret = 0;
             if (paramList.Count > 0)
             {
                 foreach (YZXStateEntity obj in paramList) {

                     obj.report_status = "营销：" + obj.report_status;
                 }


                 ret = SMSService.YZXStateCode(paramList);

             }
             return ret == 1 ? true : false;

         }
    }
}
