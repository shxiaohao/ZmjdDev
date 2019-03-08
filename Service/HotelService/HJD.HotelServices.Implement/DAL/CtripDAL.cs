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
using HJD.OtaCrawlerService.Contract.Ctrip.Hotel;

namespace HJD.HotelServices
{
    internal class CtripDAL : RepositoryBase
    {      
        internal static List<CrawlerHotelBaseRoom> GetCtripHotelRoom(long hotelid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelID", DbType.Int64, hotelid);

            return CtripDB.ExecuteSqlString<CrawlerHotelBaseRoom>("CtripDB.GetBaseRoomByHotel", parameters);
        }

        internal static List<CrawlerHotelRoom> GetCtripHotelRoomPrice(long baseRoomId, DateTime night)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@BaseRoomID", DbType.Int64, baseRoomId);
            parameters.AddInParameter("@Night", DbType.DateTime, night);

            return CtripDB.ExecuteSqlString<CrawlerHotelRoom>("CtripDB.GetPriceRateByRoom", parameters);
        }

        internal static List<CrawlerHotelRoomEx> GetPriceRateByHotel(long hotelid, DateTime checkIn, DateTime checkOut)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelID", DbType.Int64, hotelid);
            parameters.AddInParameter("@CheckIn", DbType.DateTime, checkIn);
            parameters.AddInParameter("@CheckOut", DbType.DateTime, checkOut);

            return CtripDB.ExecuteSqlString<CrawlerHotelRoomEx>("CtripDB.GetPriceRateByHotel", parameters);
        }

        internal static List<CtripHotelRoomPriceNightEntity> GetCtripHotelRoomPriceNight(long hotelid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelID", DbType.Int64, hotelid);

            return CtripDB.ExecuteSqlString<CtripHotelRoomPriceNightEntity>("CtripDB.GetPriceRateNightByHotel", parameters);
        }

        internal static List<CtripHotelRoomPriceNightEntity> GetCtripHotelRoomPriceNight2(long hotelid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelID", DbType.Int64, hotelid);

            return CtripDB.ExecuteSqlString<CtripHotelRoomPriceNightEntity>("CtripDB.GetPriceRateNightByHotel2", parameters);
        }

        internal static List<CtripHotelRoomPriceNightEntity> GetCtripHotelRoomPriceNightWhereId(long hotelid, string pid)
        {
            var parameters = new DBParameterCollection();
            parameters.AddInParameter("@HotelID", DbType.Int64, hotelid);
            parameters.AddInParameter("@PID", DbType.String, pid);

            return CtripDB.ExecuteSqlString<CtripHotelRoomPriceNightEntity>("CtripDB.GetPriceRateNightByHotelAndId", parameters);
        }

        /// <summary>
        /// 插入Room表数据
        /// </summary>
        /// <param name="chb"></param>
        /// <returns></returns>
        internal static int InsertCtripRoom(CrawlerHotelBaseRoom chb)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@ID", DbType.Int64, chb.ID);
            dbParameterCollection.AddInParameter("@HotelID", DbType.Int64, chb.HotelID);
            dbParameterCollection.AddInParameter("@BaseRoomID", DbType.Int64, chb.BaseRoomID);
            dbParameterCollection.AddInParameter("@Name", DbType.String, chb.Name);
            dbParameterCollection.AddInParameter("@Area", DbType.String, chb.Area);
            dbParameterCollection.AddInParameter("@Floor", DbType.String, chb.Floor);
            dbParameterCollection.AddInParameter("@BedType", DbType.String, chb.BedType);
            dbParameterCollection.AddInParameter("@NonSmokingRoom", DbType.String, (chb.NonSmokingRoom != null ? chb.NonSmokingRoom : ""));
            dbParameterCollection.AddInParameter("@MaxGuests", DbType.String, chb.MaxGuests);

            object obj = CtripDB.ExecuteSprocAndReturnSingleField("SP_CtripRoom_InsertOrUpdate", dbParameterCollection);

            return Convert.ToInt32(obj);
        }

        /// <summary>
        /// 插入PriceRate表数据
        /// </summary>
        /// <param name="room"></param>
        /// <returns></returns>
        internal static int InsertCtripPriceRate(CrawlerHotelRoom room)
        {
            DBParameterCollection dbParameterCollection = new DBParameterCollection();
            dbParameterCollection.AddInParameter("@ID", DbType.Int64, room.ID);
            dbParameterCollection.AddInParameter("@BaseRoomID", DbType.Int64, room.BaseRoomID);
            dbParameterCollection.AddInParameter("@RoomID", DbType.String, room.RoomID);
            dbParameterCollection.AddInParameter("@Night", DbType.DateTime, room.Night);
            dbParameterCollection.AddInParameter("@Name", DbType.String, room.Name);
            dbParameterCollection.AddInParameter("@BedType", DbType.String, room.BedType);
            dbParameterCollection.AddInParameter("@Breakfast", DbType.String, room.Breakfast);
            dbParameterCollection.AddInParameter("@Broadband", DbType.String, room.Broadband);
            dbParameterCollection.AddInParameter("@Policy", DbType.String, room.Policy);
            dbParameterCollection.AddInParameter("@PolicyTip", DbType.String, room.PolicyTip);
            dbParameterCollection.AddInParameter("@Price", DbType.Decimal, room.Price);
            dbParameterCollection.AddInParameter("@IsGift", DbType.Int32, room.IsGift);
            dbParameterCollection.AddInParameter("@GiftTip", DbType.String, (room.GiftTip != null ? room.GiftTip : ""));
            dbParameterCollection.AddInParameter("@PackageSummary", DbType.String, (room.PackageSummary != null ? room.PackageSummary : ""));
            dbParameterCollection.AddInParameter("@DelPrice", DbType.String, (room.DelPrice != null ? room.DelPrice : ""));
            dbParameterCollection.AddInParameter("@SoldPrice", DbType.String, (room.SoldPrice != null ? room.SoldPrice : ""));
            dbParameterCollection.AddInParameter("@Discount", DbType.String, (room.Discount != null ? room.Discount : ""));
            dbParameterCollection.AddInParameter("@PayType", DbType.String, (room.PayType != null ? room.PayType : ""));
            dbParameterCollection.AddInParameter("@CanSell", DbType.Int32, room.CanSell);

            object obj = CtripDB.ExecuteSprocAndReturnSingleField("SP_PriceRate_InsertOrUpdate", dbParameterCollection);

            return Convert.ToInt32(obj);
        }
    }
}
