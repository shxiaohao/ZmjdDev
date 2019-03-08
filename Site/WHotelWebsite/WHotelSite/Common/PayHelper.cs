using HJDAPI.APIProxy;
using HJDAPI.Common.Security;
using HJDAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WHotelSite.Common
{
    public class PayHelper
    {
        public static string GenFreePayUrl(long OrderID, int price, int CID)
        {
            Int64 url_TimeStamp = Signature.GenTimeStamp();
            string testpayUrl = System.Configuration.ConfigurationManager.AppSettings["testpayUrl"];
            int url_CID = CID;
            string url_RequestType = String.Format("{0}Pay/FreePayFinish?orderid={1}&TimeStamp={2}&CID={3}&Price={4}",
        testpayUrl, OrderID, url_TimeStamp, url_CID, price
         );
            string MD5Key = Config.MD5Key;
            string Sign = Signature.GenSignature(url_TimeStamp, url_CID, MD5Key, url_RequestType);
            string UserInfoUrl = String.Format("{0}&sign={1}",
                url_RequestType, Sign
                 );

            return UserInfoUrl;
        }

        public static string GenPointPayUrl(long OrderID, int price, int CID)
        {
            Int64 url_TimeStamp = Signature.GenTimeStamp();
            string testpayUrl = System.Configuration.ConfigurationManager.AppSettings["testpayUrl"];
            int url_CID = CID;
            string url_RequestType = String.Format("{0}Pay/PointPayFinish?orderid={1}&TimeStamp={2}&CID={3}&Price={4}",
        testpayUrl, OrderID, url_TimeStamp, url_CID, price
         );
            string MD5Key = Config.MD5Key;
            string Sign = Signature.GenSignature(url_TimeStamp, url_CID, MD5Key, url_RequestType);
            string UserInfoUrl = String.Format("{0}&sign={1}",
                url_RequestType, Sign
                 );

            return UserInfoUrl;
        }




        public static CheckOrderBeforePayResponse CheckOrderBeforePay(long orderId)
        { 
            CheckOrderBeforePayRequestParams p = new CheckOrderBeforePayRequestParams();
            p.OrderID = orderId;
            p.TimeStamp = (long)Math.Round((DateTime.Now - new DateTime(1971, 1, 1, 8, 0, 0)).TotalSeconds, 0);
            p.SourceID = 3;
            p.RequestType = "CheckOrderBeforePay";
            p.Sign = HJDAPI.Common.Security.Signature.GenSignature(p.TimeStamp, p.SourceID, WHotelSite.Common.Config.MD5Key, p.RequestType);
            p.AppVer = "4.2";
            p.UserID = UserState.UserID;
            CheckOrderBeforePayResponse response = new Order().CheckOrderBeforePay(p);

            return response;
        }



    }
}