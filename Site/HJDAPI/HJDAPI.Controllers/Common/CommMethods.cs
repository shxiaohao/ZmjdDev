using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Reflection;
//using System.ServiceModel.Channels;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace HJDAPI.Controllers.Common
{
    public class CommMethods
    {
        /// <summary>
        /// 将金额平分
        /// </summary>
        /// <param name="Amount"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public static  List<decimal> AvgAmount(decimal Amount, int num)
        {
            List<decimal> l = new List<decimal>();

            if (num == 1)
            {
                l.Add(Amount);
            }
            else
            {
                for (int i = 0; i < num - 1; i++)
                {
                    l.Add((int)(Amount / num));
                }
                l.Add(Amount - l.Sum());
            }

            return l;
        }

        //判断是否为手机号
        public static bool IsPhone(string phone)
        {
            if (phone.Length == 11)
            {
                if (phone.StartsWith("1"))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 根据支付类型判断是否发在支付完成后自动发短信给用户
        /// 支付方式为“分销商记账”或“分销商记帐-无短信”的订单，在退款时，不需要发短信。
        /// </summary>
        /// <param name="payType"></param>
        /// <returns></returns>
        public static bool CheckSendSMSByPayType(int payType)
        {
            return (payType == (int)HJD.HotelManagementCenter.Domain.PayType.Retailer ||
                payType == (int)HJD.HotelManagementCenter.Domain.PayType.Retailer_NoSMS ||
                payType == (int)HJD.HotelManagementCenter.Domain.PayType.RetailerCreditPay ||
                payType == (int)HJD.HotelManagementCenter.Domain.PayType.RetailerPreReceivePay) 
                ? false : true; //6	分销商计帐 or 30分销商计帐.无短信 不发短信    
        }

        //判断是否为周末
        public static bool IsWeekend(DateTime CheckIn)
        {
            return ((int)CheckIn.DayOfWeek) % 6 == 0;
        }

        public static bool IsFestival(DateTime CheckIn)
        {

            return false; //TODO: 判断是否为节假日
        }

        public static List<int> TranStrIDsToList(string IDs)
        {
            return IDs.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(_=> int.Parse(_)).ToList();
        }

        public static int GetNightCount(DateTime CheckIn, DateTime CheckOut)
        {
            TimeSpan ts1 = new TimeSpan(CheckIn.Date.Ticks);
            TimeSpan ts2 = new TimeSpan(CheckOut.Date.Ticks);
            TimeSpan ts = ts1.Subtract(ts2).Duration();
            return ts.Days;
        }

        public static DateTime GetDefaultCheckIn()
        {
            return DateTime.Now.AddDays(1).Date;
           // return DateTime.Now.AddDays(6 - (int)DateTime.Now.DayOfWeek).Date; //缺省取本周六，如果今天是周日，那取下周六
        }

        /// <summary>
        ///计算两点GPS坐标的距离
        /// </summary>
        /// <param name="lat1">第一点的纬度坐标</param>
        /// <param name="lon1">第一点的经度坐标</param>
        /// <param name="lat2">第二点的纬度坐标</param>
        /// <param name="lon2">第二点的经度坐标</param>
        /// <returns></returns>
        public static double CalcDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double jl_jd = 102834.74258026089786013677476285;
            double jl_wd = 111712.69150641055729984301412873;
            double b = Math.Abs((lon1 - lon2) * jl_jd);
            double a = Math.Abs((lat1 - lat2) * jl_wd);
            return Math.Sqrt((a * a + b * b));

        }

        public static string TransEnCodeHTML2HTML(string str)
        {
            return str.Replace("&gt;", ">").Replace("&lt;", "<").Replace("&quot;","\"");
        }

        public static DateTime TransSpecialDateFormat2StandardDate(string dateTimeStr)
        {
            if(string.IsNullOrWhiteSpace(dateTimeStr) || dateTimeStr == "0"){
                return DateTime.Now.Date;
            }
            else
            {
                DateTime date = DateTime.MinValue;
                try
                {
                    DateTime.TryParse(dateTimeStr, out date);
                    if (date == null || date == DateTime.MinValue)
                    {
                        string splitStr = "-,—,/";
                        string[] strArray = dateTimeStr.Split(splitStr.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        if (strArray.Length == 3)
                        {
                            date = DateTime.Parse(string.Format("{0}/{1}/{2}", strArray[2], strArray[1], strArray[0]));//dd-MM-yyyy转成yyyy-MM-dd
                        }
                    }
                    return date;
                }
                catch (Exception ex)
                {
                    Log.WriteLog("错误的日期转换,收到的日期字符串为:" + dateTimeStr + ex.Message + ex.StackTrace);
                    return DateTime.MinValue;
                }
            }
        }
        
        /// </summary>
        /// <param name="appType"></param>
        /// <returns></returns>
        public static int GetTerminalId(string appType)
        {
            if(string.IsNullOrWhiteSpace(appType)){
                return 1;
            }

            if(appType.ToLower().Contains("ios")){
                return 2;
            }
            else if (appType.ToLower().Contains("android"))
            {
                return 3;
            }
            return 0;
        }

        /// <summary>  
        /// 时间戳转为C#格式时间  
        /// </summary>  
        /// <param name="timeStamp">Unix时间戳格式</param>  
        /// <returns>C#格式时间</returns>  
        public static DateTime GetTime(long timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));

            return dtStart.AddMilliseconds(timeStamp);
            //long lTime = timeStamp * 10000000;
            //TimeSpan toNow = new TimeSpan(lTime);
            //return dtStart.Add(toNow);
        }  

        /// <summary>
        /// 是否需要修改昵称 昵称是默认格式  133****7106
        /// </summary>
        /// <param name="oldNickName"></param>
        /// <returns></returns>
        public static bool NeedModifyNickName(string oldNickName)
        {
            var reg = new Regex("1\\d{2}[*]{4}\\d{4}", RegexOptions.Compiled);
            return reg.Match(oldNickName).Success;
        }

        public static string GenShortenUrl(string toUrl)
        {
            var url = "http://dm12.me/GenShortenUrl/" + toUrl;
            string result = HttpHelper.Get(url, "utf-8");
            var shortUrl = "http://dm12.me/" + result;
            return shortUrl;
        }

        /// <summary>
        /// Unix时间戳转换为DateTime
        /// </summary>
        /// <param name="timestamp">时间戳</param>
        /// <returns>日期</returns>
        public static DateTime GetDateTime(long timestamp)
        {
            System.DateTime time = System.DateTime.MinValue;
            DateTime start = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            try
            {
                time = timestamp.ToString().Length == 10 ? start.AddSeconds(timestamp) : start.AddMilliseconds(timestamp);
            }
            catch (Exception ex)
            {
                return start;
            }
            return time;
        }
        /// <summary>
        /// DateTime转换为Unix时间戳
        /// </summary>
        /// <param name="dt">日期</param>
        /// <returns>时间戳</returns>
        public static long GetTimestamp(DateTime dt)
        {
            double intResult = 0;
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            intResult = (dt - startTime).TotalMilliseconds;
            return Convert.ToInt64(Math.Round(intResult, 0));
        }
    }
}