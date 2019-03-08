using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HJD.Framework.Interface;
using HJD.WeixinServices.Contract;
using System.Data;
using HJD.WeixinServices.Contracts;
using Newtonsoft.Json;
using System.IO;
using HJD.WeixinService.Contract;

namespace HJD.WeixinServices.Implement
{
    public class CommDAL
    {
        static readonly Object _obj = new object();

        private static IDatabaseManager CommDB = DatabaseManagerFactory.Create("CommDB");

        public static List<CommDictEntity> GetCommDictList(int type)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("type", DbType.Int32, type);
            return CommDB.ExecuteSqlString<CommDictEntity>("CommDB.GetCommDictList", parameters);
        }

        public static CommDictEntity GetCommDict(int typeid, int dictKey)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@type", DbType.Int32, typeid);
            parameters.AddInParameter("@dictKey", DbType.Int32, dictKey);

            return CommDB.ExecuteSqlString<CommDictEntity>("CommDB.GetCommDict", parameters).FirstOrDefault();
        }

        #region 微信活动

        /// <summary>
        /// 获取微信活动
        /// </summary>
        /// <returns></returns>
        public static List<WeixinActivityEntity> GetWeixinActives()
        {
            var parameters = new DBParameterCollection();
            return CommDB.ExecuteSqlString<WeixinActivityEntity>("CommDB.GetWeixinActives", parameters);
        }

        /// <summary>
        /// 获取指定ID的微信活动
        /// </summary>
        /// <returns></returns>
        public static List<WeixinActivityEntity> GetOneWeixinActive(int aid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ActiveID", DbType.Int32, aid);
            return CommDB.ExecuteSqlString<WeixinActivityEntity>("CommDB.GetOneWeixinActive", parameters);
        }

        public static int EditWeixinActive(WeixinActivityEntity wa)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ActivityID", DbType.Int32, wa.ActivityID);
            parameters.AddInParameter("@ActivityKeyWord", DbType.String, wa.ActivityKeyWord);
            parameters.AddInParameter("@ActivityStartDateTime", DbType.DateTime, wa.ActivityStartDateTime);
            parameters.AddInParameter("@ActivityFinishDateTime", DbType.DateTime, wa.ActivityFinishDateTime);
            parameters.AddInParameter("@ActiveEndTime", DbType.DateTime, wa.ActiveEndTime);
            parameters.AddInParameter("@ActivityNotStartWord", DbType.String, wa.ActivityNotStartWord);
            parameters.AddInParameter("@ActivityFinishWord", DbType.String, wa.ActivityFinishWord);
            parameters.AddInParameter("@Solutions", DbType.String, wa.Solutions);
            parameters.AddInParameter("@TxtCanEmpty", DbType.Boolean, wa.TxtCanEmpty);
            parameters.AddInParameter("@EnrollTxtSuccess", DbType.String, wa.EnrollTxtSuccess);
            parameters.AddInParameter("@HasEnrollTxtMessage", DbType.String, wa.HasEnrollTxtMessage);
            parameters.AddInParameter("@EnrollTxtAlert", DbType.String, wa.EnrollTxtAlert);
            parameters.AddInParameter("@HasPhotoStep", DbType.Boolean, wa.HasPhotoStep);
            parameters.AddInParameter("@DefaultPhotoSuccess", DbType.String, wa.DefaultPhotoSuccess);
            parameters.AddInParameter("@EnrollPhotoSuccess", DbType.String, wa.EnrollPhotoSuccess);
            parameters.AddInParameter("@Type", DbType.Int32, wa.Type);
            parameters.AddInParameter("@HaveSignUp", DbType.Int32, wa.HaveSignUp);
            parameters.AddInParameter("@WeixinSignUpTopBannerUrl", DbType.String, wa.WeixinSignUpTopBannerUrl);
            parameters.AddInParameter("@WeixinSignUpTopBannerTitle", DbType.String, wa.WeixinSignUpTopBannerTitle);
            parameters.AddInParameter("@WeixinSignUpTopBannerTitle2", DbType.String, wa.WeixinSignUpTopBannerTitle2);
            parameters.AddInParameter("@WeixinSignUpTopBannerTitleAlign", DbType.Int32, wa.WeixinSignUpTopBannerTitleAlign);
            parameters.AddInParameter("@WeixinSignUpShareTitle", DbType.String, wa.WeixinSignUpShareTitle);
            parameters.AddInParameter("@WeixinSignUpShareLink", DbType.String, wa.WeixinSignUpShareLink);
            parameters.AddInParameter("@WeixinSignUpShareImgUrl", DbType.String, wa.WeixinSignUpShareImgUrl);
            parameters.AddInParameter("@WeixinSignUpShareTip", DbType.String, wa.WeixinSignUpShareTip);
            parameters.AddInParameter("@WeixinSignUpResultLink", DbType.String, wa.WeixinSignUpResultLink);
            parameters.AddInParameter("@WeixinAcountId", DbType.Int32, wa.WeixinAcountId);
            parameters.AddInParameter("@RelPartnerIds", DbType.String, wa.RelPartnerIds);
            parameters.AddInParameter("@NeedPaySign", DbType.Int32, wa.NeedPaySign);
            parameters.AddInParameter("@PayPrice", DbType.Decimal, wa.PayPrice);
            parameters.AddInParameter("@ReturnPrice", DbType.Decimal, wa.ReturnPrice);
            parameters.AddInParameter("@IsInvite", DbType.Int32, wa.IsInvite);
            parameters.AddInParameter("@PersonMaxLucks", DbType.Int32, wa.PersonMaxLucks);

            var update = CommDB.ExecuteSqlStringAndReturnSingleField("CommDB.EditWeixinActive", parameters);

            try
            {
                return Convert.ToInt32(update);
            }
            catch (Exception ex)
            {

            }

            return 0;
        }

        public static bool IsPartWeixinActivity(int acode, string phone)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ActiveCode", DbType.Int32, acode);
            parameters.AddInParameter("@Phone", DbType.String, phone);

            var haveCountObj = CommDB.ExecuteSqlStringAndReturnSingleField("CommDB.IsPartWeixinActivity", parameters);

            try
            {
                var haveCount = Convert.ToInt32(haveCountObj);
                return haveCount > 0;
            }
            catch (Exception ex)
            {

            }

            return false;
        }

        public static List<ActiveWeixinLuckyReport> GetActiveWeixinLuckyReport(int activeId, int luckcode)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ActiveId", DbType.Int32, activeId);
            parameters.AddInParameter("@LuckCode", DbType.Int32, luckcode);

            return CommDB.ExecuteSqlString<ActiveWeixinLuckyReport>("CommDB.GetActiveWeixinLuckyReport", parameters);
        }

        public static ActiveWeixinLuckyUser GetActiveWeixinLuckyUser(int activeId, int luckcode)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ActiveId", DbType.Int32, activeId);
            parameters.AddInParameter("@LuckCode", DbType.Int32, luckcode);

            return CommDB.ExecuteSqlString<ActiveWeixinLuckyUser>("CommDB.GetActiveWeixinLuckyUser", parameters).FirstOrDefault();
        }

        /// <summary>
        /// 获取所有微信合作伙伴信息
        /// </summary>
        /// <returns></returns>
        public static List<ActiveWeixinPartner> GetAllWeixinPartners()
        {
            var parameters = new DBParameterCollection();

            return CommDB.ExecuteSqlString<ActiveWeixinPartner>("CommDB.GetAllWeixinPartners", parameters);
        }

        /// <summary>
        /// 添加/修改微信合作伙伴
        /// </summary>
        /// <param name="wp"></param>
        /// <returns></returns>
        public static int AddActiveWeixinPartner(ActiveWeixinPartner wp)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@Id", DbType.Int32, wp.Id);
            parameters.AddInParameter("@PartnerCode", DbType.String, wp.PartnerCode);
            parameters.AddInParameter("@Name", DbType.String, wp.Name);
            parameters.AddInParameter("@Brief", DbType.String, wp.Brief);
            parameters.AddInParameter("@QrCodeUrl", DbType.String, wp.QrCodeUrl);
            parameters.AddInParameter("@LogoUrl", DbType.String, wp.LogoUrl);
            parameters.AddInParameter("@LuckCodeCount", DbType.Int32, wp.LuckCodeCount);
            parameters.AddInParameter("@State", DbType.Int32, wp.State);
            parameters.AddInParameter("@CreateTime", DbType.DateTime, wp.CreateTime);
            parameters.AddInParameter("@ActiveFund", DbType.Decimal, wp.ActiveFund);
            parameters.AddInParameter("@ActiveJoinCost", DbType.Int32, wp.ActiveJoinCost);
            parameters.AddInParameter("@ActiveMinReward", DbType.Decimal, wp.ActiveMinReward);
            parameters.AddInParameter("@ActiveMaxReward", DbType.Decimal, wp.ActiveMaxReward);
            parameters.AddInParameter("@ShortName", DbType.String, wp.ShortName);
            
            lock (_obj)
            {
                var update = CommDB.ExecuteSqlStringAndReturnSingleField("CommDB.AddActiveWeixinPartner", parameters);

                try
                {
                    return Convert.ToInt32(update);
                }
                catch (Exception ex)
                {

                }
            }

            return 0;
        }

        /// <summary>
        /// 获取指定活动指定用户的抽奖码获得情况
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="openid"></param>
        /// <returns></returns>
        public static List<ActiveWeixinLuckCode> GetActiveWeixinLuckCodeInfo(int activeId, string openid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ActiveId", DbType.Int32, activeId);
            parameters.AddInParameter("@Openid", DbType.String, openid);

            return CommDB.ExecuteSqlString<ActiveWeixinLuckCode>("CommDB.GetActiveWeixinLuckCodeInfo", parameters);
        }

        /// <summary>
        /// 获取指定活动指定TagName的抽奖码获得情况
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="tagName">抽奖码标签，如 报名奖励、翻倍卡助力 等</param>
        /// <returns></returns>
        public static List<ActiveWeixinLuckCode> GetActiveWeixinLuckCodeInfoByTagName(int activeId, string tagName)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ActiveId", DbType.Int32, activeId);
            parameters.AddInParameter("@TagName", DbType.String, tagName);

            return CommDB.ExecuteSqlString<ActiveWeixinLuckCode>("CommDB.GetActiveWeixinLuckCodeInfoByTagName", parameters);
        }

        /// <summary>
        /// 获取指定活动指定TagName指定SourceOpenid的抽奖码获得情况
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="tagName">抽奖码标签，如 报名奖励、翻倍卡助力 等</param>
        /// <param name="sourceOpenid">一般为该抽奖码的助力人</param>
        /// <returns></returns>
        public static List<ActiveWeixinLuckCode> GetActiveWeixinLuckCodeInfoByTagNameAndSource(int activeId, string tagName, string sourceOpenid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ActiveId", DbType.Int32, activeId);
            parameters.AddInParameter("@TagName", DbType.String, tagName);
            parameters.AddInParameter("@SourceOpenid", DbType.String, sourceOpenid);

            return CommDB.ExecuteSqlString<ActiveWeixinLuckCode>("CommDB.GetActiveWeixinLuckCodeInfoByTagNameAndSource", parameters);
        }

        /// <summary>
        /// 获取所有的活动分组
        /// </summary>
        /// <returns></returns>
        public static List<ActiveRuleGroup> GetActiveRuleGroups()
        {
            var parameters = new DBParameterCollection();
            return CommDB.ExecuteSqlString<ActiveRuleGroup>("CommDB.GetActiveRuleGroups", parameters);
        }

        /// <summary>
        /// 更新活动分组
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int EditActiveRuleGroup(ActiveRuleGroup obj)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ID", DbType.Int32, obj.ID);
            parameters.AddInParameter("@Title", DbType.String, obj.Title);
            parameters.AddInParameter("@SubTitle", DbType.String, obj.SubTitle);
            parameters.AddInParameter("@PicUrl", DbType.String, obj.PicUrl);
            parameters.AddInParameter("@Description", DbType.String, obj.Description);
            parameters.AddInParameter("@ActiveId", DbType.Int32, obj.ActiveId);
            parameters.AddInParameter("@Type", DbType.Int32, obj.Type);

            lock (_obj)
            {
                var update = CommDB.ExecuteSqlStringAndReturnSingleField("CommDB.EditActiveRuleGroup", parameters);

                try
                {
                    return Convert.ToInt32(update);
                }
                catch (Exception ex)
                {

                }
            }

            return 0;
        }

        /// <summary>
        /// 获取所有微信活动扩展信息
        /// </summary>
        /// <returns></returns>
        public static List<ActiveRuleEx> GetActiveRuleExs()
        {
            var parameters = new DBParameterCollection();
            return CommDB.ExecuteSqlString<ActiveRuleEx>("CommDB.GetActiveRuleExs", parameters);
        }

        /// <summary>
        /// 更新微信活动扩展信息
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int EditActiveRuleEx(ActiveRuleEx obj)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ID", DbType.Int32, obj.ID);
            parameters.AddInParameter("@HotelId", DbType.Int32, obj.HotelId);
            parameters.AddInParameter("@ActiveId", DbType.Int32, obj.ActiveId);
            parameters.AddInParameter("@GroupId", DbType.Int32, obj.GroupId);
            parameters.AddInParameter("@Title", DbType.String, obj.Title);
            parameters.AddInParameter("@SubTitle", DbType.String, obj.SubTitle);
            parameters.AddInParameter("@PicUrl", DbType.String, obj.PicUrl);
            parameters.AddInParameter("@RoomInfo", DbType.String, obj.RoomInfo);
            parameters.AddInParameter("@OfferCount", DbType.Int32, obj.OfferCount);
            parameters.AddInParameter("@OrderNum", DbType.Int32, obj.OrderNum);
            parameters.AddInParameter("@Description", DbType.String, obj.Description);

            lock (_obj)
            {
                var update = CommDB.ExecuteSqlStringAndReturnSingleField("CommDB.EditActiveRuleEx", parameters);

                try
                {
                    return Convert.ToInt32(update);
                }
                catch (Exception ex)
                {

                }
            }

            return 0;
        }

        /// <summary>
        /// 根据活动ID获取扩展表信息（根据activeid关联groupid，再通过groupid查询exs）
        /// </summary>
        /// <param name="activeId"></param>
        /// <returns></returns>
        public static List<ActiveRuleEx> GetActiveRuleExsByActiveId(int activeId)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ActiveID", DbType.Int32, activeId);
            return CommDB.ExecuteSqlString<ActiveRuleEx>("CommDB.GetActiveRuleExsByActiveId", parameters);
        }

        /// <summary>
        /// 【For Vote】根据活动ID获取投票活动list（根据activeid关联groupid，再通过groupid查询exs）
        /// </summary>
        /// <param name="activeId"></param>
        /// <returns></returns>
        public static List<ActiveRuleExForVote> GetActiveRuleExsForVoteByActiveId(int activeId)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ActiveID", DbType.Int32, activeId);
            return CommDB.ExecuteSqlString<ActiveRuleExForVote>("CommDB.GetActiveRuleExsForVoteByActiveId", parameters);
        }

        /// <summary>
        /// 【For Vote】根据活动ID和RuleExID获取投票活动list（根据activeid关联groupid，再通过groupid查询exs）
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="exid"></param>
        /// <returns></returns>
        public static ActiveRuleExForVote GetActiveRuleExsForVoteByActiveIdAndID(int activeId, int exid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ActiveID", DbType.Int32, activeId);
            parameters.AddInParameter("@ID", DbType.Int32, exid);
            return CommDB.ExecuteSqlString<ActiveRuleExForVote>("CommDB.GetActiveRuleExsForVoteByActiveIdAndID", parameters).FirstOrDefault();
        }

        /// <summary>
        /// 获取指定sourceid的奖品信息
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="sourceId"></param>
        /// <returns></returns>
        public static List<ActiveRulePrize> GetActiveRulePrizeBySourceId(int activeId, int sourceId)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ActiveID", DbType.Int32, activeId);
            parameters.AddInParameter("@SourceId", DbType.Int32, sourceId);
            return CommDB.ExecuteSqlString<ActiveRulePrize>("CommDB.GetActiveRulePrizeBySourceId", parameters);
        }

        /// <summary>
        /// 获取指定报名用户的抽奖记录
        /// </summary>
        /// <param name="activeDrawId"></param>
        /// <returns></returns>
        public static List<ActiveLuckDrawRecord> GetActiveLuckDrawRecordByDrawId(int activeDrawId)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ActiveDrawId", DbType.Int32, activeDrawId);
            return CommDB.ExecuteSqlString<ActiveLuckDrawRecord>("CommDB.GetActiveLuckDrawRecordByDrawId", parameters);
        }

        /// <summary>
        /// 获取指定报名用户的抽奖记录（包含奖品详细信息）
        /// </summary>
        /// <param name="activeDrawId"></param>
        /// <returns></returns>
        public static List<ActiveLuckDrawRecordContainPrize> GetActiveLuckRecordAndPrizeByDrawId(int activeDrawId)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ActiveDrawId", DbType.Int32, activeDrawId);
            return CommDB.ExecuteSqlString<ActiveLuckDrawRecordContainPrize>("CommDB.GetActiveLuckRecordAndPrizeByDrawId", parameters);
        }

        /// <summary>
        /// 插入抽奖记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static int AddActiveLuckDrawRecord(ActiveLuckDrawRecord entity)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ActiveDrawId", DbType.Int32, entity.ActiveDrawId);
            parameters.AddInParameter("@PrizeId", DbType.Int32, entity.PrizeId);
            parameters.AddInParameter("@Remark", DbType.String, entity.Remark);
            parameters.AddInParameter("@State", DbType.Int32, entity.State);
            parameters.AddInParameter("@ActiveId", DbType.Int32, entity.ActiveId);
            parameters.AddInParameter("@CreateTime", DbType.DateTime, DateTime.Now);

            var add = CommDB.ExecuteSqlStringAndReturnSingleField("CommDB.AddActiveLuckDrawRecord", parameters);

            try
            {
                return Convert.ToInt32(add);
            }
            catch (Exception ex)
            {

            }

            return 0;
        }

        /// <summary>
        /// 获取指定活动id和RuleExId的所有代言人信息
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="exid"></param>
        /// <returns></returns>
        public static List<ActiveRuleSpokesmanAndDrawInfo> GetActiveSpokesmanInfoByActiveIdAndExId(int activeId, int exid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ActiveId", DbType.Int32, activeId);
            parameters.AddInParameter("@ExId", DbType.Int32, exid);
            return CommDB.ExecuteSqlString<ActiveRuleSpokesmanAndDrawInfo>("CommDB.GetActiveSpokesmanInfoByActiveIdAndExId", parameters);
        }

        /// <summary>
        /// 获取指定活动ID指定wx openid的投票记录
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="weixinAccount">wx openid</param>
        /// <returns></returns>
        public static List<ActiveVoteRecord> GetActiveVoteRecordForType1ByWxAccount(int activeId, string weixinAccount)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ActiveId", DbType.Int32, activeId);
            parameters.AddInParameter("@WeixinAccount", DbType.String, weixinAccount);
            return CommDB.ExecuteSqlString<ActiveVoteRecord>("CommDB.GetActiveVoteRecordForType1ByWxAccount", parameters);
        }

        /// <summary>
        /// 获取指定活动ID指定sourceid(draw id)的被投票记录
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="sourceId">(draw id)</param>
        /// <returns></returns>
        public static List<ActiveVoteRecord> GetActiveVoteRecordForType2BySourceId(int activeId, int sourceId)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ActiveId", DbType.Int32, activeId);
            parameters.AddInParameter("@SourceId", DbType.Int32, sourceId);
            return CommDB.ExecuteSqlString<ActiveVoteRecord>("CommDB.GetActiveVoteRecordForType2BySourceId", parameters);
        }


        /// <summary>
        /// 查询通过当前drawid（包括自己）投过该酒店的记录
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="sourceId">大使ID（包括自己）</param>
        /// <param name="reltionId">酒店ID</param>
        /// <param name="weixinAccount"></param>
        /// <returns></returns>
        public static List<ActiveVoteRecord> GetActiveVoteRecordForType2BySourceIdAndReltionId(int activeId, int sourceId, int reltionId, string weixinAccount)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ActiveId", DbType.Int32, activeId);
            parameters.AddInParameter("@SourceId", DbType.Int32, sourceId);
            parameters.AddInParameter("@ReltionId", DbType.Int32, reltionId);
            parameters.AddInParameter("@WeixinAccount", DbType.String, weixinAccount);
            return CommDB.ExecuteSqlString<ActiveVoteRecord>("CommDB.GetActiveVoteRecordForType2BySourceIdAndReltionId", parameters);
        }

        /// <summary>
        /// 新增投票
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static int AddActiveVoteRecord(ActiveVoteRecord entity)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@WeixinAccount", DbType.String, entity.WeixinAccount);
            parameters.AddInParameter("@UserId", DbType.Int64, entity.UserId);
            parameters.AddInParameter("@SourceId", DbType.Int32, entity.SourceId);
            parameters.AddInParameter("@SourceType", DbType.Int32, entity.SourceType);
            parameters.AddInParameter("@ReltionId", DbType.Int32, entity.ReltionId);
            parameters.AddInParameter("@State", DbType.Int32, entity.State);
            parameters.AddInParameter("@ActiveId", DbType.Int32, entity.ActiveId);
            parameters.AddInParameter("@CreateTime", DbType.DateTime, DateTime.Now);

            var add = CommDB.ExecuteSqlStringAndReturnSingleField("CommDB.AddActiveVoteRecord", parameters);

            try
            {
                return Convert.ToInt32(add);
            }
            catch (Exception ex)
            {

            }

            return 0;
        }

        /// <summary>
        /// 获取指定用户报名ID代言的RuleEx记录
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="activeDrawId">用户报名ID</param>
        /// <returns></returns>
        public static List<ActiveRuleExForVote> GetActiveRuleExsForVoteByDrawId(int activeId, int activeDrawId)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ActiveID", DbType.Int32, activeId);
            parameters.AddInParameter("@ActiveDrawId", DbType.Int32, activeDrawId);
            return CommDB.ExecuteSqlString<ActiveRuleExForVote>("CommDB.GetActiveRuleExsForVoteByDrawId", parameters);
        }

        /// <summary>
        /// 新增指定活动的代言记录
        /// </summary>
        /// <param name="entity">代言关联对象</param>
        /// <returns></returns>
        public static int AddActiveRuleSpokesman(ActiveRuleSpokesman entity)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ActiveDrawId", DbType.Int32, entity.ActiveDrawId);
            parameters.AddInParameter("@ExId", DbType.String, entity.RuleExId);
            parameters.AddInParameter("@ActiveId", DbType.String, entity.ActiveId);
            parameters.AddInParameter("@CreateTime", DbType.DateTime, DateTime.Now);

            var add = CommDB.ExecuteSqlStringAndReturnSingleField("CommDB.AddActiveRuleSpokesman", parameters);

            try
            {
                return Convert.ToInt32(add);
            }
            catch (Exception ex)
            {

            }

            return 0;
        }

        /// <summary>
        /// 累加指定ActiveRuleEx Id的OfferCount字段
        /// </summary>
        /// <param name="activeRuleExId">ActiveRuleEx主键Id</param>
        /// <returns></returns>
        public static int AddUpActiveRuleExOfferCountById(int activeRuleExId)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ID", DbType.Int32, activeRuleExId);

            lock (_obj)
            {
                var update = CommDB.ExecuteSqlStringAndReturnSingleField("CommDB.AddUpActiveRuleExOfferCountById", parameters);

                try
                {
                    return Convert.ToInt32(update);
                }
                catch (Exception ex)
                {

                }
            }

            return 0;
        }

        #endregion

        #region 微信h5活动(微信授权用户、活动分享记录等相关处理)

        /// <summary>
        /// 根据openid获取微信用户授权得到的信息
        /// </summary>
        /// <param name="openid"></param>
        /// <returns></returns>
        public static ActiveWeixinUser GetActiveWeixinUser(string openid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@Openid", DbType.String, openid);

            var list = CommDB.ExecuteSqlString<ActiveWeixinUser>("CommDB.GetActiveWeixinUser", parameters);
            if (list == null || list.Count <= 0)
            {
                return null;
            }

            return list.FirstOrDefault();
        }

        public static ActiveWeixinUser GetActiveWeixinUserById(int id)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@Id", DbType.Int32, id);

            var list = CommDB.ExecuteSqlString<ActiveWeixinUser>("CommDB.GetActiveWeixinUserById", parameters);
            if (list == null || list.Count <= 0)
            {
                return null;
            }

            return list.FirstOrDefault();
        }

        /// <summary>
        /// 添加一个以openid为主键的微信用户记录
        /// </summary>
        /// <param name="aw"></param>
        /// <returns></returns>
        public static int AddActiveWeixinUser(ActiveWeixinUser aw)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@Openid", DbType.String, aw.Openid);
            parameters.AddInParameter("@Nickname", DbType.String, aw.Nickname);
            parameters.AddInParameter("@Sex", DbType.String, aw.Sex);
            parameters.AddInParameter("@Province", DbType.String, aw.Province);
            parameters.AddInParameter("@City", DbType.String, aw.City);
            parameters.AddInParameter("@Country", DbType.String, aw.Country);
            parameters.AddInParameter("@Headimgurl", DbType.String, aw.Headimgurl);
            parameters.AddInParameter("@Privilege", DbType.String, aw.Privilege);
            parameters.AddInParameter("@Unionid", DbType.String, aw.Unionid);
            parameters.AddInParameter("@Phone", DbType.String, aw.Phone);
            parameters.AddInParameter("@ActiveID", DbType.Int32, aw.ActiveID);

            var add = CommDB.ExecuteSqlStringAndReturnSingleField("CommDB.AddActiveWeixinUser", parameters);

            try
            {
                return Convert.ToInt32(add);
            }
            catch (Exception ex)
            {

            }

            return 0;
        }

        /// <summary>
        /// 根据活动ID和openid获取微信用户的报名记录
        /// </summary>
        /// <param name="openid"></param>
        /// <returns></returns>
        public static ActiveWeixinDraw GetActiveWeixinDraw(int activeId, string openid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ActiveId", DbType.Int32, activeId);
            parameters.AddInParameter("@Openid", DbType.String, openid);

            var list = CommDB.ExecuteSqlString<ActiveWeixinDraw>("CommDB.GetActiveWeixinDraw", parameters);
            if (list == null || list.Count <= 0)
            {
                return null;
            }

            return list.FirstOrDefault();
        }

        /// <summary>
        /// 根据用户报名手机号和活动Id获取报名记录
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        public static ActiveWeixinDraw GetActiveWeixinDrawByPhone(int activeId, string phone)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ActiveId", DbType.Int32, activeId);
            parameters.AddInParameter("@Phone", DbType.String, phone);

            var list = CommDB.ExecuteSqlString<ActiveWeixinDraw>("CommDB.GetActiveWeixinDrawByPhone", parameters);
            if (list == null || list.Count <= 0)
            {
                return null;
            }

            return list.FirstOrDefault();
        }

        /// <summary>
        /// 根据报名Id和活动Id获取报名记录
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ActiveWeixinDraw GetActiveWeixinDrawById(int activeId, int id)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ActiveId", DbType.Int32, activeId);
            parameters.AddInParameter("@Id", DbType.Int32, id);

            var list = CommDB.ExecuteSqlString<ActiveWeixinDraw>("CommDB.GetActiveWeixinDrawById", parameters);
            if (list == null || list.Count <= 0)
            {
                return null;
            }

            return list.FirstOrDefault();
        }

        /// <summary>
        /// 添加一个以openid为主键的微信用户指定活动报名记录
        /// </summary>
        /// <param name="aw"></param>
        /// <returns></returns>
        public static int AddActiveWeixinDraw(ActiveWeixinDraw aw)
        {
            aw.IsPay = 0;
            aw.PayTime = DateTime.Now;

            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ActiveID", DbType.Int32, aw.ActiveID);
            parameters.AddInParameter("@PartnerId", DbType.Int32, aw.PartnerId);
            parameters.AddInParameter("@Openid", DbType.String, aw.Openid);
            parameters.AddInParameter("@UserName", DbType.String, aw.UserName);
            parameters.AddInParameter("@Phone", DbType.String, aw.Phone);
            parameters.AddInParameter("@IsShare", DbType.Int32, aw.IsShare);
            parameters.AddInParameter("@ShareTime", DbType.DateTime, aw.ShareTime);
            parameters.AddInParameter("@SendFriendCount", DbType.Int32, aw.SendFriendCount);
            parameters.AddInParameter("@LastSendFriendTime", DbType.DateTime, aw.LastSendFriendTime);
            parameters.AddInParameter("@IsPay", DbType.Int32, aw.IsPay == null ? 0 : aw.IsPay);
            parameters.AddInParameter("@PayTime", DbType.DateTime, aw.PayTime);
            parameters.AddInParameter("@HeadImgUrl", DbType.String, aw.HeadImgUrl);

            var add = CommDB.ExecuteSqlStringAndReturnSingleField("CommDB.AddActiveWeixinDraw", parameters);

            try
            {
                return Convert.ToInt32(add);
            }
            catch (Exception ex)
            {

            }

            return 0;
        }

        public static int UpdateActiveWeixinDrawIsShare(int activeId, string openid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ActiveID", DbType.Int32, activeId);
            parameters.AddInParameter("@Openid", DbType.String, openid);

            var add = CommDB.ExecuteSqlStringAndReturnSingleField("CommDB.UpdateActiveWeixinDrawIsShare", parameters);

            try
            {
                return Convert.ToInt32(add);
            }
            catch (Exception ex)
            {

            }

            return 0;
        }

        public static int UpdateActiveWeixinDrawIsPay(int activeId, string openid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ActiveID", DbType.Int32, activeId);
            parameters.AddInParameter("@Openid", DbType.String, openid);

            var add = CommDB.ExecuteSqlStringAndReturnSingleField("CommDB.UpdateActiveWeixinDrawIsPay", parameters);

            try
            {
                return Convert.ToInt32(add);
            }
            catch (Exception ex)
            {

            }

            return 0;
        }

        /// <summary>
        /// 修改指定活动指定用户的报名记录的手机号码
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="openid"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        public static int UpdateActiveWeixinDrawPhone(int activeId, string openid, string phone)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ActiveID", DbType.Int32, activeId);
            parameters.AddInParameter("@Openid", DbType.String, openid);
            parameters.AddInParameter("@Phone", DbType.String, phone);

            var add = CommDB.ExecuteSqlStringAndReturnSingleField("CommDB.UpdateActiveWeixinDrawPhone", parameters);

            try
            {
                return Convert.ToInt32(add);
            }
            catch (Exception ex)
            {

            }

            return 0;
        }

        /// <summary>
        /// 修改指定活动指定用户的报名记录的头像
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="openid"></param>
        /// <param name="headimgurl"></param>
        /// <returns></returns>
        public static int UpdateActiveWeixinDrawHeadImgUrl(int activeId, string openid, string headimgurl)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ActiveID", DbType.Int32, activeId);
            parameters.AddInParameter("@Openid", DbType.String, openid);
            parameters.AddInParameter("@HeadImgUrl", DbType.String, headimgurl);

            var add = CommDB.ExecuteSqlStringAndReturnSingleField("CommDB.UpdateActiveWeixinDrawHeadImgUrl", parameters);

            try
            {
                return Convert.ToInt32(add);
            }
            catch (Exception ex)
            {

            }

            return 0;
        }

        public static int UpdateActiveWeixinDrawSendCount(int activeId, string openid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ActiveID", DbType.Int32, activeId);
            parameters.AddInParameter("@Openid", DbType.String, openid);

            var add = CommDB.ExecuteSqlStringAndReturnSingleField("CommDB.UpdateActiveWeixinDrawSendCount", parameters);

            try
            {
                return Convert.ToInt32(add);
            }
            catch (Exception ex)
            {

            }

            return 0;
        }

        /// <summary>
        /// 获取指定openid阅读过的分享者
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="openid"></param>
        /// <returns></returns>
        public static List<ActiveWeixinDraw> GetActiveWeixinDrawByReadUser(int activeId, string openid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ActiveId", DbType.Int32, activeId);
            parameters.AddInParameter("@Openid", DbType.String, openid);

            return CommDB.ExecuteSqlString<ActiveWeixinDraw>("CommDB.GetActiveWeixinDrawByReadUser", parameters);
        }

        /// <summary>
        /// 获取指定活动指定分享者的被阅读记录
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="shareOpenid"></param>
        /// <returns></returns>
        public static List<ActiveWeixinShareRead> GetActiveWeixinShareReadList(int activeId, string shareOpenid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ActiveID", DbType.Int32, activeId);
            parameters.AddInParameter("@SharerOpenid", DbType.String, shareOpenid);

            if (activeId == 31)
            {
                return CommDB.ExecuteSqlString<ActiveWeixinShareRead>("CommDB.GetActiveWeixinShareReadVillaSpringList", parameters);
            }
            else if (activeId == 32)
            {
                return CommDB.ExecuteSqlString<ActiveWeixinShareRead>("CommDB.GetActiveWeixinShareReadHeartValleyList", parameters);
            }

            return CommDB.ExecuteSqlString<ActiveWeixinShareRead>("CommDB.GetActiveWeixinShareReadList", parameters);
        }

        /// <summary>
        /// 插入新的阅读记录
        /// </summary>
        /// <param name="aw"></param>
        /// <returns></returns>
        public static int AddActiveWeixinShareRead(ActiveWeixinShareRead aw)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ActiveID", DbType.Int32, aw.ActiveID);
            parameters.AddInParameter("@SharerOpenid", DbType.String, aw.SharerOpenid);
            parameters.AddInParameter("@Openid", DbType.String, aw.Openid);
            parameters.AddInParameter("@ReadCount", DbType.Int32, aw.ReadCount);
            parameters.AddInParameter("@LastReadTime", DbType.DateTime, aw.LastReadTime);

            try
            {
                object add = null;

                if (aw.ActiveID == 31)
                {
                    add = CommDB.ExecuteSqlStringAndReturnSingleField("CommDB.AddActiveWeixinShareReadVillaSpring", parameters);
                }
                else if (aw.ActiveID == 32)
                {
                    add = CommDB.ExecuteSqlStringAndReturnSingleField("CommDB.AddActiveWeixinShareReadHeartValley", parameters);
                }
                else
                {
                    add = CommDB.ExecuteSqlStringAndReturnSingleField("CommDB.AddActiveWeixinShareRead", parameters);
                }

                try
                {
                    return Convert.ToInt32(add);
                }
                catch (Exception ex)
                {

                }
            }
            catch (Exception ex)
            {
                try
                {
                    var parametersStr = JsonConvert.SerializeObject(parameters);
                    AddWeixinReadLog(parametersStr);
                }
                catch (Exception ex2)
                {

                }
            }

            return 0;
        }

        /// <summary>
        /// 插入新的阅读记录【最新】
        /// </summary>
        /// <param name="aw"></param>
        /// <returns></returns>
        public static int AddActiveWeixinShareRead_Luck(ActiveWeixinShareRead aw)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ActiveID", DbType.Int32, aw.ActiveID);
            parameters.AddInParameter("@SharerOpenid", DbType.String, aw.SharerOpenid);
            parameters.AddInParameter("@Openid", DbType.String, aw.Openid);
            parameters.AddInParameter("@ReadCount", DbType.Int32, aw.ReadCount);
            parameters.AddInParameter("@LastReadTime", DbType.DateTime, aw.LastReadTime);

            try
            {
                object add = null;

                add = CommDB.ExecuteSqlStringAndReturnSingleField("CommDB.AddActiveWeixinShareRead_Luck", parameters);

                try
                {
                    return Convert.ToInt32(add);
                }
                catch (Exception ex)
                {

                }
            }
            catch (Exception ex)
            {

            }

            return 0;
        }

        /// <summary>
        /// 插入新的抽奖码记录
        /// </summary>
        /// <param name="aluck"></param>
        /// <returns></returns>
        public static int AddActiveWeixinLuckCode(ActiveWeixinLuckCode aluck)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ActiveId", DbType.Int32, aluck.ActiveId);
            parameters.AddInParameter("@Openid", DbType.String, aluck.Openid);
            parameters.AddInParameter("@LuckCode", DbType.String, aluck.LuckCode);
            parameters.AddInParameter("@PartnerId", DbType.Int32, aluck.PartnerId);
            parameters.AddInParameter("@TagName", DbType.String, aluck.TagName);
            parameters.AddInParameter("@SourceOpenid", DbType.String, aluck.SourceOpenid);
            //parameters.AddInParameter("@CreateTime", DbType.DateTime, aluck.CreateTime);

            try
            {
                object add = null;

                add = CommDB.ExecuteSqlStringAndReturnSingleField("CommDB.AddActiveWeixinLuckCode", parameters);

                try
                {
                    return Convert.ToInt32(add);
                }
                catch (Exception ex)
                {

                }
            }
            catch (Exception ex)
            {

            }

            return 0;
        }

        /// <summary>
        /// 插入新的虚拟价值（如宝石）记录
        /// </summary>
        /// <param name="aluck"></param>
        /// <returns></returns>
        public static int AddActiveWeixinFicMoney(ActiveWeixinFicMoney aficmoney)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ActiveId", DbType.Int32, aficmoney.ActiveId);
            parameters.AddInParameter("@Openid", DbType.String, aficmoney.Openid);
            parameters.AddInParameter("@Value", DbType.Int32, aficmoney.Value);
            parameters.AddInParameter("@PartnerId", DbType.Int32, aficmoney.PartnerId);
            parameters.AddInParameter("@Remark", DbType.String, aficmoney.Remark);
            //parameters.AddInParameter("@CreateTime", DbType.DateTime, aficmoney.CreateTime);

            try
            {
                object add = null;

                add = CommDB.ExecuteSqlStringAndReturnSingleField("CommDB.AddActiveWeixinFicMoney", parameters);

                try
                {
                    return Convert.ToInt32(add);
                }
                catch (Exception ex)
                {

                }
            }
            catch (Exception ex)
            {

            }

            return 0;
        }

        /// <summary>
        /// 获取指定活动指定用户的虚拟价值（如宝石）获得情况
        /// </summary>
        /// <param name="activeId"></param>
        /// <param name="openid"></param>
        /// <returns></returns>
        public static List<ActiveWeixinFicMoney> GetActiveWeixinFicMoneyInfo(int activeId, string openid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ActiveId", DbType.Int32, activeId);
            parameters.AddInParameter("@Openid", DbType.String, openid);

            return CommDB.ExecuteSqlString<ActiveWeixinFicMoney>("CommDB.GetActiveWeixinFicMoneyInfo", parameters);
        }

        /// <summary>
        /// 应急使用
        /// </summary>
        /// <param name="msg"></param>
        public static void AddWeixinReadLog(string msg)
        {
            string logFile = @"D:\Log\WeixinService\AddWeixinReadLog_" + DateTime.Now.ToString("MMdd") + ".txt";
            File.AppendAllText(logFile, string.Format("{0}  {1} \r\n\r\n", msg, DateTime.Now));
        }

        /// <summary>
        /// 查询微信报名统计
        /// </summary>
        /// <param name="typeid"></param>
        /// <param name="dictKey"></param>
        /// <returns></returns>
        public static ActiveWeixinStatResult GetWeixinAtvStatResult(int activeId)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@Activeid", DbType.Int32, activeId);

            if (activeId == 31)
            {
                return CommDB.ExecuteSqlString<ActiveWeixinStatResult>("CommDB.GetWeixinStatReportData_VillaSpring", parameters).FirstOrDefault();
            }
            else if (activeId == 32)
            {
                return CommDB.ExecuteSqlString<ActiveWeixinStatResult>("CommDB.GetWeixinStatReportData_HeartValley", parameters).FirstOrDefault();
            }

            return CommDB.ExecuteSqlString<ActiveWeixinStatResult>("CommDB.GetWeixinStatReportData_Def", parameters).FirstOrDefault();
        }

        /// <summary>
        /// 查询微信报名统计(只返回抽奖码数量)
        /// </summary>
        /// <param name="typeid"></param>
        /// <param name="dictKey"></param>
        /// <returns></returns>
        public static ActiveWeixinStatResult GetActiveWeixinLuckCodeCount(int activeId)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@Activeid", DbType.Int32, activeId);

            return CommDB.ExecuteSqlString<ActiveWeixinStatResult>("CommDB.GetActiveWeixinLuckCodeCount", parameters).FirstOrDefault();
        }

        /// <summary>
        /// 获取指定openid的WeixinUser
        /// </summary>
        /// <param name="openid"></param>
        /// <returns></returns>
        public static WeixinUser GetWeixinUser(string openid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@Openid", DbType.String, openid);

            var list = CommDB.ExecuteSqlString<WeixinUser>("CommDB.GetWeixinUser", parameters);
            if (list == null || list.Count <= 0)
            {
                return null;
            }

            return list.FirstOrDefault();
        }

        /// <summary>
        /// 获取指定id的WeixinUser
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static WeixinUser GetWeixinUserById(int id)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@Id", DbType.String, id);

            var list = CommDB.ExecuteSqlString<WeixinUser>("CommDB.GetWeixinUserById", parameters);
            if (list == null || list.Count <= 0)
            {
                return null;
            }

            return list.FirstOrDefault();
        }

        /// <summary>
        /// 获取指定unionid和weixinaccount的weixinuser信息
        /// </summary>
        /// <param name="unionid"></param>
        /// <param name="weixinAccount"></param>
        /// <returns></returns>
        public static WeixinUser GetWeixinUserByUnionidAndAccount(string unionid, string weixinAccount)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@unionid", DbType.String, unionid);
            parameters.AddInParameter("@weixinAccount", DbType.String, weixinAccount);

            var list = CommDB.ExecuteSqlString<WeixinUser>("CommDB.GetWeixinUserByUnionidAndAccount", parameters);
            if (list == null || list.Count <= 0)
            {
                return null;
            }

            return list.FirstOrDefault();
        }

        /// <summary>
        /// add WeixinRewardRecord（by fromwxuid）
        /// </summary>
        /// <param name="wr"></param>
        /// <returns></returns>
        public static int AddWeixinRewardRecordByWxuid(WeixinRewardRecord wr)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@SourceId", DbType.Int64, wr.SourceId);
            parameters.AddInParameter("@SourceType", DbType.Int32, wr.SourceType);
            parameters.AddInParameter("@ReWxUid", DbType.Int32, wr.ReWxUid);
            parameters.AddInParameter("@Wishing", DbType.String, wr.Wishing);
            parameters.AddInParameter("@Amount", DbType.Int32, wr.Amount);
            parameters.AddInParameter("@Number", DbType.Int32, wr.Number);
            parameters.AddInParameter("@ActiveId", DbType.Int32, wr.ActiveId);
            parameters.AddInParameter("@ActiveName", DbType.String, wr.ActiveName);
            parameters.AddInParameter("@Remark", DbType.String, wr.Remark);
            parameters.AddInParameter("@SceneId", DbType.Int32, wr.SceneId);
            parameters.AddInParameter("@SendName", DbType.String, wr.SendName);
            parameters.AddInParameter("@WillSendTime", DbType.DateTime, wr.WillSendTime);
            parameters.AddInParameter("@RealSendTime", DbType.DateTime, wr.RealSendTime);
            parameters.AddInParameter("@State", DbType.Int32, wr.State);

            var add = CommDB.ExecuteSqlStringAndReturnSingleField("CommDB.AddWeixinRewardRecordByWxuid", parameters);

            try
            {
                return Convert.ToInt32(add);
            }
            catch (Exception ex)
            {

            }

            return 0;
        }

        /// <summary>
        /// add WeixinRewardRecord
        /// </summary>
        /// <param name="wr"></param>
        /// <returns></returns>
        public static int AddWeixinRewardRecord(WeixinRewardRecord wr)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@SourceId", DbType.Int64, wr.SourceId);
            parameters.AddInParameter("@SourceType", DbType.Int32, wr.SourceType);
            parameters.AddInParameter("@ReOpenid", DbType.String, wr.ReOpenid);
            parameters.AddInParameter("@Wishing", DbType.String, wr.Wishing);
            parameters.AddInParameter("@Amount", DbType.Int32, wr.Amount);
            parameters.AddInParameter("@Number", DbType.Int32, wr.Number);
            parameters.AddInParameter("@ActiveId", DbType.Int32, wr.ActiveId);
            parameters.AddInParameter("@ActiveName", DbType.String, wr.ActiveName);
            parameters.AddInParameter("@Remark", DbType.String, wr.Remark);
            parameters.AddInParameter("@SceneId", DbType.Int32, wr.SceneId);
            parameters.AddInParameter("@SendName", DbType.String, wr.SendName);
            parameters.AddInParameter("@WillSendTime", DbType.DateTime, wr.WillSendTime);
            parameters.AddInParameter("@RealSendTime", DbType.DateTime, wr.RealSendTime);
            parameters.AddInParameter("@State", DbType.Int32, wr.State);

            var add = CommDB.ExecuteSqlStringAndReturnSingleField("CommDB.AddWeixinRewardRecord", parameters);

            try
            {
                return Convert.ToInt32(add);
            }
            catch (Exception ex)
            {

            }

            return 0;
        }

        /// <summary>
        /// add WeixinRewardRecord（for wx redpack union active）
        /// </summary>
        /// <param name="wr"></param>
        /// <returns></returns>
        public static int AddWeixinRewardRecordForRedpackUnion(WeixinRewardRecord wr)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@SourceId", DbType.Int64, wr.SourceId);
            parameters.AddInParameter("@SourceType", DbType.Int32, wr.SourceType);
            parameters.AddInParameter("@ReOpenid", DbType.String, wr.ReOpenid);
            parameters.AddInParameter("@Wishing", DbType.String, wr.Wishing);
            parameters.AddInParameter("@Amount", DbType.Int32, wr.Amount);
            parameters.AddInParameter("@Number", DbType.Int32, wr.Number);
            parameters.AddInParameter("@ActiveId", DbType.Int32, wr.ActiveId);
            parameters.AddInParameter("@ActiveName", DbType.String, wr.ActiveName);
            parameters.AddInParameter("@Remark", DbType.String, wr.Remark);
            parameters.AddInParameter("@SceneId", DbType.Int32, wr.SceneId);
            parameters.AddInParameter("@SendName", DbType.String, wr.SendName);
            parameters.AddInParameter("@WillSendTime", DbType.DateTime, wr.WillSendTime);
            parameters.AddInParameter("@RealSendTime", DbType.DateTime, wr.RealSendTime);
            parameters.AddInParameter("@State", DbType.Int32, wr.State);

            var add = CommDB.ExecuteSqlStringAndReturnSingleField("CommDB.AddWeixinRewardRecordForRedpackUnion", parameters);

            try
            {
                return Convert.ToInt32(add);
            }
            catch (Exception ex)
            {

            }

            return 0;
        }

        /// <summary>
        /// 获取指定活动类型指定Openid的红包发送记录
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="activeId"></param>
        /// <param name="reOpenid"></param>
        /// <returns></returns>
        public static List<WeixinRewardRecord> GetWeixinRewardRecordByWxActive(int sourceType, int activeId, string reOpenid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@SourceType", DbType.Int32, sourceType);
            parameters.AddInParameter("@ActiveId", DbType.Int32, activeId);
            parameters.AddInParameter("@ReOpenid", DbType.String, reOpenid);

            return CommDB.ExecuteSqlString<WeixinRewardRecord>("CommDB.GetWeixinRewardRecordByWxActive", parameters);
        }

        /// <summary>
        /// 扣除相关合作伙伴的指定资金
        /// </summary>
        /// <param name="partnerId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int UpdateWxPartnerActiveFund(long partnerId, decimal value)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ID", DbType.Int32, partnerId);
            parameters.AddInParameter("@Value", DbType.Decimal, value);

            var update = CommDB.ExecuteSqlStringAndReturnSingleField("CommDB.UpdateWxPartnerActiveFund", parameters);

            try
            {
                return Convert.ToInt32(update);
            }
            catch (Exception ex)
            {

            }

            return 0;
        }

        #endregion

        #region 微信公众号操作相关

        /// <summary>
        /// 更新微信用户的用户信息，存在则只更新关注状态和时间（如是否关注）
        /// </summary>
        /// <param name="w"></param>
        /// <returns></returns>
        public static int UpdateWeixinUserSubscribe(WeixinUser w)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@Id", DbType.Int32, w.ID);
            parameters.AddInParameter("@Openid", DbType.String, w.Openid);
            parameters.AddInParameter("@Unionid", DbType.String, w.Unionid);
            parameters.AddInParameter("@Nickname", DbType.String, w.Nickname);
            parameters.AddInParameter("@Sex", DbType.String, w.Sex);
            parameters.AddInParameter("@Province", DbType.String, w.Province);
            parameters.AddInParameter("@City", DbType.String, w.City);
            parameters.AddInParameter("@Country", DbType.String, w.Country);
            parameters.AddInParameter("@Headimgurl", DbType.String, w.Headimgurl);
            parameters.AddInParameter("@Privilege", DbType.String, w.Privilege);
            parameters.AddInParameter("@Phone", DbType.String, w.Phone);
            parameters.AddInParameter("@Remark", DbType.String, w.Remark);
            parameters.AddInParameter("@GroupId", DbType.String, w.GroupId);
            parameters.AddInParameter("@Subscribe", DbType.String, w.Subscribe);
            parameters.AddInParameter("@SubscribeTime", DbType.DateTime, w.SubscribeTime);
            parameters.AddInParameter("@Language", DbType.String, w.Language);
            parameters.AddInParameter("@WeixinAcount", DbType.String, w.WeixinAcount);
            parameters.AddInParameter("@CreateTime", DbType.DateTime, w.CreateTime);

            lock (_obj)
            {
                var update = CommDB.ExecuteSqlStringAndReturnSingleField("CommDB.UpdateWeixinUserSubscribe", parameters);

                try
                {
                    return Convert.ToInt32(update);
                }
                catch (Exception ex)
                {

                }
            }

            return 0;
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="w"></param>
        /// <returns></returns>
        public static int UpdateWeixinUserInfo(WeixinUser w)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@Id", DbType.Int32, w.ID);
            parameters.AddInParameter("@Openid", DbType.String, w.Openid);
            parameters.AddInParameter("@Unionid", DbType.String, w.Unionid);
            parameters.AddInParameter("@Nickname", DbType.String, w.Nickname);
            parameters.AddInParameter("@Sex", DbType.String, w.Sex ?? "");
            parameters.AddInParameter("@Province", DbType.String, w.Province ?? "");
            parameters.AddInParameter("@City", DbType.String, w.City ?? "");
            parameters.AddInParameter("@Country", DbType.String, w.Country ?? "");
            parameters.AddInParameter("@Headimgurl", DbType.String, w.Headimgurl ?? "");
            parameters.AddInParameter("@Privilege", DbType.String, w.Privilege ?? "");
            parameters.AddInParameter("@Phone", DbType.String, w.Phone);
            parameters.AddInParameter("@Remark", DbType.String, w.Remark);
            parameters.AddInParameter("@GroupId", DbType.String, w.GroupId);
            parameters.AddInParameter("@Subscribe", DbType.Int32, w.Subscribe);
            parameters.AddInParameter("@SubscribeTime", DbType.DateTime, w.SubscribeTime);
            parameters.AddInParameter("@Language", DbType.String, w.Language ?? "");
            parameters.AddInParameter("@WeixinAcount", DbType.String, w.WeixinAcount);
            parameters.AddInParameter("@CreateTime", DbType.DateTime, w.CreateTime);

            lock (_obj)
            {
                var update = CommDB.ExecuteSqlStringAndReturnSingleField("CommDB.UpdateWeixinUserInfo", parameters);

                try
                {
                    return Convert.ToInt32(update);
                }
                catch (Exception ex)
                {

                }
            }

            return 0;
        }

        /// <summary>
        /// 查询指定unionid指定微信号下的用户信息（如关注状态）
        /// </summary>
        /// <param name="weixinAcount"></param>
        /// <param name="unionid"></param>
        /// <returns></returns>
        public static WeixinUser GetWeixinUserByUnionid(string weixinAcount, string unionid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@WeixinAcount", DbType.String, weixinAcount);
            parameters.AddInParameter("@Unionid", DbType.String, unionid);

            var list = CommDB.ExecuteSqlString<WeixinUser>("CommDB.GetWeixinUserByUnionid", parameters);
            if (list == null || list.Count <= 0)
            {
                return null;
            }

            return list.FirstOrDefault();
        }

        #endregion

        #region 自定义活动相关处理

        public static int AddCustomActiveUser(CustomActiveUser cuser)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@Id", DbType.Int32, cuser.Id);
            parameters.AddInParameter("@OpenId", DbType.String, cuser.OpenId);
            parameters.AddInParameter("@Unionid", DbType.String, cuser.Unionid);
            parameters.AddInParameter("@UserName", DbType.String, cuser.UserName);
            parameters.AddInParameter("@Phone", DbType.String, cuser.Phone);
            parameters.AddInParameter("@Point", DbType.Int32, cuser.Point);
            parameters.AddInParameter("@ActiveId", DbType.Int32, cuser.ActiveId);
            parameters.AddInParameter("@State", DbType.Int32, cuser.State);
            parameters.AddInParameter("@CreateTime", DbType.DateTime, cuser.CreateTime);

            var add = CommDB.ExecuteSqlStringAndReturnSingleField("CommDB.AddCustomActiveUser", parameters);

            try
            {
                return Convert.ToInt32(add);
            }
            catch (Exception ex)
            {

            }

            return 0;
        }

        public static CustomActiveUser GetCustomActiveUser(int activeid, string phone)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ActiveId", DbType.Int32, activeid);
            parameters.AddInParameter("@Phone", DbType.String, phone);

            return CommDB.ExecuteSqlString<CustomActiveUser>("CommDB.GetCustomActiveUser", parameters).FirstOrDefault();
        }

        #endregion

        #region 微信公众号素材相关
        /// <summary>
        /// 这里添加方法注释
        /// </summary>
        public static int AddWeixinMaterial(WeixinMaterialEntity weixinmaterial)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@Type", weixinmaterial.Type);
            parameters.Add("@Title", weixinmaterial.Title);
            parameters.Add("@Content", weixinmaterial.Content);
            parameters.Add("@WeiXinMediaID", weixinmaterial.WeiXinMediaID);
            parameters.Add("@Digest", weixinmaterial.Digest);
            parameters.Add("@ThumbMediaId", weixinmaterial.ThumbMediaId);
            parameters.Add("@Author", weixinmaterial.Author);
            parameters.Add("@Url", weixinmaterial.Url);
            parameters.Add("@ContentSourceUrl", weixinmaterial.ContentSourceUrl);
            parameters.Add("@SourceUpdateTime", weixinmaterial.SourceUpdateTime);
            parameters.Add("@Creator", weixinmaterial.Creator);
            parameters.Add("@CreateTime", weixinmaterial.CreateTime);
            parameters.Add("@Editor", weixinmaterial.Editor);
            parameters.Add("@UpdateTime", weixinmaterial.UpdateTime);
            parameters.Add("@WeixinAcountId", weixinmaterial.WeixinAcountId);
            parameters.Add("@CategoryId", weixinmaterial.CategoryId);
            parameters.Add("@State", weixinmaterial.State);
            int i = CommDB.ExecuteNonQuery("SP_WeixinMaterial_Insert", parameters);
            return i;
        }

        /// <summary>
        /// 这里添加方法注释
        /// </summary>
        public static int UpdateWeixinMaterial(WeixinMaterialEntity weixinmaterial)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@IDX", weixinmaterial.IDX);
            parameters.Add("@Type", weixinmaterial.Type);
            parameters.Add("@Title", weixinmaterial.Title);
            parameters.Add("@Content", weixinmaterial.Content);
            parameters.Add("@WeiXinMediaID", weixinmaterial.WeiXinMediaID);
            parameters.Add("@Digest", weixinmaterial.Digest);
            parameters.Add("@ThumbMediaId", weixinmaterial.ThumbMediaId);
            parameters.Add("@Author", weixinmaterial.Author);
            parameters.Add("@Url", weixinmaterial.Url);
            parameters.Add("@ContentSourceUrl", weixinmaterial.ContentSourceUrl);
            parameters.Add("@SourceUpdateTime", weixinmaterial.SourceUpdateTime);
            parameters.Add("@Creator", weixinmaterial.Creator);
            parameters.Add("@CreateTime", weixinmaterial.CreateTime);
            parameters.Add("@Editor", weixinmaterial.Editor);
            parameters.Add("@UpdateTime", weixinmaterial.UpdateTime);
            parameters.Add("@WeixinAcountId", weixinmaterial.WeixinAcountId);
            parameters.Add("@CategoryId", weixinmaterial.CategoryId);
            parameters.Add("@State", weixinmaterial.State);
            int i = CommDB.ExecuteNonQuery("SP_WeixinMaterial_Update", parameters);
            return i;
        }

        /// <summary>
        /// 查询所有数据,ExecuteSqlString参数为sql配置文件中的key,可以根据实际情况进行更改
        /// </summary>
        public static List<WeixinMaterialEntity> FindAllWeixinMaterial(string key, string catype, int pageIndex, int pageSize)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@Key", DbType.String, key);
            parameters.AddInParameter("@Start", DbType.Int32, (pageIndex - 1) * pageSize);
            parameters.AddInParameter("@End", DbType.Int32, pageIndex * pageSize);
            if (string.IsNullOrEmpty(catype) || catype == "0")
            {
                return CommDB.ExecuteSqlString<WeixinMaterialEntity>("CommDB.GetMaterialList", parameters);
            }
            else
            {
                parameters.AddInParameter("@type", DbType.Int32, int.Parse(catype));
                return CommDB.ExecuteSqlString<WeixinMaterialEntity>("CommDB.GetMaterialListWithType", parameters);
            }
        }
        public static int GetMaterialListCount(string key, string catype)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@Key", DbType.String, key);

            if (string.IsNullOrEmpty(catype) || catype == "0")
            {
                return int.Parse(CommDB.ExecuteSqlStringAndReturnSingleField("CommDB.GetMaterialListCount", parameters).ToString());
            }
            else
            {
                parameters.AddInParameter("@type", DbType.Int32, int.Parse(catype));
                return int.Parse(CommDB.ExecuteSqlStringAndReturnSingleField("CommDB.GetMaterialListCountWithType", parameters).ToString());
            }
        }
        public static WeixinMaterialEntity GetMaterialByIDX(int IDX)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@IDX", DbType.String, IDX);
            return CommDB.ExecuteSqlString<WeixinMaterialEntity>("CommDB.GetMaterialByID", parameters).FirstOrDefault();
        }
        /// <summary>
        /// 这里添加方法注释
        /// </summary>
        public static int DeleteWeixinMaterial(WeixinMaterialEntity weixinmaterial)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@IDX", weixinmaterial.IDX);
            parameters.Add("@Type", weixinmaterial.Type);
            parameters.Add("@Title", weixinmaterial.Title);
            parameters.Add("@Content", weixinmaterial.Content);
            parameters.Add("@WeiXinMediaID", weixinmaterial.WeiXinMediaID);
            parameters.Add("@Digest", weixinmaterial.Digest);
            parameters.Add("@ThumbMediaId", weixinmaterial.ThumbMediaId);
            parameters.Add("@Author", weixinmaterial.Author);
            parameters.Add("@Url", weixinmaterial.Url);
            parameters.Add("@ContentSourceUrl", weixinmaterial.ContentSourceUrl);
            parameters.Add("@SourceUpdateTime", weixinmaterial.SourceUpdateTime);
            parameters.Add("@Creator", weixinmaterial.Creator);
            parameters.Add("@CreateTime", weixinmaterial.CreateTime);
            parameters.Add("@Editor", weixinmaterial.Editor);
            parameters.Add("@UpdateTime", weixinmaterial.UpdateTime);
            parameters.Add("@WeixinAcountId", weixinmaterial.WeixinAcountId);
            parameters.Add("@CategoryId", weixinmaterial.CategoryId);
            parameters.Add("@State", weixinmaterial.State);
            int i = CommDB.ExecuteNonQuery("SP_WeixinMaterial_Delete", parameters);
            return i;
        }


        /// <summary>
        /// 这里添加方法注释
        /// </summary>
        public static int AddWeixinMaterialCategory(WeixinMaterialCategoryEntity weixinmaterialcategory)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@Parent", weixinmaterialcategory.Parent);
            parameters.Add("@Name", weixinmaterialcategory.Name);
            parameters.Add("@State", weixinmaterialcategory.State);
            int i = CommDB.ExecuteNonQuery("SP_WeixinMaterialCategory_Insert", parameters);
            return i;
        }

        /// <summary>
        /// 这里添加方法注释
        /// </summary>
        public static int UpdateWeixinMaterialCategory(WeixinMaterialCategoryEntity weixinmaterialcategory)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@Id", weixinmaterialcategory.Id);
            parameters.Add("@Parent", weixinmaterialcategory.Parent);
            parameters.Add("@Name", weixinmaterialcategory.Name);
            parameters.Add("@State", weixinmaterialcategory.State);
            int i = CommDB.ExecuteNonQuery("SP_WeixinMaterialCategory_Update", parameters);
            return i;
        }

        /// <summary>
        /// 查询所有数据,ExecuteSqlString参数为sql配置文件中的key,可以根据实际情况进行更改
        /// </summary>
        public static List<WeixinMaterialCategoryEntity> FindAllWeixinMaterialCategory()
        {
            var parameters = new DBParameterCollection();
            return CommDB.ExecuteSqlString<WeixinMaterialCategoryEntity>("CommDB.GetMaterialCategoryList", parameters);
        }

        /// <summary>
        /// 这里添加方法注释
        /// </summary>
        public static int DeleteWeixinMaterialCategory(WeixinMaterialCategoryEntity weixinmaterialcategory)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@Id", weixinmaterialcategory.Id);
            parameters.Add("@Parent", weixinmaterialcategory.Parent);
            parameters.Add("@Name", weixinmaterialcategory.Name);
            parameters.Add("@State", weixinmaterialcategory.State);
            int i = CommDB.ExecuteNonQuery("SP_WeixinMaterialCategory_Delete", parameters);
            return i;
        }

        /// <summary>
        /// 根据IDX获取微信素材信息
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public static WeixinMaterialEntity GetWeixinMaterialByIdx(int idx)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@IDX", DbType.Int32, idx);

            return CommDB.ExecuteSqlString<WeixinMaterialEntity>("CommDB.GetWeixinMaterialByIdx", parameters).FirstOrDefault();
        }

        /// <summary>
        /// 关联酒店ID与素材ID
        /// </summary>
        /// <param name="weixinmaterialcategory"></param>
        /// <returns></returns>
        public static int UpdMaterialHotelID(int MaterialID, int HotelID)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@MaterialID", MaterialID);
            parameters.Add("@HotelID", HotelID);
            int i = CommDB.ExecuteNonQuery("SP_WeixinMaterialHotelRel_Update", parameters);
            return i;
        }

        /// <summary>
        /// 删除酒店ID与素材ID
        /// </summary>
        /// <param name="weixinmaterialcategory"></param>
        /// <returns></returns>
        public static int DelMaterialHotelID(int MaterialID, int HotelID)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@MaterialID", MaterialID);
            parameters.Add("@HotelID", HotelID);
            int i = CommDB.ExecuteNonQuery("SP_WeixinMaterialHotelRel_Delete", parameters);
            return i;
        }
        #endregion
    }
}
