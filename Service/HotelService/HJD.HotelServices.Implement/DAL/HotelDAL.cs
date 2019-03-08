using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using HJD.Framework.Interface;
using HJD.HotelServices.Contracts;
using System.Data;
using System.Data.Common;
using System.Collections;
using System.Configuration;
using HJD.HotelServices.Entity;

using HJD.Framework.WCF;
using HJD.AccountServices.Contracts;
using HJD.AccountServices.Entity;
using System.Xml.Serialization;
using System.IO;
using System.Text;
//新接口
using HJD.Framework.Log;
using HJD.HotelServices.Implement.Entity;
using HJD.HotelServices.Contracts.Comments;


namespace HJD.HotelServices
{
    internal class HotelDAL : RepositoryBase
    {
       

        private static int cfgHotelReviewCheckRange = int.Parse(ConfigurationManager.AppSettings["HotelReviewCheckRange"]);

        internal static List<HotelPackageRateItemEntity> GetHotelPackageRate(int HotelID, int pid = 0)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelID", DbType.Int32, HotelID);
            parameters.AddInParameter("@PID", DbType.Int32, pid);

            return HotelDB.ExecuteSqlString<HotelPackageRateItemEntity>("HotelDB.GetHotelPackageRate", parameters);
        }

        internal static List<int> GetOrderHotelIdsByUserId(long userId, int state)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@userid", DbType.Int64, userId);
            parameters.AddInParameter("@state", DbType.Int32, state);

