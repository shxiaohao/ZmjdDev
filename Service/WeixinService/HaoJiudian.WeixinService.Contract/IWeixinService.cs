using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using HJD.WeixinServices.Contracts;
using HJD.WeixinService.Contract;

namespace HJD.WeixinServices.Contract
{
    [ServiceContract(Namespace = "http://www.zmjiudian.com/")]
    public interface IWeixinService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Content"></param>
        /// <param name="toUser"></param>
        /// <param name="fromuserName"></param>
        /// <returns></returns>
        [OperationContract]
        ResponseEntity GetWeixinResponseText(RequestEntity requestEntity);

        [OperationContract]
        ResponseEntity GetWeixinResponseImage(RequestEntity requestEntity);

        [OperationContract]
        string GetTicket();

        [OperationContract]
        string GetToken();

        [OperationContract]
        ResultEntity Add_News(List<NewsEntity> articles);

        [OperationContract]
        ResultEntity Add_Material(string fileUrl, string fileName);

        [OperationContract]
        ResultEntity Update_News(string media_id, NewsEntity article);

        [OperationContract]
        ResultEntity Del_Material(string media_id);

        [OperationContract]
        List<WeixinActivityEntity> GetWeixinActives();
        
        [OperationContract]
        WeixinActivityEntity GetOneWeixinActive(int activeId);

        [OperationContract]
        bool RefWeixinActivesCache();

        [OperationContract]
        bool RefOneWeixinActivesCache(int activeId);

        [OperationContract]
        List<WeixinActivityEntity> GetWeixinActives2();

        [OperationContract]
        bool EditWeixinActive(WeixinActivityEntity wa);

        [OperationContract]
        bool IsPartWeixinActivity(int acode, string phone);

        [OperationContract]
        List<ActiveWeixinLuckyReport> GetActiveWeixinLuckyReport(int activeId, int luckcode);

        [OperationContract]
        ActiveWeixinLuckyUser GetActiveWeixinLuckyUser(int activeId, int luckcode);

        [OperationContract]
        List<ActiveRuleGroup> GetActiveRuleGroups();

        [OperationContract]
        bool EditActiveRuleGroup(ActiveRuleGroup obj);

        [OperationContract]
        List<ActiveRuleEx> GetActiveRuleExs();

        [OperationContract]
        bool EditActiveRuleEx(ActiveRuleEx obj);

        [OperationContract]
        List<ActiveRuleEx> GetActiveRuleExsByActiveId(int activeId);

        [OperationContract]
        List<ActiveRuleExForVote> GetActiveRuleExsForVoteByActiveId(int activeId);

        [OperationContract]
        ActiveRuleExForVote GetActiveRuleExsForVoteByActiveIdAndID(int activeId, int exid);

        [OperationContract]
        List<ActiveRulePrize> GetActiveRulePrizeBySourceId(int activeId, int sourceId);

        [OperationContract]
        List<ActiveLuckDrawRecord> GetActiveLuckDrawRecordByDrawId(int activeDrawId);

        [OperationContract]
        List<ActiveLuckDrawRecordContainPrize> GetActiveLuckRecordAndPrizeByDrawId(int activeDrawId);

        [OperationContract]
        int AddActiveLuckDrawRecord(ActiveLuckDrawRecord entity);

        [OperationContract]
        List<ActiveRuleSpokesmanAndDrawInfo> GetActiveSpokesmanInfoByActiveIdAndExId(int activeId, int exid);

        [OperationContract]
        List<ActiveVoteRecord> GetActiveVoteRecordForType1ByWxAccount(int activeId, string weixinAccount);

        [OperationContract]
        List<ActiveVoteRecord> GetActiveVoteRecordForType2BySourceId(int activeId, int sourceId);

        [OperationContract]
        List<ActiveVoteRecord> GetActiveVoteRecordForType2BySourceIdAndReltionId(int activeId, int sourceId, int reltionId, string weixinAccount);

        /// <summary>
        /// 新增投票
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [OperationContract]
        int AddActiveVoteRecord(ActiveVoteRecord entity);
        
        [OperationContract]
        List<ActiveRuleExForVote> GetActiveRuleExsForVoteByDrawId(int activeId, int activeDrawId);

        /// <summary>
        /// 新增指定活动的代言记录
        /// </summary>
        /// <param name="entity">代言关联对象</param>
        /// <returns></returns>
        [OperationContract]
        int AddActiveRuleSpokesman(ActiveRuleSpokesman entity);

        [OperationContract]
        bool AddUpActiveRuleExOfferCountById(int activeRuleExId);

        #region 微信h5

        /// <summary>
        /// 根据openid获取微信用户授权得到的信息
        /// </summary>
        /// <param name="openid"></param>
        /// <returns></returns>
        [OperationContract]
        ActiveWeixinUser GetActiveWeixinUser(string openid);

