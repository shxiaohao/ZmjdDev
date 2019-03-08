using HJD.Framework.WCF;
using HJD.HotelServices.Contracts;
using HJDAPI.Controllers.Common;
using HJDAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Controllers.Adapter
{
    public class PointsAdapter
    {
        public static IHotelService HotelService = ServiceProxyFactory.Create<IHotelService>("BasicHttpBinding_IHotelService");

        public const int PointsExpireYear = 2; //积分两年过期
        /// <summary>
        /// 消费用户积分
        /// </summary>
        /// <param name="param"></param>
        /// <returns>0：成功 其它：不成功</returns>
        public static int ConsumeUserPoints(ConsumeUserPointsParam param)
        {
            return HotelService.ConsumeUserPoints(param.userID, param.requiredPoints, param.typeID, param.businessID);
        }

        public static int RefundUserPoints(long userID, HJD.HotelServices.Contracts.HotelServiceEnums.ConsumeUserPointsBizType typeID, int businessID)
        {
            return HotelService.RefundUserPoints(userID, typeID, businessID);
        }

        public static List<PointsEntity> GetUserPointsList(long userId)
        {
            return HotelService.GetPointsEntity(userId);
        }


        public static List<PointsConsumeEntity> GetPointsConsumeEntity(long userId)
        {
            return HotelService.GetPointsConsumeEntity(userId);
        }

        public static int PresentPoints(int bizID, HJD.HotelServices.Contracts.HotelServiceEnums.ConsumeUserPointsBizType type, long userID, int points)
        {

            PointsEntity pe = new PointsEntity()
                  {
                      BusinessID = bizID,
                      TypeID = (int)type,
                      UserID = userID,
                      TotalPoint = points,
                      LeavePoint = points,
                      Approver = 0,
                      ExpiredTime = DateTime.Now.AddYears(PointsExpireYear)
                  };//如果是活动通过赠送 需要设置过期日期
          return   HotelService.InsertOrUpdatePoints(pe);
        }
        public static PointResult GetPersonalPoint(long userId)
        {
            try
            {
                List<PointsEntity> pointsList = PointsAdapter.GetUserPointsList(userId);
                if (pointsList != null && pointsList.Count != 0)
                {
                    int canUsePoints = pointsList.Sum(i => i.LeavePoint);

                    BindObjectName4Points(pointsList);

                    List<PointsConsumeEntity> consumeRecordList = PointsAdapter.GetPointsConsumeEntity(userId);
                    consumeRecordList = (from i in consumeRecordList orderby i.CreateTime descending select i).ToList();
                    if (consumeRecordList != null && consumeRecordList.Count != 0)
                    {
                        BindObjectName4PointsConsume(consumeRecordList);
                    }

                    return new PointResult()
                    {
                        CanUsePoints = canUsePoints,
                        pointConsumeList = consumeRecordList,
                        pointList = pointsList
                    };
                }
                else
                {
                    return new PointResult()
                    {
                        CanUsePoints = 0,
                        pointList = new List<PointsEntity>(),
                        pointConsumeList = new List<PointsConsumeEntity>()
                    };
                }
            }
            catch( Exception err)
            {
                Log.WriteLog("GetPersonalPoint:" + err.Message + err.StackTrace);
                return new PointResult()
                {
                    CanUsePoints = 0,
                    pointList = new List<PointsEntity>(),
                    pointConsumeList = new List<PointsConsumeEntity>()
                };
            }
        }

        /// <summary>
        /// key and value
        /// </summary>
        /// <param name="pointsList"></param>
        /// <returns></returns>
        private static void BindObjectName4Points(List<PointsEntity> pointsList)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (pointsList != null && pointsList.Count != 0)
            {
                foreach (var c in pointsList.GroupBy(i => i.TypeCode))
                {
                    string typeCode = c.Select(i => i.TypeCode).First();
                    if (string.IsNullOrEmpty(typeCode))
                    {
                        continue;
                    }
                    List<long> sourceIds = c.Select(i => (long)i.BusinessID).Distinct().ToList();
                    List<SourceIDAndObjectNameEntity> commentObjectNameList = HotelController.HotelService.GetObjectNamesByTypeCode(sourceIds, typeCode, 0);

                    foreach (SourceIDAndObjectNameEntity temp in commentObjectNameList)
                    {
                        string key = typeCode + temp.SourceID;
                        dic.Add(key, temp.ObjectName);
                    }
                }
                foreach (PointsEntity ac in pointsList)
                {
                    string key = ac.TypeCode + ac.BusinessID;
                    if (dic.ContainsKey(key))
                    {
                        ac.ObjectName = dic[key];
                    }
                }
            }
            //return dic;
        }

        /// <summary>
        /// key and value
        /// </summary>
        /// <param name="consumeRecordList"></param>
        /// <returns></returns>
        private static void BindObjectName4PointsConsume(List<PointsConsumeEntity> consumeRecordList)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (consumeRecordList != null && consumeRecordList.Count != 0)
            {
                foreach (var c in consumeRecordList.GroupBy(i => i.TypeCode))
                {
                    string typeCode = c.Select(i => i.TypeCode).First();
                    if (string.IsNullOrEmpty(typeCode))
                    {
                        continue;
                    }
                    List<long> sourceIds = c.Select(i => (long)i.BusinessID).Distinct().ToList();
                    List<SourceIDAndObjectNameEntity> commentObjectNameList = HotelController.HotelService.GetObjectNamesByTypeCode(sourceIds, typeCode, 0);

                    foreach (SourceIDAndObjectNameEntity temp in commentObjectNameList)
                    {
                        string key = typeCode + temp.SourceID;
                        dic.Add(key, temp.ObjectName);
                    }
                }

                foreach (PointsConsumeEntity ac in consumeRecordList)
                {
                    string key = ac.TypeCode + ac.BusinessID;
                    if (dic.ContainsKey(key))
                    {
                        ac.ObjectName = dic[key];
                    }
                }
            }
            //return dic;
        }
    }
}