            List<int> list = HotelDB.ExecuteSqlSingle<int>("HotelDB.GetOrderHotelIdsByUserId", parameters);
            if (list != null && list.Count != 0)
            {
                return list;
            }
            else
            {
                return new List<int>();
            }
        }

        internal static List<int> GetCommentHotelIdsByUserId(long userId)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@userid", DbType.Int64, userId);

            List<int> list = HotelDB.ExecuteSqlSingle<int>("HotelDB.GetCommentHotelIdsByUserId", parameters);
            if (list != null && list.Count != 0)
            {
                return list;
            }
            else
            {
                return new List<int>();
            }
        }

        /// <summary>
        /// 根据品鉴酒店ID只增加品鉴酒店申请数量 不改剩余量。如果品鉴酒店剩余量为0则报名失败,大于0则成功（大于0返回的是品鉴酒店ID）
        /// </summary>
        /// <param name="id">待评鉴酒店表的记录ID 非酒店ID</param>
        /// <returns></returns>
        internal static long ApplyInspectorHotel(long id)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@id", DbType.Int64, id);

            object obj = HotelBizDB.ExecuteSprocAndReturnSingleField("SP_InspectorHotel_Applicate", dbParameterCollection);

            return Convert.ToInt64(obj);
        }

        internal static long CheckInspectorRefHotel(long id, int isPass)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@id", DbType.Int64, id);
            dbParameterCollection.AddInParameter("@ispass", DbType.Int32, isPass);

            object obj = HotelBizDB.ExecuteSprocAndReturnSingleField("SP_InspectorHotel_CheckUpdate", dbParameterCollection);

            return Convert.ToInt64(obj);
        }


        internal static long CheckVoucher(int hvid, int isPass, long userid, string TelPhone, int id = 0)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@HVID", DbType.Int32, hvid);
            dbParameterCollection.AddInParameter("@ispass", DbType.Int32, isPass);
            dbParameterCollection.AddInParameter("@UserId", DbType.Int32, userid);
            dbParameterCollection.AddInParameter("@TelPhone", DbType.Int32, TelPhone);
            dbParameterCollection.AddInParameter("@ID", DbType.Int32, id);

            object obj = HotelBizDB.ExecuteSprocAndReturnSingleField("SP_Voucher_CheckUpdate", dbParameterCollection);

            return Convert.ToInt64(obj);
        }

        internal static int InsertOrUpdateInspectorHotel(InspectorHotel eh)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@id", DbType.Int64, eh.ID);
            dbParameterCollection.AddInParameter("@hotelid", DbType.Int32, eh.HotelID);
            dbParameterCollection.AddInParameter("@IsValid", DbType.Boolean, eh.IsValid);
            dbParameterCollection.AddInParameter("@IsExpired", DbType.Boolean, eh.IsExpired);
            dbParameterCollection.AddInParameter("@MaxCount", DbType.Int32, eh.MaxCount);
            dbParameterCollection.AddInParameter("@LeaveCount", DbType.Int32, eh.LeaveCount);
            dbParameterCollection.AddInParameter("@ExpiredDate", DbType.Date, eh.ExpiredDate);
            dbParameterCollection.AddInParameter("@CreateTime", DbType.Date, eh.CreateTime);
            dbParameterCollection.AddInParameter("@UpdateTime", DbType.Date, eh.UpdateTime);
            dbParameterCollection.AddInParameter("@Creator", DbType.Int64, eh.Creator);
            dbParameterCollection.AddInParameter("@Updator", DbType.Int64, eh.Updator);
            dbParameterCollection.AddInParameter("@Description", DbType.String, eh.Description);
            dbParameterCollection.AddInParameter("@Note", DbType.String, eh.Note);
            dbParameterCollection.AddInParameter("@RequiredPoint", DbType.Int32, eh.RequiredPoint);
            dbParameterCollection.AddInParameter("@Rank", DbType.Int32, eh.Rank);

            dbParameterCollection.AddInParameter("@PackageId", DbType.Int32, eh.PackageId);
            dbParameterCollection.AddInParameter("@UseDateType", DbType.Int32, eh.UseDateType);

            object obj = HotelBizDB.ExecuteSprocAndReturnSingleField("SP_InspectorHotel_InsertOrUpdate", dbParameterCollection);

            return Convert.ToInt32(obj);
        }

        internal static int InsertOrUpdateInspectorRefHotel(InspectorRefHotel eh)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@id", DbType.Int64, eh.ID);
            dbParameterCollection.AddInParameter("@Inspector", DbType.Int64, eh.Inspector);
            dbParameterCollection.AddInParameter("@InspectorHotel", DbType.Int64, eh.InspectorHotel);
            dbParameterCollection.AddInParameter("@HotelID", DbType.Int32, eh.HotelID);
            dbParameterCollection.AddInParameter("@State", DbType.Int32, eh.State);
            dbParameterCollection.AddInParameter("@CheckInDate", DbType.Date, eh.CheckInDate);
            dbParameterCollection.AddInParameter("@CheckOutDate", DbType.Date, eh.CheckOutDate);
            dbParameterCollection.AddInParameter("@CreateTime", DbType.Date, eh.CreateTime);
            dbParameterCollection.AddInParameter("@UpdateTime", DbType.Date, eh.UpdateTime);
            dbParameterCollection.AddInParameter("@CheckUser", DbType.Int64, eh.CheckUser);
            dbParameterCollection.AddInParameter("@CheckTime", DbType.Date, eh.CheckTime);
            dbParameterCollection.AddInParameter("@HVID", DbType.Int32, eh.HVID);
            dbParameterCollection.AddInParameter("@VID", DbType.Int32, eh.VID);

            object obj = HotelBizDB.ExecuteSprocAndReturnSingleField("SP_InspectorRefHotel_InsertOrUpdate", dbParameterCollection);

            return Convert.ToInt32(obj);
        }

        internal static int UpdateInspectorRefHotel(int state, int vid, int hvid)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@State", DbType.Int32, state);
            dbParameterCollection.AddInParameter("@HVID", DbType.Int32, hvid);
            dbParameterCollection.AddInParameter("@VID", DbType.Int32, vid);

            object obj = HotelBizDB.ExecuteSprocAndReturnSingleField("SP_InspectorRefHotel_InsertOrUpdate", dbParameterCollection);

            return Convert.ToInt32(obj);
        }

        /// <summary>
        /// 更新品鉴酒店涉及的点评 以及更新是否通知品鉴师写点评
        /// </summary>
        /// <param name="ehID"></param>
        /// <param name="commentID"></param>
        /// <param name="hasSendWriteComment"></param>
        /// <returns></returns>
        internal static int UpdateInspectorRefHotel4Comment(int ehID, int commentID, bool hasSendWriteComment = false)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@ID", DbType.Int32, ehID);
            dbParameterCollection.AddInParameter("@CommentID", DbType.Int32, commentID);
            dbParameterCollection.AddInParameter("@HasSendWriteComment", DbType.Boolean, hasSendWriteComment);
            object obj = HotelBizDB.ExecuteSprocAndReturnSingleField("SP_InspectorRefHotel_UpdateComment", dbParameterCollection);

            return Convert.ToInt32(obj);
        }

        internal static List<InspectorRefHotel> GetInspectorRefHotelList(long evaHotelID, long userID, int hvid)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@evahotelid", DbType.Int64, evaHotelID);
            dbParameterCollection.AddInParameter("@userid", DbType.Int64, userID);
            dbParameterCollection.AddInParameter("@HVID", DbType.Int64, hvid);

            List<InspectorRefHotel> list = HotelBizDB.ExecuteSqlString<InspectorRefHotel>("HotelBizDB.GetInspectorRefHotelList", dbParameterCollection);
            if (list != null && list.Count != 0)
            {
                return list;
            }
            else
            {
                return new List<InspectorRefHotel>();
            }
        }

        /// <summary>
        /// 这里添加方法注释
        /// </summary>
        public static int UpdateVoucher(VoucherEntity voucher)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@ID", voucher.ID);
            parameters.Add("@VoucherNum", voucher.VoucherNum == "" ? null : voucher.VoucherNum);
            parameters.Add("@HVID", voucher.HVID);
            parameters.Add("@State", voucher.State);
            parameters.Add("@ToUserName", voucher.ToUserName);
            parameters.Add("@OperationUserId", voucher.OperationUserId);
            parameters.Add("@CreatTime", voucher.CreatTime);
            parameters.Add("@UpdateTime", voucher.UpdateTime);
            parameters.Add("@ToVouchersType", voucher.ToVouchersType);
            parameters.Add("@TelPhone", voucher.TelPhone);
            parameters.Add("@Remark", voucher.Remark);
            parameters.Add("@ShippingAddress", voucher.ShippingAddress);
            parameters.Add("@UserId", voucher.UserId);
            int i = HotelBizDB.ExecuteNonQuery("SP_Voucher_Update", parameters);
            return i;
        }

        public static List<HotelVoucherEntity> GetUseHotelVoucher(out int count)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@EndDate", DbType.DateTime, System.DateTime.Now.ToShortDateString());
            var obj = HotelBizDB.ExecuteSqlStringAndReturnSingleField("HotelBizDB.GetUseHotelVoucherCount", parameters);
            count = Convert.ToInt32(obj);

            return HotelBizDB.ExecuteSqlString<HotelVoucherEntity>("HotelBizDB.GetUseHotelVoucher", parameters);
        }

        public static List<VoucherEntity> GetUseVoucherList(long userid, int hvid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@UserId", DbType.Int64, userid);
            parameters.AddInParameter("@HVID", DbType.Int32, hvid);
            return HotelBizDB.ExecuteSqlString<VoucherEntity>("HotelBizDB.GetUseVoucherList", parameters);
        }

        public static List<HotelVoucherEntity> GetHotelVoucherByID(int ID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ID", DbType.Int32, ID);
            return HotelBizDB.ExecuteSqlString<HotelVoucherEntity>("HotelBizDB.GetHotelVoucherById", parameters);
        }

        public static List<VRateEntity> GetVRateByStr(int HVID, DateTime date,int type)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HVID", DbType.Int32, HVID);
            parameters.AddInParameter("@DATE", DbType.DateTime, date);
            parameters.AddInParameter("@TYPE", DbType.Int32, type);
            return HotelBizDB.ExecuteSqlString<VRateEntity>("HotelBizDB.GetVRateByStr", parameters);
        }

        public static List<VRateEntity> GetVRateByHVID(int HVID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HVID", DbType.Int32, HVID);
            return HotelBizDB.ExecuteSqlString<VRateEntity>("HotelBizDB.GetVRateByHVID", parameters);
        }
        

        internal static List<InspectorRefHotel> GetInspectorRefHotelListByHVID(int hvid, long userID)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@HVID", DbType.Int32, hvid);
            dbParameterCollection.AddInParameter("@userid", DbType.Int64, userID);

            List<InspectorRefHotel> list = HotelBizDB.ExecuteSqlString<InspectorRefHotel>("HotelBizDB.GetInspectorRefHotelListByHVID", dbParameterCollection);
            if (list != null && list.Count != 0)
            {
                return list;
            }
            else
            {
                return new List<InspectorRefHotel>();
            }
        }

        internal static List<InspectorRefHotel> GetInspectorHotelOrderList(int OrderState, int lastID, int pageSize)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@OrderState", DbType.Int32, OrderState);
            dbParameterCollection.AddInParameter("@lastID", DbType.Int32, lastID);
            dbParameterCollection.AddInParameter("@pageSize", DbType.Int32, pageSize);

            List<InspectorRefHotel> list = HotelBizDB.ExecuteSqlString<InspectorRefHotel>("HotelBizDB.GetInspectorHotelOrderList", dbParameterCollection);
            if (list != null && list.Count != 0)
            {
                return list;
            }
            else
            {
                return new List<InspectorRefHotel>();
            }
        }

        internal static List<InspectorHotel> GetInspectorHotelList(InspectorHotelSearchParam param, out int count)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@IsExpired", DbType.Boolean, param.Filter == null ? null : param.Filter.IsExpired);
            dbParameterCollection.AddInParameter("@IsValid", DbType.Boolean, param.Filter == null ? null : param.Filter.IsValid);
            dbParameterCollection.AddInParameter("@MaxLimitTime", DbType.DateTime, param.Filter == null ? null : param.Filter.MaxLimitTime);

            object obj = HotelBizDB.ExecuteSqlStringAndReturnSingleField("HotelBizDB.GetInspectorHotelCount", dbParameterCollection);
            count = Convert.ToInt32(obj);

            param.PageIndex = param.PageIndex == 0 ? 1 : param.PageIndex;
            int start = param.PageSize * (param.PageIndex - 1);
            dbParameterCollection.AddInParameter("@start", DbType.Int32, start);
            dbParameterCollection.AddInParameter("@end", DbType.Int32, param.PageSize + start);

            bool isNeedOtherSort = param.Sort == null || param.Sort.Keys == null || param.Sort.Keys.Count == 0 ? false : true;
            List<InspectorHotel> list = null;
            if (!isNeedOtherSort)
            {
                list = HotelBizDB.ExecuteSqlString<InspectorHotel>("HotelBizDB.GetInspectorHotelList", dbParameterCollection);
            }
            else
            {
                list = HotelBizDB.ExecuteSqlString<InspectorHotel>("HotelBizDB.GetInspectorHotelListOrderByRank", dbParameterCollection);
            }
            if (list != null && list.Count != 0)
            {
                return list;
            }
            else
            {
                return new List<InspectorHotel>();
            }
        }

        internal static InspectorRefHotel GetInspectorRefHotelById(long id)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@id", DbType.Int64, id);

            return HotelBizDB.ExecuteSqlString<InspectorRefHotel>("HotelBizDB.GetInspectorRefHotelById", parameters).FirstOrDefault<InspectorRefHotel>();
        }

        internal static bool UpdateInspectorHotelOrder(InspectorRefHotel ihr)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@id", DbType.Int32, ihr.ID);

            parameters.AddInParameter("@CheckInDate", DbType.Date, ihr.CheckInDate);
            parameters.AddInParameter("@CheckOutDate", DbType.Date, ihr.CheckOutDate);
            parameters.AddInParameter("@OrderState", DbType.Int32, ihr.OrderState);
            parameters.AddInParameter("@FaxNo", DbType.String, ihr.FaxNo);
            parameters.AddInParameter("@HotelConfirmInfo", DbType.String, ihr.HotelConfirmInfo);
            parameters.AddInParameter("@NoticeForHotel", DbType.String, ihr.NoticeForHotel);
            parameters.AddInParameter("@Notice", DbType.String, ihr.Notice);

            HotelBizDB.ExecuteSp("SP_InspectorRefHotel_OrderUpdate", parameters);
            return true;
        }


        internal static InspectorHotel GetInspectorHotelById(long id)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@id", DbType.Int64, id);

            return HotelBizDB.ExecuteSqlString<InspectorHotel>("HotelBizDB.GetInspectorHotelById", parameters).FirstOrDefault<InspectorHotel>();
        }

        //internal static HotelVoucherEntity GetHotelVoucherById(int id)
        //{
        //    var parameters = new DBParameterCollection();
        //    parameters.AddInParameter("@id", DbType.Int32, id);

        //    return HotelBizDB.ExecuteSqlString<HotelVoucherEntity>("HotelBizDB.GetHotelVoucherById", parameters).FirstOrDefault<HotelVoucherEntity>();
        //}


        internal static List<SimplePackageEntity> GetSimplePackageInfo(string packageIDs)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@pids", DbType.String, packageIDs);

            return HotelDB.ExecuteSqlString<SimplePackageEntity>("HotelDB.GetSimplePackageInfo", parameters);
        }

        internal static List<int> GetPackageHotelList()
        {
            var parameters = new DBParameterCollection();

            return HotelDB.ExecuteSqlSingle<int>("HotelDB.GetPackageHotelList", parameters);
        }

        internal static List<AttractionCategoryRelEntity> GetAttractionCategoryRel()
        {
            var parameters = new DBParameterCollection();
            return HotelDB.ExecuteSqlString<AttractionCategoryRelEntity>("HotelDB.GetAttractionCategoryRel", parameters);
        }

        internal static PackageEntity GetSharePckageInfoByPrice(int hotelid, int price)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelID", DbType.Int32, hotelid);
            parameters.AddInParameter("@price", DbType.Int32, price);

            return HotelDB.ExecuteSqlString<PackageEntity>("HotelDB.GetSharePckageInfoByPrice", parameters).FirstOrDefault();
        }

        internal static List<RoomInfoEntity> GetRoomInfo(int hotelid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelID", DbType.Int32, hotelid);

            return HotelDB.ExecuteSqlString<RoomInfoEntity>("HotelDB.GetRoomInfo", parameters);
        }

        internal static List<HotelSightEntity> GetHotelSight(int hotelid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelID", DbType.Int32, hotelid);

            return HotelDB.ExecuteSqlString<HotelSightEntity>("HotelDB.GetHotelSight", parameters);
        }

        internal static List<HotelRestaurantEntity> GetHotelRestaurant(int hotelid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelID", DbType.Int32, hotelid);

            return HotelDB.ExecuteSqlString<HotelRestaurantEntity>("HotelDB.GetHotelRestaurant", parameters);
        }

        internal static List<HotelPackageOrderEntity> GetHotelPackageOrder(int hotelid, DateTime StartDate, DateTime EndDate)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelID", DbType.Int32, hotelid);
            parameters.AddInParameter("@StartDate", DbType.DateTime, StartDate);
            parameters.AddInParameter("@EndDate", DbType.DateTime, EndDate);

            return HotelDB.ExecuteSqlString<HotelPackageOrderEntity>("HotelDB.GetHotelPackageOrder", parameters);
        }

        internal static List<SpecialDealPackageEntity> GetSpecialDealPackages()
        {
            var parameters = new DBParameterCollection();
            return HotelDB.ExecuteSqlString<SpecialDealPackageEntity>("HotelDB.GetSpecialDealPackages", parameters);
        }

        internal static List<PackageEntity> GetPackage(int hotelid, int pid = 0, int terminalType = 1)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelID", DbType.Int32, hotelid);
            parameters.AddInParameter("@PID", DbType.Int32, pid);
            //parameters.AddInParameter("@IsSale", DbType.Boolean, isSale);

            string SQLName = "HotelDB.GetPackage";

            if (terminalType == 3) SQLName = "HotelDB.GetPackageForWebSite";  //如果是网站，则部分套餐不显示

            return HotelDB.ExecuteSqlString<PackageEntity>(SQLName, parameters);
        }

        internal static HotelContactEntity GetHotelContactByHotelID(int hotelid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelID", DbType.Int32, hotelid);

            List<HotelContactEntity> list = HotelDB.ExecuteSqlString<HotelContactEntity>("HotelDB.GetHotelContactByHotelID", parameters);
            if (list == null)
                return null;
            else
                return list.FirstOrDefault();
        }

        public static HotelContactPackageEntity GetHotelContactPackageByHotelIDAndPid(int hotelid,int pid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelID", DbType.Int32, hotelid);
            parameters.AddInParameter("@pid", DbType.Int32, pid);

            List<HotelContactPackageEntity> list = HotelDB.ExecuteSqlString<HotelContactPackageEntity>("HotelDB.GetHotelContactPackageByHotelIDAndPid", parameters);
            if (list == null)
                return null;
            else
                return list.FirstOrDefault();
        }

        internal static List<PackageEntity> GetHotelPackageByCode(int hotelid, string code, int pid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelID", DbType.Int32, hotelid);
            parameters.AddInParameter("@c", DbType.String, string.IsNullOrWhiteSpace(code) ? "" : code);
            parameters.AddInParameter("@pid", DbType.Int32, pid);

            return HotelDB.ExecuteSqlString<PackageEntity>("HotelDB.GetHotelPackageByCode", parameters);
        }

        internal static List<FeaturedCommentEntity> GetHotelFeaturedCommentInfo(int hotelid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelID", DbType.Int32, hotelid);

            return HotelDB.ExecuteSqlString<FeaturedCommentEntity>("HotelDB.GetHotelFeaturedCommentInfo", parameters);
        }
        internal static List<int> GetHotelFeaturedInfo(int hotelid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelID", DbType.Int32, hotelid);

            return HotelDB.ExecuteSqlSingle<int>("HotelDB.GetHotelFeaturedInfo", parameters);
        }

        

        internal static List<TopTownHotelRoomEntity> GetTopTownHotelRoom(int hotelid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelID", DbType.Int32, hotelid);

            return HotelDB.ExecuteSqlString<TopTownHotelRoomEntity>("HotelDB.GetTopTownHotelRoom", parameters);
        }
        internal static List<TopTownHotelPriceEntity> GetTopTownHotelRoomPrice(int hotelid, DateTime checkIn, DateTime checkOut)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelID", DbType.Int32, hotelid);
            parameters.AddInParameter("@checkIn", DbType.Date, checkIn.Date);
            parameters.AddInParameter("@checkOut", DbType.Date, checkOut.Date);

            return HotelDB.ExecuteSqlString<TopTownHotelPriceEntity>("HotelDB.GetTopTownHotelRoomPrice", parameters);
        }
        internal static List<JLHotelRoomEntity> GetJLHotelRoom(int hotelid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelID", DbType.Int32, hotelid);

            return HotelDB.ExecuteSqlString<JLHotelRoomEntity>("HotelDB.GetJLHotelRoom", parameters);
        }

        internal static List<JLHotelRoomPriceEntity> GetJLHotelRoomPrice(int hotelid, DateTime checkIn, DateTime checkOut)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelID", DbType.Int32, hotelid);
            parameters.AddInParameter("@checkIn", DbType.Date, checkIn.Date);
            parameters.AddInParameter("@checkOut", DbType.Date, checkOut.Date);

            return HotelDB.ExecuteSqlString<JLHotelRoomPriceEntity>("HotelDB.GetJLHotelRoomPrice", parameters);
        }

        internal static HotelRoomInfoEntity GetOnePRoomInfoByRoomID(int roomID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@RoomID", DbType.Int32, roomID);

            return HotelDB.ExecuteSqlString<HotelRoomInfoEntity>("HotelDB.GetHotelRoomInfoByRoomID", parameters).FirstOrDefault();
        }

        internal static List<PRoomInfoEntity> GetPRoomInfo(int hotelid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelID", DbType.Int32, hotelid); 

            return HotelDB.ExecuteSqlString<PRoomInfoEntity>("HotelDB.GetPRoomInfo", parameters);
        }

        internal static List<PRoomInfoEntity> GetPRoomInfoWithPID(int hotelid, int pid = 0)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelID", DbType.Int32, hotelid);
            parameters.AddInParameter("@PID", DbType.Int32, pid);

            return HotelDB.ExecuteSqlString<PRoomInfoEntity>("HotelDB.GetPRoomInfoWithPID", parameters);
        }

        internal static List<PRoomOptionsEntity> GetPRoomOptions(int hotelid, int pid = 0)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelID", DbType.Int32, hotelid);
            parameters.AddInParameter("@PID", DbType.Int32, pid);

            return HotelDB.ExecuteSqlString<PRoomOptionsEntity>("HotelDB.GetPRoomOptions", parameters);
        }

        internal static List<PItemEntity> GetPItem(int hotelid, int pid = 0)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelID", DbType.Int32, hotelid);
            parameters.AddInParameter("@PID", DbType.Int32, pid);

            return HotelDB.ExecuteSqlString<PItemEntity>("HotelDB.GetPItem", parameters);
        }

        internal static List<PRateEntity> GetPRate(int hotelid, int pid = 0)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelID", DbType.Int32, hotelid);
            parameters.AddInParameter("@PID", DbType.Int32, pid);

            return HotelDB.ExecuteSqlString<PRateEntity>("HotelDB.GetPRate", parameters);
        }

        internal static List<PCountEntity> GetPCount(int hotelid, int pid = 0)
        {
            var parameters = new DBParameterCollection();

            if (pid == 0)
            {
                parameters.AddInParameter("@HotelID", DbType.Int32, hotelid);
                return HotelDB.ExecuteSqlString<PCountEntity>("HotelDB.GetPCount", parameters);
            }
            else
            {
                parameters.AddInParameter("@PID", DbType.Int32, pid);
                return HotelDB.ExecuteSqlString<PCountEntity>("HotelDB.GetOnePackagePCount", parameters);
            }
        }

        internal static List<NoBedRoomEntity> GetHotelNoBedDateRoomInfo(int hotelid, DateTime startDate, DateTime endDate)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelID", DbType.Int32, hotelid);
            parameters.AddInParameter("@startDate", DbType.Date, startDate);
            parameters.AddInParameter("@endDate", DbType.Date, endDate);

            return HotelDB.ExecuteSqlString<NoBedRoomEntity>("HotelDB.GetHotelNoBedDateRoomInfo", parameters);
        }

        //internal static List<HotelRoomCountEntity> GetHotelRoomCountInfo(int hotelid, DateTime startDate, DateTime endDate)
        //{
        //    var parameters = new DBParameterCollection();
        //    parameters.AddInParameter("@HotelID", DbType.Int32, hotelid);
        //    parameters.AddInParameter("@startDate", DbType.Date, startDate);
        //    parameters.AddInParameter("@endDate", DbType.Date, endDate);

        //    return HotelDB.ExecuteSqlString<HotelRoomCountEntity>("HotelDB.GetHotelRoomCountInfo", parameters);
        //}

        internal static List<HotelPackageRoomBedCountEntity> GetHotelPackageRoomBedCount(int hotelid, DateTime startDate, DateTime endDate)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelID", DbType.Int32, hotelid);
            parameters.AddInParameter("@startDate", DbType.Date, startDate);
            parameters.AddInParameter("@endDate", DbType.Date, endDate);

            return HotelDB.ExecuteSqlString<HotelPackageRoomBedCountEntity>("HotelDB.GetHotelPackageRoomBedCount", parameters);
        }

        internal static List<HotelPackageRoomBedCountEntity> GetHotelPackRoomBedCount(int hotelid, DateTime startDate, DateTime endDate)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelID", DbType.Int32, hotelid);
            parameters.AddInParameter("@startDate", DbType.Date, startDate);
            parameters.AddInParameter("@endDate", DbType.Date, endDate);

            return HotelDB.ExecuteSqlString<HotelPackageRoomBedCountEntity>("HotelDB.GetHotelPackRoomBedCount", parameters);
        }

        internal static int GetHotelRoomTypeCount(int hotelid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelID", DbType.Int32, hotelid);

            return (int)HotelDB.ExecuteSqlStringAndReturnSingleField("HotelDB.GetHotelRoomTypeCount", parameters);
        }

        internal static List<RoomSouldItemEntity> GetHotelRoomSouldInfo(int hotelid, DateTime startDate, DateTime endDate)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelID", DbType.Int32, hotelid);
            parameters.AddInParameter("@startDate", DbType.Date, startDate);
            parameters.AddInParameter("@endDate", DbType.Date, endDate);

            return HotelDB.ExecuteSqlString<RoomSouldItemEntity>("HotelDB.GetHotelRoomSouldInfo", parameters);
        }

        public static HotelRoomInfoEntity GetOnePRoomInfoByPID(int pID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@PID", DbType.Int32, pID);

            return HotelDB.ExecuteSqlString<HotelRoomInfoEntity>("HotelDB.GetHotelRoomInfoByPID", parameters).FirstOrDefault();
        }

        internal static List<PackRoomBedStateEntity20> GetPackRoomBedState(int hotelid, DateTime StartDate, DateTime EndDate)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@hotelid", hotelid);
            parameters.Add("@StartDate", StartDate.Date);
            parameters.Add("@EndDate", EndDate);
            return HotelDB.ExecuteSqlString<PackRoomBedStateEntity20>("HotelDB.GetPackRoomBedState", parameters);
        }

        internal static List<Hotel3Entity> GetHotel3(int hotelid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelID", DbType.Int32, hotelid);

            return HotelDB.ExecuteSqlString<Hotel3Entity>("HotelDB.GetHotel3", parameters);
        }

        internal static List<InterestEntity> QueryInterest(int districtid, float glat, float glon, int distance = 300000)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@districtid", DbType.String, districtid);
            dbParameterCollection.AddInParameter("@glat", DbType.Double, glat);
            dbParameterCollection.AddInParameter("@glon", DbType.Double, glon);
            dbParameterCollection.AddInParameter("@distance", DbType.Int32, distance);

            return HotelDB.ExecuteSproc<InterestEntity>("sp_QueryInterest", dbParameterCollection);
        }

        internal static List<InterestEntity> QueryInterest4AD(int districtid, float glat, float glon, int distance = 300000)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@districtid", DbType.String, districtid);
            dbParameterCollection.AddInParameter("@glat", DbType.Double, glat);
            dbParameterCollection.AddInParameter("@glon", DbType.Double, glon);
            dbParameterCollection.AddInParameter("@distance", DbType.Int32, distance);

            return HotelDB.ExecuteSproc<InterestEntity>("sp_QueryInterest_AD", dbParameterCollection);
        }

        internal static List<InterestEntity> QueryInterest4ADSelected(int districtid, float glat, float glon, int distance = 300000)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@districtid", DbType.String, districtid);
            dbParameterCollection.AddInParameter("@glat", DbType.Double, glat);
            dbParameterCollection.AddInParameter("@glon", DbType.Double, glon);
            dbParameterCollection.AddInParameter("@distance", DbType.Int32, distance);

            return HotelDB.ExecuteSproc<InterestEntity>("sp_QueryInterest_ADSelected", dbParameterCollection);
        }

        internal static int QueryInterestHotelCount(int districtid, float glat, float glon, int distance = 300000)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@districtid", DbType.String, districtid);
            dbParameterCollection.AddInParameter("@glat", DbType.Double, glat);
            dbParameterCollection.AddInParameter("@glon", DbType.Double, glon);
            dbParameterCollection.AddInParameter("@distance", DbType.Int32, distance);

            return Convert.ToInt32(HotelDB.ExecuteSprocAndReturnSingleField("sp_QueryInterest_HotelCount",
                                                       dbParameterCollection));
        }

        internal static int QueryInterestHotelCountSelected(int districtid, float glat, float glon, int distance = 300000)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@districtid", DbType.String, districtid);
            dbParameterCollection.AddInParameter("@glat", DbType.Double, glat);
            dbParameterCollection.AddInParameter("@glon", DbType.Double, glon);
            dbParameterCollection.AddInParameter("@distance", DbType.Int32, distance);

            return Convert.ToInt32(HotelDB.ExecuteSprocAndReturnSingleField("sp_QueryInterest_HotelCountSelected", dbParameterCollection));
        }

        internal static List<CityEntity> GetZMJDCityData()
        {
            return HotelDB.ExecuteSqlString<CityEntity>("HotelDB.GetZMJDCityData");
        }

        internal static List<CityEntity> GetZMJDSelectedCityData()
        {
            return HotelDB.ExecuteSqlString<CityEntity>("HotelDB.GetZMJDSelectedCityData");
        }

        internal static List<CityEntity> GetZMJDAllCityData()
        {
            return HotelDB.ExecuteSqlString<CityEntity>("HotelDB.GetZMJDAllCityData");
        }

        internal static List<CityEntity> GetZMJDLoveCityData()
        {
            return HotelDB.ExecuteSqlString<CityEntity>("HotelDB.GetZMJDLoveCityData");
        }

        internal static List<CityEntity> GetZMJLoveDCityData()
        {
            return HotelDB.ExecuteSqlString<CityEntity>("HotelDB.GetZMJDLoveCityData");
        }

        internal static bool UpdateHotelInfo(HotelInfoEditEntity hi)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@OperatorUserID", DbType.Double, hi.OperatorUserID);
            dbParameterCollection.AddInParameter("@HotelID", DbType.Double, hi.HotelID);
            dbParameterCollection.AddInParameter("@p1", DbType.String, hi.p1);
            dbParameterCollection.AddInParameter("@OpType", DbType.Int32, hi.OpType);

            HotelDB.ExecuteSp("sp_hotelinfo_update", dbParameterCollection);

            return true;
        }

        internal static List<HotelReviewExEntity> GetHotelReviewByWriting(List<WritingIDGroup> Writinglist)
        {
            List<HotelReviewExEntity> cr = new List<HotelReviewExEntity>();
            List<HotelReviewExEntity> or = new List<HotelReviewExEntity>();
            List<HotelReviewExEntity> whr = new List<HotelReviewExEntity>();


            if (Writinglist.Where(w => w.type == 1).Count() > 0)
            {

                DBParameterCollection dbParameterCollection = new DBParameterCollection();
                dbParameterCollection.AddInParameter("@Writinglist", DbType.String, string.Join<int>(",", Writinglist.Where(w => w.type == 1).Select(w => w.writing)));

                cr = HotelDB.ExecuteSqlString<HotelReviewExEntity>("HotelDB.GetHotelReviewByWriting", dbParameterCollection);
            }

            if (Writinglist.Where(w => w.type == 2).Count() > 0)
            {

                DBParameterCollection p = new DBParameterCollection();
                p.AddInParameter("@Writinglist", DbType.String, string.Join<int>(",", Writinglist.Where(w => w.type == 2).Select(w => w.writing)));

                or = HotelDB.ExecuteSqlString<HotelReviewExEntity>("HotelDB.GetOTAHotelReviewByWriting", p);
            }

            //type:3 周末酒店自己用户写的点评
            if (Writinglist.Where(w => w.type == 3).Count() > 0)
            {

                DBParameterCollection p = new DBParameterCollection();
                p.AddInParameter("@Writinglist", DbType.String, string.Join<int>(",", Writinglist.Where(w => w.type == 3).Select(w => w.writing)));

                whr = HotelDB.ExecuteSqlString<HotelReviewExEntity>("HotelDB.GetWHotelReviewByWriting", p);
            }

            cr.AddRange(or);
            cr.AddRange(whr);

            return cr;
        }

        internal static List<HotelReviewExEntity> GetHotelReviewByCommentId(int commentId)
        {
            List<HotelReviewExEntity> whr = new List<HotelReviewExEntity>();
            //type:3 周末酒店自己用户写的点评
            DBParameterCollection p = new DBParameterCollection();
            p.AddInParameter("@commentId", DbType.Int32, commentId);

            whr = HotelDB.ExecuteSqlString<HotelReviewExEntity>("HotelDB.GetHotelReviewByCommentId", p);

            return whr;
        }


        internal static List<FeaturedReviewEntity> GetFeatruedReviewList(List<int> Writinglist, int featured)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@Writinglist", DbType.String, string.Join<int>(",", Writinglist));
            dbParameterCollection.AddInParameter("@Featured", DbType.Int32, featured);

            return HotelDB.ExecuteSqlString<FeaturedReviewEntity>("HotelDB.GetFeatruedReviewList",
                                                        dbParameterCollection);
        }

        internal static List<HotelInfoChannelEntity> GetHotelChannelByHotel(List<int> hotelID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelIdList", DbType.String, string.Join(",", hotelID));

            return HotelDB.ExecuteSqlString<HotelInfoChannelEntity>("HotelDB.GetHotelChannelByHotel", parameters);
        }

        /// <summary>
        /// 获取一个酒店的照片列表
        /// </summary>
        /// <param name="hotelID"></param>
        /// <returns></returns>
        internal static List<HotelPhotoEntity> GetHotelPhotos(int hotelID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelId", DbType.Int32, hotelID);

            return HotelDB.ExecuteSqlString<HotelPhotoEntity>("HotelDB.GetHotelPhotos", parameters);
        }

        internal static string GetCommDict()
        {
            //var parameters = new DBParameterCollection();
            //parameters.AddInParameter("@uid", DbType.AnsiString, uid);
            var retval = CommDB.ExecuteSqlStringAndReturnSingleField("HotelDB.GetCommDict");
            if (retval == null) return "";
            else return Convert.ToString(retval);
        }

        internal static List<int> GetHotelReviewIDListForCheckData(int i)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@writing", DbType.Int32, i);
            dbParameterCollection.AddInParameter("@HotelReviewCheckRange", DbType.Int32, cfgHotelReviewCheckRange);

            return HotelDB.ExecuteSqlSingle<int>("HotelDB.GetHotelReviewIDListForCheckData",
                                                       dbParameterCollection);
        }

        internal static List<HotelReviewIDAndTimeStamp> GetHotelReviewIDListByTimeStamp(long lastTimeStamp)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@LastTimeStamp", DbType.Int64, lastTimeStamp);
            dbParameterCollection.AddInParameter("@HotelReviewCheckRange", DbType.Int32, cfgHotelReviewCheckRange);


            return HotelDB.ExecuteSqlString<HotelReviewIDAndTimeStamp>("HotelDB.GetHotelReviewIDListByTimeStamp", dbParameterCollection);
        }

        /// <summary>
        /// 获取酒店列主题、特色或标签点评 
        /// </summary>
        /// <param name="hotelID"></param>
        /// <returns></returns>
        internal static List<HotelTFTReviewItemEntity> GetHotelTFTReview(int hotelID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelId", DbType.Int32, hotelID);
            return HotelDB.ExecuteSqlString<HotelTFTReviewItemEntity>("HotelDB.GetHotelTFTReview", parameters);
        }

        /// <summary>
        /// 获取酒店列主题相关标签 
        /// </summary>
        /// <param name="hotelID"></param>
        /// <returns></returns>
        internal static List<HotelInterestTagEntity> GetHotelInterestTag(int hotelID, int interestID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelId", DbType.Int32, hotelID);
            parameters.AddInParameter("@interestID", DbType.Int32, interestID);
            return HotelDB.ExecuteSqlString<HotelInterestTagEntity>("HotelDB.GetHotelInterestTag", parameters);
        }

        internal static List<HotelTagInfoEntity> GetHotelShowTags(int hotelID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelId", DbType.Int32, hotelID);
            return HotelDB.ExecuteSqlString<HotelTagInfoEntity>("HotelDB.GetHotelShowTags", parameters);
        }

        internal static List<HotelTagInfoEntity> GetHotelInterestTags(int hotelID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelId", DbType.Int32, hotelID);
            return HotelDB.ExecuteSqlString<HotelTagInfoEntity>("HotelDB.GetHotelInterestTags", parameters);
        }

        internal static List<Int32> GetHotelInterestIDs(int hotelID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelId", DbType.Int32, hotelID);
            return HotelDB.ExecuteSqlSingle<int>("HotelDB.GetHotelInterestIDs", parameters);
        }

        /// <summary>
        /// 获取酒店列主题、特色或标签点评 
        /// </summary>
        /// <param name="hotelID"></param>
        /// <returns></returns>
        internal static List<HotelTFTRelItemEntity> GetHotelTFTRel(int hotelID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelId", DbType.Int32, hotelID);
            return HotelDB.ExecuteSqlString<HotelTFTRelItemEntity>("HotelDB.GetHotelTFTRel", parameters);
        }

        /// <summary>
        /// 获取酒店列表中每个酒店前3个点评 
        /// </summary>
        /// <param name="hotelID"></param>
        /// <returns></returns>
        internal static List<HotelReviewTop3Entity> GetTop3CommentByHotel(List<int> hotelID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelIdList", DbType.String, string.Join(",", hotelID));

            return HotelDB.ExecuteSqlString<HotelReviewTop3Entity>("HotelDB.GetTop3HotelReviewWriting", parameters);
        }

        internal static List<HotelReviewSummaryEntity> GetHotelReviewNumAndRating(List<int> hotelID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelIdList", DbType.String, string.Join(",", hotelID));

            return HotelDB.ExecuteSqlString<HotelReviewSummaryEntity>("HotelDB.GetHotelReviewNumAndRating", parameters);
        }

        internal static List<int> GetValidPackageHotelID(DateTime startDate, DateTime endDate, string tag)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@startDate", DbType.Date, startDate.Date);

            parameters.AddInParameter("@endDate", DbType.Date, endDate.Date);

            parameters.AddInParameter("@tag", DbType.String, tag);

            return HotelDB.ExecuteSqlSingle<int>("HotelDB.GetValidPackageHotelID", parameters);
        }

        internal static List<CanSaleHotelInfoEntity> GetJLTourValidPackageHotel(DateTime startDate, DateTime endDate)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@startDate", DbType.Date, startDate.Date);

            parameters.AddInParameter("@endDate", DbType.Date, endDate.Date);

            return HotelDB.ExecuteSqlString<CanSaleHotelInfoEntity>("HotelDB.GetJLTourValidPackageHotel", parameters);
        }

        internal static List<int> GetNearAllHotelListBySight(int sight, string sortname)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@Sight", DbType.Int32, sight);

            parameters.AddInParameter("@Sort", DbType.AnsiString, sortname);

            return HotelDB.ExecuteSqlSingle<int>("HotelDB.GetNearHotelListBySight", parameters);
        }

        ///// <summary>
        ///// 附近酒店通过景点
        ///// </summary>
        ///// <param name="sightID">景点</param>
        ///// <returns></returns>
        //internal static List<HotelDistanceEntity> GetNearHotelBySight(int sightID)
        //{
        //    var parameters = new DBParameterCollection();
        //    parameters.AddInParameter("@Sight", DbType.Int32, sightID);

        //    return HotelDB.ExecuteSqlString<HotelDistanceEntity>("HotelDB.GetNearHotelBySight", parameters);
        //}

        /// <summary>
        /// 附近酒店通过酒店
        /// </summary>
        /// <param name="hotelID">酒店</param>
        /// <returns></returns>
        internal static List<HotelDistanceEntity> GetNearHotelByHotel(int hotelID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@hotelid", DbType.Int32, hotelID);
            return HotelDB.ExecuteSqlString<HotelDistanceEntity>("HotelDB.GetNearHotelByHotel", parameters);
        }

        /// <summary>
        /// 附近酒店通过酒店
        /// </summary>
        /// <param name="HotelID">酒店</param>
        /// <returns></returns>
        internal static List<HotelDistanceEntity> GetNearHotelByHotel(int HotelID, int District, int rank)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@hotelid", DbType.Int32, HotelID);
            parameters.AddInParameter("@district", DbType.Int32, District);
            parameters.AddInParameter("@rw", DbType.Int32, rank);
            return HotelDB.ExecuteSproc<HotelDistanceEntity>("sp1_GetPreferenceHotellist", parameters);
        }

        /// <summary>
        /// 根据餐馆查找附近酒店
        /// </summary>
        /// <param name="HotelID">酒店</param>
        /// <returns></returns>
        internal static List<HotelDistanceEntity> GetNearbyHotelByRestaurant(int district, double lat, double lon)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@district", DbType.Int32, district);
            parameters.AddInParameter("@lat", DbType.Double, lat);
            parameters.AddInParameter("@lon", DbType.Double, lon);

            return HotelDB.ExecuteSqlString<HotelDistanceEntity>("HotelDB.GetNearHotelByRestaurant", parameters);
        }

        /// <summary>
        /// 获取酒店最低价
        /// </summary>
        /// <param name="hotelID"></param>
        /// <returns></returns>
        internal static int GetHotelMinPrice(int hotelID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelId", DbType.Int32, hotelID);

            object p = HotelDB.ExecuteSqlStringAndReturnSingleField("HotelDB.GetHotelMinPrice", parameters);
            return (p is System.DBNull) ? 0 : Convert.ToInt32(p);
        }

        /// <summary>
        /// 获取酒店设施
        /// </summary>
        /// <param name="hotelID"></param>
        /// <returns></returns>
        internal static List<HotelFacilityEntity> GetFacilityList(List<int> hotelID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelIdList", DbType.String, string.Join(",", hotelID));

            return HotelDB.ExecuteSqlString<HotelFacilityEntity>("HotelDB.GetHotelFacilityList", parameters);
        }

        internal static List<HotelReview4TagKeywordEntity> GetNewHotelReviewData(string lastTime, string now)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@LastTime", DbType.String, lastTime);
            parameters.AddInParameter("@Now", DbType.String, now);

            return HotelDB.ExecuteSqlString<HotelReview4TagKeywordEntity>("HotelDB.GetRecentHotelReview", parameters);
        }

        internal static List<HotelReview4TagKeywordEntity> GetOldHotelReviewData(int writing)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@writing", DbType.Int32, writing);

            return HotelDB.ExecuteSqlString<HotelReview4TagKeywordEntity>("HotelDB.GetEarlyHotelReview", parameters);
        }

        internal static int GetSeoKeywordReviewID4Do()
        {
            return HotelDB.ExecuteSqlSingle<int>("HotelDB.GetSeoKeywordReviewID4Do").FirstOrDefault();
        }

        internal static void AddZhongdangTag(int hotelId, int writing, int tagId)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@hotelId", DbType.Int32, hotelId);
            parameters.AddInParameter("@tagId", DbType.Int32, tagId);
            parameters.AddInParameter("@writing", DbType.Int32, writing);

            HotelDB.ExecuteNonQuery("sp1_AddHotelTag", parameters);
        }

        internal static void AddSeoKeyword(int hotelId, int writing, int keywordId)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@hotelId", DbType.Int32, hotelId);
            parameters.AddInParameter("@keywordId", DbType.Int32, keywordId);
            parameters.AddInParameter("@writing", DbType.Int32, writing);

            HotelDB.ExecuteNonQuery("sp1_AddHotelSeoKeyword", parameters);
        }

        internal static List<ZhongdangHotelRuleEntity> GetZhongdangRuleList()
        {
            return HotelDB.ExecuteSqlString<ZhongdangHotelRuleEntity>("HotelDB.GetZhongdangRuleList");
        }

        internal static List<SeoKeywordRuleEntity> GetSeoKeywordRuleList()
        {
            return HotelDB.ExecuteSqlString<SeoKeywordRuleEntity>("HotelDB.GetSeoKeywordRuleList");
        }

        internal static List<ZhongdangHotelEntity> GetZhongdangHotelInfo()
        {
            return HotelDB.ExecuteSqlString<ZhongdangHotelEntity>("HotelDB.GetZhongdangHotelInfo");
        }

        internal static void ChooseZhongdangHotel()
        {
            HotelDB.ExecuteNonQuery("");
        }

        /// <summary>
        /// 获取酒店点评关键字
        /// </summary>
        /// <param name="hotelID"></param>
        /// <returns></returns>
        internal static List<HotelKeyWordEntity> GetHotelKeyWordData(List<int> hotelID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelIdList", DbType.String, string.Join(",", hotelID));

            return HotelDB.ExecuteSqlString<HotelKeyWordEntity>("HotelDB.GetHotelKeyWordList", parameters);
        }

        /// <summary>
        /// 获取酒店点评关键字的点评
        /// </summary>
        /// <param name="hotelID"></param>
        /// <returns></returns>
        internal static List<HotelKeyWordWritingEntity> GetHotelKeyWordWritingData(List<int> hotelID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelIdList", DbType.String, string.Join(",", hotelID));

            return HotelDB.ExecuteSqlString<HotelKeyWordWritingEntity>("HotelDB.GetHotelKeyWordWritingList", parameters);
        }

        internal static List<CommentKeyWordEntity> GetCommentKeyWordListData(string keyword)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@keyword", DbType.String, keyword);
            return HotelDB.ExecuteSqlString<CommentKeyWordEntity>("HotelDB.GetCommentKeyWordList", parameters);
        }

        internal static List<int> GetHotelSimilarCommentByWriting(int writing)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@Writing", DbType.Int32, writing);

            return HotelDB.ExecuteSqlSingle<int>("HotelDB.GetHotelSimilarCommentByWriting",
                                                       dbParameterCollection);
        }

        internal static List<int> GetSimilarReviewHotelList()
        {
            return HotelDB.ExecuteSqlSingle<int>("HotelDB.GetSimilarReviewHotelList");
        }

        internal static List<HotelFacilityEntity> GetAllFacilityList()
        {
            return HotelDB.ExecuteSqlString<HotelFacilityEntity>("HotelDB.GetFacilityList");
        }

        internal static List<NearbyHotelsEntity> GetNearbyPOIByHotel(int hotelID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@hotelID", DbType.Int32, hotelID);

            return HotelDB.ExecuteSqlString<NearbyHotelsEntity>("HotelDB.GetNearbyPOIByHotel", parameters);
        }

        internal static List<int> GetPOIGroupList(List<int> POIList)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@POILIst", DbType.String, string.Join(",", POIList));

            return DestDB.ExecuteSqlSingle<int>("HotelDB.GetPOIGroup", parameters);
        }

        internal static List<SimpleHotelRankEntity> GetHotelRank(List<int> hotelID, int ResourceID, int HotelTypeID, RankType rt)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelIdList", DbType.String, string.Join(",", hotelID));
            parameters.AddInParameter("@ResourceID", DbType.Int32, ResourceID);
            parameters.AddInParameter("@HotelTypeID", DbType.Int32, HotelTypeID);
            parameters.AddInParameter("@RankType", DbType.Int32, (int)rt);

            return HotelDB.ExecuteSqlString<SimpleHotelRankEntity>("HotelDB.GetHotelRank", parameters);
        }

        internal static List<HotelRankEntity> GetHotelRankByHotel(List<int> hotelID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelIdList", DbType.String, string.Join(",", hotelID));

            return HotelDB.ExecuteSqlString<HotelRankEntity>("HotelDB.GetHotelRankByHotel", parameters);
        }

        internal static List<HotelClassEntity> GetHotelClassByHotel(List<int> hotelID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelIdList", DbType.String, string.Join(",", hotelID));

            return HotelDB.ExecuteSqlString<HotelClassEntity>("HotelDB.GetHotelClassByHotel", parameters);
        }

        internal static List<HotelInfoExEntity> GetHotelInfoEX(List<int> hotelID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelIdList", DbType.String, string.Join(",", hotelID));

            return HotelDB.ExecuteSqlString<HotelInfoExEntity>("HotelDB.GetHotelInfoEX", parameters);
        }

        internal static List<HotelInfoExEntity> GetHotelInfoByDistrict(List<int> DistrictList)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@DistrictList", DbType.String, string.Join(",", DistrictList));

            return HotelDB.ExecuteSqlString<HotelInfoExEntity>("HotelDB.GetHotelInfoByDistrict", parameters);
        }

        //public static List<GetHotelShowPhotoEntity> GetHotelShowPhoto(List<int> hotelID)
        //{
        //    var parameters = new DBParameterCollection();
        //    parameters.AddInParameter("@hotelID", DbType.String, string.Join(",", hotelID));

        //    return HotelDB.ExecuteSqlString<GetHotelShowPhotoEntity>("HotelDB.GetHotelShowPhoto", parameters);
        //}

        internal static List<RatingEntity> HotelReviewGroupRating(List<int> hotelID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelIdList", DbType.String, string.Join(",", hotelID));

            var ret = HotelDB.ExecuteSqlString<RatingEntity>("HotelDB.GetHotelReviewGroupRating", parameters);

            //foreach (RatingEntity p in ret)
            //{
            //    //p.RatingType = (RatingType)p.Rating;
            //    p.UserIdentityType = (UserIdentityType)p.UserIdentity;
            //}

            return ret;
        }

        #region Hotel Comment Audit
        /// <summary>
        /// 修改酒店点评状态
        /// </summary>
        /// <param name="connectString"></param>
        /// <param name="writings">多个使用逗号隔开</param>
        /// <param name="deleted"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public static bool SetHotelReviewStatus(HotelReviewAuditEntity c)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@writing", DbType.AnsiString, c.Writing);
            dbParameterCollection.AddInParameter("@deleteds", DbType.AnsiString, c.Deleted);
            dbParameterCollection.AddInParameter("@status", DbType.AnsiString, c.Status);
            dbParameterCollection.AddInParameter("@statusDetail", DbType.Int16, c.statusDetail);
            dbParameterCollection.AddInParameter("@operator", DbType.AnsiString, c.opUid);

            object obj = HotelDB.ExecuteSprocAndReturnSingleField("sp1_HotelReview_manage", dbParameterCollection);
            return Convert.ToInt32(obj) == 0;
        }

        /// <summary>
        /// 修改酒店点评状态
        /// </summary>
        /// <param name="connectString"></param>
        /// <param name="writings">多个使用逗号隔开</param>
        /// <param name="deleted"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public static void SetCtripHotelReviewStatus(HotelReviewAuditEntity c)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@writing", DbType.AnsiString, c.Writing);
            dbParameterCollection.AddInParameter("@deleteds", DbType.AnsiString, c.Deleted);
            dbParameterCollection.AddInParameter("@operator", DbType.AnsiString, c.opUid);

            object obj = CommDB.ExecuteSprocAndReturnSingleField("sp1_comm_writing_hotelcomment_manage", dbParameterCollection);
        }
        #endregion

        #region 用户提交点评

        /// <summary>
        /// 保存酒店点评到CommDB
        /// </summary>
        /// <param name="hre">点评实体</param>
        /// <returns></returns>
        internal static HotelReviewSaveResult SaveHotelReviewInCommdb(HotelReviewEntity hre)
        {

            if (hre == null || string.IsNullOrEmpty(hre.Uid) || hre.Hotel <= 0)
                return new HotelReviewSaveResult() { Writing = 0, WritingReturn = 0, Retcode = -1 };

            int hotelid;
            hre.RoomName = Regex.Replace(hre.RoomName, @"<.{1,}>|\(.{1,}\)|\[.{1,}\]", "", RegexOptions.IgnoreCase);

            hotelid = hre.Hotel;
            if (hre.OrderID.ToString() != "")
            {
                //追溯到原订单管理的酒店ID。
                if (hre.OrderHotel > 0)
                    hotelid = hre.OrderHotel;
            }



            DBParameterCollection dbParameterCollection = new DBParameterCollection();

            dbParameterCollection.AddInParameter("@uid", DbType.AnsiString, hre.Uid);
            dbParameterCollection.AddInParameter("@Hotel", DbType.Int32, hotelid);
            dbParameterCollection.AddInParameter("@Title", DbType.AnsiString, hre.Title);
            dbParameterCollection.AddInParameter("@RatingRoom", DbType.Int32, hre.RatingRoom);
            dbParameterCollection.AddInParameter("@RatingAtmosphere", DbType.Int32, hre.RatingAtmosphere);
            dbParameterCollection.AddInParameter("@RatingService", DbType.Int32, hre.RatingService);
            dbParameterCollection.AddInParameter("@RatingCostBenefit", DbType.Int32, hre.RatingCostBenefit);
            dbParameterCollection.AddInParameter("@user_identity", DbType.Int32, hre.User_Identity);
            dbParameterCollection.AddInParameter("@identitytxt", DbType.AnsiString, hre.Identitytxt);
            dbParameterCollection.AddInParameter("@CommentSubject", DbType.AnsiString, hre.CommentSubject);
            dbParameterCollection.AddInParameter("@Content", DbType.AnsiString, hre.Content);
            dbParameterCollection.AddInParameter("@IsRecommend", DbType.AnsiString, hre.NewisCommand);
            dbParameterCollection.AddInParameter("@OrderID", DbType.Int32, hre.OrderID);
            dbParameterCollection.AddInParameter("@room", DbType.Int32, hre.Room);
            dbParameterCollection.AddInParameter("@roomname", DbType.AnsiString, hre.RoomName);
            dbParameterCollection.AddInParameter("@ipaddress", DbType.AnsiString, hre.Ipaddress);
            dbParameterCollection.AddInParameter("@deleted", DbType.AnsiString, hre.Deleted);

            var ret = CommDB.ExecuteSproc<HotelReviewSaveResult>("sp_comm_add_hotel_comment", dbParameterCollection);
            if (ret != null && ret.Count > 0)
                return ret[0];
            else
                return null;

        }

        /// <summary>
        /// 保存酒店点评到HotelDB
        /// </summary>
        /// <param name="hre">点评实体</param>
        /// <returns></returns>
        internal static HotelReviewSaveResult SaveHotelReviewInHoteldb(HotelReviewEntity hre)
        {

            if (hre == null || hre.Hotel == 0)
                return new HotelReviewSaveResult() { Writing = 0, WritingReturn = 0, Retcode = -1 };

            DBParameterCollection dbParameterCollection = new DBParameterCollection();

            dbParameterCollection.AddInParameter("@uid", DbType.String, hre.Uid);
            dbParameterCollection.AddInParameter("@Hotel", DbType.Int32, hre.Hotel);
            dbParameterCollection.AddInParameter("@title", DbType.AnsiString, hre.Title);
            dbParameterCollection.AddInParameter("@RatingRoom", DbType.Int32, hre.RatingRoom);
            dbParameterCollection.AddInParameter("@RatingAtmosphere", DbType.Int32, hre.RatingAtmosphere);
            dbParameterCollection.AddInParameter("@RatingService", DbType.Int32, hre.RatingService);
            dbParameterCollection.AddInParameter("@RatingCostBenefit", DbType.Int32, hre.RatingCostBenefit);
            dbParameterCollection.AddInParameter("@RatingValued", DbType.Int32, hre.RatingValued);
            dbParameterCollection.AddInParameter("@Content", DbType.AnsiString, hre.Content);
            dbParameterCollection.AddInParameter("@ipaddress", DbType.AnsiString, hre.Ipaddress);
            dbParameterCollection.AddInParameter("@OrderID", DbType.Int32, hre.OrderID);
            if (hre.OrderID > 0)
            {
                dbParameterCollection.AddInParameter("@room", DbType.Int32, hre.Room);
                dbParameterCollection.AddInParameter("@roomname", DbType.AnsiString, hre.RoomName);
            }
            dbParameterCollection.AddInParameter("@user_identity", DbType.Int32, hre.User_Identity);
            dbParameterCollection.AddInParameter("@identitytxt", DbType.String, hre.Identitytxt);
            dbParameterCollection.AddInParameter("@commentsubject", DbType.String, hre.CommentSubject);

            dbParameterCollection.AddInParameter("@status", DbType.String, hre.Status);
            dbParameterCollection.AddInParameter("@statusDetail", DbType.Int16, hre.StatusDetail);
            dbParameterCollection.AddInParameter("@deleted", DbType.String, hre.Deleted);
            dbParameterCollection.AddInParameter("@operator", DbType.String, hre.Operator);
            dbParameterCollection.AddInParameter("@OldWriting", DbType.Int32, hre.OldWriting);
            dbParameterCollection.AddInParameter("@writingType", DbType.String, hre.WritingType);
            dbParameterCollection.AddInParameter("@IsRecommend", DbType.String, hre.NewisCommand);
            dbParameterCollection.AddInParameter("@EmailNotice", DbType.String, hre.EmailNotice);
            dbParameterCollection.AddInParameter("@source", DbType.Int32, hre.Source);
            dbParameterCollection.AddInParameter("@NeedDealLater", DbType.Int32, hre.NeedDealLater);
            dbParameterCollection.AddInParameter("@wholerate", DbType.Int32, hre.WholeRate);
            dbParameterCollection.AddInParameter("@Wdates", DbType.String, hre.Wdates);
            dbParameterCollection.AddInParameter("@UserID", DbType.Int64, hre.UserID);
            dbParameterCollection.AddInParameter("@ACode", DbType.String, hre.ACode);

            var ret = HotelDB.ExecuteSproc<HotelReviewSaveResult>("sp1_HotelReview_insert", dbParameterCollection);
            if (ret != null && ret.Count > 0)
                return ret[0];
            else
                return null;

        }

        internal static HotelReviewSaveResult HotelReviewAddtionContentSubmit(HotelReviewAddtionEntity hotelreviewAddcontent)
        {

            if (hotelreviewAddcontent == null || hotelreviewAddcontent.Writing <= 0)
                return new HotelReviewSaveResult() { Writing = 0, WritingReturn = 0, Retcode = -1 };

            DBParameterCollection dbParameterCollection = new DBParameterCollection();

            dbParameterCollection.AddInParameter("@uid", DbType.String, hotelreviewAddcontent.Uid);
            dbParameterCollection.AddInParameter("@writing", DbType.Int32, hotelreviewAddcontent.Writing);
            dbParameterCollection.AddInParameter("@content", DbType.String, hotelreviewAddcontent.Content);
            dbParameterCollection.AddInParameter("@OrderID", DbType.Int32, hotelreviewAddcontent.OrderID);

            var ret = HotelDB.ExecuteSproc<HotelReviewSaveResult>("sp1_HotelReviewAdd_add", dbParameterCollection);
            if (ret != null && ret.Count > 0)
                return ret[0];
            else
                return null;
        }



        internal static List<SOAHotelUrlEntity> GetHotelUrlByIdlist(string idlist)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelIdList", DbType.String, idlist);
            var list = HotelDB.ExecuteSqlString<SOAHotelUrlEntity>("HotelDB.GetHotelUrlByIdlist", parameters);
            return list;
        }

        #endregion

        internal static List<NewestHotelReviewEntity> GetNewestHotelReviewByDistrictID(int districtID, int top)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@district", DbType.Int32, districtID);
            parameters.AddInParameter("@top", DbType.Int32, top);
            return HotelDB.ExecuteSqlString<NewestHotelReviewEntity>("HotelDB.GetNewestHotelReview", parameters);
        }

        internal static List<NewestHotelReviewEntity> GetNewestHotelReviewByRegion(int districtID, int top)
        {
            //string district = "  '%." + districtID + ".%'";
            var parameters = new DBParameterCollection();
            //  parameters.AddInParameter("@District", DbType.AnsiString, district);
            parameters.AddInParameter("@District", DbType.String, string.Format("%.{0}.%", districtID));
            parameters.AddInParameter("@top", DbType.Int32, top);
            return HotelDB.ExecuteSqlString<NewestHotelReviewEntity>("HotelDB.GetNewestHotelReviewByRegion", parameters);
        }

        /// <summary>
        /// 根据携程酒店ID获得驴评酒店ID
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="hoteloriid"></param>
        /// <returns></returns>
        internal static int GetHotelid(int hoteloriid)
        {
            int retCount = 0;
            try
            {
                var parameters = new DBParameterCollection();
                parameters.AddInParameter("@hoteloriid", DbType.Int32, hoteloriid);
                var rethotel = HotelDB.ExecuteSqlStringAndReturnSingleField("HotelDB.GetHotelidByHotelOriid", parameters);
                if (rethotel == null)
                    retCount = hoteloriid;
                else
                    retCount = Convert.ToInt32(rethotel);
            }
            catch (Exception)
            {
                retCount = hoteloriid;
            }
            return retCount == 0 ? hoteloriid : retCount;
        }

        /// <summary>
        /// 根据周末酒店ID获取OTA表中的携程酒店ID
        /// </summary>
        /// <param name="hotelid">周密酒店ID</param>
        /// <returns></returns>
        internal static int GetHotelOriId(int hotelid)
        {
            int oriId = 0;
            try
            {
                var parameters = new DBParameterCollection();
                parameters.AddInParameter("@HotelID", DbType.Int32, hotelid);
                var rethotel = HotelDB.ExecuteSqlStringAndReturnSingleField("HotelDB.GetHotelOriidByHotelid", parameters);
                if (rethotel == null)
                    oriId = hotelid;
                else
                    oriId = Convert.ToInt32(rethotel);
            }
            catch (Exception)
            {
                oriId = hotelid;
            }
            return oriId == 0 ? hotelid : oriId;
        }

        internal static void sp1_MemberProfile_ChangeMemType(string profileUrlNo, byte hasUGC, int memType)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@profileurlno", DbType.AnsiString, profileUrlNo);
            parameters.AddInParameter("@HasUGC", DbType.Byte, hasUGC);
            parameters.AddInParameter("@memType", DbType.Int32, memType);

            CommDB.ExecuteSprocAndReturnSingleField("sp1_MemberProfile_uploadHasUGC", parameters);

        }
        //
        internal static List<HotelRankEntity> GetHotelClassList()
        {
            //var parameters = new DBParameterCollection();
            //parameters.AddInParameter("@District", DbType.Int32, districtID);

            return HotelDB.ExecuteSqlString<HotelRankEntity>("HotelDB.GetHotelInfoClassList");
        }

        internal static List<HotelRankEntity> GetDistrictHotelClassByDistrict(int district)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@District", DbType.Int32, district);

            return HotelDB.ExecuteSqlString<HotelRankEntity>("HotelDB.GetDistrictHotelClassByDistrict", parameters);
        }

        internal static List<HotelRankEntity> GetHotelRankOrNumberByClass(List<int> hotelID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelIdList", DbType.String, string.Join(",", hotelID));

            return HotelDB.ExecuteSqlString<HotelRankEntity>("HotelDB.GetHotelRankOrNumberByClass", parameters);
        }

        internal static List<DistrictAirportTrainstationEntity> GetDistrictAirportTrainstationList(int districtID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@District", DbType.Int32, districtID);

            return HotelDB.ExecuteSqlString<DistrictAirportTrainstationEntity>("HotelDB.GetDistrictAirportTrainstation", parameters);
        }

        internal static int GetWritingIdByCommWritingId(int commwriting)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@writing", DbType.Int32, commwriting);

            var id = HotelDB.ExecuteSqlStringAndReturnSingleField("HotelDB.GetWritingIdByCommWritingId", parameters);
            if (id != null)
                return Convert.ToInt32(id);
            else
                return 0;
        }

        internal static int GetLpHotelWritingFbId(int writing)
        {
            var args = new DBParameterCollection();

            args.AddInParameter("@writing", DbType.Int32, writing);

            var val = HotelDB.ExecuteSqlStringAndReturnSingleField("HotelDB.GetLpHotelWritingFbId", args);
            if (val == null || val == DBNull.Value)
                return 0;
            else
                return Convert.ToInt32(val);

        }

        //internal static string GetUserOldNickname(string uid)
        //{
        //    if (string.IsNullOrEmpty(uid))
        //        return "";

        //    var args = new DBParameterCollection();
        //    args.AddInParameter("@Uid", DbType.AnsiString, uid);

        //    var val = CommDB.ExecuteSqlStringAndReturnSingleField("HotelDB.GetUserOldNickname", args);
        //    if (val == null || val == DBNull.Value)
        //        return "";
        //    else
        //        return Convert.ToString(val);
        //}

        /// <summary>
        /// 提供所有的目的地列表
        /// </summary>
        /// <returns></returns>
        internal static List<Entity.SOAHJDUrlEntity> GetDistrictInfo()
        {
            var val = CommDB.ExecuteSqlString<Entity.SOAHJDUrlEntity>("HotelDB.GetDistrictInfo");
            if (val == null)
                return null;
            else
                return val;
        }

        internal static List<HotelRankByDistrict> GetHotelByDistrict(int pDistinctID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@DistrictID", DbType.Int32, pDistinctID);

            return HotelDB.ExecuteSqlString<HotelRankByDistrict>("HotelDB.GetHotelByDistrict", parameters);
        }

        internal static List<int> GetDistrictListForHotelCheckData()
        {
            return HotelDB.ExecuteSqlSingle<int>("HotelDB.GetDistrictListForHotelCheckData");
        }

        internal static Dictionary<int, List<HotelClassRankEntity>> GetHotelClassRankByDistrict(int pDistinctID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@DistrictID", DbType.Int32, pDistinctID);

            var dd = HotelDB.ExecuteSqlString<HotelClassRankEntity>("HotelDB.GetHotelClassRankByDistrict", parameters);

            Dictionary<int, List<HotelClassRankEntity>> ret = new Dictionary<int, List<HotelClassRankEntity>>();
            dd.ForEach(p =>
            {
                if (ret.ContainsKey(p.HotelID))
                    ret[p.HotelID].Add(p);
                else
                    ret.Add(p.HotelID, new List<HotelClassRankEntity> { p });
            });
            return ret;
        }

        internal static List<HJDHotelReview> GetHotelcommentCache(int hotelid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@Hotelid", DbType.Int32, hotelid);

            var data = HotelDB.ExecuteSqlString<HJDHotelReview>("HotelDB.GetHotelReviewCache", parameters);
            return data;
        }

        internal static List<ClassZoneEntity> GetClassZone(int district, string classIDs)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@DistrictID", DbType.Int32, district);
            parameters.AddInParameter("@ClassID", DbType.AnsiString, classIDs);

            var dd = HotelDB.ExecuteSqlString<ClassZoneEntity>("HotelDB.GetZone", parameters);
            dd.ForEach(p => p.ZoneType = ZoneType.Zone);

            return dd;
        }

        internal static List<ClassZoneEntity> GetClassLocation(int district, string classIDs)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@DistrictID", DbType.Int32, district);
            parameters.AddInParameter("@ClassID", DbType.AnsiString, classIDs);
            var dd = HotelDB.ExecuteSqlString<ClassZoneEntity>("HotelDB.GetLocation", parameters);
            dd.ForEach(p => p.ZoneType = ZoneType.Location);

            return dd;
        }

        internal static List<ClassStarEntity> GetClassStar(int district)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@DistrictID", DbType.Int32, district);

            return HotelDB.ExecuteSqlString<ClassStarEntity>("HotelDB.GetClassStar", parameters);
        }

        internal static List<ClassBrandEntity> GetClassBrand(int district, string HotelClass)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@DistrictID", DbType.Int32, district);
            parameters.AddInParameter("@HotelClass", DbType.AnsiString, HotelClass);
            return HotelDB.ExecuteSqlString<ClassBrandEntity>("HotelDB.GetClassBrand", parameters);
        }

        internal static void GenerateHJDHotelReviewCacheForApi(int o, int p, string q)
        {

            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@Cross", DbType.Int32, o);
            parameters.AddInParameter("@Refday", DbType.Int32, p);
            parameters.AddInParameter("@WritingType", DbType.AnsiString, q);

            HotelDB.ExecuteNonQuery("sp1_HotelReview_Cache_i", parameters);

        }

        internal static void RefreshHotelInfo_Rank(int i)
        {

            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@step", DbType.Int32, i);

            HotelDB.ExecuteNonQuery("sp_RefreshHotelInfo_Rank", parameters);

        }

        internal static List<int> GetHotelIDListForCheckData(int i)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@HotelId", DbType.Int32, i);
            //dbParameterCollection.AddInParameter("@HotelReviewCheckRange", DbType.Int32, cfgHotelReviewCheckRange);

            return HotelDB.ExecuteSqlSingle<int>("HotelDB.GetHotelIDListForCheckData", dbParameterCollection);
        }

        internal static int GetHotelCountByDistrict(int intFatherDistrictID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@DistrictID", DbType.Int32, intFatherDistrictID);

            return HotelDB.ExecuteSqlSingle<int>("HotelDB.GetHotelCountByDistrict", parameters).FirstOrDefault();
        }

        internal static List<HotelReviewDelLogEntity> GetHotelReviewDelLog(int writingID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@writingID", DbType.Int32, writingID);

            return CommDB.ExecuteSqlString<HotelReviewDelLogEntity>("HotelDB.GetHotelReviewDelLog", parameters);
        }

        ///// <summary>
        /////  获取酒店有用的用户id
        ///// </summary>
        ///// <param name="writingList"></param>
        ///// <returns></returns>
        //internal static List<ReviewUsefulUserEntity> GetHotelReviewUsefulUserList(List<int> writingList)
        //{
        //    var parameters = new DBParameterCollection();
        //    parameters.AddInParameter("@ID", DbType.String, string.Join(",", writingList));

        //    return CommDB.ExecuteSqlString<ReviewUsefulUserEntity>("HotelDB.GetHotelReviewUsefulUserList", parameters);
        //}

        internal static string InsertCommwritingCommentReview(long userid, int writing, string ipaddress)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@writing", DbType.Int32, writing);
            parameters.AddInParameter("@userid", DbType.Int64, userid);
            parameters.AddInParameter("@ipaddress", DbType.AnsiString, ipaddress);
            string result = string.Empty;
            result = HotelDB.ExecuteSprocAndReturnSingleField("sp1_CommwritinghotelCommentReview_Insert", parameters).ToString();
            return result;
        }

        internal static List<int> SearchHotelIds(SearchHotelArgu argu)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@districts", DbType.String, string.Join(",", argu.DistrictID));
            parameters.AddInParameter("@type", DbType.Int32, argu.Type);
            parameters.AddInParameter("@sort", DbType.String, argu.Sort);
            parameters.AddInParameter("@order", DbType.String, argu.Order);
            parameters.AddInParameter("@start", DbType.Int32, argu.Start);
            parameters.AddInParameter("@count", DbType.Int32, argu.Count);
            parameters.AddInParameter("@hotelId", DbType.Int32, argu.HotelId);
            parameters.AddInParameter("@lat", DbType.Single, argu.Lat);
            parameters.AddInParameter("@lng", DbType.Single, argu.Lng);
            parameters.AddInParameter("@offset", DbType.Single, argu.Offset);
            var res = HotelDB.ExecuteSprocSingle<int>("sp1_SearchHotelIds", parameters);
            return res;
        }

        internal static int SearchHotelCount(SearchHotelArgu argu)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@districts", DbType.String, string.Join(",", argu.DistrictID));
            parameters.AddInParameter("@type", DbType.Int32, argu.Type);
            parameters.AddInParameter("@sort", DbType.String, argu.Sort);
            parameters.AddInParameter("@hotelId", DbType.Int32, argu.HotelId);
            parameters.AddInParameter("@lat", DbType.Single, argu.Lat);
            parameters.AddInParameter("@lng", DbType.Single, argu.Lng);
            parameters.AddInParameter("@offset", DbType.Single, argu.Offset);
            var m = HotelDB.ExecuteSprocAndReturnSingleField("sp1_SearchHotelCount", parameters);
            if (m == null || m == DBNull.Value) return 0;
            return (int)m;
        }

        internal static HotelItem GetHotel(int id)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@id", DbType.Int32, id);
            var res = HotelDB.ExecuteSqlString<HotelItem>("HotelDB.GetHotel", parameters).FirstOrDefault();
            return res;
        }

        internal static List<SimpleHotelItem> GetSimpleHotel(List<int> HotelIDs)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelIDs", DbType.String, string.Join(",", HotelIDs));
            return HotelDB.ExecuteSqlString<SimpleHotelItem>("HotelDB.GetSimpleHotels", parameters);
        }

        internal static List<PackageEntity> GetPackageListByHotelId(int HotelId)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelId", DbType.Int32, HotelId);
            return HotelDB.ExecuteSqlString<PackageEntity>("HotelDB.GetPackageListByHotelId", parameters);
        }


        internal static ZhongDangPriceSectionEntity GetZhongDangPriceSection(int DistrictID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@DistrictID", DbType.Int32, DistrictID);
            return HotelDB.ExecuteSqlString<ZhongDangPriceSectionEntity>("HotelDB.GetZhongDangPriceSection", parameters).FirstOrDefault();
        }

        internal static List<DownLoadCtripCommentEntity> GetDownCtripCommentStartDate()
        {

            return HotelDB.ExecuteSqlString<DownLoadCtripCommentEntity>("HotelDB.GetDownCtripCommentStartDate");
        }

        internal static void AddDownLoaCtripCommentTime(DateTime BegTime, int flag, string note, int pagesum, int pagecum)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@BegTime", DbType.DateTime, BegTime);
            parameters.AddInParameter("@Flag", DbType.Int32, flag);
            parameters.AddInParameter("@note", DbType.String, note);
            parameters.AddInParameter("@pagesum", DbType.Int16, pagesum);
            parameters.AddInParameter("@pagecum", DbType.Int16, pagecum);

            HotelDB.ExecuteSprocAndReturnSingleField("sp1_DownLoaCtripCommentTime_set", parameters);
        }

        internal static HotelReviewSaveResult sp1_HotelReview_Log(HotelReviewWritingLog hcl)
        {
            if (hcl == null)
                return new HotelReviewSaveResult() { Writing = 0, WritingReturn = 0, Retcode = -1 };

            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@Writing", DbType.Int32, hcl.Writing);
            parameters.AddInParameter("@Ctripwriting", DbType.Int32, hcl.Ctripwriting);
            parameters.AddInParameter("@Source", DbType.String, hcl.Source);
            parameters.AddInParameter("@Orderid", DbType.Int32, hcl.Orderid);
            parameters.AddInParameter("@OrderStatus", DbType.AnsiString, hcl.OrderStatus);
            parameters.AddInParameter("@Photostatus", DbType.AnsiString, hcl.Photostatus);
            var ret = HotelDB.ExecuteSproc<HotelReviewSaveResult>("sp1_HotelReview_Log", parameters);
            if (ret != null && ret.Count > 0)
                return ret[0];
            else
                return null;
        }

        internal static void sp1_HotelReview_CtripPic_set(int ctrippicid, int ctripWritingid, string uid, string picUrl, int hotelid, string title, int imgid, int status)
        {
            var parameters = new DBParameterCollection();

            parameters.AddInParameter("@ctrippicid", DbType.Int32, ctrippicid);
            parameters.AddInParameter("@ctripWritingid", DbType.Int32, ctripWritingid);
            parameters.AddInParameter("@Uid", DbType.AnsiString, uid);
            parameters.AddInParameter("@PicUrl", DbType.String, picUrl);
            parameters.AddInParameter("@Hotelid", DbType.Int32, hotelid);
            parameters.AddInParameter("@title", DbType.String, title);
            parameters.AddInParameter("@imgid", DbType.Int32, imgid);
            parameters.AddInParameter("@status", DbType.Int32, status);
            var ret = HotelDB.ExecuteSproc<HotelReviewSaveResult>("sp1_HotelReview_CtripPic_set", parameters);
        }

        #region 短租 度假
        //mongodb 所用排序
        internal static List<VacationHotelPyRank> GetVacationHotelPyRank(int district)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@District", DbType.String, district);

            return HotelDB.ExecuteSqlString<VacationHotelPyRank>("HotelDB.GetVacationHotelPyRank", parameters);
        }
        internal static List<UnitInfoEntity> GetUnitInfoByDistrict(List<int> DistrictList)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@DistrictList", DbType.String, string.Join(",", DistrictList));

            return HotelDB.ExecuteSqlString<UnitInfoEntity>("HotelDB.GetUnitInfoByDistrict", parameters);
        }
        internal static List<UnitInfoEntity> GetUnitInfo(List<int> unitID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@IdList", DbType.String, string.Join(",", unitID));

            return HotelDB.ExecuteSqlString<UnitInfoEntity>("HotelDB.GetUnitInfo", parameters);
        }
        internal static List<ClassZoneEntity> GetUnitZone(int district)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@DistrictID", DbType.Int32, district);


            var dd = HotelDB.ExecuteSqlString<ClassZoneEntity>("HotelDB.GetUnitZone", parameters);
            dd.ForEach(p => p.ZoneType = ZoneType.Zone);

            return dd;
        }

        internal static List<ClassZoneEntity> GetUnitLocation(int district)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@DistrictID", DbType.Int32, district);

            var dd = HotelDB.ExecuteSqlString<ClassZoneEntity>("HotelDB.GetUnitlocation", parameters);
            dd.ForEach(p => p.ZoneType = ZoneType.Location);

            return dd;
        }
        #endregion

        #region MyRegion






        public static object DeSerialize(string xml, Type type)
        {
            return DeSerialize(xml, type, Encoding.UTF8);
        }


        public static object DeSerializeUTF8(string xml, Type type)
        {
            return DeSerialize(xml, type, Encoding.UTF8);
        }

        public static object DeSerialize(string xml, Type type, Encoding encode)
        {
            try
            {
                XmlSerializer mySerializer = new XmlSerializer(type);
                MemoryStream myFileStream = new MemoryStream(encode.GetBytes(xml));
                object myObject = mySerializer.Deserialize(myFileStream);
                myFileStream.Close();
                myFileStream.Dispose();
                return myObject;
            }
            catch (Exception e)
            {
                e.WriteLog();
                return null;
            }
        }

        #endregion

        public static List<DistrictAggregateRelEntity> GenAggregateDistricts(int districtID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@DistrictId", DbType.Int32, districtID);

            return CommDB.ExecuteSqlString<DistrictAggregateRelEntity>("HotelDB.GetAggregateDistrict", parameters);
        }

        internal static List<NearbyHotelEntity> GetHotelDistanceListByHotel(int hotelid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelID", DbType.Int32, hotelid);

            return HotelDB.ExecuteSqlString<NearbyHotelEntity>("HotelDB.GetHotelDistanceListByHotel", parameters);
        }

        internal static int GetHotelPOIDistanceByHotel(int hotelid, int poiid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelID", DbType.Int32, hotelid);
            parameters.AddInParameter("@poiid", DbType.Int32, poiid);

            object o = HotelDB.ExecuteSqlStringAndReturnSingleField("HotelDB.GetHotelPOIDistanceByHotel", parameters);

            if (o is System.DBNull)
                return 0;
            else
                return Convert.ToInt32(o);
        }

        internal static string GetBookingUrlByHotel(int hotelID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelID", DbType.Int32, hotelID);
            object o = HotelDB.ExecuteSqlStringAndReturnSingleField("HotelDB.GetBookingUrlByHotel", parameters);
            if (o is System.DBNull || o == null)
                return "http://www.booking.com/hotel/fr/hoteldesarts.html?aid=354319";
            else
                return o.ToString();
        }

        internal static string GetAgodaUrlByHotel(int hotelID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelID", DbType.Int32, hotelID);
            object o = HotelDB.ExecuteSqlStringAndReturnSingleField("HotelDB.GetAgodaUrlByHotel", parameters);
            if (o is System.DBNull || o == null)
                return "";
            else
                return o.ToString();
        }

        internal static int GetHotelIDByHotelOriID(int hotelOriID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelOriID", DbType.Int32, hotelOriID);
            object o = HotelDB.ExecuteSqlStringAndReturnSingleField("HotelDB.GetHotelIDByHotelOriID", parameters);
            if (o is System.DBNull || o == null)
                return 0;
            else
                return Convert.ToInt32(o);
        }

        internal static List<TagEntity> GetTagParams4HotelList(int districtID, string classIDs)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@DistrictID", DbType.Int32, districtID);
            parameters.AddInParameter("@ClassID", DbType.AnsiString, classIDs);
            return HotelDB.ExecuteSqlString<TagEntity>("HotelDB.GetTagParams4HotelList", parameters);
        }

        internal static List<TagEntity> GetHotelTag(int hotelID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelID", DbType.Int32, hotelID);
            return HotelDB.ExecuteSqlString<TagEntity>("HotelDB.GetHotelTag", parameters);
        }

        internal static List<FeaturedEntity> GetHotelFeatured(int hotelID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelID", DbType.Int32, hotelID);
            return HotelDB.ExecuteSqlString<FeaturedEntity>("HotelDB.GetHotelFeatured", parameters);
        }

        internal static List<HotelThemeEntity> GetAllHotelTheme()
        {
            return HotelDB.ExecuteSqlString<HotelThemeEntity>("HotelDB.GetAllHotelTheme");
        }

        internal static List<InterestEntity2> GetAllInterest()
        {
            return HotelDB.ExecuteSqlString<InterestEntity2>("HotelDB.GetAllInterest");
        }

        internal static List<HotelThemeEntity> GetDistrictHotelTheme(int districtid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@districtid", DbType.Int32, districtid);
            return HotelDB.ExecuteSqlString<HotelThemeEntity>("HotelDB.GetDistrictHotelTheme", parameters);
        }

        internal static List<HotelThemeEntity> GetNearbyHotelTheme(float GLat, float GLon)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@GLat", DbType.Double, GLat);
            parameters.AddInParameter("@GLon", DbType.Double, GLon);
            return HotelDB.ExecuteSqlString<HotelThemeEntity>("HotelDB.GetNearbyHotelTheme", parameters);
        }

        internal static List<FeaturedEntity> GetFeaturedList()
        {
            return HotelDB.ExecuteSqlString<FeaturedEntity>("HotelDB.GetFeaturedList");
        }

        internal static List<TagNameEntity> GetTagList(List<int> tagIDs)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@tagIDs", DbType.AnsiString, string.Join(",", tagIDs));
            return HotelDB.ExecuteSqlString<TagNameEntity>("HotelDB.GetTagList", parameters);
        }

        internal static List<int> GetHotelTagReviewID(int tagID, int hotelID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@TagID", DbType.Int32, tagID);
            parameters.AddInParameter("@HotelID", DbType.Int32, hotelID);
            return HotelDB.ExecuteSqlSingle<int>("HotelDB.GetHotelTagReviewID", parameters);
        }

        internal static List<HotelOTAEntity> GetHotelOTAByCreateTime(DateTime CreateTime)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@CreateTime", DbType.DateTime, CreateTime);
            return HotelDB.ExecuteSqlString<HotelOTAEntity>("HotelDB.GetHotelOTAByCreateTime", parameters);
        }

        public static List<HotelOTAPrice> QueryHotelOTAPriceList(List<int> hotelIdList, DateTime arrivalTime, DateTime departureTime)
        {


            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelIDs", DbType.AnsiString, string.Join(",", hotelIdList));
            parameters.AddInParameter("@FromDate", DbType.Date, arrivalTime);
            parameters.AddInParameter("@ToDate", DbType.Date, departureTime < arrivalTime.AddDays(1) ? arrivalTime : departureTime.AddDays(-1));


            return HotelDB.ExecuteSproc<HotelOTAPrice>("sp_PricePlan_GetList", parameters);
        }

        public static List<HotelOTAPrice> QueryHotelOTAPriceListWithOTAID(List<int> hotelIdList, DateTime arrivalTime, DateTime departureTime, int OTAID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelIDs", DbType.AnsiString, string.Join(",", hotelIdList));
            parameters.AddInParameter("@FromDate", DbType.Date, arrivalTime.Date);
            parameters.AddInParameter("@ToDate", DbType.Date, departureTime < arrivalTime.AddDays(1) ? arrivalTime.Date : departureTime.AddDays(-1).Date);
            parameters.AddInParameter("@OTAID", DbType.Int32, OTAID);

            return HotelDB.ExecuteSproc<HotelOTAPrice>("sp_PricePlan_GetListWithOTAID", parameters);
        }

        public static DBParameterCollection GenQueryHotelParam(
            long FirstFilter, //--首要过滤条件。为目的地或频道ID
            List<long> Filter,
            int minPrice,
            int maxPrice,// -- 0:表示无需价格过滤
            DateTime checkIn,
            DateTime checkOut,
            int start,
            int count,
            int orderBy,// -- Rank = 1, Price = 2, Distance = 3
            int orderDirect,// --1 : OrderBy.Desc   0: OrderBy.Asc
            int nearByPOI,// -- POI点  用于按POI点由近到远排序， orderBy = 3时需提供
            string InitHotelList,// --初始酒店集 ，如此刻附近酒店集
            int PriceWithOTAID,
            double lat,
            double lng,
            int distance,
            double nlat,
            double nlng,
            double slat,
            double slng,
            double n1lat,
            double n1lng,
            double s1lat,
            double s1lng,
            bool needFilterCol,
            bool needHotelID
          )
        {

            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@FirstFilter", DbType.Int64, FirstFilter);
            parameters.AddInParameter("@Filter", DbType.String, string.Join(",", Filter));
            parameters.AddInParameter("@minPrice", DbType.Int32, minPrice);
            parameters.AddInParameter("@maxPrice", DbType.Int32, maxPrice);
            parameters.AddInParameter("@checkIn", DbType.Date, checkIn.Date);
            parameters.AddInParameter("@checkOut", DbType.Date, checkOut < checkIn.AddDays(1) ? checkIn.Date : checkOut.AddDays(-1).Date);
            parameters.AddInParameter("@start", DbType.Int32, start);
            parameters.AddInParameter("@count", DbType.Int32, count);
            parameters.AddInParameter("@orderBy", DbType.Int32, orderBy);
            parameters.AddInParameter("@orderDirect", DbType.Int32, orderDirect);
            parameters.AddInParameter("@nearByPOI", DbType.Int32, nearByPOI);
            parameters.AddInParameter("@InitHotelList", DbType.String, InitHotelList);
            parameters.AddInParameter("@needFilterCol", DbType.Boolean, needFilterCol);// BIT = 1, --是否需要返回各属性值  
            parameters.AddInParameter("@needHotelID", DbType.Boolean, needHotelID);//  BIT = 1 , --是否需要返回酒店列表  
            parameters.AddInParameter("@OTAID", DbType.Int32, PriceWithOTAID);
            parameters.AddInParameter("@Distance", DbType.Int32, distance);
            parameters.AddInParameter("@lat", DbType.Double, lat);
            parameters.AddInParameter("@lng", DbType.Double, lng);
            parameters.AddInParameter("@nlat", DbType.Double, nlat);
            parameters.AddInParameter("@nlng", DbType.Double, nlng);
            parameters.AddInParameter("@slat", DbType.Double, slat);
            parameters.AddInParameter("@slng", DbType.Double, slng);

            parameters.AddInParameter("@n1lat", DbType.Double, n1lat);
            parameters.AddInParameter("@n1lng", DbType.Double, n1lng);
            parameters.AddInParameter("@s1lat", DbType.Double, s1lat);
            parameters.AddInParameter("@s1lng", DbType.Double, s1lng);

            return parameters;

        }

        public static DataSet QueryHotel(
          long FirstFilter, //--首要过滤条件。为目的地或频道ID
          List<long> Filter,
          int minPrice,
          int maxPrice,// -- 0:表示无需价格过滤
          DateTime checkIn,
          DateTime checkOut,
          int start,
          int count,
          int orderBy,// -- Rank = 1, Price = 2, Distance = 3
          int orderDirect,// --1 : OrderBy.Desc   0: OrderBy.Asc
          int nearByPOI,// -- POI点  用于按POI点由近到远排序， orderBy = 3时需提供
          string InitHotelList,// --初始酒店集 ，如此刻附近酒店集
          int PriceWithOTAID,
          double lat,
          double lng,
          int distance,
          double nlat,
          double nlng,
          double slat,
          double slng,
          double n1lat,
          double n1lng,
          double s1lat,
          double s1lng,
          bool needFilterCol,
          bool needHotelID
        )
        {

            var parameters = GenQueryHotelParam(
             FirstFilter, //--首要过滤条件。为目的地或频道ID
            Filter,
             minPrice,
             maxPrice,// -- 0:表示无需价格过滤
             checkIn,
             checkOut,
             start,
             count,
             orderBy,// -- Rank = 1, Price = 2, Distance = 3
             orderDirect,// --1 : OrderBy.Desc   0: OrderBy.Asc
             nearByPOI,// -- POI点  用于按POI点由近到远排序， orderBy = 3时需提供
             InitHotelList,// --初始酒店集 ，如此刻附近酒店集
             PriceWithOTAID,
             lat,
             lng,
             distance,
             nlat,
             nlng,
             slat,
             slng,
             n1lat,
             n1lng,
             s1lat,
             s1lng,
             needFilterCol,
             needHotelID
          );

            return HotelDB.ExecuteSp("sp_Hotel_Query", parameters);

        }

        public static DataSet QueryHotelForMagiCall(
          long FirstFilter, //--首要过滤条件。为目的地或频道ID
          List<long> Filter,
          int minPrice,
          int maxPrice,// -- 0:表示无需价格过滤
          DateTime checkIn,
          DateTime checkOut,
          int start,
          int count,
          int orderBy,// -- Rank = 1, Price = 2, Distance = 3
          int orderDirect,// --1 : OrderBy.Desc   0: OrderBy.Asc
          int nearByPOI,// -- POI点  用于按POI点由近到远排序， orderBy = 3时需提供
          string InitHotelList,// --初始酒店集 ，如此刻附近酒店集
          int PriceWithOTAID,
          double lat,
          double lng,
          int distance,
          double nlat,
          double nlng,
          double slat,
          double slng,
          double n1lat,
          double n1lng,
          double s1lat,
          double s1lng,
          bool needFilterCol,
          bool needHotelID
        )
        {

            var parameters = GenQueryHotelParam(
             FirstFilter, //--首要过滤条件。为目的地或频道ID
            Filter,
             minPrice,
             maxPrice,// -- 0:表示无需价格过滤
             checkIn,
             checkOut,
             start,
             count,
             orderBy,// -- Rank = 1, Price = 2, Distance = 3
             orderDirect,// --1 : OrderBy.Desc   0: OrderBy.Asc
             nearByPOI,// -- POI点  用于按POI点由近到远排序， orderBy = 3时需提供
             InitHotelList,// --初始酒店集 ，如此刻附近酒店集
             PriceWithOTAID,
             lat,
             lng,
             distance,
             nlat,
             nlng,
             slat,
             slng,
             n1lat,
             n1lng,
             s1lat,
             s1lng,
             needFilterCol,
             needHotelID
          );

            return HotelDB.ExecuteSp("sp_Hotel_Query_ForMagiCall", parameters);

        }

        public static DataSet QueryReview(
          long FirstFilter, //--首要过滤条件。为目的地或频道ID
          List<long> Filter,
          int start,
          int count,
          int orderBy,// -- Date = 0, Score = 1, Source = 2(点评来源排序 先把周末酒店用户写的放前再按写点评日期降序)
          int orderDirect// --1 : OrderBy.Desc, 0: OrderBy.Asc
        )
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@FirstFilter", DbType.Int64, FirstFilter);
            parameters.AddInParameter("@Filter", DbType.String, string.Join(",", Filter));
            parameters.AddInParameter("@start", DbType.Int32, start);
            parameters.AddInParameter("@count", DbType.Int32, count);
            parameters.AddInParameter("@orderBy", DbType.Int32, orderBy);
            parameters.AddInParameter("@orderDirect", DbType.Int32, orderDirect);

            //HotelService.writeLog(string.Format("FirstFilter:{0};Filter:{1};start:{2};count:{3};orderBy:{4};orderDirect:{5};", FirstFilter, string.Join(",", Filter), start, count, orderBy, orderDirect));

            // return HotelDB.ExecuteSp("sp_ReviewFilet_Search", parameters);
            return HotelReviewsDB.ExecuteSp("sp_Review_Query", parameters);
        }

        public static DataSet QuerySEOKeywordReview(
         int hotelID, //--首要过滤条件。为目的地
         int seoKeywordID,
            int start,
         int count,
         int orderBy,// -- time = 0 score = 1 
         int orderDirect// --1 : OrderBy.Desc   0: OrderBy.Asc
       )
        {

            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@hotelID", DbType.Int32, hotelID);
            parameters.AddInParameter("@seoKeywordID", DbType.Int32, seoKeywordID);
            parameters.AddInParameter("@start", DbType.Int32, start);
            parameters.AddInParameter("@count", DbType.Int32, count);
            parameters.AddInParameter("@orderBy", DbType.Int32, orderBy);
            parameters.AddInParameter("@orderDirect", DbType.Int32, orderDirect);

            return HotelDB.ExecuteSp("sp_SEOKeywordReview_Query", parameters);

        }

        public static List<ValuedDistrictEntity> GetMostValuedDistrict()
        {
            List<int> districtIds = GetValuedDistrict();
            List<ValuedDistrictEntity> list = new List<ValuedDistrictEntity>();
            var parameters = new DBParameterCollection();

            parameters.AddInParameter("@DistrictId", DbType.String, string.Join(",", districtIds));
            var ret = HotelDB.ExecuteSqlString<ValuedDistrictEntity>("HotelDB.GetMostValuedDistrict", parameters);
            return districtIds.Select(id => ret.Where(v => v.DistrictId == id).FirstOrDefault()).ToList();
        }

        public static List<FilterDicEntity> GetAllHotelFilterDic()
        {
            return HotelDB.ExecuteSqlString<FilterDicEntity>("HotelDB.GetAllHotelFilterDic");
        }

        public static List<int> GetValuedDistrict()
        {
            List<string> list = CommDB.ExecuteSqlSingle<string>("CommDB.GetValuedDistrictIds");
            List<int> result = new List<int>();
            foreach (string id in list)
            {
                result.Add(int.Parse(id));
            }
            return result;
        }

        public static List<UniqueDistrictEntity> GetMostUniqueDistrict()
        {
            return HotelDB.ExecuteSqlString<UniqueDistrictEntity>("HotelDB.GetMostUniqueDistrict");
        }

        public static List<HotelReviewEntity> GetReviewStatus(int hotelID, long userID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("resource", DbType.Int32, hotelID);
            parameters.AddInParameter("userid", DbType.Int64, userID);
            return HotelDB.ExecuteSqlString<HotelReviewEntity>("HotelDB.GetReviewStatus", parameters);
        }

        public static List<CommDictEntity> GetCommDictList(int type)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("type", DbType.Int32, type);
            return CommDB.ExecuteSqlString<CommDictEntity>("CommDB.GetCommDictList", parameters);
        }

        public static List<HJD.HotelManagementCenter.Domain.HolidayInfoEntity> GetHolidays()
        {
            var parameters = new DBParameterCollection();
            return CommDB.ExecuteSqlString<HJD.HotelManagementCenter.Domain.HolidayInfoEntity>("CommDB.GetHolidays", parameters);
        }

        internal static List<int> GetHotelReviewWritingListByUserid(long userid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("userid", DbType.Int64, userid);
            return HotelDB.ExecuteSqlSingle<int>("HotelDB.GetHotelReviewWritingListByUserid", parameters);
        }

        public static List<ValuedHotelEntity> GetBaseValuedHotel()
        {
            return HotelDB.ExecuteSqlString<ValuedHotelEntity>("HotelDB.GetBaseValuedHotel");
        }

        public static void AddValuedHotel(int HotelId, double FinalScore, int type)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@hotelId", DbType.Int32, HotelId);
            parameters.AddInParameter("@FinalScore", DbType.Double, FinalScore);
            parameters.AddInParameter("@type", DbType.Int32, type);
            HotelDB.ExecuteNonQuery("sp1_AddValuedHotel", parameters);
        }

        public static void TruncateValued()
        {
            var parameters = new DBParameterCollection();
            HotelDB.ExecuteNonQuery("sp1_TruncateValued", parameters);
        }

        public static List<HotelSEOKeyword> GetHotelSEOKeyword(int hotelID = 0)
        {
            if (hotelID == 0)
            {
                return HotelDB.ExecuteSqlString<HotelSEOKeyword>("HotelDB.GetHotelSEOKeyword");
            }
            else
            {
                var parameters = new DBParameterCollection();
                parameters.AddInParameter("@hotelId", DbType.Int32, hotelID);
                return HotelDB.ExecuteSqlString<HotelSEOKeyword>("HotelDB.GetHotelSEOKeywordByHotel", parameters);
            }
        }

        public static void MoveReview(MoveReview17U mr)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ReviewId", DbType.Int32, mr.ReviewID);
            parameters.AddInParameter("@HotelId", DbType.Int32, mr.HotelId);
            parameters.AddInParameter("@Creator", DbType.String, mr.Creator);
            parameters.AddInParameter("@CreateDate", DbType.DateTime, mr.CreateDate);
            parameters.AddInParameter("@Content", DbType.String, mr.Content);
            parameters.AddInParameter("@TuijianHotel", DbType.Int32, mr.TuiJianHotel);
            parameters.AddInParameter("@Voucher", DbType.Int32, mr.Voucher);
            parameters.AddInParameter("@HOEVoucherFlag", DbType.Int32, mr.HOEVoucherFlag);
            parameters.AddInParameter("@HDMDType", DbType.Int32, mr.HDMDType);
            parameters.AddInParameter("@RoomTypeName", DbType.String, mr.RoomTypeName);
            parameters.AddInParameter("@OverRallRating", DbType.Int32, mr.OverRallRating);
            parameters.AddInParameter("@DealState", DbType.Int32, mr.DealState);

            HotelDB.ExecuteNonQuery("sp1_MoveReview_17U", parameters);
        }

        public static void MoveReview(MoveReviewElong mr)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ReviewId", DbType.Int32, mr.ReviewID);
            parameters.AddInParameter("@HotelId", DbType.Int32, mr.HotelId);
            parameters.AddInParameter("@Creator", DbType.String, mr.Creator);
            parameters.AddInParameter("@CreateDate", DbType.DateTime, mr.CreateDate);
            parameters.AddInParameter("@Title", DbType.String, mr.Title);
            parameters.AddInParameter("@Content", DbType.String, mr.Content);
            parameters.AddInParameter("@RecommendType", DbType.Int32, mr.RecommendType);
            parameters.AddInParameter("@UsefulTotal", DbType.Int32, mr.UsefulTotal);
            parameters.AddInParameter("@UserRating", DbType.Int32, mr.UserRating);
            parameters.AddInParameter("@CommentTitularId", DbType.Int32, mr.CommentTitularId);
            parameters.AddInParameter("@CommentSourceId", DbType.Int32, mr.CommentSourceId);
            parameters.AddInParameter("@ParentReviewId", DbType.Int32, mr.ParentReviewId);
            parameters.AddInParameter("@DealState", DbType.Int32, mr.DealState);

            OTADataDB.ExecuteNonQuery("sp1_MoveReview_Elong", parameters);
        }

        public static void MoveReview(MoveReviewCtrip mr)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@Writing", DbType.Int32, mr.Writing);
            parameters.AddInParameter("@CtripHotelId", DbType.Int32, mr.CtripHotelId);
            parameters.AddInParameter("@reviewType", DbType.String, mr.reviewType);
            parameters.AddInParameter("@commentdate", DbType.DateTime, mr.commentdate);
            parameters.AddInParameter("@ctripCommentId", DbType.Int32, mr.ctripCommentId);
            parameters.AddInParameter("@roomType", DbType.String, mr.roomType);
            parameters.AddInParameter("@wholerate", DbType.Double, mr.wholerate);
            parameters.AddInParameter("@score_weisheng", DbType.Int32, mr.score_weisheng);
            parameters.AddInParameter("@score_fuwu", DbType.Int32, mr.score_fuwu);
            parameters.AddInParameter("@score_sheshi", DbType.Int32, mr.score_sheshi);
            parameters.AddInParameter("@score_weizhi", DbType.String, mr.score_weizhi);
            parameters.AddInParameter("@commentdetail", DbType.String, mr.commentdetail);
            parameters.AddInParameter("@hotelfeedback", DbType.String, mr.hotelfeedback);
            if (mr.hotelfeedbackdate > DateTime.MinValue)
                parameters.AddInParameter("@hotelfeedbackdate", DbType.DateTime, mr.hotelfeedbackdate);
            parameters.AddInParameter("@updatetime", DbType.DateTime, mr.updatetime);
            parameters.AddInParameter("@username", DbType.String, mr.username);
            parameters.AddInParameter("@DealState", DbType.Int32, mr.DealState);
            parameters.AddInParameter("@AddReview", DbType.String, mr.AddReview);
            parameters.AddInParameter("@IsRecommand", DbType.Boolean, mr.IsRecommand);

            OTADataDB.ExecuteNonQuery("sp1_MoveReview_Ctrip", parameters);
        }

        internal static int InitReviewOTA(MoveReviewOTA mr)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@Writing", DbType.Int32, mr.ID);
            parameters.AddInParameter("@OTAID", DbType.Int32, mr.OTAReviewID);
            parameters.AddInParameter("@HotelID", DbType.Int32, mr.HotelId);
            parameters.AddInParameter("@Creator", DbType.String, mr.Creator);
            parameters.AddInParameter("@CreateDate", DbType.DateTime, mr.CreateDate);
            parameters.AddInParameter("@CommentSubject", DbType.String, mr.CommentSubject);
            parameters.AddInParameter("@Content", DbType.String, mr.Content);
            parameters.AddInParameter("@ChannelID", DbType.Int32, mr.ChannelID);
            parameters.AddInParameter("@Score", DbType.Int32, mr.Score);
            return Convert.ToInt32(HotelDB.ExecuteSprocAndReturnSingleField("sp1_OTAHotelReview_insert", parameters));
        }

        internal static void InitReview17U(MoveReview17U mr)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@OTAID", DbType.Int32, mr.ReviewID);
            parameters.AddInParameter("@HotelID", DbType.Int32, mr.HAOJIUDIANHotelId);
            parameters.AddInParameter("@Creator", DbType.String, mr.Creator);
            parameters.AddInParameter("@CreateDate", DbType.DateTime, mr.CreateDate);
            parameters.AddInParameter("@Content", DbType.String, mr.Content);
            parameters.AddInParameter("@ChannelID", DbType.Int32, 4);
            HotelDB.ExecuteNonQuery("sp1_OTAHotelReview_insert", parameters);
        }

        internal static void InitReviewElong(MoveReviewElong mr)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@OTAID", DbType.Int32, mr.ReviewID);
            parameters.AddInParameter("@HotelID", DbType.Int32, mr.HAOJIUDIANHotelId);
            parameters.AddInParameter("@Creator", DbType.String, mr.Creator);
            parameters.AddInParameter("@CreateDate", DbType.DateTime, mr.CreateDate);
            parameters.AddInParameter("@Content", DbType.String, mr.Content);
            parameters.AddInParameter("@ChannelID", DbType.Int32, 6);
            parameters.AddInParameter("@CommentSubject", DbType.String, mr.Title);
            parameters.AddInParameter("@Score", DbType.Int32, mr.RecommendType + 3);//elong 的推荐（1）、普通（0）、不推荐（-1）分别对应2，3，4分

            HotelDB.ExecuteNonQuery("sp1_OTAHotelReview_insert", parameters);
        }

        internal static int InitReviewBooking(MoveReviewBooking mr)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@OTAID", DbType.Int32, mr.ReviewId);
            parameters.AddInParameter("@HotelID", DbType.Int32, mr.HotelId);
            parameters.AddInParameter("@Creator", DbType.String, mr.Uid);
            parameters.AddInParameter("@CreateDate", DbType.DateTime, mr.ReviewDate);
            parameters.AddInParameter("@Content", DbType.String, mr.ReviewContent);
            parameters.AddInParameter("@ChannelID", DbType.Int32, 1);
            parameters.AddInParameter("@CommentSubject", DbType.String, mr.Title);
            parameters.AddInParameter("@Score", DbType.Int32, mr.Score);

            return HotelDB.ExecuteNonQuery("sp1_OTAHotelReview_insert", parameters);
        }

        internal static int InitReviewCtrip(MoveReviewCtrip mr)
        {
            int userIdenty = 0;
            switch (mr.reviewType)
            {
                case "商务出差": userIdenty = 10; break;
                case "带小孩出游": userIdenty = 20; break;
                case "和家人出游": userIdenty = 30; break;
                case "和朋友出游": userIdenty = 40; break;
                case "独自出游": userIdenty = 50; break;
                case "代人预订": userIdenty = 60; break;
            }

            DBParameterCollection dbParameterCollection = new DBParameterCollection();

            dbParameterCollection.AddInParameter("@writing", DbType.Int32, mr.Writing);
            dbParameterCollection.AddInParameter("@uid", DbType.String, mr.username);
            dbParameterCollection.AddInParameter("@Hotel", DbType.Int32, mr.HotelId);
            dbParameterCollection.AddInParameter("@title", DbType.AnsiString, "");
            dbParameterCollection.AddInParameter("@RatingRoom", DbType.Int32, mr.score_weisheng);
            dbParameterCollection.AddInParameter("@RatingAtmosphere", DbType.Int32, mr.score_weizhi);
            dbParameterCollection.AddInParameter("@RatingService", DbType.Int32, mr.score_fuwu);
            dbParameterCollection.AddInParameter("@RatingCostBenefit", DbType.Int32, mr.score_sheshi);
            dbParameterCollection.AddInParameter("@RatingValued", DbType.Int32, 0);
            dbParameterCollection.AddInParameter("@Content", DbType.AnsiString, mr.commentdetail);
            dbParameterCollection.AddInParameter("@ipaddress", DbType.AnsiString, "");
            dbParameterCollection.AddInParameter("@OrderID", DbType.Int32, 0);
            dbParameterCollection.AddInParameter("@room", DbType.Int32, 0);
            dbParameterCollection.AddInParameter("@roomname", DbType.AnsiString, mr.roomType);
            dbParameterCollection.AddInParameter("@user_identity", DbType.Int32, userIdenty);
            dbParameterCollection.AddInParameter("@identitytxt", DbType.AnsiString, mr.reviewType);
            dbParameterCollection.AddInParameter("@commentsubject", DbType.AnsiString, "");
            dbParameterCollection.AddInParameter("@writingType", DbType.StringFixedLength, "C");
            dbParameterCollection.AddInParameter("@status", DbType.StringFixedLength, "P");
            dbParameterCollection.AddInParameter("@statusDetail", DbType.Int32, 1);
            dbParameterCollection.AddInParameter("@deleted", DbType.StringFixedLength, "F");
            dbParameterCollection.AddInParameter("@operator", DbType.AnsiString, "");
            dbParameterCollection.AddInParameter("@IsRecommend", DbType.StringFixedLength, mr.IsRecommand ? "T" : "F");
            dbParameterCollection.AddInParameter("@EmailNotice", DbType.StringFixedLength, "F");
            dbParameterCollection.AddInParameter("@UserID", DbType.Int64, 661391);
            dbParameterCollection.AddInParameter("@wholerate", DbType.Int32, mr.wholerate);
            dbParameterCollection.AddInParameter("@Wdates", DbType.AnsiString, mr.commentdate);
            dbParameterCollection.AddInParameter("@otaReviewid", DbType.Int32, mr.ctripCommentId);
            dbParameterCollection.AddInParameter("@channeltype", DbType.Int32, 2);


            var ret = HotelDB.ExecuteSproc<HotelReviewSaveResult>("sp1_HotelReview_insert", dbParameterCollection);

            return ret.FirstOrDefault().Writing;
        }

        internal static void UpdateReviewCtrip(int ctripCommentId, int CtripHotelId)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();

            dbParameterCollection.AddInParameter("@ctripCommentId", DbType.Int32, ctripCommentId);
            dbParameterCollection.AddInParameter("@CtripHotelId", DbType.Int32, CtripHotelId);
            dbParameterCollection.AddInParameter("@dealstate", DbType.Int32, 9);
            OTADataDB.ExecuteNonQuery("sp_UpdateReviewCtripState", dbParameterCollection);
        }

        internal static void UpdateReviewELong(int ReviewID, int HotelId)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();

            dbParameterCollection.AddInParameter("@ReviewID", DbType.Int32, ReviewID);
            dbParameterCollection.AddInParameter("@HotelId", DbType.Int32, HotelId);
            dbParameterCollection.AddInParameter("@dealstate", DbType.Int32, 9);
            OTADataDB.ExecuteNonQuery("sp_UpdateReviewELongState", dbParameterCollection);
        }

        internal static void UpdateReview17U(int ReviewID, int HotelId)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();

            dbParameterCollection.AddInParameter("@ReviewID", DbType.Int32, ReviewID);
            dbParameterCollection.AddInParameter("@HotelId", DbType.Int32, HotelId);
            dbParameterCollection.AddInParameter("@dealstate", DbType.Int32, 9);
            OTADataDB.ExecuteNonQuery("sp_UpdateReview17UState", dbParameterCollection);
        }

        internal static List<OTACodeEntity> GetOTAHotelCode(int channelid, List<int> otahotelid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@channelid", DbType.Int32, channelid);
            parameters.AddInParameter("@otahotelidlist", DbType.String, String.Join(",", otahotelid));
            return HotelDB.ExecuteSqlString<OTACodeEntity>("HotelDB.GetOTAHotelCode", parameters);
        }

        internal static void BindUserAccountAndOrders(long userid, string phone)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();

            dbParameterCollection.AddInParameter("@userid", DbType.Int64, userid);
            dbParameterCollection.AddInParameter("@phone", DbType.String, phone);

            HotelDB.ExecuteNonQuery("sp4_Orders_BindUserAccount", dbParameterCollection);
        }

        internal static List<int> GetPackagedInterestPlace()
        {
            var parameters = new DBParameterCollection();
            return HotelDB.ExecuteSqlSingle<int>("HotelDB.GetPackagedInterestPlace", parameters);
        }

        internal static int UpdateInspectorRefHotelState()
        {
            var parameters = new DBParameterCollection();
            Object obj = HotelBizDB.ExecuteSprocAndReturnSingleField("SP_InspectorHotel_BatchExpire", parameters);
            try
            {
                return Convert.ToInt32(obj);
            }
            catch
            {
                return 0;
            }
        }

        internal static List<InspectorHotel> GetInspectorHotelByHotelId(int hotelId)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelId", DbType.Int32, hotelId);
            return HotelBizDB.ExecuteSqlString<InspectorHotel>("HotelBizDB.GetInspectorHotelByHotelId", parameters);
        }

        internal static int InsertOrUpdatePoints(PointsEntity point)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@ID", DbType.Int32, point.ID);
            dbParameterCollection.AddInParameter("@TypeID", DbType.Int32, point.TypeID == 0 ? 1 : point.TypeID);
            dbParameterCollection.AddInParameter("@BusinessID", DbType.Int32, point.BusinessID);
            dbParameterCollection.AddInParameter("@UserID", DbType.Int64, point.UserID);
            dbParameterCollection.AddInParameter("@TotalPoint", DbType.Int32, point.TotalPoint);
            dbParameterCollection.AddInParameter("@LeavePoint", DbType.Int32, point.LeavePoint);
            dbParameterCollection.AddInParameter("@Approver", DbType.Int64, point.Approver);
            dbParameterCollection.AddInParameter("@ExpiredTime", DbType.DateTime, point.ExpiredTime == DateTime.MinValue ? new DateTime(2100, 1, 1).Date : point.ExpiredTime);
            HotelBizDB.ExecuteNonQuery("SP_Points_InsertOrUpdate", dbParameterCollection);
            return 0;
        }

        internal static int DeletePoints(int id)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@ID", DbType.Int32, id);
            HotelBizDB.ExecuteNonQuery("SP_Points_Delete", dbParameterCollection);
            return 0;
        }

        internal static int InsertOrUpdatePointsConsume(PointsConsumeEntity pointConsume)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@ID", DbType.Int32, pointConsume.ID);
            dbParameterCollection.AddInParameter("@UserID", DbType.Int64, pointConsume.UserID);
            dbParameterCollection.AddInParameter("@BusinessID", DbType.Int64, pointConsume.BusinessID);
            dbParameterCollection.AddInParameter("@TypeID", DbType.Int32, pointConsume.TypeID == 0 ? 2 : pointConsume.TypeID);
            dbParameterCollection.AddInParameter("@ConsumePoint", DbType.Int32, pointConsume.ConsumePoint);
            dbParameterCollection.AddInParameter("@State", DbType.Int32, pointConsume.State);
            HotelBizDB.ExecuteNonQuery("SP_PointsConsume_InsertOrUpdate", dbParameterCollection);
            return 0;
        }

        internal static List<PointsEntity> GetPointsEntity(long userId)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@userid", DbType.Int64, userId);
            List<PointsEntity> list = HotelBizDB.ExecuteSqlString<PointsEntity>("HotelBizDB.GetPointsEntity", dbParameterCollection);
            return list != null ? list : new List<PointsEntity>();
        }

        internal static List<PointsEntity> GetExpirePointsEntity(DateTime startTime, DateTime endTime, string userids = "")
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@StartTime", DbType.DateTime, startTime);
            dbParameterCollection.AddInParameter("@EndTime", DbType.DateTime, endTime);
            dbParameterCollection.AddInParameter("@UserIds", DbType.String, userids);
            List<PointsEntity> list = new List<PointsEntity>();
            if (string.IsNullOrWhiteSpace(userids))
            {
                list = HotelBizDB.ExecuteSqlString<PointsEntity>("HotelBizDB.GetExpirePointsEntity", dbParameterCollection);
            }
            else
            {
                list = HotelBizDB.ExecuteSqlString<PointsEntity>("HotelBizDB.GetExpirePointsByUserIds", dbParameterCollection);
            }
            return list != null ? list : new List<PointsEntity>();
        }
        internal static List<PointsEntity> GetPointsEntityByToDayAndTypeId(int typeId)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@TypeID", DbType.Int32, typeId);
            List<PointsEntity> list = HotelBizDB.ExecuteSqlString<PointsEntity>("HotelBizDB.GetPointsEntityByToDayAndTypeId", dbParameterCollection);
            return list != null ? list : new List<PointsEntity>();
        }

        internal static List<ExpirePointsEntity> GetExpirePoints(DateTime startTime, DateTime endTime)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@startTime", DbType.DateTime, startTime);
            dbParameterCollection.AddInParameter("@endTime", DbType.DateTime, endTime);
            List<ExpirePointsEntity> list = HotelBizDB.ExecuteSqlString<ExpirePointsEntity>("HotelBizDB.GetPointsEntityByToDayAndTypeId", dbParameterCollection);
            return list != null ? list : new List<ExpirePointsEntity>();
        }

        internal static List<PointsConsumeEntity> GetPointsConsumeEntity(long userId)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@userid", DbType.Int64, userId);
            List<PointsConsumeEntity> list = HotelBizDB.ExecuteSqlString<PointsConsumeEntity>("HotelBizDB.GetPointsConsumeEntity", dbParameterCollection);
            return list != null ? list : new List<PointsConsumeEntity>();
        }

        internal static List<PointsTypeDefineEntity> GetPointsTypeDefineList(string code, int type)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@code", DbType.String, code);
            dbParameterCollection.AddInParameter("@type", DbType.Int32, type);
            List<PointsTypeDefineEntity> list = HotelBizDB.ExecuteSqlString<PointsTypeDefineEntity>("HotelBizDB.GetPointsTypeDefine", dbParameterCollection);
            return list != null ? list : new List<PointsTypeDefineEntity>();
        }

        internal static PointsConsumeEntity GetPointsConsumeByIDOrTypeID(int id, HJD.HotelServices.Contracts.HotelServiceEnums.ConsumeUserPointsBizType typeID, int businessID, int state)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            List<PointsConsumeEntity> list = null;
            if (id != 0)
            {
                dbParameterCollection.AddInParameter("@id", DbType.Int32, id);
                list = HotelBizDB.ExecuteSqlString<PointsConsumeEntity>("HotelBizDB.GetPointsConsumeByID", dbParameterCollection);
            }
            else if (  businessID != 0)
            {
                dbParameterCollection.AddInParameter("@typeID", DbType.Int32, (int)typeID);
                dbParameterCollection.AddInParameter("@businessID", DbType.Int32, businessID);
                dbParameterCollection.AddInParameter("@state", DbType.Int32, state == 0 ? 1 : state);
                list = HotelBizDB.ExecuteSqlString<PointsConsumeEntity>("HotelBizDB.GetPointsConsumeByTypeIDAndBusinessID", dbParameterCollection);
            }
            return list != null && list.Count != 0 ? list[0] : new PointsConsumeEntity();
        }

        internal static List<SourceIDAndObjectNameEntity> GetObjectNamesByCommentIDs(List<long> ids)
        {
            List<SourceIDAndObjectNameEntity> list = null;
            if (ids != null && ids.Count != 0)
            {
                DBParameterCollection dbParameterCollection = new DBParameterCollection();
                dbParameterCollection.AddInParameter("@CommentIDList", DbType.String, string.Join(",", ids));
                list = HotelDB.ExecuteSqlString<SourceIDAndObjectNameEntity>("HotelDB.GetObjectNamesByCommentIDs", dbParameterCollection);
            }
            return list == null ? new List<SourceIDAndObjectNameEntity>() : list;
        }

        internal static List<SourceIDAndObjectNameEntity> GetObjectNamesByInspectorRefHotelIDs(List<long> ids)
        {
            List<SourceIDAndObjectNameEntity> list = null;
            if (ids != null && ids.Count != 0)
            {
                DBParameterCollection dbParameterCollection = new DBParameterCollection();
                dbParameterCollection.AddInParameter("@InspectorRefHotelIDList", DbType.String, string.Join(",", ids));
                list = HotelDB.ExecuteSqlString<SourceIDAndObjectNameEntity>("HotelDB.GetObjectNamesByInspectorRefHotelIDs", dbParameterCollection);
            }
            return list == null ? new List<SourceIDAndObjectNameEntity>() : list;
        }

        internal static PointsEntity GetPointsByIDOrTypeIDAndBusinessID(int id, int typeID, int businessID)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            List<PointsEntity> list = null;
            if (id != 0)
            {
                dbParameterCollection.AddInParameter("@id", DbType.Int32, id);
                list = HotelBizDB.ExecuteSqlString<PointsEntity>("HotelBizDB.GetPointsByID", dbParameterCollection);
            }
            else if (typeID != 0 && businessID != 0)
            {
                dbParameterCollection.AddInParameter("@typeID", DbType.Int32, typeID);
                dbParameterCollection.AddInParameter("@businessID", DbType.Int32, businessID);
                list = HotelBizDB.ExecuteSqlString<PointsEntity>("HotelBizDB.GetPointsByTypeIDAndBusinessID", dbParameterCollection);
            }
            return list != null && list.Count != 0 ? list[0] : new PointsEntity();
        }

        internal static List<PointsEntity> GetPointslistNumByUserIdAndTypeId(long userId, int typeId)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@userId", DbType.Int32, userId);
            dbParameterCollection.AddInParameter("@typeID", DbType.Int32, typeId);
            var list = HotelBizDB.ExecuteSqlString<PointsEntity>("HotelBizDB.GetPointslistNumByUserIdAndTypeId", dbParameterCollection);
            return list == null ? new List<PointsEntity>() : list;
        }

        //<summary>
        //更新对应的数据 来实现相关信息
        //</summary>
        //<returns></returns>
        internal static int UpdateInspectorStateByPointsNum()
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            HotelBizDB.ExecuteNonQuery("SP_Inspector_UpdateState2", dbParameterCollection);
            return 0;
        }

        /// <summary>
        /// 根据周末酒店ID，获取CtripDB中的真实携程酒店ID
        /// </summary>
        /// <param name="hotelId">周末酒店ID</param>
        /// <returns></returns>
        internal static int GetCtrpHotelId(int hotelId)
        {
            int realHotelOriId = 0;
            try
            {
                //先得到我们OTA中的携程酒店ID
                var defHotelOriId = GetHotelOriId(hotelId);

                //然后得到真实的携程酒店ID
                var parameters = new DBParameterCollection();
                parameters.AddInParameter("@HotelOriID", DbType.Int32, defHotelOriId);
                var rethotel = CtripDB.ExecuteSqlStringAndReturnSingleField("CtripDB.GetCtripHotelIdByHotelId", parameters);
                if (rethotel != null)
                    realHotelOriId = Convert.ToInt32(rethotel);
            }
            catch (Exception)
            {
                //realHotelOriId = hotelId;
            }
            return realHotelOriId;
        }

        internal static string GetPriceChannelIdByHotelId(long hotelid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelID", DbType.Int64, hotelid);

            return HotelDB.ExecuteSqlStringAndReturnSingleField("HotelDB.GetPriceChannelIdByHotelId", parameters).ToString();
        }

        internal static int GetHotelContactsFor3(int hotelid)
        {
            int retCount = 0;
            try
            {
                var parameters = new DBParameterCollection();
                parameters.AddInParameter("@HotelId", DbType.Int32, hotelid);
                var rethotel = HotelDB.ExecuteSqlStringAndReturnSingleField("HotelDB.GetHotelContactsFor3", parameters);
                if (rethotel != null)
                    retCount = 1;
            }
            catch (Exception ex)
            {
                //retCount = hotelid;
            }
            return retCount;
        }

        public static List<int> Query_InterestHotel(int HotelID, int InterestID, long UserID, int Distance)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelID", DbType.Int32, HotelID);
            parameters.AddInParameter("@InterestID", DbType.Int32, InterestID);
            parameters.AddInParameter("@distance", DbType.Int32, Distance);
            List<int> hIDs = HotelDB.ExecuteSprocSingle<int>("Query_InterestHotel", parameters);
            return hIDs != null ? hIDs : new List<int>();
        }

        /// <summary>
        /// 获得酒店的设施集合
        /// </summary>
        /// <param name="hotelID"></param>
        /// <returns></returns>
        public static List<HotelFacilityEntity> GetHotelFacilitysByHotelID(int hotelID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelID", DbType.Int32, hotelID);
            return HotelDB.ExecuteSqlString<HotelFacilityEntity>("HotelDB.GetHotelFacilitysByHotelID", parameters);
        }

        #region Top 20 套餐排序


        public static List<AlbumPackageSimpleEntity> GetTopNPackageScreenList(int albumsID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@AlbumsID", DbType.Int32, albumsID);
            List<AlbumPackageSimpleEntity> list = HotelBizDB.ExecuteSqlString<AlbumPackageSimpleEntity>("HotelBizDB.GetTopNPackageScreenList", parameters);
            return list != null ? list : new List<AlbumPackageSimpleEntity>();
        }
        public static List<AlbumPackageSimpleEntity> GetTopNPackageListByAlbumIDs(string albumIDs)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@AlbumsIDs", DbType.String, albumIDs);
            List<AlbumPackageSimpleEntity> list = HotelBizDB.ExecuteSqlString<AlbumPackageSimpleEntity>("HotelBizDB.GetTopNPackageListByAlbumIDs", parameters);
            return list != null ? list : new List<AlbumPackageSimpleEntity>();
        }


        public static List<TopNPackagesEntity> GetTopNPackagesEntityList(TopNPackageSearchParam param, out int count)
        {
            TopNPackageSearchFilter entity = param.Filter;
            if (entity == null)
            {
                entity = new TopNPackageSearchFilter();
            }

            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelName", DbType.String, entity.HotelName);
            parameters.AddInParameter("@HotelID", DbType.Int32, entity.HotelID);
            parameters.AddInParameter("@PackageName", DbType.String, entity.PackageName);
            parameters.AddInParameter("@PackageID", DbType.Int32, entity.PackageID);
            parameters.AddInParameter("@IsValid", DbType.Boolean, entity.IsValid);
            parameters.AddInParameter("@NeedNotSale", DbType.Boolean, entity.NeedNotSale);
            parameters.AddInParameter("@NeedRankRange", DbType.Int32, entity.NeedRankRange);
            parameters.AddInParameter("@AlbumsID", DbType.Int32, entity.AlbumsID);

            parameters.AddInParameter("@VipFirstBuyPackage", DbType.Int32, entity.VipFirstBuyPackage);

            parameters.AddInParameter("@DateStr", DbType.String, entity.DateStr == null ? "" : entity.DateStr);
            parameters.AddInParameter("@GoToDistrictID", DbType.Int32, entity.GoToDistrictId);
            parameters.AddInParameter("@StartDistrictId", DbType.Int32, entity.StartDistrictId);
            
            object obj = HotelBizDB.ExecuteSqlStringAndReturnSingleField("HotelBizDB.GetTopNPackagesEntityListCount", parameters);
            int count1 = 0;
            if (obj != null)
            {
                count1 = Convert.ToInt32(obj);
            }
            count = count1;

            parameters.AddInParameter("@start", DbType.Int32, param.Start);
            parameters.AddInParameter("@end", DbType.Int32, param.PageSize + param.Start);

            List<TopNPackagesEntity> list = new List<TopNPackagesEntity>();
            if (entity.AlbumsID == 10) list = HotelBizDB.ExecuteSqlString<TopNPackagesEntity>("HotelBizDB.GetTopNPackagesEntityList2", parameters);
            else list = HotelBizDB.ExecuteSqlString<TopNPackagesEntity>("HotelBizDB.GetTopNPackagesEntityList", parameters);
            return list != null ? list : new List<TopNPackagesEntity>();
        }
        /// <summary>
        /// 获取分组套餐专辑列表,Package.PackageGroupName相同且不为空，只取价格最低的套餐 
        /// </summary>
        /// <param name="param"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static List<TopNPackagesEntity> GetTopNGroupPackagesEntityList(TopNPackageSearchParam param, out int count)
        {
            TopNPackageSearchFilter entity = param.Filter;
            if (entity == null)
            {
                entity = new TopNPackageSearchFilter();
            }

            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelName", DbType.String, entity.HotelName);
            parameters.AddInParameter("@HotelID", DbType.Int32, entity.HotelID);
            parameters.AddInParameter("@PackageName", DbType.String, entity.PackageName);
            parameters.AddInParameter("@PackageID", DbType.Int32, entity.PackageID);
            parameters.AddInParameter("@IsValid", DbType.Boolean, entity.IsValid);
            parameters.AddInParameter("@NeedNotSale", DbType.Boolean, entity.NeedNotSale);
            parameters.AddInParameter("@NeedRankRange", DbType.Int32, entity.NeedRankRange);
            parameters.AddInParameter("@AlbumsID", DbType.Int32, entity.AlbumsID);

            parameters.AddInParameter("@VipFirstBuyPackage", DbType.Int32, entity.VipFirstBuyPackage);

            parameters.AddInParameter("@DateStr", DbType.String, entity.DateStr == null ? "" : entity.DateStr);
            parameters.AddInParameter("@GoToDistrictID", DbType.Int32, entity.GoToDistrictId);
            parameters.AddInParameter("@StartDistrictId", DbType.Int32, entity.StartDistrictId);

            object obj = HotelBizDB.ExecuteSqlStringAndReturnSingleField("HotelBizDB.GetTopNGroupPackagesEntityListCount", parameters);
            int count1 = 0;
            if (obj != null)
            {
                count1 = Convert.ToInt32(obj);
            }
            count = count1;

            parameters.AddInParameter("@start", DbType.Int32, param.Start);
            parameters.AddInParameter("@end", DbType.Int32, param.PageSize );

            List<TopNPackagesEntity> list = new List<TopNPackagesEntity>();
            if (entity.AlbumsID == 10) list = HotelBizDB.ExecuteSqlString<TopNPackagesEntity>("HotelBizDB.GetTopNGroupPackagesEntityList2", parameters);
            else list = HotelBizDB.ExecuteSqlString<TopNPackagesEntity>("HotelBizDB.GetTopNGroupPackagesEntityList", parameters);
            return list != null ? list : new List<TopNPackagesEntity>();
        }

        #region Top 20 套餐排序 包括搜索

        public static List<TopNPackagesEntity> GetTopNPackagesEntityListByDistrictIds(TopNPackageSearchParam param, out int count)
        {
            TopNPackageSearchFilter entity = param.Filter;
            if (entity == null)
            {
                entity = new TopNPackageSearchFilter();
            }

            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelName", DbType.String, entity.HotelName);
            parameters.AddInParameter("@HotelID", DbType.Int32, entity.HotelID);
            parameters.AddInParameter("@PackageName", DbType.String, entity.PackageName);
            parameters.AddInParameter("@PackageID", DbType.Int32, entity.PackageID);
            parameters.AddInParameter("@IsValid", DbType.Boolean, entity.IsValid);
            parameters.AddInParameter("@NeedRankRange", DbType.Int32, entity.NeedRankRange);
            parameters.AddInParameter("@AlbumsID", DbType.Int32, entity.AlbumsID);
            parameters.AddInParameter("@DistrictIds", DbType.String, entity.DistrictIds);

            parameters.AddInParameter("@VipFirstBuyPackage", DbType.Int32, entity.VipFirstBuyPackage);

            parameters.AddInParameter("@DateStr", DbType.String, entity.DateStr);
            parameters.AddInParameter("@GoToDistrictID", DbType.Int32, entity.GoToDistrictId);

            object obj = HotelBizDB.ExecuteSqlStringAndReturnSingleField("HotelBizDB.GetTopNPackagesEntityListByDistrictIdsCount", parameters);
            int count1 = 0;
            if (obj != null)
            {
                count1 = Convert.ToInt32(obj);
            }
            count = count1;

            parameters.AddInParameter("@start", DbType.Int32, param.Start);
            parameters.AddInParameter("@end", DbType.Int32, param.PageSize + param.Start);

            List<TopNPackagesEntity> list = new List<TopNPackagesEntity>();
            if (entity.AlbumsID == 10) list = HotelBizDB.ExecuteSqlString<TopNPackagesEntity>("HotelBizDB.GetTopNPackagesEntityList2", parameters);
            else list = HotelBizDB.ExecuteSqlString<TopNPackagesEntity>("HotelBizDB.GetTopNPackagesEntityListByDistrictIds", parameters);
            return list != null ? list : new List<TopNPackagesEntity>();
        }

        public static List<TopNPackagesEntity> GetTopNGroupPackagesEntityListByDistrictIds(TopNPackageSearchParam param, out int count)
        {
            TopNPackageSearchFilter entity = param.Filter;
            if (entity == null)
            {
                entity = new TopNPackageSearchFilter();
            }

            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelName", DbType.String, entity.HotelName);
            parameters.AddInParameter("@HotelID", DbType.Int32, entity.HotelID);
            parameters.AddInParameter("@PackageName", DbType.String, entity.PackageName);
            parameters.AddInParameter("@PackageID", DbType.Int32, entity.PackageID);
            parameters.AddInParameter("@IsValid", DbType.Boolean, entity.IsValid);
            parameters.AddInParameter("@NeedRankRange", DbType.Int32, entity.NeedRankRange);
            parameters.AddInParameter("@AlbumsID", DbType.Int32, entity.AlbumsID);
            parameters.AddInParameter("@DistrictIds", DbType.String, entity.DistrictIds);

            parameters.AddInParameter("@VipFirstBuyPackage", DbType.Int32, entity.VipFirstBuyPackage);

            parameters.AddInParameter("@DateStr", DbType.String, entity.DateStr);
            parameters.AddInParameter("@GoToDistrictID", DbType.Int32, entity.GoToDistrictId);
            parameters.AddInParameter("@StartDistrictId", DbType.Int32, entity.StartDistrictId);

            object obj = HotelBizDB.ExecuteSqlStringAndReturnSingleField("HotelBizDB.GetTopNGroupPackagesEntityListByDistrictIdsCount", parameters);
            int count1 = 0;
            if (obj != null)
            {
                count1 = Convert.ToInt32(obj);
            }
            count = count1;

            parameters.AddInParameter("@start", DbType.Int32, param.Start);
            parameters.AddInParameter("@end", DbType.Int32, param.PageSize );

            List<TopNPackagesEntity> list = new List<TopNPackagesEntity>();
            if (entity.AlbumsID == 10) list = HotelBizDB.ExecuteSqlString<TopNPackagesEntity>("HotelBizDB.GetTopNGroupPackagesEntityList2", parameters);
            else list = HotelBizDB.ExecuteSqlString<TopNPackagesEntity>("HotelBizDB.GetTopNGroupPackagesEntityListByDistrictIds", parameters);
            return list != null ? list : new List<TopNPackagesEntity>();
        }

        public static List<TopNPackagesEntity> GetTopNPackagesAddSearchEntityList(TopNPackageSearchParam param, out int count)
        {
            TopNPackageSearchFilter entity = param.Filter;
            if (entity == null)
            {
                entity = new TopNPackageSearchFilter();
            }

            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelName", DbType.String, entity.HotelName);
            parameters.AddInParameter("@HotelID", DbType.Int32, entity.HotelID);
            parameters.AddInParameter("@PackageName", DbType.String, entity.PackageName);
            parameters.AddInParameter("@PackageID", DbType.Int32, entity.PackageID);
            parameters.AddInParameter("@IsValid", DbType.Boolean, entity.IsValid);
            parameters.AddInParameter("@NeedRankRange", DbType.Int32, entity.NeedRankRange);
            parameters.AddInParameter("@AlbumsID", DbType.Int32, entity.AlbumsID);
            parameters.AddInParameter("@lat", DbType.Decimal, param.lat);
            parameters.AddInParameter("@lng", DbType.Decimal, param.lng);
            parameters.AddInParameter("@geoScopeType", DbType.Int32, param.geoScopeType);
            parameters.AddInParameter("@districtID", DbType.Int32, param.districtID);

            parameters.AddInParameter("@VipFirstBuyPackage", DbType.Int32, entity.VipFirstBuyPackage);

            object obj = HotelBizDB.ExecuteSqlStringAndReturnSingleField("HotelBizDB.GetTopNPackagesEntityListWithInCount", parameters);
            int count1 = 0;
            if (obj != null)
            {
                count1 = Convert.ToInt32(obj);
            }
            count = count1;

            parameters.AddInParameter("@start", DbType.Int32, param.Start);
            parameters.AddInParameter("@end", DbType.Int32, param.PageSize + param.Start);

            List<TopNPackagesEntity> list = new List<TopNPackagesEntity>();
            if (entity.AlbumsID == 10) list = HotelBizDB.ExecuteSqlString<TopNPackagesEntity>("HotelBizDB.GetTopNPackagesEntityWithInList2", parameters);
            else list = HotelBizDB.ExecuteSqlString<TopNPackagesEntity>("HotelBizDB.GetTopNPackagesEntityWithInList", parameters);
            return list != null ? list : new List<TopNPackagesEntity>();
        }
        #endregion

        #region 专辑套餐对应城市
        public static List<HotelDestnInfo> GetHotelDestnInfo(int albumsID)
        {
            
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@AlbumsID", DbType.Int32, albumsID);
            //parameters.AddInParameter("@InChina", DbType.Boolean, inChina);
            List<HotelDestnInfo> list = new List<HotelDestnInfo>();
            list = HotelBizDB.ExecuteSqlString<HotelDestnInfo>("HotelBizDB.GetHotelDestnInfo", parameters);
            return list != null ? list : new List<HotelDestnInfo>();
        }
        #endregion


        #region 专辑套餐对应城市
        public static List<HotelDestnInfo> GetHotelDestWithIn(int albumsID,float lat, float lng)
        {

            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@AlbumsID", DbType.Int32, albumsID);
            parameters.AddInParameter("@lat", DbType.Double, lat);
            parameters.AddInParameter("@lng", DbType.Double, lng);
            List<HotelDestnInfo> list = new List<HotelDestnInfo>();
            list = HotelBizDB.ExecuteSqlString<HotelDestnInfo>("HotelBizDB.GetHotelDestnInfoWithIn", parameters);
            return list != null ? list : new List<HotelDestnInfo>();
        }
        #endregion


        public static List<TopNPackagesEntity> GetTopNPackagesEntityList4HotelAlbums(TopNPackageSearchParam param, out int count)
        {
            TopNPackageSearchFilter entity = param.Filter;
            if (entity == null)
            {
                entity = new TopNPackageSearchFilter();
            }

            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelName", DbType.String, entity.HotelName);
            parameters.AddInParameter("@HotelID", DbType.Int32, entity.HotelID);
            parameters.AddInParameter("@IsValid", DbType.Boolean, entity.IsValid); 
            parameters.AddInParameter("@NeedRankRange", DbType.Int32, entity.NeedRankRange);
            parameters.AddInParameter("@AlbumsID", DbType.Int32, entity.AlbumsID);

            object obj = HotelBizDB.ExecuteSqlStringAndReturnSingleField("HotelBizDB.GetTopNPackagesEntityListCount4HotelAlbums", parameters);
            int count1 = 0;
            if (obj != null)
            {
                count1 = Convert.ToInt32(obj);
            }
            count = count1;

            parameters.AddInParameter("@start", DbType.Int32, param.Start);
            parameters.AddInParameter("@end", DbType.Int32, param.PageSize + param.Start);

            List<TopNPackagesEntity> list = HotelBizDB.ExecuteSqlString<TopNPackagesEntity>("HotelBizDB.GetTopNPackagesEntityList4HotelAlbums", parameters);
            return list != null ? list : new List<TopNPackagesEntity>();
        }

        public static List<TopNPackagesEntity> GetTopNGroupPackagesEntityList4HotelAlbums(TopNPackageSearchParam param, out int count)
        {
            TopNPackageSearchFilter entity = param.Filter;
            if (entity == null)
            {
                entity = new TopNPackageSearchFilter();
            }

            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelName", DbType.String, entity.HotelName);
            parameters.AddInParameter("@HotelID", DbType.Int32, entity.HotelID);
            parameters.AddInParameter("@IsValid", DbType.Boolean, entity.IsValid);
            parameters.AddInParameter("@NeedRankRange", DbType.Int32, entity.NeedRankRange);
            parameters.AddInParameter("@AlbumsID", DbType.Int32, entity.AlbumsID);
            parameters.AddInParameter("@StartDistrictId", DbType.Int32, entity.StartDistrictId);
            parameters.AddInParameter("@PackageID", DbType.Int32, entity.PackageID);

            object obj = HotelBizDB.ExecuteSqlStringAndReturnSingleField("HotelBizDB.GetTopNGroupPackagesEntityListCount4HotelAlbums", parameters);
            int count1 = 0;
            if (obj != null)
            {
                count1 = Convert.ToInt32(obj);
            }
            count = count1;

            parameters.AddInParameter("@start", DbType.Int32, param.Start);
            parameters.AddInParameter("@end", DbType.Int32, param.PageSize + param.Start);

            List<TopNPackagesEntity> list = HotelBizDB.ExecuteSqlString<TopNPackagesEntity>("HotelBizDB.GetTopNGroupPackagesEntityList4HotelAlbums", parameters);
            return list != null ? list : new List<TopNPackagesEntity>();
        }
        public static int InsertOrUpdateTopNPackage(TopNPackagesEntity tnpe)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ID", DbType.Int32, tnpe.ID);
            parameters.AddInParameter("@PID", DbType.Int32, tnpe.PID);
            parameters.AddInParameter("@Rank", DbType.Int32, tnpe.Rank);
            parameters.AddInParameter("@Creator", DbType.Int64, tnpe.Creator);
            parameters.AddInParameter("@Updator", DbType.Int64, tnpe.Updator);
            parameters.AddInParameter("@IsValid", DbType.Boolean, tnpe.IsValid);
            parameters.AddInParameter("@RecomemndWord", DbType.String, tnpe.RecomemndWord);
            parameters.AddInParameter("@RecomemndWord2", DbType.String, tnpe.RecomemndWord2);
            parameters.AddInParameter("@Title", DbType.String, tnpe.Title);

            parameters.AddInParameter("@MarketPrice", DbType.Int32, tnpe.MarketPrice);
            parameters.AddInParameter("@CoverPicSUrl", DbType.String, tnpe.CoverPicSUrl);
            parameters.AddInParameter("@AlbumsID", DbType.Int32, tnpe.AlbumsID == 0 ? 1 : tnpe.AlbumsID);
            parameters.AddInParameter("@HotelID", DbType.Int32, tnpe.HotelID);
            parameters.AddInParameter("@RecomendPicShortNames", DbType.String, tnpe.RecomendPicShortNames);
            parameters.AddInParameter("@RecomendPicShortNames2", DbType.String, tnpe.RecomendPicShortNames2);
            parameters.AddInParameter("@Brief", DbType.String, tnpe.Brief);

            Object obj = HotelBizDB.ExecuteSprocAndReturnSingleField("SP_TopNPackages_InsertOrUpdate", parameters);
            return Convert.ToInt32(obj);
        }

        public static List<TypeAndPrice> GetPackageTypeAndPriceList(List<int> pids)
        {
            if (pids == null || pids.Count == 0)
            {
                return new List<TypeAndPrice>();
            }
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@PIDList", DbType.String, string.Join(",", pids));

            List<TypeAndPrice> list = HotelDB.ExecuteSqlString<TypeAndPrice>("HotelDB.GetPackageTypeAndPriceList", parameters);

            DateTime specialDate = DateTime.Now;
            int day = (int)specialDate.DayOfWeek;
            if (day > 0 && day < 6)
            {
                //这周六
                specialDate.AddDays(6 - day);
            }
            else
            {
                //下周六
                specialDate.AddDays(day == 0 ? 6 : 7);
            }
            List<TypeAndPrice> list2 = GetPackageTypeAndPriceListByDate(pids, specialDate);
            foreach (var item in list2)
            {
                list.ForEach((_) =>
                {
                    if (_.Type == 6 && _.PID == item.PID)
                    {
                        _.Price = item.Price;//以特定日期价格为准
                    }
                });
            }
            return list != null ? list : new List<TypeAndPrice>();
        }

        public static List<TypeAndPrice> GetPackageTypeAndPriceListByDate(List<int> pids, DateTime dt)
        {
            if (pids == null || pids.Count == 0)
            {
                return new List<TypeAndPrice>();
            }
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@PIDList", DbType.String, string.Join(",", pids));
            parameters.AddInParameter("@specialDate", DbType.String, dt.ToString("yyyy-MM-dd"));

            List<TypeAndPrice> list = HotelDB.ExecuteSqlString<TypeAndPrice>("HotelDB.GetPackageTypeAndPriceListByDate", parameters);
            return list != null ? list : new List<TypeAndPrice>();
        }

        public static List<TopNPackageItem> GetTopNPackageContent(List<int> pids)
        {
            if (pids == null || pids.Count == 0)
            {
                return new List<TopNPackageItem>();
            }
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@PIDList", DbType.String, string.Join(",", pids));

            List<TopNPackageItem> list = HotelDB.ExecuteSqlString<TopNPackageItem>("HotelDB.GetTopNPackageContent", parameters);
            return list != null ? list : new List<TopNPackageItem>();
        }
        public static List<TopNPackageItem> GetPackageContentNofilterPackageState(List<int> pids)
        {
            if (pids == null || pids.Count == 0)
            {
                return new List<TopNPackageItem>();
            }
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@PIDList", DbType.String, string.Join(",", pids));

            List<TopNPackageItem> list = HotelDB.ExecuteSqlString<TopNPackageItem>("HotelDB.GetPackageContentNofilterPackageState", parameters);
            return list != null ? list : new List<TopNPackageItem>();
        }

        public static int UpdateTopNPackageBatch(bool IsValid, long Updator)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@Updator", DbType.Int64, Updator);
            parameters.AddInParameter("@IsValid", DbType.Boolean, IsValid);

            Object obj = HotelBizDB.ExecuteSprocAndReturnSingleField("SP_TopNPackages_BatchUpdateIsValid", parameters);
            return Convert.ToInt32(obj);
        }

        public static int DeleteTopNPackage(int id)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@id", DbType.Int32, id);

            Object obj = HotelBizDB.ExecuteSprocAndReturnSingleField("SP_TopNPackages_Delete", parameters);
            return Convert.ToInt32(obj);
        }

        /// <summary>
        /// 计算两个地区之间的距离  单位米
        /// </summary>
        /// <param name="districtID1"></param>
        /// <param name="districtID2"></param>
        /// <returns></returns>
        public static int CalculateDistrictDistance(int districtID1, int districtID2)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@districtID1", DbType.Int32, districtID1);
            parameters.AddInParameter("@districtID2", DbType.Int32, districtID2);
            parameters.AddInParameter("@lat", DbType.Single, 0.0);
            parameters.AddInParameter("@lon", DbType.Single, 0.0);

            Object obj = DestDB.ExecuteSprocAndReturnSingleField("SP_Calculate2DistrictDistance", parameters);
            return Convert.ToInt32(obj);
        }

        public static int CalculateUserDistrictDistance(double userLat, double userLon, int districtID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@lat", DbType.Single, userLat);
            parameters.AddInParameter("@lon", DbType.Single, userLon);

            parameters.AddInParameter("@districtID1", DbType.Int32, 0);
            parameters.AddInParameter("@districtID2", DbType.Int32, districtID);

            Object obj = DestDB.ExecuteSprocAndReturnSingleField("SP_Calculate2DistrictDistance", parameters);
            return Convert.ToInt32(obj);
        }

        public static List<ArounDistrictEntity> CalculateNearDistrictByDistance(int districtID, float lat, float lon, int distance = 300000)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@lat", DbType.Double, lat);
            parameters.AddInParameter("@lon", DbType.Double, lon);
            parameters.AddInParameter("@districtID", DbType.Int32, districtID);
            parameters.AddInParameter("@maxDistance", DbType.Int32, distance);

            DataSet ds = DestDB.ExecuteSp("SP_CalculateNearDistrictByDistance", parameters);
            List<ArounDistrictEntity> aroundList = new List<ArounDistrictEntity>();
            if (ds != null && ds.Tables != null && ds.Tables.Count != 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    aroundList.Add(new ArounDistrictEntity()
                    {
                        DistrictID = int.Parse(row["DistrictID"].ToString()),
                        DistrictName = row["DistrictName"].ToString(),
                        Distance = float.Parse(row["Distance"].ToString()),
                        HotelCount = int.Parse(row["HotelCount"].ToString()),
                        Lat = double.Parse(row["lat"].ToString()),
                        Lon = double.Parse(row["lon"].ToString())
                    });
                }
            }
            return aroundList;
        }

        /// <summary>
        /// 批量获取酒店列表过滤标签名称ID
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static List<FilterDicEntity> GetHotelListFilterTagInfos(SearchHotelListFilterTagInfoParam param)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@zoneids", DbType.String, param.zoneids == null || !param.zoneids.Any() ? "" : string.Join(",", param.zoneids));
            parameters.AddInParameter("@interestids", DbType.String, param.interestids == null || !param.interestids.Any() ? "" : string.Join(",", param.interestids));
            parameters.AddInParameter("@classids", DbType.String, param.classids == null || !param.classids.Any() ? "" : string.Join(",", param.classids));
            parameters.AddInParameter("@facilitys", DbType.String, param.facilitys == null || !param.facilitys.Any() ? "" : string.Join(",", param.facilitys));
            parameters.AddInParameter("@featuredtrees", DbType.String, param.featuredtreeids == null || !param.featuredtreeids.Any() ? "" : string.Join(",", param.featuredtreeids));

            //parameters.AddInParameter("@triptypeids", DbType.String, param.triptypeids == null || param.triptypeids.Count() == 0 ? "" : string.Join(",", param.triptypeids));

            return HotelDB.ExecuteSqlString<FilterDicEntity>("HotelDB.GetHotelListFilterTagInfos", parameters);
        }
        #endregion

        #region 开放点评
        internal static int InsertUserAddHotels(string hotelName, long userID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ID", DbType.Int32, 0);
            parameters.AddInParameter("@HotelName", DbType.String, hotelName);
            parameters.AddInParameter("@HotelID", DbType.Int32, 0);
            parameters.AddInParameter("@Creator", DbType.Int64, userID);
            parameters.AddInParameter("@Updator", DbType.Int64, userID);

            Object obj = HotelDB.ExecuteSprocAndReturnSingleField("SP_UserAddHotels_InsertOrUpdate", parameters);
            return Convert.ToInt32(obj);
        }

        internal static int InsertUserAddHotelCommentRel(int addHotelID, int commentID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@UserAddHotelID", DbType.Int32, addHotelID);
            parameters.AddInParameter("@CommentID", DbType.Int32, commentID);
            Object obj = HotelDB.ExecuteSprocAndReturnSingleField("SP_UserAddHotelCommentRel_Insert", parameters);
            return Convert.ToInt32(obj);
        }

        internal static List<CommentAddHotelEntity> GetUserAddHotelByComment(int commentID, long userID = 0)
        {
            try
            {
                var parameters = new DBParameterCollection();
                parameters.AddInParameter("@commentID", DbType.Int32, commentID);
                parameters.AddInParameter("@userID", DbType.Int64, userID);
                List<CommentAddHotelEntity> list = HotelDB.ExecuteSqlString<CommentAddHotelEntity>("HotelDB.GetUserAddHotelByComment", parameters);
                return list != null && list.Count != 0 ? list : new List<CommentAddHotelEntity>();
            }
            catch (Exception ex)
            {
                HJD.HotelServices.Implement.Business.LogHelper.WriteLog("由点评获取添加酒店出现异常" + ex.Message + " " + ex.StackTrace);
                return new List<CommentAddHotelEntity>();
            }
        }

        internal static int GetUserAddHotelForComment(string hotelName, long userID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@hotelName", DbType.String, hotelName);
            parameters.AddInParameter("@userID", DbType.Int64, userID);
            object addHotelIDobj = HotelDB.ExecuteSqlStringAndReturnSingleField("HotelDB.GetUserAddHotelForComment", parameters);
            return addHotelIDobj != DBNull.Value ? Convert.ToInt32(addHotelIDobj) : 0;
        }
        #endregion

        #region 更新酒店与区域之间的对应关系
        /// <summary>
        /// 更新酒店区域关联信息
        /// </summary>
        /// <param name="hotelID"></param>
        /// <param name="zoneID"></param>
        /// <returns></returns>
        public static string UpdateHotelDistrictZoneRel(int hotelID, int zoneID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@DistrictZoneID", DbType.Int32, zoneID);
            parameters.AddInParameter("@HotelID", DbType.Int32, hotelID);

            object obj = HotelDB.ExecuteSprocAndReturnSingleField("SP_HotelDistrictZoneRel_Update", parameters);
            return obj.ToString();//作为下一步更新hotelfilter的数据
        }

        /// <summary>
        /// 更新区域关联携程酒店
        /// </summary>
        /// <param name="districtZoneName"></param>
        /// <returns></returns>
        public static string GetDistrictZoneMapOtherZoneHotel(int districtZoneID, int type = 1)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@type", DbType.Int32, type);
            parameters.AddInParameter("@districtZoneID", DbType.Int32, districtZoneID);

            object obj = HotelDB.ExecuteSqlStringAndReturnSingleField("HotelDB.GetDistrictZoneMapOtherZoneHotel", parameters);
            return obj.ToString();//作为下一步更新hotelfilter或hoteldistrictzonerel的数据
        }

        /// <summary>
        /// 插入区域与XC关联的酒店信息
        /// </summary>
        /// <param name="zoneId"></param>
        /// <param name="hotelIds"></param>
        /// <param name="deleteOld"></param>
        /// <returns></returns>
        public static int InsertHotelDistrictZoneRel(int zoneId, string hotelIds, bool deleteOld = false)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@hotelID", DbType.Int32, 0);
            parameters.AddInParameter("@zoneIDs", DbType.String, "");
            parameters.AddInParameter("@zoneID", DbType.Int32, zoneId);
            parameters.AddInParameter("@hotelIDs", DbType.String, hotelIds);
            parameters.AddInParameter("@deleteOld", DbType.Boolean, deleteOld);

            HotelDB.ExecuteNonQuery("SP_HotelDistrictZoneRel_Insert", parameters);
            return 0;
        }

        /// <summary>
        /// 酒店ID大于0 以酒店ID为基准 更新HotelFilter;区域ID大于0 以区域ID为基准更新酒店区域关联关系
        /// </summary>
        /// <param name="hotelID"></param>
        /// <param name="zoneID"></param>
        /// <param name="type"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        public static int UpdateHotelFilter(int hotelID, int zoneID, int type, IEnumerable<int> ids)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@hotelID", DbType.Int32, hotelID);
            parameters.AddInParameter("@zoneID", DbType.Int32, zoneID);
            parameters.AddInParameter("@type", DbType.Int32, zoneID > 0 && hotelID == 0 ? 18 : type);//zone大于0且酒店ID等于0时 类型一定是酒店区域关联关系
            parameters.AddInParameter("@ids", DbType.String, ids != null && ids.Count() > 0 ? string.Join(",", ids) : "");
            HotelDB.ExecuteNonQuery("SP_HotelFilter_UpdateOneHotel", parameters);
            return 0;
        }

        /// <summary>
        /// 主题特色关联酒店数量（默认按酒店数量降序排列）
        /// </summary>
        /// <param name="interestType"></param>
        /// <returns></returns>
        public static IEnumerable<InterestHotelCountEntity> GetInterestHotelCountList(int interestType)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@interestType", DbType.Int32, interestType);
            return HotelDB.ExecuteSqlString<InterestHotelCountEntity>("HotelDB.GetInterestHotelCountList", parameters);
        }

        /// <summary>
        /// 批量获取酒店基本信息
        /// </summary>
        /// <param name="hotelIDs"></param>
        /// <returns></returns>
        public static IEnumerable<HotelBasicInfo> GetHotelBasicInfoList(IEnumerable<int> hotelIDs)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@hotelIDs", DbType.String, string.Join(",", hotelIDs));
            return HotelDB.ExecuteSqlString<HotelBasicInfo>("HotelDB.GetHotelBasicInfoList", parameters);
        }
        #endregion

        public static List<PointsEntity> GetPointListByTypeIDAndUserID(int typeid, IEnumerable<long> ffRelIds)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@TypeID", DbType.Int32, typeid);
            parameters.AddInParameter("@items", DbType.String, string.Join(",", ffRelIds));
            return HotelDB.ExecuteSqlString<PointsEntity>("HotelBizDB.GetPointListByTypeIDAndUserID", parameters);
        }
        public static List<PointsEntity> GetPointListByTypeIDAndBusinessID(int typeid, IEnumerable<long> ffRelIds)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@TypeID", DbType.Int32, typeid);
            parameters.AddInParameter("@items", DbType.String, string.Join(",", ffRelIds));
            return HotelDB.ExecuteSqlString<PointsEntity>("HotelBizDB.GetPointListByTypeIDAndBusinessID", parameters);
        }

        /// <summary>
        /// 获取酒店相关主题 已发布的
        /// </summary>
        /// <param name="hotelID"></param>
        /// <returns></returns>
        public static IEnumerable<InterestEntity> GetInterestListByHotel(int hotelID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@hotelID", DbType.Int32, hotelID);
            return HotelDB.ExecuteSqlString<InterestEntity>("HotelDB.GetInterestListByHotel", parameters);
        }

        public static IEnumerable<PointsEntity> GetExpiredPoints(int typeID = 0)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@typeID", DbType.Int32, typeID);
            parameters.AddInParameter("@curDate", DbType.DateTime, DateTime.Now.Date);
            return HotelBizDB.ExecuteSqlString<PointsEntity>("HotelBizDB.GetExpiredPoints", parameters);
        }

        public static IEnumerable<InspectorRefHotel> GetNeedWriteCommentInspectorRefHotel(int maxDay)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@deadLine", DbType.DateTime, DateTime.Now.AddDays(0 - maxDay).Date);
            return HotelBizDB.ExecuteSqlString<InspectorRefHotel>("HotelBizDB.GetNeedWriteCommentInspectorRefHotel", parameters);
        }

        public static HotelMapBasicInfo GetHotelMapInfo(int hotelID)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@hotelID", DbType.Int32, hotelID);
            List<HotelMapBasicInfo> info = HotelDB.ExecuteSqlString<HotelMapBasicInfo>("HotelDB.GetHotelMapInfo", parameters);
            return info == null || info.Count == 0 ? new HotelMapBasicInfo() : info.First();
        }

        public static IEnumerable<InspectorRefHotel> GetInspectorRefHotelByUserID(long userID, int state = 0)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@userID", DbType.Int64, userID);
            parameters.AddInParameter("@state", DbType.Int32, state);
            return HotelBizDB.ExecuteSqlString<InspectorRefHotel>("HotelBizDB.GetInspectorRefHotelByUserID", parameters);
        }

        public static string GetDistrictZonePicSUrl(int districtId)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@districtId", DbType.Int32, districtId);
            object obj = HotelDB.ExecuteSqlStringAndReturnSingleField("HotelDB.GetDistrictZonePicSUrl", parameters);
            return obj == null ? "" : obj.ToString();
        }

        #region OtaPriceCtrip 操作

        /// <summary>
        /// 删除指定酒店指定日期的携程抓取套餐数据
        /// </summary>
        /// <param name="hotelID">zmjiudian酒店ID</param>
        /// <param name="night"></param>
        /// <returns></returns>
        public static bool DeleteOtaPriceCtrip(int hotelID, DateTime night)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelId", DbType.Int32, hotelID);
            parameters.AddInParameter("@Night", DbType.DateTime, night);
            return HotelDB.ExecuteNonQueryWithSqlString("HotelDB.DeleteOtaPriceCtrip", parameters) > 0;
        }

        public static bool InsertOtaPriceCtrip(OTAPriceCtrip priceEntity)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelId", DbType.Int64, priceEntity.HotelID);
            parameters.AddInParameter("@HotelOriId", DbType.Int64, priceEntity.HotelOriId);
            parameters.AddInParameter("@Night", DbType.Date, priceEntity.Night);
            parameters.AddInParameter("@RoomTypeID", DbType.Int64, priceEntity.RoomTypeID);
            parameters.AddInParameter("@RoomTypeName", DbType.String, priceEntity.RoomTypeName);
            parameters.AddInParameter("@RoomID", DbType.String, priceEntity.RoomID);
            parameters.AddInParameter("@RoomName", DbType.String, priceEntity.RoomName);
            parameters.AddInParameter("@Price", DbType.Int32, priceEntity.Price);
            parameters.AddInParameter("@CurrencyType", DbType.String, priceEntity.CurrencyType);
            parameters.AddInParameter("@Promotion", DbType.Int32, priceEntity.Promotion);
            parameters.AddInParameter("@PromotionInfo", DbType.String, priceEntity.PromotionInfo);
            parameters.AddInParameter("@ReturnCash", DbType.Int32, priceEntity.ReturnCash);
            parameters.AddInParameter("@IsGift", DbType.Int32, priceEntity.IsGift);
            parameters.AddInParameter("@GiftInfo", DbType.String, priceEntity.GiftInfo);
            parameters.AddInParameter("@BedType", DbType.String, priceEntity.BedType);
            parameters.AddInParameter("@Area", DbType.String, priceEntity.Area);
            parameters.AddInParameter("@Breakfast", DbType.String, priceEntity.Breakfast);
            parameters.AddInParameter("@Broadband", DbType.String, priceEntity.Broadband);
            parameters.AddInParameter("@PayType", DbType.String, priceEntity.PayType);
            parameters.AddInParameter("@Policy", DbType.String, priceEntity.Policy);
            parameters.AddInParameter("@PolicyTip", DbType.String, priceEntity.PolicyTip);
            parameters.AddInParameter("@PackageSummary", DbType.String, priceEntity.PackageSummary);
            parameters.AddInParameter("@CreateTime", DbType.DateTime, priceEntity.CreateTime);
            return HotelDB.ExecuteNonQuery("SP_OTAPriceCtrip_InsertOrUpdate", parameters) > 0;
        }

        /// <summary>
        /// 获取指定酒店指定日期的携程抓取套餐数据
        /// </summary>
        /// <param name="hotelID">zmjiudian酒店ID</param>
        /// <returns></returns>
        public static List<OTAPriceCtrip> GetOtaPriceCtrip(int hotelID, DateTime checkIn, DateTime checkOut)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelId", DbType.Int32, hotelID);
            parameters.AddInParameter("@CheckIn", DbType.DateTime, checkIn);
            parameters.AddInParameter("@CheckOut", DbType.DateTime, checkOut);
            return HotelDB.ExecuteSqlString<OTAPriceCtrip>("HotelDB.GetOtaPriceCtrip", parameters);
        }

        /// <summary>
        /// 根据携程抓取套餐ID获取套餐对象
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static OTAPriceCtrip GetOtaPriceCtripByPid(int pid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ID", DbType.Int32, pid);
            var list = HotelDB.ExecuteSqlString<OTAPriceCtrip>("HotelDB.GetOtaPriceCtripByPid", parameters);
            if (list != null && list.Count > 0)
            {
                return list.First();
            }

            return new OTAPriceCtrip();
        }

        /// <summary>
        /// 插入携程RoomId（套餐ID）映射关系的Idx记录
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        public static bool InsertOtaPackageIdxCtrip(string roomId)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@RoomID", DbType.String, roomId);
            return HotelDB.ExecuteNonQuery("SP_OTAPackageIdxCtrip_InsertOrUpdate", parameters) > 0;
        }

        /// <summary>
        /// 根据携程RoomId（套餐ID）得到映射关系的Idx
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static int GetOtaPackageIdxCtrip(string roomId)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@RoomID", DbType.String, roomId);
            var obj = HotelDB.ExecuteSqlStringAndReturnSingleField("HotelDB.GetOtaPackageIdxCtrip", parameters);
            if (obj != null)
            {
                try
                {
                    return Convert.ToInt32(obj);
                }
                catch (Exception ex)
                {

                }
            }

            return 0;
        }

        #endregion

        #region 添加浏览 分享记录

        /// <summary>
        /// 插入点评浏览记录
        /// </summary>
        /// <param name="cbre"></param>
        /// <returns></returns>
        internal static int InsertBrowsingRecord(BrowsingRecordEntity browsing)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@BusinessType", DbType.Int32, browsing.BusinessType);
            parameters.AddInParameter("@BusinessID", DbType.Int64, browsing.BusinessID);
            parameters.AddInParameter("@Visitor", DbType.Int64, browsing.Visitor);
            parameters.AddInParameter("@TerminalType", DbType.Int32, browsing.TerminalType);
            parameters.AddInParameter("@ClientIP", DbType.String, browsing.ClientIP);
            parameters.AddInParameter("@SessionID", DbType.String, browsing.SessionID);
            parameters.AddInParameter("@OpenID", DbType.String, browsing.OpenID);

            parameters.AddInParameter("@AppVer", DbType.String, browsing.AppVer);

            object obj = HotelBizDB.ExecuteSprocAndReturnSingleField("SP_BrowsingRecord_Insert", parameters);
            return Convert.ToInt32(obj);
        }

        /// <summary>
        /// 插入用户搜索记录
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        internal static int InsertSearchRecord(SearchRecordEntity search)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@BusinessType", DbType.Int32, search.BusinessType);
            parameters.AddInParameter("@BusinessID", DbType.Int64, search.BusinessID);
            parameters.AddInParameter("@OptionUser", DbType.Int64, search.OptionUser);
            parameters.AddInParameter("@TerminalType", DbType.Int32, search.TerminalType);
            parameters.AddInParameter("@ClientIP", DbType.String, search.ClientIP);
            parameters.AddInParameter("@SessionID", DbType.String, search.SessionID);
            parameters.AddInParameter("@OpenID", DbType.String, search.OpenID);
            parameters.AddInParameter("@AppVer", DbType.String, search.AppVer);

            object obj = HotelBizDB.ExecuteSprocAndReturnSingleField("SP_SearchRecord_Insert", parameters);
            return Convert.ToInt32(obj);
        }

        /// <summary>
        /// 插入点评分享记录
        /// </summary>
        /// <param name="csre"></param>
        /// <returns></returns>
        internal static int InsertShareRecord(ShareRecordEntity share)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@BusinessType", DbType.Int32, share.BusinessType);
            parameters.AddInParameter("@BusinessID", DbType.Int64, share.BusinessID);
            parameters.AddInParameter("@ShareUser", DbType.Int64, share.ShareUser);
            parameters.AddInParameter("@TerminalType", DbType.Int32, share.TerminalType);
            parameters.AddInParameter("@ClientIP", DbType.String, share.ClientIP);
            parameters.AddInParameter("@SessionID", DbType.String, share.SessionID);
            parameters.AddInParameter("@OpenID", DbType.String, share.OpenID);

            parameters.AddInParameter("@AppVer", DbType.String, share.AppVer);

            object obj = HotelBizDB.ExecuteSprocAndReturnSingleField("SP_ShareRecord_Insert", parameters);
            return Convert.ToInt32(obj);
        }

        internal static List<ShareRecordEntity> GetShareRecordList(CommonRecordQueryParam param)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@businessId", DbType.Int32, param.businessId);
            parameters.AddInParameter("@businessType", DbType.Int32, param.businessType);
            parameters.AddInParameter("@userId", DbType.Int64, param.userId);

            parameters.AddInParameter("@start", DbType.Int32, param.start);
            parameters.AddInParameter("@count", DbType.Int32, param.count);

            return HotelBizDB.ExecuteSqlString<ShareRecordEntity>("HotelDB.GetShareRecordList", parameters);
        }
        #endregion

        #region 微信酒店列表

        public static List<int> GetHotelListByDistrict(int districtId)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@DistrictId", DbType.Int32, districtId);
            return HotelDB.ExecuteSqlSingle<int>("HotelDB.GetHotelListByDistrict", parameters);
        }

        public static List<int> GetHotelListByDistrictInterest(int districtId, int interestId)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@DistrictId", DbType.Int32, districtId);
            parameters.AddInParameter("@InterestId", DbType.Int32, interestId);
            return HotelDB.ExecuteSqlSingle<int>("HotelDB.GetHotelListByDistrictInterest", parameters);
        }

        public static List<int> GetHotelListByInterest(int interestId)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@InterestId", DbType.Int32, interestId);
            return HotelDB.ExecuteSqlSingle<int>("HotelDB.GetHotelListByInterest", parameters);
        }

        public static List<string> GetProvinceListByInterest(int interestId)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@InterestId", DbType.Int32, interestId);
            return HotelDB.ExecuteSqlSingle<string>("HotelDB.GetProvinceListByInterest", parameters);
        }

        public static List<string> GetProvinceListInChina()
        {
            var parameters = new DBParameterCollection();
            return HotelDB.ExecuteSqlSingle<string>("HotelDB.GetProvinceListInChina", parameters);
        }

        public static List<string> GetProvinceListUnChina()
        {
            var parameters = new DBParameterCollection();
            return HotelDB.ExecuteSqlSingle<string>("HotelDB.GetProvinceListUnChina", parameters);
        }

        #endregion

        public static List<HotelFilterColEntity> GetManyHotelFilterCols(IEnumerable<int> hotelIds)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@items", DbType.String, hotelIds == null ? "" : string.Join(",", hotelIds));
            return HotelDB.ExecuteSqlString<HotelFilterColEntity>("HotelDB.GetManyHotelFilterCols", parameters);
        }

        public static int GetBrowseringCountOneComment(int commentId, int[] terminalTypeArray)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@commentId", DbType.Int32, commentId);
            parameters.AddInParameter("@items", DbType.String, terminalTypeArray == null || terminalTypeArray.Length == 0 ? "0,1,2,3,4,5" : string.Join(",", terminalTypeArray));
            var obj = HotelDB.ExecuteSqlStringAndReturnSingleField("HotelDB.GetBrowseringCountOneComment", parameters);
            return Convert.ToInt32(obj);
        }

        public static List<DistrictHotFilterTagEntity> GetDistrictHotFilterTagList(int districtId)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@districtId", DbType.Int32, districtId);
            return HotelBizDB.ExecuteSqlString<DistrictHotFilterTagEntity>("HotelBizDB.GetDistrictHotFilterTagList", parameters);
        }

        public static int UpsertDistrictHotFilterTag(DistrictHotFilterTagEntity hotFilterTag)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@DistrictId", DbType.Int32, hotFilterTag.DistrictId);
            dbParameterCollection.AddInParameter("@CategoryId", DbType.Int32, hotFilterTag.CategoryId);
            dbParameterCollection.AddInParameter("@Value", DbType.Int32, hotFilterTag.Value);
            dbParameterCollection.AddInParameter("@HotelCount", DbType.Int32, hotFilterTag.HotelCount);
            HotelBizDB.ExecuteNonQuery("SP_DistrictHotFilterTag_Upsert", dbParameterCollection);
            return 0;
        }

        public static InterestEntity GetOneInterestEntity(int Id)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@Id", DbType.Int32, Id);
            var interestList = HotelDB.ExecuteSqlString<InterestEntity>("HotelDB.GetOneInterestEntity", parameters);

            return interestList != null && interestList.Any() ? interestList.First() : new InterestEntity();
        }

        public static int UpdateCommentForOrderId(long orderId, int commentId)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@orderId", DbType.Int64, orderId);
            dbParameterCollection.AddInParameter("@commentId", DbType.Int32, commentId);
            HotelDB.ExecuteNonQuery("SP_Comments_UpdateForOrderID", dbParameterCollection);
            return 0;
        }

        public static List<HotelFilterColEntity> GetHotelDisplayFilterTagList(int hotelId)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@hotelId", DbType.Int32, hotelId);
            return HotelDB.ExecuteSqlString<HotelFilterColEntity>("HotelDB.GetHotelDisplayFilterTagList", parameters);
        }

        public static List<HotelRoomTypeFilterTagEntity> GetHotelRoomTypeFilterTagList(int hotelId)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@hotelId", DbType.Int32, hotelId);
            return HotelDB.ExecuteSqlString<HotelRoomTypeFilterTagEntity>("HotelDB.GetHotelRoomTypeFilterTagList", parameters);
        }

        public static List<PRoomInfoEntity> GetProomInfoList(int hotelId)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@hotelId", DbType.Int32, hotelId);
            return HotelDB.ExecuteSqlString<PRoomInfoEntity>("HotelDB.GetProomInfoList", parameters);
        }

        public static List<CanSellCheapHotelPackageEntity> GetCheapHotelPackage4Botao(DateTime startTime)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@curNow", DbType.DateTime, startTime);
            return HotelDB.ExecuteSqlString<CanSellCheapHotelPackageEntity>("HotelDB.GetCheapHotelPackage4Botao", parameters);
        }

        /// <summary>
        /// 根据携程映射关系的Idx得到RoomId（套餐ID）
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static string GetOtaRoomIdCtripByIdx(int pid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ID", DbType.Int32, pid);
            var obj = HotelDB.ExecuteSqlStringAndReturnSingleField("HotelDB.GetOtaRoomIdCtripByIdx", parameters);
            if (obj != null)
            {
                try
                {
                    return obj.ToString();
                }
                catch (Exception ex)
                {

                }
            }

            return "";
        }

        public static HotelContacts GetHotelContacts(long hotelid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelID", DbType.Int64, hotelid);

            return HotelDB.ExecuteSqlString<HotelContacts>("HotelDB.GetHotelContacts", parameters).First();
        }

        #region 铂韬结算订单数据

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static int InsertBotaoSettlementRecord(BotaoSettlementEntity entity)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@OrderId", DbType.Int64, entity.orderId);
            dbParameterCollection.AddInParameter("@OrderType", DbType.Int32, (int)entity.orderType);
            dbParameterCollection.AddInParameter("@Price", DbType.Int32, entity.price);
            dbParameterCollection.AddInParameter("@SettleType", DbType.Int32, (int)entity.settleType);
            HotelDB.ExecuteNonQuery("SP_BotaoSettlement_Insert", dbParameterCollection);
            return 0;
        }

        /// <summary>
        /// 获取待结算订单列表
        /// </summary>
        /// <returns></returns>
        public static List<BotaoSettleOrderEntity> GetBotaoSettleOrderList()
        {
            var parameters = new DBParameterCollection();
            return HotelDB.ExecuteSqlString<BotaoSettleOrderEntity>("HotelDB.GetBotaoSettleOrderList", parameters);
        }

        #endregion

        /// <summary>
        /// 套餐每日价格
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        internal static List<PackageCalendarPriceEntity> GetPackageCalendarPriceList(int pid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@PackageId", DbType.Int32, pid);
            return HotelDB.ExecuteSproc<PackageCalendarPriceEntity>("sp_PackageCalendarPrice_Select", parameters);
            //return HotelDB.ExecuteSqlString<PackageCalendarPriceEntity>("HotelDB.GetPackageCalendarPriceList", parameters);
        }

        internal static List<int> GetPackRoomHotelIdList(DateTime date)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@date", DbType.DateTime, date);
            return HotelDB.ExecuteSqlSingle<int>("HotelDB.GetPackRoomHotelIdList", parameters);
        }

        internal static int GetHotelIdByName(string hotelName)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@name", DbType.String, hotelName);
            var obj = HotelDB.ExecuteSqlStringAndReturnSingleField("HotelDB.GetHotelIdByName", parameters);

            return obj == null ? 0 : Convert.ToInt32(obj);
        }

        internal static List<NearbyHotelEntity> GetHotelListNearPOI(int poiId)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@poiId", DbType.Int32, poiId);
            return HotelDB.ExecuteSqlString<NearbyHotelEntity>("HotelDB.GetHotelListNearPOI", parameters);
        }

        internal static List<SameSerialPackageEntity> GetSameSerialPackageEntityList(int pId)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@pId", DbType.Int32, pId);
            return HotelDB.ExecuteSqlString<SameSerialPackageEntity>("HotelDB.GetSameSerialPackageEntityList", parameters);
        }
        internal static List<SameSerialPackageEntity> GetSerialPackageItemListByPid(int pId)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@pId", DbType.Int32, pId);
            return HotelDB.ExecuteSqlString<SameSerialPackageEntity>("HotelDB.GetSerialPackageItemListByPid", parameters);
        }

        #region 套餐专辑列表

        /// <summary>
        /// 专辑列表
        /// </summary>
        /// <param name="param"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        internal static List<PackageAlbumsEntity> GetPackageAlbumsList(TopNPackageSearchParam param, out int count)
        {
            TopNPackageSearchFilter entity = param.Filter;
            if (entity == null)
            {
                entity = new TopNPackageSearchFilter();
            }

            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@IsValid", DbType.Boolean, entity.IsValid);
            parameters.AddInParameter("@Type", DbType.Int32, entity.Type);

            if (entity.GroupNo != null && entity.GroupNo.Any())
            {
                parameters.AddInParameter("@GroupNo", DbType.String, string.Join(",", entity.GroupNo));
            }
            else
            {
                parameters.AddInParameter("@GroupNo", DbType.String, null);
            }

            object obj = HotelBizDB.ExecuteSqlStringAndReturnSingleField("HotelBizDB.GetPackageAlbumsListCount", parameters);
            int count1 = 0;
            if (obj != null)
            {
                count1 = Convert.ToInt32(obj);
            }
            count = count1;

            parameters.AddInParameter("@start", DbType.Int32, param.Start);
            parameters.AddInParameter("@count", DbType.Int32, param.PageSize + param.Start);

            List<PackageAlbumsEntity> list = HotelBizDB.ExecuteSqlString<PackageAlbumsEntity>("HotelBizDB.GetPackageAlbumsList", parameters);
            return list != null ? list : new List<PackageAlbumsEntity>();
        }

        internal static List<PackageAlbumsEntity> GetAllPackageAlbums()
        {
            var parameters = new DBParameterCollection();
            var result = HotelBizDB.ExecuteSqlString<PackageAlbumsEntity>("HotelBizDB.GetAllPackageAlbums", parameters);
            return result;
        }

        internal static List<PackageAlbumsEntity> GetPackageAlbumsByGroupNo(string groupNo)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@GroupNo", DbType.String, groupNo);
            var result = HotelBizDB.ExecuteSqlString<PackageAlbumsEntity>("HotelBizDB.GetPackageAlbumsByGroupNo", parameters);
            return result;
        }

        /// <summary>
        /// 单张专辑信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal static PackageAlbumsEntity GetOnePackageAlbums(int id)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@id", DbType.Int32, id);
            var result = HotelBizDB.ExecuteSqlString<PackageAlbumsEntity>("HotelBizDB.GetOnePackageAlbums", parameters).FirstOrDefault();
            return result == null ? new PackageAlbumsEntity() : result;
        }

        /// <summary>
        /// 删掉专辑
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal static int DelPackageAlbums(int id)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@id", DbType.Int32, id);
            HotelBizDB.ExecuteSprocAndReturnSingleField("SP_PackageAlbums_Delete", parameters);
            return 0;
        }

        /// <summary>
        /// 新增或更新专辑
        /// </summary>
        /// <param name="album"></param>
        /// <returns></returns>
        internal static int InsertOrUpdatePackageAlbums(PackageAlbumsEntity album)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ID", DbType.Int32, album.ID);
            parameters.AddInParameter("@Name", DbType.String, album.Name);
            parameters.AddInParameter("@SubTitle", DbType.String, album.SubTitle);
            parameters.AddInParameter("@Rank", DbType.Int32, album.Rank);
            parameters.AddInParameter("@Description", DbType.String, album.Description);
            parameters.AddInParameter("@IsValid", DbType.Boolean, album.IsValid);
            parameters.AddInParameter("@CoverPicSUrl", DbType.String, album.CoverPicSUrl);
            parameters.AddInParameter("@LabelPicSUrl", DbType.String, album.LabelPicSUrl);
            parameters.AddInParameter("@Author", DbType.String, album.Author);
            parameters.AddInParameter("@Date", DbType.DateTime, album.Date);
            parameters.AddInParameter("@Type", DbType.Int32, album.Type);
            parameters.AddInParameter("@GroupNo", DbType.String, album.GroupNo);
            parameters.AddInParameter("@ShowSortNo", DbType.Int32, album.ShowSortNo);
            parameters.AddInParameter("@ICON", DbType.String, album.ICON);

            Object obj = HotelBizDB.ExecuteSprocAndReturnSingleField("SP_PackageAlbums_InsertOrUpdate", parameters);
            return Convert.ToInt32(obj);
        }

        #endregion

        #region 批量获取房型列表
        /// <summary>
        /// 获取房间列表
        /// </summary>
        /// <param name="roomIds"></param>
        /// <returns></returns>
        internal static List<PRoomInfoEntity> GetPRoomInfoEntityList(IEnumerable<int> roomIds)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@roomids", DbType.String, roomIds != null && roomIds.Any() ? string.Join(",", roomIds) : "0");
            return HotelDB.ExecuteSqlString<PRoomInfoEntity>("HotelDB.GetPRoomInfoEntityList", parameters);
        }
        #endregion

        #region 获取绑定的专辑列表
        internal static List<RelPackageAlbumsEntity> GetRelPackageAlbums(IEnumerable<int> hotelIds, IEnumerable<int> pIds)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@hotelIds", DbType.String, (hotelIds == null || !hotelIds.Any()) ? "" : string.Join(",", hotelIds));
            parameters.AddInParameter("@pIds", DbType.String, (pIds == null || !pIds.Any()) ? "" : string.Join(",", pIds));
            return HotelBizDB.ExecuteSqlString<RelPackageAlbumsEntity>("HotelBizDB.GetRelPackageAlbums", parameters);
        }
        #endregion

        internal static List<HotelTop1PackageInfoEntity> GetHotelTop1PackageInfo(IEnumerable<int> HotelIDList, DateTime CheckIn, DateTime CheckOut)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@hotellist", DbType.String, string.Join(",", HotelIDList));
            parameters.AddInParameter("@CheckIn", DbType.Date, CheckIn.Date);
            return HotelDB.ExecuteSqlString<HotelTop1PackageInfoEntity>("HotelDB.GetHotelTop1PackageInfo", parameters);
        }

        /// <summary>
        /// 这里添加方法注释
        /// </summary>
        public static int AddTravelPerson(TravelPersonEntity travelperson)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@UserID", travelperson.UserID);
            parameters.Add("@TravelPersonName", travelperson.TravelPersonName);
            parameters.Add("@IDType", travelperson.IDType);
            parameters.Add("@IDNumber", travelperson.IDNumber);
            parameters.Add("@CreateTime", travelperson.CreateTime = System.DateTime.Now);
            parameters.Add("@UpdateTime", travelperson.UpdateTime = System.DateTime.Now);
            parameters.Add("@state", travelperson.State=1);
            parameters.Add("@Birthday ", travelperson.Birthday);
            int i = HotelBizDB.ExecuteNonQuery("SP_TravelPerson_Insert", parameters);
            return i;
        }

        /// <summary>
        /// 这里添加方法注释
        /// </summary>
        public static int UpdateTravelPerson(TravelPersonEntity travelperson)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@ID", travelperson.ID);
            parameters.Add("@UserID", travelperson.UserID);
            parameters.Add("@TravelPersonName", travelperson.TravelPersonName);
            parameters.Add("@IDType", travelperson.IDType);
            parameters.Add("@IDNumber", travelperson.IDNumber);
            //parameters.Add("@CreateTime", travelperson.CreateTime);
            parameters.Add("@UpdateTime",  System.DateTime.Now);
            parameters.Add("@state", 1);
            parameters.Add("@Birthday ", travelperson.Birthday);
            int i = HotelBizDB.ExecuteNonQuery("SP_TravelPerson_Update", parameters);
            return i;
        }

        public static TravelPersonEntity GetTravelPersonById(int id)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@id", DbType.Int32, id);
            var obj = HotelBizDB.ExecuteSqlString<TravelPersonEntity>("HotelBizDB.GetTravelPersonById", parameters).FirstOrDefault();
            return obj == null ? new TravelPersonEntity() : obj;
        }

        /// <summary>
        /// 根据用户id得到出行人 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static List<TravelPersonEntity> GetTravelPersonByUserId(long userId)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@userId", DbType.Int64, (long)userId);
            return HotelBizDB.ExecuteSqlString<TravelPersonEntity>("HotelBizDB.GetTravelPersonByUserId", parameters);
        }

        public static List<TravelPersonEntity> GetTravelPersonByIds(string Ids)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@Ids", DbType.String, Ids);
            return HotelBizDB.ExecuteSqlString<TravelPersonEntity>("HotelBizDB.GetTravelPersonByIds", parameters);
        }

        /// <summary>
        /// 删除出行人
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteTravelPerson(int id)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@id", DbType.Int32, id);

            object obj = HotelBizDB.ExecuteSprocAndReturnSingleField("SP_TravelPerson_Delete", dbParameterCollection);
            return Convert.ToInt32(obj) == 0;
        }

        internal static List<ChannelInfoEntity> GetAllChannelInfoList( )
        {
            var parameters = new DBParameterCollection();
            return HotelDB.ExecuteSqlString<ChannelInfoEntity>("HotelDB.GetAllChannelInfoList", parameters);
        }
        


        internal static List<CanSellDistrictCheapHotel> GetCanSellDistrictCheapHotelList(HJD.HotelServices.Contracts.HotelServiceEnums.HotelDistrictRange range)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@range", DbType.Int32, (int)range);
            return HotelDB.ExecuteSqlString<CanSellDistrictCheapHotel>("HotelDB.GetCanSellDistrictCheapHotelList", parameters);
        }

        internal static List<HotelTop1PackageInfoEntity> GetHotelPackageFirstAvailablePriceByPIds(IEnumerable<int> pIds, DateTime? checkIn, DateTime? checkOut)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@pids", DbType.String, string.Join(",", pIds));
            parameters.AddInParameter("@checkIn", DbType.String, checkIn.HasValue ? checkIn.Value.Date.ToString("yyyy-MM-dd") : "");
            return HotelDB.ExecuteSqlString<HotelTop1PackageInfoEntity>("HotelDB.GetHotelPackageFirstAvailablePriceByPIds", parameters);
        }

        internal static List<HotelTop1PackageInfoEntity> GetHotelPackageMinPriceByPIds(IEnumerable<int> pIds, DateTime? checkIn, DateTime? checkOut)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@pids", DbType.String, string.Join(",", pIds));
            parameters.AddInParameter("@checkIn", DbType.String, checkIn.HasValue ? checkIn.Value.Date.ToString("yyyy-MM-dd") : "");
            return HotelDB.ExecuteSqlString<HotelTop1PackageInfoEntity>("HotelDB.GetHotelPackageMinPriceByPIds", parameters);
        }
        internal static PackageEntity GetOnePackageEntity(int pId)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@pId", DbType.Int32, pId);
            var obj = HotelDB.ExecuteSqlString<PackageEntity>("HotelDB.GetOnePackageEntity", parameters).FirstOrDefault();
            return obj == null ? new PackageEntity() : obj;
        }

        internal static List<TopScoreHotelEntity> GetTopScoreHotelList(BsicSearchParam param, out int totalCount)
        {
            var parameters = new DBParameterCollection();

            parameters.AddInParameter("@lat", DbType.Double, param.lat);
            parameters.AddInParameter("@lng", DbType.Double, param.lng);
            parameters.AddInParameter("@districtID", DbType.Int32, param.districtid);

            object obj = HotelBizDB.ExecuteSqlStringAndReturnSingleField("HotelBizDB.GetTopScoreHotelListCount", parameters);
            totalCount = obj != null ? Convert.ToInt32(obj) : 0;

            parameters.AddInParameter("@start", DbType.Int32, param.start);
            parameters.AddInParameter("@count", DbType.Int32, param.count);
            return HotelBizDB.ExecuteSqlString<TopScoreHotelEntity>("HotelBizDB.GetTopScoreHotelList", parameters);
        }

        internal static List<PackageAlbumsEntity> GetPackageAlbumByGeoInfo(int districtID, float lat, float lng)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@districtID", DbType.Int32, districtID);
            parameters.AddInParameter("@lat", DbType.Double, lat);
            parameters.AddInParameter("@lng", DbType.Double, lng);
            return HotelBizDB.ExecuteSqlString<PackageAlbumsEntity>("HotelBizDB.GetPackageAlbumByGeoInfo", parameters);
        }

        internal static int UpdateTopScoreHotel(int Id, float RankScore)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@Id", DbType.Int32, Id);
            parameters.AddInParameter("@RankScore", DbType.Double, RankScore);
            HotelBizDB.ExecuteNonQuery("SP_TopScoreHotel_UpdateRank", parameters);
            return 0;
        }

        internal static Dictionary<int, bool> CanShowCtripPrice(IEnumerable<int> hotelIds)
        {
            var hasHotel = hotelIds != null && hotelIds.Any() ? true : false;
            var result = new Dictionary<int, bool>();
            if (hasHotel)
            {
                var parameters = new DBParameterCollection();
                parameters.AddInParameter("@hotelIds", DbType.String, string.Join(",", hotelIds));
                var data = HotelDB.ExecuteSqlString<HotelContactEntity>("HotelDB.GetManyHotelContacts", parameters);
                result = data.ToDictionary(_ => _.HotelID, _ => _.ShowCtripPrice);
            }
            return result;
        }

        internal static List<TopScoreHotelEntity> GetHotelBrowsingRecordList(BsicSearchParam param, out int totalCount)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@userId", DbType.Int64, param.userId);
            object obj = HotelBizDB.ExecuteSqlStringAndReturnSingleField("HotelBizDB.GetHotelBrowsingRecordCount", parameters);
            totalCount = obj != null ? Convert.ToInt32(obj) : 0;

            parameters.AddInParameter("@start", DbType.Int32, param.start);
            parameters.AddInParameter("@count", DbType.Int32, param.count);
            return HotelBizDB.ExecuteSqlString<TopScoreHotelEntity>("HotelBizDB.GetHotelBrowsingRecordList", parameters);
        }

        internal static List<BrowsingRecordEntity> GetBrowsingRecordList(BsicSearchParam param, out int totalCount)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@userId", DbType.Int64, param.userId);
            object obj = HotelBizDB.ExecuteSqlStringAndReturnSingleField("HotelBizDB.GetBrowsingRecordCount", parameters);
            totalCount = obj != null ? Convert.ToInt32(obj) : 0;

            parameters.AddInParameter("@start", DbType.Int32, param.start);
            parameters.AddInParameter("@count", DbType.Int32, param.count);
            return HotelBizDB.ExecuteSqlString<BrowsingRecordEntity>("HotelBizDB.GetBrowsingRecordList", parameters);
        }

        internal static List<TopScoreHotelEntity> GetHotelWithInDistance(float lat, float lng, int count, int start, out int totalCount)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@lat", DbType.Double, lat);
            parameters.AddInParameter("@lng", DbType.Double, lng);
            object obj = HotelBizDB.ExecuteSqlStringAndReturnSingleField("HotelBizDB.GetHotelWithIn300KMCount", parameters);
            totalCount = obj != null ? Convert.ToInt32(obj) : 0;
            parameters.AddInParameter("@count", DbType.Int32, count);
            parameters.AddInParameter("@start", DbType.Int32, start);

            return HotelBizDB.ExecuteSqlString<TopScoreHotelEntity>("HotelBizDB.GetHotelWithIn300KMList", parameters);
        }
        

        internal static List<SearchRecordEntity> GetSearchRecordList(CommonRecordQueryParam param, out int totalCount)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@userId", DbType.Int64, param.userId);
            parameters.AddInParameter("@businessType", DbType.Int32, param.businessType);
            object obj = HotelBizDB.ExecuteSqlStringAndReturnSingleField("HotelBizDB.GetSearchRecordCount", parameters);
            totalCount = obj != null ? Convert.ToInt32(obj) : 0;

            parameters.AddInParameter("@start", DbType.Int32, param.start);
            parameters.AddInParameter("@count", DbType.Int32, param.count);
            return HotelBizDB.ExecuteSqlString<SearchRecordEntity>("HotelBizDB.GetSearchRecordList", parameters);
        }

        /// <summary>
        /// 查询指定酒店的基于OTA数据的房态数据
        /// </summary>
        /// <param name="hotelId">zmjd酒店Id</param>
        /// <returns></returns>
        public static List<OtaRoomBedState> GetOtaRoomBedStateByHid(int hotelId)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelId", DbType.Int32, hotelId);
            return HotelDB.ExecuteSqlString<OtaRoomBedState>("HotelDB.GetOtaRoomBedStateByHid", parameters);
        }

        /// <summary>
        /// 获取有合作的酒店的房态汇总信息
        /// </summary>
        /// <returns></returns>
        public static List<OtaHotelRoomState> GetOtaHotelRoomStates()
        {
            var date = DateTime.Now.Date;
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@Date", DbType.DateTime, date);
            return HotelDB.ExecuteSqlString<OtaHotelRoomState>("HotelDB.GetOtaHotelRoomStates", parameters);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<ActiveRuleGroupEntity> GetWXActiveRuleGroupList(int id)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@ID", DbType.Int32, id);
            return HotelDB.ExecuteSqlString<ActiveRuleGroupEntity>("CommDB.GetWXActiveRuleGroupList", parameters);
        }
        public static List<ActiveRuleExEntity> GetWXActiveRuleExList(int groupId)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@GroupId", DbType.Int32, groupId);
            return HotelDB.ExecuteSqlString<ActiveRuleExEntity>("CommDB.GetWXActiveRuleExList", parameters);
        }

        /// <summary>
        /// 根据hotelid获取ota信息
        /// </summary>
        /// <returns></returns>
        public static List<HotelOTAEntity> GetOtaListByHotelID(int hotelid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelId", DbType.Int32, hotelid);
            return HotelDB.ExecuteSqlString<HotelOTAEntity>("HotelDB.GetOtaListByHotelID", parameters);
        }

        public static List<SimpleHotelEntity> GetHotelPackageDistrictInfo(double lat = 0, double lng = 0, int geoScopeType = 0)
        {
            var parameters = new DBParameterCollection();
            //parameters.AddInParameter("@HotelId", DbType.Int32, hotelid);
            parameters.AddInParameter("@lat", DbType.Double, lat);
            parameters.AddInParameter("@lng", DbType.Double, lng);
            parameters.AddInParameter("@geoScopeType", DbType.Int32, geoScopeType);
            return HotelDB.ExecuteSqlString<SimpleHotelEntity>("HotelDB.GetHotelPackageDistrictInfo", parameters);
        }

        /// <summary>
        /// 获取酒店携程价显示策略
        /// </summary>
        /// <param name="hotelid"></param>
        /// <returns>0:可以显示最低价  1：不显示最低价 </returns>
        public static Int16 GetHotelCtripPricePolicy(int hotelid)
        {
            var parameters = new DBParameterCollection();
            //parameters.AddInParameter("@HotelId", DbType.Int32, hotelid);
            parameters.AddInParameter("@hotelid", DbType.Int32, hotelid);
            return Convert.ToInt16(HotelDB.ExecuteSqlStringAndReturnSingleField("HotelDB.GetHotelCtripPricePolicy", parameters));
        }

        public static List<PackageItemEntity> GetHotelPackageByDistrictId(int distictId, DateTime checkIn, DateTime checkOut, int pageIndex, int pageSize, out int totalCount)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@DistictID", DbType.Int32, distictId);
            parameters.AddInParameter("@StartDate", DbType.DateTime, checkIn);
            parameters.AddInParameter("@EndDate", DbType.DateTime, checkOut);
            object obj = HotelBizDB.ExecuteSqlStringAndReturnSingleField("HotelDB.GetHotelPackageByDistrictIdCount", parameters);
            totalCount = obj != null ? Convert.ToInt32(obj) : 0;

            parameters.AddInParameter("@pageIndex", DbType.Int32, pageIndex);
            parameters.AddInParameter("@pageSize", DbType.Int32, pageSize);
            return HotelDB.ExecuteSqlString<PackageItemEntity>("HotelDB.GetHotelPackageByDistrictId", parameters);
        }

        public static PackageEntity GetFirstVipPackageByPackageId(int pid)
        {

            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@PID", DbType.Int32, pid);
            PackageEntity pe = HotelDB.ExecuteSqlString<PackageEntity>("HotelDB.GetFirstVipPackageByPackageId", parameters).FirstOrDefault();
            return pe == null ? new PackageEntity() : pe;
        }

        public static List<TravelPersonEntity> GetBookUserDateInfoByExchangCouponId(int ExchangCouponId)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@ExchangCouponId", ExchangCouponId);
            return HotelBizDB.ExecuteSqlString<TravelPersonEntity>("HotelBizDB.GetBookUserDateInfoByExchangCouponId", parameters);
        }

        public static List<RetailHotel> GetRetailHotelList(int packageType, int sort, string searchWord, int start, int count)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@PackageType", packageType);
            parameters.Add("@searchWord", string.IsNullOrEmpty(searchWord) ? "" : searchWord);
            parameters.Add("@sort", sort);
            parameters.Add("@start", start);
            parameters.Add("@count", count);
            return HotelBizDB.ExecuteSqlString<RetailHotel>("HotelDB.GetRetailHotelList", parameters);
        }

        public static int GetRetailHotelListCount(int packageType, string searchWord="")
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@PackageType", packageType);
            parameters.Add("@searchWord", string.IsNullOrEmpty(searchWord) ? "" : searchWord);
            object obj = HotelBizDB.ExecuteSqlStringAndReturnSingleField("HotelDB.GetRetailHotelListCount", parameters);
            return obj == null ? 0 : Convert.ToInt32(obj);
        }

        public static List<RetailPackageEntity> GetRetailPackageList(int hotelId)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@HotelId", hotelId);
            return HotelBizDB.ExecuteSqlString<RetailPackageEntity>("HotelDB.GetRetailPackageList", parameters);
        }

        public static List<PRateEntity> GetPrateListByPids(string pids)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@PIDS", pids);
            return HotelBizDB.ExecuteSqlString<PRateEntity>("HotelDB.GetPrateListByPids", parameters);
        }

        public static RetailHotel GetHotelChannel(int hotelid)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@HotelId", hotelid);
            return HotelBizDB.ExecuteSqlString<RetailHotel>("HotelDB.GetHotelChannel", parameters).First();
        }

        public static RetailPackageEntity GetRetailPackageInfo(int pid, long cid)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@PID", pid);
            parameters.Add("@CID", cid);
            return HotelBizDB.ExecuteSqlString<RetailPackageEntity>("HotelDB.GetRetailPackageInfo", parameters).FirstOrDefault();
        }
        public static List<TopNPackagesEntity> GetTopNPackagesListByAlbumIdOrPID(int albumId = 0, int pid = 0)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@AlbumID", DbType.Int32, albumId);
            parameters.AddInParameter("@PID", DbType.Int32, pid);
            return HotelBizDB.ExecuteSqlString<TopNPackagesEntity>("HotelBizDB.GetTopNPackagesListByAlbumIdOrPID", parameters);
        }
        public static int UpdateTopNPackageTitle(int pid, string title)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@PID", DbType.Int32, pid);
            dbParameterCollection.AddInParameter("@Title", DbType.String, title);

            object obj = HotelBizDB.ExecuteSprocAndReturnSingleField("SP_TopNPackages_Title_Update", dbParameterCollection);
            return Convert.ToInt32(obj);
        }
    }
}