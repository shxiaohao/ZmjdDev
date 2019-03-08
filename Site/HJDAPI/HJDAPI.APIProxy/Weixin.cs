using HJD.AccessService.Contract.Model;
using HJD.AccessService.Contract.Params;
using HJD.HotelManagementCenter.Domain.Comm;
using HJD.PhotoServices.Contracts;
using HJD.WeixinService.Contract;
using HJD.WeixinServices.Contracts;
using HJDAPI.Common.Helpers;
using HJDAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HJDAPI.APIProxy
{
    public class Weixin : BaseProxy
    {
        public WeixinConfig GetWeixinConfig(string url2)
        {
            string url = APISiteUrl + "api/Coupon/GetWeixinConfig";
            CookieContainer cc = new CookieContainer();
            WeixinConfig config2 = new WeixinConfig() { debug = !IsProductEvn, url = url2 };
            string json = HttpRequestHelper.PostJson(url, config2, ref cc);
            return JsonConvert.DeserializeObject<WeixinConfig>(json);
        }

        public WeixinConfig GetWeixinConfigSly(string url2)
        {
            string url = APISiteUrl + "api/Coupon/GetWeixinConfigSly";
            CookieContainer cc = new CookieContainer();
            WeixinConfig config2 = new WeixinConfig() { debug = !IsProductEvn, url = url2 };
            string json = HttpRequestHelper.PostJson(url, config2, ref cc);
            return JsonConvert.DeserializeObject<WeixinConfig>(json);
        }

        public WeixinConfig GetWeixinConfigSlycd(string url2)
        {
            string url = APISiteUrl + "api/Coupon/GetWeixinConfigSlycd";
            CookieContainer cc = new CookieContainer();
            WeixinConfig config2 = new WeixinConfig() { debug = !IsProductEvn, url = url2 };
            string json = HttpRequestHelper.PostJson(url, config2, ref cc);
            return JsonConvert.DeserializeObject<WeixinConfig>(json);
        }

        public WeixinConfig GetWeixinConfigMwzs(string url2)
        {
            string url = APISiteUrl + "api/Coupon/GetWeixinConfigMwzs";
            CookieContainer cc = new CookieContainer();
            WeixinConfig config2 = new WeixinConfig() { debug = !IsProductEvn, url = url2 };
            string json = HttpRequestHelper.PostJson(url, config2, ref cc);
            return JsonConvert.DeserializeObject<WeixinConfig>(json);
        }

        public WeixinChatRecordResult GetWeixinChatRecord(WeixinChatRecordParams rp)
        {
            string url = APISiteUrl + "api/WeixinApi/GetWeixinChatRecord";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, rp, ref cc);
            return JsonConvert.DeserializeObject<WeixinChatRecordResult>(json);
        }

        public static bool IsPartWeixinActivity(int acode, string phone)
        {
            string url = APISiteUrl + "api/WeixinApi/IsPartWeixinActivity";
            string postDataStr = string.Format("acode={0}&phone={1}", acode, phone);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<bool>(json);
        }

        public static List<KeyValueEntity> GetWeiXinChannelCode()
        {
            string url = APISiteUrl + "api/WeixinApi/GetWeiXinChannelCode";
            string postDataStr = string.Format("");

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<List<KeyValueEntity>>(json);
        }

        #region 微信h5

        public List<WeixinActivityEntity> GetWeixinActives()
        {
            string url = APISiteUrl + "api/WeixinApi/GetWeixinActives";
            string postDataStr = "";

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<List<WeixinActivityEntity>>(json);
        }

        public WeixinActivityEntity GetOneWeixinActive(int activeid)
        {
            string url = APISiteUrl + "api/WeixinApi/GetOneWeixinActive";
            string postDataStr = string.Format("activeid={0}", activeid);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<WeixinActivityEntity>(json);
        }

        /// <summary>
        /// 根据openid获取微信用户授权得到的信息
        /// </summary>
        /// <param name="openid"></param>
        /// <returns></returns>
        public ActiveWeixinUser GetActiveWeixinUser(string openid)
        {
            string url = APISiteUrl + "api/WeixinApi/GetActiveWeixinUser";
            string postDataStr = string.Format("openid={0}", openid);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<ActiveWeixinUser>(json);
        }

        public ActiveWeixinUser GetActiveWeixinUserById(int id)
        {
            string url = APISiteUrl + "api/WeixinApi/GetActiveWeixinUserById";
            string postDataStr = string.Format("id={0}", id);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<ActiveWeixinUser>(json);
        }

        /// <summary>
        /// 添加一个以openid为主键的微信用户记录
        /// </summary>
        /// <param name="aw"></param>
        /// <returns></returns>
        public int AddActiveWeixinUser(ActiveWeixinUser aw)
        {
            string url = APISiteUrl + "api/WeixinApi/AddActiveWeixinUser";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, aw, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        /// <summary>
        /// 根据活动ID和openid获取微信用户的报名记录
        /// </summary>
        /// <param name="openid"></param>
        /// <returns></returns>
        public ActiveWeixinDraw GetActiveWeixinDraw(int activeId, string openid)
        {
            string url = APISiteUrl + "api/WeixinApi/GetActiveWeixinDraw";
            string postDataStr = string.Format("activeId={0}&openid={1}", activeId, openid);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<ActiveWeixinDraw>(json);
        }

        /// <summary>
        /// 根据用户报名手机号和活动Id获取报名记录
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        public ActiveWeixinDraw GetActiveWeixinDrawByPhone(int activeId, string phone)
        {
            string url = APISiteUrl + "api/WeixinApi/GetActiveWeixinDrawByPhone";
            string postDataStr = string.Format("activeId={0}&phone={1}", activeId, phone);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<ActiveWeixinDraw>(json);
        }

        /// <summary>
        /// 添加一个以openid为主键的微信用户指定活动报名记录
        /// </summary>
        /// <param name="aw"></param>
        /// <returns></returns>
        public int AddActiveWeixinDraw(ActiveWeixinDraw aw)
        {
            string url = APISiteUrl + "api/WeixinApi/AddActiveWeixinDraw";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, aw, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        public int UpdateActiveWeixinDrawIsShare(int activeId, string openid)
        {
            string url = APISiteUrl + "api/WeixinApi/UpdateActiveWeixinDrawIsShare";
            string postDataStr = string.Format("activeId={0}&openid={1}", activeId, openid);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        public int UpdateActiveWeixinDrawIsPay(int activeId, string openid)
        {
            string url = APISiteUrl + "api/WeixinApi/UpdateActiveWeixinDrawIsPay";
            string postDataStr = string.Format("activeId={0}&openid={1}", activeId, openid);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        public int UpdateActiveWeixinDrawSendCount(int activeId, string openid)
        {
            string url = APISiteUrl + "api/WeixinApi/UpdateActiveWeixinDrawSendCount";
            string postDataStr = string.Format("activeId={0}&openid={1}", activeId, openid);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        public int UpdateActiveWeixinDrawHeadImgUrl(int activeId, string openid, string headimgurl)
        {
            string url = APISiteUrl + "api/WeixinApi/UpdateActiveWeixinDrawHeadImgUrl";
            string postDataStr = string.Format("activeId={0}&openid={1}&headimgurl={2}", activeId, openid, headimgurl);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        public List<ActiveWeixinDraw> GetActiveWeixinDrawByReadUser(int activeId, string openid)
        {
            string url = APISiteUrl + "api/WeixinApi/GetActiveWeixinDrawByReadUser";
            string postDataStr = string.Format("activeId={0}&openid={1}", activeId, openid);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<List<ActiveWeixinDraw>>(json);
        }

        /// <summary>
        /// 获取指定活动指定分享者的被阅读记录
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="shareOpenid"></param>
        /// <returns></returns>
        public List<ActiveWeixinShareRead> GetActiveWeixinShareReadList(int activeId, string shareOpenid)
        {
            string url = APISiteUrl + "api/WeixinApi/GetActiveWeixinShareReadList";
            string postDataStr = string.Format("activeId={0}&shareOpenid={1}", activeId, shareOpenid);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<List<ActiveWeixinShareRead>>(json);
        }

        /// <summary>
        /// 插入新的阅读记录
        /// </summary>
        /// <param name="aw"></param>
        /// <returns></returns>
        public int AddActiveWeixinShareRead(ActiveWeixinShareRead aw)
        {
            string url = APISiteUrl + "api/WeixinApi/AddActiveWeixinShareRead";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, aw, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        /// <summary>
        /// 插入新的阅读记录【最新】
        /// </summary>
        /// <param name="aw"></param>
        /// <returns></returns>
        public int AddActiveWeixinShareRead_Luck(ActiveWeixinShareRead aw)
        {
            string url = APISiteUrl + "api/WeixinApi/AddActiveWeixinShareRead_Luck";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, aw, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        /// <summary>
        /// 插入新的抽奖码记录
        /// </summary>
        /// <param name="aluck"></param>
        /// <returns></returns>
        public int AddActiveWeixinLuckCode(ActiveWeixinLuckCode aluck)
        {
            string url = APISiteUrl + "api/WeixinApi/AddActiveWeixinLuckCode";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, aluck, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        /// <summary>
        /// 发布增加新抽奖码的队列任务
        /// </summary>
        /// <param name="aluck"></param>
        /// <returns></returns>
        public bool PublishWeixinLuckCodeTask(ActiveWeixinLuckCode aluck)
        {
            string url = APISiteUrl + "api/WeixinApi/PublishWeixinLuckCodeTask";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, aluck, ref cc);
            return JsonConvert.DeserializeObject<bool>(json);
        }

        /// <summary>
        /// 发布增加宝石的队列任务
        /// </summary>
        /// <param name="aluck"></param>
        /// <returns></returns>
        public bool PublishWeixinGemTask(ActiveWeixinLuckCode aluck)
        {
            string url = APISiteUrl + "api/WeixinApi/PublishWeixinGemTask";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, aluck, ref cc);
            return JsonConvert.DeserializeObject<bool>(json);
        }

        /// <summary>
        /// 获取指定活动指定用户的抽奖码获得情况
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="openid"></param>
        /// <returns></returns>
        public List<ActiveWeixinLuckCode> GetActiveWeixinLuckCodeInfo(int activeId, string openid)
        {
            string url = APISiteUrl + "api/WeixinApi/GetActiveWeixinLuckCodeInfo";
            string postDataStr = string.Format("activeId={0}&openid={1}", activeId, openid);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<List<ActiveWeixinLuckCode>>(json);
        }

        /// <summary>
        /// 插入新的虚拟价值（如宝石）记录
        /// </summary>
        /// <param name="aluck"></param>
        /// <returns></returns>
        public int AddActiveWeixinFicMoney(ActiveWeixinFicMoney aficmoney)
        {
            string url = APISiteUrl + "api/WeixinApi/AddActiveWeixinFicMoney";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, aficmoney, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        /// <summary>
        /// 获取指定活动指定用户的虚拟价值（如宝石）获得情况
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="openid"></param>
        /// <returns></returns>
        public List<ActiveWeixinFicMoney> GetActiveWeixinFicMoneyInfo(int activeId, string openid)
        {
            string url = APISiteUrl + "api/WeixinApi/GetActiveWeixinFicMoneyInfo";
            string postDataStr = string.Format("activeId={0}&openid={1}", activeId, openid);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<List<ActiveWeixinFicMoney>>(json);
        }

        /// <summary>
        /// 获取所有微信合作伙伴信息
        /// </summary>
        /// <returns></returns>
        public List<ActiveWeixinPartner> GetAllWeixinPartners()
        {
            string url = APISiteUrl + "api/WeixinApi/GetAllWeixinPartners";
            string postDataStr = string.Format("");

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<List<ActiveWeixinPartner>>(json);
        }

        /// <summary>
        /// 查询微信报名统计记录
        /// </summary>
        /// <param name="activeId"></param>
        /// <returns></returns>
        public ActiveWeixinStatResult GetWeixinAtvStatResult(int activeId)
        {
            string url = APISiteUrl + "api/WeixinApi/GetWeixinAtvStatResult";
            string postDataStr = string.Format("activeId={0}", activeId);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<ActiveWeixinStatResult>(json);
        }

        /// <summary>
        /// 查询微信报名统计记录(只返回抽奖码数量)
        /// </summary>
        /// <param name="activeId"></param>
        /// <returns></returns>
        public ActiveWeixinStatResult GetActiveWeixinLuckCodeCount(int activeId)
        {
            string url = APISiteUrl + "api/WeixinApi/GetActiveWeixinLuckCodeCount";
            string postDataStr = string.Format("activeId={0}", activeId);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<ActiveWeixinStatResult>(json);
        }

        /// <summary>
        /// 获取指定openid的WeixinUser
        /// </summary>
        /// <param name="openid"></param>
        /// <returns></returns>
        public WeixinUser GetWeixinUser(string openid)
        {
            string url = APISiteUrl + "api/WeixinApi/GetWeixinUser";
            string postDataStr = string.Format("openid={0}", openid);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<WeixinUser>(json);
        }

        #endregion

        #region 微信号回复报名相关操作

        /// <summary>
        /// 添加 ActiveLuckyDraw
        /// </summary>
        /// <param name="activeCode"></param>
        /// <param name="WXAccount"></param>
        /// <param name="phone"></param>
        /// <param name="userName"></param>
        /// <param name="Msg"></param>
        /// <returns></returns>
        public string AddActiveLuckyDraw(int activeCode, string WXAccount, string phone, string userName, string Msg)
        {
            string url = APISiteUrl + "api/WeixinApi/AddActiveLuckyDraw";
            string postDataStr = string.Format("activeCode={0}&WXAccount={1}&phone={2}&userName={3}&Msg={4}", activeCode, WXAccount, phone, userName, Msg);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<string>(json);
        }

        /// <summary>
        /// 查询指定活动的所有 ActiveLuckyDraw
        /// </summary>
        /// <param name="activeCode"></param>
        /// <returns></returns>
        public List<ActvieLuckyDrawEntity> GetLuckyDrawByActiveCode(int activeCode)
        {
            string url = APISiteUrl + "api/WeixinApi/GetLuckyDrawByActiveCode";
            string postDataStr = string.Format("activeCode={0}", activeCode);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<List<ActvieLuckyDrawEntity>>(json);
        }

        #endregion

        #region 微信公众号相关接口操作

        /// <summary>
        /// 更新微信用户的用户信息（如是否关注）
        /// </summary>
        /// <param name="w"></param>
        /// <returns></returns>
        public int UpdateWeixinUserSubscribe(WeixinUser w)
        {
            string url = APISiteUrl + "api/WeixinApi/UpdateWeixinUserSubscribe";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, w, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        /// <summary>
        /// 获取指定Openid的用户信息
        /// </summary>
        /// <param name="openid"></param>
        /// <param name="backJson"></param>
        /// <returns></returns>
        public WeixinUser GetWeixinUserSubscribeInfo(string openid)
        {
            string url = APISiteUrl + "api/WeixinApi/GetWeixinUserSubscribeInfo";
            string postDataStr = string.Format("openid={0}", openid);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<WeixinUser>(json);
        }

        /// <summary>
        /// 查询指定unionid指定微信号下的用户信息（如关注状态）
        /// </summary>
        /// <param name="weixinAcount"></param>
        /// <param name="unionid"></param>
        /// <returns></returns>
        public WeixinUser GetWeixinUserByUnionid(string weixinAcount, string unionid)
        {
            string url = APISiteUrl + "api/WeixinApi/GetWeixinUserByUnionid";
            string postDataStr = string.Format("weixinAcount={0}&unionid={1}", weixinAcount, unionid);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<WeixinUser>(json);
        }

        /// <summary>
        /// 获取指定微信账号的token
        /// </summary>
        /// <param name="weixinAcount"></param>
        /// <returns></returns>
        public string GetWeixinToken(string weixinAcount)
        {
            string url = APISiteUrl + "api/WeixinApi/GetWeixinToken";
            string postDataStr = string.Format("weixinAcount={0}", weixinAcount);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<string>(json);
        }

        /// <summary>
        /// 获取指定微信账号的token【根据枚举】
        /// </summary>
        /// <param name="weixinAcount"></param>
        /// <returns></returns>
        public string GetWeixinTokenByCode(int weixinAcount)
        {
            string url = APISiteUrl + "api/WeixinApi/GetWeixinTokenByCode";
            string postDataStr = string.Format("weixinAcount={0}", weixinAcount);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<string>(json);
        }

        /// <summary>
        /// 指定微信公众号发送自定义信息【Text】
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool SendWeixinTextMsg(WeixinMsgText msg)
        {
            string url = APISiteUrl + "api/WeixinApi/SendWeixinTextMsg";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, msg, ref cc);
            return JsonConvert.DeserializeObject<bool>(json);
        }

        /// <summary>
        /// 指定微信公众号发送自定义信息【News】
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool SendWeixinNewsMsg(WeixinMsgNews msg)
        {
            string url = APISiteUrl + "api/WeixinApi/SendWeixinNewsMsg";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, msg, ref cc);
            return JsonConvert.DeserializeObject<bool>(json);
        }

        /// <summary>
        /// 发送微信模板信息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public string SendTemplateMessage(WeixinTempMsgEntity msg)
        {
            string url = APISiteUrl + "api/WeixinApi/SendTemplateMessage";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, msg, ref cc);
            return JsonConvert.DeserializeObject<string>(json);
        }

        /// <summary>
        /// 通用活动红包发送
        /// </summary>
        /// <param name="addEntity"></param>
        /// <returns></returns>
        public int GenActiveRedPackReward(WeixinRewardRecord addEntity)
        {
            string url = APISiteUrl + "api/WeixinApi/GenActiveRedPackReward";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, addEntity, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }
        
        /// <summary>
        /// 通用活动红包发送(for redpack union active)
        /// </summary>
        /// <param name="addEntity"></param>
        /// <returns></returns>
        public int GenActiveRedPackRewardForUnion(WeixinRewardRecord addEntity)
        {
            string url = APISiteUrl + "api/WeixinApi/GenActiveRedPackRewardForUnion";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, addEntity, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        /// <summary>
        /// 获取指定活动类型指定Openid的红包发送记录
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="activeId"></param>
        /// <param name="reOpenid"></param>
        /// <returns></returns>
        public List<WeixinRewardRecord> GetWeixinRewardRecordByWxActive(int sourceType, int activeId, string reOpenid)
        {
            string url = APISiteUrl + "api/WeixinApi/GetWeixinRewardRecordByWxActive";
            string postDataStr = string.Format("sourceType={0}&activeId={1}&reOpenid={2}", sourceType, activeId, reOpenid);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<List<WeixinRewardRecord>>(json);
        }

        /// <summary>
        /// 扣除相关合作伙伴的指定资金
        /// </summary>
        /// <param name="partnerId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public int UpdateWxPartnerActiveFund(long partnerId, decimal value)
        {
            string url = APISiteUrl + "api/WeixinApi/UpdateWxPartnerActiveFund";
            string postDataStr = string.Format("partnerId={0}&value={1}", partnerId, value);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        #endregion

        #region 自定义活动相关操作

        public int AddCustomActiveUser(CustomActiveUser cuser)
        {
            string url = APISiteUrl + "api/WeixinApi/AddCustomActiveUser";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, cuser, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        public CustomActiveUser GetCustomActiveUser(int activeid, string phone)
        {
            string url = APISiteUrl + "api/WeixinApi/GetCustomActiveUser";
            string postDataStr = string.Format("activeid={0}&phone={1}", activeid, phone);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<CustomActiveUser>(json);
        }

        /// <summary>
        /// 上传指定二维码，并返回上传后的地址路径
        /// </summary>
        /// <param name="oriQrcodeImg"></param>
        /// <returns></returns>
        public string UploadWeixinQrcodeImg(string oriQrcodeImg)
        {
            string url = APISiteUrl + "api/WeixinApi/UploadWeixinQrcodeImg";
            string postDataStr = string.Format("oriQrcodeImg={0}", oriQrcodeImg);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<string>(json);
        }

        #endregion

        #region 微信大使投票活动

        /// <summary>
        /// 获取指定活动ID指定sourceid(draw id)的被投票记录
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="sourceId"></param>
        /// <param name="today">是否筛选今天的数据</param>
        /// <returns></returns>
        public List<ActiveVoteRecord> GetActiveVoteRecordForType2BySourceId(int activeId, int sourceId, int today = 0)
        {
            string url = APISiteUrl + "api/WeixinApi/GetActiveVoteRecordForType2BySourceId";
            string postDataStr = string.Format("activeId={0}&sourceId={1}&today={2}", activeId, sourceId, today);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<List<ActiveVoteRecord>>(json);
        }

        /// <summary>
        /// 获取指定活动ID指定sourceid(draw id)的被投票记录
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="sourceId">大使ID（包括自己）</param>
        /// <param name="reltionId">酒店ID</param>
        /// <param name="today">是否筛选今天的数据</param>
        /// <returns></returns>
        public List<ActiveVoteRecord> GetActiveVoteRecordForType2BySourceIdAndReltionId(int activeId, int sourceId, int reltionId, string weixinAccount, int today = 0)
        {
            string url = APISiteUrl + "api/WeixinApi/GetActiveVoteRecordForType2BySourceIdAndReltionId";
            string postDataStr = string.Format("activeId={0}&sourceId={1}&reltionId={2}&weixinAccount={3}&today={4}", activeId, sourceId, reltionId, weixinAccount, today);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<List<ActiveVoteRecord>>(json);
        }

        /// <summary>
        /// 获取指定报名用户的抽奖记录（包含奖品详细信息）
        /// </summary>
        /// <param name="activeDrawId"></param>
        /// <returns></returns>
        public List<ActiveLuckDrawRecordContainPrize> GetActiveLuckRecordAndPrizeByDrawId(int activeDrawId)
        {
            string url = APISiteUrl + "api/WeixinApi/GetActiveLuckRecordAndPrizeByDrawId";
            string postDataStr = string.Format("activeDrawId={0}", activeDrawId);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<List<ActiveLuckDrawRecordContainPrize>>(json);
        }

        /// <summary>
        /// 插入抽奖记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int AddActiveLuckDrawRecord(ActiveLuckDrawRecord entity)
        {
            string url = APISiteUrl + "api/WeixinApi/AddActiveLuckDrawRecord";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, entity, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        /// <summary>
        /// 获取指定活动ID指定wx openid的投票记录
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="weixinAccount"></param>
        /// <param name="today">是否筛选今天的数据</param>
        /// <returns></returns>
        public List<ActiveVoteRecord> GetActiveVoteRecordForType1ByWxAccount(int activeId, string weixinAccount, int today = 0)
        {
            string url = APISiteUrl + "api/WeixinApi/GetActiveVoteRecordForType1ByWxAccount";
            string postDataStr = string.Format("activeId={0}&weixinAccount={1}&today={2}", activeId, weixinAccount, today);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<List<ActiveVoteRecord>>(json);
        }

        /// <summary>
        /// 新增投票记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int AddActiveVoteRecord(ActiveVoteRecord entity)
        {
            string url = APISiteUrl + "api/WeixinApi/AddActiveVoteRecord";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, entity, ref cc);
            return JsonConvert.DeserializeObject<int>(json);
        }

        /// <summary>
        /// 获取指定活动ID指定sourceid的奖品信息
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="sourceId"></param>
        /// <returns></returns>
        public List<ActiveRulePrize> GetActiveRulePrizeBySourceId(int activeId, int sourceId)
        {
            string url = APISiteUrl + "api/WeixinApi/GetActiveRulePrizeBySourceId";
            string postDataStr = string.Format("activeId={0}&&sourceId={1}", activeId, sourceId);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<List<ActiveRulePrize>>(json);
        }

        #endregion

        #region 微信素材相关操作

        public List<int> PhotoUploadWithUrls(string[] picUrls)
        {
            string url = APISiteUrl + "api/Photo/PhotoUploadWithUrls";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, picUrls, ref cc);
            return JsonConvert.DeserializeObject<List<int>>(json);
        }

        public List<PHSPhotoInfoEntity> GenPhotoPaths(List<int> ids)
        {
            string url = APISiteUrl + "api/Photo/GenPhotoPaths";
            CookieContainer cc = new CookieContainer();
            string json = HttpRequestHelper.PostJson(url, ids, ref cc);
            return JsonConvert.DeserializeObject<List<PHSPhotoInfoEntity>>(json);
        }

        /// <summary>
        /// 根据IDX获取微信素材信息
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public WeixinMaterialEntity GetWeixinMaterialByIdx(int idx)
        {
            string url = APISiteUrl + "api/WeixinApi/GetWeixinMaterialByIdx";
            string postDataStr = string.Format("idx={0}", idx);

            CookieContainer cc = new CookieContainer();

            string json = HttpRequestHelper.Get(url, postDataStr, ref cc);
            return JsonConvert.DeserializeObject<WeixinMaterialEntity>(json);
        }

        #endregion
    }
}
