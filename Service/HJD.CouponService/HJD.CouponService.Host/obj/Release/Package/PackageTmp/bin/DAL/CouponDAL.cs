using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HJD.Framework.Interface;
using HJD.CouponService.Contracts.Entity;
using System.Data;
using System.Text;

namespace HJD.CouponService.Impl.DAL
{
    public class CouponDAL
    {
        private static IDatabaseManager CouponDB = DatabaseManagerFactory.Create("CouponDB");
        private static IDatabaseManager HotelDB = DatabaseManagerFactory.Create("HotelDB");
        private static IDatabaseManager traceLogDB = DatabaseManagerFactory.Create("TraceLogDB");

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
        internal static bool  UpdOldVIPtoNewVIP(long userId)
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
        internal static bool  Del6MVIP(long userId)
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
            parameters.AddInParameter("@SKUID", DbType.Int32 , SKUID);
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
           return  CouponDB.ExecuteSqlString<CouponTypeDefineEntity>("CouponDB.GetCouponTypeDefine", parameters);
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

        internal static CouponTypeDefineEntity GetCouponTypeDefineByCode(CouponActivityType.CouponActivityCode code)
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
            parameters.AddInParameter("@IsValid", DbType.Boolean, sae.IsValid);
            parameters.AddInParameter("@GroupCount", DbType.Int32, sae.GroupCount);
            parameters.AddInParameter("@SerialNO", DbType.String, sae.SerialNO);
           // parameters.AddInParameter("@RelProductID", DbType.Int32, sae.RelProductID);
            parameters.AddInParameter("@RelPackageAlbumsID", DbType.Int32, sae.RelPackageAlbumsID);
            parameters.AddInParameter("@Tags", DbType.String, sae.Tags);
            parameters.AddInParameter("@MoreDetailUrl", DbType.String, sae.MoreDetailUrl);
            parameters.AddInParameter("@Properties", DbType.String, sae.Properties);

            Object obj = CouponDB.ExecuteSprocAndReturnSingleField("SP_CouponActivity_Insert", parameters);
            return Convert.ToInt32(obj);
        }

        internal static int UpdateCouponActivity(CouponActivityEntity sae)
        {
            CouponActivityEntity oldca = GetOneCouponActivityByIdAndType(sae.ID, sae.Type);
            if (sae.Type == 300 && oldca != null)
            {
                ObjectUpdateLog(sae.CurUserId, sae.ID, "", OpLogBizType.CouponActivity300, oldca, sae);
            }
            else if (sae.Type == 200 && oldca != null)
            {
                ObjectUpdateLog(sae.CurUserId, sae.ID, "", OpLogBizType.CouponActivity200, oldca, sae);
            }
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ID", DbType.Int32, sae.ID);
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
            parameters.AddInParameter("@IsValid", DbType.Boolean, sae.IsValid);
            parameters.AddInParameter("@GroupCount", DbType.Int32, sae.GroupCount);
            parameters.AddInParameter("@SerialNO", DbType.String, sae.SerialNO);
          //  parameters.AddInParameter("@RelProductID", DbType.Int32, sae.RelProductID);
            parameters.AddInParameter("@RelPackageAlbumsID", DbType.Int32, sae.RelPackageAlbumsID);
            parameters.AddInParameter("@Tags", DbType.String, sae.Tags);
            parameters.AddInParameter("@MoreDetailUrl", DbType.String, sae.MoreDetailUrl);
            parameters.AddInParameter("@Properties", DbType.String, sae.Properties);

            Object obj = CouponDB.ExecuteSprocAndReturnSingleField("SP_CouponActivity_Update", parameters);
            return Convert.ToInt32(obj);
        }

