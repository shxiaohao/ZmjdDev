using HJD.AccessService.Contract.Model;
using HJD.AccessService.Contract.Model.Hotel;
using HJD.HotelPrice.Contract.DataContract;
using HJD.HotelServices.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace HJD.AccessService.Implement.Helper
{
    public class HotelHelper
    {
        static object mutex = new object();

        public static List<int> GetHotelListForPriceSlot()
        {
            var hotelList = new List<int>();

            var sql = string.Format(@"
select hotel.HotelId 
from (select distinct HotelId hid from HotelState (nolock) where State = 1) activeHotel
join Hotel hotel on hotel.HotelId = activeHotel.HID
--where hotel.HotelId = 598483
order by hotel.HotelId desc

--select hotel.HotelId 
--from (select distinct HotelId hid from HotelState (nolock) where State = 1) activeHotel
--join Hotel hotel on hotel.HotelId = activeHotel.HID
--where hotel.HotelId in
--(
--select hotelid from HotelPriceSlot_UnList
--)

");
            var reader = DBHelper.ExecuteReader(DBHelper.HotelDbConn, System.Data.CommandType.Text, sql);
            using (reader)
            {
                while (reader.Read())
                {
                    hotelList.Add(Convert.ToInt32(reader["HotelId"]));
                }
            }

            return hotelList;
        }
        
        /// <summary>
        /// 获取所有没有HotelBasePrice的酒店ID
        /// </summary>
        /// <returns></returns>
        public static List<int> GetNoBasePriceHotels()
        {
            var hotelList = new List<int>();

            var sql = string.Format(@"
select distinct h.HotelId 
from HotelDB.dbo.Hotel h 
inner join HotelDB.dbo.HotelState hs on hs.HotelID = h.HotelId 
left join HotelDB.dbo.HotelBasePrice hb on hb.HotelID = h.HotelId 
where (hs.State = 1 or hs.State = 2) and hb.HotelID is null 
");
            var reader = DBHelper.ExecuteReader(DBHelper.HotelDbConn, System.Data.CommandType.Text, sql);
            using (reader)
            {
                while (reader.Read())
                {
                    hotelList.Add(Convert.ToInt32(reader["HotelId"]));
                }
            }

            return hotelList;
        }

        /// <summary>
        /// 获取所有PricePlanEx不完整的酒店ID
        /// </summary>
        /// <param name="channelid"></param>
        /// <returns></returns>
        public static List<int> GetHaveNullPricePlanExHids(int channelid)
        {
            var hotelList = new List<int>();

            var sql = string.Format(@"
select distinct pp.HotelID 
from PricePlan pp
left join PricePlanEx ppx on pp.HotelID = ppx.HotelID and ppx.ChannelID = pp.ChannelID and ppx.Date = pp.Date
where pp.ChannelID = {0} and (ppx.Name is null or ppx.Brief is null)
order by pp.HotelID
", channelid);

            var reader = DBHelper.ExecuteReader(DBHelper.HotelDbConn, System.Data.CommandType.Text, sql);
            using (reader)
            {
                while (reader.Read())
                {
                    hotelList.Add(Convert.ToInt32(reader["HotelID"]));
                }
            }

            return hotelList;
        }

        /// <summary>
        /// 获取指定酒店的
        /// </summary>
        /// <returns></returns>
        public static List<DateTime> GetHaveNullPricePlanExDateListByHid(int channelid, int hotelid)
        {
            var dateList = new List<DateTime>();

            var sql = string.Format(@"
select pp.HotelID,pp.Date,pp.ChannelID,pp.Price 
from PricePlan pp
left join PricePlanEx ppx on pp.HotelID = ppx.HotelID and ppx.ChannelID = pp.ChannelID and ppx.Date = pp.Date
where pp.ChannelID = {0} and (ppx.Name is null or ppx.Brief is null) and pp.HotelID = {1}
order by pp.Date
", channelid, hotelid);

            var reader = DBHelper.ExecuteReader(DBHelper.HotelDbConn, System.Data.CommandType.Text, sql);
            using (reader)
            {
                while (reader.Read())
                {
                    dateList.Add(Convert.ToDateTime(reader["Date"].ToString()));
                }
            }

            return dateList;
        }

        /// <summary>
        /// 初始化/更新指定酒店的HotelBasePrice
        /// </summary>
        /// <returns></returns>
        public static int AddHotelBasePrice(int hotelid, int min, int max, int channel)
        {
            var sql = string.Format(@"
IF NOT EXISTS( select top 1 HotelId from HotelDB.dbo.HotelBasePrice where HotelId = '{0}')
begin
insert into HotelDB.dbo.HotelBasePrice values ('{0}','{1}','{2}','RMB',GETDATE(),'{3}')
end
  
else
begin
update HotelDB.dbo.HotelBasePrice set MinPrice = '{1}', MaxPrice = '{2}' where HotelID = '{0}' and Channel = '{3}' and Currency = 'RMB'
end
", hotelid, min, max, channel);

            return ExcSql(sql);
        }

        public static int AddPriceSlot(HotelPriceSlot pslot)
        {
            lock (mutex)
            {
                var add = 0;

                var parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@HotelId", SqlDbType.Int) { Value = pslot.HotelId });
                parameters.Add(new SqlParameter("@Night", SqlDbType.DateTime) { Value = pslot.Night });
                parameters.Add(new SqlParameter("@Slot_0_400", SqlDbType.Bit) { Value = pslot.Slot_0_400 });
                parameters.Add(new SqlParameter("@Slot_0_600", SqlDbType.Bit) { Value = pslot.Slot_0_600 });
                parameters.Add(new SqlParameter("@Slot_0_800", SqlDbType.Bit) { Value = pslot.Slot_0_800 });
                parameters.Add(new SqlParameter("@Slot_0_1000", SqlDbType.Bit) { Value = pslot.Slot_0_1000 });
                parameters.Add(new SqlParameter("@Slot_0_1500", SqlDbType.Bit) { Value = pslot.Slot_0_1500 });
                parameters.Add(new SqlParameter("@Slot_0_2000", SqlDbType.Bit) { Value = pslot.Slot_0_2000 });
                parameters.Add(new SqlParameter("@Slot_0_0", SqlDbType.Bit) { Value = pslot.Slot_0_0 });

                parameters.Add(new SqlParameter("@Slot_400_600", SqlDbType.Bit) { Value = pslot.Slot_400_600 });
                parameters.Add(new SqlParameter("@Slot_400_800", SqlDbType.Bit) { Value = pslot.Slot_400_800 });
                parameters.Add(new SqlParameter("@Slot_400_1000", SqlDbType.Bit) { Value = pslot.Slot_400_1000 });
                parameters.Add(new SqlParameter("@Slot_400_1500", SqlDbType.Bit) { Value = pslot.Slot_400_1500 });
                parameters.Add(new SqlParameter("@Slot_400_2000", SqlDbType.Bit) { Value = pslot.Slot_400_2000 });
                parameters.Add(new SqlParameter("@Slot_400_0", SqlDbType.Bit) { Value = pslot.Slot_400_0 });

                parameters.Add(new SqlParameter("@Slot_600_800", SqlDbType.Bit) { Value = pslot.Slot_600_800 });
                parameters.Add(new SqlParameter("@Slot_600_1000", SqlDbType.Bit) { Value = pslot.Slot_600_1000 });
                parameters.Add(new SqlParameter("@Slot_600_1500", SqlDbType.Bit) { Value = pslot.Slot_600_1500 });
                parameters.Add(new SqlParameter("@Slot_600_2000", SqlDbType.Bit) { Value = pslot.Slot_600_2000 });
                parameters.Add(new SqlParameter("@Slot_600_0", SqlDbType.Bit) { Value = pslot.Slot_600_0 });

                parameters.Add(new SqlParameter("@Slot_800_1000", SqlDbType.Bit) { Value = pslot.Slot_800_1000 });
                parameters.Add(new SqlParameter("@Slot_800_1500", SqlDbType.Bit) { Value = pslot.Slot_800_1500 });
                parameters.Add(new SqlParameter("@Slot_800_2000", SqlDbType.Bit) { Value = pslot.Slot_800_2000 });
                parameters.Add(new SqlParameter("@Slot_800_0", SqlDbType.Bit) { Value = pslot.Slot_800_0 });

                parameters.Add(new SqlParameter("@Slot_1000_1500", SqlDbType.Bit) { Value = pslot.Slot_1000_1500 });
                parameters.Add(new SqlParameter("@Slot_1000_2000", SqlDbType.Bit) { Value = pslot.Slot_1000_2000 });
                parameters.Add(new SqlParameter("@Slot_1000_0", SqlDbType.Bit) { Value = pslot.Slot_1000_0 });
                parameters.Add(new SqlParameter("@Slot_1500_2000", SqlDbType.Bit) { Value = pslot.Slot_1500_2000 });
                parameters.Add(new SqlParameter("@Slot_1500_0", SqlDbType.Bit) { Value = pslot.Slot_1500_0 });
                parameters.Add(new SqlParameter("@Slot_2000_0", SqlDbType.Bit) { Value = pslot.Slot_2000_0 });

                parameters.Add(new SqlParameter("@MinPrice", SqlDbType.Int) { Value = pslot.MinPrice });
                parameters.Add(new SqlParameter("@MaxPrice", SqlDbType.Int) { Value = pslot.MaxPrice });
                parameters.Add(new SqlParameter("@ChannelId", SqlDbType.Int) { Value = pslot.ChannelId });
                
                parameters.Add(new SqlParameter("@Prices", SqlDbType.NVarChar) { Value = pslot.Prices });
                parameters.Add(new SqlParameter("@SellState", SqlDbType.Int) { Value = pslot.SellState });
                parameters.Add(new SqlParameter("@CreateTime", SqlDbType.DateTime) { Value = pslot.CreateTime });
                parameters.Add(new SqlParameter("@UpdateTime", SqlDbType.DateTime) { Value = pslot.UpdateTime });

                var addsql = string.Format(@"
DECLARE @retcode INT 
BEGIN TRANSACTION
SET @retcode = 0  

IF NOT EXISTS( select * from HotelPriceSlot where HotelId = @HotelId and Night = @Night)
begin
Insert into HotelPriceSlot values 
(@HotelId,@Night,
@Slot_0_400,
@Slot_0_600,
@Slot_0_800,
@Slot_0_1000,
@Slot_0_1500,
@Slot_0_2000,
@Slot_0_0,
@Slot_400_600,
@Slot_400_800,
@Slot_400_1000,
@Slot_400_1500,
@Slot_400_2000,
@Slot_400_0,
@Slot_600_800,
@Slot_600_1000,
@Slot_600_1500,
@Slot_600_2000,
@Slot_600_0,
@Slot_800_1000,
@Slot_800_1500,
@Slot_800_2000,
@Slot_800_0,
@Slot_1000_1500,
@Slot_1000_2000,
@Slot_1000_0,
@Slot_1500_2000,
@Slot_1500_0,
@Slot_2000_0,
@MinPrice,
@MaxPrice,
@ChannelId,
@Prices,@SellState,@CreateTime,@UpdateTime)
end

else
begin
update HotelPriceSlot 
set 
Slot_0_400 = @Slot_0_400,
Slot_0_600 = @Slot_0_600,
Slot_0_800 = @Slot_0_800,
Slot_0_1000 = @Slot_0_1000,
Slot_0_1500 = @Slot_0_1500,
Slot_0_2000 = @Slot_0_2000,
Slot_0_0 = @Slot_0_0,
Slot_400_600 = @Slot_400_600,
Slot_400_800 = @Slot_400_800,
Slot_400_1000 = @Slot_400_1000,
Slot_400_1500 = @Slot_400_1500,
Slot_400_2000 = @Slot_400_2000,
Slot_400_0 = @Slot_400_0,
Slot_600_800 = @Slot_600_800,
Slot_600_1000 = @Slot_600_1000,
Slot_600_1500 = @Slot_600_1500,
Slot_600_2000 = @Slot_600_2000,
Slot_600_0 = @Slot_600_0,
Slot_800_1000 = @Slot_800_1000,
Slot_800_1500 = @Slot_800_1500,
Slot_800_2000 = @Slot_800_2000,
Slot_800_0 = @Slot_800_0,
Slot_1000_1500 = @Slot_1000_1500,
Slot_1000_2000 = @Slot_1000_2000,
Slot_1000_0 = @Slot_1000_0,
Slot_1500_2000 = @Slot_1500_2000,
Slot_1500_0 = @Slot_1500_0,
Slot_2000_0 = @Slot_2000_0,
MinPrice = @MinPrice,
MaxPrice = @MaxPrice,
ChannelId = @ChannelId,
Prices = @Prices,SellState = @SellState,UpdateTime = @UpdateTime 
where HotelId = @HotelId and Night = @Night
end

Exit_Flag:   
IF @@TranCount > 0
    IF @RetCode = 0
        COMMIT TRANSACTION
    ELSE
        ROLLBACK TRANSACTION
SELECT @retcode AS ExecInfo");

                add = ExcSql(addsql, parameters.ToArray());

                return add;   
            }
        }

        public static int UpdatePricePlan(HotelPriceSlot priceSlot, int nightCount = 1, int vipPrice = 0)
        {
            var sql = string.Format(@"
IF NOT EXISTS( SELECT Price FROM PricePlan WHERE HotelID = {1} AND Date = '{2}' AND ChannelID = '{3}' )
		BEGIN

		INSERT INTO [HotelDB].[dbo].[PricePlan]
		  (
			[HotelID],
			[Date],
			[ChannelID],
			[Price],
            [VipPrice],            
            [NightCount]            
		  )
		VALUES
		  (
			{1},
			'{2}',
			'{3}',
			{0},
            {4},
            {5}
		  )

		END
	ELSE
		BEGIN

		UPDATE PricePlan
		SET    Price = {0}, VipPrice = {4}, NightCount = {5}
		WHERE  HotelID = {1}
			   AND Date = '{2}'
			   AND ChannelID = '{3}'

		END", priceSlot.MinPrice, priceSlot.HotelId, priceSlot.Night.Date, priceSlot.ChannelId, vipPrice, nightCount);

            return ExcSql(sql);
        }

        public static int UpdatePricePlanEx(HotelPriceSlot priceSlot, string code, string brief)
        {
            var sql = string.Format(@"
IF NOT EXISTS( select * from HotelDB.dbo.PricePlanEx where Hotelid = '{0}' and [Date] = '{1}' and ChannelId = '{2}')
Insert into HotelDB.dbo.PricePlanEx values  ('{0}','{1}','{2}','{3}','{4}')
ELSE
Update HotelDB.dbo.PricePlanEx Set Name = '{3}',Brief = '{4}' where Hotelid = '{0}' and [Date] = '{1}' and ChannelId = '{2}'
", priceSlot.HotelId, priceSlot.Night.Date, priceSlot.ChannelId, code, brief);

            return ExcSql(sql);
        }

        /// <summary>
        /// 更新HotelMinPrice
        /// </summary>
        /// <param name="hotelMinPrice"></param>
        /// <returns></returns>
        public static int UpdateHotelMinPrice(HotelMinPriceEntity hotelMinPrice)
        {
            var sql = string.Format(@"
IF NOT EXISTS( select * from HotelDB.dbo.HotelMinPrice where Hotelid = '{0}' and [Date] = '{1}' and ChannelId = '{2}' and Type = '{9}')
INSERT INTO [HotelDB].[dbo].[HotelMinPrice] 
([HotelID],[Date],[ChannelID],[Price],[VipPrice],[NightCount],[PID],[Name],[Brief],[Type],[UpdateTime])
VALUES ({0},'{1}',{2},{3},{4},{5},{6},'{7}','{8}',{9},GETDATE())
ELSE
Update HotelDB.dbo.HotelMinPrice Set Price = {3},VipPrice = {4},NightCount = {5},PID = {6},Name = '{7}',Brief = '{8}',UpdateTime = getdate() where Hotelid = '{0}' and [Date] = '{1}' and ChannelId = '{2}' and Type = '{9}'
", hotelMinPrice.HotelID, hotelMinPrice.Date.Date.ToString("yyyy-MM-dd"), hotelMinPrice.ChannelID, hotelMinPrice.Price, hotelMinPrice.VipPrice, hotelMinPrice.NightCount, hotelMinPrice.PID, hotelMinPrice.Name, hotelMinPrice.Brief, hotelMinPrice.Type);  //hotelMinPrice.UpdateTime

            return ExcSql(sql);
        }

        /// <summary>
        /// 清空指定酒店、渠道、日期的PricePlan、PricePlanEx、HotelPriceSlot记录
        /// </summary>
        /// <param name="hotelId"></param>
        /// <param name="channelId"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static int ClearPricePlanAndPriceSlot(int hotelId, int channelId, DateTime date)
        {
            var sql = string.Format(@"
delete from Hoteldb.dbo.PricePlan where HotelID = {0} and ChannelID = {1} and Date = '{2}'
delete from Hoteldb.dbo.PricePlanEx where HotelID = {0} and ChannelID = {1} and Date = '{2}'
delete from Hoteldb.dbo.HotelPriceSlot where HotelID = {0} and ChannelID = {1} and Night = '{2}'
delete from Hoteldb.dbo.HotelMinPrice where HotelID = {0} and ChannelID = {1} and Date = '{2}'
", hotelId, channelId, date);

            return ExcSql(sql);
        }

        /// <summary>
        /// 清空指定日期前的HotelPriceSlot记录
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static int ClearPricePlan(DateTime date)
        {
            var sql = string.Format("delete from HotelPriceSlot where night < '{0}'", date.Date);

            return ExcSql(sql);
        }

        public static int UpdateHotelEnabled(int hotelid)
        {
            var sql = string.Format("update hotel set Enabled = Enabled where HotelId in ('{0}')", hotelid);

            return ExcSql(sql);
        }

        #region 更新PRate价格的相关操作

        /// <summary>
        /// 
        /// </summary>
        /// <param name="priceSource"></param>
        /// <returns></returns>
        public static List<HotelPackageEntity> GetAllPackageByPriceSource(int priceSource)
        {
            var packageList = new List<HotelPackageEntity>();

            var sql = string.Format(@"
select * from hoteldb..Package 
where PriceSource = 1 --and HotelID in (59384,298896,13201,272436,459379)
");

            var reader = DBHelper.ExecuteReader(DBHelper.HotelDbConn, System.Data.CommandType.Text, sql);
            using (reader)
            {
                while (reader.Read())
                {
                    var packageEntity = new HotelPackageEntity();
                    packageEntity.Id = Convert.ToInt32(reader["ID"].ToString());
                    packageEntity.HotelId = Convert.ToInt32(reader["HotelID"].ToString());
                    packageEntity.Code = reader["Code"].ToString();
                    packageEntity.RoomID = Convert.ToInt32(reader["RoomID"].ToString());
                    packageEntity.Brief = reader["Brief"].ToString();
                    packageEntity.SerialNO = reader["SerialNO"].ToString();
                    packageEntity.PriceSource = Convert.ToInt32(reader["PriceSource"].ToString());

                    packageList.Add(packageEntity);
                }
            }

            return packageList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prateMatchOta"></param>
        /// <returns></returns>
        public static int UpdatePRateMatchOta(PRateMatchOtaEntity prateMatchOta)
        {
            var sql = string.Format(@"
insert into HotelDB.dbo.PRateMatchOta 
(PID,Date,PriceSource,Price,SellState)
values ({0},'{1}',{2},{3},{4})", prateMatchOta.PID, prateMatchOta.Date.ToString("yyyy-MM-dd"), prateMatchOta.PriceSource, prateMatchOta.Price, prateMatchOta.SellState);

            return ExcSql(sql);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="notMatchOta"></param>
        /// <returns></returns>
        public static int UpdatePackageNotMatchOta(PackageNotMatchOta notMatchOta)
        {
            var sql = string.Format(@"
IF NOT EXISTS( select * from HotelDB.dbo.PackageNotMatchOta where HotelID = '{0}' and PID = '{1}' and Code = '{2}' and Brief = '{3}')
insert into HotelDB.dbo.PackageNotMatchOta (HotelID,PID,Code,Brief) values ('{0}','{1}','{2}','{3}')", notMatchOta.HotelID, notMatchOta.PID, notMatchOta.Code, notMatchOta.Brief);

            return ExcSql(sql);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static int SP4_PRate_UpdateWithOTAPrice()
        {
            var sql = string.Format(@"exec HotelDB.dbo.SP4_PRate_UpdateWithOTAPrice");

            return ExcSql(sql);
        }

        #endregion

        public static int ExcSql(string sql)
        {
            lock (mutex)
            {
                return DBHelper.ExecuteNonQuery(DBHelper.HotelDbConn, System.Data.CommandType.Text, sql);
            }
        }

        public static int ExcSql(string sql, SqlParameter[] commandParameters)
        {
            lock (mutex)
            {
                return DBHelper.ExecuteNonQuery(DBHelper.HotelDbConn, System.Data.CommandType.Text, sql, commandParameters);
            }
        }
    }
}
