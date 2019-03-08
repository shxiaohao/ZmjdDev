using HJD.AccessService.Contract.Model;
using HJD.AccessService.Contract.Model.Dialog;
using HJDAPI.Common.Helpers;
using HJDAPI.Controllers;
using HJDAPI.Controllers.Adapter;
using HJDAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.APIProxy
{
    public class Access : BaseProxy
    {
      

        /// <summary>
        /// 记录单条行为日志
        /// </summary>
        /// <param name="behavior"></param>
        /// <returns></returns>
        public int RecordBehavior(Behavior behavior)
        {
            if (IsProductEvn)
            {
                return new AccessAdapter().RecordBehaviorQueue(new List<Behavior> { behavior });
            }
            else
            {
                string url = APISiteUrl + "api/Access/RecordBehavior";
                CookieContainer cc = new CookieContainer();

                string json = HttpRequestHelper.PostJson(url, behavior, ref cc);
                return JsonConvert.DeserializeObject<int>(json);
            }
        }

        /// <summary>
        /// 批量记录行为日志
        /// </summary>
        /// <param name="behaviorQueue"></param>
        /// <returns></returns>
        public int RecordBehaviorQueue(List<Behavior> behaviorQueue)
        {
            if (IsProductEvn)
            {
                return new AccessAdapter().RecordBehaviorQueue(behaviorQueue);
            }
            else
            {
                string url = APISiteUrl + "api/Access/RecordBehaviorQueue";
                CookieContainer cc = new CookieContainer();

                string json = HttpRequestHelper.PostJson(url, behaviorQueue, ref cc);
                return JsonConvert.DeserializeObject<int>(json);
            }
        }

        public List<HotelSearchResult> SearchHotel(string keywords, int limitCount)
        {
            string url = APISiteUrl + "api/Access/SearchHotel";
            string postDataStr = string.Format("keywords={0}&limitCount={1}", keywords, limitCount);
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<List<HotelSearchResult>>(json);
        }

        public bool SendSMS(string phone, string sms)
        {
            string url = APISiteUrl + "api/Access/SendSMS";
            string postDataStr = string.Format("phone={0}&sms={1}", phone, sms);
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<bool>(json);
        }

        public bool AddPriceSlot(int hotelId)
        {
            string url = APISiteUrl + "api/Access/AddPriceSlot";
            string postDataStr = string.Format("hotelId={0}", hotelId);
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<bool>(json);
        }

        #region Qa 搜索

        public QaSearchResult QaSearchHotel(string keywords, int limitCount)
        {
            string url = APISiteUrl + "api/Access/QaSearchHotel";
            string postDataStr = string.Format("keywords={0}&limitCount={1}", keywords, limitCount);
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<QaSearchResult>(json);
        }

        #endregion

        #region 短链接生成

        public string GenShortUrl(int t, string url)
        {
            string apiUrl = APISiteUrl + "api/Access/GenShortUrl";
            string postDataStr = string.Format("t={0}&url={1}", t, url);
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(apiUrl, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<string>(json);
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
        public ValidationResult GenValidationCodeBase64Str(string identityId)
        {
            string url = APISiteUrl + "api/Access/GenValidationCodeBase64Str";
            string postDataStr = string.Format("identityId={0}", identityId);
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<ValidationResult>(json);
        }

        /// <summary>
        /// 3. 验证 3.1 过了有效期或者不正确 返回新的验证码 存到缓存  3.2 正确的 返回结果 
        /// </summary>
        /// <param name="identityId">可以是手机号、sessionId、userId等对同一个用户当前唯一的标识</param>
        /// <returns></returns>
        public ValidationResult VerifyValidationCode(string identityId, string validationCode)
        {
            string url = APISiteUrl + "api/Access/VerifyValidationCode";
            string postDataStr = string.Format("identityId={0}&validationCode={1}", identityId, validationCode);
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<ValidationResult>(json);
        }

        #endregion
    }
}