        public static int AddCouponActivitySKURel(int CouponActivityID, string RelSKUIDs)
        {
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

        internal static CouponActivityEntity GetOneCouponActivity(int id, bool isLock)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@id", DbType.Int32, id);

            List<CouponActivityEntity> obj = isLock ? CouponDB.ExecuteSqlString<CouponActivityEntity>("CouponDB.GetOneCouponActivityWithLock", parameters) : CouponDB.ExecuteSqlString<CouponActivityEntity>("CouponDB.GetOneCouponActivityWithNoLock", parameters);
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

        
        
        internal static CouponActivityEntity GetToDayCouponActivity()
        {
            var parameters = new DBParameterCollection();
            List<CouponActivityEntity> itemList = CouponDB.ExecuteSqlString<CouponActivityEntity>("CouponDB.GetTodayCouponActivity", parameters);
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

        internal static List<ExchangeCouponEntity> GetExchangeCouponEntityListByIDList(List<int> IDList)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@IDList", DbType.String, string.Join(",", IDList)); 
            return CouponDB.ExecuteSqlString<ExchangeCouponEntity>("CouponDB.GetExchangeCouponEntityListByIDList", parameters); 
        }

        internal static List<ExchangeCouponEntity> GetExchangeCouponEntityListByPhone(string phoneNum, int activityType)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@PhoneNum", DbType.String, phoneNum);
            parameters.AddInParameter("@ActivityType", DbType.Int32, activityType);
            List<ExchangeCouponEntity> itemList = CouponDB.ExecuteSqlString<ExchangeCouponEntity>("CouponDB.GetExchangeCouponEntityListByPhone", parameters);
            return itemList != null && itemList.Count != 0 ? itemList : new List<ExchangeCouponEntity>();
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
            parameters.AddInParameter("@Price", DbType.Int32, ece.Price);
            parameters.AddInParameter("@SettlePrice", DbType.Int32, ece.SettlePrice);
            parameters.AddInParameter("@PromotionID", DbType.Int32, ece.PromotionID);
            parameters.AddInParameter("@AddInfo", DbType.String, ece.AddInfo);

            Object obj = CouponDB.ExecuteSprocAndReturnSingleField("SP_ExchangeCoupon_Insert", parameters);
            return Convert.ToInt32(obj);
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
            parameters.AddInParameter("@Price", DbType.Int32, coe.Price);
            parameters.AddInParameter("@State", DbType.Int32, coe.State);

            Object obj = HotelDB.ExecuteSprocAndReturnSingleField("SP_CommOrders_Insert", parameters);
            return Convert.ToInt32(obj);
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

        internal static int UpdateRemark(int couponid, string remark)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@CouponID", DbType.Int32, couponid);
            parameters.AddInParameter("@Remark", DbType.String, remark);
            object obj = CouponDB.ExecuteSprocAndReturnSingleField("SP_RefundCoupons_Remark_Update", parameters);
            return Convert.ToInt32(obj);
        }

        internal static List<RefundCouponsEntity> GetRefundCouponsList(RefundCouponsQueryParam param,out int count)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@State", DbType.Int32, param.State);
            parameters.AddInParameter("@PayType", DbType.Int32, param.PayType);

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

        internal static int Insert2Refund(RefundCouponsEntity rce)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@CouponID", DbType.Int32, rce.CouponID);
            parameters.AddInParameter("@Creator", DbType.Int64, rce.Creator);
            parameters.AddInParameter("@Updator", DbType.Int64, rce.Updator);

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

            Object obj = CouponDB.ExecuteSprocAndReturnSingleField("SP_RefundCoupons_Insert", parameters);//只加入到待退列表 不更改兑换券状态；兑换券状态需要到后台审核通过后变化
            return Convert.ToInt32(obj);
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
            return itemList != null && itemList.Count != 0 ? itemList.FindAll(_=>_.RefundState == 0) : new List<ExchangeCouponEntity>();
        }

        /// <summary>
        /// 主要添加待退券进入退款列表 如果支付类型是支付宝 无需插入HotelDB Refund表
        /// </summary>
        /// <param name="rce"></param>
        /// <returns></returns>        
        internal static int CancelRefundCoupon(RefundCouponsEntity rce)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@CouponID", DbType.Int32, rce.CouponID);
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

        internal static int CopyCouponActivity(int activityId, long updator, CouponActivityType.CouponActivityMerchant merchantCode)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ID", DbType.Int32, activityId);
            parameters.AddInParameter("@Updator", DbType.Int64, updator);
            parameters.AddInParameter("@MerchantCode", DbType.String, merchantCode == CouponActivityType.CouponActivityMerchant.zmjd ? "" : merchantCode.ToString());

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

        public static int UpdateExchangeState(ExchangeCouponEntity param)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@ID", param.ID);
            parameters.Add("@ExchangeNo", param.ExchangeNo);
            parameters.Add("@State", param.State);
            parameters.Add("@Updator", param.Updator);
            int i = CouponDB.ExecuteNonQuery("SP_ExchangeCoupon_UpdateState", parameters);
            return i;
        }

        public static List<UsedCouponProductEntity> GetUsedCouponProductBySupplierId(int supplierId, DateTime startTime, DateTime endTime)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@SupplierId", DbType.Int32, supplierId);
            dbParameterCollection.AddInParameter("@startTime", DbType.DateTime, startTime);
            dbParameterCollection.AddInParameter("@endTime", DbType.DateTime, endTime);
            return CouponDB.ExecuteSqlString<UsedCouponProductEntity>("CouponDB.GetUsedCouponProductBySupplierId", dbParameterCollection);
        }

        public static UsedConsumerCouponInfoEntity GetUsedCouponProductByExchangeNo(string exchangeNo)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@ExchangeNo", DbType.String, exchangeNo);
            return CouponDB.ExecuteSqlString<UsedConsumerCouponInfoEntity>("CouponDB.GetUsedCouponProductByExchangeNo", dbParameterCollection).FirstOrDefault();
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
    }
}