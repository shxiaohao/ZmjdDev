using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HJD.Framework.Interface;
using HJD.CouponService.Contracts.Entity;
using System.Data;
using System.Text;
using HJD.CouponService.Impl.Helper;
using HJD.Framework.Tool.IDManager;

namespace HJD.CouponService.Impl.DAL
{
    public class CouponDAL
    {
        private static IDatabaseManager CouponDB = DatabaseManagerFactory.Create("CouponDB");
        private static IDatabaseManager HotelDB = DatabaseManagerFactory.Create("HotelDB");
        private static IDatabaseManager traceLogDB = DatabaseManagerFactory.Create("TraceLogDB");
        private IDatabaseManager CommDB = DatabaseManagerFactory.Create("CommDB_SELECT");

        //获得现金券列表
        internal static List<AcquiredCoupon> GetAcquiredCouponList(long userId, bool? isExpired)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@userId", DbType.Int64, userId);
            if (isExpired.HasValue)
            {
                if ((bool)isExpired)
                {
                    return CouponDB.ExecuteSqlString<AcquiredCoupon>("CouponDB.GetExpiredCouponList", parameters);
                }
                else
                {
                    return CouponDB.ExecuteSqlString<AcquiredCoupon>("CouponDB.GetUnexpiredCouponList", parameters);
                }
            }
            else
            {
                return CouponDB.ExecuteSqlString<AcquiredCoupon>("CouponDB.GetUserCouponList", parameters);
            }
        }

        /// <summary>
        /// 获取用户某一类型的红包数据
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="typeID"></param>
        /// <returns></returns>
        internal static List<OriginCoupon> GetUserOrgCouponInfoByType(long userId, int typeID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@userId", DbType.Int64, userId);
            parameters.AddInParameter("@typeID", DbType.Int32, typeID);
            if (userId != 0)
            {
                return CouponDB.ExecuteSqlString<OriginCoupon>("CouponDB.GetUserOrgCouponInfoByType", parameters);
            }
            else
            {
                return new List<OriginCoupon>();
            }
        }

        /// <summary>
        /// VIP续费
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        internal static bool ReNewVIPAfterPayment(long userID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@userID", DbType.Int64, userID); 
            CouponDB.ExecuteSp("SP_ExchangeCoupon_ReNewVIPAfterPayment", parameters);
            return true;
        }

