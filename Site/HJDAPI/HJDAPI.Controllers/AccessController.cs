using HJD.AccessService.Contract;
using HJD.AccessService.Contract.Model;
using HJD.AccessService.Contract.Model.Dialog;
using HJD.Framework.Interface;
using HJD.Framework.WCF;
using HJD.HotelManagementCenter.IServices;
using HJDAPI.Common.Helpers;
using HJDAPI.Controllers.Activity;
using HJDAPI.Controllers.Adapter;
using HJDAPI.Controllers.Common;
using HJDAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml;

namespace HJDAPI.Controllers
{
    public class AccessController : BaseApiController
    {
        public static ICacheProvider LocalCache5Min = CacheManagerFactory.Create("DynamicCacheFor5Min");
        public static IAccessService AccessService = ServiceProxyFactory.Create<IAccessService>("IAccessService");
        public static ISMSService SMSService = ServiceProxyFactory.Create<ISMSService>("ISMSService");
        public static IFundService FundService = ServiceProxyFactory.Create<IFundService>("IFundService");

        [System.Web.Http.HttpGet]
        public string GenShortUrlWithDM12(string longURL)
        {
            return AccessAdapter.GenShortUrlWithDM12(longURL);
        }


        [HttpPost]
        public int RecordBehaviorQueue(List<Behavior> behaviorQueue)
        {
            new AccessAdapter().RecordBehaviorQueue(behaviorQueue);
            return 1;
        }

        [HttpPost]
        public int RecordBehavior(Behavior behavior)
        {
            try
            {
                //检测SessionId和IP地址
                if (behavior != null)
                {
                    if (behavior.ID == null)
                    {
                        behavior.ID = new Guid();
                    }

                    if (string.IsNullOrEmpty(behavior.AppKey))
                    {
                        behavior.AppKey = (HttpContext.Current != null && HttpContext.Current.Session != null && !string.IsNullOrEmpty(HttpContext.Current.Session.SessionID)) ? HttpContext.Current.Session.SessionID : "";
                    }

                    if (string.IsNullOrEmpty(behavior.IP))
                    {
                        behavior.IP = HttpContext.Current.Request.UserHostAddress;
                    }
                }
            }
            catch (Exception ex) { }

            System.IO.File.AppendAllText(string.Format(@"D:\Log\API\ApiLog_{0}.txt", DateTime.Now.ToString("MM_dd")), behavior.Code + " & " + behavior.Value + " & " + behavior.RecordLayer + " & ");

            return new AccessAdapter().RecordBehaviorQueue(new List<Behavior> { behavior });
        }

        [System.Web.Http.HttpGet]
        public List<HotelSearchResult> SearchHotel(string keywords, int limitCount)
        {
            return AccessService.SearchHotel(keywords, limitCount);
        }

        [System.Web.Http.HttpGet]
        public bool SendSMS(string phone, string sms)
        {
            //Log.WriteLog(string.Format("API SendSMS:{0} {1} {2} {3} {4}", phone, sms, Request.RequestUri, Request.Properties["http_X_FORWARDED_FOR"]));
            return SMServiceController.SendSMS(phone.Trim(), sms);
            //return SMSService.SendSMS(phone, sms);
        }
        
        [System.Web.Http.HttpGet]
        public bool AddPriceSlot(int hotelId)
        {
            AccessService.AddPriceSlot(hotelId);

            return true;
        }

        #region Qa 搜索

        [System.Web.Http.HttpGet]
        public QaSearchResult QaSearchHotel(string keywords, int limitCount, string checkIn = "", string checkOut = "", int minPrice = -1, int maxPrice = -1)
        {
            return AccessService.QaSearchHotel(keywords, limitCount, checkIn, checkOut, minPrice, maxPrice);
        }

        #endregion

        #region Fund(用户奖励基金相关操作)

        [System.Web.Http.HttpGet]
        public int DeductUserFund(Int64 userId, long orderId, decimal fund)
        {
            return FundService.DeductUserFund(userId, orderId, fund);
        }