        [OperationContract]
        ActiveWeixinUser GetActiveWeixinUserById(int id);

        /// <summary>
        /// 添加一个以openid为主键的微信用户记录
        /// </summary>
        /// <param name="aw"></param>
        /// <returns></returns>
        [OperationContract]
        int AddActiveWeixinUser(ActiveWeixinUser aw);

        /// <summary>
        /// 根据活动ID和openid获取微信用户的报名记录
        /// </summary>
        /// <param name="openid"></param>
        /// <returns></returns>
        [OperationContract]
        ActiveWeixinDraw GetActiveWeixinDraw(int activeId, string openid);

        /// <summary>
        /// 根据用户报名手机号和活动Id获取报名记录
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        [OperationContract]
        ActiveWeixinDraw GetActiveWeixinDrawByPhone(int activeId, string phone);

        /// <summary>
        /// 根据报名Id和活动Id获取报名记录
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        ActiveWeixinDraw GetActiveWeixinDrawById(int activeId, int id);

        /// <summary>
        /// 添加一个以openid为主键的微信用户指定活动报名记录
        /// </summary>
        /// <param name="aw"></param>
        /// <returns></returns>
        [OperationContract]
        int AddActiveWeixinDraw(ActiveWeixinDraw aw);

        [OperationContract]
        int UpdateActiveWeixinDrawIsShare(int activeId, string openid);

        [OperationContract]
        int UpdateActiveWeixinDrawIsPay(int activeId, string openid);

        [OperationContract]
        int UpdateActiveWeixinDrawPhone(int activeId, string openid, string phone);

        [OperationContract]
        int UpdateActiveWeixinDrawHeadImgUrl(int activeId, string openid, string headimgurl);

        [OperationContract]
        int UpdateActiveWeixinDrawSendCount(int activeId, string openid);

        /// <summary>
        /// 获取指定openid阅读过的分享者
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="openid"></param>
        /// <returns></returns>
        [OperationContract]
        List<ActiveWeixinDraw> GetActiveWeixinDrawByReadUser(int activeId, string openid);

        /// <summary>
        /// 获取指定活动指定分享者的被阅读记录
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="shareOpenid"></param>
        /// <returns></returns>
        [OperationContract]
        List<ActiveWeixinShareRead> GetActiveWeixinShareReadList(int activeId, string shareOpenid);

        /// <summary>
        /// 插入新的阅读记录
        /// </summary>
        /// <param name="aw"></param>
        /// <returns></returns>
        [OperationContract]
        int AddActiveWeixinShareRead(ActiveWeixinShareRead aw);

        /// <summary>
        /// 插入新的阅读记录【最新】
        /// </summary>
        /// <param name="aw"></param>
        /// <returns></returns>
        [OperationContract]
        int AddActiveWeixinShareRead_Luck(ActiveWeixinShareRead aw);

        /// <summary>
        /// 查询微信报名统计
        /// </summary>
        /// <param name="activeId"></param>
        /// <returns></returns>
        [OperationContract]
        ActiveWeixinStatResult GetWeixinAtvStatResult(int activeId);

        /// <summary>
        /// 查询微信报名统计(只返回抽奖码数量)
        /// </summary>
        /// <param name="activeId"></param>
        /// <returns></returns>
        [OperationContract]
        ActiveWeixinStatResult GetActiveWeixinLuckCodeCount(int activeId);


        /// <summary>
        /// 插入新的抽奖码记录
        /// </summary>
        /// <param name="aluck"></param>
        /// <returns></returns>
        [OperationContract]
        int AddActiveWeixinLuckCode(ActiveWeixinLuckCode aluck);
        
        /// <summary>
        /// 插入新的虚拟价值（如宝石）记录
        /// </summary>
        /// <param name="aluck"></param>
        /// <returns></returns>
        [OperationContract]
        int AddActiveWeixinFicMoney(ActiveWeixinFicMoney aficmoney);

        /// <summary>
        /// 获取指定活动指定用户的虚拟价值（如宝石）获得情况
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="openid"></param>
        /// <returns></returns>
        [OperationContract]
        List<ActiveWeixinFicMoney> GetActiveWeixinFicMoneyInfo(int activeId, string openid);

        /// <summary>
        /// 发布增加新抽奖码的队列任务
        /// </summary>
        /// <param name="aluck"></param>
        /// <returns></returns>
        [OperationContract]
        bool PublishWeixinLuckCodeTask(ActiveWeixinLuckCode aluck);
        
        /// <summary>
        /// 发布增加宝石的队列任务
        /// </summary>
        /// <param name="aluck"></param>
        /// <returns></returns>
        [OperationContract]
        bool PublishWeixinGemTask(ActiveWeixinLuckCode aluck);

