using HJD.AccessService.Contract.Model;
using HJD.AccessService.Contract.Model.Acount;
using HJD.AccessService.Contract.Model.Fund;
using HJD.AccessService.Implement.Helper;
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
    public class WeixinHelper
    {
        #region select

        /// <summary>
        /// 【常规免费住】检查当前用户Openid还可以生成几个抽奖码（规则：阅读数满3/满5/满n次获得一个抽奖码）
        /// </summary>
        /// <returns></returns>
        public static int GetWeixinUserHaveLuckCodeCount(ActiveWeixinLuckCode aluck)
        {
            var count = 0;

//            var sql = string.Format(@"
//select 
//(select COUNT(1)/3 from ActiveWeixinShareRead where ActiveId = {0} and SharerOpenId = '{1}')
//-
//(select COUNT(1) from ActiveWeixinLuckCode where ActiveId = {0} and OpenId = '{1}' and PartnerId = 0 and tagname = '分享奖励')
//genCount", aluck.ActiveId, aluck.Openid);
            
            var sql = string.Format(@"
DECLARE @PersonMaxLucks int
select @PersonMaxLucks = PersonMaxLucks from ActiveRule where ActiveId = {0}

DECLARE @HasLucks int
select @HasLucks = COUNT(1) from ActiveWeixinLuckCode where ActiveId = {0} and OpenId = '{1}' and PartnerId = 0 and tagname = '分享奖励'

IF @PersonMaxLucks = 0 or (@HasLucks+1) < @PersonMaxLucks
select (select COUNT(1)/3 from ActiveWeixinShareRead where ActiveId = {0} and SharerOpenId = '{1}')-@HasLucks genCount
ELSE
select 0 genCount
", aluck.ActiveId, aluck.Openid);

            var reader = DatabaseHelper.ExecuteReader(DatabaseHelper.CommDbConn, System.Data.CommandType.Text, sql);
            using (reader)
            {
                while (reader.Read())
                {
                    var obj = new UserRecommendRel();
                    count = Convert.ToInt32(reader["genCount"].ToString());
                }
            }

            return count;
        }

        /// <summary>
        /// 【微信红包联欢】检查当前用户Openid还可以生成几个抽奖码（规则：阅读数满1/满3/满5/满n次获得一个抽奖码）
        /// </summary>
        /// <returns></returns>
        public static int GetWeixinUserHaveLuckCodeCountForRedpack(ActiveWeixinLuckCode aluck)
        {
            var count = 0;

            var sql = string.Format(@"
select 
(select COUNT(1) from ActiveWeixinShareRead where ActiveId = {0} and SharerOpenId = '{1}')
-
(select isnull(SUM(Value),0) from ActiveWeixinFicMoney where ActiveId = {0} and OpenId = '{1}' and PartnerId = 0 and Remark = '分享奖励')
genCount", aluck.ActiveId, aluck.Openid);

            var reader = DatabaseHelper.ExecuteReader(DatabaseHelper.CommDbConn, System.Data.CommandType.Text, sql);
            using (reader)
            {
                while (reader.Read())
                {
                    var obj = new UserRecommendRel();
                    count = Convert.ToInt32(reader["genCount"].ToString());
                }
            }

            return count;
        }
        
        /// <summary>
        /// 根据WeixinMediaIdId查询存在的记录条数
        /// </summary>
        /// <param name="weixinMediaId"></param>
        /// <returns></returns>
        public static int GetWeixinMaterialCountByMediaId(string weixinMediaId)
        {
            var count = 0;

            var sql = string.Format(@"select COUNT(1) getCount from [commdb].[dbo].[WeixinMaterial] where WeiXinMediaID = '{0}'", weixinMediaId);

            var reader = DatabaseHelper.ExecuteReader(DatabaseHelper.CommDbConn, System.Data.CommandType.Text, sql);
            using (reader)
            {
                while (reader.Read())
                {
                    var obj = new UserRecommendRel();
                    count = Convert.ToInt32(reader["getCount"].ToString());
                }
            }

            return count;
        }

        /// <summary>
        /// 根据Title查询存在的记录条数
        /// </summary>
        /// <param name="title"></param>
        /// <param name="cid"></param>
        /// <returns></returns>
        public static int GetWeixinMaterialCountByTitle(string title, int cid)
        {
            var count = 0;

            var sql = string.Format(@"select COUNT(1) getCount from [commdb].[dbo].[WeixinMaterial] where title = '{0}' and CategoryId = {1}", title, cid);

            var reader = DatabaseHelper.ExecuteReader(DatabaseHelper.CommDbConn, System.Data.CommandType.Text, sql);
            using (reader)
            {
                while (reader.Read())
                {
                    var obj = new UserRecommendRel();
                    count = Convert.ToInt32(reader["getCount"].ToString());
                }
            }

            return count;
        }

        #region 全员分享红包奖励相关查询

        /// <summary>
        /// 获取待发送的红包奖励记录
        /// </summary>
        /// <returns></returns>
        public static List<WeixinRewardRecord> GetWeixinRewardWillSendRecord()
        {
            var list = new List<WeixinRewardRecord>();

            var sql = string.Format(@"
Select ID,SourceId,SourceType,ReOpenid,Wishing,Amount,Number,ActiveId,ActiveName,Remark,SceneId,SendName,WillSendTime,RealSendTime,State 
From commdb.dbo.WeixinRewardRecord 
Where WillSendTime < '{0}' And State = 0 
Order by WillSendTime"
                , DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            var reader = DatabaseHelper.ExecuteReader(DatabaseHelper.CommDbConn, System.Data.CommandType.Text, sql);
            using (reader)
            {
                while (reader.Read())
                {
                    var obj = new WeixinRewardRecord();
                    obj.ID = Convert.ToInt32(reader["ID"].ToString());
                    obj.SourceId = Convert.ToInt64(reader["SourceId"].ToString());
                    obj.SourceType = Convert.ToInt32(reader["SourceType"].ToString());
                    obj.ReOpenid = reader["ReOpenid"].ToString();
                    obj.Wishing = Convert.ToString(reader["Wishing"].ToString());
                    obj.Amount = Convert.ToInt32(reader["Amount"].ToString());
                    obj.Number = Convert.ToInt32(reader["Number"].ToString());
                    obj.ActiveId = Convert.ToInt32(reader["ActiveId"].ToString());
                    obj.ActiveName = reader["ActiveName"].ToString();
                    obj.Remark = reader["Remark"].ToString();
                    obj.SceneId = Convert.ToInt32(reader["SceneId"].ToString());
                    obj.SendName = reader["SendName"].ToString();
                    obj.WillSendTime = Convert.ToDateTime(reader["WillSendTime"].ToString());
                    obj.RealSendTime = Convert.ToDateTime(reader["RealSendTime"].ToString());
                    obj.State = Convert.ToInt32(reader["State"].ToString());
                    list.Add(obj);
                }
            }

            return list;
        }

        #endregion

        #endregion

        #region insert or update

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

        public static int InsertWeixinMaterial(WeixinMaterialModel obj)
        {
            var add = 0;

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@Type", SqlDbType.Int) { Value = obj.Type });
            parameters.Add(new SqlParameter("@Title", SqlDbType.NVarChar) { Value = obj.Title });
            parameters.Add(new SqlParameter("@Content", SqlDbType.NVarChar) { Value = obj.Content });
            parameters.Add(new SqlParameter("@WeiXinMediaID", SqlDbType.NVarChar) { Value = obj.WeiXinMediaID });
            parameters.Add(new SqlParameter("@Digest", SqlDbType.NVarChar) { Value = obj.Digest });
            parameters.Add(new SqlParameter("@ThumbMediaId", SqlDbType.NVarChar) { Value = obj.ThumbMediaId });
            parameters.Add(new SqlParameter("@Author", SqlDbType.NVarChar) { Value = obj.Author });
            parameters.Add(new SqlParameter("@Url", SqlDbType.NVarChar) { Value = obj.Url });
            parameters.Add(new SqlParameter("@ContentSourceUrl", SqlDbType.NVarChar) { Value = obj.ContentSourceUrl });
            parameters.Add(new SqlParameter("@SourceUpdateTime", SqlDbType.DateTime) { Value = obj.SourceUpdateTime });
            parameters.Add(new SqlParameter("@Creator", SqlDbType.BigInt) { Value = obj.Creator });
            parameters.Add(new SqlParameter("@CreateTime", SqlDbType.DateTime) { Value = obj.CreateTime });
            parameters.Add(new SqlParameter("@Editor", SqlDbType.BigInt) { Value = obj.Editor });
            parameters.Add(new SqlParameter("@UpdateTime", SqlDbType.DateTime) { Value = obj.UpdateTime });
            parameters.Add(new SqlParameter("@WeixinAcountId", SqlDbType.Int) { Value = obj.WeixinAcountId });
            parameters.Add(new SqlParameter("@CategoryId", SqlDbType.Int) { Value = obj.CategoryId });
            parameters.Add(new SqlParameter("@State", SqlDbType.Int) { Value = obj.State });

            var addsql = @"
IF NOT EXISTS( select WeiXinMediaID from [commdb].[dbo].[WeixinMaterial] where title = @Title)
begin
INSERT INTO [commdb].[dbo].[WeixinMaterial]
           ([Type]
           ,[Title]
           ,[Content]
           ,[WeiXinMediaID]
           ,[Digest]
           ,[ThumbMediaId]
           ,[Author]
           ,[Url]
           ,[ContentSourceUrl]
           ,[SourceUpdateTime]
           ,[Creator]
           ,[CreateTime]
           ,[Editor]
           ,[UpdateTime]
           ,[WeixinAcountId]
           ,[CategoryId]
           ,[State])
     VALUES
           (@Type
           ,@Title
           ,@Content
           ,@WeixinMediaID
           ,@Digest
           ,@ThumbMediaId
           ,@Author
           ,@Url
           ,@ContentSourceUrl
           ,@SourceUpdateTime
           ,@Creator
           ,@CreateTime
           ,@Editor
           ,@UpdateTime
           ,@WeixinAcountId
           ,@CategoryId
           ,@State
           )
end
";

            add = ExcSql(addsql, parameters.ToArray(), DatabaseHelper.CommDbConn);

            return add;
        }

        /// <summary>
        /// 修改指定待发奖励红包记录的state
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public static int UpdateWeixinRewardRecordState(int id, int state)
        {
            var add = 0;

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@ID", SqlDbType.Int) { Value = id });
            parameters.Add(new SqlParameter("@State", SqlDbType.Int) { Value = state });

            var addsql = @"
DECLARE @retcode INT 
BEGIN TRANSACTION
SET @retcode = 0  

update commdb.dbo.WeixinRewardRecord set State = @State,RealSendTime = GETDATE() where ID = @ID

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
        /// 扣除相关合作伙伴的指定资金
        /// </summary>
        /// <param name="partnerId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int UpdateWxPartnerActiveFund(long partnerId, decimal value)
        {
            var add = 0;

            var parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@ID", SqlDbType.Int) { Value = partnerId });
            parameters.Add(new SqlParameter("@Value", SqlDbType.Decimal) { Value = value });

            var addsql = @"
DECLARE @retcode INT 
BEGIN TRANSACTION
SET @retcode = 0  

update commdb..ActiveWeixinPartner set ActiveFund = ActiveFund - @Value where id = @ID

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

        public static int ExcSql(string sql, string connectString)
        {
            return DatabaseHelper.ExecuteNonQuery(connectString, System.Data.CommandType.Text, sql);
        }

        public static int ExcSql(string sql, SqlParameter[] commandParameters)
        {
            return DatabaseHelper.ExecuteNonQuery(DatabaseHelper.AcountDbConn, System.Data.CommandType.Text, sql, commandParameters);
        }

        public static int ExcSql(string sql, SqlParameter[] commandParameters, string connectString)
        {
            return DatabaseHelper.ExecuteNonQuery(connectString, System.Data.CommandType.Text, sql, commandParameters);
        }
    }
}
