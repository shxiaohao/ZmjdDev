using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HJDAPI.Common.Helpers
{
    public static class StringHelper
    {
        public static string UidMask(string uid)
        {
            if (uid == null || uid =="")
            {
                return string.Empty;
            }

            uid = uid.Trim();
            //首先匹配信用卡帐号
            if (uid.Length >= 16)
            {
                if (Regex.IsMatch(uid, @"^\w*[0-9]{16}$"))
                {
                    return uid.Substring(0, uid.Length - 4) + "****";
                }
            }

            //匹配手机以及其他手机用户（后面11位是手机号的）
            if (uid.Length >= 11)
            {
                string sUidT = uid.Substring(uid.Length - 11, 11);
                if (Regex.IsMatch(sUidT, @"^13[0-9]{9}$|^15[0-9]{9}$|^18[0-9]{9}$"))
                {
                    return uid.Substring(0, uid.Length - 8) + "****" + uid.Substring(7);
                }
            }
            return uid;
        }

        /// <summary>
        /// 获取处理过的链接
        /// </summary>
        /// <param name="commentReview"></param>
        /// <returns></returns>
        public static string GetCleanCommentReview(string commentReview)
        {
            Regex reg = new Regex("", RegexOptions.IgnoreCase|RegexOptions.Compiled|RegexOptions.Multiline);
            
            reg.Replace(commentReview, "");
            
            //MatchCollection mc = reg.Matches(commentReview,0);
            //if (mc != null && mc.Count > 0)
            //{
            //    mc[0].
            //    foreach (var item in mc)
            //    {
            //        item.
            //    }
            //}
            return "";
        }

        public static string SetHighlight(bool needHighlight,   string highlightString, string defaultString)
        {
            return (needHighlight == true && highlightString.Length > 0 ) ?
                highlightString :
                defaultString;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static int TransAppTypeHeaderToAppType(string appTypeHeader)
        {
            if (string.IsNullOrWhiteSpace(appTypeHeader) || new Regex("web", RegexOptions.IgnoreCase).IsMatch(appTypeHeader, 0))
            {
                return 3;//代表web
            }
            else if(new Regex("iOS",RegexOptions.IgnoreCase).IsMatch(appTypeHeader,0)){
                return 1;//iOS
            }
            else if (new Regex("Android", RegexOptions.IgnoreCase).IsMatch(appTypeHeader, 0))
            {
                return 2;//Android
            }
            else{
                return 0;//未知渠道
            }
        }

       public static string FormatMoney(decimal money)
        {
           return  (money * 100 == ((int)money) * 100)?((int)money).ToString() : money.ToString();
        }

        /// <summary>
        /// 将字符串转换成字典，字符串格式如下：
        /// 电话:52728385  
        /// 地址：普陀区天地软件园
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ParseStringToDic(string str)
        {
            Dictionary<string, string> dicProperties = new Dictionary<string, string>();

            if (str != null && str.Length > 0)
            {
                foreach (var item in str.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                {
                    string[] list = item.Replace("：",":").Split(':');
                    if (list.Length == 2)
                    {
                        if (!dicProperties.ContainsKey(list[0]))
                        {
                            dicProperties.Add(list[0], list[1]);
                        }
                        else
                        {
                            dicProperties[list[0]] = dicProperties[list[0]] + "   \r\n  "+ list[1];
                        }
                    }
                }
            }
            return dicProperties;
        }
    }
}