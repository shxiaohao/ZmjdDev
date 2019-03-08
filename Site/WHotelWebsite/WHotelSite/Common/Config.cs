using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WHotelSite.Common
{
    public class Config
    {
        public static string magicalPreUserName = "zmjdappuser";
        public static string magicalPreUserPassword = "zmjdappuserpwd";

        public static string KefuHeadPhoto
        {
            get 
            {
                return System.Configuration.ConfigurationManager.AppSettings["kefuHeadPhoto"];
            }
        }

        public static string AvatarSize
        {
            get 
            {
                return System.Configuration.ConfigurationManager.AppSettings["AvatarSize"];
            }
        }

        private static string mWebSiteUrl = "";
        
        public static string WebSiteUrl
        {
            get
            {
                if( mWebSiteUrl.Length == 0)
                { 
                    mWebSiteUrl = (string)System.Configuration.ConfigurationManager.AppSettings["WebSiteUrl"];
                }
                return mWebSiteUrl;
            }
        }

        public static string chinaPayUrl
        {
            get
            {
               return System.Configuration.ConfigurationManager.AppSettings["chinaPayUrl"]; 
            }
        } 
        
        public static string cmbPayUrl
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["cmbPayUrl"]; 
            }
        }

        private static string mAccess_Control_Allow_Origin_URL = "";
        public static string Access_Control_Allow_Origin_URL
        {
            get
            {
                if (mAccess_Control_Allow_Origin_URL.Length == 0)
                {
                    mAccess_Control_Allow_Origin_URL = WebSiteUrl.TrimEnd('/');

                }
                return mAccess_Control_Allow_Origin_URL;
            }
        }


        public static string MD5KeyForOldOrder  //不清楚这个是如果用的，所以将MD5Key改成MD5KeyForOldOrder 再观察
        {
            get
            {
                if(System.Configuration.ConfigurationManager.AppSettings["MD5Key"] == null )
                {
                    return "WHMD5Key";
                }
                else
                {
                    return System.Configuration.ConfigurationManager.AppSettings["MD5Key"];
                }
            }
        }

        public static string MD5Key  
        {
            get
            {
                    return System.Configuration.ConfigurationManager.AppSettings["MD5Key"];
            }
        }


        public static string TenPay_pre_pay_url
        {
            get
            {
                if (System.Configuration.ConfigurationManager.AppSettings["TenPay_pre_pay_url"] == null)
                {
                    return "http://tenpay.zmjiudian.com/api/tenpay/pay_req_forService";
                }
                else
                {
                    return System.Configuration.ConfigurationManager.AppSettings["TenPay_pre_pay_url"];
                }
            }
        }

        public static string DefaultPayChannelList
        {
            get
            {
                if (System.Configuration.ConfigurationManager.AppSettings["DefaultPayChannelList"] == null)
                {
                    return "tenpay,alipay,chinapay";
                }
                else
                {
                    return System.Configuration.ConfigurationManager.AppSettings["DefaultPayChannelList"];
                }
            }
        }

        public static string APISiteUrl
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["APISiteUrl"];
            }
        }
    }
}