        [System.Web.Http.HttpGet]
        public int ReturnUserFund(Int64 userId, long orderId, decimal fund)
        {
            return FundService.ReturnUserFund(userId, orderId, fund);
        }

        #endregion

        #region 短链接生成

        [System.Web.Http.HttpGet]
        public string GenShortUrl(int t, string url)
        {
            return AccessAdapter.GenShortUrl(t, url);
        }

        #endregion

        #region 验证码图片生成
        /// <summary>
        /// 1.生成验证码
        /// 2.存到缓存中 设置有效期
        /// 返回验证码base64字符串
        /// </summary>
        /// <param name="identityId">可以是手机号、sessionId、userId等对同一个用户当前唯一的标识</param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public ValidationResult GenValidationCodeBase64Str(string identityId)
        {
            var result = new ValidationResult();

            var validationCode = "";
            var base64Str = ValidationImg.GenValidationImg(out validationCode);

            if (!string.IsNullOrWhiteSpace(base64Str))
            {
                //记录验证码到memcached中 过期时间1min钟 key为手机号 value为验证码加到期时间的组合
                //如果手机号找不到缓存过期 则在验证的时候需要重新获取验证码;缓存没过期但是验证码已经到过期时间 仍然要重新获取验证码;
                CouponAdapter.SetCache(string.Format("Validation_{0}", identityId), string.Format("{0},{1}", validationCode, HJDAPI.Common.Security.Signature.GenTimeStamp() + 63));
                result = new ValidationResult()
                {
                    base64Url = base64Str,
                    data = "",
                    errorcode = "0",
                    errormsg = "success",
                    result = true
                };
            }
            else
            {
                result = new ValidationResult()
                {
                    base64Url = "",
                    data = "",
                    errorcode = "1001",
                    errormsg = "please regenerate validationCode",
                    result = false
                };
            }

            return result;
        }

        /// <summary>
        /// 3. 验证 3.1 过了有效期或者不正确 返回新的验证码 存到缓存  3.2 正确的 返回结果 
        /// </summary>
        /// <param name="identityId">可以是手机号、sessionId、userId等对同一个用户当前唯一的标识</param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public ValidationResult VerifyValidationCode(string identityId, string validationCode)
        {
            var result = new ValidationResult();
            var cacheValue = CouponAdapter.GetCache(string.Format("Validation_{0}", identityId));
            if (string.IsNullOrWhiteSpace(validationCode))
            {
                result = new ValidationResult()
                {
                    base64Url = "",
                    data = "",
                    errorcode = "1002",
                    errormsg = "fail no input validationCode",
                    result = false
                };
            }
            else if (string.IsNullOrWhiteSpace(cacheValue))
            {
                result = new ValidationResult()
                {
                    base64Url = "",
                    data = "",
                    errorcode = "1003",
                    errormsg = "fail no cache validationCode",
                    result = false
                };
            }
            else
            {
                var valueStrs = cacheValue.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (HJDAPI.Common.Security.Signature.GenTimeStamp() <= long.Parse(valueStrs[1]) && valueStrs[0].ToLower().Equals(validationCode.ToLower()))
                {
                    result = new ValidationResult()
                    {
                        base64Url = "",
                        data = "",
                        errorcode = "0",
                        errormsg = "success",
                        result = true
                    };
                }
                else{
                    result = new ValidationResult()
                    {
                        base64Url = "",
                        data = "",
                        errorcode = "1004",
                        errormsg = "fail has expiration or wrong validationCode",
                        result = true
                    };
                }
            }

            //验证失败 自动生成下一次需要验证的验证码
            if (result.errorcode != "0")
            {
                var newValidationCode = "";
                var base64Str = ValidationImg.GenValidationImg(out newValidationCode);
                result.base64Url = base64Str;
                CouponAdapter.SetCache(string.Format("Validation_{0}", identityId), string.Format("{0},{1}", newValidationCode, HJDAPI.Common.Security.Signature.GenTimeStamp() + 63));
            }

            return result;
        }

        #endregion
    }
}