        //获得红包列表
        internal static List<OriginCoupon> GetCashCouponList(long userId)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@userId", DbType.Int64, userId);
            if (userId != 0)
            {
                return CouponDB.ExecuteSqlString<OriginCoupon>("CouponDB.GetOriginCouponList", parameters);
            }
            else
            {
                return new List<OriginCoupon>();
            }
        }

        /// <summary>
        /// 将老金牌VIP转成新金牌VIP
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        internal static bool UpdOldVIPtoNewVIP(long userId)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@userId", DbType.Int64, userId);
            CouponDB.ExecuteSp("sp_ExchangeCoupon_UpdOldVIPtoNewVIP", parameters);

            return true;
        }

        /// <summary>
        ///  清除用户6M金牌数据以及相应的角色
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        internal static bool Del6MVIP(long userId)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@userId", DbType.Int64, userId);
            CouponDB.ExecuteSp("sp_ExchangeCoupon_Del6MVIP", parameters);

            return true;
        }

        internal static int UpdateAcquiredCoupon(AcquiredCoupon param)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@id", DbType.Int64, param.ID);
            parameters.AddInParameter("@originId", DbType.Int64, param.OriginId);
            parameters.AddInParameter("@userId", DbType.Int64, param.UserId);
            parameters.AddInParameter("@totalMoney", DbType.Int64, param.TotalMoney);
            parameters.AddInParameter("@restMoney", DbType.Int64, param.RestMoney);
            parameters.AddInParameter("@expiredMoney", DbType.Int64, param.ExpiredMoney);//过期金额待斟酌 不是很有必要
            parameters.AddInParameter("@acquiredTime", DbType.DateTime, param.AcquiredTime);
            parameters.AddInParameter("@expiredTime", DbType.DateTime, param.ExpiredTime);
            return CouponDB.ExecuteNonQuery("SP_AcquiredCoupon_InsertOrUpdate", parameters);
        }

        internal static int InsertAcquiredCoupon(AcquiredCoupon param)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@originId", DbType.Int64, param.OriginId);
            parameters.AddInParameter("@userId", DbType.Int64, param.UserId);
            parameters.AddInParameter("@totalMoney", DbType.Int64, param.TotalMoney);
            parameters.AddInParameter("@restMoney", DbType.Int64, param.RestMoney);
            parameters.AddInParameter("@expiredMoney", DbType.Int64, param.ExpiredMoney);//过期金额待斟酌 不是很有必要
            parameters.AddInParameter("@acquiredTime", DbType.DateTime, param.AcquiredTime);
            parameters.AddInParameter("@expiredTime", DbType.DateTime, param.ExpiredTime);
            parameters.AddInParameter("@phoneNo", DbType.String, param.PhoneNo);
            return CouponDB.ExecuteNonQuery("SP_AcquiredCoupon_InsertAndReturn", parameters);
        }

        internal static int GetUserCouponSum(long userId)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@userId", DbType.Int64, userId);
            Object obj = CouponDB.ExecuteSqlStringAndReturnSingleField("CouponDB.GetUserCouponSum", parameters);
            try
            {
                if (obj == null || Convert.IsDBNull(obj))
                {
                    return 0;
                }
                return (int)obj;
            }
            catch
            {
                return Convert.ToInt32(obj.ToString());
            }
        }

        internal static int UpdateUseCouponRecord(long orderId, decimal money)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@orderId", DbType.Int64, orderId);
            parameters.AddInParameter("@discountMoney", DbType.Int64, money);
            return CouponDB.ExecuteNonQuery("SP_UseCouponRecord_InsertOrUpdate", parameters);
        }

        internal static long GenerateOriginCoupon(long userId, int typeId, long sourceId, int totalMoney, int cashMoney, string guid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@id", DbType.Int64, 0);
            parameters.AddInParameter("@userId", DbType.Int64, userId);
            parameters.AddInParameter("@typeId", DbType.Int32, typeId);

            parameters.AddInParameter("@sourceId", DbType.Int64, sourceId);
            parameters.AddInParameter("@totalMoney", DbType.Int64, totalMoney);
            parameters.AddInParameter("@cashMoney", DbType.Int32, cashMoney);
            parameters.AddInParameter("@GUID", DbType.String, guid);
            parameters.AddInParameter("@state", DbType.Int16, 0);
            object obj = CouponDB.ExecuteSprocAndReturnSingleField("SP_OriginCoupon_InsertOrUpdate", parameters, "ExecInfo");
            try
            {
                return (long)obj;
            }
            catch
            {
                return Convert.ToInt64(obj.ToString());
            }
        }

        internal static long UpdateOriginCoupon(long id, string guid, int state)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@id", DbType.Int64, id);
            parameters.AddInParameter("@userId", DbType.Int64, 0);
            parameters.AddInParameter("@typeId", DbType.Int32, 0);
            parameters.AddInParameter("@sourceId", DbType.Int64, 0);
            parameters.AddInParameter("@totalMoney", DbType.Int64, 0);
            parameters.AddInParameter("@cashMoney", DbType.Int32, 0);
            parameters.AddInParameter("@GUID", DbType.String, guid);
            parameters.AddInParameter("@state", DbType.Int16, state);
            return CouponDB.ExecuteNonQuery("SP_OriginCoupon_InsertOrUpdate", parameters);
        }

        internal static long UpdateCashCouponByKeyColumn(long sourceId, int typeId, long userId, int state)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@id", DbType.Int64, 0);
            parameters.AddInParameter("@userId", DbType.Int64, userId);
            parameters.AddInParameter("@typeId", DbType.Int32, typeId);
            parameters.AddInParameter("@sourceId", DbType.Int64, sourceId);
            parameters.AddInParameter("@totalMoney", DbType.Int64, 0);
            parameters.AddInParameter("@cashMoney", DbType.Int32, 0);
            parameters.AddInParameter("@GUID", DbType.String, null);
            parameters.AddInParameter("@state", DbType.Int16, state);
            object obj = CouponDB.ExecuteSprocAndReturnSingleField("SP_OriginCoupon_InsertOrUpdate", parameters, "ExecInfo");
            try
            {
                return (long)obj;
            }
            catch
            {
                return Convert.ToInt64(obj.ToString());
            }
        }

        internal static long GetAcquiredCouponId(long userId, long originId, string phoneNo)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@userId", DbType.Int64, userId);
            parameters.AddInParameter("@originId", DbType.Int64, originId);
            parameters.AddInParameter("@phoneNo", DbType.String, phoneNo);
            object obj = CouponDB.ExecuteSprocAndReturnSingleField("SP_AcquiredCoupon_UpdateAndReturn", parameters, "ExecInfo");
            try
            {
                return (long)obj;
            }
            catch
            {
                return Convert.ToInt64(obj.ToString());
            }
        }

        internal static AcquiredCoupon GetAcquiredCouponById(long id)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@id", DbType.Int64, id);
            return CouponDB.ExecuteSqlString<AcquiredCoupon>("CouponDB.GetAcquiredCouponById", parameters).FirstOrDefault<AcquiredCoupon>();
        }

        internal static long GetOriginIdByGUID(string guid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@guid", DbType.String, guid);
            object obj = CouponDB.ExecuteSqlStringAndReturnSingleField("CouponDB.GetOriginIdByGUID", parameters);
            try
            {
                if (obj == null || Convert.IsDBNull(obj))
                {
                    return 0;
                }
                return (long)obj;
            }
            catch
            {
                return Convert.ToInt64(obj.ToString());
            }
        }

        internal static int GetOrderRecord(long orderId)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@orderId", DbType.Int64, orderId);
            object obj = CouponDB.ExecuteSqlStringAndReturnSingleField("CouponDB.GetOrderRecord", parameters);
            try
            {
                if (obj == null || Convert.IsDBNull(obj))
                {
                    return 0;
                }
                return (int)obj;
            }
            catch
            {
                return Convert.ToInt32(obj.ToString());
            }
        }
        /// <summary>
        /// userId不能为空 一定是抢走的红包
        /// </summary>
        /// <param name="originId"></param>
        /// <returns></returns>
        internal static List<AcquiredCoupon> GetAcquiredCouponList(long originId)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@originId", DbType.Int64, originId);
            return CouponDB.ExecuteSqlString<AcquiredCoupon>("CouponDB.GetAcquiredCouponList", parameters);
        }

        internal static string GetOriginGUIDByOrderAndTypeId(long sourceId, int typeId)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@sourceId", DbType.Int64, sourceId);
            parameters.AddInParameter("@typeId", DbType.Int32, typeId);
            object obj = CouponDB.ExecuteSqlStringAndReturnSingleField("CouponDB.GetOriginGUIDByOrderAndTypeId", parameters);
            try
            {
                if (obj == null || Convert.IsDBNull(obj))
                {
                    return "";
                }
                return obj.ToString();
            }
            catch
            {
                return "";
            }
        }

        internal static int GetAcquiredCouponByPhone(string phoneNo, string guid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@phoneNo", DbType.String, phoneNo);
            parameters.AddInParameter("@guid", DbType.String, guid);
            object obj = CouponDB.ExecuteSqlStringAndReturnSingleField("CouponDB.GetAcquiredCouponByPhone", parameters);
            try
            {
                if (obj == null || Convert.IsDBNull(obj))
                {
                    return 0;
                }
                return (int)obj;
            }
            catch
            {
                return Convert.ToInt32(obj.ToString());
            }
        }
        internal static CouponActivitySKURelEntity GetCouponActivitySKURelBySKUID(int SKUID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@SKUID", DbType.Int32, SKUID);
            var list = CouponDB.ExecuteSqlString<CouponActivitySKURelEntity>("CouponDB.GetCouponActivitySKURelBySKUID", parameters);
            if (list != null && list.Count != 0)
            {
                return list[0];
            }
            else
            {
                return new CouponActivitySKURelEntity();
            }
        }

        internal static CouponActivityEntity GetCouponActivityBySKUID(int SKUID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@SKUID", DbType.Int32, SKUID);
            var list = CouponDB.ExecuteSqlString<CouponActivityEntity>("CouponDB.GetCouponActivityBySKUID", parameters);
            if (list != null && list.Count != 0)
            {
                return list[0];
            }
            else
            {
                return new CouponActivityEntity();
            }
        }

        internal static List<CouponActivityEntity> GetCouponActivityBySourceID(int sourceID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@SourceID", DbType.Int32, sourceID);
            var list = CouponDB.ExecuteSqlString<CouponActivityEntity>("CouponDB.GetCouponActivityBySourceID", parameters);
            return list;
        }

        internal static List<CouponActivitySKURelEntity> GetCouponActivitySKURelByActivityID(int ActivityID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ActivityID", DbType.Int32, ActivityID);
            var list = CouponDB.ExecuteSqlString<CouponActivitySKURelEntity>("CouponDB.GetCouponActivitySKURelByActivityID", parameters);
            if (list != null && list.Count != 0)
            {
                return list;
            }
            else
            {
                return new List<CouponActivitySKURelEntity>();
            }
        }


        internal static OriginCoupon GetCashCouponAmount(long id, string guid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@id", DbType.Int64, id);
            parameters.AddInParameter("@guid", DbType.String, guid);
            List<OriginCoupon> list = CouponDB.ExecuteSqlString<OriginCoupon>("CouponDB.GetCashCoupon", parameters);
            if (list != null && list.Count != 0)
            {
                return list[0];
            }
            else
            {
                return new OriginCoupon();
            }
        }
        internal static List<CouponTypeDefineEntity> GetCouponTypeDefine()
        {
            var parameters = new DBParameterCollection();
            return CouponDB.ExecuteSqlString<CouponTypeDefineEntity>("CouponDB.GetCouponTypeDefine", parameters);
        }

        internal static CouponTypeDefineEntity GetCouponTypeDefineById(int typeId)
        {
            CouponTypeDefineEntity define = null;
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@id", DbType.Int32, typeId);
            List<CouponTypeDefineEntity> list = CouponDB.ExecuteSqlString<CouponTypeDefineEntity>("CouponDB.GetCouponTypeDefineById", parameters);
            if (list != null && list.Count != 0)
            {
                define = list[0];
            }
            else
            {
                define = new CouponTypeDefineEntity();
            }
            return define;
        }

        internal static CouponTypeDefineEntity GetCouponTypeDefineByType(int type)
        {
            CouponTypeDefineEntity define = null;
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@type", DbType.Int32, type);
            List<CouponTypeDefineEntity> list = CouponDB.ExecuteSqlString<CouponTypeDefineEntity>("CouponDB.GetCouponTypeDefineByType", parameters);
            if (list != null && list.Count != 0)
            {
                define = list[0];
            }
            else
            {
                define = new CouponTypeDefineEntity();
            }
            return define;
        }

        internal static CouponTypeDefineEntity GetCouponTypeDefineByCode(CouponActivityCode code)
        {
            CouponTypeDefineEntity define = null;
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@code", DbType.String, code);
            List<CouponTypeDefineEntity> list = CouponDB.ExecuteSqlString<CouponTypeDefineEntity>("CouponDB.GetCouponTypeDefineByCode", parameters);
            if (list != null && list.Count != 0)
            {
                define = list[0];
            }
            else
            {
                define = new CouponTypeDefineEntity();
            }
            return define;
        }

        internal static CouponTypeDefineEntity GetCouponTypeDefineEntityByCode(string code)
        {
            CouponTypeDefineEntity define = null;
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@code", DbType.String, code);
            List<CouponTypeDefineEntity> list = CouponDB.ExecuteSqlString<CouponTypeDefineEntity>("CouponDB.GetCouponTypeDefineEntityByCode", parameters);
            if (list != null && list.Count != 0)
            {
                define = list[0];
            }
            else
            {
                define = new CouponTypeDefineEntity();
            }
            return define;
        }

        internal static void InsertOrUpdateInspectorReward(InspectorRewardEntity ire)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@orderid", DbType.Int64, ire.OrderId);
            dbParameterCollection.AddInParameter("@inspector", DbType.Int64, ire.Inspector);
            dbParameterCollection.AddInParameter("@commission", DbType.Int32, ire.Commission);
            dbParameterCollection.AddInParameter("@state", DbType.Int32, ire.State);
            CouponDB.ExecuteSprocAndReturnSingleField("SP_InspectorReward_InsertOrUpdate", dbParameterCollection);
        }

        internal static List<InspectorRewardItem> GetInspectorRewardItemList(long userid)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@userid", DbType.Int64, userid);
            List<InspectorRewardItem> itemList = CouponDB.ExecuteSqlString<InspectorRewardItem>("CouponDB.GetInspectorRewardByUserId", dbParameterCollection);
            return itemList != null && itemList.Count != 0 ? itemList : new List<InspectorRewardItem>();
        }

        internal static List<UseCouponRecordEntity> GetUseCouponRecordByUserID(long userid)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@userid", DbType.Int64, userid);
            List<UseCouponRecordEntity> itemList = CouponDB.ExecuteSqlString<UseCouponRecordEntity>("CouponDB.GetUseCouponRecordByUserID", dbParameterCollection);
            return itemList != null && itemList.Count != 0 ? itemList : new List<UseCouponRecordEntity>();
        }

        internal static List<AcquiredCoupon> GetAcquireCouponRecordByUserID(long userid)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@userid", DbType.Int64, userid);
            List<AcquiredCoupon> itemList = CouponDB.ExecuteSqlString<AcquiredCoupon>("CouponDB.GetAcquireCouponRecordByUserID", dbParameterCollection);
            return itemList != null && itemList.Count != 0 ? itemList : new List<AcquiredCoupon>();
        }

        internal static OriginCoupon GetOriginCouponByTypeAndSourceID(long sourceId, int typeId)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@sourceId", DbType.Int64, sourceId);
            parameters.AddInParameter("@typeId", DbType.Int32, typeId);
            List<OriginCoupon> obj = CouponDB.ExecuteSqlString<OriginCoupon>("CouponDB.GetOriginCoupon", parameters);
            return obj != null && obj.Count != 0 ? obj[0] : null;
        }

        internal static int InsertCouponActivity(CouponActivityEntity sae)
        {
            //CouponActivityEntity oldca = GetOneCouponActivityByIdAndType(sae.ID, sae.Type);
            //if (sae.Type == 300 && oldca != null)
            //{
            //    ObjectUpdateLog(sae.CurUserId, sae.ID, "", OpLogBizType.CouponActivity300, oldca, sae);
            //}
            //else if (sae.Type == 200 && oldca != null)
            //{
            //    ObjectUpdateLog(sae.CurUserId, sae.ID, "", OpLogBizType.CouponActivity200, oldca, sae);
            //}
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@SourceID", DbType.Int32, sae.SourceID);
            parameters.AddInParameter("@EffectiveTime", DbType.DateTime, sae.EffectiveTime);
            parameters.AddInParameter("@TotalNum", DbType.Int32, sae.TotalNum);
            parameters.AddInParameter("@SellNum", DbType.Int32, sae.SellNum);
            parameters.AddInParameter("@SingleBuyNum", DbType.Int32, sae.SingleBuyNum);
            parameters.AddInParameter("@Type", DbType.Int32, sae.Type);
            parameters.AddInParameter("@Description", DbType.String, sae.Description);
            parameters.AddInParameter("@Notice", DbType.String, sae.Notice);
            parameters.AddInParameter("@PicPath", DbType.String, sae.PicPath);
            parameters.AddInParameter("@Price", DbType.String, sae.Price);
            parameters.AddInParameter("@State", DbType.Int32, sae.State);
            parameters.AddInParameter("@Creator", DbType.Int64, sae.Creator);
            parameters.AddInParameter("@Updator", DbType.Int64, sae.Updator);
            parameters.AddInParameter("@ExpireTime", DbType.DateTime, sae.ExpireTime);
            parameters.AddInParameter("@StartTime", DbType.DateTime, sae.StartTime);
            parameters.AddInParameter("@Rank", DbType.Int32, sae.Rank);
            parameters.AddInParameter("@ReturnPolicy", DbType.Int32, sae.ReturnPolicy);
            parameters.AddInParameter("@ManuSellNum", DbType.Int32, sae.ManuSellNum);
            parameters.AddInParameter("@MinBuyNum", DbType.Int32, sae.MinBuyNum);
            parameters.AddInParameter("@NightRoomCount", DbType.Int32, sae.NightRoomCount);
            parameters.AddInParameter("@EDMTitle", DbType.String, sae.EDMTitle);
            parameters.AddInParameter("@IsFestivalCanUse", DbType.Int32, sae.IsFestivalCanUse);
            parameters.AddInParameter("@IsAllowMultiRoom", DbType.Boolean, sae.IsAllowMultiRoom);
            parameters.AddInParameter("@GroupNo", DbType.Int32, sae.GroupNo);
            parameters.AddInParameter("@PackageInfo", DbType.String, sae.PackageInfo);
            parameters.AddInParameter("@PageTitle", DbType.String, sae.PageTitle);
            parameters.AddInParameter("@PriceLabel", DbType.String, sae.PriceLabel);
            parameters.AddInParameter("@MarketPrice", DbType.Int32, sae.MarketPrice);
            parameters.AddInParameter("@ExchangeMethod", DbType.Int32, sae.ExchangeMethod);
            parameters.AddInParameter("@RecommendTitle", DbType.String, sae.RecommendTitle);
            parameters.AddInParameter("@LabelPicUrl", DbType.String, sae.LabelPicUrl);
            parameters.AddInParameter("@SaleEndDate", DbType.DateTime, sae.SaleEndDate);
            parameters.AddInParameter("@RelationId", DbType.Int32, sae.RelationId);
            parameters.AddInParameter("@IsVipExclusive", DbType.Boolean, sae.IsVipExclusive);
            parameters.AddInParameter("@GroupSponsorCanBuyVIPFirstCoupon", DbType.Boolean, sae.GroupSponsorCanBuyVIPFirstCoupon);
            parameters.AddInParameter("@IsValid", DbType.Boolean, sae.IsValid);
            parameters.AddInParameter("@GroupCount", DbType.Int32, sae.GroupCount);
            parameters.AddInParameter("@SerialNO", DbType.String, sae.SerialNO);
            parameters.AddInParameter("@MerchantCode", DbType.String, sae.MerchantCode);
            // parameters.AddInParameter("@RelProductID", DbType.Int32, sae.RelProductID);
            parameters.AddInParameter("@RelPackageAlbumsID", DbType.Int32, sae.RelPackageAlbumsID);
            parameters.AddInParameter("@Tags", DbType.String, sae.Tags);
            parameters.AddInParameter("@MoreDetailUrl", DbType.String, sae.MoreDetailUrl);
            parameters.AddInParameter("@Properties", DbType.String, sae.Properties);
            parameters.AddInParameter("@CouponNote", DbType.String, sae.CouponNote);
            parameters.AddInParameter("@GroupDay", DbType.Int32, sae.GroupDay);
            parameters.AddInParameter("@UseDecription", DbType.String, sae.UseDecription);
            parameters.AddInParameter("@SupplierId", DbType.Int32, sae.SupplierID);
            //parameters.AddInParameter("@GroupPurchaseNum", DbType.Int32, sae.GroupPurchaseNum);
            parameters.AddInParameter("@IsShowCountDown", DbType.Boolean, sae.IsShowCountDown);
            parameters.AddInParameter("@WeixinAcountId", DbType.Int32, sae.WeixinAcountId);

            Object obj = CouponDB.ExecuteSprocAndReturnSingleField("SP_CouponActivity_Insert", parameters);
            return Convert.ToInt32(obj);
        }

        internal static int UpdateCouponActivity(CouponActivityEntity sae)
        {
            TimeLog log = new TimeLog("UpdateCouponActivity",1);
            CouponActivityEntity oldca = GetOneCouponActivityByIdAndType(sae.ID, sae.Type);
            log.AddLog("GetOneCouponActivityByIdAndType");
            if (sae.Type == 300 && oldca != null)
            {
                ObjectUpdateLog(sae.CurUserId, sae.ID, "", OpLogBizType.CouponActivity300, oldca, sae);
                log.AddLog("ObjectUpdateLog_Type300");
            }
            else if (sae.Type == 200 && oldca != null)
            {
                ObjectUpdateLog(sae.CurUserId, sae.ID, "", OpLogBizType.CouponActivity200, oldca, sae);
                log.AddLog("ObjectUpdateLog_Type200");
            }
            else if (sae.Type == 600 && oldca != null)
            {
                ObjectUpdateLog(sae.CurUserId, sae.ID, "", OpLogBizType.CouponActivity600, oldca, sae);
                log.AddLog("ObjectUpdateLog_Type600");
            }
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ID", DbType.Int32, sae.ID);
            parameters.AddInParameter("@SourceID", DbType.Int32, sae.SourceID);
            parameters.AddInParameter("@EffectiveTime", DbType.DateTime, sae.EffectiveTime);
            parameters.AddInParameter("@TotalNum", DbType.Int32, sae.TotalNum);
            parameters.AddInParameter("@SellNum", DbType.Int32, sae.SellNum);
            parameters.AddInParameter("@SingleBuyNum", DbType.Int32, sae.SingleBuyNum);
            parameters.AddInParameter("@Description", DbType.String, sae.Description);
            parameters.AddInParameter("@Notice", DbType.String, sae.Notice);
            parameters.AddInParameter("@PicPath", DbType.String, sae.PicPath);
            parameters.AddInParameter("@Price", DbType.String, sae.Price);
            parameters.AddInParameter("@State", DbType.Int32, sae.State);
            parameters.AddInParameter("@Updator", DbType.Int64, sae.Updator);
            parameters.AddInParameter("@ExpireTime", DbType.DateTime, sae.ExpireTime);
            parameters.AddInParameter("@StartTime", DbType.DateTime, sae.StartTime);
            parameters.AddInParameter("@Rank", DbType.Int32, sae.Rank);
            parameters.AddInParameter("@ReturnPolicy", DbType.Int32, sae.ReturnPolicy);
            parameters.AddInParameter("@ManuSellNum", DbType.Int32, sae.ManuSellNum);
            parameters.AddInParameter("@NightRoomCount", DbType.Int32, sae.NightRoomCount);
            parameters.AddInParameter("@MinBuyNum", DbType.Int32, sae.MinBuyNum);
            parameters.AddInParameter("@EDMTitle", DbType.String, sae.EDMTitle);
            parameters.AddInParameter("@IsFestivalCanUse", DbType.Int32, sae.IsFestivalCanUse);
            parameters.AddInParameter("@IsAllowMultiRoom", DbType.Boolean, sae.IsAllowMultiRoom);
            parameters.AddInParameter("@GroupNo", DbType.Int32, sae.GroupNo);
            parameters.AddInParameter("@PackageInfo", DbType.String, sae.PackageInfo);
            parameters.AddInParameter("@PageTitle", DbType.String, sae.PageTitle);
            parameters.AddInParameter("@PriceLabel", DbType.String, sae.PriceLabel);
            parameters.AddInParameter("@MarketPrice", DbType.Int32, sae.MarketPrice);
            parameters.AddInParameter("@ExchangeMethod", DbType.Int32, sae.ExchangeMethod);
            parameters.AddInParameter("@RecommendTitle", DbType.String, sae.RecommendTitle);
            parameters.AddInParameter("@LabelPicUrl", DbType.String, sae.LabelPicUrl);
            parameters.AddInParameter("@MerchantCode", DbType.String, sae.MerchantCode);
            parameters.AddInParameter("@SaleEndDate", DbType.DateTime, sae.SaleEndDate);
            parameters.AddInParameter("@RelationId", DbType.Int32, sae.RelationId);
            parameters.AddInParameter("@IsVipExclusive", DbType.Boolean, sae.IsVipExclusive);
            parameters.AddInParameter("@GroupSponsorCanBuyVIPFirstCoupon", DbType.Boolean, sae.GroupSponsorCanBuyVIPFirstCoupon);
            parameters.AddInParameter("@IsValid", DbType.Boolean, sae.IsValid);
            parameters.AddInParameter("@GroupCount", DbType.Int32, sae.GroupCount);
            parameters.AddInParameter("@SerialNO", DbType.String, sae.SerialNO);
            //  parameters.AddInParameter("@RelProductID", DbType.Int32, sae.RelProductID);
            parameters.AddInParameter("@RelPackageAlbumsID", DbType.Int32, sae.RelPackageAlbumsID);
            parameters.AddInParameter("@Tags", DbType.String, sae.Tags);
            parameters.AddInParameter("@MoreDetailUrl", DbType.String, sae.MoreDetailUrl);
            parameters.AddInParameter("@Properties", DbType.String, sae.Properties);
            parameters.AddInParameter("@CouponNote", DbType.String, sae.CouponNote);
            parameters.AddInParameter("@GroupDay", DbType.Int32, sae.GroupDay);
            parameters.AddInParameter("@UseDecription", DbType.String, sae.UseDecription);
            parameters.AddInParameter("@SupplierId", DbType.Int32, sae.SupplierID);
            parameters.AddInParameter("@IsShowCountDown", DbType.Boolean, sae.IsShowCountDown);
            parameters.AddInParameter("@WeixinAcountId", DbType.Int32, sae.WeixinAcountId);

            Object obj = CouponDB.ExecuteSprocAndReturnSingleField("SP_CouponActivity_Update", parameters);
            log.AddLog("SP_CouponActivity_Update");
            log.Finish();
            return Convert.ToInt32(obj);
        }

        internal static int UpdateExchangeNO(ExchangeCouponEntity param)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ID", DbType.Int32, param.ID);
            parameters.AddInParameter("@State", DbType.Int32, param.State);
            parameters.AddInParameter("@ExchangeNo", DbType.String, param.ExchangeNo);
            parameters.AddInParameter("@Updator", DbType.Int64, param.Updator);
            Object obj = CouponDB.ExecuteSprocAndReturnSingleField("SP_ExchangeCoupon_UpdateExchangeNo", parameters);
            return Convert.ToInt32(obj);
        }

        public static int AddCouponActivitySKURel(int CouponActivityID, string RelSKUIDs)
        {
            if (RelSKUIDs == null)
            {
                RelSKUIDs = "";
            }
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@CouponActivityID", CouponActivityID);
            parameters.Add("@SKUIDs", RelSKUIDs);
            int i = CouponDB.ExecuteNonQuery("SP_CouponActivitySKURel_Insert", parameters);
            return i;
        }

        public static int AddBizOpLogData(BizOpLogEntity bizoplog)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@OperatorUserID", bizoplog.OperatorUserID);
            parameters.Add("@BizType", (int)bizoplog.BizType);
            parameters.Add("@BizID", bizoplog.BizID);
            parameters.Add("@BizIDStr", bizoplog.BizIDStr);
            parameters.Add("@OpType", bizoplog.OpType);
            parameters.Add("@OpContent", bizoplog.OpContent);
            int i = traceLogDB.ExecuteNonQuery("SP_BizOpLog_Insert", parameters);
            return i;
        }
        public static bool ObjectUpdateLog(long editor, long lKey, string strKey, OpLogBizType logType, object oldObj, object newObj, string LogPropertyList = "AllProperty", string ExceptLogPropertyList = "")
        {

            AddBizOpLogData(new BizOpLogEntity()
            {
                BizID = lKey,
                BizIDStr = strKey,
                BizType = logType,
                OpType = (int)OpLogBizOpType.Update,
                OpContent = GenCommUpdateLogDesc(oldObj, newObj, LogPropertyList, ExceptLogPropertyList),
                OperatorUserID = editor
            });

            return true;
        }
        public static string GenCommUpdateLogDesc(object oldObj, object newObj, string LogPropertyList = "AllProperty", string ExceptLogPropertyList = "")
        {
            List<string> logPropertyList = new List<string>();
            List<string> exceptLogPropertyList = new List<string>();
            StringBuilder sb = new StringBuilder();
            if (LogPropertyList != "AllProperty")
            {
                logPropertyList = LogPropertyList.Split(",".ToCharArray()).ToList();
            }
            if (ExceptLogPropertyList != "")
            {
                exceptLogPropertyList = ExceptLogPropertyList.Split(",".ToCharArray()).ToList();
            }
            foreach (System.Reflection.PropertyInfo p in oldObj.GetType().GetProperties())
            {
                try
                {
                    if (exceptLogPropertyList.Count == 0 || !exceptLogPropertyList.Contains(p.Name)) //排除
                    {
                        if (logPropertyList.Count == 0 || logPropertyList.Contains(p.Name)) //只记录需要记录的属性
                        {

                            string oldValue = p.GetValue(oldObj) == null ? "" : p.GetValue(oldObj).ToString();
                            string newValue = p.GetValue(newObj) == null ? "" : p.GetValue(newObj).ToString();

                            if (oldValue != newValue)
                            {
                                sb.Append(string.Format("{0}:oldValue={1}: newValue={2}", p.Name, oldValue, newValue));
                            }
                        }
                    }
                }
                catch (Exception err)
                {
                    string msg = err.Message;
                }
            }

            return sb.ToString();
        }
        public static CouponActivityEntity GetOneCouponActivityByIdAndType(int id, int type)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@id", DbType.Int32, id);
            parameters.AddInParameter("@type", DbType.Int32, type);

            List<CouponActivityEntity> obj = CouponDB.ExecuteSqlString<CouponActivityEntity>("CouponDB.GetOneCouponActivityByIdAndType", parameters);
            return obj != null && obj.Count != 0 ? obj[0] : new CouponActivityEntity();
        }


        public static int SP4_ExchangeCoupon_AdjustPriceForPackageSKU(int payid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@payid", DbType.Int32, payid);
            return  CouponDB.ExecuteNonQuery("SP4_ExchangeCoupon_AdjustPriceForPackageSKU", parameters);  
        }


        internal static CouponActivityEntity GetOneCouponActivity(int id, bool isLock)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@id", DbType.Int32, id);

            List<CouponActivityEntity> obj = isLock ? CouponDB.ExecuteSqlString<CouponActivityEntity>("CouponDB.GetOneCouponActivityWithLock", parameters) : CouponDB.ExecuteSqlString<CouponActivityEntity>("CouponDB.GetOneCouponActivityWithNoLock", parameters);
            return obj != null && obj.Count != 0 ? obj[0] : new CouponActivityEntity();
        }


        internal static CouponActivityEntity GetOneCouponActivityAndSKU(int id, bool isLock)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@id", DbType.Int32, id);

            List<CouponActivityEntity> obj = isLock ? CouponDB.ExecuteSqlString<CouponActivityEntity>("CouponDB.GetOneCouponActivityAndSKUWithLock", parameters) : CouponDB.ExecuteSqlString<CouponActivityEntity>("CouponDB.GetOneCouponActivityAndSKUWithNoLock", parameters);
            return obj != null && obj.Count != 0 ? obj[0] : new CouponActivityEntity();
        }

        internal static CouponActivityEntity GetCouponActivityByBizIdAndBizType(int bizId, int bizType)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@BizId", DbType.Int32, bizId);
            parameters.AddInParameter("@BizType", DbType.Int32, bizType);

            List<CouponActivityEntity> obj = CouponDB.ExecuteSqlString<CouponActivityEntity>("CouponDB.GetCouponActivityByBizIdAndBizType", parameters);
            return obj != null && obj.Count != 0 ? obj[0] : new CouponActivityEntity();
        }

        internal static List<CouponActivityEntity> GetCouponActivityList(CouponActivityQueryParam param, out int totalCount)
        {
            var parameters = new DBParameterCollection();
            totalCount = 0;
            parameters.AddInParameter("@stateItems", DbType.String, param.stateArray == null ? "0,1,2,3,4" : string.Join(",", param.stateArray));
            parameters.AddInParameter("@activityTypeItems", DbType.String, param.activityTypeArray == null ? "200,300,400" : string.Join(",", param.activityTypeArray));
            parameters.AddInParameter("@merchantCode", DbType.String, param.merchantCode.ToString());
            parameters.AddInParameter("@lastEditTime", DbType.DateTime, param.lastEditTime == null ? new DateTime(2000, 1, 1, 0, 0, 0) : (DateTime)param.lastEditTime);
            parameters.AddInParameter("@GroupNo", DbType.Int32, param.GroupNo);


            object obj = CouponDB.ExecuteSqlStringAndReturnSingleField("CouponDB.GetCouponActivityCount", parameters);
            totalCount = Convert.ToInt32(obj);
            if (totalCount == 0)
            {
                return new List<CouponActivityEntity>();
            }
            parameters.AddInParameter("@start", DbType.Int32, param.PageIndex == 0 ? 0 : (param.PageIndex - 1) * param.PageSize);
            parameters.AddInParameter("@count", DbType.Int32, param.PageSize == 0 ? 10 : param.PageSize);
            List<CouponActivityEntity> itemList = CouponDB.ExecuteSqlString<CouponActivityEntity>("CouponDB.GetCouponActivityList", parameters);
            return itemList != null && itemList.Count != 0 ? itemList : new List<CouponActivityEntity>();
        }

        internal static List<CouponActivityEntity> GetCouponActivityBySKUIDSList(CouponActivityQueryParam param, out int totalCount)
        {
            var parameters = new DBParameterCollection();
            totalCount = 0;
            parameters.AddInParameter("@stateItems", DbType.String, param.stateArray == null ? "0,1,2,3,4" : string.Join(",", param.stateArray));
            parameters.AddInParameter("@activityTypeItems", DbType.String, param.activityTypeArray == null ? "200,300,400" : string.Join(",", param.activityTypeArray));
            parameters.AddInParameter("@lastEditTime", DbType.DateTime, param.lastEditTime == null ? new DateTime(2000, 1, 1, 0, 0, 0) : (DateTime)param.lastEditTime);
            parameters.AddInParameter("@SKUIDS", DbType.String, param.SKUIDS);
            parameters.AddInParameter("@ProductAlbumID", DbType.String, param.ProductAlbumID);

            //parameters.AddInParameter("@merchantCode", DbType.String, param.merchantCode.ToString());
            //parameters.AddInParameter("@GroupNo", DbType.Int32, param.GroupNo);


            object obj = CouponDB.ExecuteSqlStringAndReturnSingleField("CouponDB.GetCouponActivityBySKUIDSCount", parameters);
            totalCount = Convert.ToInt32(2);
            if (totalCount == 0)
            {
                return new List<CouponActivityEntity>();
            }
            parameters.AddInParameter("@start", DbType.Int32, param.PageIndex == 0 ? 0 : (param.PageIndex - 1) * param.PageSize);
            parameters.AddInParameter("@count", DbType.Int32, param.PageSize == 0 ? 10 : param.PageSize);
            List<CouponActivityEntity> itemList = CouponDB.ExecuteSqlString<CouponActivityEntity>("CouponDB.GetCouponActivityBySKUIDSList", parameters);
            return itemList != null && itemList.Count != 0 ? itemList : new List<CouponActivityEntity>();
        }


        internal static List<CouponActivityEntity> MemberCouponActivityList(CouponActivityQueryParam param, out int totalCount)
        {
            var parameters = new DBParameterCollection();
            totalCount = 0;
            parameters.AddInParameter("@stateItems", DbType.String, param.stateArray == null ? "0,1,2,3,4" : string.Join(",", param.stateArray));
            parameters.AddInParameter("@activityTypeItems", DbType.String, param.activityTypeArray == null ? "200,300,400" : string.Join(",", param.activityTypeArray));
            parameters.AddInParameter("@merchantCode", DbType.String, param.merchantCode.ToString());
            parameters.AddInParameter("@lastEditTime", DbType.DateTime, param.lastEditTime == null ? new DateTime(2000, 1, 1, 0, 0, 0) : (DateTime)param.lastEditTime);
            parameters.AddInParameter("@GroupNo", DbType.Int32, param.GroupNo);
            parameters.AddInParameter("@HotelName", DbType.String, param.HotelName);


            object obj = CouponDB.ExecuteSqlStringAndReturnSingleField("CouponDB.MemberCouponActivityListCount", parameters);
            totalCount = Convert.ToInt32(obj);
            if (totalCount == 0)
            {
                return new List<CouponActivityEntity>();
            }
            parameters.AddInParameter("@start", DbType.Int32, param.PageIndex == 0 ? 0 : (param.PageIndex - 1) * param.PageSize);
            parameters.AddInParameter("@count", DbType.Int32, param.PageSize == 0 ? 10 : param.PageSize);
            List<CouponActivityEntity> itemList = CouponDB.ExecuteSqlString<CouponActivityEntity>("CouponDB.MemberCouponActivityList", parameters);
            return itemList != null && itemList.Count != 0 ? itemList : new List<CouponActivityEntity>();
        }

        internal static List<CouponActivityEntity> MemberRetailCouponActivityList(CouponActivityQueryParam param, out int totalCount)
        {
            var parameters = new DBParameterCollection();
            totalCount = 0;
            parameters.AddInParameter("@stateItems", DbType.String, param.stateArray == null ? "0,1,2,3,4" : string.Join(",", param.stateArray));
            //parameters.AddInParameter("@activityTypeItems", DbType.String, param.activityTypeArray == null ? "200,300,400" : string.Join(",", param.activityTypeArray));
            //parameters.AddInParameter("@merchantCode", DbType.String, param.merchantCode.ToString());
            parameters.AddInParameter("@lastEditTime", DbType.DateTime, param.lastEditTime == null ? new DateTime(2000, 1, 1, 0, 0, 0) : (DateTime)param.lastEditTime);
            //parameters.AddInParameter("@GroupNo", DbType.Int32, param.GroupNo);
            //parameters.AddInParameter("@HotelName", DbType.String, param.HotelName);
            parameters.AddInParameter("@Search", DbType.String, param.Search);


            object obj = CouponDB.ExecuteSqlStringAndReturnSingleField("CouponDB.MemberRetailCouponActivityListCount", parameters);
            totalCount = Convert.ToInt32(obj);
            if (totalCount == 0)
            {
                return new List<CouponActivityEntity>();
            }
            parameters.AddInParameter("@start", DbType.Int32, param.PageIndex == 0 ? 0 : (param.PageIndex - 1) * param.PageSize);
            parameters.AddInParameter("@count", DbType.Int32, param.PageSize == 0 ? 10 : param.PageSize);
            List<CouponActivityEntity> itemList = CouponDB.ExecuteSqlString<CouponActivityEntity>("CouponDB.MemberRetailCouponActivityList", parameters);
            return itemList != null && itemList.Count != 0 ? itemList : new List<CouponActivityEntity>();
        }



        internal static CouponActivityEntity GetToDayCouponActivity()
        {
            var parameters = new DBParameterCollection();
            List<CouponActivityEntity> itemList = CouponDB.ExecuteSqlString<CouponActivityEntity>("CouponDB.GetTodayCouponActivity", parameters);
            return itemList != null && itemList.Count != 0 ? itemList[0] : new CouponActivityEntity();
        }

        internal static CouponActivityEntity GetToDayCouponActivityAndSKU()
        {
            var parameters = new DBParameterCollection();
            List<CouponActivityEntity> itemList = CouponDB.ExecuteSqlString<CouponActivityEntity>("CouponDB.GetToDayCouponActivityAndSKU", parameters);
            return itemList != null && itemList.Count != 0 ? itemList[0] : new CouponActivityEntity();
        }

        internal static List<ExchangeCouponEntity> GetExchangeCouponEntityListByUser(long userId, int activityType)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@UserID", DbType.Int64, userId);
            parameters.AddInParameter("@ActivityType", DbType.Int32, activityType);
            List<ExchangeCouponEntity> itemList = CouponDB.ExecuteSqlString<ExchangeCouponEntity>("CouponDB.GetExchangeCouponEntityListByUser", parameters);
            return itemList != null && itemList.Count != 0 ? itemList : new List<ExchangeCouponEntity>();
        }

        internal static List<ExchangeCouponEntity> GetExchangCouponByCategoryId(long userId, int cParentId, int start, int count, int couponId)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@UserID", DbType.Int64, userId);
            parameters.AddInParameter("@ParentId", DbType.Int32, cParentId);
            parameters.AddInParameter("@start", DbType.Int32, start);
            parameters.AddInParameter("@count", DbType.Int32, count);
            parameters.AddInParameter("@ID", DbType.Int32, couponId);
            List<ExchangeCouponEntity> itemList = CouponDB.ExecuteSqlString<ExchangeCouponEntity>("CouponDB.GetExchangCouponByCategoryId", parameters);
            return itemList != null && itemList.Count != 0 ? itemList : new List<ExchangeCouponEntity>();
        }


        internal static List<ExchangeCouponEntity> GetExchangeCouponEntityListByIDList(List<int> IDList)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@IDList", DbType.String, string.Join(",", IDList));
            return CouponDB.ExecuteSqlString<ExchangeCouponEntity>("CouponDB.GetExchangeCouponEntityListByIDList", parameters);
        }

        public static List<ExchangeCouponEntity> GetExchangeCouponEntityListBySKUID(int skuid, int state)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@SKUID", DbType.Int32, skuid);
            parameters.AddInParameter("@State", DbType.Int32, state);
            return CouponDB.ExecuteSqlString<ExchangeCouponEntity>("CouponDB.GetExchangeCouponEntityListBySKUID", parameters);
        }

        public static List<ExchangeCouponEntity> GetExchangeCouponEntityListNoJoinBookBySKUID(int skuid, int state)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@SKUID", DbType.Int32, skuid);
            parameters.AddInParameter("@State", DbType.Int32, state);
            return CouponDB.ExecuteSqlString<ExchangeCouponEntity>("CouponDB.GetExchangeCouponEntityListNoJoinBookBySKUID", parameters);
        }

        
        internal static List<ExchangeCouponEntity> GetExchangeCouponEntityListByPhone(string phoneNum, int activityType)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@PhoneNum", DbType.String, phoneNum);
            parameters.AddInParameter("@ActivityType", DbType.Int32, activityType);
            List<ExchangeCouponEntity> itemList = CouponDB.ExecuteSqlString<ExchangeCouponEntity>("CouponDB.GetExchangeCouponEntityListByPhone", parameters);
            return itemList != null && itemList.Count != 0 ? itemList : new List<ExchangeCouponEntity>();
        }
        internal static List<ExchangeCouponEntity> GetExchangeCouponEntityListByGroupId(int groupid, int activityType)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@GroupId", DbType.Int32, groupid);
            parameters.AddInParameter("@ActivityType", DbType.Int32, activityType);
            List<ExchangeCouponEntity> itemList = CouponDB.ExecuteSqlString<ExchangeCouponEntity>("CouponDB.GetExchangeCouponEntityListByGroupId", parameters);
            return itemList != null && itemList.Count != 0 ? itemList : new List<ExchangeCouponEntity>();
        }

        internal static int CancelUnPayExchangeCouponOrderByActivityIDAndUserID(long userId, int activityID, int SKUID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@UserID", DbType.Int64, userId);
            parameters.AddInParameter("@ActivityID", DbType.Int32, activityID);
            parameters.AddInParameter("@SKUID", DbType.Int32, SKUID);

            //Object obj = CouponDB.ExecuteSprocAndReturnSingleField("SP_ExchangeCoupon_CancelUnPayOrderByActivityIDAndUserID", parameters);
            //return Convert.ToInt32(obj);
            int rn = CouponDB.ExecuteNonQuery("SP_ExchangeCoupon_CancelUnPayOrderByActivityIDAndUserID", parameters);
            return rn;
        }


        internal static int InsertExchangeCoupon(ExchangeCouponEntity ece)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@UserID", DbType.Int64, ece.UserID);
            parameters.AddInParameter("@CID", DbType.Int64, ece.CID);
            parameters.AddInParameter("@SKUID", DbType.Int32, ece.SKUID);
            parameters.AddInParameter("@PhoneNum", DbType.String, ece.PhoneNum != null ? ece.PhoneNum.Trim() : ece.PhoneNum);
            parameters.AddInParameter("@ExchangeNo", DbType.String, ece.ExchangeNo);
            parameters.AddInParameter("@ActivityID", DbType.Int32, ece.ActivityID);
            parameters.AddInParameter("@State", DbType.Int32, ece.State);
            parameters.AddInParameter("@Type", DbType.Int32, ece.Type);
            parameters.AddInParameter("@ActivityType", DbType.Int32, ece.ActivityType);
            parameters.AddInParameter("@PayID", DbType.Int32, ece.PayID);
            parameters.AddInParameter("@Price", DbType.Decimal, ece.Price);
            parameters.AddInParameter("@OriPrice", DbType.Int32, ece.OriPrice);
            parameters.AddInParameter("@Points", DbType.Decimal, ece.Points);
            parameters.AddInParameter("@SettlePrice", DbType.Decimal, ece.SettlePrice);
            parameters.AddInParameter("@PromotionID", DbType.Int32, ece.PromotionID);
            parameters.AddInParameter("@AddInfo", DbType.String, ece.AddInfo);
            parameters.AddInParameter("@GroupId", DbType.Int32, ece.GroupId);
            parameters.AddInParameter("@CustomerType", DbType.Int32, ece.CustomerType);
            parameters.AddInParameter("@InnerBuyGroup", DbType.Int32, ece.InnerBuyGroup);
            parameters.AddInParameter("@IsVIPInvatation", DbType.Boolean, ece.IsVIPInvatation);
            parameters.AddInParameter("@CashCouponID", DbType.Int32, ece.CashCouponID);
            parameters.AddInParameter("@CashCouponAmount", DbType.Decimal, ece.CashCouponAmount);
            parameters.AddInParameter("@VoucherIDs", DbType.String, ece.VoucherIDs);
            parameters.AddInParameter("@VoucherAmount", DbType.Decimal, ece.VoucherAmount);
            parameters.AddInParameter("@UserUseHousingFundAmount", DbType.Decimal, ece.UserUseHousingFundAmount);
            parameters.AddInParameter("@OperationState", DbType.Int32, ece.OperationState);
            parameters.AddInParameter("@TraveIDs", DbType.String, ece.TraveIDs != null ? ece.TraveIDs : "");
            parameters.AddInParameter("@FromWeixinUid", DbType.Int32, ece.FromWeixinUid);
            parameters.AddInParameter("@PhotoUrl", DbType.String, ece.PhotoUrl);
            parameters.AddInParameter("@SupplierID", DbType.Int32, ece.SupplierID);
            parameters.AddInParameter("@IsDistributed", DbType.Boolean, ece.IsDistributed);
            parameters.AddInParameter("@CouponOrderId", DbType.Int64, ece.CouponOrderId);

            Object obj = CouponDB.ExecuteSprocAndReturnSingleField("SP_ExchangeCoupon_Insert", parameters);
            return Convert.ToInt32(obj);
        }



        public static Int64 GetNextId(NextIdType tablename)
        {
            //if (tablename == null) throw new ArgumentNullException("tablename", "表名称不能为空");
            ISeqIdGenerator tableIdManagerDb = SeqIdGeneratorFactory.GetIdGenerator("CommDB_SELECT");
            string ss = tablename.ToString();
            return tableIdManagerDb.GetNextId(ss);
        }

        internal static int UpdateExchangeCoupon(ExchangeCouponEntity ece)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ID", DbType.Int32, ece.ID);
            parameters.AddInParameter("@ExchangeNo", DbType.String, ece.ExchangeNo);
            parameters.AddInParameter("@PayID", DbType.Int32, ece.PayID);
            parameters.AddInParameter("@ExchangeTargetID", DbType.Int64, ece.ExchangeTargetID);
            parameters.AddInParameter("@State", DbType.Int32, ece.State);
            parameters.AddInParameter("@CancelTime", DbType.DateTime, ece.CancelTime);
            parameters.AddInParameter("@ExchangeTime", DbType.DateTime, ece.ExchangeTime);
            parameters.AddInParameter("@Updator", DbType.Int64, ece.Updator);

            Object obj = CouponDB.ExecuteSprocAndReturnSingleField("SP_ExchangeCoupon_Update", parameters);
            return Convert.ToInt32(obj);
        }

        internal static int UpdateExchangeCouponForSMSAlert(int ID, int SMSAlertType)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ID", DbType.Int32, ID);
            parameters.AddInParameter("@SMSAlertType", DbType.Int32, SMSAlertType);

            Object obj = CouponDB.ExecuteSprocAndReturnSingleField("SP_ExchangeCoupon_UpdateForSMSAlert", parameters);
            return Convert.ToInt32(obj);
        }

        internal static int UpdateExchangeCouponForPhotoUrl(int ID, string PhotoUrl)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ID", DbType.Int32, ID);
            parameters.AddInParameter("@PhotoUrl", DbType.String, PhotoUrl);

            Object obj = CouponDB.ExecuteSprocAndReturnSingleField("SP_ExchangeCoupon_UpdateForPhotoUrl", parameters);
            return Convert.ToInt32(obj);
        }

        /// <summary>
        /// 活动ID获得当前所有的券列表
        /// 默认with(nolock)
        /// </summary>
        /// <param name="activityID"></param>
        /// <returns></returns>
        internal static List<ExchangeCouponEntity> GetExchangeCouponPageList(int activityID, int state, int pageSize, int pageIndex, out int total)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ActivityID", DbType.Int32, activityID);
            parameters.AddInParameter("@state", DbType.Int32, state);
            parameters.AddInParameter("@pageSize", DbType.Int32, pageSize);
            parameters.AddInParameter("@pageIndex", DbType.Int32, pageIndex);

            object obj = CouponDB.ExecuteSqlStringAndReturnSingleField("CouponDB.GetExchangeCouponPageListCount", parameters);
            total = Convert.ToInt32(obj);

            List<ExchangeCouponEntity> itemList = CouponDB.ExecuteSqlString<ExchangeCouponEntity>("CouponDB.GetExchangeCouponPageList", parameters);
            return itemList != null && itemList.Count != 0 ? itemList : new List<ExchangeCouponEntity>();
        }
        public static List<ExchangeCouponEntity> GetExchangeOrderList(int pageSize, int pageIndex, int followOperation, int skuid, string skuName, string phoneNum, string supplierName, string thirdorderid, string exchangeNo, out int totalCount)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@SKUID", DbType.Int32, skuid);
            parameters.AddInParameter("@FollowOperation", DbType.Int32, followOperation);
            parameters.AddInParameter("@SKUName", DbType.String, skuName);
            //parameters.AddInParameter("@state", DbType.Int32, state);
            //parameters.AddInParameter("@ExchangeCouponId", DbType.Int32, exchangeCouponId);
            parameters.AddInParameter("@PhoneNum", DbType.String, phoneNum);
            //parameters.AddInParameter("@Operation", DbType.String, operation);
            parameters.AddInParameter("@SupplierName", DbType.String, supplierName);
            parameters.AddInParameter("@pageSize", DbType.Int32, pageSize);
            parameters.AddInParameter("@pageIndex", DbType.Int32, pageIndex);
            parameters.AddInParameter("@ThirdPartyOrderID", DbType.String, thirdorderid);
            parameters.AddInParameter("@ExchangeNo", DbType.String, exchangeNo);
            object obj = CouponDB.ExecuteSqlStringAndReturnSingleField("CouponDB.GetExchangeOrderListCount", parameters);
            totalCount = Convert.ToInt32(obj);

            List<ExchangeCouponEntity> itemList = CouponDB.ExecuteSqlString<ExchangeCouponEntity>("CouponDB.GetExchangeOrderList", parameters);
            return itemList != null && itemList.Count != 0 ? itemList : new List<ExchangeCouponEntity>();
        }

        public static CouponOrderOperationEntity GetCouponOderOperationByCouponId(int couponId)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ExchangeID", DbType.Int32, couponId);

            List<CouponOrderOperationEntity> itemList = CouponDB.ExecuteSqlString<CouponOrderOperationEntity>("CouponDB.GetCouponOderOperationByCouponId", parameters);
            return (itemList != null && itemList.Count > 0) ? itemList.First() : new CouponOrderOperationEntity();
        }
        /// <summary>
        /// 活动ID获得当前所有的券列表
        /// 默认with(nolock)
        /// </summary>
        /// <param name="activityID"></param>
        /// <returns></returns>
        internal static List<ExchangeCouponEntity> GetExchangeCouponEntityListByActivity(int activityID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ActivityID", DbType.Int32, activityID);
            List<ExchangeCouponEntity> itemList = CouponDB.ExecuteSqlString<ExchangeCouponEntity>("CouponDB.GetExchangeCouponEntityListByActivity", parameters);
            return itemList != null && itemList.Count != 0 ? itemList : new List<ExchangeCouponEntity>();
        }


        /// <summary>
        /// 用户ID活动ID获得当前所有的券列表
        /// 默认with(nolock)
        /// </summary>
        /// <param name="activityID"></param>
        /// <returns></returns>
        internal static List<ExchangeCouponEntity> GetExchangeCouponEntityListByActivityIDAndUserID(int activityID, long userID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ActivityID", DbType.Int32, activityID);
            parameters.AddInParameter("@userID", DbType.Int64, userID);
            List<ExchangeCouponEntity> itemList = CouponDB.ExecuteSqlString<ExchangeCouponEntity>("CouponDB.GetExchangeCouponEntityListByActivityIDAndUserID", parameters);
            return itemList != null && itemList.Count != 0 ? itemList : new List<ExchangeCouponEntity>();
        }

        internal static int InsertCommOrders(CommOrderEntity coe)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@Name", DbType.String, coe.Name);
            parameters.AddInParameter("@PhoneNum", DbType.String, coe.PhoneNum);
            parameters.AddInParameter("@TypeID", DbType.Int32, coe.TypeID);
            parameters.AddInParameter("@CustomID", DbType.Int32, coe.CustomID);
            parameters.AddInParameter("@Price", DbType.Decimal, coe.Price);
            parameters.AddInParameter("@Points", DbType.Decimal, coe.Points);
            parameters.AddInParameter("@State", DbType.Int32, coe.State);

            Object obj = HotelDB.ExecuteSprocAndReturnSingleField("SP_CommOrders_Insert", parameters);
            return Convert.ToInt32(obj);
        }
        /// <summary>
        /// 这里添加方法注释
        /// </summary>
        internal static int AddCouponActivityBizRel(CouponActivityBizRelEntity couponactivitybizrel)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@CouponActivityId", couponactivitybizrel.CouponActivityId);
            parameters.Add("@BizID", couponactivitybizrel.BizID);
            parameters.Add("@BizType", couponactivitybizrel.BizType);
            int i = CouponDB.ExecuteNonQuery("SP_CouponActivityBizRel_Insert", parameters);
            return i;
        }
        internal static int AddOrUpdateCouponActivityBizRel(CouponActivityBizRelEntity couponactivitybizrel)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@CouponActivityId", couponactivitybizrel.CouponActivityId);
            parameters.Add("@BizID", couponactivitybizrel.BizID);
            parameters.Add("@BizType", couponactivitybizrel.BizType);
            int i = CouponDB.ExecuteNonQuery("SP_CouponActivityBizRel_InsertOrUpdate", parameters);
            return i;
        }
        internal static int AddCouponOrderOperation(CouponOrderOperationEntity couponorderoperation)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@ExchangeID", couponorderoperation.ExchangeID);
            parameters.Add("@OperationState", couponorderoperation.OperationState);
            parameters.Add("@Remark", couponorderoperation.Remark == null ? "" : couponorderoperation.Remark);
            parameters.Add("@Creator", couponorderoperation.Creator);
            parameters.Add("@CreateTime", couponorderoperation.CreateTime);
            int i = CouponDB.ExecuteNonQuery("SP_CouponOrderOperation_Insert", parameters);
            return i;
        }

        /// <summary>
        /// 这里添加方法注释
        /// </summary>
        internal static int UpdateCouponOrderOperation(CouponOrderOperationEntity couponorderoperation)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@ID", couponorderoperation.ID);
            parameters.Add("@ExchangeID", couponorderoperation.ExchangeID);
            parameters.Add("@OperationState", couponorderoperation.OperationState);
            parameters.Add("@Remark", couponorderoperation.Remark == null ? "" : couponorderoperation.Remark);
            parameters.Add("@Creator", couponorderoperation.Creator);
            parameters.Add("@CreateTime", couponorderoperation.CreateTime);
            int i = CouponDB.ExecuteNonQuery("SP_CouponOrderOperation_Update", parameters);
            return i;
        }


        internal static List<CouponActivityBizRelEntity> GetCouponActivityBizRelByCouponActivityIdOrBizID(int cid, int bizID, int bizType)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@CID", DbType.Int32, cid);
            parameters.AddInParameter("@BizID", DbType.Int32, bizID);
            parameters.AddInParameter("@BizType", DbType.Int32, bizType);
            List<CouponActivityBizRelEntity> itemList = HotelDB.ExecuteSqlString<CouponActivityBizRelEntity>("CouponDB.GetCouponActivityBizRelByCouponActivityIdOrBizID", parameters);
            return itemList;
        }
        /// <summary>
        /// 这里添加方法注释
        /// </summary>
        internal static int UpdateCouponActivityBizRel(CouponActivityBizRelEntity couponactivitybizrel)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@ID", couponactivitybizrel.ID);
            parameters.Add("@CouponActivityId", couponactivitybizrel.CouponActivityId);
            parameters.Add("@BizID", couponactivitybizrel.BizID);
            parameters.Add("@BizType", couponactivitybizrel.BizType);
            int i = CouponDB.ExecuteNonQuery("SP_CouponActivityBizRel_Update", parameters);
            return i;
        }


        internal static CommOrderEntity GetOneCommOrderEntity(int idx)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@IDX", DbType.Int32, idx);
            List<CommOrderEntity> itemList = HotelDB.ExecuteSqlString<CommOrderEntity>("CouponDB.GetOneCommOrderEntity", parameters);
            return itemList != null && itemList.Count != 0 ? itemList[0] : new CommOrderEntity();
        }

        internal static List<ExchangeCouponEntity> GetExchangeCouponEntityByPayID(int payID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@PayID", DbType.Int32, payID);
            List<ExchangeCouponEntity> itemList = CouponDB.ExecuteSqlString<ExchangeCouponEntity>("CouponDB.GetExchangeCouponEntityByPayID", parameters);
            return itemList != null && itemList.Count != 0 ? itemList : new List<ExchangeCouponEntity>();
        }
        internal static List<ExchangeCouponEntity> GetExchangeCouponEntityByCID(long CID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@CID", DbType.Int32, CID);
            List<ExchangeCouponEntity> itemList = CouponDB.ExecuteSqlString<ExchangeCouponEntity>("CouponDB.GetExchangeCouponEntityByCID", parameters);
            return itemList != null && itemList.Count != 0 ? itemList : new List<ExchangeCouponEntity>();
        }

        internal static ExchangeCouponEntity GetOneExchangeCouponByCouponNo(string couponNo)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@CouponNo", DbType.String, couponNo);
            List<ExchangeCouponEntity> itemList = CouponDB.ExecuteSqlString<ExchangeCouponEntity>("CouponDB.GetOneExchangeCouponByCouponNo", parameters);
            return itemList != null && itemList.Count != 0 ? itemList[0] : new ExchangeCouponEntity();
        }

        internal static List<ExchangeCouponEntity> GetUsedCouponByOrderID(long orderID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@orderID", DbType.Int64, orderID);
            List<ExchangeCouponEntity> itemList = CouponDB.ExecuteSqlString<ExchangeCouponEntity>("CouponDB.GetUsedCouponByOrderID", parameters);
            return itemList != null && itemList.Count != 0 ? itemList : new List<ExchangeCouponEntity>();
        }

        /// <summary>
        /// 单张房券退款
        /// </summary>
        /// <param name="couponID"></param>
        /// <param name="couponNo"></param>
        /// <returns></returns>
        //internal static int CouponRefund(int couponID,string couponNo)
        //{
        //    var parameters = new DBParameterCollection();
        //    parameters.AddInParameter("@ID", DbType.Int32, couponID);
        //    parameters.AddInParameter("@ExchangeNo", DbType.String, couponNo);
        //    object obj = CouponDB.ExecuteSprocAndReturnSingleField("SP_ExchangeCoupon_Refund", parameters);
        //    return Convert.ToInt32(obj);
        //}

        ///// <summary>
        ///// 过期房券自动退款
        ///// </summary>
        ///// <returns></returns>
        //internal static int ExpirationCouponAutoRefund()
        //{
        //    var parameters = new DBParameterCollection();
        //    object obj = CouponDB.ExecuteSprocAndReturnSingleField("SP_ExchangeCoupon_LoopUpdateState4Refund", parameters);
        //    return Convert.ToInt32(obj);
        //}

        internal static int ReturnOrderConsumedCoupon(long orderID, long updator)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@OrderID", DbType.Int64, orderID);
            parameters.AddInParameter("@Updator", DbType.Int64, updator);
            //parameters.AddInParameter("@ActivityType", DbType.Int32, activityType);
            object obj = CouponDB.ExecuteSprocAndReturnSingleField("SP_ExchangeCoupon_CancelOrder", parameters);
            return Convert.ToInt32(obj);
        }

        //ToDo 下次更新带参数的活动 未及时支付券
        internal static int UpdateCouponState4TimeOut(int activityID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@activityID", DbType.Int32, activityID);
            object obj = CouponDB.ExecuteSprocAndReturnSingleField("SP_ExchangeCoupon_LoopUpdateState", parameters);
            return Convert.ToInt32(obj);
        }

        public static int GetActivityLockedCount(int activityID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@activityID", DbType.Int32, activityID);
            object obj = CouponDB.ExecuteSqlStringAndReturnSingleField("CouponDB.GetActivityLockedCount", parameters);

            return Convert.ToInt32(obj);
        }
        public static List<WillCloseProductGroupEntity> GetWillCloseProductGroup(int GroupCount, int SKUID, int Hour)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@GroupCount", DbType.Int32, GroupCount);
            parameters.AddInParameter("@SKUID", DbType.Int32, SKUID);
            parameters.AddInParameter("@Hour", DbType.Int32, Hour);
            return CouponDB.ExecuteSqlString<WillCloseProductGroupEntity>("CouponDB.GetWillCloseProductGroup", parameters);
        }

        internal static int UpdateRemark(int RefundCouponIDX, string remark)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@RefundCouponIDX", DbType.Int32, RefundCouponIDX);
            parameters.AddInParameter("@Remark", DbType.String, remark);
            object obj = CouponDB.ExecuteSprocAndReturnSingleField("SP_RefundCoupons_Remark_Update", parameters);
            return Convert.ToInt32(obj);
        }

        internal static List<RefundCouponsEntity> GetRefundCouponsList(RefundCouponsQueryParam param, out int count)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@State", DbType.Int32, param.State);
            parameters.AddInParameter("@PayType", DbType.Int32, param.PayType);
            parameters.AddInParameter("@RefoundCouponIDX", DbType.Int32, param.RefoundCouponIDX);
            parameters.AddInParameter("@PhoneNum", DbType.String, param.PhoneNum == null ? "" : param.PhoneNum);

            object obj = CouponDB.ExecuteSqlStringAndReturnSingleField("CouponDB.GetRefundCouponsCount", parameters);
            count = Convert.ToInt32(obj);

            param.PageIndex = param.PageIndex == 0 ? 1 : param.PageIndex;
            int start = param.PageSize * (param.PageIndex - 1);
            parameters.AddInParameter("@start", DbType.Int32, start);
            parameters.AddInParameter("@end", DbType.Int32, param.PageSize + start);

            List<RefundCouponsEntity> itemList = CouponDB.ExecuteSqlString<RefundCouponsEntity>("CouponDB.GetRefundCouponsList", parameters);
            return itemList != null && itemList.Count != 0 ? itemList : new List<RefundCouponsEntity>();
        }

        internal static int UpdateRefundCoupon(RefundCouponsEntity rce)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@CouponID", DbType.Int32, rce.CouponID);
            parameters.AddInParameter("@State", DbType.Int32, rce.State);
            parameters.AddInParameter("@Updator", DbType.Int64, rce.Updator);

            Object obj = CouponDB.ExecuteSprocAndReturnSingleField("SP_RefundCoupons_Update", parameters);
            return Convert.ToInt32(obj);
        }

        internal static int UpdateRefundCouponForPartRefund(RefundCouponsEntity rce)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@RefundCouponIDX", DbType.Int32, rce.RefundCouponIDX);
            parameters.AddInParameter("@State", DbType.Int32, rce.State);
            parameters.AddInParameter("@Updator", DbType.Int64, rce.Updator);

            Object obj = CouponDB.ExecuteSprocAndReturnSingleField("SP_RefundCoupons_UpdateForPartRefund", parameters);
            return Convert.ToInt32(obj);
        }
        internal static int UpdateThirdPartyRefundCoupon(int couponid, int thirdstate, int couponRefundstate)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@BizID", DbType.Int32, couponid);
            parameters.AddInParameter("@ThirdPartyOrderRelState", DbType.Int32, thirdstate);
            parameters.AddInParameter("@couponRefundstate", DbType.Int32, couponRefundstate);

            Object obj = CouponDB.ExecuteSprocAndReturnSingleField("SP_CouponOrderState_Update", parameters);
            return Convert.ToInt32(obj);
        }
        internal static int Insert2Refund(RefundCouponsEntity rce)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@RefundCouponIDX", DbType.Int32, rce.RefundCouponIDX);
            parameters.AddInParameter("@Creator", DbType.Int64, rce.Creator);
            parameters.AddInParameter("@Updator", DbType.Int64, rce.Updator);
            parameters.AddInParameter("@State", DbType.Int32, rce.RefundState);
            Object obj = CouponDB.ExecuteSprocAndReturnSingleField("SP_RefundCoupons_Insert2Refund", parameters);
            return Convert.ToInt32(obj);
        }

        internal static int AddRefundCoupon(RefundCouponsEntity rce)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@CouponID", DbType.Int32, rce.CouponID);
            parameters.AddInParameter("@State", DbType.Int32, rce.State);
            parameters.AddInParameter("@Creator", DbType.Int64, rce.Creator);
            parameters.AddInParameter("@Updator", DbType.Int64, rce.Updator);
            parameters.AddInParameter("@Remark", DbType.String, rce.Remark);
            parameters.AddInParameter("@RefundPrice", DbType.Decimal, rce.Price);
            Object obj = CouponDB.ExecuteSprocAndReturnSingleField("SP_RefundCoupons_Insert", parameters);//只加入到待退列表 不更改兑换券状态；兑换券状态需要到后台审核通过后变化
            return Convert.ToInt32(obj);
        }
        internal static List<ExchangeCouponEntity> GetExchangeCouponListByUserIDSourceID(Int64 UserID, Int64 SourceID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@UserID", DbType.Int64, UserID);
            parameters.AddInParameter("@SourceID", DbType.Int64, SourceID);
            return CouponDB.ExecuteSqlString<ExchangeCouponEntity>("CouponDB.GetExchangeCouponListByUserIDSourceID", parameters);
        }


        internal static ExchangeCouponEntity GetOneExchangeCoupon(int couponID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@CouponID", DbType.Int32, couponID);
            List<ExchangeCouponEntity> itemList = CouponDB.ExecuteSqlString<ExchangeCouponEntity>("CouponDB.GetOneExchangeCoupon", parameters);
            return itemList != null && itemList.Count != 0 ? itemList[0] : new ExchangeCouponEntity();
        }

        /// <summary>
        /// 状态到期未使用需要退款且尚未添加到refundcoupons表
        /// </summary>
        /// <returns></returns>
        internal static List<ExchangeCouponEntity> GetWaitingRefundCouponList()
        {
            var parameters = new DBParameterCollection();
            List<ExchangeCouponEntity> itemList = CouponDB.ExecuteSqlString<ExchangeCouponEntity>("CouponDB.GetWaitingRefundCouponList", parameters);
            //RefundState等于0表示 尚未加入退券refundcoupons表中
            return itemList != null && itemList.Count != 0 ? itemList.FindAll(_ => _.RefundState == 0) : new List<ExchangeCouponEntity>();
        }

        /// <summary>
        /// 主要添加待退券进入退款列表 如果支付类型是支付宝 无需插入HotelDB Refund表
        /// </summary>
        /// <param name="rce"></param>
        /// <returns></returns>        
        internal static int CancelRefundCoupon(RefundCouponsEntity rce)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@RefundCouponIDX", DbType.Int32, rce.RefundCouponIDX);
            parameters.AddInParameter("@Updator", DbType.Int64, rce.Updator);

            //除了超时支付进来必须退的兑换券 其他兑换券只要确认通过都可取消
            Object obj = CouponDB.ExecuteSprocAndReturnSingleField("SP_RefundCoupons_Cancel", parameters);
            return Convert.ToInt32(obj);
        }

        /// <summary>
        /// 获取房券的兑换价格列表
        /// </summary>
        /// <param name="couponActivity"></param>
        /// <returns></returns>
        internal static IEnumerable<CouponRateEntity> GetCouponRateEntityList(int couponActivity)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@couponActivity", DbType.Int32, couponActivity);
            return CouponDB.ExecuteSqlString<CouponRateEntity>("CouponDB.GetCouponRateEntityList", parameters);
        }

        /// <summary>
        /// 更新房券 兑换价格列表
        /// </summary>
        /// <param name="cre"></param>
        /// <returns></returns>
        internal static int UpdateCouponRateEntity(CouponRateEntity cre)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ID", DbType.Int32, cre.ID);
            parameters.AddInParameter("@CouponActivity", DbType.Int32, cre.CouponActivity);
            parameters.AddInParameter("@DateType", DbType.Int32, cre.DateType);
            parameters.AddInParameter("@Date", DbType.DateTime, cre.Date);
            parameters.AddInParameter("@Price", DbType.Int32, cre.Price);
            parameters.AddInParameter("@VIPPrice", DbType.Int32, cre.VIPPrice);
            parameters.AddInParameter("@Creator", DbType.Int64, cre.Creator);
            parameters.AddInParameter("@Updator", DbType.Int64, cre.Updator);

            CouponDB.ExecuteSprocAndReturnSingleField("SP_CouponRate_InsertOrUpdate", parameters);
            return 0;
        }

        /// <summary>
        /// 删除房券价格
        /// </summary>
        /// <param name="cre"></param>
        /// <returns></returns>
        internal static int DeleteCouponRateEntity(CouponRateEntity cre)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ID", DbType.Int32, cre.ID);
            parameters.AddInParameter("@CouponActivity", DbType.Int32, cre.CouponActivity);

            CouponDB.ExecuteSprocAndReturnSingleField("SP_CouponRate_Delete", parameters);
            return 0;
        }

        internal static IEnumerable<ExchangeCouponEntity> GetExchangeCouponList(int activityID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@activityID", DbType.Int32, activityID);
            return CouponDB.ExecuteSqlString<ExchangeCouponEntity>("CouponDB.GetExchangeCouponList", parameters);
        }

        internal static List<ExchangeCouponEntity> GetExchangeCouponListByUserSKU(long userId, int skuid, int promotionId)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@UserID", DbType.Int64, userId);
            parameters.AddInParameter("@SKUID", DbType.Int32, skuid);
            parameters.AddInParameter("@PromotionID", DbType.Int32, promotionId);
            return CouponDB.ExecuteSqlString<ExchangeCouponEntity>("CouponDB.GetExchangeCouponListByUserSKU", parameters);
        }


        internal static List<ExchangeCouponEntity> GetExchangeCouponListByUserIDAndSKU(long userId, int skuid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@UserID", DbType.Int64, userId);
            parameters.AddInParameter("@SKUID", DbType.Int32, skuid);
            return CouponDB.ExecuteSqlString<ExchangeCouponEntity>("CouponDB.GetExchangeCouponListByUserIDAndSKU", parameters);
        }

        internal static int CopyCouponActivity(int activityId, long updator, CouponActivityMerchant merchantCode)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ID", DbType.Int32, activityId);
            parameters.AddInParameter("@Updator", DbType.Int64, updator);
            parameters.AddInParameter("@MerchantCode", DbType.String, merchantCode == CouponActivityMerchant.zmjd ? "" : merchantCode.ToString());

            parameters.AddOutParameter("@NewCouponID", DbType.Int32, 4);

            CouponDB.ExecuteSprocAndReturnSingleField("SP_CouponActivity_Copy", parameters);
            return 0;
        }

        internal static List<ExchangeCouponEntity> GetNeedSettlementBotaoCouponList()
        {
            var parameters = new DBParameterCollection();
            return CouponDB.ExecuteSqlString<ExchangeCouponEntity>("CouponDB.GetNeedSettlementBotaoCouponList", parameters);
        }

        internal static List<long> GetNeedCancelMemberUserList()
        {
            var parameters = new DBParameterCollection();
            return CouponDB.ExecuteSqlSingle<long>("CouponDB.GetNeedCancelMemberUserList", parameters);
        }

        internal static int IsVipNoPayReserveUser(string userid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@UserId", DbType.String, userid);

            object obj = CouponDB.ExecuteSqlStringAndReturnSingleField("CouponDB.IsVipNoPayReserveUser", parameters);
            try
            {
                if (obj == null || Convert.IsDBNull(obj))
                {
                    return 0;
                }
                return (int)obj;
            }
            catch
            {
                return Convert.ToInt32(obj.ToString());
            }
        }

        /// <summary>
        /// 获取指定userid、typeid=8的originCoupon数据
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public static OriginCoupon GetOriginCouponByUserIdForT8(long userid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@UserId", DbType.Int64, userid);

            var itemList = CouponDB.ExecuteSqlString<OriginCoupon>("CouponDB.GetOriginCouponByUserIdForT8", parameters);
            return itemList != null && itemList.Count != 0 ? itemList[0] : new OriginCoupon();
        }

        public static List<DistrictInfoForWxappEntity> GetRoomCouponDistrictInfoForWxapp(List<int> districtIds)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@districtIds", DbType.String, string.Join<int>(",", districtIds));

            return CouponDB.ExecuteSqlString<DistrictInfoForWxappEntity>("CouponDB.GetRoomCouponDistrictInfoForWxapp", dbParameterCollection);
        }
        public static List<DistrictInfoForWxappEntity> GetRoomCouponDistrictInfoForLatLngWxapp(List<int> districtIds, double lat = 0, double lng = 0, int geoScopeType = 3, bool inchina = true)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@districtIds", DbType.String, string.Join<int>(",", districtIds));
            dbParameterCollection.AddInParameter("@lat", DbType.Double, lat);
            dbParameterCollection.AddInParameter("@lng", DbType.Double, lng);
            dbParameterCollection.AddInParameter("@geoScopeType", DbType.Int32, geoScopeType);
            dbParameterCollection.AddInParameter("@InChina", DbType.Boolean, inchina);

            return CouponDB.ExecuteSqlString<DistrictInfoForWxappEntity>("CouponDB.GetRoomCouponDistrictInfoForLatLngWxapp", dbParameterCollection);
        }

        /// <summary>
        /// 这里添加方法注释
        /// </summary>
        public static int AddVoucherChannel(VoucherChannelEntity voucherchannel)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@DefineID", voucherchannel.DefineID);
            parameters.Add("@Name", voucherchannel.Name);
            parameters.Add("@Description", voucherchannel.Description);
            parameters.Add("@state", voucherchannel.State);
            parameters.Add("@StartDate", voucherchannel.StartDate);
            parameters.Add("@EndDate", voucherchannel.EndDate);
            parameters.Add("@ExpireDate", voucherchannel.ExpireDate);
            parameters.Add("@Creator", voucherchannel.Creator);
            parameters.Add("@CreatDate", voucherchannel.CreatDate);
            parameters.Add("@CountNum", voucherchannel.CountNum);
            parameters.Add("@Code", voucherchannel.Code);

            int i = CouponDB.ExecuteNonQuery("SP_VoucherChannel_Insert", parameters);
            return i;
        }

        /// <summary>
        /// 这里添加方法注释
        /// </summary>
        public static int UpdateVoucherChannel(VoucherChannelEntity voucherchannel)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@IDX", voucherchannel.IDX);
            parameters.Add("@DefineID", voucherchannel.DefineID);
            parameters.Add("@Name", voucherchannel.Name);
            parameters.Add("@Description", voucherchannel.Description);
            parameters.Add("@state", voucherchannel.State);
            parameters.Add("@StartDate", voucherchannel.StartDate);
            parameters.Add("@EndDate", voucherchannel.EndDate);
            parameters.Add("@ExpireDate", voucherchannel.ExpireDate);
            parameters.Add("@Creator", voucherchannel.Creator);
            parameters.Add("@CreatDate", voucherchannel.CreatDate);
            parameters.Add("@CountNum", voucherchannel.CountNum);
            parameters.Add("@Code", voucherchannel.Code);
            int i = CouponDB.ExecuteNonQuery("SP_VoucherChannel_Update", parameters);
            return i;
        }

        /// <summary>
        /// 这里添加方法注释
        /// </summary>
        public static int AddVoucherDefine(VoucherDefineEntity voucherdefine)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@Name", voucherdefine.Name);
            parameters.Add("@Brief", voucherdefine.Brief);
            parameters.Add("@Description", voucherdefine.Description);
            parameters.Add("@Discount", voucherdefine.Discount);
            parameters.Add("@Creator", voucherdefine.Creator);
            parameters.Add("@CreatDateTime", voucherdefine.CreatDateTime);
            parameters.Add("@state", voucherdefine.State);
            parameters.Add("@UpdateTime", voucherdefine.UpdateTime);
            int i = CouponDB.ExecuteNonQuery("SP_VoucherDefine_Insert", parameters);
            return i;
        }

        /// <summary>
        /// 这里添加方法注释
        /// </summary>
        public static int UpdateVoucherDefine(VoucherDefineEntity voucherdefine)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@IDX", voucherdefine.IDX);
            parameters.Add("@Name", voucherdefine.Name);
            parameters.Add("@Brief", voucherdefine.Brief);
            parameters.Add("@Description", voucherdefine.Description);
            parameters.Add("@Discount", voucherdefine.Discount);
            parameters.Add("@Creator", voucherdefine.Creator);
            parameters.Add("@CreatDateTime", voucherdefine.CreatDateTime);
            parameters.Add("@state", voucherdefine.State);
            parameters.Add("@UpdateTime", voucherdefine.UpdateTime);
            int i = CouponDB.ExecuteNonQuery("SP_VoucherDefine_Update", parameters);
            return i;
        }


        /// <summary>
        /// 这里添加方法注释
        /// </summary>
        public static int AddVoucherItems(VoucherItemsEntity voucheritems)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@VoucherChannelID", voucheritems.VoucherChannelID);
            parameters.Add("@Creator", voucheritems.Creator);
            int i = CouponDB.ExecuteNonQuery("SP_VoucherItems_GenItems", parameters);
            return i;
        }

        /// <summary>
        /// 这里添加方法注释
        /// </summary>
        public static int UpdateVoucherItems(VoucherItemsEntity voucheritems)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@IDX", voucheritems.IDX);
            parameters.Add("@VoucherChannelID", voucheritems.VoucherChannelID);
            parameters.Add("@ExchangeNo", voucheritems.ExchangeNo);
            parameters.Add("@CreateTime", voucheritems.CreateTime);
            parameters.Add("@ExpireTime", voucheritems.ExpireTime);
            parameters.Add("@State", voucheritems.State);
            parameters.Add("@Userid", voucheritems.Userid);
            parameters.Add("@RelBizID", voucheritems.RelBizID);
            parameters.Add("@RelBizType", voucheritems.RelBizType);
            parameters.Add("@ExchangeTime", voucheritems.ExchangeTime);
            int i = CouponDB.ExecuteNonQuery("SP_VoucherItems_Update", parameters);
            return i;
        }

        public static List<VoucherDefineEntity> GetVoucherDefineList(int idx, string name)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@IDX", DbType.Int32, idx);
            dbParameterCollection.AddInParameter("@Name", DbType.String, name);
            return CouponDB.ExecuteSqlString<VoucherDefineEntity>("CouponDB.GetVoucherDefineList", dbParameterCollection);
        }

        public static List<VoucherChannelEntity> GetVoucherChanneList(int idx, string name, int defineid)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@IDX", DbType.Int32, idx);
            dbParameterCollection.AddInParameter("@Name", DbType.String, name);
            dbParameterCollection.AddInParameter("@DefineID", DbType.Int32, defineid);
            return CouponDB.ExecuteSqlString<VoucherChannelEntity>("CouponDB.GetVoucherChanneList", dbParameterCollection);
        }
        public static List<VoucherChannelEntity> GetVoucherChanneByCode(string code)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@Code", DbType.String, code);
            return CouponDB.ExecuteSqlString<VoucherChannelEntity>("CouponDB.GetVoucherChanneByCode", dbParameterCollection);
        }
        public static List<VoucherItemsEntity> GetVoucherItemByVoucherChannelid(int channelid)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@IDX", DbType.Int32, channelid);
            return CouponDB.ExecuteSqlString<VoucherItemsEntity>("CouponDB.GetVoucherItemByVoucherChannelid", dbParameterCollection);
        }



        /// <summary>
        /// 这里添加方法注释
        /// </summary>
        public static int AddUsedConsumerCouponInfo(UsedConsumerCouponInfoEntity usedconsumercouponinfo)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@ExchangeNo", usedconsumercouponinfo.ExchangeNo);
            parameters.Add("@SupplierId", usedconsumercouponinfo.SupplierId);
            parameters.Add("@OperatorId", usedconsumercouponinfo.OperatorId);
            parameters.Add("@CreateTime", usedconsumercouponinfo.CreateTime);
            parameters.Add("@IP", usedconsumercouponinfo.IP);
            int i = CouponDB.ExecuteNonQuery("SP_UsedConsumerCouponInfo_Insert", parameters);
            return i;
        }

        /// <summary>
        /// 这里添加方法注释
        /// </summary>
        public static int UpdateUsedConsumerCouponInfo(UsedConsumerCouponInfoEntity usedconsumercouponinfo)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@ID", usedconsumercouponinfo.ID);
            parameters.Add("@ExchangeNo", usedconsumercouponinfo.ExchangeNo);
            parameters.Add("@SupplierId", usedconsumercouponinfo.SupplierId);
            parameters.Add("@OperatorId", usedconsumercouponinfo.OperatorId);
            parameters.Add("@CreateTime", usedconsumercouponinfo.CreateTime);
            parameters.Add("@IP", usedconsumercouponinfo.IP);
            int i = CouponDB.ExecuteNonQuery("SP_UsedConsumerCouponInfo_Update", parameters);
            return i;
        }
        /// <summary>
        /// 这里添加方法注释
        /// </summary>
        public static int DelUsedConsumerCouponInfo(int id)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@ID", id);
            int i = CouponDB.ExecuteNonQuery("SP_UsedConsumerCouponInfo_Delete", parameters);
            return i;
        }

        public static int UpdateExchangeState(ExchangeCouponEntity param)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@ID", param.ID);
            parameters.Add("@ExchangeNo", param.ExchangeNo);
            parameters.Add("@State", param.State);
            parameters.Add("@OperationState", param.OperationState);
            parameters.Add("@Updator", param.Updator);
            int i = CouponDB.ExecuteNonQuery("SP_ExchangeCoupon_UpdateState", parameters);
            return i;
        }
        public static int UpdateOperationState(ExchangeCouponEntity param)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@ID", param.ID);
            parameters.Add("@Updator", param.Updator);
            parameters.Add("@OperationState", param.OperationState);
            int i = CouponDB.ExecuteNonQuery("SP_ExchangeCoupon_UpdateOperationState", parameters);
            return i;
        }

        public static int UpdateTravelIDs(ExchangeCouponEntity param)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@ID", param.ID);
            parameters.Add("@Updator", param.Updator);
            parameters.Add("@TraveIDs", param.TraveIDs);
            int i = CouponDB.ExecuteNonQuery("SP_ExchangeCoupon_UpdateTravelIDs", parameters);
            return i;
        }

        public static int UpdateOperationRemark(int ID, string Remark)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@CouponID", ID);
            parameters.Add("@OperationRemark", Remark);
            int i = CouponDB.ExecuteNonQuery("SP_ExchangeCoupon_OperationRemark_Update", parameters);
            return i;
        }

        public static List<UsedCouponProductEntity> GetUsedCouponProductBySupplierId(int supplierId, DateTime startTime, DateTime endTime, int start, int count)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@SupplierId", DbType.Int32, supplierId);
            dbParameterCollection.AddInParameter("@startTime", DbType.DateTime, startTime);
            dbParameterCollection.AddInParameter("@endTime", DbType.DateTime, endTime);
            dbParameterCollection.AddInParameter("@Start", DbType.Int32, start);
            dbParameterCollection.AddInParameter("@Count", DbType.Int32, count);
            return CouponDB.ExecuteSqlString<UsedCouponProductEntity>("CouponDB.GetUsedCouponProductBySupplierId", dbParameterCollection);
        }

        public static List<BookNoUsedExchangeCouponInfoEntity> GetBookNoUsedExchangeCouponBySupplierId(int supplierId, DateTime startTime, DateTime endTime, int state = 2, int start = 0, int count = 100000)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@SupplierId", DbType.Int32, supplierId);
            dbParameterCollection.AddInParameter("@startTime", DbType.DateTime, startTime);
            dbParameterCollection.AddInParameter("@endTime", DbType.DateTime, endTime);
            dbParameterCollection.AddInParameter("@State", DbType.Int32, state);
            
            //dbParameterCollection.AddInParameter("@Start", DbType.Int32, start);
            //dbParameterCollection.AddInParameter("@Count", DbType.Int32, count);
            return CouponDB.ExecuteSqlString<BookNoUsedExchangeCouponInfoEntity>("CouponDB.GetBookNoUsedExchangeCouponBySupplierId", dbParameterCollection);
        }

        public static UsedConsumerCouponInfoEntity GetUsedCouponProductByExchangeNo(string exchangeNo)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@ExchangeNo", DbType.String, exchangeNo);
            return CouponDB.ExecuteSqlString<UsedConsumerCouponInfoEntity>("CouponDB.GetUsedCouponProductByExchangeNo", dbParameterCollection).FirstOrDefault();
        }

        public static int GetUserCouponByCategoryParentId(long userId, int cParentId)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@UserID", DbType.Int64, userId);
            dbParameterCollection.AddInParameter("@ParentID", DbType.Int32, cParentId);
            object i = CouponDB.ExecuteSqlStringAndReturnSingleField("CouponDB.GetUserCouponByCategoryParentId", dbParameterCollection);
            return i != null ? Convert.ToInt32(i) : 0;
        }

        /// <summary>
        /// 获取指定SKUID的消费券产品列表
        /// </summary>
        /// <param name="skuids">String可包含多个SKUID，英文逗号间隔</param>
        /// <returns></returns>
        public static List<SKUCouponActivityEntity> GetSKUCouponActivityListBySKUIds(string skuids)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@SKUIds", DbType.String, skuids);
            return CouponDB.ExecuteSqlString<SKUCouponActivityEntity>("CouponDB.GetSKUCouponActivityListBySKUIds", dbParameterCollection);
        }

        public static List<CouponActivityWithSKUEntity> GetCouponActivityListBySKUIds(List<int> skuids)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@SKUIds", DbType.String, string.Join(",", skuids));
            return CouponDB.ExecuteSqlString<CouponActivityWithSKUEntity>("CouponDB.GetCouponActivityListBySKUIds", dbParameterCollection);
        }

        public static List<SKUCouponActivityEntity> GetSKUCouponActivityListByAlbumId(int albumId, int start, int count, int districtID, out int totalCount)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@AlbumId", DbType.Int32, albumId);
            dbParameterCollection.AddInParameter("@SaleEndDate", DbType.DateTime, System.DateTime.Now);
            dbParameterCollection.AddInParameter("@DistrictID", DbType.Int32, districtID);

            var obj = CouponDB.ExecuteSqlStringAndReturnSingleField("CouponDB.GetSKUCouponActivityListByAlbumIdCount", dbParameterCollection);
            totalCount = Convert.ToInt32(obj);
            dbParameterCollection.AddInParameter("@start", DbType.Int32, start);
            dbParameterCollection.AddInParameter("@count", DbType.Int32, count);
            return CouponDB.ExecuteSqlString<SKUCouponActivityEntity>("CouponDB.GetSKUCouponActivityListByAlbumId", dbParameterCollection);
        }

        public static List<SKUAlbumEntity> GetSKUAlbumEntityListByAlbumId(int albumId, int start, int count, out int totalCount)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@AlbumId", DbType.Int32, albumId);
            var obj = CouponDB.ExecuteSqlStringAndReturnSingleField("CouponDB.GetSKUAlbumEntityListByAlbumIdCount", dbParameterCollection);
            totalCount = Convert.ToInt32(obj);

            dbParameterCollection.AddInParameter("@start", DbType.Int32, start);
            dbParameterCollection.AddInParameter("@count", DbType.Int32, count);
            return CouponDB.ExecuteSqlString<SKUAlbumEntity>("CouponDB.GetSKUAlbumEntityListByAlbumId", dbParameterCollection);
        }

        public static ProductAlbumSumEntity GetProductAlbumSum(int albumId)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@AlbumId", DbType.Int32, albumId);
            return CouponDB.ExecuteSqlString<ProductAlbumSumEntity>("CouponDB.GetProductAlbumSum", dbParameterCollection).First();
        }

        public static List<SKUCouponActivityEntity> GetOldVIPSKUCouponActivityListByAlbumId(int albumId, int start, int count, int districtID, out int totalCount)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@AlbumId", DbType.Int32, albumId);
            dbParameterCollection.AddInParameter("@SaleEndDate", DbType.DateTime, System.DateTime.Now);
            dbParameterCollection.AddInParameter("@DistrictID", DbType.Int32, districtID);


            var obj = CouponDB.ExecuteSqlStringAndReturnSingleField("CouponDB.GetSKUCouponActivityListByAlbumIdCount", dbParameterCollection);
            totalCount = Convert.ToInt32(obj);
            dbParameterCollection.AddInParameter("@start", DbType.Int32, start);
            dbParameterCollection.AddInParameter("@count", DbType.Int32, count);
            return CouponDB.ExecuteSqlString<SKUCouponActivityEntity>("CouponDB.GetOldVIPSKUCouponActivityListByAlbumId", dbParameterCollection);
        }

        public static List<SKUCouponActivityEntity> SKUCouponActivityByCategory(int category = 14, int districtID = 2, double lat = 0, double lng = 0, int geoScopeType = 3, int start = 0, int count = 15, int sort = 0, int payType = 0, double locLat = 0, double locLng = 0)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@CategoryParentId", DbType.Int32, category);
            dbParameterCollection.AddInParameter("@DistrictID", DbType.Int32, districtID);
            dbParameterCollection.AddInParameter("@lat", DbType.Double, lat);
            dbParameterCollection.AddInParameter("@lng", DbType.Double, lng);
            dbParameterCollection.AddInParameter("@locLat", DbType.Double, locLat);
            dbParameterCollection.AddInParameter("@locLng", DbType.Double, locLng);
            dbParameterCollection.AddInParameter("@geoScopeType", DbType.Int32, geoScopeType);
            dbParameterCollection.AddInParameter("@Sort", DbType.Int32, sort);
            dbParameterCollection.AddInParameter("@PayType", DbType.Int32, payType);

            dbParameterCollection.AddInParameter("@Start", DbType.Int32, start);
            dbParameterCollection.AddInParameter("@Count", DbType.Int32, count);
            return CouponDB.ExecuteSqlString<SKUCouponActivityEntity>("CouponDB.SKUCouponActivityByCategory", dbParameterCollection);
        }

        public static int SKUCouponActivityByCategoryCount(int category = 14, int districtID = 2, double lat = 0, double lng = 0, int geoScopeType = 3, int payType = 0)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@CategoryParentId", DbType.Int32, category);
            dbParameterCollection.AddInParameter("@DistrictID", DbType.Int32, districtID);
            dbParameterCollection.AddInParameter("@lat", DbType.Double, lat);
            dbParameterCollection.AddInParameter("@lng", DbType.Double, lng);
            dbParameterCollection.AddInParameter("@geoScopeType", DbType.Int32, geoScopeType);
            dbParameterCollection.AddInParameter("@PayType", DbType.Int32, payType);
            var obj = CouponDB.ExecuteSqlStringAndReturnSingleField("CouponDB.SKUCouponActivityByCategoryCount", dbParameterCollection);
            return Convert.ToInt32(obj);
        }


        public static int UpdateExchangeCouponSettle(string ids, string code)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@ids", ids);
            parameters.Add("@codes", code);
            int i = CouponDB.ExecuteNonQuery("SP_ExchangeCoupon_UpdateHasSettle", parameters);
            return i;
        }
        public static List<ExchangeCouponEntity> GetExchangeListByNO(string codes, int type)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@codes", DbType.String, codes);
            if (type == 2)
            {
                return CouponDB.ExecuteSqlString<ExchangeCouponEntity>("CouponDB.GetExchangeListByNONotReady", dbParameterCollection);
            }
            else if (type == 3)
            {
                return CouponDB.ExecuteSqlString<ExchangeCouponEntity>("CouponDB.GetExchangeListByNOHadSettle", dbParameterCollection);
            }
            else
            {
                return CouponDB.ExecuteSqlString<ExchangeCouponEntity>("CouponDB.GetExchangeListByNO", dbParameterCollection);
            }
        }
        public static List<ExchangeSettleModel> GetExchangeCheckByNO(string codes)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@codes", DbType.String, codes);
            return CouponDB.ExecuteSqlString<ExchangeSettleModel>("CouponDB.GetExchangeCheckByNO", dbParameterCollection);
        }
        internal static List<ExchangeCouponForSettleEntity> GetExchangeCouponSettleList(int AID, int state, string startdate, string enddate, int pageSize, int pageIndex, out int total)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@state", state);
            parameters.Add("@startdate", startdate);
            parameters.Add("@enddate", enddate);
            parameters.Add("@pageSize", pageSize);
            parameters.Add("@pageIndex", pageIndex);
            parameters.Add("@aid", AID);
            object obj = CouponDB.ExecuteSqlStringAndReturnSingleField("CouponDB.GetExchangeCouponSettleListCount", parameters);
            total = Convert.ToInt32(obj);
            return CouponDB.ExecuteSqlString<ExchangeCouponForSettleEntity>("CouponDB.GetExchangeCouponSettleList", parameters);
        }

        /// <summary>
        /// 这里添加方法注释
        /// </summary>
        internal static int AddRedShare(RedShareEntity redshare)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@TotalAmount", redshare.TotalAmount);
            parameters.Add("@TotalCount", redshare.TotalCount);
            parameters.Add("@MaxAmount", redshare.MaxAmount);
            parameters.Add("@MinAmount", redshare.MinAmount);
            parameters.Add("@RedUrl", redshare.RedUrl);
            parameters.Add("@CreateTime", redshare.CreateTime);
            parameters.Add("@CreateUser", redshare.CreateUser);
            parameters.Add("@GUID", redshare.GUID);
            parameters.Add("@ShareTitle", redshare.ShareTitle);
            parameters.Add("@ShareDesc", redshare.ShareDesc);
            int i = CouponDB.ExecuteNonQuery("SP_RedShare_Insert", parameters);
            return i;
        }

        /// <summary>
        /// 这里添加方法注释
        /// </summary>
        internal static int UpdateRedShare(RedShareEntity redshare)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            //parameters.Add("@ID", redshare.ID);
            //parameters.Add("@TotalAmount", redshare.TotalAmount);
            //parameters.Add("@TotalCount", redshare.TotalCount);
            //parameters.Add("@MaxAmount", redshare.MaxAmount);
            //parameters.Add("@MinAmount", redshare.MinAmount);
            //parameters.Add("@RedUrl", redshare.RedUrl);
            //parameters.Add("@CreateTime", redshare.CreateTime);
            //parameters.Add("@CreateUser", redshare.CreateUser);
            parameters.Add("@GUID", redshare.GUID);
            parameters.Add("@ShareTitle", redshare.ShareTitle);
            parameters.Add("@ShareDesc", redshare.ShareDesc);
            int i = CouponDB.ExecuteNonQuery("SP_RedShare_Update", parameters);
            return i;
        }

        internal static List<RedShareEntity> GetRedShareList(int pageIndex, int pageSize, out int total)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@pageIndex", pageIndex);
            parameters.Add("@pageSize", pageSize);
            object obj = CouponDB.ExecuteSqlStringAndReturnSingleField("CouponDB.GetRedShareListCount", parameters);
            total = Convert.ToInt32(obj);
            return CouponDB.ExecuteSqlString<RedShareEntity>("CouponDB.GetRedShareList", parameters);
        }

        internal static List<RedShareEntity> GetRedShareEntityByGUID(string guid)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@GUID", guid);
            return CouponDB.ExecuteSqlString<RedShareEntity>("CouponDB.GetRedShareEntityByGUID", parameters);
        }



        /// <summary>
        /// 这里添加方法注释
        /// </summary>
        internal static int AddRetailProduct(RetailProductEntity retailproduct)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@No", retailproduct.No);
            parameters.Add("@Name", retailproduct.Name);
            parameters.Add("@BuyLimit", retailproduct.BuyLimit);
            parameters.Add("@Commission", retailproduct.Commission);
            parameters.Add("@RelBizType", retailproduct.RelBizType);
            parameters.Add("@RelBizID", retailproduct.RelBizID);
            parameters.Add("@Rank", retailproduct.Rank);
            parameters.Add("@State", retailproduct.State);
            parameters.Add("@Creator", retailproduct.Creator);
            parameters.Add("@CreateTime", retailproduct.CreateTime);
            parameters.Add("@UpdateTime", retailproduct.UpdateTime);
            parameters.Add("@ShareDesc", retailproduct.ShareDesc);
            int i = CouponDB.ExecuteNonQuery("SP_RetailProduct_Insert", parameters);
            return i;
        }

        /// <summary>
        /// 这里添加方法注释
        /// </summary>
        internal static int UpdateRetailProduct(RetailProductEntity retailproduct)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@ID", retailproduct.ID);
            parameters.Add("@No", retailproduct.No);
            parameters.Add("@Name", retailproduct.Name);
            parameters.Add("@BuyLimit", retailproduct.BuyLimit);
            parameters.Add("@Commission", retailproduct.Commission);
            parameters.Add("@RelBizType", retailproduct.RelBizType);
            parameters.Add("@RelBizID", retailproduct.RelBizID);
            parameters.Add("@Rank", retailproduct.Rank);
            parameters.Add("@State", retailproduct.State);
            parameters.Add("@Creator", retailproduct.Creator);
            parameters.Add("@CreateTime", retailproduct.CreateTime);
            parameters.Add("@UpdateTime", retailproduct.UpdateTime);
            parameters.Add("@ShareDesc", retailproduct.ShareDesc);
            int i = CouponDB.ExecuteNonQuery("SP_RetailProduct_Update", parameters);
            return i;
        }

        internal static List<RetailProductEntity> GetRetailProductById(int id)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@ID", id);
            return CouponDB.ExecuteSqlString<RetailProductEntity>("CouponDB.GetRetailProductById", parameters);
        }

        internal static List<CouponActivityRetailEntity> GetRetailProductList(int id, int relBizId, int state, int count, int start, out int totalCount)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@ID", id);
            parameters.Add("@RelBizId", relBizId);
            parameters.Add("@State", state);

            object obj = CouponDB.ExecuteSqlStringAndReturnSingleField("CouponDB.GetRetailProductListCount", parameters);
            totalCount = Convert.ToInt32(obj);

            parameters.Add("@Count", count);
            parameters.Add("@Start", start);
            return CouponDB.ExecuteSqlString<CouponActivityRetailEntity>("CouponDB.GetRetailProductList", parameters);
        }


        public static List<CouponActivityRetailEntity> GetSKURetailProductList(int count, int start, out int totalCount)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            object obj = CouponDB.ExecuteSqlStringAndReturnSingleField("CouponDB.GetSKURetailProductListCount", parameters);
            totalCount = Convert.ToInt32(obj);

            parameters.Add("@Count", count);
            parameters.Add("@Start", start);
            return CouponDB.ExecuteSqlString<CouponActivityRetailEntity>("CouponDB.GetSKURetailProductList", parameters);
        }

        internal static List<SKUCouponActivityEntity> GetSKUListByActivityId(int activityId)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@CouponActivityID", activityId);
            return CouponDB.ExecuteSqlString<SKUCouponActivityEntity>("CouponDB.GetSKUListByActivityId", parameters);
        }
        internal static int CopyRetail(int id, long updator)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@ID", id);
            parameters.Add("@Updator", updator);
            int i = CouponDB.ExecuteNonQuery("SP_CouponActivity_CopyForRetail", parameters);
            return i;
        }

        internal static int ProductAndCouponCopyRetail(int id, long updator)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@ID", id);
            parameters.Add("@Updator", updator);
            int i = CouponDB.ExecuteNonQuery("SP_CouponActivityAndSKU_CopyForRetail", parameters);
            return i;
        }

        internal static CouponActivityRetailDetailEntity GetRetailProductInfoByIDAndCID(int ID, long CID)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@ID", ID);
            parameters.Add("@CID", CID);
            return CouponDB.ExecuteSqlString<CouponActivityRetailDetailEntity>("CouponDB.GetRetailProductInfoByIDAndCID", parameters).First();
        }

        public static int AddRetailUrl(RetailUrlEntity retailurl)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@RetailID", retailurl.RetailID);
            parameters.Add("@CID", retailurl.CID);
            parameters.Add("@ResourceURL", retailurl.ResourceURL);
            parameters.Add("@ShortResourceURL", retailurl.ShortResourceURL);
            parameters.Add("@ProductUrl", retailurl.ProductUrl);
            parameters.Add("@ShortProductUrl", retailurl.ShortProductUrl);
            parameters.Add("@CreateTime", retailurl.CreateTime);
            parameters.Add("@SKUID", retailurl.SKUID);
            parameters.Add("@PID", retailurl.PID);
            int i = CouponDB.ExecuteNonQuery("SP_RetailUrl_Insert", parameters);
            return i;
        }

        public static int AddSupplierCoupon(SupplierCouponEntity suppliercoupon)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@CouponCode", suppliercoupon.CouponCode);
            parameters.Add("@State", suppliercoupon.State);
            parameters.Add("@SupplierId", suppliercoupon.SupplierId);
            int i = CouponDB.ExecuteNonQuery("SP_SupplierCoupon_Insert", parameters);
            return i;
        }

        public static int UpdateSupplierCoupon(SupplierCouponEntity suppliercoupon)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@ID", suppliercoupon.ID);
            parameters.Add("@CouponCode", suppliercoupon.CouponCode);
            parameters.Add("@State", suppliercoupon.State);
            parameters.Add("@SupplierId", suppliercoupon.SupplierId);
            int i = CouponDB.ExecuteNonQuery("SP_SupplierCoupon_Update", parameters);
            return i;
        }

        public static List<SupplierCouponEntity> GetTopCountSupplierCouponBySupplierID(int count, int supplierId, int state)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@Count", count);
            parameters.Add("@SupplierId", supplierId);
            parameters.Add("@State", state);
            return CouponDB.ExecuteSqlString<SupplierCouponEntity>("CouponDB.GetTopCountSupplierCouponBySupplierID", parameters);
        }

        public static List<SupplierCouponEntity> GetSupplierCouponInfo(int supplierId)
        {

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@SupplierId", supplierId);
            //parameters.Add("@State", state); 
            return CouponDB.ExecuteSqlString<SupplierCouponEntity>("CouponDB.GetSupplierCouponInfo", parameters);
        }
        public static List<CouponActivityRetailEntity> GetSearchProductList(SearchProductRequestEntity param)
        {
            //string SortWord = " rank asc";
            //switch (param.Sort)
            //{
            //    case (int)ProductSearchSort.DefaultSort:
            //        SortWord = " rank asc";
            //        break;
            //    case (int)ProductSearchSort.ManuSellNum:
            //        SortWord = " ManuSellNum desc";
            //        break;
            //    case (int)ProductSearchSort.PriceAsc:
            //        SortWord = " SKUPrice asc";
            //        break;
            //    case (int)ProductSearchSort.PriceDesc:
            //        SortWord = " SKUPrice desc";
            //        break;
            //    case (int)ProductSearchSort.RewardAsc:
            //        SortWord = " Commission asc";
            //        break;
            //    case (int)ProductSearchSort.RewardDesc:
            //        SortWord = " Commission desc";
            //        break;
            //    default:
            //        SortWord = " rank asc";
            //        break;

            //}

            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters.Add("@SearchWord", param.SearchWord.Trim());
            parameters.Add("@Sort", param.Sort);
            parameters.Add("@Screen", param.Screen.ProductType == null ? "200,600" : string.Join(",", param.Screen.ProductType));

            //var count = CouponDB.ExecuteSqlStringAndReturnSingleField("CouponDB.GetSearchProductListCount", parameters);
            //totalCount = count != null ? Convert.ToInt32(count) : 0;


            parameters.Add("@Start", param.Start);
            parameters.Add("@Count", param.Count);

            return CouponDB.ExecuteSqlString<CouponActivityRetailEntity>("CouponDB.GetSearchProductList", parameters);
        }


        public static List<CouponActivityRetailEntity> GetSearchProductListByCategory(SearchProductRequestEntity param)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters.Add("@SearchWord", param.SearchWord.Trim());
            parameters.Add("@Sort", param.Sort);
            parameters.Add("@Screen", param.Screen.ProductType == null ? "14" : string.Join(",", param.Screen.ProductType));

            parameters.Add("@Start", param.Start);
            parameters.Add("@Count", param.Count);

            return CouponDB.ExecuteSqlString<CouponActivityRetailEntity>("CouponDB.GetSearchProductListByCategory", parameters);
        }
        public static int GetSearchProductListByCategoryCount(SearchProductRequestEntity param)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters.Add("@SearchWord", param.SearchWord.Trim());
            parameters.Add("@Screen", param.Screen.ProductType == null ? "14" : string.Join(",", param.Screen.ProductType));

            var count = CouponDB.ExecuteSqlStringAndReturnSingleField("CouponDB.GetSearchProductListByCategoryCount", parameters);
            return count != null ? Convert.ToInt32(count) : 0;
        }


        public static int GetSearchProductListCount(SearchProductRequestEntity param)
        {

            string SortWord = " rank asc";
            switch (param.Sort)
            {
                case (int)ProductSearchSort.DefaultSort:
                    SortWord = " rank asc";
                    break;
                case (int)ProductSearchSort.ManuSellNum:
                    SortWord = " ManuSellNum desc";
                    break;
                case (int)ProductSearchSort.PriceAsc:
                    SortWord = " Price asc";
                    break;
                case (int)ProductSearchSort.PriceDesc:
                    SortWord = " Price desc";
                    break;
                case (int)ProductSearchSort.RewardAsc:
                    SortWord = " rawerd asc";
                    break;
                case (int)ProductSearchSort.RewardDesc:
                    SortWord = " rawerd asc";
                    break;
                default:
                    SortWord = " rank asc";
                    break;

            }

            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters.Add("@SearchWord", param.SearchWord.Trim());
            parameters.Add("@SortWord", SortWord);
            parameters.Add("@Screen", param.Screen.ProductType == null ? "200,600" : string.Join(",", param.Screen.ProductType));

            var count = CouponDB.ExecuteSqlStringAndReturnSingleField("CouponDB.GetSearchProductListCount", parameters);
            return count != null ? Convert.ToInt32(count) : 0;
        }


        public static int AddUserCouponConsumeLog(UserCouponConsumeLogEntity usercouponconsumelog)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@OrderType", usercouponconsumelog.OrderType);
            parameters.Add("@OrderID", usercouponconsumelog.OrderID);
            parameters.Add("@UserCouponItemID", usercouponconsumelog.UserCouponItemID);
            parameters.Add("@ConsumeAmount", usercouponconsumelog.ConsumeAmount);
            parameters.Add("@ConsumeType", usercouponconsumelog.ConsumeType);
            parameters.Add("@CreateTime", usercouponconsumelog.CreateTime);
            int i = CouponDB.ExecuteNonQuery("SP_UserCouponConsumeLog_Insert", parameters);
            return i;
        }

        public static int UpdateUserCouponConsumeLog(UserCouponConsumeLogEntity usercouponconsumelog)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@IDX", usercouponconsumelog.IDX);
            parameters.Add("@OrderType", usercouponconsumelog.OrderType);
            parameters.Add("@OrderID", usercouponconsumelog.OrderID);
            parameters.Add("@UserCouponItemID", usercouponconsumelog.UserCouponItemID);
            parameters.Add("@ConsumeAmount", usercouponconsumelog.ConsumeAmount);
            parameters.Add("@ConsumeType", usercouponconsumelog.ConsumeType);
            parameters.Add("@CreateTime", usercouponconsumelog.CreateTime);
            int i = CouponDB.ExecuteNonQuery("SP_UserCouponConsumeLog_Update", parameters);
            return i;
        }

        public static int AddUserCouponDefine(UserCouponDefineEntity usercoupondefine)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@Type", usercoupondefine.Type);
            parameters.Add("@Name", usercoupondefine.Name);
            parameters.Add("@Description", usercoupondefine.Description == null ? "" : usercoupondefine.Description);
            parameters.Add("@RequireAmount", usercoupondefine.RequireAmount);
            parameters.Add("@DiscountAmount", usercoupondefine.DiscountAmount);
            parameters.Add("@State", usercoupondefine.State);
            parameters.Add("@Creater", usercoupondefine.Creater);
            parameters.Add("@CreateTime", usercoupondefine.CreateTime);
            parameters.Add("@ValidUntilDate", usercoupondefine.ValidUntilDate);
            parameters.Add("@ExpirationDay", usercoupondefine.ExpirationDay);
            parameters.Add("@ExpirationType", usercoupondefine.ExpirationType);
            parameters.Add("@StartUseDate", usercoupondefine.StartUseDate);
            parameters.Add("@IsAccordingProduct", usercoupondefine.IsAccordingProduct);
            parameters.Add("@MarketingType", usercoupondefine.MarketingType);
            //parameters.Add("@Tips", usercoupondefine.Tips);
            int id = Convert.ToInt32( CouponDB.ExecuteSprocAndReturnSingleField("SP_UserCouponDefine_Insert", parameters));
            return id;
        }

        public static int UpdateUserCouponDefine(UserCouponDefineEntity usercoupondefine)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@IDX", usercoupondefine.IDX);
            parameters.Add("@Type", usercoupondefine.Type);
            parameters.Add("@Name", usercoupondefine.Name);
            parameters.Add("@Description", usercoupondefine.Description == null ? "" : usercoupondefine.Description);
            parameters.Add("@RequireAmount", usercoupondefine.RequireAmount);
            parameters.Add("@DiscountAmount", usercoupondefine.DiscountAmount);
            parameters.Add("@State", usercoupondefine.State);
            parameters.Add("@Creater", usercoupondefine.Creater);
            parameters.Add("@CreateTime", usercoupondefine.CreateTime);
            parameters.Add("@ValidUntilDate", usercoupondefine.ValidUntilDate);
            parameters.Add("@ExpirationDay", usercoupondefine.ExpirationDay);
            parameters.Add("@ExpirationType", usercoupondefine.ExpirationType);
            parameters.Add("@StartUseDate", usercoupondefine.StartUseDate);
            parameters.Add("@IsAccordingProduct", usercoupondefine.IsAccordingProduct);
            
            //parameters.Add("@Tips", usercoupondefine.Tips);
            int i = CouponDB.ExecuteNonQuery("SP_UserCouponDefine_Update", parameters);
            return i;
        }

        public static int AddUserCouponItem(UserCouponItemEntity usercouponitem)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@CouponDefineID", usercouponitem.CouponDefineID);
            parameters.Add("@UserID", usercouponitem.UserID);
            parameters.Add("@ExpiredDate", usercouponitem.ExpiredDate);
            parameters.Add("@RestAmount", usercouponitem.RestAmount);
            parameters.Add("@State", usercouponitem.State);
            parameters.Add("@CreateTime", usercouponitem.CreateTime);
            parameters.Add("@Creator", usercouponitem.Creator);
            parameters.Add("@StartDate", usercouponitem.StartDate);
            object i = CouponDB.ExecuteSprocAndReturnSingleField("SP_UserCouponItem_Insert", parameters);
            return Convert.ToInt32(i);
        }

        public static int UpdateUserCouponItem(UserCouponItemEntity usercouponitem)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@IDX", usercouponitem.IDX);
            parameters.Add("@CouponDefineID", usercouponitem.CouponDefineID);
            parameters.Add("@UserID", usercouponitem.UserID);
            parameters.Add("@ExpiredDate", usercouponitem.ExpiredDate);
            parameters.Add("@RestAmount", usercouponitem.RestAmount);
            parameters.Add("@State", usercouponitem.State);
            parameters.Add("@CreateTime", usercouponitem.CreateTime);
            parameters.Add("@Creator", usercouponitem.Creator);
            parameters.Add("@StartDate", usercouponitem.StartDate);
            int i = CouponDB.ExecuteNonQuery("SP_UserCouponItem_Update", parameters);
            return i;
        }

        /// <summary>
        /// 这里添加方法注释
        /// </summary>
        public static int AddUserCouponUseCondation(UserCouponUseCondationEntity usercouponusecondation)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@CouponDefineID", usercouponusecondation.CouponDefineID);
            parameters.Add("@CondationType", usercouponusecondation.CondationType);
            parameters.Add("@IntVal", usercouponusecondation.IntVal);
            parameters.Add("@Creater", usercouponusecondation.Creater);
            parameters.Add("@CreateTime", usercouponusecondation.CreateTime);
            int i = CouponDB.ExecuteNonQuery("SP_UserCouponUseCondation_Insert", parameters);
            return i;
        }

        /// <summary>
        /// 这里添加方法注释
        /// </summary>
        public static int UpdateUserCouponUseCondation(UserCouponUseCondationEntity usercouponusecondation)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@IDX", usercouponusecondation.IDX);
            parameters.Add("@CouponDefineID", usercouponusecondation.CouponDefineID);
            parameters.Add("@CondationType", usercouponusecondation.CondationType);
            parameters.Add("@IntVal", usercouponusecondation.IntVal);
            parameters.Add("@Creater", usercouponusecondation.Creater);
            parameters.Add("@CreateTime", usercouponusecondation.CreateTime);
            int i = CouponDB.ExecuteNonQuery("SP_UserCouponUseCondation_Update", parameters);
            return i;
        }

        public static int DeleteUserCouponUseCondation(int idx)
        {

            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@IDX", DbType.Int32, idx);
            CouponDB.ExecuteSprocAndReturnSingleField("SP_UserCouponUseCondation_Delete", parameters);
            return 0;
        }

        public static List<UserCouponUseCondationEntity> GetCouponCondationByCouponDefineId(int couponDefineId)
        {

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@CouponDefineID", couponDefineId);
            return CouponDB.ExecuteSqlString<UserCouponUseCondationEntity>("CouponDB.GetCouponCondationByCouponDefineId", parameters);
        }
        public static List<UserCouponUseCondationEntity> GetCouponCondationByIntVal(int intVal, int condationType)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@IntVal", intVal);
            parameters.Add("@CondationType", condationType);
            return CouponDB.ExecuteSqlString<UserCouponUseCondationEntity>("CouponDB.GetCouponCondationByIntVal", parameters);
        }

        public static List<UserCouponDefineEntity> GetCouponDefineByIntVals(string intVals, int condationType, long userId)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@IntVals", intVals);
            parameters.Add("@UserId", userId);
            parameters.Add("@CondationType", condationType);
            return CouponDB.ExecuteSqlString<UserCouponDefineEntity>("CouponDB.GetCouponDefineByIntVals", parameters);
        }

        public static List<UserCouponDefineEntity> GetUserCouponDefineListByType(int type)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@Type", type);
            return CouponDB.ExecuteSqlString<UserCouponDefineEntity>("CouponDB.GetUserCouponDefineListByType", parameters);
        }
        public static UserCouponDefineEntity GetUserCouponDefineByID(int id)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@IDX", id);
            var obj = CouponDB.ExecuteSqlString<UserCouponDefineEntity>("CouponDB.GetUserCouponDefineByID", parameters);
            return obj == null ? new UserCouponDefineEntity() : obj.First();
        }

        public static List<UserCouponDefineEntity> GetUserCouponDefineListByIds(string ids,long userId)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@IDXs", ids);
            parameters.Add("@UserID", userId);
            //var obj = CouponDB.ExecuteSqlString<UserCouponDefineEntity>("CouponDB.GetUserCouponDefineListByIds", parameters);
            return CouponDB.ExecuteSqlString<UserCouponDefineEntity>("CouponDB.GetUserCouponDefineListByIds", parameters);
        }

        public static int GetUserCouponItemByUserId(long userId, int couponDefineId)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@UserId", userId);
            parameters.Add("@CouponDefineId", couponDefineId);
            object obj = CouponDB.ExecuteSqlStringAndReturnSingleField("CouponDB.GetUserCouponItemByUserId", parameters);
            return obj == null ? 0 : Convert.ToInt32(obj);
        }

        public static List<UserCouponDefineEntity> GetCouponDefineByIntval(int intval, CondationType condationtype)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@IntVal", intval);
            parameters.Add("@CondationType", (int)condationtype);
            return CouponDB.ExecuteSqlString<UserCouponDefineEntity>("CouponDB.GetCouponDefineByIntval", parameters);
        }
        public static int GetUserCouponItemByUserIdAndCouponDefineId(long userId, int couponDefineId)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@UserId", userId);
            parameters.Add("@CouponDefineId", couponDefineId);
            object obj = CouponDB.ExecuteSqlStringAndReturnSingleField("CouponDB.GetUserCouponItemByUserIdAndCouponDefineId", parameters);
            return obj == null ? 0 : Convert.ToInt32(obj);
        }

        public static List<UserCouponItemInfoEntity> GetUserCouponInfoListByUserId(long userId, UserCouponState state)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@UserId", userId);
            parameters.Add("@State", (int)state);
            return CouponDB.ExecuteSqlString<UserCouponItemInfoEntity>("CouponDB.GetUserCouponInfoListByUserId", parameters);
        }
        public static List<UserCouponItemInfoEntity> GetNewVIPGiftUserCouponInfoListByUserId(long userId)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@UserId", userId);
            return CouponDB.ExecuteSqlString<UserCouponItemInfoEntity>("CouponDB.GetNewVIPGiftUserCouponInfoListByUserId", parameters);
        }

        public static bool UpdateUserCoupoinItemByIDs(List<Int32> IDs, Int32 CouponState)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@IDs", DbType.String, string.Join(",", IDs));
            parameters.AddInParameter("@CouponState", DbType.Int32, CouponState);

            CouponDB.ExecuteSp("SP4_UserCouponItem_UpdState", parameters);

            return true;
        }

        public static List<UserCouponItemInfoEntity> GetUserCouponInfoAllListByUserId(long userId)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@UserId", userId);
            return CouponDB.ExecuteSqlString<UserCouponItemInfoEntity>("CouponDB.GetUserCouponInfoAllListByUserId", parameters);
        }

        public static List<UserCouponItemInfoEntity> GetUserCouponInfoListByUserIdAndType(long userId, UserCouponState state, UserCouponType type)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@UserId", userId);
            parameters.Add("@State", (int)state);
            parameters.Add("@Type", (int)type);
            return CouponDB.ExecuteSqlString<UserCouponItemInfoEntity>("CouponDB.GetUserCouponInfoListByUserIdAndType", parameters);
        }

        public static List<UserCouponUseCondationEntity> GetUserCouponUserCondationList(int couponDefineId)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@CouponDefineID", couponDefineId);
            return CouponDB.ExecuteSqlString<UserCouponUseCondationEntity>("CouponDB.GetUserCouponUserCondationList", parameters);
        }

        public static List<UserCouponUseCondationEntity> GetUserCouponUserCondationListByUCDList(List<int> couponDefineIdList)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@CouponDefineIDList", DbType.String, string.Join(",", couponDefineIdList)); 
            return CouponDB.ExecuteSqlString<UserCouponUseCondationEntity>("CouponDB.GetUserCouponUserCondationListByUCDList", parameters);
        }
        public static List<UserCouponUseCondationEntity> GetUserCouponUserCondationListByTypeAndSourceID( CondationType type , int SourceID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@condationType ", DbType.Int32, (int)type);
            parameters.AddInParameter("@IntVal ", DbType.Int32, SourceID);
            return CouponDB.ExecuteSqlString<UserCouponUseCondationEntity>("CouponDB.GetUserCouponUserCondationListByTypeAndSourceID", parameters);
        }
        public static int GetUserCouponInfoCountByUserId(int userId, UserCouponState state)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@UserId", userId);
            parameters.Add("@State", (int)state);
            var obj = CouponDB.ExecuteSqlStringAndReturnSingleField("CouponDB.GetUserCouponInfoCountByUserId", parameters);
            return obj == null ? 0 : Convert.ToInt32(obj);
        }
        public static int GetUserCouponInfoCountByUserIdAndType(long userId, UserCouponState state, UserCouponType type)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@UserId", userId);
            parameters.Add("@State", (int)state);
            parameters.Add("@Type", (int)type);
            var obj = CouponDB.ExecuteSqlStringAndReturnSingleField("CouponDB.GetUserCouponInfoCountByUserIdAndType", parameters);
            return obj == null ? 0 : Convert.ToInt32(obj);
        }

        public static List<UserCouponConsumeLogEntity> GetUserCouponLogByCouponItemID(int idx)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@IDX", idx);
            return CouponDB.ExecuteSqlString<UserCouponConsumeLogEntity>("CouponDB.GetUserCouponLogByCouponItemID", parameters);
        }

        public static bool GetProductAlbumSKUBySKUIDAndAlbumId(int skuid, int albumid)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@SKUID", skuid);
            parameters.Add("@AlbumID", albumid);
            object obj = CouponDB.ExecuteSqlStringAndReturnSingleField("CouponDB.GetProductAlbumSKUBySKUIDAndAlbumId", parameters);
            return obj == null ? false : Convert.ToInt32(obj) > 0 ? true : false;
        }

        public static UserCouponItemInfoEntity GetUserCouponItemInfoByID(int id)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@ID", id);
            //parameters.Add("@Type", (int)cashType);
            var obj = CouponDB.ExecuteSqlString<UserCouponItemInfoEntity>("CouponDB.GetUserCouponItemInfoByID", parameters);
            return obj == null ? new UserCouponItemInfoEntity() : obj.First();
        }

        public static UserCouponItemEntity GetUserCouponItemByID(int id)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@ID", id);
            var obj = CouponDB.ExecuteSqlString<UserCouponItemEntity>("CouponDB.GetUserCouponItemByID", parameters);
            return obj == null ? new UserCouponItemEntity() : obj.First();
        }

        /// <summary>
        /// 这里添加方法注释
        /// </summary>
        public static int AddRedPool(RedPoolEntity redpool)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@RedPoolName", redpool.RedPoolName);
            parameters.Add("@MinOrderPrice", redpool.MinOrderPrice);
            parameters.Add("@MaxOrderPrice", redpool.MaxOrderPrice);
            parameters.Add("@Type", redpool.Type);
            parameters.Add("@Creator", redpool.Creator);
            parameters.Add("@CreateTime", redpool.CreateTime);
            object i = CouponDB.ExecuteSprocAndReturnSingleField("SP_RedPool_Insert", parameters);
            return Convert.ToInt32(i);
        }

        /// <summary>
        /// 这里添加方法注释
        /// </summary>
        public static int UpdateRedPool(RedPoolEntity redpool)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@ID", redpool.ID);
            parameters.Add("@RedPoolName", redpool.RedPoolName);
            parameters.Add("@MinOrderPrice", redpool.MinOrderPrice);
            parameters.Add("@MaxOrderPrice", redpool.MaxOrderPrice);
            parameters.Add("@Type", redpool.Type);
            parameters.Add("@Creator", redpool.Creator);
            parameters.Add("@CreateTime", redpool.CreateTime);
            int i = CouponDB.ExecuteNonQuery("SP_RedPool_Update", parameters);
            return i;
        }
        /// <summary>
        /// 这里添加方法注释
        /// </summary>
        public static int AddRedPoolDetail(RedPoolDetailEntity redpooldetail)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@RedPoolId", redpooldetail.RedPoolId);
            parameters.Add("@BizID", redpooldetail.BizID);
            parameters.Add("@BizType", redpooldetail.BizType);
            parameters.Add("@Count", redpooldetail.Count);
            parameters.Add("@Picture", redpooldetail.Picture);
            parameters.Add("@Creator", redpooldetail.Creator);
            parameters.Add("@CreateTime", redpooldetail.CreateTime);
            parameters.Add("@State", redpooldetail.State);
            int i = CouponDB.ExecuteNonQuery("SP_RedPoolDetail_Insert", parameters);
            return i;
        }

        /// <summary>
        /// 这里添加方法注释
        /// </summary>
        public static int UpdateRedPoolDetail(RedPoolDetailEntity redpooldetail)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@ID", redpooldetail.ID);
            parameters.Add("@RedPoolId", redpooldetail.RedPoolId);
            parameters.Add("@BizID", redpooldetail.BizID);
            parameters.Add("@BizType", redpooldetail.BizType);
            parameters.Add("@Count", redpooldetail.Count);
            parameters.Add("@Picture", redpooldetail.Picture);
            parameters.Add("@Creator", redpooldetail.Creator);
            parameters.Add("@CreateTime", redpooldetail.CreateTime);
            parameters.Add("@State", redpooldetail.State);
            int i = CouponDB.ExecuteNonQuery("SP_RedPoolDetail_Update", parameters);
            return i;
        }

        /// <summary>
        /// 这里添加方法注释
        /// </summary>
        public static int AddRedActivity(RedActivityEntity redactivity)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@GUID", redactivity.GUID);
            parameters.Add("@Name", redactivity.Name);
            parameters.Add("@BizID", redactivity.BizID);
            parameters.Add("@BizType", redactivity.BizType);
            parameters.Add("@RedPoolId", redactivity.RedPoolId);
            parameters.Add("@ExpireTime", redactivity.ExpireTime);
            parameters.Add("@Count", redactivity.Count);
            parameters.Add("@Creator", redactivity.Creator);
            parameters.Add("@CreateTime", redactivity.CreateTime);
            parameters.Add("@IsShowResult", redactivity.IsShowResult);
            object i = CouponDB.ExecuteSprocAndReturnSingleField("SP_RedActivity_Insert", parameters);
            return Convert.ToInt32(i);
        }

        /// <summary>
        /// 这里添加方法注释
        /// </summary>
        public static int UpdateRedActivity(RedActivityEntity redactivity)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@ID", redactivity.ID);
            parameters.Add("@GUID", redactivity.GUID);
            parameters.Add("@Name", redactivity.Name);
            parameters.Add("@BizID", redactivity.BizID);
            parameters.Add("@BizType", redactivity.BizType);
            parameters.Add("@RedPoolId", redactivity.RedPoolId);
            parameters.Add("@ExpireTime", redactivity.ExpireTime);
            parameters.Add("@Count", redactivity.Count);
            parameters.Add("@Creator", redactivity.Creator);
            parameters.Add("@CreateTime", redactivity.CreateTime);
            parameters.Add("@IsShowResult", redactivity.IsShowResult);
            int i = CouponDB.ExecuteNonQuery("SP_RedActivity_Update", parameters);
            return i;
        }

        /// <summary>
        /// 这里添加方法注释
        /// </summary>
        public static int AddRedRecord(RedRecordEntity redrecord)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@UserId", redrecord.UserId);
            parameters.Add("@PhoneNum", redrecord.PhoneNum);
            parameters.Add("@RedActivityID", redrecord.RedActivityID);
            parameters.Add("@RedPoolDetailID", redrecord.RedPoolDetailID);
            parameters.Add("@BizID", redrecord.BizID);
            parameters.Add("@BizType", redrecord.BizType);
            parameters.Add("@OpenId", redrecord.OpenId == null ? "" : redrecord.OpenId);
            parameters.Add("@State", redrecord.State);
            parameters.Add("@CreateTime", redrecord.CreateTime);
            object i = CouponDB.ExecuteSprocAndReturnSingleField("SP_RedRecord_Insert", parameters);
            return Convert.ToInt32(i);
        }

        /// <summary>
        /// 这里添加方法注释
        /// </summary>
        public static int UpdateRedRecord(RedRecordEntity redrecord)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@ID", redrecord.ID);
            parameters.Add("@UserId", redrecord.UserId);
            parameters.Add("@PhoneNum", redrecord.PhoneNum);
            parameters.Add("@RedActivityID", redrecord.RedActivityID);
            parameters.Add("@RedPoolDetailID", redrecord.RedPoolDetailID);
            parameters.Add("@BizID", redrecord.BizID);
            parameters.Add("@BizType", redrecord.BizType);
            parameters.Add("@OpenId", redrecord.OpenId == null ? "" : redrecord.OpenId);
            parameters.Add("@State", redrecord.State);
            parameters.Add("@CreateTime", redrecord.CreateTime);
            int i = CouponDB.ExecuteNonQuery("SP_RedRecord_Update", parameters);
            return i;
        }

        public static List<RedPoolEntity> GetRedPoolListByType(RedPoolType type)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@Type", (int)type);
            var obj = CouponDB.ExecuteSqlString<RedPoolEntity>("CouponDB.GetRedPoolListByType", parameters);
            return obj == null ? new List<RedPoolEntity>() : obj;
        }

        public static RedPoolEntity GetRedPoolByID(int id)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@ID", id);
            var obj = CouponDB.ExecuteSqlString<RedPoolEntity>("CouponDB.GetRedPoolByID", parameters);
            return (obj == null || obj.Count == 0) ? new RedPoolEntity() : obj.First();
        }

        public static List<RedPoolDetailEntity> GetRedDetailByPoolId(int poolId)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@RedPoolId", poolId);
            var obj = CouponDB.ExecuteSqlString<RedPoolDetailEntity>("CouponDB.GetRedDetailByPoolId", parameters);
            return (obj == null || obj.Count == 0) ? new List<RedPoolDetailEntity>() : obj;
        }

        public static RedPoolDetailEntity GetRedDetailBylId(int id)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@ID", id);
            var obj = CouponDB.ExecuteSqlString<RedPoolDetailEntity>("CouponDB.GetRedDetailBylId", parameters);
            return (obj != null && obj.Count > 0) ? obj.First() : new RedPoolDetailEntity();
        }

        public static List<RedActivityEntity> GetRedActivityListByType(RedActivityType type)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@BizType", (int)type);
            var obj = CouponDB.ExecuteSqlString<RedActivityEntity>("CouponDB.GetRedActivityListByType", parameters);
            return (obj == null || obj.Count == 0) ? new List<RedActivityEntity>() : obj;
        }

        public static RedActivityEntity GetRedActivityById(int id)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@ID", id);
            var obj = CouponDB.ExecuteSqlString<RedActivityEntity>("CouponDB.GetRedActivityById", parameters);
            return obj.Count == 0 ? new RedActivityEntity() : obj.First();
        }

        public static RedActivityEntity GetRedActivityByGUID(string guid)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@GUID", guid);
            var obj = CouponDB.ExecuteSqlString<RedActivityEntity>("CouponDB.GetRedActivityByGUID", parameters);
            return obj.Count == 0 ? new RedActivityEntity() : obj.First();
        }

        public static RedRecordEntity GetRedRecordByRedActivityIdAndPhone(int activityId, string phone)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@RedActivityID", activityId);
            parameters.Add("@PhoneNum", phone);
            var obj = CouponDB.ExecuteSqlString<RedRecordEntity>("CouponDB.GetRedRecordByRedActivityIdAndPhone", parameters);
            return obj.Count == 0 ? new RedRecordEntity() : obj.First();
        }

        public static List<RedRecordEntity> GetRedRecordByActivityId(int activityId, int count, int start, out int totalCount)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@RedActivityID", activityId);
            object objcount = CouponDB.ExecuteSqlStringAndReturnSingleField("CouponDB.GetRedRecordByActivityIdCount", parameters);
            totalCount = Convert.ToInt32(objcount);

            parameters.Add("@Count", count);
            parameters.Add("@Start", start);
            var obj = CouponDB.ExecuteSqlString<RedRecordEntity>("CouponDB.GetRedRecordByActivityId", parameters);
            return obj.Count == 0 ? new List<RedRecordEntity>() : obj;
        }

        public static List<RedPoolEntity> GetRedPoolByOrderPrice(decimal price)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@OrderPrice", price);
            var obj = CouponDB.ExecuteSqlString<RedPoolEntity>("CouponDB.GetRedPoolByOrderPrice", parameters);
            return obj.Count == 0 ? new List<RedPoolEntity>() : obj;
        }

        public static List<RedActivityEntity> GetRedActivityByBizIDAndBizType(long bizId, RedActivityType bizType)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@BizID", bizId);
            parameters.Add("@BizType", (int)bizType);
            return CouponDB.ExecuteSqlString<RedActivityEntity>("CouponDB.GetRedActivityByBizIDAndBizType", parameters);
            //return obj.Count == 0 ? new List<RedActivityEntity>() : obj;
        }

        public static RedRecordEntity GetRedRecordById(int id)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@ID", id);
            var obj = CouponDB.ExecuteSqlString<RedRecordEntity>("CouponDB.GetRedRecordById", parameters);
            return (obj != null && obj.Count > 0) ? obj.First() : new RedRecordEntity();
        }

        /// <summary>
        /// 这里添加方法注释
        /// </summary>
        internal static int UpdateExchangeSettlePrice(decimal SettlePrice, int ExchangeID, long curuserID, string OperationRemark, int supplierID =0)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@SettlePrice", SettlePrice);
            parameters.Add("@ID", ExchangeID);
            parameters.Add("@Creator", curuserID);
            parameters.Add("@OperationRemark", OperationRemark);
            parameters.Add("@SupplierID", supplierID);
            int i = CouponDB.ExecuteNonQuery("SP_CouponOrderSettlePrice_Update", parameters);
            return i;
        }

        public static string GetThirdCouponOrderID(int couponid, int type)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("Bizid", couponid);
            parameters.Add("@Type", type);
            var obj = CouponDB.ExecuteSqlStringAndReturnSingleField("CouponDB.GetThirdCouponOrderID", parameters);
            if (obj != null)
            {
                return obj.ToString();
            }
            else
            {
                return "";
            }
        }

        internal static int UpdateCouponActivityPicPath(int activityid, string picpath)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@ID", activityid);
            parameters.Add("@PicPath", picpath);
            int i = CouponDB.ExecuteNonQuery("SP_CouponActivityPicPath_Update", parameters);
            return i;
        }

        public static string GetCouponActivityPicBySPUID(int spuid)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("SPUID", spuid);
            var obj = CouponDB.ExecuteSqlStringAndReturnSingleField("CouponDB.GetCouponActivityPicBySPUID", parameters);
            if (obj != null)
            {
                return obj.ToString();
            }
            else
            {
                return "";
            }
        }
        public static List<ExchangeCouponEntity> GetExchangeOrderListBySupplierID(int supplierID)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@SupplierID", supplierID);
            return CouponDB.ExecuteSqlString<ExchangeCouponEntity>("CouponDB.GetExchangeOrderListBySupplierID", parameters);
        }
        public static ExchangeCouponEntity GetExchangCouponByCouponId(int couponId)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@CouponId", couponId);
            return CouponDB.ExecuteSqlString<ExchangeCouponEntity>("CouponDB.GetExchangCouponByCouponId", parameters).FirstOrDefault();
        }

        public static List<ExchangeCouponEntity> GetExchangCouponByCouponOrderID(long couponOrderID)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@CouponOrderID", couponOrderID);
            return CouponDB.ExecuteSqlString<ExchangeCouponEntity>("CouponDB.GetExchangCouponByCouponOrderID", parameters);
        }
        public static List<ExchangeCouponEntity> GetExpiredNotRefundExchangeCouponList()
        {
            var parameters = new DBParameterCollection();
            return CouponDB.ExecuteSqlString<ExchangeCouponEntity>("CouponDB.GetExpiredNotRefundExchangeCouponList", parameters);
        }
        public static int  UpdateExchangeCouponExpiredNotRefund()
        {
             var parameters = new DBParameterCollection();
             return CouponDB.ExecuteNonQuery("SP4_ExchangeCoupon_UpdateExpiredNotRefund", parameters);
        }
        public static int UpdateOperationRemarkByCouponId(int couponId, string remark)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@CouponID", couponId);
            parameters.Add("@OperationRemark", remark);
            int i = CouponDB.ExecuteNonQuery("SP_ExchangeCoupon_Remark_Update", parameters);
            return i;
        }
        public static List<SKUCouponActivityEntity> SKUCouponActivityByID(int ID = 0, int districtID = 2, double lat = 0, double lng = 0, int geoScopeType = 3, int start = 0, int count = 15, int sort = 0, int payType = 0, double locLat = 0, double locLng = 0)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@ID", DbType.Int32, ID);
            dbParameterCollection.AddInParameter("@DistrictID", DbType.Int32, districtID);
            dbParameterCollection.AddInParameter("@lat", DbType.Double, lat);
            dbParameterCollection.AddInParameter("@lng", DbType.Double, lng);
            dbParameterCollection.AddInParameter("@locLat", DbType.Double, locLat);
            dbParameterCollection.AddInParameter("@locLng", DbType.Double, locLng);
            dbParameterCollection.AddInParameter("@geoScopeType", DbType.Int32, geoScopeType);
            dbParameterCollection.AddInParameter("@Sort", DbType.Int32, sort);
            dbParameterCollection.AddInParameter("@PayType", DbType.Int32, payType);
            dbParameterCollection.AddInParameter("@Start", DbType.Int32, start);
            dbParameterCollection.AddInParameter("@Count", DbType.Int32, count);
            return CouponDB.ExecuteSqlString<SKUCouponActivityEntity>("CouponDB.SKUCouponActivityByID", dbParameterCollection);
        }
        public static int UpdateSupplierCouponState(SupplierCouponEntity suppliercoupon,int couponID)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@CouponCode", suppliercoupon.CouponCode);
            parameters.Add("@State", suppliercoupon.State);
            parameters.Add("@SupplierId", suppliercoupon.SupplierId);
            parameters.Add("@CouponID", couponID);
            int i = CouponDB.ExecuteNonQuery("SP_SupplierCoupon_UpdateState", parameters);
            return i;
        }
        public static UserCouponUseCondationEntity GetUserCouponUseCondationEntity(int idx)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@IDX", idx);
            var obj = CouponDB.ExecuteSqlString<UserCouponUseCondationEntity>("CouponDB.GetUserCouponUseCondationEntity", parameters);
            return (obj != null && obj.Count > 0) ? obj.FirstOrDefault() : new UserCouponUseCondationEntity();
        }
        public static List<UserCouponUseCondationEntity> GetCouponCondationByMarketingType(int intVal, int condationType, int marketingType)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@IntVal", intVal);
            parameters.Add("@CondationType", condationType);
            parameters.Add("@MarketingType", marketingType);
            return CouponDB.ExecuteSqlString<UserCouponUseCondationEntity>("CouponDB.GetCouponCondationByMarketingType", parameters);
        }
    }
}