using HJD.AccessService.Contract.Model.Acount;
using HJD.AccessService.Contract.Model.Fund;
using HJD.AccessServiceTask.Helper;
using HJD.WeixinServices.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HJD.AccessServiceTask.Job.Helper
{
    public class AcountHelper
    {
        #region select

        /// <summary>
        /// 查询所有已经注册的被推荐人
        /// </summary>
        /// <returns></returns>
        public static List<OriginCoupon> GetRegisteredRecommendUserInfo()
        {
            var list = new List<OriginCoupon>();
            var sql = @"
select * from
(
select oc.ID,oc.sourceId,oc.typeId,oc.userId,oc.cashMoney,oc.totalMoney,oc.createTime,oc.acquiredTime,oc.state,oc.expiredTime,oc.GUID,uiex.RegisterTime 
from coupondb.dbo.OriginCoupon oc
inner join accountdb.dbo.User_Info ui on ui.UserID = oc.userId and ui.MobileState = 2
inner join accountdb.dbo.User_Info_Ex uiex on ui.UserID = uiex.UserID
where oc.typeId = 8 and oc.sourceId > 0 and oc.sourceId <> oc.userId

union

select oc.ID,o.UserID as sourceId,oc.typeId,oc.userId,oc.cashMoney,oc.totalMoney,oc.createTime,oc.acquiredTime,oc.state,oc.expiredTime,oc.GUID,uiex.RegisterTime 
from coupondb.dbo.OriginCoupon oc
inner join accountdb.dbo.User_Info ui on ui.UserID = oc.userId and ui.MobileState = 2
inner join accountdb.dbo.User_Info_Ex uiex on ui.UserID = uiex.UserID
inner join hoteldb.dbo.Orders o on o.OrderID = oc.sourceId
where oc.typeId = 100 and oc.sourceId > 0 and oc.state = 1 and o.UserID <> oc.userId and oc.createTime >= '2016-04-05'
) a
order by a.createTime,a.userId
";

            var reader = DatabaseHelper.ExecuteReader(DatabaseHelper.AcountDbConn, System.Data.CommandType.Text, sql);
            using (reader)
            {
                while (reader.Read())
                {
                    var obj = new OriginCoupon();
                    obj.Id = Convert.ToInt32(reader["ID"].ToString());
                    obj.UserId = Convert.ToInt64(reader["UserID"].ToString());
                    obj.SourceId = Convert.ToInt64(reader["sourceId"].ToString());
                    obj.TypeId = Convert.ToInt32(reader["typeId"].ToString());
                    obj.UserId = Convert.ToInt64(reader["userId"].ToString());
                    obj.CashMoney = Convert.ToDecimal(reader["cashMoney"].ToString());
                    obj.TotalMoney = Convert.ToDecimal(reader["totalMoney"].ToString());
                    obj.CreateTime = Convert.ToDateTime(reader["createTime"].ToString());
                    obj.AcquiredTime = Convert.ToDateTime((!string.IsNullOrEmpty(reader["acquiredTime"].ToString()) ? reader["acquiredTime"].ToString() : "2000-01-01"));
                    obj.State = Convert.ToInt32(reader["state"].ToString());
                    obj.ExpiredTime = Convert.ToDateTime((!string.IsNullOrEmpty(reader["expiredTime"].ToString()) ? reader["expiredTime"].ToString() : "2000-01-01"));
                    obj.GUID = reader["GUID"].ToString();
                    obj.RegisterTime = Convert.ToDateTime((!string.IsNullOrEmpty(reader["RegisterTime"].ToString()) ? reader["RegisterTime"].ToString() : "2000-01-01"));
                    list.Add(obj);
                }
            }

            return list;
        }

        /// <summary>
        /// 获取所有的被推荐人数据
        /// </summary>
        /// <returns></returns>
        public static List<UserRecommendRel> GetAllRecommendRelList()
        {
            var list = new List<UserRecommendRel>();

            var sql = @"select ID,UserId,ReUserId,RecommendChannel,RecommendDate,ReRegisterDate from accountdb.dbo.User_recommendrel where RecommendDate <= '2017-04-28 13:00:00' order by id";

            var reader = DatabaseHelper.ExecuteReader(DatabaseHelper.AcountDbConn, System.Data.CommandType.Text, sql);
            using (reader)
            {
                while (reader.Read())
                {
                    var obj = new UserRecommendRel();
                    obj.Id = Convert.ToInt32(reader["ID"].ToString());
                    obj.UserId = Convert.ToInt64(reader["UserID"].ToString());
                    obj.ReUserId = Convert.ToInt64(reader["ReUserId"].ToString());
                    obj.RecommendChannel = Convert.ToInt32(reader["RecommendChannel"].ToString());
                    obj.RecommendDate = Convert.ToDateTime(reader["RecommendDate"].ToString());
                    obj.ReRegisterDate = Convert.ToDateTime(reader["ReRegisterDate"].ToString());
                    list.Add(obj);
                }
            }


            return list;
        }

        /// <summary>
        /// 查询出还有哪些推荐人没有创建用户基金记录
        /// </summary>
        /// <returns></returns>
        public static List<UserRecommendRel> GetNoFundRecommendUsers()
        {
            var list = new List<UserRecommendRel>();

            var sql = @"
select distinct ur.UserId from accountdb.dbo.User_recommendrel ur
left join funddb.dbo.UserFund uf on uf.UserId = ur.UserId
where uf.Id is null";

            var reader = DatabaseHelper.ExecuteReader(DatabaseHelper.AcountDbConn, System.Data.CommandType.Text, sql);
            using (reader)
            {
                while (reader.Read())
                {
                    var obj = new UserRecommendRel();
                    obj.UserId = Convert.ToInt64(reader["UserID"].ToString());
                    list.Add(obj);
                }
            }

            return list;
        }

        /// <summary>
        /// 获取指定用户的指定日期完成的订单相关信息
        /// </summary>
        /// <returns></returns>
        public static List<CheckInOrderInfo> GetCheckInOrderInfoByUserAndDate(DateTime date, long userId)
        {
            var list = new List<CheckInOrderInfo>();

            var sql = string.Format(@"
select o.ID,o.OrderID,o.Amount,o.SubmitDate,Convert(datetime,op.CheckIn) CheckIn,op.NightCount,o.UserID,o.HotelID,h.HotelName
from hoteldb.dbo.Orders o
join hoteldb.dbo.OrderPackage op on op.ID = o.ID
join hoteldb.dbo.hotel h on h.HotelId = o.HotelID
where o.State = 12 and dateadd(DAY, op.NightCount, op.CheckIn) = '{0}' and o.UserID = '{1}'", date.ToString("yyyy-MM-dd"), userId);

            var reader = DatabaseHelper.ExecuteReader(DatabaseHelper.AcountDbConn, System.Data.CommandType.Text, sql);
            using (reader)
            {
                while (reader.Read())
                {
                    var obj = new CheckInOrderInfo();
                    obj.Id = Convert.ToInt32(reader["ID"].ToString());
                    obj.OrderID = Convert.ToInt64(reader["OrderID"].ToString());
                    obj.Amount = Convert.ToDecimal(reader["Amount"].ToString());
                    obj.SubmitDate = Convert.ToDateTime(reader["SubmitDate"].ToString());
                    obj.CheckIn = Convert.ToDateTime(reader["CheckIn"].ToString());
                    obj.NightCount = Convert.ToInt32(reader["NightCount"].ToString());
                    obj.UserID = Convert.ToInt64(reader["UserID"].ToString());
                    obj.HotelID = Convert.ToInt32(reader["HotelID"].ToString());
                    obj.HotelName = Convert.ToString(reader["HotelName"].ToString());
                    list.Add(obj);
                }
            }

            return list;
        }

        /// <summary>
        /// 获取指定用户指定日期的奖励明细
        /// </summary>
        /// <param name="date"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static List<UserFundIncomeDetail> GetFundIncomeDetailByUser(DateTime date, long userId)
        {
            var list = new List<UserFundIncomeDetail>();

            var sql = string.Format(@"
select Id,UserId,TypeId,Fund,Label,RelationUserId,OriginOrderId,OriginAmount,OriCreateTime,CreateTime 
from FundDB.dbo.UserFundIncomeDetail
where userid = '{0}' and oricreatetime >= '{1}' and oricreatetime < '{2}'
order by userid,typeid,oricreatetime", userId, date.Date.ToString("yyyy-MM-dd"), date.Date.AddDays(1).ToString("yyyy-MM-dd"));

            var reader = DatabaseHelper.ExecuteReader(DatabaseHelper.AcountDbConn, System.Data.CommandType.Text, sql);
            using (reader)
            {
                while (reader.Read())
                {
                    var obj = new UserFundIncomeDetail();
                    obj.Id = Convert.ToInt32(reader["ID"].ToString());
                    obj.UserId = Convert.ToInt64(reader["UserId"].ToString());
                    obj.TypeId = Convert.ToInt32(reader["TypeId"].ToString());
                    obj.Fund = Convert.ToDecimal(reader["Fund"].ToString());
                    obj.Label = Convert.ToString(reader["Label"].ToString());
                    obj.RelationUserId = Convert.ToInt64(reader["RelationUserId"].ToString());
                    obj.OriginOrderId = Convert.ToInt64(reader["OriginOrderId"].ToString());
                    obj.OriginAmount = Convert.ToDecimal(reader["OriginAmount"].ToString());
                    obj.OriCreateTime = Convert.ToDateTime(reader["OriCreateTime"].ToString());
                    obj.CreateTime = Convert.ToDateTime(reader["CreateTime"].ToString());
                    list.Add(obj);
                }
            }

            return list;
        }

        /// <summary>
        /// 查询出还没有奖励1%住基金的铂金会员的所有已完成的订单
        /// </summary>
        /// <param name="customerType">5 铂金会员</param>
        /// <returns></returns>
        public static List<CheckInOrderInfo> GetCheckInOrderInfoByVipUser(int customerType = 5)
        {
            var list = new List<CheckInOrderInfo>();

//            var sql = string.Format(@"
//select o.ID,o.OrderID,o.Amount,o.SubmitDate,Convert(datetime,op.CheckIn) CheckIn,op.NightCount,o.UserID,o.HotelID,h.HotelName,ufi.OriginOrderId
//from hoteldb.dbo.Orders o
//join hoteldb.dbo.OrderPackage op on op.ID = o.ID
//join hoteldb.dbo.hotel h on h.HotelId = o.HotelID
//left join funddb.dbo.UserFundIncomeDetail ufi on ufi.TypeId = 9 and ufi.UserId = o.UserID and ufi.OriginOrderId = o.OrderID
//where o.State = 12 and op.CustomerType = {0} and dateadd(DAY, op.NightCount, op.CheckIn) < GETDATE() and ufi.OriginOrderId is null", customerType);

            var sql = string.Format(@"
select o.ID,o.OrderID,o.Amount,o.SubmitDate,Convert(datetime,op.CheckIn) CheckIn,op.NightCount,o.UserID,o.HotelID,h.HotelName,ufi.OriginOrderId
from hoteldb.dbo.Orders o
join hoteldb.dbo.OrderPackage op on op.ID = o.ID
join hoteldb.dbo.hotel h on h.HotelId = o.HotelID
left join funddb.dbo.UserFundIncomeDetail ufi on ufi.TypeId = 9 and ufi.UserId = o.UserID and ufi.OriginOrderId = o.OrderID
left join accountdb.dbo.UserRight_UserRoleRel ur on ur.UserID = o.UserID
where o.State = 12 and ur.RoleID = 17 and dateadd(DAY, op.NightCount, op.CheckIn) > '2017-01-01' and dateadd(DAY, op.NightCount, op.CheckIn) < GETDATE() and ufi.OriginOrderId is null");

            var reader = DatabaseHelper.ExecuteReader(DatabaseHelper.AcountDbConn, System.Data.CommandType.Text, sql);
            using (reader)
            {
                while (reader.Read())
                {
                    var obj = new CheckInOrderInfo();
                    obj.Id = Convert.ToInt32(reader["ID"].ToString());
                    obj.OrderID = Convert.ToInt64(reader["OrderID"].ToString());
                    obj.Amount = Convert.ToDecimal(reader["Amount"].ToString());
                    obj.SubmitDate = Convert.ToDateTime(reader["SubmitDate"].ToString());
                    obj.CheckIn = Convert.ToDateTime(reader["CheckIn"].ToString());
                    obj.NightCount = Convert.ToInt32(reader["NightCount"].ToString());
                    obj.UserID = Convert.ToInt64(reader["UserID"].ToString());
                    obj.HotelID = Convert.ToInt32(reader["HotelID"].ToString());
                    obj.HotelName = Convert.ToString(reader["HotelName"].ToString());
                    list.Add(obj);
                }
            }

            return list;
        }

        #endregion

        #region insert or update

        /// <summary>
        /// 查询用户推荐被推荐的主从关系表数据
        /// </summary>
        /// <param name="oriCouponEntity"></param>
        /// <returns></returns>
        public static int InsertUserRecommendRelInfo(UserRecommendRel obj)
        {
            var add = 0;

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@UserId", SqlDbType.BigInt) { Value = obj.UserId });
            parameters.Add(new SqlParameter("@ReUserId", SqlDbType.BigInt) { Value = obj.ReUserId });
            parameters.Add(new SqlParameter("@RecommendChannel", SqlDbType.Int) { Value = obj.RecommendChannel });
            parameters.Add(new SqlParameter("@RecommendDate", SqlDbType.DateTime) { Value = obj.RecommendDate });
            parameters.Add(new SqlParameter("@ReRegisterDate", SqlDbType.DateTime) { Value = obj.ReRegisterDate });

            var addsql = @"
DECLARE @retcode INT 
BEGIN TRANSACTION
SET @retcode = 0  

IF NOT EXISTS( select ID from accountdb.dbo.User_RecommendRel where ReUserId = @ReUserId)
begin
Insert into accountdb.dbo.User_RecommendRel values (@UserId,@ReUserId,@RecommendChannel,@RecommendDate,@ReRegisterDate)
end

Exit_Flag:   
IF @@TranCount > 0
    IF @RetCode = 0
        COMMIT TRANSACTION
    ELSE
        ROLLBACK TRANSACTION
SELECT @retcode AS ExecInfo
";

            add = ExcSql(addsql, parameters.ToArray());

            return add;
        }

        /// <summary>
        /// 创建用户基金记录
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int InsertUserFundInfo(UserFund obj)
        {
            var add = 0;

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@UserId", SqlDbType.BigInt) { Value = obj.UserId });
            parameters.Add(new SqlParameter("@TotalFund", SqlDbType.NVarChar) { Value = obj.TotalFund });
            parameters.Add(new SqlParameter("@CreateTime", SqlDbType.DateTime) { Value = obj.CreateTime });
            parameters.Add(new SqlParameter("@UpdateTime", SqlDbType.DateTime) { Value = obj.UpdateTime });

            var addsql = @"
DECLARE @retcode INT 
BEGIN TRANSACTION
SET @retcode = 0  

IF NOT EXISTS( select Id from funddb.dbo.UserFund where UserId = @UserId)
begin
Insert into funddb.dbo.UserFund values (@UserId,@TotalFund,GETDATE(),GETDATE())
end

Exit_Flag:   
IF @@TranCount > 0
    IF @RetCode = 0
        COMMIT TRANSACTION
    ELSE
        ROLLBACK TRANSACTION
SELECT @retcode AS ExecInfo
";

            add = ExcSql(addsql, parameters.ToArray());

            return add;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int InsertUserFundIncomeDetail(UserFundIncomeDetail obj)
        {
            var add = 0;

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@UserId", SqlDbType.BigInt) { Value = obj.UserId });
            parameters.Add(new SqlParameter("@TypeId", SqlDbType.NVarChar) { Value = obj.TypeId });
            parameters.Add(new SqlParameter("@Fund", SqlDbType.NVarChar) { Value = obj.Fund });
            parameters.Add(new SqlParameter("@Label", SqlDbType.NVarChar) { Value = obj.Label });
            parameters.Add(new SqlParameter("@RelationUserId", SqlDbType.BigInt) { Value = obj.RelationUserId });
            parameters.Add(new SqlParameter("@OriginOrderId", SqlDbType.BigInt) { Value = obj.OriginOrderId });
            parameters.Add(new SqlParameter("@OriginAmount", SqlDbType.NVarChar) { Value = obj.OriginAmount });
            parameters.Add(new SqlParameter("@OriCreateTime", SqlDbType.DateTime) { Value = obj.OriCreateTime });
            parameters.Add(new SqlParameter("@CreateTime", SqlDbType.DateTime) { Value = obj.CreateTime });

            var addsql = @"
DECLARE @retcode INT 
BEGIN TRANSACTION
SET @retcode = 0  

IF NOT EXISTS( select Id from funddb.dbo.UserFundIncomeDetail where RelationUserId = @RelationUserId and TypeId = @TypeId and OriginOrderId = @OriginOrderId and OriCreateTime = @OriCreateTime)
begin
Insert into funddb.dbo.UserFundIncomeDetail values (@UserId,@TypeId,@Fund,@Label,@RelationUserId,@OriginOrderId,@OriginAmount,@OriCreateTime,@CreateTime)
end

Exit_Flag:   
IF @@TranCount > 0
    IF @RetCode = 0
        COMMIT TRANSACTION
    ELSE
        ROLLBACK TRANSACTION
SELECT @retcode AS ExecInfo
";

            add = ExcSql(addsql, parameters.ToArray());

            return add;
        }

        /// <summary>
        /// 插入/更新指定用户的朋友注册奖励统计记录
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int InsertOrUpdateFundIncomeStat_1(UserFundIncomeStat obj)
        {
            var add = 0;

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@UserId", SqlDbType.BigInt) { Value = obj.UserId });
            parameters.Add(new SqlParameter("@Day", SqlDbType.NVarChar) { Value = obj.Day });
            parameters.Add(new SqlParameter("@TypeId", SqlDbType.Int) { Value = obj.TypeId });
            parameters.Add(new SqlParameter("@Fund", SqlDbType.NVarChar) { Value = obj.Fund });
            parameters.Add(new SqlParameter("@Label", SqlDbType.NVarChar) { Value = obj.Label });
            parameters.Add(new SqlParameter("@CreateTime", SqlDbType.DateTime) { Value = obj.CreateTime });

            var addsql = @"
DECLARE @retcode INT 
BEGIN TRANSACTION
SET @retcode = 0  

DECLARE @label2 nvarchar(200)
DECLARE @recount nvarchar(200)

select 
@label2 = 
'您推荐的 ' + 
CONVERT(nvarchar(10),count(distinct RelationUserId)) + 
' 位朋友已注册成功',
@recount = CONVERT(nvarchar(10),count(distinct RelationUserId))
from FundDB.dbo.UserFundIncomeDetail where userid = @UserId and typeid = @TypeId

--select @label

IF (recount > 0)
begin

IF NOT EXISTS( select Id from Funddb.dbo.UserFundIncomeStat where userid = @UserId and typeid = @TypeId)
begin
Insert into funddb.dbo.UserFundIncomeStat values (@UserId,@Day,@TypeId,@Fund,@label2,@CreateTime)
end
else
begin
update Funddb.dbo.UserFundIncomeStat set Label = @label2,[Day] = @Day,CreateTime = @CreateTime  where userid = @UserId and typeid = @TypeId
end

end

Exit_Flag:   
IF @@TranCount > 0
    IF @RetCode = 0
        COMMIT TRANSACTION
    ELSE
        ROLLBACK TRANSACTION
SELECT @retcode AS ExecInfo
";

            add = ExcSql(addsql, parameters.ToArray());

            return add;
        }

        /// <summary>
        /// 插入/更新指定用户的订单奖励统计记录
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int InsertOrUpdateFundIncomeStat_2(UserFundIncomeStat obj)
        {
            var add = 0;

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@UserId", SqlDbType.BigInt) { Value = obj.UserId });
            parameters.Add(new SqlParameter("@Day", SqlDbType.NVarChar) { Value = obj.Day });
            parameters.Add(new SqlParameter("@TypeId", SqlDbType.Int) { Value = obj.TypeId });
            parameters.Add(new SqlParameter("@Fund", SqlDbType.NVarChar) { Value = obj.Fund });
            parameters.Add(new SqlParameter("@Label", SqlDbType.NVarChar) { Value = obj.Label });
            parameters.Add(new SqlParameter("@CreateTime", SqlDbType.DateTime) { Value = obj.CreateTime });

            var addsql = @"
DECLARE @retcode INT 
BEGIN TRANSACTION
SET @retcode = 0  

IF NOT EXISTS( select Id from Funddb.dbo.UserFundIncomeStat where userid = @UserId and typeid = @TypeId and [day] = @Day)
begin
Insert into funddb.dbo.UserFundIncomeStat values (@UserId,@Day,@TypeId,@Fund,@Label,@CreateTime)
end
else
begin
update Funddb.dbo.UserFundIncomeStat set Fund = @Fund,CreateTime = @CreateTime where userid = @UserId and typeid = @TypeId and [day] = @Day
end

Exit_Flag:   
IF @@TranCount > 0
    IF @RetCode = 0
        COMMIT TRANSACTION
    ELSE
        ROLLBACK TRANSACTION
SELECT @retcode AS ExecInfo
";

            add = ExcSql(addsql, parameters.ToArray());

            return add;
        }

        /// <summary>
        /// 手动添加用户基金
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static int AddUserFund(UserFundIncomeDetail obj)
        {
            var add = 0;

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@UserId", SqlDbType.BigInt) { Value = obj.UserId });
            parameters.Add(new SqlParameter("@TypeId", SqlDbType.NVarChar) { Value = obj.TypeId });
            parameters.Add(new SqlParameter("@Fund", SqlDbType.NVarChar) { Value = obj.Fund });
            parameters.Add(new SqlParameter("@Label", SqlDbType.NVarChar) { Value = obj.Label });
            parameters.Add(new SqlParameter("@RelationUserId", SqlDbType.BigInt) { Value = obj.RelationUserId });
            parameters.Add(new SqlParameter("@OriginOrderId", SqlDbType.BigInt) { Value = obj.OriginOrderId });
            parameters.Add(new SqlParameter("@OriginAmount", SqlDbType.NVarChar) { Value = obj.OriginAmount });
            parameters.Add(new SqlParameter("@OriCreateTime", SqlDbType.DateTime) { Value = obj.OriCreateTime });

            var addsql = @"
EXECUTE funddb.dbo.SP_UserFund_Insert @UserId,@TypeId,@Fund,@Label,@RelationUserId,@OriginOrderId,@OriginAmount,@OriCreateTime
";

            add = ExcSql(addsql, parameters.ToArray());

            return add;
        }

        /// <summary>
        /// 重新计算用户基金记录
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static int ResetUserFund(Int64 userId)
        {
            var add = 0;

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@UserId", SqlDbType.BigInt) { Value = userId });

            var addsql = @"
DECLARE @retcode INT 
BEGIN TRANSACTION
SET @retcode = 0  

DECLARE @totalfund decimal(18,2) 

select @totalfund = ( isnull(SUM(ui.fund),0) - (select isnull(SUM(ue.fund),0) from funddb.dbo.UserFundExpendDetail ue where ue.UserId = @UserId) )
from funddb.dbo.UserFundIncomeDetail ui where ui.UserId = @UserId

update funddb.dbo.UserFund set TotalFund = @totalfund,UpdateTime = GETDATE() where UserId = @UserId

Exit_Flag:   
IF @@TranCount > 0
    IF @RetCode = 0
        COMMIT TRANSACTION
    ELSE
        ROLLBACK TRANSACTION
SELECT @retcode AS ExecInfo
";

            add = ExcSql(addsql, parameters.ToArray());

            return add;
        }

        #endregion

        #region delete


        #endregion

        public static int ExcSql(string sql)
        {
            return DatabaseHelper.ExecuteNonQuery(DatabaseHelper.AcountDbConn, System.Data.CommandType.Text, sql);
        }

        public static int ExcSql(string sql, SqlParameter[] commandParameters)
        {
            return DatabaseHelper.ExecuteNonQuery(DatabaseHelper.AcountDbConn, System.Data.CommandType.Text, sql, commandParameters);
        }
    }
}