        /// <summary>
        /// 获取指定活动指定用户的抽奖码获得情况
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="openid"></param>
        /// <returns></returns>
        [OperationContract]
        List<ActiveWeixinLuckCode> GetActiveWeixinLuckCodeInfo(int activeId, string openid);

        /// <summary>
        /// 获取指定活动指定TagName的抽奖码获得情况
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="tagName">抽奖码标签，如 报名奖励、翻倍卡助力 等</param>
        /// <returns></returns>
        [OperationContract]
        List<ActiveWeixinLuckCode> GetActiveWeixinLuckCodeInfoByTagName(int activeId, string tagName);

        /// <summary>
        /// 获取指定活动指定TagName指定SourceOpenid的抽奖码获得情况
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="tagName">抽奖码标签，如 报名奖励、翻倍卡助力 等</param>
        /// <param name="sourceOpenid">一般为该抽奖码的助力人</param>
        /// <returns></returns>
        [OperationContract]
        List<ActiveWeixinLuckCode> GetActiveWeixinLuckCodeInfoByTagNameAndSource(int activeId, string tagName, string sourceOpenid);

        /// <summary>
        /// 获取所有微信合作伙伴信息
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<ActiveWeixinPartner> GetAllWeixinPartners();

        /// <summary>
        /// 添加/修改微信合作伙伴
        /// </summary>
        /// <param name="wp"></param>
        /// <returns></returns>
        [OperationContract]
        int AddActiveWeixinPartner(ActiveWeixinPartner wp);

        /// <summary>
        /// 获取指定openid的WeixinUser
        /// </summary>
        /// <param name="openid"></param>
        /// <returns></returns>
        [OperationContract]
        WeixinUser GetWeixinUser(string openid);

        /// <summary>
        /// 获取指定id的WeixinUser
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [OperationContract]
        WeixinUser GetWeixinUserById(int id);

        /// <summary>
        /// 获取指定unionid和weixinaccount的weixinuser信息
        /// </summary>
        /// <param name="unionid"></param>
        /// <param name="weixinAccount"></param>
        /// <returns></returns>
        [OperationContract]
        WeixinUser GetWeixinUserByUnionidAndAccount(string unionid, string weixinAccount);

        /// <summary>
        /// 添加指定奖励记录（根据fromwxuid）
        /// </summary>
        /// <param name="wp"></param>
        /// <returns></returns>
        [OperationContract]
        int AddWeixinRewardRecordByWxuid(WeixinRewardRecord wr);

        /// <summary>
        /// 添加指定奖励记录
        /// </summary>
        /// <param name="wp"></param>
        /// <returns></returns>
        [OperationContract]
        int AddWeixinRewardRecord(WeixinRewardRecord wr);

        /// <summary>
        /// 添加指定奖励记录（for wx redpack union active）
        /// </summary>
        /// <param name="wp"></param>
        /// <returns></returns>
        [OperationContract]
        int AddWeixinRewardRecordForRedpackUnion(WeixinRewardRecord wr);

        /// <summary>
        /// 获取指定活动类型指定Openid的红包发送记录
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="activeId"></param>
        /// <param name="reOpenid"></param>
        /// <returns></returns>
        [OperationContract]
        List<WeixinRewardRecord> GetWeixinRewardRecordByWxActive(int sourceType, int activeId, string reOpenid);

        /// <summary>
        /// 扣除相关合作伙伴的指定资金
        /// </summary>
        /// <param name="partnerId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [OperationContract]
        int UpdateWxPartnerActiveFund(long partnerId, decimal value);

        #endregion

        /// <summary>
        /// 更新微信用户的用户信息，存在则只更新关注状态和时间（如是否关注）
        /// </summary>
        /// <param name="w"></param>
        /// <returns></returns>
        [OperationContract]
        int UpdateWeixinUserSubscribe(WeixinUser w);

        /// <summary>
        /// 更新微信用户的用户信息
        /// </summary>
        /// <param name="w"></param>
        /// <returns></returns>
        [OperationContract]
        int UpdateWeixinUserInfo(WeixinUser w);

        /// <summary>
        /// 获取指定Openid的用户信息
        /// </summary>
        /// <param name="openid"></param>
        /// <param name="backJson"></param>
        /// <returns></returns>
        [OperationContract]
        WeixinUser GetWeixinUserSubscribeInfo(string openid, WeiXinChannelCode ChannelCode = WeiXinChannelCode.周末酒店订阅号);

        /// <summary>
        /// 获取指定Openid的用户信息
        /// </summary>
        /// <param name="openid"></param>
        /// <param name="backJson"></param>
        /// <returns></returns>
        [OperationContract]
        WeixinUser GetWeixinUserSubscribeInfo2(string openid, int ChannelCode);

