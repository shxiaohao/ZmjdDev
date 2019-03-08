using CommLib;
using HJD.AccessService.Contract.Model;
using HJD.AccessService.Contract.Params;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HJD.AccessService.Implement.Helper
{
    public class BehaviorHelper
    {
        public static string FormatBehaviorString(Behavior behavior)
        {
            var formatStr = "{ID} | {Page} | {Code} | {Event} | {Value} | {UserId} | {Phone} | {AppKey} | {ClientType} | {AppVer} | {IP} | {RecordLayer} | {RecordTime}";

            try
            {
                formatStr = formatStr
                .Replace("{ID}", behavior.ID.ToString())
                .Replace("{Page}", behavior.Page)
                .Replace("{Code}", behavior.Code)
                .Replace("{Event}", behavior.Event)
                .Replace("{Value}", behavior.Value)
                .Replace("{UserId}", behavior.UserId.ToString())
                .Replace("{Phone}", behavior.Phone ?? "")
                .Replace("{AppKey}", behavior.AppKey ?? "")
                .Replace("{ClientType}", behavior.ClientType)
                .Replace("{AppVer}", behavior.AppVer ?? "")
                .Replace("{IP}", behavior.IP ?? "")
                .Replace("{RecordLayer}", behavior.RecordLayer ?? "")
                .Replace("{RecordTime}", behavior.RecordTime.ToString("yyyy-MM-dd HH:mm:ss"));

                //if (behavior.Code.Contains("H5Home"))
                //{
                //    System.IO.File.AppendAllText(string.Format(@"D:\Log\AccessService\AccessLog_{0}.txt", DateTime.Now.ToString("MM_dd")), formatStr);
                //}
            }
            catch (Exception ex)
            {
                //System.IO.File.AppendAllText(string.Format(@"D:\Log\AccessService\AccessLog_{0}.txt", DateTime.Now.ToString("MM_dd")), ex.Message);
            }

            //formatStr = string.Format(formatStr
            //    , behavior.ID
            //    , behavior.Page
            //    , behavior.Code
            //    , behavior.Event
            //    , behavior.Value
            //    , behavior.AppKey
            //    , behavior.ClientType
            //    , behavior.RecordTime);

            return formatStr;
        }

        public static Behavior GetBehaviorByString(string behaviorString)
        {
            var behavior = new Behavior();

            if (!string.IsNullOrEmpty(behaviorString))
            {
                try
                {
                    var bList = Regex.Split(behaviorString, " \\| ");
                    behavior.ID = new Guid(bList[0]);
                    behavior.Page = bList[1];
                    behavior.Code = bList[2];
                    behavior.Event = bList[3];
                    behavior.Value = bList[4];
                    behavior.UserId = Convert.ToInt32(bList[5]);
                    behavior.Phone = bList[6];
                    behavior.AppKey = bList[7];
                    behavior.ClientType = bList[8];
                    behavior.AppVer = bList[9];
                    behavior.IP = bList[10];
                    behavior.RecordLayer = bList[11];
                    behavior.RecordTime = DateTime.Parse(bList[12]);
                }
                catch (Exception ex)
                {
                    behavior.Code = "读取错误";
                    behavior.Value = ex.Message;
                }
            }

            return behavior;
        }

        #region 数据分析

        public static void Who()
        { }

        public static void When()
        { }

        public static void Where()
        { }

        public static void What()
        { }

        public static void Why()
        { }

        public static void HowMuch()
        { }

        public static void How()
        { }

        public static void Report()
        { 
            
        }

        public static void ConvertValue()
        { 
            
        }

        #endregion

        #region 注释代码

        //        /*
//xxxxxx-xxxxx-xxxxx-00001　Hotel　{"HotelID":"188660"}　搜索　{"Keywords":"苏州 金鸡湖"}　Hotel　{"HotelID":"318109"}　xxxxxx-xxxxxx-xxxxx-11111　Android/IOS/www　2015-05-21 15:00:01.0001
//xxxxxx-xxxxx-xxxxx-00002　Hotel　{"HotelID":"318109"}　页面加载　{}　{}　{}　xxxxxx-xxxxxx-xxxxx-11111　Android/IOS/www　2015-05-21 15:00:03.0001
//xxxxxx-xxxxx-xxxxx-00003　Hotel　{"HotelID":"318109"}　特惠专享　{}　Package　{"HotelID":"318109","CheckIn":"2015-05-22","CheckOut":"2015-05-23"}　xxxxxx-xxxxxx-xxxxx-11111　Android/IOS/www　2015-05-21 15:05:01.0001
//xxxxxx-xxxxx-xxxxx-00004　Package　{"HotelID":"318109","CheckIn":"2015-05-22","CheckOut":"2015-05-23"}　页面加载　{}　{}　{}　xxxxxx-xxxxxx-xxxxx-11111　Android/IOS/www　2015-05-21 15:05:03.0001
//xxxxxx-xxxxx-xxxxx-00005　Package　{"HotelID":"318109","CheckIn":"2015-05-22","CheckOut":"2015-05-23"}　购买　{}　Book　{"HotelID":"318109","Package":"931","CheckIn":"2015-05-22","CheckOut":"2015-05-23"}　xxxxxx-xxxxxx-xxxxx-11111　Android/IOS/www　2015-05-21 15:10:01.0001
// */

//        /// <summary>
//        /// 将行为对象格式化为文本字符串
//        /// </summary>
//        /// <param name="behavior">行文对象</param>
//        /// <returns></returns>
//        public static string FormatBehaviorString(Behavior behavior)
//        {
//            //xxxxxx-xxxxx-xxxxx-00001　Index　{}　搜索　{"Keywords":"苏州 金鸡湖"}　Hotel　{"HotelID":"318109"}　xxxxxx-xxxxxx-xxxxx-11111　Android/IOS/www　2015-05-21 15:00:01.0001

//            var formatStr = "{0}　{1}　{2}　{3}　{4}　{5}　{6}　{7}　{8}　{9}";
//            formatStr = string.Format(formatStr
//                , behavior.ID
//                , behavior.PageCode
//                , behavior.PageValue
//                , behavior.EventCode
//                , behavior.EventValue
//                , behavior.TargetCode
//                , behavior.TargetValue
//                , behavior.AppKey
//                , behavior.ClientType
//                , behavior.RecordTime);

//            return formatStr;
//        }

//        public static Behavior ConvertBehaviorIndexSearchHotel(BehaviorIndexSearchHotel behaviorParams)
//        {
//            var behavior = new Behavior { 
//                ID = behaviorParams.ID,
//                PageCode = "Index",
//                PageValue = "{}",
//                EventCode = "搜索",
//                EventValue = string.Format("{{\"Keywords\":\"{0}\"}}", behaviorParams.Keywords),
//                TargetCode = "Hotel",
//                TargetValue = string.Format("{{\"HotelID\":\"{0}\"}}", behaviorParams.SearchHotelID),
//                AppKey = behaviorParams.AppKey,
//                ClientType = behaviorParams.ClientType,
//                RecordTime = behaviorParams.RecordTime
//            };

//            return behavior;
//        }

//        public static Behavior ConvertBehaviorZoneSearchHotel(BehaviorZoneSearchHotel behaviorParams)
//        {
//            var behavior = new Behavior
//            {
//                ID = behaviorParams.ID,
//                PageCode = "Zone",
//                PageValue = string.Format("{{\"Zone\":\"{0}\",\"Theme\":\"{1}\",\"Sight\":\"{2}\",\"Star\":\"{3}\",\"Price\":\"{4}\"}}",
//                            behaviorParams.Zone, behaviorParams.Theme, behaviorParams.Sight, behaviorParams.Star, behaviorParams.Price),
//                EventCode = "搜索",
//                EventValue = string.Format("{{\"Keywords\":\"{0}\"}}", behaviorParams.Keywords),
//                TargetCode = "Hotel",
//                TargetValue = string.Format("{{\"HotelID\":\"{0}\"}}", behaviorParams.SearchHotelID),
//                AppKey = behaviorParams.AppKey,
//                ClientType = behaviorParams.ClientType,
//                RecordTime = behaviorParams.RecordTime
//            };

//            return behavior;
//        }

//        public static Behavior ConvertBehaviorHotelSearchHotel(BehaviorHotelSearchHotel behaviorParams)
//        {
//            var behavior = new Behavior
//            {
//                ID = behaviorParams.ID,
//                PageCode = "Hotel",
//                PageValue = string.Format("{{\"HotelID\":\"{0}\"}}", behaviorParams.HotelID),
//                EventCode = "搜索",
//                EventValue = string.Format("{{\"Keywords\":\"{0}\"}}", behaviorParams.Keywords),
//                TargetCode = "Hotel",
//                TargetValue = string.Format("{{\"HotelID\":\"{0}\"}}", behaviorParams.SearchHotelID),
//                AppKey = behaviorParams.AppKey,
//                ClientType = behaviorParams.ClientType,
//                RecordTime = behaviorParams.RecordTime
//            };

//            return behavior;
//        }

//        public static Behavior ConvertBehaviorPackageSearchHotel(BehaviorPackageSearchHotel behaviorParams)
//        {
//            var behavior = new Behavior
//            {
//                ID = behaviorParams.ID,
//                PageCode = "Package",
//                PageValue = string.Format("{{\"HotelID\":\"{0}\",\"CheckIn\":\"{1}\",\"CheckOut\":\"{2}\"}}",
//                            behaviorParams.HotelID, behaviorParams.CheckIn, behaviorParams.CheckOut),
//                EventCode = "搜索",
//                EventValue = string.Format("{{\"Keywords\":\"{0}\"}}", behaviorParams.Keywords),
//                TargetCode = "Hotel",
//                TargetValue = string.Format("{{\"HotelID\":\"{0}\"}}", behaviorParams.SearchHotelID),
//                AppKey = behaviorParams.AppKey,
//                ClientType = behaviorParams.ClientType,
//                RecordTime = behaviorParams.RecordTime
//            };

//            return behavior;
//        }

//        public static Behavior ConvertBehaviorBookSearchHotel(BehaviorBookSearchHotel behaviorParams)
//        {
//            var behavior = new Behavior
//            {
//                ID = behaviorParams.ID,
//                PageCode = "Book",
//                PageValue = string.Format("{{\"HotelID\":\"{0}\",\"Package\":\"{1}\",\"CheckIn\":\"{2}\",\"CheckOut\":\"{3}\"}}",
//                            behaviorParams.HotelID, behaviorParams.Package, behaviorParams.CheckIn, behaviorParams.CheckOut),
//                EventCode = "搜索",
//                EventValue = string.Format("{{\"Keywords\":\"{0}\"}}", behaviorParams.Keywords),
//                TargetCode = "Hotel",
//                TargetValue = string.Format("{{\"HotelID\":\"{0}\"}}", behaviorParams.SearchHotelID),
//                AppKey = behaviorParams.AppKey,
//                ClientType = behaviorParams.ClientType,
//                RecordTime = behaviorParams.RecordTime
//            };

//            return behavior;
//        }

        #endregion
    }
}
