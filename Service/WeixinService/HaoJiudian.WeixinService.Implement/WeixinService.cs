using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HJD.WeixinServices.Contract;
using System.ServiceModel;
using System.Configuration;
using HJD.Framework.Interface;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using HJD.WeixinServices.Contracts;
using HJD.WeixinServices.Implement.Helper;
using HJD.WeixinService.Implement.Entity;
using System.Text.RegularExpressions;
using HJD.WeixinService.Implement.Helper;
using HJD.AccessService.Contract;
using HJD.Framework.WCF;
using HJD.WeixinService.Contract;

namespace HJD.WeixinServices.Implement
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public partial class WeixinService : IWeixinService
    {
        public static ICacheProvider LocalCache1h = CacheManagerFactory.Create("DynamicCacheFor1h");
        private static IMemcacheProvider memCacheWeixin = MemcacheManagerFactory.Create("WeixinCache");
        private string CacheWeixinVer { get { return ConfigurationManager.AppSettings["CacheWeixinVer"]; } }
        private string CacheKeyWeixinBaseInfo { get { return "WeixinBaseInfo" + CacheWeixinVer; } }

        static readonly Object _obj = new object();

        /// <summary>
        /// 微信智能回复（文字类型请求）
        /// </summary>
        /// <param name="requestEntity"></param>
        /// <returns></returns>
        public ResponseEntity GetWeixinResponseText(RequestEntity requestEntity)
        {
            ResponseEntity responseEntity = new ResponseEntity { RequestEntity = requestEntity };
            string createTime = WeixinHelper.ConvertDateTimeInt(DateTime.Now).ToString();

            try
            {
                #region 返现

                if (requestEntity.Content.StartsWith("返现"))
                {
                    string pattern = @"\D";
                    Regex reg = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    string numstr = reg.Replace(requestEntity.Content, "");

                    long orderID = 0;
                    bool isNum = long.TryParse(numstr, out orderID);

                    //return RequestRebate(toUser, fromuserName, createTime, orderID);
                    responseEntity.ResponseEvent = "RequestRebate";
                    responseEntity.ResponseContent = "";

                    requestEntity.OrderID = orderID;
                }

                #endregion

                #region zeta

                else if (requestEntity.Content.ToLower().StartsWith("zeta"))
                {
                    //string Content = WeiXinAdapter.LogWeiXinInfo(toUser, content);
                    //return WeixinHelper.GenWeiXinTextReturn(Content, requestEntity.FromUserName, requestEntity.ToUserName, createTime);
                    responseEntity.ResponseEvent = "LogWeiXinInfo";
                    responseEntity.ResponseContent = "";
                }

                #endregion

                #region zmjd-zeta

                else if (requestEntity.Content.ToLower().Equals("zmjd-zeta"))
                {
                    string Content = "感谢关注周末酒店，请输入zeta+您的手机号码，比如“zeta13980001800”";
                    //return WeixinHelper.GenWeiXinTextReturn(Content, requestEntity.FromUserName, requestEntity.ToUserName, createTime);
                    responseEntity.ResponseEvent = "";
                    responseEntity.ResponseContent = WeixinHelper.GenWeiXinTextReturn(Content, requestEntity.FromUserName, requestEntity.ToUserName, createTime);
                }

                #endregion

                #region

                else
                {
                    //先检查是否与活动有关，用户内容开头包含了某一个活动的“活动关键字”，则认为参与该活动
                    requestEntity.EnrollActivity = requestEntity.ActivityList.Find(a => WeixinHelper.ContainsActiveKeywords(a.ActivityKeyWord, requestEntity.Content));

                    //检测请求内容中包含的手机号
                    long orderID = 0;
                    var keywords = (requestEntity.EnrollActivity != null && !string.IsNullOrEmpty(requestEntity.EnrollActivity.ActivityKeyWord) ? requestEntity.EnrollActivity.ActivityKeyWord : "");
                    bool isNum = WeixinHelper.GetContentOrderId(requestEntity.Content, keywords, out orderID);
                    requestEntity.OrderID = orderID;

                    #region 微信活动

                    //在活动期内 & 包含活动关键字 & 包含手机号码 (暂时先不验证活动开始时间)
                    if (requestEntity.EnrollActivity != null) //&& isNum && orderID.ToString().Length == 11 && orderID > 12000000000 && orderID < 20000000000)
                    {
                        if (DateTime.Now < requestEntity.EnrollActivity.ActivityFinishDateTime) // || true
                        {
                            //记录参与活动 HELLO
                            if (requestEntity.Content.Contains("爱的故事"))
                            {
                                WeixinHelper.CacheCustomHelloTip(requestEntity.FromUserName);
                            }

                            responseEntity.ResponseEvent = "AddActiveLuckyDraw";
                            responseEntity.ResponseContent = "";
                        }
                        else
                        {
                            responseEntity.ResponseEvent = "";
                            responseEntity.ResponseContent = WeixinHelper.GenWeiXinTextReturn(requestEntity.EnrollActivity.ActivityFinishWord, requestEntity.FromUserName, requestEntity.ToUserName, createTime);
                        }
                    }

                    #endregion

                    else
                    {
                        //自定义验证
                        string customEvent = "", customContent = "";
                        if (WeixinHelper.CustomAction(requestEntity.Content, requestEntity.FromUserName, ref customEvent, ref customContent))
                        {
                            responseEntity.ResponseEvent = customEvent;
                            responseEntity.ResponseContent = customContent;
                        }
                        else
                        {
                            string strReturn = WeixinHelper.getResponseForTextInput2(requestEntity.Content, requestEntity.FromUserName, requestEntity.ToUserName, createTime);
                            if (string.IsNullOrEmpty(strReturn))
                            {
                                string Content = "";

                                //检查主题查找返回机制
                                Content = WeixinHelper.ThemeDataMachine(requestEntity.Content, requestEntity.FromUserName, requestEntity.ToUserName);
                                if (string.IsNullOrEmpty(Content))
                                {
                                    //检查城市地点查找返回机制
                                    Content = WeixinHelper.CityDataMachine(requestEntity.Content, requestEntity.FromUserName, requestEntity.ToUserName);

                                    #region 酒店相关信息检索功能

                                    if (string.IsNullOrEmpty(Content))
                                    {
                                        ////在转入多客服之前，进行酒店名称的匹配检索（纯文字链接方式返回）
                                        //Content = WeixinHelper.SearchRelationHotels(requestEntity.Content);  

                                        //搜索的匹配项目数目
                                        var articleCount = 0;

                                        //在转入多客服之前，进行酒店名称的匹配检索
                                        Content = WeixinHelper.SearchRelationHotelItems(requestEntity.Content, ref articleCount);
                                        if (!string.IsNullOrEmpty(Content) && articleCount > 0)
                                        {
                                            strReturn = WeixinHelper.GenWeiXinTextForLinkArticleReturn(Content, requestEntity.FromUserName, requestEntity.ToUserName, createTime, articleCount);
                                        }
                                    }

                                    #endregion

                                    #region 多客服

                                    if (string.IsNullOrEmpty(Content))
                                    {
                                        //if (DateTime.Now.Hour > 23 || DateTime.Now.Hour < 8)
                                        //{
                                        //    Content = "您好，我们的微信客服时间是每天早上8点至晚上12点，您的需求已经收到，我们会在工作时间尽快回复您。";
                                        //}
                                        //else
                                        //{
                                        //    //转入多客服系统
                                        //    //return WeixinHelper.GenWeiXinTextForTransfer_Customer_Service(requestEntity.FromUserName, requestEntity.ToUserName, createTime);
                                        //    responseEntity.ResponseEvent = "";
                                        //    responseEntity.ResponseContent = WeixinHelper.GenWeiXinTextForTransfer_Customer_Service(requestEntity.FromUserName, requestEntity.ToUserName, createTime);
                                        //    responseEntity.RequestEntity = requestEntity;

                                        //    return responseEntity;
                                        //}

                                        //转入多客服系统
                                        //return WeixinHelper.GenWeiXinTextForTransfer_Customer_Service(requestEntity.FromUserName, requestEntity.ToUserName, createTime);
                                        responseEntity.ResponseEvent = "";
                                        responseEntity.ResponseContent = WeixinHelper.GenWeiXinTextForTransfer_Customer_Service(requestEntity.FromUserName, requestEntity.ToUserName, createTime);
                                        responseEntity.RequestEntity = requestEntity;

                                        return responseEntity;
                                    }

                                    #endregion
                                }

                                if (string.IsNullOrEmpty(strReturn)) strReturn = WeixinHelper.GenWeiXinTextReturn(Content, requestEntity.FromUserName, requestEntity.ToUserName, createTime);
                            }

                            //return strReturn;
                            responseEntity.ResponseEvent = "";
                            responseEntity.ResponseContent = strReturn;
                            responseEntity.RequestEntity = requestEntity;
                        }
                    }
                }

                #endregion

            }
            catch (Exception ex)
            {
                File.AppendAllText(string.Format(@"D:\Log\WeixinService\ErrorLog_{0}.txt", DateTime.Now.ToString("yyyyMMdd")), ex.Message + "\r\n" + ex.StackTrace + "\r\n");
            }

            return responseEntity;
        }

        public ResponseEntity GetWeixinResponseImage(RequestEntity requestEntity)
        {
            ResponseEntity responseEntity = new ResponseEntity { RequestEntity = requestEntity };

            #region 爱的故事 活动检测

            string customEvent = "", customContent = "";
            if (WeixinHelper.CustomActionImage(requestEntity.Content, requestEntity.FromUserName, ref customEvent, ref customContent))
            {
                responseEntity.ResponseEvent = customEvent;
                responseEntity.ResponseContent = customContent;
            }

            #endregion

            return responseEntity;
        }

        public string GetTicket()
        {
            return WeiXinToken.GetTicket();
        }

        public string GetToken()
        {
            return WeiXinToken.GetToken();
        }

        public ResultEntity Add_News(List<NewsEntity> articles)
        {
            return WeixinHelper.Add_News(articles);
        }

        public ResultEntity Add_Material(string fileUrl, string fileName)
        {
            return WeixinHelper.Add_Material(fileUrl, fileName);
        }

         public ResultEntity Update_News(string media_id, NewsEntity article)
         {
             return WeixinHelper.Update_News(media_id, article);
         }

        public ResultEntity Del_Material(string media_id)
        {
            return WeixinHelper.Del_Material(media_id);
        }

        public List<WeixinActivityEntity> GetWeixinActives()
        {
            var weixinActives = LocalCache1h.GetData<List<WeixinActivityEntity>>("GetWeixinActives", () => { return CommDAL.GetWeixinActives(); });

            return weixinActives;
        }

        /// <summary>
        /// 获取指定ID的微信活动
        /// </summary>
        /// <param name="activeId"></param>
        /// <returns></returns>
        public WeixinActivityEntity GetOneWeixinActive(int activeId)
        {
            var weixinActiveEntity = LocalCache1h.GetData<WeixinActivityEntity>(string.Format("GetWeixinActives_{0}", activeId), () => 
            {
                var activeEntity = new WeixinActivityEntity();
                var weixinActiveList = CommDAL.GetOneWeixinActive(activeId);
                if (weixinActiveList != null && weixinActiveList.Count > 0 && weixinActiveList.Exists(w => w.ActivityID == activeId))
                {
                    activeEntity = weixinActiveList.Find(w => w.ActivityID == activeId);
                }

                return activeEntity;
            });

            return weixinActiveEntity;
        }

        public bool RefWeixinActivesCache()
        {
            lock (_obj)
            {
                try
                {
                    LocalCache1h.Set("GetWeixinActives", CommDAL.GetWeixinActives());   
                }
                catch (Exception ex)
                {
                    
                }
            }

            return true;
        }

        public bool RefOneWeixinActivesCache(int activeId)
        {
            lock (_obj)
            {
                try
                {
                    var activeEntity = new WeixinActivityEntity();
                    var weixinActiveList = CommDAL.GetOneWeixinActive(activeId);
                    if (weixinActiveList != null && weixinActiveList.Count > 0 && weixinActiveList.Exists(w => w.ActivityID == activeId))
                    {
                        activeEntity = weixinActiveList.Find(w => w.ActivityID == activeId);
                    }

                    if (activeEntity != null)
                    {
                        LocalCache1h.Set(string.Format("GetWeixinActives_{0}", activeId), activeEntity);   
                    }
                }
                catch (Exception ex)
                {

                }
            }

            return true;
        }

        public List<WeixinActivityEntity> GetWeixinActives2()
        {
            return CommDAL.GetWeixinActives();
        }

        public bool EditWeixinActive(WeixinActivityEntity wa)
        {
            return CommDAL.EditWeixinActive(wa) > 0;
        }

        public bool IsPartWeixinActivity(int acode, string phone)
        {
            return CommDAL.IsPartWeixinActivity(acode, phone);
        }

        public List<ActiveWeixinLuckyReport> GetActiveWeixinLuckyReport(int activeId, int luckcode)
        {
            return CommDAL.GetActiveWeixinLuckyReport(activeId, luckcode);
        }

        public ActiveWeixinLuckyUser GetActiveWeixinLuckyUser(int activeId, int luckcode)
        {
            return CommDAL.GetActiveWeixinLuckyUser(activeId, luckcode);
        }

        #region 微信扩展信息分组处理

        public List<ActiveRuleGroup> GetActiveRuleGroups()
        {
            return CommDAL.GetActiveRuleGroups();
        }

        public bool EditActiveRuleGroup(ActiveRuleGroup obj)
        {
            return CommDAL.EditActiveRuleGroup(obj) > 0;
        }

        public List<ActiveRuleEx> GetActiveRuleExs()
        {
            return CommDAL.GetActiveRuleExs();
        }

        public bool EditActiveRuleEx(ActiveRuleEx obj)
        {
            return CommDAL.EditActiveRuleEx(obj) > 0;
        }

        /// <summary>
        /// 根据活动ID获取扩展表信息（根据activeid关联groupid，再通过groupid查询exs）
        /// </summary>
        /// <param name="activeId"></param>
        /// <returns></returns>
        public List<ActiveRuleEx> GetActiveRuleExsByActiveId(int activeId)
        {
            return CommDAL.GetActiveRuleExsByActiveId(activeId);
        }

        /// <summary>
        /// 【For Vote】根据活动ID获取投票活动list（根据activeid关联groupid，再通过groupid查询exs）
        /// </summary>
        /// <param name="activeId"></param>
        /// <returns></returns>
        public List<ActiveRuleExForVote> GetActiveRuleExsForVoteByActiveId(int activeId)
        {
            return CommDAL.GetActiveRuleExsForVoteByActiveId(activeId);
        }

        /// <summary>
        /// 【For Vote】根据活动ID和RuleExID获取投票活动list（根据activeid关联groupid，再通过groupid查询exs）
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="exid"></param>
        /// <returns></returns>
        public ActiveRuleExForVote GetActiveRuleExsForVoteByActiveIdAndID(int activeId, int exid)
        {
            return CommDAL.GetActiveRuleExsForVoteByActiveIdAndID(activeId, exid);
        }

        /// <summary>
        /// 获取指定sourceid的奖品信息
        /// </summary>
        /// <param name="sourceId"></param>
        /// <returns></returns>
        public List<ActiveRulePrize> GetActiveRulePrizeBySourceId(int activeId, int sourceId)
        {
            return CommDAL.GetActiveRulePrizeBySourceId(activeId, sourceId);
        }

        /// <summary>
        /// 获取指定报名用户的抽奖记录
        /// </summary>
        /// <param name="activeDrawId"></param>
        /// <returns></returns>
        public List<ActiveLuckDrawRecord> GetActiveLuckDrawRecordByDrawId(int activeDrawId)
        {
            return CommDAL.GetActiveLuckDrawRecordByDrawId(activeDrawId);
        }

        /// <summary>
        /// 获取指定报名用户的抽奖记录（包含奖品详细信息）
        /// </summary>
        /// <param name="activeDrawId"></param>
        /// <returns></returns>
        public List<ActiveLuckDrawRecordContainPrize> GetActiveLuckRecordAndPrizeByDrawId(int activeDrawId)
        {
            return CommDAL.GetActiveLuckRecordAndPrizeByDrawId(activeDrawId);
        }

        /// <summary>
        /// 插入抽奖记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int AddActiveLuckDrawRecord(ActiveLuckDrawRecord entity)
        {
            return CommDAL.AddActiveLuckDrawRecord(entity);
        }

        /// <summary>
        /// 获取指定活动id和RuleExId的代言人信息
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="exid"></param>
        /// <returns></returns>
        public List<ActiveRuleSpokesmanAndDrawInfo> GetActiveSpokesmanInfoByActiveIdAndExId(int activeId, int exid)
        {
            return CommDAL.GetActiveSpokesmanInfoByActiveIdAndExId(activeId, exid);
        }

        /// <summary>
        /// 获取指定活动ID指定wx openid的投票记录
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="weixinAccount">wx openid</param>
        /// <returns></returns>
        public List<ActiveVoteRecord> GetActiveVoteRecordForType1ByWxAccount(int activeId, string weixinAccount)
        {
            return CommDAL.GetActiveVoteRecordForType1ByWxAccount(activeId, weixinAccount);
        }

        /// <summary>
        /// 获取指定活动ID指定sourceid(draw id)的被投票记录
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="sourceId">(draw id)</param>
        /// <returns></returns>
        public List<ActiveVoteRecord> GetActiveVoteRecordForType2BySourceId(int activeId, int sourceId)
        {
            return CommDAL.GetActiveVoteRecordForType2BySourceId(activeId, sourceId);
        }

        /// <summary>
        /// 查询通过当前drawid（包括自己）投过该酒店的记录
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="sourceId">大使ID（包括自己）</param>
        /// <param name="reltionId">酒店ID</param>
        /// <param name="weixinAccount"></param>
        /// <returns></returns>
        public List<ActiveVoteRecord> GetActiveVoteRecordForType2BySourceIdAndReltionId(int activeId, int sourceId, int reltionId, string weixinAccount)
        {
            return CommDAL.GetActiveVoteRecordForType2BySourceIdAndReltionId(activeId, sourceId, reltionId, weixinAccount);
        }

        /// <summary>
        /// 新增投票
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int AddActiveVoteRecord(ActiveVoteRecord entity)
        {
            return CommDAL.AddActiveVoteRecord(entity);
        }

        /// <summary>
        /// 获取指定用户报名ID代言的RuleEx记录
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="activeDrawId">用户报名ID</param>
        /// <returns></returns>
        public List<ActiveRuleExForVote> GetActiveRuleExsForVoteByDrawId(int activeId, int activeDrawId)
        {
            return CommDAL.GetActiveRuleExsForVoteByDrawId(activeId, activeDrawId);
        }

        /// <summary>
        /// 新增指定活动的代言记录
        /// </summary>
        /// <param name="entity">代言关联对象</param>
        /// <returns></returns>
        public int AddActiveRuleSpokesman(ActiveRuleSpokesman entity)
        {
            return CommDAL.AddActiveRuleSpokesman(entity);
        }

        /// <summary>
        /// 累加指定ActiveRuleEx Id的OfferCount字段
        /// </summary>
        /// <param name="activeRuleExId">ActiveRuleEx主键Id</param>
        /// <returns></returns>
        public bool AddUpActiveRuleExOfferCountById(int activeRuleExId)
        {
            return CommDAL.AddUpActiveRuleExOfferCountById(activeRuleExId) > 0;
        }

        #endregion

        #region 微信h5

        /// <summary>
        /// 根据openid获取微信用户授权得到的信息
        /// </summary>
        /// <param name="openid"></param>
        /// <returns></returns>
        public ActiveWeixinUser GetActiveWeixinUser(string openid)
        {
            return CommDAL.GetActiveWeixinUser(openid);
        }

        public ActiveWeixinUser GetActiveWeixinUserById(int id)
        {
            return CommDAL.GetActiveWeixinUserById(id);
        }

        /// <summary>
        /// 添加一个以openid为主键的微信用户记录
        /// </summary>
        /// <param name="aw"></param>
        /// <returns></returns>
        public int AddActiveWeixinUser(ActiveWeixinUser aw)
        {
            return CommDAL.AddActiveWeixinUser(aw);
        }

        /// <summary>
        /// 根据活动ID和openid获取微信用户的报名记录
        /// </summary>
        /// <param name="openid"></param>
        /// <returns></returns>
        public ActiveWeixinDraw GetActiveWeixinDraw(int activeId, string openid)
        {
            return CommDAL.GetActiveWeixinDraw(activeId, openid);
        }

        /// <summary>
        /// 根据用户报名手机号和活动Id获取报名记录
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        public ActiveWeixinDraw GetActiveWeixinDrawByPhone(int activeId, string phone)
        {
            return CommDAL.GetActiveWeixinDrawByPhone(activeId, phone);
        }

        /// <summary>
        /// 根据报名Id和活动Id获取报名记录
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActiveWeixinDraw GetActiveWeixinDrawById(int activeId, int id)
        {
            return CommDAL.GetActiveWeixinDrawById(activeId, id);
        }

        /// <summary>
        /// 添加一个以openid为主键的微信用户指定活动报名记录
        /// </summary>
        /// <param name="aw"></param>
        /// <returns></returns>
        public int AddActiveWeixinDraw(ActiveWeixinDraw aw)
        {
            return CommDAL.AddActiveWeixinDraw(aw);
        }

        public int UpdateActiveWeixinDrawIsShare(int activeId, string openid)
        {
            return CommDAL.UpdateActiveWeixinDrawIsShare(activeId, openid);
        }

        public int UpdateActiveWeixinDrawIsPay(int activeId, string openid)
        {
            return CommDAL.UpdateActiveWeixinDrawIsPay(activeId, openid);
        }

        public int UpdateActiveWeixinDrawPhone(int activeId, string openid, string phone)
        {
            return CommDAL.UpdateActiveWeixinDrawPhone(activeId, openid, phone);
        }

        public int UpdateActiveWeixinDrawHeadImgUrl(int activeId, string openid, string headimgurl)
        {
            return CommDAL.UpdateActiveWeixinDrawHeadImgUrl(activeId, openid, headimgurl);
        }

        public int UpdateActiveWeixinDrawSendCount(int activeId, string openid)
        {
            return CommDAL.UpdateActiveWeixinDrawSendCount(activeId, openid);
        }

        /// <summary>
        /// 获取指定openid阅读过的分享者
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="openid"></param>
        /// <returns></returns>
        public List<ActiveWeixinDraw> GetActiveWeixinDrawByReadUser(int activeId, string openid)
        {
            return CommDAL.GetActiveWeixinDrawByReadUser(activeId, openid);
        }

        /// <summary>
        /// 获取指定活动指定分享者的被阅读记录
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="shareOpenid"></param>
        /// <returns></returns>
        public List<ActiveWeixinShareRead> GetActiveWeixinShareReadList(int activeId, string shareOpenid)
        {
            return CommDAL.GetActiveWeixinShareReadList(activeId, shareOpenid);
        }

        /// <summary>
        /// 插入新的阅读记录
        /// </summary>
        /// <param name="aw"></param>
        /// <returns></returns>
        public int AddActiveWeixinShareRead(ActiveWeixinShareRead aw)
        {
            return CommDAL.AddActiveWeixinShareRead(aw);
        }

        /// <summary>
        /// 查询微信报名统计
        /// </summary>
        /// <param name="activeId"></param>
        /// <returns></returns>
        public ActiveWeixinStatResult GetWeixinAtvStatResult(int activeId)
        {
            return CommDAL.GetWeixinAtvStatResult(activeId);
        }

        /// <summary>
        /// 查询微信报名统计(只返回抽奖码数量)
        /// </summary>
        /// <param name="activeId"></param>
        /// <returns></returns>
        public ActiveWeixinStatResult GetActiveWeixinLuckCodeCount(int activeId)
        {
            return CommDAL.GetActiveWeixinLuckCodeCount(activeId);
        }

        /// <summary>
        /// 获取指定openid的WeixinUser
        /// </summary>
        /// <param name="openid"></param>
        /// <returns></returns>
        public WeixinUser GetWeixinUser(string openid)
        {
            return CommDAL.GetWeixinUser(openid);
        }

        /// <summary>
        /// 获取指定id的WeixinUser
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public WeixinUser GetWeixinUserById(int id)
        {
            return CommDAL.GetWeixinUserById(id);
        }

        /// <summary>
        /// 获取指定unionid和weixinaccount的weixinuser信息
        /// </summary>
        /// <param name="unionid"></param>
        /// <param name="weixinAccount"></param>
        /// <returns></returns>
        public WeixinUser GetWeixinUserByUnionidAndAccount(string unionid, string weixinAccount)
        {
            return CommDAL.GetWeixinUserByUnionidAndAccount(unionid, weixinAccount);
        }

        /// <summary>
        /// add WeixinRewardRecord（by fromwxuid）
        /// </summary>
        /// <param name="wr"></param>
        /// <returns></returns>
        public int AddWeixinRewardRecordByWxuid(WeixinRewardRecord wr)
        {
            if (string.IsNullOrEmpty(wr.Remark))
            {
                wr.Remark = "周末酒店红包";
            }

            wr.ActiveName = "分享有礼";

            wr.WillSendTime = DateTime.Now;

            return CommDAL.AddWeixinRewardRecordByWxuid(wr);
        }

        /// <summary>
        /// add WeixinRewardRecord
        /// </summary>
        /// <param name="wr"></param>
        /// <returns></returns>
        public int AddWeixinRewardRecord(WeixinRewardRecord wr)
        {
            return CommDAL.AddWeixinRewardRecord(wr);
        }
        
        /// <summary>
        /// add WeixinRewardRecord（for wx redpack union active）
        /// </summary>
        /// <param name="wr"></param>
        /// <returns></returns>
        public int AddWeixinRewardRecordForRedpackUnion(WeixinRewardRecord wr)
        {
            return CommDAL.AddWeixinRewardRecordForRedpackUnion(wr);
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
            return CommDAL.GetWeixinRewardRecordByWxActive(sourceType, activeId, reOpenid);
        }

        /// <summary>
        /// 扣除相关合作伙伴的指定资金
        /// </summary>
        /// <param name="partnerId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public int UpdateWxPartnerActiveFund(long partnerId, decimal value)
        {
            return CommDAL.UpdateWxPartnerActiveFund(partnerId, value);
        }

        #region 【最新】

        /// <summary>
        /// 插入新的阅读记录【最新】
        /// </summary>
        /// <param name="aw"></param>
        /// <returns></returns>
        public int AddActiveWeixinShareRead_Luck(ActiveWeixinShareRead aw)
        {
            return CommDAL.AddActiveWeixinShareRead_Luck(aw);
        }

        /// <summary>
        /// 插入新的抽奖码记录
        /// </summary>
        /// <param name="aluck"></param>
        /// <returns></returns>
        public int AddActiveWeixinLuckCode(ActiveWeixinLuckCode aluck)
        {
            return CommDAL.AddActiveWeixinLuckCode(aluck);
        }

        /// <summary>
        /// 插入新的虚拟价值（如宝石）记录
        /// </summary>
        /// <param name="aluck"></param>
        /// <returns></returns>
        public int AddActiveWeixinFicMoney(ActiveWeixinFicMoney aficmoney)
        {
            return CommDAL.AddActiveWeixinFicMoney(aficmoney);
        }

        /// <summary>
        /// 获取指定活动指定用户的虚拟价值（如宝石）获得情况
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="openid"></param>
        /// <returns></returns>
        public List<ActiveWeixinFicMoney> GetActiveWeixinFicMoneyInfo(int activeId, string openid)
        {
            return CommDAL.GetActiveWeixinFicMoneyInfo(activeId, openid);
        }

        /// <summary>
        /// 发布增加新抽奖码的队列任务
        /// </summary>
        /// <param name="aluck"></param>
        /// <returns></returns>
        public bool PublishWeixinLuckCodeTask(ActiveWeixinLuckCode aluck)
        {
            return new WeixinHelper().PublishWeixinLuckCodeTask(aluck);
        }

        /// <summary>
        /// 发布增加宝石的队列任务
        /// </summary>
        /// <param name="aluck"></param>
        /// <returns></returns>
        public bool PublishWeixinGemTask(ActiveWeixinLuckCode aluck)
        {
            return new WeixinHelper().PublishWeixinGemTask(aluck);
        }

        /// <summary>
        /// 获取指定活动指定用户的抽奖码获得情况
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="openid"></param>
        /// <returns></returns>
        public List<ActiveWeixinLuckCode> GetActiveWeixinLuckCodeInfo(int activeId, string openid)
        {
            return CommDAL.GetActiveWeixinLuckCodeInfo(activeId, openid);
        }

        /// <summary>
        /// 获取指定活动指定TagName的抽奖码获得情况
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="tagName">抽奖码标签，如 报名奖励、翻倍卡助力 等</param>
        /// <returns></returns>
        public List<ActiveWeixinLuckCode> GetActiveWeixinLuckCodeInfoByTagName(int activeId, string tagName)
        {
            return CommDAL.GetActiveWeixinLuckCodeInfoByTagName(activeId, tagName);
        }

        /// <summary>
        /// 获取指定活动指定TagName指定SourceOpenid的抽奖码获得情况
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="tagName">抽奖码标签，如 报名奖励、翻倍卡助力 等</param>
        /// <param name="sourceOpenid">一般为该抽奖码的助力人</param>
        /// <returns></returns>
        public List<ActiveWeixinLuckCode> GetActiveWeixinLuckCodeInfoByTagNameAndSource(int activeId, string tagName, string sourceOpenid)
        {
            return CommDAL.GetActiveWeixinLuckCodeInfoByTagNameAndSource(activeId, tagName, sourceOpenid);
        }

        /// <summary>
        /// 获取所有微信合作伙伴信息
        /// </summary>
        /// <returns></returns>
        public List<ActiveWeixinPartner> GetAllWeixinPartners()
        {
            return CommDAL.GetAllWeixinPartners();
        }

        /// <summary>
        /// 添加/修改微信合作伙伴
        /// </summary>
        /// <param name="wp"></param>
        /// <returns></returns>
        public int AddActiveWeixinPartner(ActiveWeixinPartner wp)
        {
            return CommDAL.AddActiveWeixinPartner(wp);
        }

        #endregion

        #region 【1来5往】



        #endregion

        #endregion

        #region 微信公众号操作相关

        /// <summary>
        /// 更新微信用户的用户信息，存在则只更新关注状态和时间（如是否关注）
        /// </summary>
        /// <param name="w"></param>
        /// <returns></returns>
        public int UpdateWeixinUserSubscribe(WeixinUser w)
        {
            if (string.IsNullOrEmpty(w.Unionid))
            {
                var new_w = WeixinHelper.GetWeixinUserSubscribeInfo(w.Openid);
                if (new_w != null && !string.IsNullOrEmpty(new_w.Openid))
                {
                    new_w.ID = w.ID;
                    new_w.WeixinAcount = w.WeixinAcount;
                    new_w.CreateTime = w.CreateTime;
                    if (new_w.Subscribe == 0 || new_w.SubscribeTime < DateTime.Parse("2000-01-01")) new_w.SubscribeTime = DateTime.Now;
                    return CommDAL.UpdateWeixinUserSubscribe(new_w);
                }
            }

            return CommDAL.UpdateWeixinUserSubscribe(w);
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="w"></param>
        /// <returns></returns>
        public int UpdateWeixinUserInfo(WeixinUser w)
        {
            return CommDAL.UpdateWeixinUserInfo(w);
        }

        /// <summary>
        /// 获取指定Openid的用户信息
        /// </summary>
        /// <param name="openid"></param>
        /// <param name="backJson"></param>
        /// <returns></returns>
        public WeixinUser GetWeixinUserSubscribeInfo(string openid, WeiXinChannelCode ChannelCode = WeiXinChannelCode.周末酒店订阅号)
        {
            return memCacheWeixin.GetData<WeixinUser>(openid, "WeixinUserInfo:", () =>
            {
                return WeixinHelper.GetWeixinUserSubscribeInfo(openid, ChannelCode);
            });          
        }

        /// <summary>
        /// 获取指定Openid的用户信息
        /// </summary>
        /// <param name="openid"></param>
        /// <param name="backJson"></param>
        /// <returns></returns>
        public WeixinUser GetWeixinUserSubscribeInfo2(string openid, int channelCode = 1)
        {
            return memCacheWeixin.GetData<WeixinUser>(string.Format("{0}_{1}", openid, channelCode), "WeixinUserInfo:", () =>
            {
                return WeixinHelper.GetWeixinUserSubscribeInfo2(openid, channelCode);
            });
        }

        /// <summary>
        /// 查询指定unionid指定微信号下的用户信息（如关注状态）
        /// </summary>
        /// <param name="weixinAcount"></param>
        /// <param name="unionid"></param>
        /// <returns></returns>
        public WeixinUser GetWeixinUserByUnionid(string weixinAcount, string unionid)
        {
            return CommDAL.GetWeixinUserByUnionid(weixinAcount, unionid);
        }

        /// <summary>
        /// 获取指定微信账号的token
        /// </summary>
        /// <param name="weixinAcount"></param>
        /// <returns></returns>
        public string GetWeixinToken(string weixinAcount)
        {
            switch (weixinAcount.ToLower().Trim())
            {
                case "zmjiudian":
                    {
                        return WeiXinToken.GetToken();
                    }
                case "liuwa":
                    {
                        return HaoYiLiuwaWeiXinToken.GetToken();
                    }
                case "liuwaservice":
                    {
                        return HaoYiServiceLiuwaWeiXinToken.GetToken();
                    }
                case "hellozmjiudian":
                    {
                        return WeiXinServiceToken.GetToken();
                    } 
            }

            return "";
        }


        public string GetWeixinTokenByStrCode(string strCode, bool forceReFresh)
        {

           WeiXinChannelCode code = WeixinHelper.GetWeiXinChannelCodeByWeiXinOrgID(strCode);

           if (forceReFresh)
           {
                return GenWeixinTokenByCode(code);
           }
           else
           {
               return GetWeixinTokenByCode(code);
           }
        }

        public string GetWeixinTokenByCode(WeiXinChannelCode code)
        {
            switch (code)
            {
                case WeiXinChannelCode.周末酒店订阅号: 
                        return WeiXinToken.GetToken(); 
                case WeiXinChannelCode.周末酒店服务号: 
                        return WeiXinServiceToken.GetToken(); 
                case WeiXinChannelCode.尚旅游订阅号:
                    return ShangLvYouWeiXinToken.GetToken();  
                case WeiXinChannelCode.周末酒店服务号_皓颐:
                    return HaoYiServiceWeiXinToken.GetTicket();
                case WeiXinChannelCode.周末酒店苏州服务号_皓颐:
                    return HaoYiServiceSZWeiXinToken.GetTicket();
                case WeiXinChannelCode.周末酒店成都服务号_皓颐:
                    return HaoYiServiceCDWeiXinToken.GetTicket();
                case WeiXinChannelCode.周末酒店深圳服务号_皓颐:
                    return HaoYiServiceShenZWeiXinToken.GetTicket(); 
                case WeiXinChannelCode.遛娃指南服务号_皓颐:
                    return HaoYiServiceLiuwaWeiXinToken.GetTicket();
                case WeiXinChannelCode.遛娃指南南京服务号_皓颐:
                    return HaoYiServiceLiuwaNJWeiXinToken.GetTicket();
                case WeiXinChannelCode.遛娃指南无锡服务号_皓颐:
                    return HaoYiServiceLiuwaWXWeiXinToken.GetTicket();
                case WeiXinChannelCode.遛娃指南广州服务号_皓颐:
                    return HaoYiServiceLiuwaGZWeiXinToken.GetTicket();
                case WeiXinChannelCode.遛娃指南杭州服务号_皓颐:
                    return HaoYiServiceLiuwaHZHWeiXinToken.GetTicket();
                case WeiXinChannelCode.遛娃指南深圳订阅号_皓颐:
                    return HaoYiLiuwaSHZHWeiXinToken.GetTicket();
                case WeiXinChannelCode.遛娃指南:
                    return HaoYiServiceLiuwaWeiXinToken.GetTicket();
            }

            return "";
        }

        public string GenWeixinTokenByCode(WeiXinChannelCode code)
        {
            switch (code)
            {
                case WeiXinChannelCode.周末酒店订阅号:
                    return WeiXinToken.GenToken();
                case WeiXinChannelCode.周末酒店服务号:
                    return WeiXinServiceToken.GenToken();
                case WeiXinChannelCode.尚旅游订阅号:
                    return ShangLvYouWeiXinToken.GenTicket();
                case WeiXinChannelCode.周末酒店服务号_皓颐:
                    return HaoYiServiceWeiXinToken.GenTicket();
                case WeiXinChannelCode.周末酒店苏州服务号_皓颐:
                    return HaoYiServiceSZWeiXinToken.GetTicket();
                case WeiXinChannelCode.周末酒店成都服务号_皓颐:
                    return HaoYiServiceCDWeiXinToken.GetTicket();
                case WeiXinChannelCode.周末酒店深圳服务号_皓颐:
                    return HaoYiServiceShenZWeiXinToken.GetTicket();
                case WeiXinChannelCode.遛娃指南服务号_皓颐:
                    return HaoYiServiceLiuwaWeiXinToken.GetTicket();
            }

            return "";
        }

        /// <summary>
        /// 指定微信公众号发送自定义信息【Text】
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool SendWeixinTextMsg(WeixinMsgText msg)
        {
            if (msg == null || string.IsNullOrEmpty(msg.Msgtype) || string.IsNullOrEmpty(msg.Touser))
            {
                return false;
            }

            if (msg.WeixinAcount.ToLower().Trim() != "zmjiudian" && msg.WeixinAcount.ToLower().Trim() != "hellozmjiudian")
            {
                return false;
            }

            msg.Msgtype = "text";

            var token = GetWeixinToken(msg.WeixinAcount);
            WeixinHelper.SendWeixinMsg(token, msg);

            return true;
        }

        /// <summary>
        /// 指定微信公众号发送自定义信息【News】
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool SendWeixinNewsMsg(WeixinMsgNews msg)
        {
            if (msg == null || string.IsNullOrEmpty(msg.Msgtype) || string.IsNullOrEmpty(msg.Touser))
            {
                return false;
            }

            if (msg.WeixinAcount.ToLower().Trim() != "zmjiudian" && msg.WeixinAcount.ToLower().Trim() != "hellozmjiudian")
            {
                return false;
            }

            msg.Msgtype = "news";

            var token = GetWeixinToken(msg.WeixinAcount);
            WeixinHelper.SendWeixinMsg(token, msg);

            return true;
        }

        /// <summary>
        /// 更新微信自定义菜单
        /// </summary>
        /// <param name="MenuInfo"></param>
        /// <param name="acountId">1周末酒店订阅号 2周末酒店服务号 3尚旅游 4尚旅游成都 5尚旅游北京 6美味至尚 7周末酒店服务号_浩颐 8周末酒店苏州服务号 9周末酒店成都服务号 10周末酒店深圳服务号</param>
        /// <returns></returns>
        public string UpdateWeiXinMenu(string MenuInfo, int acountId = 1)
        {
            switch ((WeiXinChannelCode)acountId)
            {
                case WeiXinChannelCode.周末酒店订阅号:
                    return WeiXinToken.UpdateMenu(MenuInfo);
                    break;
                case WeiXinChannelCode.周末酒店服务号:
                    return WeiXinServiceToken.UpdateMenu(MenuInfo);
                    break;
                case WeiXinChannelCode.尚旅游订阅号:
                    return ShangLvYouWeiXinToken.UpdateMenu(MenuInfo);
                    break;
                case WeiXinChannelCode.尚旅游成都订阅号:
                    return ShangLvYouChengduWeiXinToken.UpdateMenu(MenuInfo);
                    break;
                case WeiXinChannelCode.美味至尚订阅号:
                    break;
                case WeiXinChannelCode.尚旅游北京订阅号:
                    return ShangLvYouBeijingWeiXinToken.UpdateMenu(MenuInfo);
                    break;
                case WeiXinChannelCode.周末酒店服务号_皓颐:
                    return HaoYiServiceWeiXinToken.UpdateMenu(MenuInfo);
                    break;
                case WeiXinChannelCode.周末酒店苏州服务号_皓颐:
                    return HaoYiServiceSZWeiXinToken.UpdateMenu(MenuInfo);
                    break;
                case WeiXinChannelCode.周末酒店成都服务号_皓颐:
                    return HaoYiServiceCDWeiXinToken.UpdateMenu(MenuInfo);
                    break;
                case WeiXinChannelCode.周末酒店深圳服务号_皓颐:
                    return HaoYiServiceShenZWeiXinToken.UpdateMenu(MenuInfo);
                    break;
                case WeiXinChannelCode.遛娃指南服务号_皓颐:
                    return HaoYiServiceLiuwaWeiXinToken.UpdateMenu(MenuInfo);
                    break;
                case WeiXinChannelCode.遛娃指南:
                    return HaoYiLiuwaWeiXinToken.UpdateMenu(MenuInfo);
                    break;
                case WeiXinChannelCode.遛娃指南南京服务号_皓颐:
                    return HaoYiServiceLiuwaNJWeiXinToken.UpdateMenu(MenuInfo);
                    break;
                case WeiXinChannelCode.遛娃指南无锡服务号_皓颐:
                    return HaoYiServiceLiuwaWXWeiXinToken.UpdateMenu(MenuInfo);
                    break;
                case WeiXinChannelCode.遛娃指南广州服务号_皓颐:
                    return HaoYiServiceLiuwaGZWeiXinToken.UpdateMenu(MenuInfo);
                    break;
                case WeiXinChannelCode.遛娃指南杭州服务号_皓颐:
                    return HaoYiServiceLiuwaHZHWeiXinToken.UpdateMenu(MenuInfo);
                    break;
                case WeiXinChannelCode.遛娃指南深圳订阅号_皓颐:
                    return HaoYiLiuwaSHZHWeiXinToken.UpdateMenu(MenuInfo);
                    break;
            }

            return "";
        }

        #endregion

        #region

        public int AddCustomActiveUser(CustomActiveUser cuser)
        {
            return CommDAL.AddCustomActiveUser(cuser);
        }

        public CustomActiveUser GetCustomActiveUser(int activeid, string phone)
        {
            return CommDAL.GetCustomActiveUser(activeid, phone);
        }

        #endregion

        #region 素材相关
        public List<WeixinMaterialEntity> FindAllWeixinMaterial(string key, string catype, int pageIndex, int pageSize)
        {
            return CommDAL.FindAllWeixinMaterial(key, catype, pageIndex, pageSize);
        }
        public int GetMaterialListCount(string key, string catype)
        {
            return CommDAL.GetMaterialListCount(key, catype);
        }
        public List<WeixinMaterialCategoryEntity> FindAllWeixinMaterialCategory()
        {
            return CommDAL.FindAllWeixinMaterialCategory();
        }
        public WeixinMaterialEntity GetMaterialByIDX(int IDX)
        {
            return CommDAL.GetMaterialByIDX(IDX);
        }
        public int AddWeixinMaterial(WeixinMaterialEntity aw)
        {
            return CommDAL.AddWeixinMaterial(aw);
        }
        public int AddWeixinMaterialCategory(WeixinMaterialCategoryEntity aw)
        {
            return CommDAL.AddWeixinMaterialCategory(aw);
        }

        public int UpdateWeixinMaterial(WeixinMaterialEntity aw)
        {
            return CommDAL.UpdateWeixinMaterial(aw);
        }
        public int UpdateWeixinMaterialCategory(WeixinMaterialCategoryEntity aw)
        {
            return CommDAL.UpdateWeixinMaterialCategory(aw);
        }

        public int DeleteWeixinMaterial(WeixinMaterialEntity aw)
        {
            return CommDAL.DeleteWeixinMaterial(aw);
        }
        public int DeleteWeixinMaterialCategory(WeixinMaterialCategoryEntity aw)
        {
            return CommDAL.DeleteWeixinMaterialCategory(aw);
        }
        public int UpdMaterialHotelID(int MaterialID, int HotelID)
        {
            return CommDAL.UpdMaterialHotelID(MaterialID, HotelID);
        }
        public int DelMaterialHotelID(int MaterialID, int HotelID)
        {
            return CommDAL.DelMaterialHotelID(MaterialID, HotelID);
        }
        /// <summary>
        /// 根据IDX获取微信素材信息
        /// </summary>
        /// <param name="idx">IDX</param>
        /// <returns></returns>
        public WeixinMaterialEntity GetWeixinMaterialByIdx(int idx)
        {
            return CommDAL.GetWeixinMaterialByIdx(idx);
        }

        #endregion
    }
}