        /// <summary>
        /// 查询指定unionid指定微信号下的用户信息（如关注状态）
        /// </summary>
        /// <param name="weixinAcount"></param>
        /// <param name="unionid"></param>
        /// <returns></returns>
        [OperationContract]
        WeixinUser GetWeixinUserByUnionid(string weixinAcount, string unionid);

        /// <summary>
        /// 获取指定微信账号的token
        /// </summary>
        /// <param name="weixinAcount"></param>
        /// <returns></returns>
        [OperationContract]
        string GetWeixinToken(string weixinAcount);


        /// <summary>
        /// 获取指定微信通道的token
        /// </summary>
        /// <param name="weixinAcount"></param>
        /// <returns></returns>
        [OperationContract]
        string GetWeixinTokenByCode(WeiXinChannelCode code);


        /// <summary>
        /// 获取指定微信通道的token
        /// </summary>
        /// <param name="strCode"></param>
        /// <returns></returns>
        [OperationContract]
        string GetWeixinTokenByStrCode(string strCode, bool forceReFresh);

        /// <summary>
        /// 重新生成指定微信通道的token
        /// </summary>
        /// <param name="weixinAcount"></param>
        /// <returns></returns>
        [OperationContract]
        string GenWeixinTokenByCode(WeiXinChannelCode code);

        /// <summary>
        /// 指定微信公众号发送自定义信息【Text】
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [OperationContract]
        bool SendWeixinTextMsg(WeixinMsgText msg);

        /// <summary>
        /// 指定微信公众号发送自定义信息【News】
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [OperationContract]
        bool SendWeixinNewsMsg(WeixinMsgNews msg);

        /// <summary>
        /// 添加自定义活动的用户信息
        /// </summary>
        /// <param name="cuser"></param>
        /// <returns></returns>
        [OperationContract]
        int AddCustomActiveUser(CustomActiveUser cuser);

        /// <summary>
        /// 获取指定自定义活动的参与人信息
        /// </summary>
        /// <param name="activeid"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        [OperationContract]
        CustomActiveUser GetCustomActiveUser(int activeid, string phone);

        /// <summary>
        /// 添加素材
        /// </summary>
        /// <param name="weixinmaterial"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute()]
        int AddWeixinMaterial(WeixinMaterialEntity weixinmaterial);

        /// <summary>
        /// 更新素材
        /// </summary>
        /// <param name="weixinmaterial"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute()]
        int UpdateWeixinMaterial(WeixinMaterialEntity weixinmaterial);

        /// <summary>
        /// 删除素材
        /// </summary>
        /// <param name="weixinmaterial"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute()]
        int DeleteWeixinMaterial(WeixinMaterialEntity weixinmaterial);

        /// <summary>
        /// 获取素材
        /// </summary>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute()]
        List<WeixinMaterialEntity> FindAllWeixinMaterial(string key,string catype, int pageIndex, int pageSize);
        /// <summary>
        /// 获取列表数据数量
        /// </summary>
        /// <param name="key"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute()]
        int GetMaterialListCount(string key,string catype);
        /// <summary>
        /// 添加素材分类
        /// </summary>
        /// <param name="weixinmaterialcategory"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute()]
        int AddWeixinMaterialCategory(WeixinMaterialCategoryEntity weixinmaterialcategory);

        /// <summary>
        /// 更新素材分类
        /// </summary>
        /// <param name="weixinmaterialcategory"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute()]
        int UpdateWeixinMaterialCategory(WeixinMaterialCategoryEntity weixinmaterialcategory);

        /// <summary>
        /// 删除素材分类
        /// </summary>
        /// <param name="weixinmaterialcategory"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute()]
        int DeleteWeixinMaterialCategory(WeixinMaterialCategoryEntity weixinmaterialcategory);

        /// <summary>
        /// 获取素材分类列表
        /// </summary>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute()]
        System.Collections.Generic.List<WeixinMaterialCategoryEntity> FindAllWeixinMaterialCategory();

        /// <summary>
        /// 获取素材
        /// </summary>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute()]
        WeixinMaterialEntity GetMaterialByIDX(int IDX);



        /// <summary>
        ///  关联酒店ID与素材ID
        /// </summary>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute()]
        int UpdMaterialHotelID(int MaterialID, int HotelID);


        /// <summary>
        ///  删除酒店ID与素材ID
        /// </summary>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute()]
        int DelMaterialHotelID(int MaterialID, int HotelID);

        /// <summary>
        /// 更新微信菜单
        /// </summary>
        /// <param name="MenuInfo"></param>
        /// <param name="acountId"></param>
        /// <returns></returns>
        [System.ServiceModel.OperationContractAttribute()]
        string UpdateWeiXinMenu(string MenuInfo, int acountId = 1);
    }
}
