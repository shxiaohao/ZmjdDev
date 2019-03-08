using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WHotelSite.Common
{
    public class ClientHelper
    {
        public static string GetUnfullName(string orgName)
        {
            if (string.IsNullOrEmpty(orgName)) return "";
            var start = orgName[0].ToString();
            var centerStr = "*";
            var endStr = "";
            if (orgName.Length > 2) { centerStr = "**"; }//endStr = orgName[2].ToString(); }
            if (orgName.Length > 3) { centerStr = "***"; }// endStr = orgName[3].ToString(); centerStr = "**"; }
            if (orgName.Length > 4) { centerStr = "****"; }//endStr = orgName[4].ToString(); centerStr = "***"; }

            return start + centerStr + endStr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cookieName"></param>
        /// <param name="cookieVal"></param>
        /// <param name="expireTime">默认1天 单位天</param>
        public static void AddCookie(string cookieName, string cookieVal, double expireTime = 1.0)
        {
            //if (HttpContext.Request.Cookies[cookieName] == null)
            //{
            //    HttpCookie cookie = new HttpCookie(cookieName);
            //    cookie.Value = cookieVal;
            //    cookie.Expires = DateTime.Now.AddDays(expireTime);
            //    cookie.Domain = ".zmjiudian.com";
            //    cookie.HttpOnly = true;
            //    HttpContext.Response.Cookies.Add(cookie);
            //}
            //else
            //{
            //    HttpContext.Response.Cookies[cookieName].Expires.AddDays(expireTime);
            //}
        }

        public static long GetBotaoUserIdFromCookie(HttpCookieCollection collection)
        {
            if (collection["bohuijinronguserid"] != null)
            {
                return long.Parse(HJDAPI.Common.Security.DES.Decrypt(collection["bohuijinronguserid"].Value, HJDAPI.Common.Security.DES.bohuijinrongDESKey));
            }
            return 0;
        }

        public static string GetBotaoPhoneNumFromCookie(HttpCookieCollection collection)
        {
            if (collection["bohuijinrong"] != null)
            {
                return HJDAPI.Common.Security.DES.Decrypt(collection["bohuijinrong"].Value, HJDAPI.Common.Security.DES.bohuijinrongDESKey);
            }
            return "";
        }

        //public static List<Pay.Models.BindCardEntity> GetEposBindList(string mobileId)
        //{
        //    var result = Pay.Adapter.YeePayAdapter.SendYeePayCommand(new Pay.Models.EPOSParam()
        //    {
        //        p0_Cmd = Pay.Models.EPOSCommendType.EposBindList,
        //        p1_MerId = null,
        //        pe_Identityid = mobileId,
        //        pe_Identitytype = "4",
        //        pe_cardType = ""
        //    });
        //    return result.re_BindList != null ? result.re_BindList : new List<Pay.Models.BindCardEntity>();
        //}
    }
}