using HJD.AccessService.Contract.Model;
using HJD.AccessService.Contract.Params;
using HJD.AccountServices.Entity;
using HJD.Framework.WCF;
using HJD.HotelManagementCenter.Domain.Comm;
using HJD.HotelManagementCenter.IServices;
using HJD.HotelServices.Contracts;
using HJD.PhotoServices.Contracts;
using HJD.WeixinService.Contract;
using HJD.WeixinServices.Contract;
using HJD.WeixinServices.Contracts;
using HJDAPI.Common.Helpers;
using HJDAPI.Controllers.Activity;
using HJDAPI.Controllers.Adapter;
using HJDAPI.Controllers.Common;
using HJDAPI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml;

namespace HJDAPI.Controllers
{
    public class WeixinApiController : BaseApiController
    {

        public static ICommService commService = ServiceProxyFactory.Create<ICommService>("ICommService");
        public static IWeixinService weixinService = ServiceProxyFactory.Create<IWeixinService>("IWeixinService");

        private string logFile = Configs.LogPath + string.Format("WeiXinLog_{0}{1}{2}.txt", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

         [System.Web.Http.HttpGet]
        public string wxapiTest(string input = "noInput")
        {
            return "wxapi:" + input;
        }


        #region 获取微信客服信息

        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
        public WeixinChatRecordResult GetWeixinChatRecord(WeixinChatRecordParams rp)
        {
            return WeiXinAdapter.GetWeixinChatRecord(rp);
        }

        [System.Web.Http.HttpGet]
        public bool IsPartWeixinActivity(int acode, string phone)
        {
            return weixinService.IsPartWeixinActivity(acode, phone);
        }

        #endregion

        #region 微信h5

        [System.Web.Http.HttpGet]
        public List<WeixinActivityEntity> GetWeixinActives()
        {
            return weixinService.GetWeixinActives();
        }

        [System.Web.Http.HttpGet]
        public WeixinActivityEntity GetOneWeixinActive(int activeid)
        {
            return weixinService.GetOneWeixinActive(activeid);
        }

        /// <summary>
        /// 根据openid获取微信用户授权得到的信息
        /// </summary>
        /// <param name="openid"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public ActiveWeixinUser GetActiveWeixinUser(string openid)
        {
            return weixinService.GetActiveWeixinUser(openid);
        }

        [System.Web.Http.HttpGet]
        public ActiveWeixinUser GetActiveWeixinUserById(int id)
        {
            return weixinService.GetActiveWeixinUserById(id);
        }

        /// <summary>
        /// 添加一个以openid为主键的微信用户记录
        /// </summary>
        /// <param name="aw"></param>
        /// <returns></returns>
        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
        public int AddActiveWeixinUser(ActiveWeixinUser aw)
        {
            return weixinService.AddActiveWeixinUser(aw);
        }

        /// <summary>
        /// 根据活动ID和openid获取微信用户的报名记录
        /// </summary>
        /// <param name="openid"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public ActiveWeixinDraw GetActiveWeixinDraw(int activeId, string openid)
        {
            return weixinService.GetActiveWeixinDraw(activeId, openid);
        }

        /// <summary>
        /// 根据用户报名手机号和活动Id获取报名记录
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public ActiveWeixinDraw GetActiveWeixinDrawByPhone(int activeId, string phone)
        {
            return weixinService.GetActiveWeixinDrawByPhone(activeId, phone);
        }

        /// <summary>
        /// 根据用户报名Id和活动Id获取报名记录
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public ActiveWeixinDraw GetActiveWeixinDrawById(int activeId, int id)
        {
            return weixinService.GetActiveWeixinDrawById(activeId, id);
        }

        /// <summary>
        /// 添加一个以openid为主键的微信用户指定活动报名记录
        /// </summary>
        /// <param name="aw"></param>
        /// <returns></returns>
        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
        public int AddActiveWeixinDraw(ActiveWeixinDraw aw)
        {
            return weixinService.AddActiveWeixinDraw(aw);
        }

        [System.Web.Http.HttpGet]
        public int UpdateActiveWeixinDrawIsShare(int activeId, string openid)
        {
            return weixinService.UpdateActiveWeixinDrawIsShare(activeId, openid);
        }

        [System.Web.Http.HttpGet]
        public int UpdateActiveWeixinDrawIsPay(int activeId, string openid)
        {
            return weixinService.UpdateActiveWeixinDrawIsPay(activeId, openid);
        }

        /// <summary>
        /// 修改指定活动指定用户的报名记录的手机号码
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="openid"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public int UpdateActiveWeixinDrawPhone(int activeId, string openid, string phone)
        {
            return weixinService.UpdateActiveWeixinDrawPhone(activeId, openid, phone);
        }

        [System.Web.Http.HttpGet]
        public int UpdateActiveWeixinDrawSendCount(int activeId, string openid)
        {
            return weixinService.UpdateActiveWeixinDrawSendCount(activeId, openid);
        }

        /// <summary>
        /// 更新指定openid的报名头像
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="openid"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public int UpdateActiveWeixinDrawHeadImgUrl(int activeId, string openid, string headimgurl)
        {
            return weixinService.UpdateActiveWeixinDrawHeadImgUrl(activeId, openid, headimgurl);
        }

        [System.Web.Http.HttpGet]
        public List<ActiveWeixinDraw> GetActiveWeixinDrawByReadUser(int activeId, string openid)
        {
            return weixinService.GetActiveWeixinDrawByReadUser(activeId, openid);
        }

        /// <summary>
        /// 获取指定活动指定分享者的被阅读记录
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="shareOpenid"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public List<ActiveWeixinShareRead> GetActiveWeixinShareReadList(int activeId, string shareOpenid)
        {
            return weixinService.GetActiveWeixinShareReadList(activeId, shareOpenid);
        }

        /// <summary>
        /// 插入新的阅读记录
        /// </summary>
        /// <param name="aw"></param>
        /// <returns></returns>
        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
        public int AddActiveWeixinShareRead(ActiveWeixinShareRead aw)
        {
            return weixinService.AddActiveWeixinShareRead(aw);
        }

        /// <summary>
        /// 插入新的阅读记录【最新】
        /// </summary>
        /// <param name="aw"></param>
        /// <returns></returns>
        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
        public int AddActiveWeixinShareRead_Luck(ActiveWeixinShareRead aw)
        {
            return weixinService.AddActiveWeixinShareRead_Luck(aw);
        }

        /// <summary>
        /// 插入新的抽奖码记录
        /// </summary>
        /// <param name="aluck"></param>
        /// <returns></returns>
        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
        public int AddActiveWeixinLuckCode(ActiveWeixinLuckCode aluck)
        {
            return weixinService.AddActiveWeixinLuckCode(aluck);
        }

        /// <summary>
        /// 插入新的虚拟价值（如宝石）记录
        /// </summary>
        /// <param name="aluck"></param>
        /// <returns></returns>
        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
        public int AddActiveWeixinFicMoney(ActiveWeixinFicMoney aficmoney)
        {
            return weixinService.AddActiveWeixinFicMoney(aficmoney);
        }

        /// <summary>
        /// 获取指定活动指定用户的虚拟价值（如宝石）获得情况
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="openid"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public List<ActiveWeixinFicMoney> GetActiveWeixinFicMoneyInfo(int activeId, string openid)
        {
            return weixinService.GetActiveWeixinFicMoneyInfo(activeId, openid);
        }

        /// <summary>
        /// 发布增加新抽奖码的队列任务
        /// </summary>
        /// <param name="aluck"></param>
        /// <returns></returns>
        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
        public bool PublishWeixinLuckCodeTask(ActiveWeixinLuckCode aluck)
        {
            return weixinService.PublishWeixinLuckCodeTask(aluck);
        }

        /// <summary>
        /// 发布增加宝石的队列任务
        /// </summary>
        /// <param name="aluck"></param>
        /// <returns></returns>
        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
        public bool PublishWeixinGemTask(ActiveWeixinLuckCode aluck)
        {
            return weixinService.PublishWeixinGemTask(aluck);
        }

        /// <summary>
        /// 获取指定活动指定用户的抽奖码获得情况
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="openid"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public List<ActiveWeixinLuckCode> GetActiveWeixinLuckCodeInfo(int activeId, string openid)
        {
            return weixinService.GetActiveWeixinLuckCodeInfo(activeId, openid);
        }

        /// <summary>
        /// 获取所有微信合作伙伴信息
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public List<ActiveWeixinPartner> GetAllWeixinPartners()
        {
            return weixinService.GetAllWeixinPartners();
        }

        /// <summary>
        /// 查询微信报名统计记录
        /// </summary>
        /// <param name="activeId"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public ActiveWeixinStatResult GetWeixinAtvStatResult(int activeId)
        {
            return weixinService.GetWeixinAtvStatResult(activeId);
        }

        /// <summary>
        /// 查询微信报名统计记录(只返回抽奖码数量)
        /// </summary>
        /// <param name="activeId"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public ActiveWeixinStatResult GetActiveWeixinLuckCodeCount(int activeId)
        {
            return weixinService.GetActiveWeixinLuckCodeCount(activeId);
        }

        /// <summary>
        /// 获取指定openid的WeixinUser
        /// </summary>
        /// <param name="openid"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public WeixinUser GetWeixinUser(string openid)
        {
            return weixinService.GetWeixinUser(openid);
        }

        #region 投票活动相关

        /// <summary>
        /// 根据活动ID获取扩展表信息（根据activeid关联groupid，再通过groupid查询exs）
        /// </summary>
        /// <param name="activeId"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public List<ActiveRuleEx> GetActiveRuleExsByActiveId(int activeId)
        {
            var _list = weixinService.GetActiveRuleExsByActiveId(activeId) ?? new List<ActiveRuleEx>();

            //默认按照名额倒叙
            _list = _list.OrderByDescending(_ => _.OfferCount).ToList();

            return _list;
        }

        /// <summary>
        /// 累加指定ActiveRuleEx Id的OfferCount字段
        /// </summary>
        /// <param name="activeRuleExId">ActiveRuleEx主键Id</param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public bool AddUpActiveRuleExOfferCountById(int activeRuleExId)
        {
            return weixinService.AddUpActiveRuleExOfferCountById(activeRuleExId);
        }

        #endregion

        #region 大投票活动相关

        /// <summary>
        /// 根据活动ID获取扩展表信息（根据activeid关联groupid，再通过groupid查询exs）
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="orderType">1按照排序字段降序 2按照投票数+排序字段降序</param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public List<ActiveRuleExForVote> GetActiveRuleExsForVoteByActiveId(int activeId, int orderType = 1)
        {
            var _list = weixinService.GetActiveRuleExsForVoteByActiveId(activeId) ?? new List<ActiveRuleExForVote>();

            //默认按照名额倒叙
            if (orderType == 1)
            {
                _list = _list.OrderByDescending(_ => _.OrderNum).ToList();
            }
            else if (orderType == 2)
            {
                _list = _list.OrderByDescending(_ => _.VoteCount).ThenByDescending(_ => _.OrderNum).ToList();
            }

            return _list;
        }

        /// <summary>
        /// 获取指定活动ID指定sourceid的奖品信息
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="sourceId"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public List<ActiveRulePrize> GetActiveRulePrizeBySourceId(int activeId, int sourceId) 
        {
            return weixinService.GetActiveRulePrizeBySourceId(activeId, sourceId);
        }

        /// <summary>
        /// 获取指定报名用户的抽奖记录
        /// </summary>
        /// <param name="activeDrawId"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public List<ActiveLuckDrawRecord> GetActiveLuckDrawRecordByDrawId(int activeDrawId)
        {
            return weixinService.GetActiveLuckDrawRecordByDrawId(activeDrawId);
        }

        /// <summary>
        /// 获取指定报名用户的抽奖记录（包含奖品详细信息）
        /// </summary>
        /// <param name="activeDrawId"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public List<ActiveLuckDrawRecordContainPrize> GetActiveLuckRecordAndPrizeByDrawId(int activeDrawId)
        {
            return weixinService.GetActiveLuckRecordAndPrizeByDrawId(activeDrawId);
        }

        /// <summary>
        /// 插入抽奖记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public int AddActiveLuckDrawRecord(ActiveLuckDrawRecord entity)
        {
            return weixinService.AddActiveLuckDrawRecord(entity);
        }

        /// <summary>
        /// 获取指定活动id和RuleExId的所有代言人信息
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="exid"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public List<ActiveRuleSpokesmanAndDrawInfo> GetActiveSpokesmanInfoByActiveIdAndExId(int activeId, int exid)
        {
            return weixinService.GetActiveSpokesmanInfoByActiveIdAndExId(activeId, exid);
        }

        /// <summary>
        /// 获取指定活动ID指定wx openid的投票记录
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="weixinAccount"></param>
        /// <param name="today">是否筛选今天的数据</param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public List<ActiveVoteRecord> GetActiveVoteRecordForType1ByWxAccount(int activeId, string weixinAccount, int today = 0)
        {
            var _list = weixinService.GetActiveVoteRecordForType1ByWxAccount(activeId, weixinAccount);

            if (today == 1)
            {
                _list = _list.Where(_ => _.CreateTime.Date == DateTime.Now.Date).ToList();
            }

            return _list;
        }

        /// <summary>
        /// 获取指定活动ID指定sourceid(draw id)的被投票记录
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="sourceId"></param>
        /// <param name="today">是否筛选今天的数据</param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public List<ActiveVoteRecord> GetActiveVoteRecordForType2BySourceId(int activeId, int sourceId, int today = 0)
        {
            var _list = weixinService.GetActiveVoteRecordForType2BySourceId(activeId, sourceId);

            if (today == 1)
            {
                _list = _list.Where(_ => _.CreateTime.Date == DateTime.Now.Date).ToList();
            }

            return _list;
        }

        /// <summary>
        /// 获取指定活动ID指定sourceid(draw id)的被投票记录
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="sourceId">大使ID（包括自己）</param>
        /// <param name="reltionId">酒店ID</param>
        /// <param name="today">是否筛选今天的数据</param>
        /// <param name="weixinAccount"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public List<ActiveVoteRecord> GetActiveVoteRecordForType2BySourceIdAndReltionId(int activeId, int sourceId, int reltionId, string weixinAccount, int today = 0)
        {
            var _list = weixinService.GetActiveVoteRecordForType2BySourceIdAndReltionId(activeId, sourceId, reltionId, weixinAccount);

            if (today == 1)
            {
                _list = _list.Where(_ => _.CreateTime.Date == DateTime.Now.Date).ToList();
            }

            return _list;
        }

        /// <summary>
        /// 新增投票记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public int AddActiveVoteRecord(ActiveVoteRecord entity)
        {
            if (DateTime.Now > DateTime.Parse("2018-12-10 23:59:59"))
            {
                return -1;
            }

            //检查当前微信account是否真实
            if (entity != null && !string.IsNullOrEmpty(entity.WeixinAccount))
            {
                var weixinDraw = weixinService.GetActiveWeixinDraw(entity.ActiveId, entity.WeixinAccount);
                if (weixinDraw == null || weixinDraw.ID <= 0)
                {
                    return -1;
                }
            }
            else {

                if (string.IsNullOrEmpty(entity.WeixinAccount))
                {
                    return -1;
                }
            }

            return weixinService.AddActiveVoteRecord(entity);
        }

        /// <summary>
        /// 获取指定用户报名ID代言的RuleEx记录
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="activeDrawId"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public List<ActiveRuleExForVote> GetActiveRuleExsForVoteByDrawId(int activeId, int activeDrawId)
        {
            return weixinService.GetActiveRuleExsForVoteByDrawId(activeId, activeDrawId);
        }

        /// <summary>
        /// 新增指定活动的代言记录
        /// </summary>
        /// <param name="entity">代言关联对象</param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public int AddActiveRuleSpokesman(ActiveRuleSpokesman entity)
        {
            return weixinService.AddActiveRuleSpokesman(entity);
        }

        #endregion

        #endregion

        #region 相关附属操作

        [System.Web.Http.HttpGet]
        public int GenNewUsersByActivePhone(string openid, long CID = 0)
        {
            var phoneList = GetWeixinSignUpPhoneList();
            foreach (var phone in phoneList)
            {
                if (CommMethods.IsPhone(phone))
                {
                    try
                    {
                        User_Info ui = AccountAdapter.GetOrRegistPhoneUser(phone, CID);
                    }
                    catch (Exception ex)
                    {
                        
                    }
                    
                }
            }

            return phoneList.Count;
        }

        public List<string> GetWeixinSignUpPhoneList()
        {
            var list = new List<string>();

            var txtList = File.ReadAllLines(@"D:\Log\WeixinService\phone.txt");
            list = txtList.ToList();

            return list;
        }

        [System.Web.Http.HttpGet]
        public List<KeyValueEntity> GetWeiXinChannelCode()
        {
            List<KeyValueEntity> result = new List<KeyValueEntity>();
            WeiXinChannelCode[] lists = (WeiXinChannelCode[])Enum.GetValues(typeof(WeiXinChannelCode));
            foreach (WeiXinChannelCode item in lists)
            {
                KeyValueEntity model = new KeyValueEntity();
                string name = item.ToString();
                int value = (int)item;
                model.Key = name;
                model.Value = value.ToString();
                result.Add(model);
            }
            return result;
        }

        #endregion

        #region 微信号回复报名相关操作

        [System.Web.Http.HttpGet]
        public string AddActiveLuckyDraw(int activeCode, string WXAccount, string phone, string userName, string Msg)
        {
            return commService.AddActiveLuckyDraw(activeCode, WXAccount, phone, userName, Msg);
        }

        [System.Web.Http.HttpGet]
        public List<ActvieLuckyDrawEntity> GetLuckyDrawByActiveCode(int activeCode)
        {
            return commService.GetLuckyDrawByActiveCode(activeCode);
        }

        #endregion

        #region 微信公众号相关接口操作

        /// <summary>
        /// 更新微信用户的用户信息（如是否关注）
        /// </summary>
        /// <param name="w"></param>
        /// <returns></returns>
        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
        public int UpdateWeixinUserSubscribe(WeixinUser w)
        {
            return weixinService.UpdateWeixinUserSubscribe(w);
        }

        /// <summary>
        /// 获取指定Openid的用户信息
        /// </summary>
        /// <param name="openid"></param>
        /// <param name="backJson"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public WeixinUser GetWeixinUserSubscribeInfo(string openid)
        {
            return weixinService.GetWeixinUserSubscribeInfo(openid);
        }

        /// <summary>
        /// 查询指定unionid指定微信号下的用户信息（如关注状态）
        /// </summary>
        /// <param name="weixinAcount"></param>
        /// <param name="unionid"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public WeixinUser GetWeixinUserByUnionid(string weixinAcount, string unionid)
        {
            return weixinService.GetWeixinUserByUnionid(weixinAcount, unionid);
        }

        /// <summary>
        /// 获取指定微信账号的token
        /// </summary>
        /// <param name="weixinAcount"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public string GetWeixinToken(string weixinAcount)
        {
            return weixinService.GetWeixinToken(weixinAcount);
        }

        /// <summary>
        /// 获取指定微信账号的token【根据枚举】
        /// </summary>
        /// <param name="weixinAcount"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public string GetWeixinTokenByCode(int weixinAcount)
        {
            return weixinService.GetWeixinTokenByCode((WeiXinChannelCode)weixinAcount);
        }

        /// <summary>
        /// 指定微信公众号发送自定义信息【Text】
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
        public bool SendWeixinTextMsg(WeixinMsgText msg)
        {
            return weixinService.SendWeixinTextMsg(msg);
        }

        /// <summary>
        /// 指定微信公众号发送自定义信息【News】
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
        public bool SendWeixinNewsMsg(WeixinMsgNews msg)
        {
            return weixinService.SendWeixinNewsMsg(msg);
        }

        /// <summary>
        /// 生成指定微信账号指定参数的二维码
        /// </summary>
        /// <param name="weixinAcount"></param>
        /// <param name="expires"></param>
        /// <param name="actionName"></param>
        /// <param name="sceneId"></param>
        /// <param name="sceneStr"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public string CreateAccountQrcode(int weixinAcount, int expires, string actionName, int sceneId = 0, string sceneStr = "")
        {
            return WeiXinAdapter.CreateAccountQrcode(weixinAcount, expires, actionName, sceneId, sceneStr);
        }

        /// <summary>
        /// 发送微信模板信息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
        public string SendTemplateMessage(WeixinTempMsgEntity msg)
        {
            return WeiXinAdapter.SendTemplateMessage(msg.WeixinAcount, msg.OpenId, msg.TempId, msg.Url, msg.First, msg.Remark, msg.List);
        }

        /// <summary>
        /// 通用活动红包发送
        /// </summary>
        /// <param name="addEntity"></param>
        /// <returns></returns>
        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
        public int GenActiveRedPackReward(WeixinRewardRecord addEntity)
        {
            return CouponAdapter.GenActiveRedPackReward(addEntity);
        }

        /// <summary>
        /// 通用活动红包发送(for redpack union active)
        /// </summary>
        /// <param name="addEntity"></param>
        /// <returns></returns>
        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
        public int GenActiveRedPackRewardForUnion(WeixinRewardRecord addEntity)
        {
            return CouponAdapter.GenActiveRedPackRewardForUnion(addEntity);
        }

        /// <summary>
        /// 获取指定活动类型指定Openid的红包发送记录
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="activeId"></param>
        /// <param name="reOpenid"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public List<WeixinRewardRecord> GetWeixinRewardRecordByWxActive(int sourceType, int activeId, string reOpenid)
        {
            return weixinService.GetWeixinRewardRecordByWxActive(sourceType, activeId, reOpenid);
        }

        /// <summary>
        /// 扣除相关合作伙伴的指定资金
        /// </summary>
        /// <param name="partnerId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public int UpdateWxPartnerActiveFund(long partnerId, decimal value)
        {
            return weixinService.UpdateWxPartnerActiveFund(partnerId, value);
        }

        #endregion

        #region 自定义活动相关操作

        [System.Web.Http.AcceptVerbs("POST")]
        [System.Web.Http.HttpPost]
        public int AddCustomActiveUser(CustomActiveUser cuser)
        {
            return weixinService.AddCustomActiveUser(cuser);
        }

        /// <summary>
        /// 获取指定自定义活动的参与人信息
        /// </summary>
        /// <param name="activeid"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public CustomActiveUser GetCustomActiveUser(int activeid, string phone)
        {
            return weixinService.GetCustomActiveUser(activeid, phone);
        }

        /// <summary>
        /// 上传指定二维码，并返回上传后的地址路径
        /// </summary>
        /// <param name="oriQrcodeImg"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public string UploadWeixinQrcodeImg(string oriQrcodeImg)
        {
            var imgUrl = oriQrcodeImg;

            var imgs = new List<string> { oriQrcodeImg };

            //上传并生成ids
            List<int> _ids = new PhotoController().PhotoUploadWithUrls(imgs.ToArray());

            //根据id获取photo对象
            List<PHSPhotoInfoEntity> _newImagePaths = new PhotoController().GenPhotoPaths(_ids);

            if (_newImagePaths != null && _newImagePaths.Count > 0)
            {
                var _newImgObj = _newImagePaths[0];
                imgUrl = _newImgObj.PhotoUrl[HJD.PhotoServices.Entity.PhotoSizeType.jupiter];
                imgUrl = imgUrl.Replace("_jupiter", "_640x426");
            }

            return imgUrl;
        }

        /// <summary>
        /// 获取指定UserId和指定微信账号的微信用户信息
        /// </summary>
        /// <param name="uid">UserId</param>
        /// <param name="weixinAcountStr">WeixinUser表的WeixinAccount字段（如 shanglvzhoumo “尚旅周末服务号”的微信用户信息记录）</param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public WeixinUser GetOpenidByUid(long uid, string weixinAcountStr)
        {
            return WeiXinAdapter.GetOpenidByUid(uid, weixinAcountStr);
        }

        #endregion

        #region wxapp api

        /// <summary>
        /// 【高端酒店特价】根据js_code获取session&openid
        /// </summary>
        /// <param name="js_code"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public string WxApp_jscode2session(string js_code = "")
        {
            var resultStr = "";

            var url = "https://api.weixin.qq.com/sns/jscode2session";
            var param = string.Format("appid={0}&secret={1}&js_code={2}&grant_type=authorization_code", "wx3480af40b1fb514d", "ed751255d1c0aa24ebfb204760a82cea", js_code);

            CookieContainer cc = new CookieContainer();
            resultStr = HttpRequestHelper.Get(url, param, ref cc);

            return resultStr;
        }

        /// <summary>
        /// 【周末酒店Lite】根据js_code获取session&openid
        /// </summary>
        /// <param name="js_code"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public string WxApp_jscode2sessionForZmjdLite(string js_code = "")
        {
            var resultStr = "";

            var url = "https://api.weixin.qq.com/sns/jscode2session";
            var param = string.Format("appid={0}&secret={1}&js_code={2}&grant_type=authorization_code", "wxa98ca314b9c10d63", "656dc17b1b1010b36b446b807bca239a", js_code);

            CookieContainer cc = new CookieContainer();
            resultStr = HttpRequestHelper.Get(url, param, ref cc);

            return resultStr;
        }

        /// <summary>
        /// 【遛娃指南Lite】根据js_code获取session&openid
        /// </summary>
        /// <param name="js_code"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public string WxApp_jscode2sessionForLiuwaLite(string js_code = "")
        {
            var resultStr = "";

            var url = "https://api.weixin.qq.com/sns/jscode2session";
            var param = string.Format("appid={0}&secret={1}&js_code={2}&grant_type=authorization_code", "wx72ddcc3b0a176bab", "c985dc1575f83f0bf0c403aef46a1d86", js_code);

            CookieContainer cc = new CookieContainer();
            resultStr = HttpRequestHelper.Get(url, param, ref cc);

            return resultStr;
        }

        /// <summary>
        /// 解密wxapp敏感数据
        /// </summary>
        /// <param name="encryptedDataStr"></param>
        /// <param name="sessionKey"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public string AES_decrypt(string encryptedDataStr, string sessionKey, string iv)  
        {  
            RijndaelManaged rijalg = new RijndaelManaged();  
            //-----------------    
            //设置 cipher 格式 AES-128-CBC    
  
            rijalg.KeySize = 128;  
  
            rijalg.Padding = PaddingMode.PKCS7;  
            rijalg.Mode = CipherMode.CBC;

            rijalg.Key = Convert.FromBase64String(sessionKey);  
            rijalg.IV = Convert.FromBase64String(iv);
  
  
            byte[] encryptedData= Convert.FromBase64String(encryptedDataStr);

            //解密    
            ICryptoTransform decryptor = rijalg.CreateDecryptor(rijalg.Key, rijalg.IV);  
  
            string result;  
              
            using (MemoryStream msDecrypt = new MemoryStream(encryptedData))  
            {  
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))  
                {  
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))  
                    {  
  
                        result = srDecrypt.ReadToEnd();  
                    }  
                }  
            }  
  
            return result;  
        }

        #endregion

        #region 微信素材相关接口操作

        /// <summary>
        /// 根据IDX获取微信素材信息
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public WeixinMaterialEntity GetWeixinMaterialByIdx(int idx)
        {
            return weixinService.GetMaterialByIDX(idx);
        }

        #endregion
    }
}
