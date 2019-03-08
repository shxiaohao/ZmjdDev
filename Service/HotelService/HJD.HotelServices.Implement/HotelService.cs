using System;
using System.Collections.Generic;
using System.Linq;
using HJD.Framework.Interface;
using HJD.HotelServices.Contracts;
using System.Data;
using System.ServiceModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using HJD.Framework.WCF;
using System.Configuration;
using HJD.Contracts;
using HJD.HotelServices.Implement.Entity;
using System.Threading.Tasks;
using HJD.HotelServices.Implement;
using System.Collections.Concurrent;
using HJD.UGCRule.Contacts;
using HJD.HotelServices.Implement.Helper;
using HJD.HotelServices.Contracts.Comments;
using HJD.HotelServices.Implement.Business;

namespace HJD.HotelServices
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public partial class HotelService : IHotelService
    {
        private DateTime HeartbeatTime = DateTime.Now;
        private static IJobAssistantService JobAssistant = ServiceProxyFactory.Create<IJobAssistantService>("JobAssistantService");
        private static IHotelTagService hotelTagSvc = ServiceProxyFactory.Create<IHotelTagService>("HotelTagService");

        //private static ICacheProvider memCacheHotel = CacheManagerFactory.Create("HotelInfoCache");
        //private static ICacheProvider memCacheHotelReview = CacheManagerFactory.Create("HotelReviewCache");
        //private static ICacheProvider memCacheNewestHotelReview = CacheManagerFactory.Create("NewestHotelReviewCache");
        //private static ICacheProvider HotelListPriceCache = CacheManagerFactory.Create("HotelListPriceCache");

        private static IMemcacheProvider memCacheHotel = MemcacheManagerFactory.Create("HotelInfoCache");
        private static IMemcacheProvider memCacheHotelReview = MemcacheManagerFactory.Create("HotelReviewCache");
        private static IMemcacheProvider memCacheNewestHotelReview = MemcacheManagerFactory.Create("NewestHotelReviewCache");
        private static IMemcacheProvider HotelListPriceCache = MemcacheManagerFactory.Create("HotelListPriceCache");
        private static IMemcacheProvider memCacheHolidayPackage = MemcacheManagerFactory.Create("HolidayPackageCache");
        //private static IMemcacheProvider memCacheHotelPriceUpdate = MemcacheManagerFactory.Create("HotelPriceUpdateCache");

  
        private string CacheHotelVer { get { return ConfigurationManager.AppSettings["CacheHotelVer"]; } }
        private string CacheHotelReviewVer { get { return ConfigurationManager.AppSettings["CacheHotelReviewVer"]; } }
        private string HotelCtripPricePolicyCacheKey { get { return "HCPP" + CacheHotelReviewVer; } }
       private string HotelTFTReviewCacheKey { get { return "HTFTR" + CacheHotelReviewVer; } }
        private string HotelTFTRelCacheKey { get { return "HTFTRel" + CacheHotelReviewVer; } }
        private string HotelShowTagsCacheKey { get { return "HSTagS" + CacheHotelReviewVer; } }
        private string HotelInterestTagsCacheKey { get { return "HITagS" + CacheHotelReviewVer; } }
        private string HotelFaciCacheKey { get { return "HotelFaciltyB" + CacheHotelVer; } }
        private string ClassZoneCacheKey { get { return "ClassB" + CacheHotelVer; } }
        private string ClassStarCacheKey { get { return "ClassStar" + CacheHotelVer; } }
        private string ClassBrandCacheKey { get { return "ClassBrand" + CacheHotelVer; } }
        private string DistrictHotelClassCacheKey { get { return "DistrictHotelClassAB" + CacheHotelVer; } }
        private string DistinctHotelCountCacheKey { get { return "DistinctHC" + CacheHotelVer; } }
        private string DistinctHotelRelCountCacheKey { get { return "DistinctRelHC" + CacheHotelVer; } }
        private string NearHotelInfoCacheKey { get { return "NearHotelInfo" + CacheHotelVer; } }
        private string NearHotelInfoBySightCacheKey { get { return "NearHotelInfoBySight" + CacheHotelVer; } }//附近酒店通过景点
        private string NearbyHotelByRestaurantCacheKey { get { return "NearbyHotelByRestaurant" + CacheHotelVer; } }
        private string HotelInfoExCacheKey { get { return "HotelExABEDEFT" + CacheHotelVer; } }
        private string UnitInfoCacheKey { get { return "UnitInfoA" + CacheHotelVer; } }
        private string HotelClassCacheKey { get { return "HotelClass" + CacheHotelVer; } }
        private string ReviewUsefulUserCacheKey { get { return "ReviewUsefulUserA" + CacheHotelVer; } }
        private string HotelKeyWordKey { get { return "HotelKeyWordKeyFD" + CacheHotelVer; } }
        private string CommentKeyWordCacheKey { get { return "CommentKeyWordCacheKeyG" + CacheHotelVer; } }
        private string HotelSimlarCommentCacheKey { get { return "HotelSimlarCommentCacheKeyEA" + CacheHotelVer; } }
        private string HotelChannelCacheKey { get { return "HotelChannel" + CacheHotelVer; } }
        private string DistrictAirportTrainCacheKey { get { return "DistrictAirportTrain" + CacheHotelVer; } }
        private string DistrictPlaceMarkCacheKey { get { return "DistrictPlaceMark" + CacheHotelVer; } }
        private string HotelInfoClassExCacheKey { get { return "HotelInfoClass" + CacheHotelVer; } }
        private string CommentSummaryCacheKey { get { return "CommentSummaryA" + CacheHotelReviewVer; } }
        private string CommentExCacheKey { get { return "CommentB" + CacheHotelReviewVer; } }
        private string NewestHotelReviewCacheKey { get { return "NewestHotelReviewA" + CacheHotelReviewVer; } }
        private string CachedtHotelReviewKey { get { return "CachedtHotelReviewNewA"; } }
        private string GetHotelIDByHotelOriIDKey { get { return "GetHotelIDByHotelOriID" + CacheHotelVer; } }
        private string GetBookingURLKey { get { return "GetBookURL" + CacheHotelVer; } }
        private string GetAgodaURLKey { get { return "GetAgodaURL" + CacheHotelVer; } }
        private string TagParamsCacheKey { get { return "TagParams" + CacheHotelVer; } }
        private string TagsCacheKey { get { return "Tags" + CacheHotelVer; } }
        private string HotelTagCacheKey { get { return "HotelTag" + CacheHotelVer; } }
        private string HotelTagReviewCacheKey { get { return "HotelTagReview" + CacheHotelVer; } }
        private string QueryHotelTagListCacheKey { get { return "QueryHotelTagList" + CacheHotelVer; } }
        private string QueryHotelMenuListCacheKey { get { return "QueryHotelMenuList" + CacheHotelVer; } }

        private string HotelPhotosCacheKey { get { return "HotelPhotos" + CacheHotelVer; } }
        private string UserRecommendHotelListCacheKey { get { return "UserRecommendHL" + CacheHotelVer; } }

        /// <summary>
        /// 酒店区间价Memcache前缀。
        /// </summary>
        private string PrefixHotelListPrice { get { return "preHotelListPrice" + CacheHotelVer; } }

        private string PrefixSharePckageInfo { get { return "PrefixSharePckageInfo" + CacheHotelVer; } }

        private string CombineKey(int district, List<int> classID)
        {
            classID = classID ?? new List<int>();
            classID.Sort();
            return string.Format("{0}_{1}", district, string.Join<int>("_", classID));
        }

        public PackageEntity GetSharePckageInfoByPrice(int hotelid, int price)
        {
            return memCacheHotel.GetData<PackageEntity>(hotelid.ToString() + ":" + price.ToString(), PrefixSharePckageInfo, () =>
            {
                return HotelDAL.GetSharePckageInfoByPrice(hotelid, price);
            });
        }

        public int GetOrderHotelCountByUserId(long userId, int state, bool isDistinct)
        {
            if (userId == 0)
            {
                return 0;
            }
            List<int> list = HotelDAL.GetOrderHotelIdsByUserId(userId, state);
            return isDistinct ? list.Distinct<int>().Count<int>() : list.Count;
        }

        /// <summary>
        /// 获得个人酒店点评数量
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="isDistinct"></param>
        /// <returns></returns>
        public int GetCommentHotelCountByUserId(long userId, bool isDistinct)
        {
            if (userId == 0)
            {
                return 0;
            }
            List<int> list = HotelDAL.GetCommentHotelIdsByUserId(userId);
            return isDistinct ? list.Distinct<int>().Count<int>() : list.Count;
        }

        public  string GetOtaHotelUrl(string channel, int otaHotelID, string sourcePageType, string sType)
        {
           return  URLHelper.GetOtaHotelUrl( channel,  otaHotelID,  sourcePageType,  sType);
        }

        public List<HotelOTAEntity> GetOtaListByHotelID(int HotelID)
        {
            return URLHelper.GetOtaListByHotelID(HotelID);
        }
        public Dictionary<string,string> GetOtaUrlListByHotelID(int HotelID, string sourcePageType, string sType)
        {
            return URLHelper.GetOtaUrlListByHotelID(HotelID, sourcePageType, sType);
        }
        //public int AddInspectorRefHotel(long id, long userId, int hotelID, DateTime checkIn, DateTime checkOut)
        //{
        //    long evaHotelID = HotelDAL.ApplyInspectorHotel(id);
        //    //book
        //    if(evaHotelID != 0){
        //        //ToDo 完成插入新纪录 HotelDAL.up
        //        InspectorRefHotel eh = new InspectorRefHotel() {
        //            State = 1,
        //            CreateTime = DateTime.Now,
        //            UpdateTime = DateTime.Now,
        //            CheckInDate = checkIn,
        //            CheckOutDate = checkOut,
        //            Inspector = userId,
        //            InspectorHotel = (int)evaHotelID,
        //            HotelID = hotelID,
        //            CheckTime = DateTime.Now
        //        };
        //        return HotelDAL.InsertOrUpdateInspectorRefHotel(eh);
        //    }
        //    return 1;
        //}

        /// <summary>
        /// 申请配件酒店
        /// 检查剩余名额 先扣名额再扣除积分
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int BookInspectorRefHotelEx(InspectorRefHotel data)
        {
            long evaHotelID = HotelDAL.ApplyInspectorHotel(data.InspectorHotel);//累加报名人数
            if (evaHotelID != 0)
            {
                long evaHotelID20 = HotelDAL.CheckInspectorRefHotel(data.InspectorHotel, 1);//先占名额 抢占后返回一个
                if (evaHotelID20 > 0)
                {
                    //ToDo 完成插入新纪录 HotelDAL.up
                    InspectorRefHotel eh = new InspectorRefHotel()
                    {
                        State = 1,
                        CreateTime = DateTime.Now,
                        UpdateTime = DateTime.Now,
                        CheckInDate = data.CheckInDate,
                        CheckOutDate = data.CheckOutDate,
                        Inspector = data.Inspector,
                        InspectorHotel = data.InspectorHotel,
                        HotelID = data.HotelID,
                        CheckTime = DateTime.Now
                    };

                    int inspectorRefHotelID = HotelDAL.InsertOrUpdateInspectorRefHotel(eh);//增加一条品鉴申请记录
                    int flag2 = ConsumeInspectorHotelPoints(inspectorRefHotelID, data.InspectorHotel, data.Inspector);//继续更新相关的数据 如积分 剩余名额的扣除

                    return 0;
                }
            }
            return 1;//已没有名额
        }

        /// <summary>
        /// 申请房券
        /// 检查剩余名额 先扣名额再扣除积分
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string BookVoucherInspectorRefHotelEx(InspectorRefHotel data)
        {
            try
            {
                int needPoint = CalHotelVoucherPoints(data.HVID, data.CheckInDate, data.CheckOutDate);
                //计算申请人的积分
                List<PointsEntity> canUseEntityList = HotelDAL.GetPointsEntity(data.Inspector).FindAll(i => i.LeavePoint > 0);
                List<PointsEntity> finalEntity = new List<PointsEntity>();
                int sumDeductPoints = 0;
                foreach (PointsEntity temp in canUseEntityList)
                {
                    sumDeductPoints += temp.LeavePoint;
                }
                if (needPoint > sumDeductPoints)
                {
                    return "品鉴酒店需要" + needPoint + "积分";//积分不够
                }
                else
                {
                    long evaHotelID20 = HotelDAL.CheckVoucher(data.HVID, 1, data.Inspector, data.InspectorPhone);//先占名额 抢占后返回一个
                    if (evaHotelID20 > 0)
                    {
                        //ToDo 完成插入新纪录 HotelDAL.up
                        InspectorRefHotel eh = new InspectorRefHotel()
                        {
                            State = 1,
                            CreateTime = DateTime.Now,
                            UpdateTime = DateTime.Now,
                            CheckInDate = data.CheckInDate,
                            CheckOutDate = data.CheckOutDate,
                            Inspector = data.Inspector,
                            InspectorHotel = data.InspectorHotel,
                            HotelID = data.HotelID,
                            CheckTime = DateTime.Now,
                            HVID = data.HVID,
                            VID = Convert.ToInt32(evaHotelID20)
                        };
                        int inspectorRefHotelID = HotelDAL.InsertOrUpdateInspectorRefHotel(eh);//增加一条品鉴申请记录
                        //int flag2 = ConsumeHotelVoucherPoints(inspectorRefHotelID, data.HVID, data.Inspector, data.CheckInDate,data.CheckOutDate);//继续更新相关的数据 如积分 剩余名额的扣除
                        //扣除积分
                        ConsumeUserPoints(data.Inspector, needPoint, HJD.HotelServices.Contracts.HotelServiceEnums.ConsumeUserPointsBizType.FreeInspectHotel, inspectorRefHotelID);

                        return "";
                    }
                }
            }
            catch (Exception e)
            {
                LogHelper.WriteLog("BookVoucherInspectorRefHotelEx" + e);
            }
            return "名额已满";//已没有名额
        }


        public int CheckInspectorRefHotel(InspectorRefHotel eh)
        {
            int isPass = eh.State == 3 ? 1 : 0;
            long IDX = HotelDAL.CheckInspectorRefHotel(eh.InspectorHotel, isPass);
            if (IDX > 0)
            {
                return HotelDAL.InsertOrUpdateInspectorRefHotel(eh);
            }
            return 1;
        }

        public int CheckInspectorRefHotelEx(InspectorRefHotel eh, HJD.HotelServices.Contracts.HotelServiceEnums.ConsumeUserPointsBizType typeID)
        {
            int irhID = (int)eh.ID;
            int isPass = eh.State == 3 ? 1 : 0;
            long IDX = HotelDAL.CheckInspectorRefHotel(eh.InspectorHotel, isPass);
            if (IDX > 0)
            {
                int ss = HotelDAL.InsertOrUpdateInspectorRefHotel(eh);
                int ihID = (int)eh.InspectorHotel;
                InspectorHotel ih = HotelDAL.GetInspectorHotelById(ihID);
                int requiredPoints = ih.RequiredPoint;
                long userID = eh.Inspector;
                if (isPass == 1 && ss == 0 && requiredPoints > 0)
                {
                    ConsumeUserPoints(userID, requiredPoints, typeID, irhID);
                }
                else if (isPass == 0 && ss == 0)
                {
                    RefundUserPoints(userID, typeID, irhID);
                }
                return 0;
            }
            return 1;
        }

        /// <summary>
        /// 扣除积分
        /// </summary>
        /// <param name="inspectorRefHotelID"></param>
        /// <param name="inspectorHotelID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        private int ConsumeInspectorHotelPoints(int inspectorRefHotelID, int inspectorHotelID, long userID)
        {
            InspectorHotel ih = HotelDAL.GetInspectorHotelById(inspectorHotelID);
            int requiredPoints = ih.RequiredPoint;
            if (requiredPoints > 0)
            {
                ConsumeUserPoints(userID, requiredPoints, HJD.HotelServices.Contracts.HotelServiceEnums.ConsumeUserPointsBizType.FreeInspectHotel, inspectorRefHotelID);//消费积分
            }
            return 0;
            //long IDX = HotelDAL.CheckInspectorRefHotel(inspectorHotelID, 1);//先占名额 抢占后返回一个
            //if (IDX > 0)
            //{
            //InspectorHotel ih = HotelDAL.GetInspectorHotelById(inspectorHotelID);
            //int requiredPoints = ih.RequiredPoint;
            //if (requiredPoints > 0)
            //{
            //    ConsumeUserPoints(userID, requiredPoints, 2, inspectorRefHotelID);//消费积分
            //}
            //return 0;
            //}
            //return 1;
        }

        public List<VRateEntity> GetVRateByHVID(int HVID)
        {
            return HotelDAL.GetVRateByHVID(HVID);
        }

        /// <summary>
        /// 扣除积分
        /// </summary>
        /// <param name="inspectorRefHotelID">申请记录id</param>
        /// <param name="hvid">房券id</param>
        /// <param name="userID"></param>
        /// <returns></returns>
        private int ConsumeHotelVoucherPoints(int inspectorRefHotelID, int hvid, long userID, DateTime CheckInDate, DateTime CheckOutDate)
        {
            int requiredPoints = CalHotelVoucherPoints(hvid, CheckInDate, CheckOutDate);

            if (requiredPoints > 0)
            {
                ConsumeUserPoints(userID, requiredPoints, HJD.HotelServices.Contracts.HotelServiceEnums.ConsumeUserPointsBizType.FreeInspectHotel, inspectorRefHotelID);//消费积分
            }

            return 0;
        }
        /// <summary>
        /// 计算需要积分
        /// </summary>
        /// <param name="hvid"></param>
        /// <param name="CheckInDate"></param>
        /// <param name="CheckOutDate"></param>
        /// <returns></returns>
        private static int CalHotelVoucherPoints(int hvid, DateTime CheckInDate, DateTime CheckOutDate)
        {
            //HotelVoucherEntity ve = HotelDAL.GetHotelVoucherByID(hvid).FirstOrDefault();
            //判断是否在指定日内
            int requiredPoints = 0;
            List<VRateEntity> vr = HotelDAL.GetVRateByHVID(hvid);

            if (vr != null && vr.Count > 0)
            {


                for (DateTime curDate = CheckInDate.Date; curDate < CheckOutDate; curDate = curDate.AddDays(1))
                {
                    var tempList = vr.Where(v => v.Type == 8 && v.Date.Date == curDate);
                    VRateEntity r = new VRateEntity();
                    if (tempList.Count() > 0)
                    {
                        r = tempList.First();
                    }
                    else
                    {
                        int dayOfWeek = (int)curDate.DayOfWeek;
                        if (dayOfWeek == 0)
                        {
                            dayOfWeek = 7;
                        }

                        var weekList = vr.Where(v => v.Type == dayOfWeek);
                        if (weekList.Count() > 0)
                        {
                            r = weekList.First();
                        }
                        else
                        {
                            var normalList = vr.Where(v => v.Type == 0);
                            if (normalList.Count() > 0)
                            {
                                r = normalList.First();
                            }
                        }
                    }
                    if (r.ID > 0)
                    {
                        requiredPoints += r.Integral;
                    }
                }
            }
            return requiredPoints;
        }


        /// <summary>
        /// 审核品鉴酒店 数据
        /// 更新状态为通过3,不通过2
        /// </summary>
        /// <param name="eh">如果审核通过 只更新状态；如果审核不通过 则退名额 退积分</param>
        /// <returns></returns>
        public int CheckHotelVoucherApply(InspectorRefHotel eh)
        {
            if (eh.State == 2)
            {
                HotelDAL.InsertOrUpdateInspectorRefHotel(eh);//更新申请品鉴酒店状态 审核通过或者不通过

                VoucherEntity v = new VoucherEntity();
                v.ID = eh.VID;
                v.HVID = eh.HVID;
                v.State = 0;
                int i = HotelDAL.UpdateVoucher(v);

                int inspectorHotelID = eh.InspectorHotel;
                long IDX = HotelDAL.CheckInspectorRefHotel(inspectorHotelID, 0);//退掉占着的名额
                if (IDX > 0)
                {
                    RefundUserPoints(eh.Inspector, HJD.HotelServices.Contracts.HotelServiceEnums.ConsumeUserPointsBizType.FreeInspectHotel, eh.ID);//退积分
                    return 0;
                }
                return 1;
            }
            else if (eh.State == 3)
            {
                VoucherEntity v = new VoucherEntity();
                v.ID = eh.VID;
                v.HVID = eh.HVID;
                v.State = 0;
                int i = HotelDAL.UpdateVoucher(v);
                //如果还没有扣除积分 此时需要把积分扣掉再做 找businiessID为此品鉴酒店申请记录的积分消费记录
                PointsConsumeEntity pce = HotelDAL.GetPointsConsumeByIDOrTypeID(0, HJD.HotelServices.Contracts.HotelServiceEnums.ConsumeUserPointsBizType.FreeInspectHotel, eh.ID, 1);
                PointsConsumeEntity pce2 = HotelDAL.GetPointsConsumeByIDOrTypeID(0, HJD.HotelServices.Contracts.HotelServiceEnums.ConsumeUserPointsBizType.RefundFreeInspectHotel, eh.ID, 2);
                if (pce.ID > 0 && pce2.ID > 0)
                {
                    int totalPoints = GetAvailablePointByUserID(eh.Inspector, 0);
                    //说明是审核不通过之后 再点击通过 此时要验证酒店是否还有
                    long IDX = HotelDAL.CheckInspectorRefHotel(eh.InspectorHotel, 1);//占用名额
                    if (IDX > 0)
                    {
                        HotelDAL.InsertOrUpdateInspectorRefHotel(eh);//不通过之后再设置通过的数据
                        ConsumeInspectorHotelPoints(eh.ID, eh.InspectorHotel, eh.Inspector);//再次消费积分
                        return 0;
                    }
                }
                else
                {
                    HotelDAL.InsertOrUpdateInspectorRefHotel(eh);//这是直接审核通过的数据
                    return 0;
                }
                return 3;
            }
            else
            {
                return 2;
            }
        }

        /// <summary>
        /// 审核品鉴酒店 数据
        /// 更新状态为通过3,不通过2
        /// </summary>
        /// <param name="eh">如果审核通过 只更新状态；如果审核不通过 则退名额 退积分</param>
        /// <returns></returns>
        public int CheckInspectorHotelApply(InspectorRefHotel eh)
        {
            if (eh.State == 2)
            {
                HotelDAL.InsertOrUpdateInspectorRefHotel(eh);//更新申请品鉴酒店状态 审核通过或者不通过

                int inspectorHotelID = eh.InspectorHotel;
                //eh.HVID>0表示房券id
                if (eh.HVID > 0)
                {
                    RefundUserPoints(eh.Inspector, HJD.HotelServices.Contracts.HotelServiceEnums.ConsumeUserPointsBizType.FreeInspectHotel, eh.ID);//退积分
                    return 0;
                }
                else
                {
                    long IDX = HotelDAL.CheckInspectorRefHotel(inspectorHotelID, 0);//退掉占着的名额
                    if (IDX > 0)
                    {
                        RefundUserPoints(eh.Inspector, HJD.HotelServices.Contracts.HotelServiceEnums.ConsumeUserPointsBizType.FreeInspectHotel, eh.ID);//退积分
                        return 0;
                    }
                }
                return 1;
            }
            else if (eh.State == 3)
            {
                //如果还没有扣除积分 此时需要把积分扣掉再做 找businiessID为此品鉴酒店申请记录的积分消费记录
                PointsConsumeEntity pce = HotelDAL.GetPointsConsumeByIDOrTypeID(0, HJD.HotelServices.Contracts.HotelServiceEnums.ConsumeUserPointsBizType.FreeInspectHotel, eh.ID, 1);
                PointsConsumeEntity pce2 = HotelDAL.GetPointsConsumeByIDOrTypeID(0, HJD.HotelServices.Contracts.HotelServiceEnums.ConsumeUserPointsBizType.RefundFreeInspectHotel, eh.ID, 2);
                if (pce.ID > 0 && pce2.ID > 0)
                {
                    int totalPoints = GetAvailablePointByUserID(eh.Inspector, 0);
                    //说明是审核不通过之后 再点击通过 此时要验证酒店是否还有
                    if (eh.HVID > 0)//走房券
                    {
                        long evaHotelID20 = HotelDAL.CheckVoucher(eh.HVID, 1, eh.Inspector, eh.InspectorPhone);
                        //HotelDAL.InsertOrUpdateInspectorRefHotel(eh);//不通过之后再设置通过的数据
                        ConsumeInspectorHotelPoints(eh.ID, eh.InspectorHotel, eh.Inspector);//再次消费积分
                        return 0;
                    }
                    else
                    {
                        long IDX = HotelDAL.CheckInspectorRefHotel(eh.InspectorHotel, 1);//占用名额
                        if (IDX > 0)
                        {
                            HotelDAL.InsertOrUpdateInspectorRefHotel(eh);//不通过之后再设置通过的数据
                            ConsumeInspectorHotelPoints(eh.ID, eh.InspectorHotel, eh.Inspector);//再次消费积分
                            return 0;
                        }
                    }
                }
                else
                {
                    HotelDAL.InsertOrUpdateInspectorRefHotel(eh);//这是直接审核通过的数据
                    return 0;
                }
                return 3;
            }
            else
            {
                return 2;
            }
        }

        /// <summary>
        /// typeID 等于 2 代表的是免费品鉴需要消耗的积分
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="requiredPoints"></param>
        /// <param name="typeID"></param>
        /// <param name="businessID">不能选品鉴酒店 您可设置为</param>
        /// <returns>0:成功 其它:不成功 </returns>
        public int ConsumeUserPoints(long userID, int requiredPoints, HJD.HotelServices.Contracts.HotelServiceEnums.ConsumeUserPointsBizType typeID, long businessID)
        {
            List<PointsEntity> canUseEntityList = HotelDAL.GetPointsEntity(userID).FindAll(i => i.LeavePoint > 0);
            List<PointsEntity> allPointsEntity = canUseEntityList.OrderBy(i => i.ExpiredTime).ToList();//按过期时间升序排列
            List<PointsEntity> finalEntity = new List<PointsEntity>();
            int sumDeductPoints = 0;
            foreach (PointsEntity temp in allPointsEntity)
            {
                if (requiredPoints <= 0)
                {
                    break;
                }
                else if (requiredPoints >= temp.LeavePoint)
                {
                    sumDeductPoints += temp.LeavePoint;
                    requiredPoints -= temp.LeavePoint;
                    temp.LeavePoint = 0;
                }
                else
                {
                    sumDeductPoints += requiredPoints;
                    temp.LeavePoint -= requiredPoints;
                    requiredPoints = 0;
                }
                finalEntity.Add(temp);
            }

            if (finalEntity.Count != 0)
            {
                foreach (PointsEntity temp in finalEntity)
                {
                    HotelDAL.InsertOrUpdatePoints(temp);
                }
            }

            PointsConsumeEntity pce = new PointsConsumeEntity();
            pce.State = 1;//默认是1
            pce.ConsumePoint = sumDeductPoints;//实际的抵扣积分数量
            pce.TypeID = (int)typeID;
            pce.BusinessID = businessID;
            pce.UserID = userID;

            return HotelDAL.InsertOrUpdatePointsConsume(pce);
        }

        /// <summary>
        /// 退用户积分
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="typeID"></param>
        /// <param name="businessID"></param>
        /// <returns>0：成功  其它：失败</returns>
        public int RefundUserPoints(long userID, HJD.HotelServices.Contracts.HotelServiceEnums.ConsumeUserPointsBizType typeID, int businessID)
        {
            List<PointsEntity> allPointsEntity = HotelDAL.GetPointsEntity(userID).FindAll(_ => _.TypeID != 7).OrderBy(i => i.ExpiredTime).ToList();//发生退积分的情况 默认不退到会过期的积分记录上
            PointsConsumeEntity pce = HotelDAL.GetPointsConsumeByIDOrTypeID(0, typeID, businessID, 1);
            int returnPoints = 0;
            if (pce != null && pce.ID != 0)
            {
                returnPoints = pce.ConsumePoint;
            }

            List<PointsEntity> finalEntity = new List<PointsEntity>();
            if (allPointsEntity != null && allPointsEntity.Count != 0 && returnPoints != 0)
            {
                foreach (PointsEntity temp in allPointsEntity)
                {
                    int gapPoints = temp.TotalPoint - temp.LeavePoint;
                    if (gapPoints == 0)
                    {
                        continue;
                    }
                    if (returnPoints <= 0)
                    {
                        break;
                    }
                    else if (returnPoints >= gapPoints)
                    {
                        temp.LeavePoint = temp.TotalPoint;
                        returnPoints -= gapPoints;
                    }
                    else
                    {
                        temp.LeavePoint += returnPoints;
                        returnPoints = 0;
                    }
                    finalEntity.Add(temp);
                }

                if (finalEntity.Count != 0)
                {
                    foreach (PointsEntity temp in finalEntity)
                    {
                        HotelDAL.InsertOrUpdatePoints(temp);
                    }
                }
            }

            PointsConsumeEntity pce2 = pce;
            pce2.ID = 0;
            pce2.State = 2;
            //pce.ConsumePoint = 0;
            pce2.TypeID =  (int)typeID ;
            pce2.BusinessID = businessID;
            pce2.UserID = userID;

            return HotelDAL.InsertOrUpdatePointsConsume(pce2);
        }

        public int InsertOrUpdateInspectorHotel(InspectorHotel eh)
        {
            return HotelDAL.InsertOrUpdateInspectorHotel(eh);
        }

        public int InsertOrUpdateInspectorRefHotel(InspectorRefHotel eh)
        {
            return HotelDAL.InsertOrUpdateInspectorRefHotel(eh);
        }

        public int UpdateInspectorRefHotel(int state, int vid, int hvid)
        {
            return HotelDAL.UpdateInspectorRefHotel(state, vid, hvid);
        }

        public int UpdateInspectorRefHotel4Comment(int ehID, int commentID)
        {
            return HotelDAL.UpdateInspectorRefHotel4Comment(ehID, commentID);
        }

        public int UpdateInspectorRefHotel4NoticeComment(int ehID, bool hasSendWriteComment)
        {
            return HotelDAL.UpdateInspectorRefHotel4Comment(ehID, 0, hasSendWriteComment);
        }

        public List<InspectorRefHotel> GetInspectorRefHotelList(long evaHotelID, long userID, int hvid = 0)
        {
            return HotelDAL.GetInspectorRefHotelList(evaHotelID, userID, hvid);
        }

        public List<HotelVoucherEntity> GetUseHotelVoucher(out int count)
        {
            return HotelDAL.GetUseHotelVoucher(out count);
        }
        public List<VoucherEntity> GetUseVoucherList(long userid, int hvid)
        {
            return HotelDAL.GetUseVoucherList(userid, hvid);
        }

        public List<HotelVoucherEntity> GetHotelVoucherByID(int ID)
        {
            return HotelDAL.GetHotelVoucherByID(ID);
        }

        public List<InspectorRefHotel> GetInspectorRefHotelListByHVID(int hvid, long userID)
        {
            return HotelDAL.GetInspectorRefHotelListByHVID(hvid, userID);
        }

        public List<InspectorHotel> GetInspectorHotelList(InspectorHotelSearchParam param, out int count)
        {
            return HotelDAL.GetInspectorHotelList(param, out count);
        }

        public List<InspectorRefHotel> GetInspectorHotelOrderList(int OrderState, int lastID, int pageSize)
        {
            return HotelDAL.GetInspectorHotelOrderList(OrderState, lastID, pageSize);
        }

        public InspectorRefHotel GetInspectorRefHotelById(long id)
        {
            InspectorRefHotel irh = HotelDAL.GetInspectorRefHotelById(id);
            return irh == null ? new InspectorRefHotel() : irh;
        }

        public bool UpdateInspectorHotelOrder(InspectorRefHotel ihr)
        {
            return HotelDAL.UpdateInspectorHotelOrder(ihr);
        }

        public InspectorHotel GetInspectorHotelById(long id)
        {
            InspectorHotel ih = HotelDAL.GetInspectorHotelById(id);
            return ih == null ? new InspectorHotel() : ih;
        }

        #region 商区操作
        public List<ClassZoneEntity> GetClassZone(int district, List<int> classID)
        {
            return memCacheHotel.GetData<List<ClassZoneEntity>>(CombineKey(district, classID), ClassZoneCacheKey
                 , () => { return GetClassZoneAccess(district, classID); });
        }

        public void SetClassZone(int district, List<int> classID)
        {
            List<ClassZoneEntity> list = GetClassZoneAccess(district, classID);
            memCacheHotel.Set(CombineKey(district, classID), ClassZoneCacheKey, list);
        }

        private List<ClassZoneEntity> GetClassZoneAccess(int district, List<int> classID)
        {
            classID = classID ?? new List<int>();
            List<ClassZoneEntity> list = HotelDAL.GetClassZone(district, string.Join<int>(",", classID));
            list.AddRange(HotelDAL.GetClassLocation(district, string.Join<int>(",", classID)));
            return list;
        }
        #endregion

        #region 品牌操作
        public List<ClassBrandEntity> GetClassBrand(int district, List<int> classID)
        {
            return memCacheHotel.GetData<List<ClassBrandEntity>>(CombineKey(district, classID), ClassBrandCacheKey, () =>
            {
                return GetClassBrandAccess(district, classID);
            });
        }

        public void SetClassBrand(int district, List<int> classID)
        {
            List<ClassBrandEntity> list = GetClassBrandAccess(district, classID);
            memCacheHotel.Set(CombineKey(district, classID), ClassBrandCacheKey, list);
        }

        private List<ClassBrandEntity> GetClassBrandAccess(int district, List<int> classID)
        {
            classID = classID ?? new List<int>();
            return HotelDAL.GetClassBrand(district, string.Join<int>(",", classID));
        }
        #endregion

        #region 酒店分类 各排名及数量

        public List<FilterDicEntity> GetHotelClassByDistrict(int district)
        {
            return memCacheHotel.GetData<List<FilterDicEntity>>(district.ToString() + "_", DistrictHotelClassCacheKey, () =>
            {
                var result = QueryHotel(new HotelSearchParas { DistrictID = district, CheckInDate = DateTime.Now, CheckOutDate = DateTime.Now.AddDays(1) });
                var menu = ParseHotelMenu(result.FilterCount, false, district);
                menu.Classes.Insert(0, new FilterDicEntity { ID = 0, Type = 6, Name = "全部酒店", Num = result.TotalCount });
                return menu.Classes;
            });
        }

        public void SetDistrictHotelClass(int district)
        {
            List<HotelRankEntity> list = GetHotelClassByDistrictAccess(district);
            memCacheHotel.Set(district.ToString() + "_", DistrictHotelClassCacheKey, list);
        }

        private List<HotelRankEntity> GetHotelClassByDistrictAccess(int district)
        {
            var query = HotelDAL.GetDistrictHotelClassByDistrict(district);

            return query;
        }
        #endregion

        #region 目的地机场、火车站
        public List<DistrictAirportTrainstationEntity> GetAirportTrain(int district)
        {
            string key = district.ToString();
            List<DistrictAirportTrainstationEntity> list = memCacheNewestHotelReview.GetData<List<DistrictAirportTrainstationEntity>>(key, DistrictAirportTrainCacheKey, () =>
            {
                return GetAirportTrainAccess(district);
            });
            return list;
        }

        public void SetAirportTrain(int district)
        {
            List<DistrictAirportTrainstationEntity> list = GetAirportTrainAccess(district);
            memCacheHotel.Set(district.ToString(), DistrictAirportTrainCacheKey, list);
        }

        private List<DistrictAirportTrainstationEntity> GetAirportTrainAccess(int district)
        {
            var query = HotelDAL.GetDistrictAirportTrainstationList(district);
            if (query != null && query.Count > 0)
            {
                query = query.Where(q => q.Type == PlaceType.Airport.ToString() || q.Type == PlaceType.Trainstation.ToString()).ToList();
            }
            return query;

        }
        #endregion

        #region 酒店分类
        public List<HotelRankEntity> GetHotelAllClass()
        {
            string key = "classh";
            List<HotelRankEntity> list = memCacheNewestHotelReview.GetData<List<HotelRankEntity>>(key, HotelInfoClassExCacheKey, () =>
            {
                return GetHotelAllClassAccess();
            });
            return list;
        }

        public void SetHotelAllClass()
        {
            List<HotelRankEntity> list = GetHotelAllClassAccess();
            memCacheHotel.Set("classh", HotelInfoClassExCacheKey, list);
        }

        private List<HotelRankEntity> GetHotelAllClassAccess()
        {

            var query = HotelDAL.GetHotelClassList();

            return query;


        }
        #endregion

        #region 获取目的地地标
        public List<DistrictAirportTrainstationEntity> GetPlaceMark(int district, PlaceType placeType)
        {
            string key = district.ToString() + "_" + placeType.ToString();
            List<DistrictAirportTrainstationEntity> list = memCacheHotel.GetData<List<DistrictAirportTrainstationEntity>>(key, DistrictPlaceMarkCacheKey, () =>
            {
                return GetPlaceMarkAccess(district, placeType);
            });
            return list;
        }

        public void SetPlaceMark(int district, PlaceType placeType)
        {
            string key = district.ToString() + "_" + placeType.ToString();
            memCacheHotel.Set(key, DistrictPlaceMarkCacheKey, GetPlaceMarkAccess(district, placeType));
        }

        private List<DistrictAirportTrainstationEntity> GetPlaceMarkAccess(int district, PlaceType placeType)
        {
            var query = HotelDAL.GetDistrictAirportTrainstationList(district) ?? new List<DistrictAirportTrainstationEntity>();
            switch (placeType)
            {
                case PlaceType.All:
                    break;
                default:
                    query = query.Where(p => p.Type == placeType.ToString()).ToList();
                    break;
            }
            return query;
        }
        #endregion

        #region 附近酒店操作
        /// <summary>
        /// 附近酒店通过酒店
        /// </summary>
        /// <param name="hotelID"></param>
        /// <returns></returns>
        public List<HotelDistanceEntity> GetNearbyHotel(int hotelID)
        {
            return memCacheHotel.GetData<List<HotelDistanceEntity>>(hotelID.ToString(), NearHotelInfoCacheKey, () =>
            {
                return GetNearbyHotelAccess(hotelID);
            });
        }

        /// <summary>
        /// 附近酒店通过酒店
        /// </summary>
        /// <param name="hotelID"></param>
        /// <returns></returns>
        public List<HotelDistanceEntity> GetNearbyHotelEx(int hotelID)
        {
            return memCacheHotel.GetData<List<HotelDistanceEntity>>(hotelID.ToString(), NearHotelInfoCacheKey + "Ex", () =>
            {
                var x = HotelDAL.GetNearHotelByHotel(hotelID);
                if (x != null && x.Count > 0)
                {

                    var query = (from q in x
                                 select new HotelDistanceEntity()
                                 {
                                     HotelId = q.HotelId,
                                     Distance = Math.Round(decimal.Parse(q.Distance.ToString()) / 1000, 2)
                                 }).ToList();
                    return query;
                }
                return new List<HotelDistanceEntity>();
            });
        }

        private List<int> GetNearHotelListbySightAccess(int Sight, string sortName)
        {
            return HotelDAL.GetNearAllHotelListBySight(Sight, sortName.ToLower());

        }

        /// <summary>
        /// 根据餐馆查找附近酒店
        /// </summary>
        public List<HotelDistanceEntity> GetNearbyHotelByRestaurant(int restaurant, int district, double lat, double lon)
        {
            return memCacheHotel.GetData<List<HotelDistanceEntity>>(restaurant.ToString(), NearbyHotelByRestaurantCacheKey, () =>
            {
                return GetNearbyHotelByRestaurantAccess(district, lat, lon);
            });
        }

        public void SetNearbyHotel(Dictionary<int, string> dic)
        {
            foreach (var key in dic.Keys)
            {
                if (!string.Equals(dic[key].Trim(), ""))
                {
                    var arrs = dic[key].TrimEnd(',').Split(',').ToList();
                    var query = (from q in arrs
                                 select new HotelDistanceEntity()
                                 {
                                     HotelId = int.Parse(q.Split('|')[0]),
                                     Distance = Math.Round(decimal.Parse(q.Split('|')[1]) / 1000, 1)
                                 }).ToList();

                    memCacheHotel.Set(key.ToString(), NearHotelInfoCacheKey, query);
                }
            }
        }
        public void SetNearbySight(Dictionary<int, string> dic)
        {
            foreach (var key in dic.Keys)
            {
                if (!string.Equals(dic[key].Trim(), ""))
                {
                    var arrs = dic[key].TrimEnd(',').Split(',').ToList();
                    var query = (from q in arrs
                                 select new HotelDistanceEntity()
                                 {
                                     HotelId = int.Parse(q.Split('|')[0]),
                                     Distance = Math.Round(decimal.Parse(q.Split('|')[1]) / 1000, 1)
                                 }).ToList();

                    memCacheHotel.Set(key.ToString(), NearHotelInfoBySightCacheKey, query);
                }
            }
        }

        public List<int> GetPackagedInterestPlace()
        {
            return HotelDAL.GetPackagedInterestPlace();
        }

        private List<HotelDistanceEntity> GetNearbyHotelAccess(int hotelID)
        {
            //string res = SearchHotelList.GetNearByHotel(hotelID);
            //GetNearHotelByHotel
            var hie = GetHotel(hotelID);
            int rank = 1;
            int district = 0;
            if (hie != null)
            {
                rank = hie.Rank;
                district = hie.DistrictId;

            }
            var x = HotelDAL.GetNearHotelByHotel(hotelID, district, rank);

            if (x != null && x.Count > 0)
            {

                var query = (from q in x
                             select new HotelDistanceEntity()
                             {
                                 HotelId = q.HotelId,
                                 Distance = Math.Round(decimal.Parse(q.Distance.ToString()) / 1000, 1)
                             }).ToList();
                return query;
            }
            return new List<HotelDistanceEntity>();
        }

        /// <summary>
        /// 根据餐馆查找附近酒店
        /// </summary>
        /// <param name="restaurant"></param>
        /// <returns></returns>
        private List<HotelDistanceEntity> GetNearbyHotelByRestaurantAccess(int district, double lat, double lon)
        {
            var x = HotelDAL.GetNearbyHotelByRestaurant(district, lat, lon);
            if (x != null && x.Count > 0)
            {
                var query = (from q in x
                             select new HotelDistanceEntity()
                             {
                                 HotelId = q.HotelId,
                                 Distance = Math.Round(q.Distance / 1000, 1)
                             }).ToList();
                return query;
            }

            return new List<HotelDistanceEntity>();
        }
        #endregion

        /// <summary>
        /// 获取酒店列表中点评信息
        /// </summary>
        /// <param name="hotelID"></param>
        /// <returns></returns>
        public List<HotelReviewSummaryEntity> GetHotelReviewSummary(List<int> hotelID)
        {
            if (hotelID == null || hotelID.Count <= 0)
                return new List<HotelReviewSummaryEntity>();

            List<HotelReviewSummaryEntity> ls =
                memCacheHotelReview.GetMultiDataEx<int, HotelReviewSummaryEntity>(hotelID, CommentSummaryCacheKey
                , noDataList =>
                {
                    return GetHotelReviewSummaryAccess(noDataList);
                }
                , ReturnValue => ReturnValue.HotelID
                , new HotelReviewSummaryEntity { HotelID = -1 });

            return (from i in ls
                    where i != null && i.HotelID > 0
                    select i).ToList();
        }

        public List<HotelTFTRelItemEntity> GetHotelTFTRel(int hotelID)
        {
            if (hotelID == null || hotelID <= 0)
                return new List<HotelTFTRelItemEntity>();


            return memCacheHotelReview.GetData<List<HotelTFTRelItemEntity>>(hotelID.ToString(), HotelTFTRelCacheKey, () =>
            {
                return HotelDAL.GetHotelTFTRel(hotelID);
            });
        }


        //public List<HotelTFTReviewEntity> GetHotelTFTReview(int hotelID)
        //{
        //    if (hotelID == null || hotelID <= 0)
        //        return new List<HotelTFTReviewEntity>();


        //    return memCacheHotelReview.GetData<List<HotelTFTReviewEntity>>(hotelID.ToString(),HotelTFTReviewCacheKey, ()=>
        //        {
        //            return GenHotelTFTReview(hotelID);
        //        });
        //}

        public List<HotelInterestTagEntity> GetHotelInterestTag(int hotelID, int interestID)
        {
            if (hotelID == null || hotelID <= 0)
                return new List<HotelInterestTagEntity>();


            return memCacheHotelReview.GetData<List<HotelInterestTagEntity>>(hotelID.ToString(), string.Format("{0}:{1}:", HotelTFTRelCacheKey, interestID), () =>
            {
                return HotelDAL.GetHotelInterestTag(hotelID, interestID);
            });
        }

        public List<PackageEntity> GetPackageListByHotelId(int hotelid)
        {
            List<PackageEntity> packageList = HotelDAL.GetPackageListByHotelId(hotelid);
            List<PackageEntity> list = new List<PackageEntity>();
            foreach (PackageEntity temp in packageList)
            {
                if (!list.Contains(temp))
                {
                    list.Add(temp);
                }
            }

            return list;
        }


        /// <summary>
        /// 获取酒店所有的标签数据
        /// </summary>
        /// <param name="hotelID"></param>
        /// <returns></returns>
        public List<HotelTagInfoEntity> GetHotelShowTags(int hotelID)
        {
            if (hotelID == null || hotelID <= 0)
                return new List<HotelTagInfoEntity>();


            return memCacheHotelReview.GetData<List<HotelTagInfoEntity>>(hotelID.ToString(), HotelShowTagsCacheKey, () =>
            {
                return HotelDAL.GetHotelShowTags(hotelID);
            });
        }

        public List<HotelTagInfoEntity> GetHotelInterestTags(int hotelID)
        {
            if (hotelID == null || hotelID <= 0)
                return new List<HotelTagInfoEntity>();


            return memCacheHotelReview.GetData<List<HotelTagInfoEntity>>(hotelID.ToString(), HotelInterestTagsCacheKey, () =>
            {
                return HotelDAL.GetHotelInterestTags(hotelID);
            });
        }

        public List<Int32> GetHotelInterestIDs(int hotelID)
        {
            return HotelDAL.GetHotelInterestIDs(hotelID);
        }


        //public List<HotelTFTReviewEntity> GenHotelTFTReview(int hotelID)
        //{
        //    List<HotelTFTReviewItemEntity> htftr = HotelDAL.GetHotelTFTReview(hotelID);

        //    List<HotelReviewExEntity> rl = GetHotelReviewEx(htftr.Select(r => r.ReviewID).ToList());


        //    List<HotelTFTReviewEntity> hl = (from hi in htftr.Distinct(new HotelTFTReviewCompare())
        //                                     select new HotelTFTReviewEntity { HotelID = hi.HotelID, TFTID = hi.TFTID, TFTName = hi.TFTName, Type = hi.Type, ReviewList = new List<HotelReviewExEntity>() }).ToList();
        //    foreach (HotelReviewExEntity r in rl)
        //    {
        //        HotelTFTReviewItemEntity ht = htftr.Where(h => h.ReviewID == r.Writing).First();
        //        hl.Where(h => h.HotelID == ht.HotelID && h.TFTID == ht.TFTID && h.Type == ht.Type).First().ReviewList.Add(r);
        //    }

        //    return hl;

        //}

        public List<HotelReviewSummaryEntity> GetHotelReviewSummaryAccess(List<int> hotelID)
        {
            List<HotelReviewTop3Entity> ht = HotelDAL.GetTop3CommentByHotel(hotelID);

            List<HotelReviewSummaryEntity> hs = HotelDAL.GetHotelReviewNumAndRating(hotelID);

            List<RatingEntity> gr = HotelDAL.HotelReviewGroupRating(hotelID);

            foreach (HotelReviewSummaryEntity i in hs)
            {
                i.HotelReviewTop3 = ht.Where(j => j.HotelID == i.HotelID).ToList();
                i.GroupRating = gr.Where(j => j.HotelID == i.HotelID).ToList();
            }

            return hs;
        }

        /// <summary>
        /// 清除缓存 酒店点评Summary
        /// </summary>
        /// <param name="hotelID"></param>
        private void ClearCacheHotelReviewSummary(int hotelID)
        {
            try
            {
                memCacheHotelReview.Remove(hotelID.ToString(), CommentSummaryCacheKey);
            }
            catch
            {
            }
        }

        public List<HotelInfoChannelExEntity> GetHotelChannelList(List<int> hotelID)
        {
            if (hotelID == null || hotelID.Count <= 0)
                return new List<HotelInfoChannelExEntity>();

            List<HotelInfoChannelExEntity> ls =
                memCacheHotel.GetMultiDataEx<int, HotelInfoChannelExEntity>(hotelID, HotelChannelCacheKey
                , noDataList =>
                {
                    return GetHotelChannelListAccess(noDataList);

                }
                , ReturnValue => ReturnValue.HotelID
                , new HotelInfoChannelExEntity { HotelID = -1 });

            var x = (from i in ls
                     where i != null && i.HotelID > 0
                     select i).ToList();
            return x;
        }

        public List<HotelInfoChannelExEntity> GetHotelChannelListAccess(List<int> hotelID)
        {
            List<HotelInfoChannelExEntity> Hice = new List<HotelInfoChannelExEntity>();

            List<HotelInfoChannelEntity> Hin = HotelDAL.GetHotelChannelByHotel(hotelID);

            if (Hin != null && Hin.Count > 0)
            {
                var Hg = (from t in Hin group t by new { t.HotelID } into g select g.Key);
                var minHotelOriID = 0;
                foreach (var h in Hg)
                {
                    HotelInfoChannelExEntity He = new HotelInfoChannelExEntity();
                    He.HotelID = h.HotelID;


                    if (!Hice.Contains(He))
                    {
                        //携程会出现一对多的情况，暂取其最小的一个酒店ID
                        List<int> m = (from j in Hin where j.HotelID == h.HotelID && j.Channel == "CTRIP" select j.HotelOriID).ToList();
                        if (m != null && m.Count > 0)
                        {
                            minHotelOriID = m.Min();
                            He.ViewHotelImgUrl = "http://hotels.ctrip.com/domestic/showhotelroompic.aspx?hotel=" + minHotelOriID + "";
                        }




                        var x = (from i in Hin where i.HotelID == h.HotelID select i).ToList();
                        He.HotelOriIDList = x;
                        Hice.Add(He);
                    }
                }
            }
            return Hice;
        }

        public void SetHotelInfoChannelEx(List<int> hotelID)
        {
            foreach (var item in hotelID)
            {
                memCacheHotel.Set(item.ToString(), HotelChannelCacheKey, GetHotelChannelListAccess(new List<int> { item }).FirstOrDefault());
            }
        }

        public List<HotelInfoExEntity> GetHotelInfoEx(List<int> hotelID)
        {
            if (hotelID == null || hotelID.Count <= 0)
                return new List<HotelInfoExEntity>();

            //memcached取缓存
            List<HotelInfoExEntity> ls =
                memCacheHotel.GetMultiDataEx<int, HotelInfoExEntity>(hotelID, HotelInfoExCacheKey
                , noDataList =>
                {
                    List<HotelInfoExEntity> hn = GetHotelInfoExAccess(noDataList);
                    return hn;
                }
                , ReturnValue => ReturnValue.HotelID);
            //, new HotelInfoExEntity { HotelID = -1 });

            return (from i in ls
                    where i != null && i.HotelID > 0
                    select i).ToList();
        }

        public void RefreshCacheHotelInfoEx(List<int> hotelID)
        {
            //hotelID.ForEach(p => memCacheHotel.Remove(p.ToString(), HotelInfoExCacheKey));
            foreach (var item in hotelID)
            {
                List<HotelInfoExEntity> hh = GetHotelInfoExAccess(new List<int> { item });
                //if (hh == null || hh.Count <= 0)
                //    hh = new List<HotelInfoExEntity>();
                memCacheHotel.Set(item.ToString(), HotelInfoExCacheKey, hh.FirstOrDefault());
            }
        }

        public static List<SimpleHotelRankEntity> GetHotelRank(List<int> hotelIDs, int ResourceID, int HotelTypeID, RankType rt)
        {
            return HotelDAL.GetHotelRank(hotelIDs, ResourceID, HotelTypeID, rt);
        }

        public List<HotelRankEntity> GetHotelRankByHotel(List<int> hotelIDs)
        {
            return HotelDAL.GetHotelRankByHotel(hotelIDs);
        }

        public List<HotelClassEntity> GetHotelClassByHotel(List<int> hotelID)
        {
            return HotelDAL.GetHotelClassByHotel(hotelID);
        }

        private List<HotelInfoExEntity> GetHotelInfoExAccess(List<int> hotelID)
        {
            List<HotelFacilityEntity> hf = HotelDAL.GetFacilityList(hotelID);
            List<HotelRankEntity> singelHotelClass = GetHotelRankByHotel(hotelID);
            List<HotelInfoExEntity> hie = HotelDAL.GetHotelInfoEX(hotelID);

            //if (hie.Count == 0)
            //    return null;

            foreach (HotelInfoExEntity i in hie)
            {
                i.FacilityList = (from j in hf where j.HotelID == i.HotelID select j.Facility).ToList();
                //i.Rank = SearchHotelList.GetHotelRanking(i.HotelID);
                i.HotelClass = (from j in singelHotelClass where j.HotelID == i.HotelID select j).ToList();
                var d = (from j in i.HotelClass where j.ClassID == 0 select j).FirstOrDefault();
                i.Rank = (d == null ? 0 : (int)d.Rank);
            }

            return hie;
        }

        public List<HotelFacilityEntity> GetAllFacilityList()
        {
            string key = "allfaci";
            List<HotelFacilityEntity> list = memCacheNewestHotelReview.GetData<List<HotelFacilityEntity>>(key, HotelFaciCacheKey, () =>
            {
                return HotelDAL.GetAllFacilityList();
            });
            return list;
        }

        public List<HotelReviewExEntity> GetHotelReviewEx(List<WritingIDGroup> writing)
        {
            if (writing == null || writing.Count <= 0)
                return new List<HotelReviewExEntity>();

            List<HotelReviewExEntity> ls =
                memCacheHotelReview.GetMultiDataEx<WritingIDGroup, HotelReviewExEntity>(writing, CommentExCacheKey
                , noDataList =>
                {
                    return HotelDAL.GetHotelReviewByWriting(noDataList);
                }
                , ReturnValue => new WritingIDGroup { writing = ReturnValue.Writing, type = ReturnValue.WritingTypeID });
            //, new HotelReviewExEntity { Writing = -1 });

            return ls;
        }
        public List<HotelReviewExEntity> GetHotelReviewByCommentId(int commentId)
        {
            return HotelDAL.GetHotelReviewByCommentId(commentId);
        }
        //public List<HotelReviewExEntity> GetFeatruedReview(List<int> writing, int featured)
        //{
        //    if (writing == null || writing.Count <= 0)
        //        return new List<HotelReviewExEntity>();

        //    List<HotelReviewExEntity> ls =
        //        memCacheHotelReview.GetMultiDataEx<int, HotelReviewExEntity>(writing, string.Format("{0}_{1}",CommentExCacheKey,featured)
        //        , noDataList =>
        //        {
        //            return GenFeatruedReview(noDataList, featured); 
        //        }
        //        , ReturnValue => ReturnValue.Writing);
        //    //, new HotelReviewExEntity { Writing = -1 });

        //    return ls;
        //}

        public List<HotelReviewExEntity> GenFeatruedReview(List<WritingIDGroup> writing, int featured)
        {
            List<HotelReviewExEntity> rl = HotelDAL.GetHotelReviewByWriting(writing);// GetHotelReviewEx(writing);
            //List<FeaturedReviewEntity> frl = HotelDAL.GetFeatruedReviewList(writing, featured);

            //foreach (HotelReviewExEntity r in rl)
            //{
            //    FeaturedReviewEntity fr = frl.Where(f => f.Writing == r.Writing).FirstOrDefault();
            //    if (fr != null)
            //    {
            //        string fw = hotelTagSvc.GetFeaturedWords(r.Content, fr.TID);
            //        if(fw.Length > 0)
            //            r.Content = r.Content.Replace(fw, ConfigurationManager.AppSettings["PreFeaturedWords"] + fw + ConfigurationManager.AppSettings["SufFeaturedWords"]);
            //    }
            //}

            return rl;
        }

        public void ClearCacheHotelReviewEx(int writing)
        {
            try
            {
                memCacheHotelReview.Remove(writing.ToString(), CommentExCacheKey);
            }
            catch { }
        }

        //public void CheckAllHotelReview()
        //{
        //    int i = int.MaxValue;
        //    List<int> Writinglist;

        //    while (true)
        //    {
        //        Writinglist = HotelDAL.GetHotelReviewIDListForCheckData(i);

        //        if (Writinglist.Count <= 0)
        //            break;


        //        i = Writinglist.Last();

        //        if (i > 5671004) continue;

        //        File.AppendAllText("D:\\HJDApp\\log\\hotelreviewlog.txt", DateTime.Now.ToShortTimeString() + " " + Writinglist.First().ToString() + ",");

        //        CheckHotelReviewByWriting(Writinglist);
        //    }
        //}


        //public void HotelReviewByTimeStamp(string TaskName, string constLastTimeStamp)
        //{
        //    //获取上次同步时间戳
        //    long lastTimeStamp = 0;

        //    var dd = JobAssistant.GetParameter(TaskName);
        //    dd.ForEach(p =>
        //    {
        //        if (p.ConfigKey == constLastTimeStamp)
        //        {
        //            lastTimeStamp = long.Parse(p.ConfigValue);
        //        }
        //    });

        //    if (constLastTimeStamp == "HotelReviewLastTimeStamp") //发帖主贴mongo同步
        //    {
        //        List<HotelReviewIDAndTimeStamp> HotelReviewlist;
        //        do
        //        {
        //            HotelReviewlist = HotelDAL.GetHotelReviewIDListByTimeStamp(lastTimeStamp);
        //            if (HotelReviewlist.Count > 0)
        //            {
        //                lastTimeStamp = (from i in HotelReviewlist select i.TimeStamp).Max();
        //                CheckHotelReviewByWriting((from i in HotelReviewlist select i.HotelReviewID).ToList());
        //                JobAssistant.SetParameter(TaskName, constLastTimeStamp, lastTimeStamp.ToString());
        //            }

        //            //心跳
        //            if ((DateTime.Now - HeartbeatTime).TotalSeconds > 30)
        //            {
        //                HeartbeatTime = DateTime.Now;
        //                JobAssistant.Heartbeat(TaskName, System.Environment.MachineName);
        //            }
        //        } while (HotelReviewlist.Count > 0);
        //    }
        //}

        //#region 酒店=〉MongoDB
        //public void CheckHotelToMongodbByDistrict(List<int> DistrictList)
        //{
        //    List<HotelInfoExEntity> hi = HotelDAL.GetHotelInfoByDistrict(DistrictList);
        //    if (hi != null)
        //    {
        //        foreach (int di in DistrictList)
        //            CheckHotelToMongodbByHotel(hi.Where(i => i.DistrictID == di).ToList());
        //    }
        //}

        ///// <summary>
        ///// 初始酒店到MongoDB
        ///// </summary>
        ///// <param name="HotelList"></param>
        //public void CheckHotelToMongodbByHotel(List<int> HotelList)
        //{
        //    CheckHotelToMongodbByHotel(GetHotelInfoEx(HotelList));
        //}

        ////public void CheckHotelToMongodbByHotel(List<HotelInfoExEntity> hi)
        ////{
        ////    foreach (HotelInfoExEntity pifd in hi)
        ////    {
        ////        //如是聚合酒店应添加进去  名称放入，字母排名放入 必须每天晚上清空再初始化
        ////        try
        ////        {
        ////            HotelListInfoMongoDB pm = TransformHotelReviewToMongoDB(pifd);

        ////            HotelListInfoForMongoDB ps = MongodbDAL.GetHotelListInfoSignature(pifd.HotelID);

        ////            if (ps == null)
        ////            {
        ////                MongodbDAL.SetHotelToMongoDBByHotel(pm);

        ////            }
        ////        }
        ////        catch (Exception e)
        ////        {
        ////            File.AppendAllText("D:\\HJDApp\\log\\hotellog.txt", pifd.HotelID.ToString() + ":" + e.ToString());
        ////        }
        ////    }
        ////}

        //private HotelListInfoMongoDB TransformHotelReviewToMongoDB(HotelInfoExEntity pifd)
        //{
        //    HotelListInfoMongoDB ret = new HotelListInfoMongoDB();


        //    ret._id = pifd.HotelID;
        //    ret.FilterCol = new List<long>();
        //    List<DistrictAggregateRelEntity> das = HotelDAL.GenAggregateDistricts(pifd.DistrictID);
        //    foreach (DistrictAggregateRelEntity da in das)
        //    {
        //        ret.FilterCol.Add(HotelListFilterPrefix.DistrictID + da.DistrictID);
        //    }

        //    //List<HotelTagEntity> htl = HotelDAL.GetHotelTagList(pifd.HotelID);
        //    //foreach (HotelTagEntity ht in htl)
        //    //{
        //    //    ret.FilterCol.Add(HotelListFilterPrefix.Tags + ht.TagID);
        //    //}

        //    List<NearbyHotelsEntity> poil = HotelDAL.GetNearbyPOIByHotel(pifd.HotelID);
        //    foreach (NearbyHotelsEntity poi in poil)
        //    {
        //        ret.FilterCol.Add(HotelListFilterPrefix.NearBy + poi.POIID);
        //    }


        //    if (poil.Count > 0)
        //    {
        //        List<int> pgl = HotelDAL.GetPOIGroupList(poil.Select(p => p.POIID).ToList<int>());
        //        foreach (int pg in pgl)
        //        {
        //            ret.FilterCol.Add(HotelListFilterPrefix.NearbyGroup + pg);
        //        }
        //    }

        //    ret.FilterCol.Add(HotelListFilterPrefix.Zone + pifd.Zone);
        //    ret.FilterCol.Add(HotelListFilterPrefix.Location + pifd.Location);

        //    List<HotelRankEntity> hc = GetHotelRankByHotel(new List<int> { pifd.HotelID });

        //    foreach (int c in hc.Select(o => o.ClassID).Distinct())
        //    {
        //        ret.FilterCol.Add(HotelListFilterPrefix.Class + c);
        //    }

        //    //ret.Rank
        //    //ret.Star

        //    List<HotelDistanceForMongoDB> HotelDistances = new List<HotelDistanceForMongoDB>();
        //    List<NearbyHotelEntity> hdl = HotelDAL.GetHotelDistanceListByHotel(pifd.HotelID);
        //    foreach (NearbyHotelEntity h in hdl)
        //    {
        //        HotelDistances.Add(new HotelDistanceForMongoDB(h.POIID, h.Distance));
        //    }

        //    ret.HotelDistances = HotelDistances;

        //    List<HotelFacilityEntity> hf = HotelDAL.GetFacilityList(new List<int> { pifd.HotelID });
        //    foreach (HotelFacilityEntity h in hf)
        //    {
        //        ret.FilterCol.Add(HotelListFilterPrefix.Facility + h.Facility);
        //    }

        //    ret.FilterCol.Add(HotelListFilterPrefix.Brand + pifd.Brand);

        //    //ret.MinPrice = HotelDAL.GetHotelMinPrice(pifd.HotelID);

        //    ret.Gloc = new Coordinate(pifd.GLAT, pifd.GLON);
        //    ret.Bloc = new Coordinate(pifd.BLAT, pifd.BLON);



        //    ret.Signature = "";

        //    ret.Signature = Signature(ret);

        //    return ret;
        //}
        //#endregion


        /// <summary>
        /// 酒店与POI的距离
        /// </summary>
        /// <param name="hotelid"></param>
        /// <param name="poiid"></param>
        /// <returns></returns>
        public int GetHotelPOIDistanceByHotel(int hotelid, int poiid)
        {
            return HotelDAL.GetHotelPOIDistanceByHotel(hotelid, poiid);
        }

        public string Signature(object ret)
        {
            MemoryStream fs = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(fs, ret);

            //new
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();

            //获取密文字节数组
            byte[] bytResult = md5.ComputeHash(fs.GetBuffer());
            fs.Close();

            return BitConverter.ToString(bytResult).Replace("-", "");
        }

        //public void CheckHotelReviewByWriting(List<WritingIDGroup> Writinglist)
        //{
        //    List<HotelReviewExEntity> pifdList = HotelDAL.GetHotelReviewByWriting(Writinglist);
        //    foreach (HotelReviewExEntity pifd in pifdList)
        //    {
        //        try
        //        {
        //            HotelReviewForMongodb pm = TransformHotelReviewToMongoDB(pifd);

        //            HotelReviewSignatureForMongoDB ps = MongodbDAL.GetHotelReviewSignatureByWriting(pifd.Writing);

        //            if (ps == null || ps.Signature != pm.Signature)
        //            {
        //                MongodbDAL.SetHotelReviewByWriting(pm);
        //                ClearCacheHotelReviewSummary(pm.Hotel);
        //                ClearCacheHotelReviewEx(pm._id);
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            File.AppendAllText("D:\\HJDApp\\log\\hotelReviewlog.txt", pifd.Writing.ToString() + ":" + e.ToString());
        //        }
        //    }
        //}

        private HotelReviewForMongodb TransformHotelReviewToMongoDB(HotelReviewExEntity pifd)
        {
            HotelReviewForMongodb ret = new HotelReviewForMongodb();

            //是否是携程订单点评
            long hasOrder = pifd.OrderID > 0 ? 1 : 0;

            ret._id = pifd.Writing;

            if (string.Compare(pifd.Deleted, "T", true) == 0)
                ret.Deleted = 1;
            else
                ret.Deleted = 0;

            switch (pifd.Status)
            {
                case "W":
                case "w":
                    ret.Status = 0;
                    break;
                case "D":
                case "d":
                    ret.Status = 1;
                    break;
                case "P":
                case "p":
                    ret.Status = 2;
                    break;
                default:
                    ret.Status = 0;
                    break;
            }

            //隐发布最近一个月内，字数低于50个字的携程点评
            //if (hasOrder > 0 && ret.Status == 2)
            //{
            //    if ((DateTime.Today - pifd.WDate).TotalDays <= 30 && pifd.Content.Length < 50)
            //    {
            //        ret.Status = 0;
            //        ret.Deleted = 1;
            //    }
            //}

            ret.FilterCol = new List<long> { 0 };
            if (pifd.UserID > 0)
            {
                ret.UserID = pifd.UserID;
                //ret.FilterCol.Add(pifd.UserID);
            }

            //ret.RegionCol = new List<long>();
            //if (pifd.HotelID > 0)
            //    ret.RegionCol.Add(HotelReviewFilterPrefix.Hotel + pifd.HotelID);
            //if (pifd.DistrictID > 0)
            //    ret.RegionCol.Add(HotelReviewFilterPrefix.District + pifd.DistrictID);

            if (pifd.HotelID > 0)
                ret.Hotel = pifd.HotelID;

            switch (pifd.UserIdentity)
            {
                case 10:
                    ret.FilterCol.Add(HotelReviewFilterPrefix.UserIdentity + (int)UserIdentityType.Business);
                    break;
                case 20:
                    ret.FilterCol.Add(HotelReviewFilterPrefix.UserIdentity + (int)UserIdentityType.Child);
                    break;
                case 30:
                    ret.FilterCol.Add(HotelReviewFilterPrefix.UserIdentity + (int)UserIdentityType.Family);
                    break;
                case 40:
                    ret.FilterCol.Add(HotelReviewFilterPrefix.UserIdentity + (int)UserIdentityType.Friend);
                    break;
                case 50:
                    ret.FilterCol.Add(HotelReviewFilterPrefix.UserIdentity + (int)UserIdentityType.Single);
                    break;
                case 70:
                    ret.FilterCol.Add(HotelReviewFilterPrefix.UserIdentity + (int)UserIdentityType.Couple);
                    break;
                default:
                    break;
            }

            switch ((int)Math.Floor(pifd.Rating + 0.5001m))
            {
                case 5:
                    ret.FilterCol.Add(HotelReviewFilterPrefix.Rate + (int)RatingType.Best);
                    break;
                case 4:
                    ret.FilterCol.Add(HotelReviewFilterPrefix.Rate + (int)RatingType.Better);
                    break;
                case 3:
                    ret.FilterCol.Add(HotelReviewFilterPrefix.Rate + (int)RatingType.Good);
                    break;
                case 2:
                    ret.FilterCol.Add(HotelReviewFilterPrefix.Rate + (int)RatingType.Normal);
                    break;
                default:
                    ret.FilterCol.Add(HotelReviewFilterPrefix.Rate + (int)RatingType.Terrible);
                    break;
            }

            ret.OrderColCheckIn = (hasOrder << 32) + pifd.Writing;

            ret.OrderColRating = (int)(pifd.Rating * 10000000);

            //按发表时间排序使用点评id
            ret.OrderColTime = pifd.WDate.ToBinary();

            //最近一个月内，字数低于50个字的携程点评，日期排序在最后。
            //if (hasOrder > 0 && ret.Status == 2)
            //{
            //    if ((DateTime.Today - pifd.WDate).TotalDays <= 30 && pifd.Content.Length < 50)
            //    {
            //        //DateTime.Today中包含时区信息，pifd.WDate中不包括，所以要转换一下
            //        DateTime today = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
            //        ret.OrderColTime = pifd.WDate.ToBinary() - today.AddDays(1).ToBinary();
            //    }
            //}

            ret.Signature = "";

            MemoryStream fs = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(fs, ret);

            //new
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();

            //获取密文字节数组
            byte[] bytResult = md5.ComputeHash(fs.GetBuffer());
            fs.Close();

            ret.Signature = BitConverter.ToString(bytResult).Replace("-", "");

            return ret;
        }

        //public List<HotelReviewExEntity> GetHotelReviewList(ArguHotelReview argu)
        //{
        //    //mongodb取列表
        //    List<HotelReviewSignatureForMongoDB> phsidlist = MongodbDAL.GetHotelReviewList(argu);

        //    if (phsidlist != null && phsidlist.Count > 0)
        //    {
        //        List<int> keylist = (from i in phsidlist
        //                             select i._id).ToList<int>();

        //        return GetHotelReviewEx(keylist);
        //    }
        //    else
        //        return new List<HotelReviewExEntity>();
        //}
        /// <summary>
        /// 获取用户的酒店点评writing
        /// </summary>
        /// <param name="argu"></param>
        /// <returns></returns>
        //public List<HotelReviewExEntity> GetHotelReviewWritingListByUserid(long userid)
        //{
        //    List<int> writingList = HotelDAL.GetHotelReviewWritingListByUserid(userid);
        //    return GetHotelReviewEx(writingList);
        //}
        /// <summary>
        /// HotelReview Audit
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        //public bool SetHotelReviewStatus(HotelReviewAuditEntity c)
        //{
        //    if (HotelDAL.SetHotelReviewStatus(c))
        //    {

        //        if (c.statusDetail == 8 || c.Deleted == 'F')  //如果是主动删除，则同步到携程。 如果恢复出来已同步
        //        {
        //            HotelDAL.SetCtripHotelReviewStatus(c);
        //        }
        //        else if (c.Deleted == 'T') //部分删除类型需要同步到携程
        //        {   //4[内容重复]:点评的内容完全一致
        //            //5[内容简单]:内容符号过多
        //            //7[敏感词]
        //            //8[关键字]
        //            List<HotelReviewDelLogEntity> dl = HotelDAL.GetHotelReviewDelLog(c.Writing);
        //            if (dl.Count > 0)
        //            {
        //                HotelDAL.SetCtripHotelReviewStatus(c);
        //            }
        //        }
        //        CheckHotelReviewByWriting(new List<int> { c.Writing });
        //    }

        //    return true;
        //}

        #region 2个提交点评的方法


        public HotelReviewSaveResult HotelReviewSubmitToHotelDB(HotelReviewEntity hotelreviewinfo)
        {
            HotelReviewSaveResult hrr = HotelDAL.SaveHotelReviewInHoteldb(hotelreviewinfo);
            if (hrr.Writing > 0)
            {
                //List<int> _i = new List<int> { hrr.Writing };
                //CheckHotelReviewByWriting(_i);
                ClearCacheHotelReviewSummary(hotelreviewinfo.Hotel);
                ClearCacheHotelReviewEx(hrr.Writing);
                ClearCacheHotel(hotelreviewinfo.Hotel);
            }
            return hrr;
        }


        #endregion

        //public long GetIndexHotelReview(int hotelID, int writing, long userID, long hideUserID)
        //{
        //    return MongodbDAL.GetIndexHotelReview(hotelID, writing, userID, hideUserID);
        //}

        //public void CheckAllHotelList()
        //{

        //    List<int> Districtlist = HotelDAL.GetDistrictListForHotelCheckData();
        //    if (Districtlist != null)
        //        Districtlist.ForEach(p =>
        //        {

        //            if (p > 17542)
        //            {

        //                File.AppendAllText("D:\\HJDApp\\log\\hotellog.txt", DateTime.Now.ToShortTimeString() + " " + p.ToString() + ",");

        //                CheckHotelToMongodbByDistrict(new List<int> { p });
        //            }
        //        }
        //    );
        //}

        public void ClearHJDHotelcommentCacheData(int hotelid)
        {
            try
            {
                var hotelnewid = HotelDAL.GetHotelid(hotelid);
                memCacheHotelReview.Remove(hotelid.ToString(), CachedtHotelReviewKey);
            }
            catch { }
        }

        public void GenerateHJDHotelReviewCacheForApi(int o, int p, string q)
        {
            HotelDAL.GenerateHJDHotelReviewCacheForApi(o, p, q);
        }

        /// <summary>
        /// 清理所有酒店缓存信息
        /// </summary>
        /// <param name="hotelids"></param>
        /// <returns></returns>
        public string RefreshSomeHotelCache(List<int> hotelids)
        {
            ClearCacheHotelInfo(hotelids);

            return "成功";
        }

        public string RefreshAllHotelCache()
        {
            try
            {
                ClearCacheHotelInfoAll();
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }

            return "成功";
        }

        /// <summary>
        /// 清除缓存 酒店信息
        /// </summary>
        /// <param name="hotelID"></param>
        private void ClearCacheHotelInfo(List<int> hotelIDlist)
        {
            try
            {
                hotelIDlist.ForEach(p =>
                {
                    memCacheHotel.Remove(p.ToString(), HotelInfoExCacheKey);
                    memCacheHotel.Remove(p.ToString(), GetHotelKey);
                    memCacheHotel.Remove(p.ToString(), Hotel3CacheKey);
                });
   
            }
            catch { }
        }


        /// <summary>
        /// 清除酒店数据缓存
        /// </summary>
        /// <param name="hotelCacheType"></param>
        /// <param name="hotelIDlist"></param>
        /// <returns></returns>
        public bool ClearHotelCache(HJD.HotelServices.Contracts.HotelServiceEnums.HotelCacheType hotelCacheType, List<int> hotelIDlist)
        {
            bool result = false;
            try
            {
                string CacheKeyPrefix = "";

                switch (hotelCacheType)
                {
                    case HotelServiceEnums.HotelCacheType.Hotel3:
                        CacheKeyPrefix = Hotel3CacheKey;
                        break;
                    case HotelServiceEnums.HotelCacheType.Hotel:
                        CacheKeyPrefix = GetHotelKey;
                        break;
                    case HotelServiceEnums.HotelCacheType.HotelInfoExCacheKey:
                        CacheKeyPrefix = HotelInfoExCacheKey;
                        break;

                }

                if (CacheKeyPrefix.Length > 0)
                {
                    hotelIDlist.ForEach(p =>
                    {
                        memCacheHotel.Remove(p.ToString(), CacheKeyPrefix);
                    });
                    result = true;
                }

            }
            catch { }

            return result;
        }


        private void ClearCacheHotelInfoAll()
        {
            int i = int.MaxValue;
            List<int> hotellist;

            while (true)
            {
                hotellist = HotelDAL.GetHotelIDListForCheckData(i);

                if (hotellist.Count <= 0)
                    break;

                i = hotellist.Last();
                ClearCacheHotelInfo(hotellist);
            }
        }

        public string SetCommentWritingUseful(int writing, long userid, string ip)
        {
            string result = HotelDAL.InsertCommwritingCommentReview(userid, writing, ip);
            ClearCacheHotelReviewEx(writing);
            return result;
        }

        public List<HotelKeyWordListEntity> GetHotelKeyWordList(List<int> hotelID)
        {
            if (hotelID == null || hotelID.Count <= 0)
                return new List<HotelKeyWordListEntity>();

            List<HotelKeyWordListEntity> ls =
                memCacheHotel.GetMultiDataEx<int, HotelKeyWordListEntity>(hotelID, HotelKeyWordKey
                , noDataList =>
                {
                    return GetHotelKeyWordListAccess(noDataList);

                }
                , ReturnValue => ReturnValue.HotelID
                , new HotelKeyWordListEntity { HotelID = -1 });

            var x = (from i in ls
                     where i != null && i.HotelID > 0
                     select i).ToList();
            return x;
        }

        public List<HotelKeyWordListEntity> GetHotelKeyWordListAccess(List<int> hotelID)
        {
            List<HotelKeyWordListEntity> hkl = new List<HotelKeyWordListEntity>();

            Dictionary<string, string> dicR = new Dictionary<string, string>();
            #region
            dicR.Add("有装修", "有装修");
            dicR.Add("饭店很好", "饭店很好");
            dicR.Add("别墅泳池高级", "别墅泳池高级");
            dicR.Add("别墅泳池奢华", "别墅泳池奢华");
            dicR.Add("海景奢华", "海景奢华");
            dicR.Add("海景高级", "海景高级");
            dicR.Add("海景干净陈旧", "海景干净陈旧");
            dicR.Add("度假很大", "度假很大");
            dicR.Add("有房型干净", "有房型干净");
            dicR.Add("早餐干净", "早餐干净");
            dicR.Add("有房间", "有房间");
            #endregion

            List<HotelKeyWordEntity> hf = new List<HotelKeyWordEntity>();
            hf = HotelDAL.GetHotelKeyWordData(hotelID).Where(i => !i.keyword.EndsWith("不")).ToList();
            foreach (string rkey in dicR.Keys)
            {
                hf = hf.Where(i => i.keyword.Replace(",", "") != dicR[rkey]).ToList();
            }
            List<HotelKeyWordWritingEntity> hw = HotelDAL.GetHotelKeyWordWritingData(hotelID);


            foreach (HotelKeyWordEntity i in hf)
            {

                i.WritingList = (from h in hw where h.ID == i.ID && h.HotelID == i.HotelID orderby h.Writing descending select h.Writing).ToList();
                i.Number = i.WritingList.Count;
            }

            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("性价比，很好", "性价比，高");
            //
            foreach (string key in dic.Keys)
            {
                #region
                //性价比，很好性价比，高
                foreach (var j in hotelID)
                {
                    int fCount = (from h in hf where h.HotelID == j && h.keyword == dic[key] select h.WritingList).Count();
                    List<int> sCountList = (from h in hf where h.HotelID == j && h.keyword == key select h.WritingList).FirstOrDefault();

                    // (from h in hf where h.HotelID == j && h.keyword == "性价比,很好" select h.WritingList)
                    if (fCount > 0 && sCountList != null && sCountList.Count() > 0)
                    {
                        int indexNum = 0;
                        foreach (HotelKeyWordEntity i in hf)
                        {
                            indexNum += 1;
                            if (i.keyword == key && i.HotelID == j)
                            {
                                break;
                            }
                        }
                        foreach (HotelKeyWordEntity i in hf)
                        {
                            if (i.keyword == dic[key] && i.HotelID == j)
                            {
                                List<int> fWList = i.WritingList;
                                fWList.AddRange(sCountList);

                                List<int> xList = (from t in fWList group t by new { t } into g select g.Key.t).ToList();
                                i.WritingList = (from k in xList orderby k descending select k).ToList();
                                i.Number = i.WritingList.Count;
                                break;
                            }
                        }
                        hf.RemoveAt(indexNum - 1);
                    }
                }
                #endregion
            }
            //
            foreach (var i in hotelID)
            {
                HotelKeyWordListEntity hkwle = new HotelKeyWordListEntity();
                hkwle.HotelID = i;
                hkwle.HotelKeyWordList = hf.Where(y => y.HotelID == i).ToList();
                hkl.Add(hkwle);

            }
            return hkl;
        }

        public void RefreshCacheGetHotelKeyWordList(List<int> hotelID)
        {

            foreach (var item in hotelID)
            {

                List<HotelKeyWordListEntity> hh = GetHotelKeyWordListAccess(new List<int> { item });
                //if (hh == null || hh.Count <= 0)
                //    hh = new List<HotelInfoExEntity>();
                memCacheHotel.Set(item.ToString(), HotelKeyWordKey, hh.FirstOrDefault());
            }
        }

        public HotelReviewSaveResult SetHotelReviewWritingLog(HotelReviewWritingLog hcl)
        {
            return HotelDAL.sp1_HotelReview_Log(hcl);
        }

        #region 酒店关键字字库
        public List<CommentKeyWordListEntity> GetCommentKeyWordList(string keyword)
        {
            string key = "CommentKeyWord_" + keyword + "_";
            List<CommentKeyWordListEntity> list = memCacheNewestHotelReview.GetData<List<CommentKeyWordListEntity>>(key, CommentKeyWordCacheKey, () =>
            {
                return GetCommentKeyWordListAccess(keyword);
            });
            return list;
        }

        public void SetGetCommentKeyWordList(string keyword)
        {
            List<CommentKeyWordListEntity> list = GetCommentKeyWordListAccess(keyword);
            string key = "CommentKeyWord_" + keyword + "_";
            memCacheHotel.Set(key, CommentKeyWordCacheKey, list);
        }

        private List<CommentKeyWordListEntity> GetCommentKeyWordListAccess(string keyword)
        {
            List<CommentKeyWordListEntity> ckwle = new List<CommentKeyWordListEntity>();
            var query = HotelDAL.GetCommentKeyWordListData(keyword);

            var Hg = (from t in query group t by new { t.KeyWord } into g select g.Key);
            foreach (var i in Hg)
            {
                CommentKeyWordListEntity ce = new CommentKeyWordListEntity();
                ce.KeyWord = i.KeyWord;
                ce.KeyWordExt = (from j in query where j.KeyWord == i.KeyWord select j.KeyWordExt).ToList();
                ckwle.Add(ce);
            }
            return ckwle;
        }

        #endregion

        public List<int> GetHotelSimilarCommentListByWriting(int writing)
        {
            string key = "HotelSimilarComment" + writing + "_";
            List<int> list = memCacheNewestHotelReview.GetData<List<int>>(key, HotelSimlarCommentCacheKey, () =>
            {
                return GetHotelSimilarCommentListByWritingAccess(writing);
            });
            return list;
        }
        public List<int> GetHotelSimilarCommentListByWritingAccess(int writing)
        {
            return HotelDAL.GetHotelSimilarCommentByWriting(writing);
        }

        public void SetGetHotelSimilarCommentListByWriting(int writing)
        {
            List<int> list = GetHotelSimilarCommentListByWritingAccess(writing);
            string key = "HotelSimilarComment" + writing + "_";
            memCacheHotel.Set(key, HotelSimlarCommentCacheKey, list);
        }

        public List<int> SimilarReviewHotelList()
        {
            string key = "SimilarReviewHotelList_";
            List<int> list = memCacheNewestHotelReview.GetData<List<int>>(key, "Hotel", () =>
            {
                return GetSimilarReviewHotelListAccess();
            });
            return list;
        }

        public List<int> GetSimilarReviewHotelListAccess()
        {
            return HotelDAL.GetSimilarReviewHotelList();
        }

        public void SetSimilarReviewHotelList()
        {
            List<int> list = GetSimilarReviewHotelListAccess();
            string key = "SimilarReviewHotelList_";
            memCacheHotel.Set(key, "Hotel", list);
        }

        #region 短租（度假公寓）mongodb  hcdu 2013-01-17 用目的地为入口，是数据量大防止超时

        public List<VacationHotelPyRank> GetVacationHotelPyRankByDistrict(int district)
        {
            string key = "HotelVacation_" + district + "_";
            List<VacationHotelPyRank> list = memCacheNewestHotelReview.GetData<List<VacationHotelPyRank>>(key, "HotelVacationKey", () =>
            {
                return HotelDAL.GetVacationHotelPyRank(district);
            });
            return list;
        }
        public void SetGetVacationHotelPyRankByDistrict(int district)
        {
            List<VacationHotelPyRank> list = HotelDAL.GetVacationHotelPyRank(district);
            string key = "HotelVacation_" + district + "_";
            memCacheHotel.Set(key, "HotelVacationKey", list);
        }

        private VacationInfoMongoDB TransformVacationHotelReviewToMongoDB(UnitInfoEntity pifd)
        {
            VacationInfoMongoDB ret = new VacationInfoMongoDB();


            ret.HotelID = pifd.ID;

            ret.Zone = pifd.Zone;
            ret.Location = pifd.Location;
            ret.DistrictID = pifd.DistrictID;
            //拼音排序
            List<VacationHotelPyRank> vr = GetVacationHotelPyRankByDistrict(pifd.DistrictID);
            ret.pySort = vr.Where(i => i.VacationHotelID == pifd.ID).FirstOrDefault().PyRank;

            ret.Signature = "";

            MemoryStream fs = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(fs, ret.HotelID.ToString() + "-" + ret.DistrictID.ToString());

            //new
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();

            //获取密文字节数组
            byte[] bytResult = md5.ComputeHash(fs.GetBuffer());
            fs.Close();

            ret.Signature = BitConverter.ToString(bytResult).Replace("-", "");

            return ret;
        }
        #endregion

        //public List<HotelListInfoEntity> GetHotelListByQuery(HotelSearchParas p, out long count)
        //{
        //    List<long> filterCol = new List<long>();

        //    if (p.ClassID != null && p.ClassID.Length > 0)
        //        foreach (int i in p.ClassID)
        //        {
        //            filterCol.Add(HotelListFilterPrefix.Class + i);
        //        }

        //    if (p.TagIDs != null && p.TagIDs.Length > 0)
        //        foreach (int i in p.TagIDs)
        //        {
        //            filterCol.Add(HotelListFilterPrefix.Tags + i);
        //        }

        //    if (p.Location != null && p.Location.Length > 0)
        //        foreach (int i in p.Location)
        //        {
        //            filterCol.Add(HotelListFilterPrefix.Location + i);
        //        }

        //    if (p.Zone != null && p.Zone.Length > 0)
        //        foreach (int i in p.Zone)
        //        {
        //            filterCol.Add(HotelListFilterPrefix.Zone + i);
        //        }

        //    if (p.Brands != null && p.Brands.Length > 0)
        //        foreach (int i in p.Brands)
        //        {
        //            filterCol.Add(HotelListFilterPrefix.Brand + i);
        //        }

        //    if (p.Facilitys != null && p.Facilitys.Length > 0)
        //        foreach (int i in p.Facilitys)
        //        {
        //            filterCol.Add(HotelListFilterPrefix.Facility + i);
        //        }

        //    if (p.Star != null && p.Star.Length > 0)
        //        foreach (int i in p.Star)
        //        {
        //            filterCol.Add(HotelListFilterPrefix.Star + i);
        //        }

        //    if (p.MinPrice <= 0 && p.MaxPrice == decimal.MaxValue)
        //    {
        //        p.MinPrice = 0;
        //        p.MaxPrice = 0;
        //    }

        //    if (p.NearbyGroupID > 0)
        //        filterCol.Add(HotelListFilterPrefix.NearbyGroup + p.NearbyGroupID);

        //    HotelListSortType sortType = (HotelListSortType)p.SortType; // TODO: 初始化为适当的值
        //    OrderBy orderBy = p.SortDirection == 1 ? OrderBy.Desc : OrderBy.Asc; // TODO: 初始化为适当的值

        //    var hl = MongodbDAL.GetHotelList(p.DistrictID, filterCol, p.CheckInDate, p.CheckOutDate, p.MinPrice, p.MaxPrice, p.StartIndex, p.ReturnCount, sortType, orderBy, out count, p.Lat, p.Lng, p.Distance, p.nLat, p.nLng, p.sLat, p.sLng);
        //    //.Select(o => new HotelListInfoEntity { _id = o._id, AvgPrice = o.AvgPrice }).ToList();

        //    //已分页，获取到count
        //    if (count > 0)
        //        return ConvertAvgPrice(hl, p.CheckInDate, p.CheckOutDate).Select(o => new HotelListInfoEntity { _id = o._id, AvgPrice = o.AvgPrice }).ToList();

        //    if (p.MaxPrice > 0)
        //    {
        //        hl = ConvertAvgPrice(hl, p.CheckInDate, p.CheckOutDate, p.MinPrice, p.MaxPrice);
        //    }
        //    count = hl.Count;

        //    if (sortType == HotelListSortType.Price)
        //    {
        //        if (orderBy == OrderBy.Asc)
        //        {
        //            hl = hl.OrderBy(h => h.AvgPrice).Skip(p.StartIndex).Take(p.ReturnCount).ToList();
        //        }
        //        else
        //        {
        //            hl = hl.OrderByDescending(h => h.AvgPrice).Skip(p.StartIndex).Take(p.ReturnCount).ToList();
        //        }
        //    }
        //    else if (sortType == HotelListSortType.Rank)
        //    {
        //        if (orderBy == OrderBy.Asc)
        //        {
        //            hl = hl.OrderBy(h => h.Rank).Skip(p.StartIndex).Take(p.ReturnCount).ToList();
        //        }
        //        else
        //        {
        //            hl = hl.OrderByDescending(h => h.Rank).Skip(p.StartIndex).Take(p.ReturnCount).ToList();
        //        }
        //    }
        //    else  //HotelListSortType.Distance
        //    {
        //        int poiid = (int)(filterCol.Where(f => f > HotelListFilterPrefix.NearBy).Where(f => f < HotelListFilterPrefix.Facility).First() - HotelListFilterPrefix.NearBy);

        //        if (p.MaxPrice == 0)
        //        {
        //            hl = ConvertAvgPrice(SortHotelListByDistance(hl, poiid, orderBy).Skip(p.StartIndex).Take(p.ReturnCount).ToList(), p.CheckInDate, p.CheckOutDate);
        //        }
        //        else
        //        {
        //            hl = SortHotelListByDistance(hl, poiid, orderBy).Skip(p.StartIndex).Take(p.ReturnCount).ToList();
        //        }
        //    }
        //    return hl.Select(o => new HotelListInfoEntity { _id = o._id, AvgPrice = o.AvgPrice }).ToList();
        //}

        private List<HotelListInfoForMongoDB> ConvertAvgPrice(List<HotelListInfoForMongoDB> hl, DateTime checkInDate, DateTime checkOutDate, decimal minValue = 0, decimal maxValue = 0)
        {
            if (hl == null)
                return null;
            System.Diagnostics.Stopwatch st = new System.Diagnostics.Stopwatch();
            st.Start();
            List<List<int>> l = new List<List<int>>();
            var ids = hl.Select(h => h._id).ToList();
            int num = 50;
            for (int i = 0; i <= ids.Count / num; i++)
            {
                l.Add(ids.Skip(i * num).Take(num).ToList());
            }
            List<Task> tasks = new List<Task>();
            ConcurrentDictionary<int, List<HotelPrice>> tmp = new ConcurrentDictionary<int, List<HotelPrice>>();

            for (int i = 0; i < l.Count; i++)
            {
                tasks.Add(Task.Factory.StartNew(a =>
                {
                    List<HotelPrice> h = null;
                    if (maxValue == 0)
                        h = QueryHotelListPrice(l[(int)a], checkInDate, checkOutDate);
                    else
                        h = QueryHotelListPrice(l[(int)a], checkInDate, checkOutDate, minValue, maxValue);
                    tmp[(int)a] = h;

                }, i));
            }
            Task.WaitAll(tasks.ToArray());
            st.Stop();
            var time = st.ElapsedMilliseconds;
            List<HotelPrice> hpl = new List<HotelPrice>();
            foreach (var x in tmp.Values)
            {
                hpl.AddRange(x);
            }
            if (hpl.Count > 0)
            {
                if (maxValue > 0)
                {
                    List<int> priceIds = hpl.Select(h => h.HotelId).ToList();
                    hl = hl.Where(h => priceIds.Contains(h._id)).ToList();
                }
                foreach (var h in hl)
                {
                    var thp = hpl.Where(hp => hp.HotelId == h._id);

                    h.AvgPrice = thp.Count() == 1 ? thp.First().Price : 0;
                }
            }
            return hl;
        }

        private List<HotelListInfoForMongoDB> SortHotelListByDistance(List<HotelListInfoForMongoDB> hl, int poiid, OrderBy orderBy)
        {
            if (orderBy == OrderBy.Asc)
            {
                return hl.OrderBy(h => h.HotelDistances.Where(r => r.POIID == poiid).FirstOrDefault().Distance).ToList();
            }
            else
            {
                return hl.OrderByDescending(h => h.HotelDistances.Where(r => r.POIID == poiid).FirstOrDefault().Distance).ToList();
            }
        }

        public int GetHotelIDByHotelOriID(int hotelOriID)
        {
            return int.Parse(memCacheHotel.GetData<object>(hotelOriID.ToString(), GetHotelIDByHotelOriIDKey, () => HotelDAL.GetHotelIDByHotelOriID(hotelOriID)).ToString());
        }

        public string GetBookingUrlByHotel(int hotelID)
        {
            return memCacheHotel.GetData<object>(hotelID.ToString(), GetBookingURLKey, () => HotelDAL.GetBookingUrlByHotel(hotelID)).ToString();
        }

        public string GetAgodaUrlByHotel(int hotelID)
        {
            return memCacheHotel.GetData<object>(hotelID.ToString(), GetAgodaURLKey, () => HotelDAL.GetAgodaUrlByHotel(hotelID)).ToString();
        }

        public List<int> GetHotelTagReviewID(int tagID, int hotelID)
        {
            return memCacheHotel.GetData<List<int>>(string.Format("{0}_{1}", tagID, hotelID), HotelTagReviewCacheKey,
                () =>
                {
                    return HotelDAL.GetHotelTagReviewID(tagID, hotelID);
                }
            );
        }

        public List<TagEntity> GetTagParams4HotelList(int districtID, int type)
        {
            List<int> classID = GetClassIDs(type);
            classID.Sort();
            string classIDs = string.Join<int>(",", classID);
            return memCacheHotel.GetData<List<TagEntity>>(string.Format("{0}_{1}", districtID, classIDs), TagParamsCacheKey,
                () =>
                {
                    return HotelDAL.GetTagParams4HotelList(districtID, classIDs);
                }
            );
        }

        public List<TagNameEntity> GetTagList(List<int> tagIDs)
        {
            return memCacheHotel.GetMultiDataEx<int, TagNameEntity>(tagIDs, TagsCacheKey, noDataList =>
                                                                    HotelDAL.GetTagList(noDataList),
                                                                    hp => hp.ID,
                                                                    new TagNameEntity { ID = 0, Name = "无" });
        }

        public List<TagEntity> GetHotelTag(int hotelID)
        {
            return memCacheHotel.GetData<List<TagEntity>>(hotelID.ToString(), HotelTagCacheKey,
                () =>
                {
                    return HotelDAL.GetHotelTag(hotelID);
                }
            );
        }

        public List<FeaturedEntity> GetHotelFeatured(int hotelID)
        {
            return memCacheHotel.GetData<List<FeaturedEntity>>(hotelID.ToString(), HotelTagCacheKey,
                () =>
                {
                    return HotelDAL.GetHotelFeatured(hotelID);
                }
            );
        }

        public List<HotelPrice> QueryHotelListPrice(List<int> hotelIDs, DateTime arrivalTime, DateTime departureTime, decimal minPrice = 0, decimal maxPrice = 1000000)
        {
            var cachedData = HotelListPriceCache.GetMultiDataEx(hotelIDs,
                                                                   string.Format("{0}:{1}:{2}", PrefixHotelListPrice, arrivalTime.ToShortDateString(), departureTime.ToShortDateString()),
                                                                    noDataList =>
                                                                    GetHotelListPrice(
                                                                        noDataList,
                                                                        arrivalTime, departureTime),
                                                                    hp => hp.HotelId,
                                                                    new HotelPrice { HotelId = 0, Price = 0 }
                );

            //返回最终结果
            if (minPrice == -1)
                return (from price in cachedData
                        select price).ToList();
            else //返回一个区间段的价格
                return (from price in cachedData
                        where price.Price >= minPrice && price.Price < maxPrice
                        select price).ToList();
        }

        /// <summary>
        /// 获取酒店列表价。如果各OTA有特殊的价格计划，可以在这里处理
        /// </summary>
        /// <param name="hotelIDs"></param>
        /// <param name="arrivalTime"></param>
        /// <param name="departureTime"></param>
        /// <returns></returns>
        public List<HotelPrice> GetHotelListPriceWithOTAID(List<int> hotelIDs, DateTime arrivalTime, DateTime departureTime, int OTAID)
        {
            List<HotelOTAPrice> hpl = HotelDAL.QueryHotelOTAPriceListWithOTAID(hotelIDs, arrivalTime, departureTime, OTAID);
            return (from h in hpl
                    group h by new { h.HotelId }
                        into hgroup
                        select new HotelPrice { HotelId = hgroup.Key.HotelId, Price = (int)hgroup.Min(h => h.Price) }
                        ).ToList();
        }

        public List<HotelPrice> QueryHotelListPriceWithOTAID(List<int> hotelIDs, DateTime arrivalTime, DateTime departureTime, int OTAID, decimal minPrice = 0, decimal maxPrice = 1000000)
        {
            var cachedData = HotelListPriceCache.GetMultiDataEx(hotelIDs,
                                                                   string.Format("{0}:{1}:{2}:{3}", PrefixHotelListPrice, OTAID, arrivalTime.ToShortDateString(), departureTime.ToShortDateString()),
                                                                    noDataList =>
                                                                    GetHotelListPriceWithOTAID(
                                                                        noDataList,
                                                                        arrivalTime, departureTime, OTAID),
                                                                    hp => hp.HotelId,
                                                                    new HotelPrice { HotelId = 0, Price = 0 }
                );

            //返回最终结果
            if (minPrice == -1)
                return (from price in cachedData
                        select price).ToList();
            else //返回一个区间段的价格
                return (from price in cachedData
                        where price.Price >= minPrice && price.Price < maxPrice
                        select price).ToList();
        }

        /// <summary>
        /// 获取酒店列表价。如果各OTA有特殊的价格计划，可以在这里处理
        /// </summary>
        /// <param name="hotelIDs"></param>
        /// <param name="arrivalTime"></param>
        /// <param name="departureTime"></param>
        /// <returns></returns>
        public List<HotelPrice> GetHotelListPrice(List<int> hotelIDs, DateTime arrivalTime, DateTime departureTime)
        {
            List<HotelOTAPrice> hpl = HotelDAL.QueryHotelOTAPriceList(hotelIDs, arrivalTime, departureTime);
            return (from h in hpl
                    group h by new { h.HotelId }
                        into hgroup
                        select new HotelPrice { HotelId = hgroup.Key.HotelId, Price = (int)hgroup.Min(h => h.Price) }
                        ).ToList();
        }

        /// <summary>
        /// 获取酒店对应关联，以便同步生产数据
        /// </summary>
        /// <param name="CreateTime"></param>
        /// <returns></returns>
        public List<HotelOTAEntity> GetHotelOTAByCreateTime(DateTime CreateTime)
        {
            return HotelDAL.GetHotelOTAByCreateTime(CreateTime);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="menuID">1：中档；2：经济；3：奢侈；4：度假村；5：精品</param>
        /// <returns>1:20；2:1,2,3,4；3:21；4:10；5：11</returns>
        public List<int> GetClassIDs(int menuID)
        {
            switch (menuID)
            {
                case 1: return new List<int> { 20 };
                case 2: return new List<int> { 2, 3 };
                case 3: return new List<int> { 21 };
                case 4: return new List<int> { 10 };
                case 5: return new List<int> { 11 };
                default: return new List<int>();
            }
        }

        public QueryReviewResult QueryReview(ArguHotelReview p)
        {
            QueryReviewResult qrr = new QueryReviewResult();

            long FirstFilter = HotelReviewFilterPrefix.Hotel + p.Hotel;

            List<long> Filter = new List<long>();

            if (p.UserIdentityType != UserIdentityType.All)
            {
                Filter.Add(HotelReviewFilterPrefix.UserIdentity + (int)p.UserIdentityType);
            }

            //特征标签ID
            if (p.FeaturedTreeID > 0)
            {
                Filter.Add(HotelReviewFilterPrefix.FeaturedTreeID + p.FeaturedTreeID);
            }

            //特征标签数组
            if (p.FeaturedTreeList != null && p.FeaturedTreeList.Count != 0)
            {
                foreach (var featureTreeId in p.FeaturedTreeList)
                {
                    Filter.Add(HotelReviewFilterPrefix.FeaturedTreeID + featureTreeId);
                }
            }

            //房型特殊处理
            if (p.RoomType > 0)
            {
                Filter.Add(HotelReviewFilterPrefix.RoomType + p.RoomType);
            }

            if (p.Featured > 0)
            {
                Filter.Add(HotelReviewFilterPrefix.Featured + p.Featured);
            }

            if (p.Tag > 0)
            {
                Filter.Add(HotelReviewFilterPrefix.Tag + p.Tag);
            }

            if (p.Theme > 0)
            {
                Filter.Add(HotelReviewFilterPrefix.Theme + p.Theme);
            }

            if (p.RatingType != RatingType.All)
            {
                Filter.Add(HotelReviewFilterPrefix.Rate + (int)p.RatingType);
                //switch (p.RatingType)
                //{
                //    case RatingType.Best:
                //    case RatingType.Better:
                //        {
                //            Filter.Add(HotelReviewFilterPrefix.Rate + (int)RatingType.Best);
                //            Filter.Add(HotelReviewFilterPrefix.Rate + (int)RatingType.Better);
                //            break;
                //        }
                //    case RatingType.Good:
                //        Filter.Add(HotelReviewFilterPrefix.Rate + (int)p.RatingType); break;
                //    case RatingType.Normal:
                //    case RatingType.Terrible:
                //        {
                //            Filter.Add(HotelReviewFilterPrefix.Rate + (int)RatingType.Normal);
                //            Filter.Add(HotelReviewFilterPrefix.Rate + (int)RatingType.Terrible);
                //            break;
                //        }
                //}
            }

            int orderBy = 0;// INT, -- time = 0 score = 1  source = 5
            int orderDirect = 0;//INT --1 : OrderBy.Desc   0: OrderBy.Asc

            switch (p.HotelReviewOrderType)
            {
                case HotelReviewOrderType.Time_Up:
                    orderBy = 0;
                    orderDirect = 0;
                    break;
                case HotelReviewOrderType.Time_Down:
                    orderBy = 0;
                    orderDirect = 1;
                    break;

                case HotelReviewOrderType.Rating_Up:
                    orderBy = 1;
                    orderDirect = 0;
                    break;
                case HotelReviewOrderType.Rating_Down:
                    orderBy = 1;
                    orderDirect = 1;
                    break;
                case HotelReviewOrderType.Source:
                    orderBy = 2;
                    orderDirect = 1;
                    break;
            }
            try {
                //LogHelper.WriteLog("sss" + FirstFilter+"," + string.Join(",", Filter));
            DataSet ds = HotelDAL.QueryReview(FirstFilter, Filter, p.Start, p.Count, orderBy, orderDirect);
            qrr.TotalCount = (int)ds.Tables[0].Rows[0][0];
            foreach (DataRow r in ds.Tables[1].Rows)
            {
                qrr.FilterCount.Add((long)r[0], (int)r[1]);
            }

            List<WritingIDGroup> ridList = new List<WritingIDGroup>();
            foreach (DataRow r in ds.Tables[2].Rows)
            {
                ridList.Add(new WritingIDGroup { writing = (int)r[0], type = (Int16)r[1] });
            }

            qrr.ReviewList = GetHotelReviewEx(ridList);// p.Featured == 0 ?:GetFeatruedReview(ridList,p.Featured);

            if (p.Hotel == 686500 && DateTime.Now <= new DateTime(2016, 4, 11, 23, 59, 59))
            {
                LogHelper.WriteLog(string.Format("这家酒店{0},Filter:{1},Start:{2},Count:{3},orderBy:{4},orderDirect:{5},ridList:{6},ReviewList:{7}", p.Hotel, string.Join(",", Filter), p.Start, p.Count, orderBy, orderDirect, string.Join(",", ridList.Select(_ => _.writing)), string.Join(",", qrr.ReviewList.Select(_ => _.Writing))));
            }
            }
            catch (Exception e)
            {
                LogHelper.WriteLog("error " + e);
            }
            return qrr;
        }

        public List<HotelReviewExEntity> QueryCommentReview(ArguHotelReview p)
        {
            List<HotelReviewExEntity> hre = new List<HotelReviewExEntity>();

            long FirstFilter = HotelReviewFilterPrefix.Hotel + p.Hotel;

            List<long> Filter = new List<long>();


            int orderBy = 0;// INT, -- time = 0 score = 1  source = 5
            int orderDirect = 0;//INT --1 : OrderBy.Desc   0: OrderBy.Asc

            switch (p.HotelReviewOrderType)
            {
                case HotelReviewOrderType.Time_Up:
                    orderBy = 0;
                    orderDirect = 0;
                    break;
                case HotelReviewOrderType.Time_Down:
                    orderBy = 0;
                    orderDirect = 1;
                    break;

                case HotelReviewOrderType.Rating_Up:
                    orderBy = 1;
                    orderDirect = 0;
                    break;
                case HotelReviewOrderType.Rating_Down:
                    orderBy = 1;
                    orderDirect = 1;
                    break;
                case HotelReviewOrderType.Source:
                    orderBy = 2;
                    orderDirect = 1;
                    break;
            }

            DataSet ds = HotelDAL.QueryReview(FirstFilter, Filter, p.Start, p.Count = 10, orderBy, orderDirect);


            List<WritingIDGroup> ridList = new List<WritingIDGroup>();
            foreach (DataRow r in ds.Tables[2].Rows)
            {
                ridList.Add(new WritingIDGroup { writing = (int)r[0], type = (Int16)r[1] });
            }
            hre = GetHotelReviewEx(ridList);
            if (p.Hotel == 686500 && DateTime.Now <= new DateTime(2016, 4, 11, 23, 59, 59))
            {
                LogHelper.WriteLog(string.Format("这家酒店{0},Filter:{1},Start:{2},Count:{3},orderBy:{4},orderDirect:{5},ridList:{6},ReviewList:{7}", p.Hotel, string.Join(",", Filter), p.Start, p.Count, orderBy, orderDirect, string.Join(",", ridList.Select(_ => _.writing)), string.Join(",", hre)));
            }
            return hre;
        }


        //public QueryReviewResult QuerySEOKeywordReview(ArguHotelSEOKeywordReview p)
        //{
        //    QueryReviewResult qrr = new QueryReviewResult();

        //    int orderBy = 0;// INT, -- time = 0 score = 1
        //    int orderDirect = 0;//INT --1 : OrderBy.Desc   0: OrderBy.Asc

        //    switch (p.HotelReviewOrderType)
        //    {
        //        case HotelReviewOrderType.Time_Up:
        //            orderBy = 0;
        //            orderDirect = 0;
        //            break;
        //        case HotelReviewOrderType.Time_Down:
        //            orderBy = 0;
        //            orderDirect = 1;
        //            break;

        //        case HotelReviewOrderType.Rating_Up:
        //            orderBy = 1;
        //            orderDirect = 0;
        //            break;
        //        case HotelReviewOrderType.Rating_Down:
        //            orderBy = 1;
        //            orderDirect = 1;
        //            break;
        //    }

        //    DataSet ds = HotelDAL.QuerySEOKeywordReview(p.HotelID, p.SEOKeywordID, p.StartIndex, p.ReturnCount, orderBy, orderDirect);

        //    qrr.TotalCount = (int)ds.Tables[0].Rows[0][0];


        //    List<int> ridList = new List<int>();
        //    foreach (DataRow r in ds.Tables[1].Rows)
        //    {
        //        ridList.Add((int)r[0]);
        //    }
        //    qrr.ReviewList = GetHotelReviewEx(ridList);

        //    return qrr;
        //}

        /// <summary>
        /// 获取酒店照片列表
        /// </summary>
        /// <param name="hotelID"></param>
        /// <returns></returns>
        public HotelPhotosEntity GetHotelPhotos(int hotelID)
        {
            return memCacheHotel.GetData<HotelPhotosEntity>(hotelID.ToString(), HotelPhotosCacheKey, () =>
            {
                HotelPhotosEntity hps = new HotelPhotosEntity();
                hps.HPList = HotelDAL.GetHotelPhotos(hotelID);
                List<SimpleHotelItem> sh = GetSimpleHotels(new List<int> { hotelID });
                hps.HotelName = sh.Count > 0 ? sh.First().Name : "";
                hps.HotelId = hotelID;
                return hps;
            });
        }

        /// <summary>
        /// 批量获取酒店照片列表
        /// </summary>
        public List<HotelPhotosEntity> GetManyHotelPhotos(IEnumerable<int> hotelIds)
        {
            var result = new List<HotelPhotosEntity>();
            if (hotelIds != null && hotelIds.Count() != 0)
            {
                foreach (var hotelId in hotelIds)
                {
                    result.Add(GetHotelPhotos(hotelId));
                }
            }
            return result;
        }

        public bool RefreshHotelPhotos(int hotelID)
        {
            memCacheHotel.Remove(hotelID.ToString(), HotelPhotosCacheKey);
            return true;
        }

        public QueryHotelResult2 QueryHotel2(HotelSearchParas p, int PriceWithOTAID = 0)
        {
            QueryHotelResult qr = QueryHotel(p, PriceWithOTAID);
            QueryHotelResult2 q = new QueryHotelResult2();
            q.FilterCount = qr.FilterCount;
            q.TotalCount = qr.TotalCount;
            q.HotelList = TransToListHotelItem(qr.HotelList, p.Interest);

            return q;
        }

        public QueryHotelResult3 QueryHotel3(HotelSearchParas p, int PriceWithOTAID = 0)
        {
            QueryHotelResult qr = QueryHotel(p, PriceWithOTAID);
            QueryHotelResult3 q = new QueryHotelResult3();
            q.FilterCount = qr.FilterCount;
            q.TotalCount = qr.TotalCount;
            q.HotelList = TransToListHotelItem3(qr.HotelList, p.Interest);
            //LogHelper.WriteLog("TotalCount " + qr.TotalCount + ", HotelList " + qr.HotelList.Count + ", HotelList2 : " + q.HotelList.Count);
            //LogHelper.WriteLog(string.Format("QueryHotel3:玩点:{0},主题:{1},纬度{2},经度{3},SortType:{4},SortDirection:{5},", p.InterestPlace, p.Interest, p.Lat, p.Lng, p.SortType, p.SortDirection));

            return q;
        }

        public QueryHotelResult3 QueryHotelForMagicall(HotelSearchParas p, int PriceWithOTAID = 0)
        {
            QueryHotelResult qr = QueryHotelForMagiCall(p, PriceWithOTAID);
            QueryHotelResult3 q = new QueryHotelResult3();
            q.FilterCount = qr.FilterCount;
            q.TotalCount = qr.TotalCount;
            q.HotelList = TransToListHotelItem3(qr.HotelList, p.Interest);

            //LogHelper.WriteLog(string.Format("QueryHotel3:玩点:{0},主题:{1},纬度{2},经度{3},SortType:{4},SortDirection:{5},", p.InterestPlace, p.Interest, p.Lat, p.Lng, p.SortType, p.SortDirection));

            return q;
        }

        private List<ListHotelItem2> TransToListHotelItem(List<SimpleHotelItem> list, int interestID)
        {
            List<int> ids = (from h in list
                             select h.Id).ToList();

            if (ids.Count == 0) return new List<ListHotelItem2>();


            return memCacheHotel.GetMultiDataEx<int, ListHotelItem2>(ids, string.Format("{0}:{1}:", GetListHotelItemsKey, interestID), noList =>
            {
                return (from h in list.Where(l => noList.Contains(l.Id))
                        select new ListHotelItem2()
                        {
                            Currency = h.Currency,
                            FeaturedList = GetHotelInterestTag(h.Id, interestID).Select(s => new FeaturedEntity() { ID = s.TFTID, Num = 0, Name = s.TFTName }).ToList(),
                            GLat = h.GLat,
                            GLon = h.GLon,
                            Id = h.Id,
                            MinPrice = h.MinPrice,
                            Name = h.Name,
                            PicSURL = h.PicSURL,
                            Rank = h.Rank,
                            ReviewCount = h.ReviewCount,
                            Score = h.Score,
                            PackgeList = HotelDAL.GetPackageListByHotelId(h.Id)
                        }).ToList();
            }
            , ReturnValue => ReturnValue.Id
                , new ListHotelItem2 { Id = -1 });

        }

        private List<ListHotelItem3> TransToListHotelItem3(List<SimpleHotelItem> list, int interestID)
        {
            List<int> ids = (from h in list
                             select h.Id).ToList();

            if (ids.Count == 0) return new List<ListHotelItem3>();
            return memCacheHotel.GetMultiDataEx<int, ListHotelItem3>(ids, string.Format("_{0}:{1}:", GetListHotelItemsKey, interestID), noList =>
            {
                return (from h in list.Where(l => noList.Contains(l.Id))
                        select new ListHotelItem3()
                        {
                            Currency = h.Currency,
                            GLat = h.GLat,
                            GLon = h.GLon,
                            Id = h.Id,
                            MinPrice = h.MinPrice,
                            Name = h.Name,
                            PictureSURLList = GetHotelPhotos(h.Id).HPList.Take(5).Select(photo => photo.SURL).ToList(),
                            PictureList = new List<string>(),
                            Rank = h.Rank,
                            Score = h.Score,
                            ReviewCount = QueryReview(new ArguHotelReview() { Hotel = h.Id }) != null ? QueryReview(new ArguHotelReview() { Hotel = h.Id }).TotalCount : 0,//h.ReviewCount,
                            HotelStar = h.Star.ToString(),
                            DistrictName = h.DistrictName,
                            ShortName = h.ShortName,
                            RecommendCount = h.RecommendCount
                        }).ToList();
            }
            , ReturnValue => ReturnValue.Id
                , new ListHotelItem3 { Id = -1 });

        }

        private List<ListHotelItem3> TransToListHotelItemWeixin(List<SimpleHotelItem> list, int interestID)
        {
            List<int> ids = (from h in list
                             select h.Id).ToList();

            if (ids.Count == 0) return new List<ListHotelItem3>();


            return memCacheHotel.GetMultiDataEx<int, ListHotelItem3>(ids, string.Format("{0}:{1}:", GetListHotelWeixinItemsKey, interestID), noList =>
            {
                return (from h in list.Where(l => noList.Contains(l.Id))
                        select new ListHotelItem3()
                        {
                            Currency = h.Currency,
                            GLat = h.GLat,
                            GLon = h.GLon,
                            Id = h.Id,
                            MinPrice = h.MinPrice,
                            Name = h.Name,
                            PictureSURLList = GetHotelPhotos(h.Id).HPList.Take(5).Select(photo => photo.SURL).ToList(),
                            PictureList = new List<string>(),
                            Rank = h.Rank,
                            Score = h.Score,
                            ReviewCount = h.ReviewCount
                        }).ToList();
            }
            , ReturnValue => ReturnValue.Id
                , new ListHotelItem3 { Id = -1 });

        }

        public List<long> GenQueryHotelParams(HotelSearchParas p, ref RankType rt, ref long firstFilter)
        {
            QueryHotelResult qhr = new QueryHotelResult();

            List<long> filterCol = new List<long>();


            if (p.AroundCityID > 0)
            {
                firstFilter = HotelListFilterPrefix.CityAround + p.AroundCityID;
                rt = RankType.district;
                if (p.Interest > 0)
                {
                    filterCol.Add(HotelListFilterPrefix.Interest + p.Interest);
                }
            }
            else if (p.Interest > 0)
            {
                firstFilter = HotelListFilterPrefix.Interest + p.Interest;
                rt = RankType.interest;
            }
            else if (p.ZonePlaceID > 0)
            {
                firstFilter = HotelListFilterPrefix.ZonePlace + p.ZonePlaceID;
                rt = RankType.interest;
            }
            else if (p.Attraction > 0)
            {
                firstFilter = HotelListFilterPrefix.Attraction + p.Attraction;
                rt = RankType.attraction;
            }
            else if (p.HotelTheme > 0)
            {
                firstFilter = HotelListFilterPrefix.HotelTheme + p.HotelTheme;
                rt = RankType.hoteltheme;
            }
            else if (p.DistrictID > 0)
            {
                firstFilter = HotelListFilterPrefix.DistrictID + p.DistrictID;
                rt = RankType.district;
            }
            else if (p.ClassID != null && p.ClassID.Length > 0)
            {
                firstFilter = HotelListFilterPrefix.Class + p.ClassID.First();
                rt = RankType.district;
            }
            else if (p.ChannelHotel == 1)//酒店中档频道页（无目的地，无class）
            {
                firstFilter = HotelListFilterPrefix.ChannelHotel;
                rt = RankType.district;
            }



            ////如果有目的地，目的地作为一个过滤项
            //if (p.HotelTheme > 0 && p.DistrictID > 0)
            //{
            //    filterCol.Add(HotelListFilterPrefix.DistrictID + p.DistrictID);
            //}

            if (p.HotelState != null && p.HotelState.Length > 0)
            {
                foreach (int i in p.HotelState)
                {
                    if (i > 0)
                        filterCol.Add(HotelListFilterPrefix.HotelState + i);
                }
            }

            if (p.InterestPlace != null && p.InterestPlace.Length > 0)
            {
                if (p.AroundCityID <= 0) //如果是查询城市周边的主题酒店，这里就不用主题目的地过滤了。这个规则不知在前端那里添加好，就添加在这里了。。。KevinCai
                {
                    foreach (int i in p.InterestPlace)
                    {
                        if (i > 0)
                            filterCol.Add(HotelListFilterPrefix.InterestPlace + i);
                    }
                }
            }

            if (p.Featured != null && p.Featured.Length > 0)
            {
                foreach (int i in p.Featured)
                {
                    if (i > 0)
                        filterCol.Add(HotelListFilterPrefix.Featured + i);
                }
            }

            if (p.ClassID != null && p.ClassID.Length > 0)
                foreach (int i in p.ClassID)
                {
                    if (i > 0)
                        filterCol.Add(HotelListFilterPrefix.Class + i);
                }

            if (p.Class2ID != null && p.Class2ID.Length > 0)
                foreach (int i in p.Class2ID)
                {
                    if (i > 0)
                        filterCol.Add(HotelListFilterPrefix.Class2 + i);
                }

            if (p.TagIDs != null && p.TagIDs.Length > 0)
                foreach (int i in p.TagIDs)
                {
                    if (i > 0)
                        filterCol.Add(HotelListFilterPrefix.Tags + i);
                }

            if (p.Location != null && p.Location.Length > 0)
                foreach (int i in p.Location)
                {
                    if (i > 0)
                        filterCol.Add(HotelListFilterPrefix.Location + i);
                }

            if (p.Zone != null && p.Zone.Length > 0)
                foreach (int i in p.Zone)
                {
                    if (i > 0)
                        filterCol.Add(HotelListFilterPrefix.Zone + i);
                }

            if (p.Brands != null && p.Brands.Length > 0)
                foreach (int i in p.Brands)
                {
                    if (i > 0)
                        filterCol.Add(HotelListFilterPrefix.Brand + i);
                }

            if (p.Facilitys != null && p.Facilitys.Length > 0)
                foreach (int i in p.Facilitys)
                {
                    if (i > 0)
                        filterCol.Add(HotelListFilterPrefix.Facility + i);
                }

            if (p.Star != null && p.Star.Length > 0)
                foreach (int i in p.Star)
                {
                    if (i > 0)
                        filterCol.Add(HotelListFilterPrefix.Star + i);
                }

            if (p.MinPrice <= 0 && p.MaxPrice == decimal.MaxValue)
            {
                p.MinPrice = 0;
                p.MaxPrice = 0;
            }

            if (p.NearbyGroupID > 0)
                filterCol.Add(HotelListFilterPrefix.NearbyGroup + p.NearbyGroupID);

            if (p.NearbyPOIID > 0)
                filterCol.Add(HotelListFilterPrefix.NearBy + p.NearbyPOIID);

            switch (p.Valued)
            {
                case 1: p.SortType = (int)HotelListSortType.ValuedFirst; break;
                case 2: filterCol.Add(HotelListFilterPrefix.Valued); break;
            }

            if (p.ChannelHotel > 0)
                filterCol.Add(HotelListFilterPrefix.ChannelHotel);

            if (p.Attraction > 0)
            {
                filterCol.Add(HotelListFilterPrefix.Attraction + p.Attraction);
                if (p.SortType == 2)
                    p.NearbyPOIID = p.Attraction;//为景区附近排序所写。 如果有景区,并按离景区距离来排序的话，那么将NearbyPOIID设置为AttractionID.
            }

            if (p.ZonePlaces != null && p.ZonePlaces.Length > 0)
            {
                foreach (int i in p.ZonePlaces)
                {
                    if (i > 0)
                        filterCol.Add(HotelListFilterPrefix.ZonePlace + i);
                }
            }
            //else if(p.ZonePlaceID > 0)
            //{
            //    filterCol.Add(HotelListFilterPrefix.ZonePlace + p.ZonePlaceID);//限制在该区域
            //}

            if (p.HotelFacilitys != null && p.HotelFacilitys.Length > 0)
                foreach (int i in p.HotelFacilitys)
                {
                    if (i > 0)
                        filterCol.Add(HotelListFilterPrefix.HotelFacility + i);
                }

            if (p.InterestArray != null && p.InterestArray.Length > 0)
            {
                foreach (int i in p.InterestArray)
                {
                    if (i > 0)
                        filterCol.Add(HotelListFilterPrefix.InterestArray + i);
                }
            }

            //出游类型数组
            if (p.TripTypeArray != null && p.TripTypeArray.Length > 0)
            {
                foreach (int i in p.TripTypeArray)
                {
                    if (i > 0)
                        filterCol.Add(HotelListFilterPrefix.TripType + i);
                }
            }

            if (p.FeaturedTreeArray != null && p.FeaturedTreeArray.Length > 0)
            {
                foreach (int i in p.FeaturedTreeArray)
                {
                    if (i > 0)
                        filterCol.Add(HotelListFilterPrefix.FeaturedTree + i);
                }
            }

            return filterCol;

        }

        public QueryHotelResult QueryHotel(HotelSearchParas p, int PriceWithOTAID = 0)
        {
            QueryHotelResult qhr = new QueryHotelResult();
            long firstFilter = 0;
            RankType rt = 0;

            List<long> filterCol = GenQueryHotelParams(p, ref  rt, ref firstFilter);


            DataSet ds = HotelDAL.QueryHotel(firstFilter, filterCol, (int)p.MinPrice, (int)p.MaxPrice, p.CheckInDate, p.CheckOutDate, p.StartIndex, p.ReturnCount,
                p.SortType, p.SortDirection, p.NearbyPOIID, p.InitHotelList, PriceWithOTAID, p.Lat, p.Lng,
                p.Distance, p.nLat, p.nLng, p.sLat, p.sLng, p.n1Lat, p.n1Lng, p.s1Lat, p.s1Lng,
                p.NeedFilterCol, p.NeedHotelID);

            qhr.TotalCount = (int)ds.Tables[0].Rows[0][0];
            if (p.NeedFilterCol)
            {
                foreach (DataRow r in ds.Tables[1].Rows)
                {
                    qhr.FilterCount.Add((long)r[0], (int)r[1]);
                }
            }

            if (p.NeedHotelID)
            {
                List<int> hidList = ds.Tables[2].Rows.Cast<DataRow>().Select(r => (int)r[0]).ToList<int>();
                qhr.HotelList = GetSimpleHotels(hidList);

                //HJD.HotelServices.Implement.Business.LogHelper.WriteLog("--------获取酒店列表的数量--------(1)" + hidList.Count + ";(2)" + qhr.HotelList.Count + " " + string.Join(",", hidList));

                //排名信息
                List<SimpleHotelRankEntity> hr = GetHotelRanks(hidList, rt == RankType.attraction ? p.Attraction : p.HotelTheme, p.Type, rt);

                foreach (SimpleHotelItem h in qhr.HotelList)
                {
                    SimpleHotelRankEntity tsh = hr.Where(o => o.HotelID == h.Id).FirstOrDefault();
                    if (tsh != null)
                        h.Rank = tsh.Rank;
                }

                if (firstFilter == 10000000000 + 35447)
                {
                    LogHotelQueryParams(p, PriceWithOTAID, firstFilter, filterCol, hidList);
                }
            }
            return qhr;
        }

        public QueryHotelResult QueryHotelForMagiCall(HotelSearchParas p, int PriceWithOTAID = 0)
        {
            QueryHotelResult qhr = new QueryHotelResult();
            long firstFilter = 0;
            RankType rt = 0;

            List<long> filterCol = GenQueryHotelParams(p, ref  rt, ref firstFilter);


            DataSet ds = HotelDAL.QueryHotelForMagiCall(firstFilter, filterCol, (int)p.MinPrice, (int)p.MaxPrice, p.CheckInDate, p.CheckOutDate, p.StartIndex, p.ReturnCount,
                p.SortType, p.SortDirection, p.NearbyPOIID, p.InitHotelList, PriceWithOTAID, p.Lat, p.Lng,
                p.Distance, p.nLat, p.nLng, p.sLat, p.sLng, p.n1Lat, p.n1Lng, p.s1Lat, p.s1Lng,
                p.NeedFilterCol, p.NeedHotelID);

            qhr.TotalCount = (int)ds.Tables[0].Rows[0][0];
            if (p.NeedFilterCol)
            {
                foreach (DataRow r in ds.Tables[1].Rows)
                {
                    qhr.FilterCount.Add((long)r[0], (int)r[1]);
                }
            }

            if (p.NeedHotelID)
            {
                List<int> hidList = ds.Tables[2].Rows.Cast<DataRow>().Select(r => (int)r[0]).ToList<int>();
                qhr.HotelList = GetSimpleHotels(hidList);

                //HJD.HotelServices.Implement.Business.LogHelper.WriteLog("--------获取酒店列表的数量--------(1)" + hidList.Count + ";(2)" + qhr.HotelList.Count + " " + string.Join(",", hidList));

                //排名信息
                List<SimpleHotelRankEntity> hr = GetHotelRanks(hidList, rt == RankType.attraction ? p.Attraction : p.HotelTheme, p.Type, rt);

                foreach (SimpleHotelItem h in qhr.HotelList)
                {
                    SimpleHotelRankEntity tsh = hr.Where(o => o.HotelID == h.Id).FirstOrDefault();
                    if (tsh != null)
                        h.Rank = tsh.Rank;
                }

                if (firstFilter == 10000000000 + 35447)
                {
                    LogHotelQueryParams(p, PriceWithOTAID, firstFilter, filterCol, hidList);
                }
            }
            return qhr;
        }

        private static void LogHotelQueryParams(HotelSearchParas p, int PriceWithOTAID, long firstFilter, List<long> filterCol, List<int> hidList)
        {
            LogHelper.WriteLog(string.Format(@"
              exec  sp_Hotel_Query_test	@FirstFilter='{0}', --首要过滤条件。为目的地或频道ID
	@Filter ='{1}',
	@minPrice ='{2}',
	@maxPrice ='{3}', -- 0:表示无需价格过滤
	@checkIn  ='{4}',
	@checkOut  ='{5}',
	@start  ='{6}', -- 从0开始
	@count  ='{7}',
	@orderBy  ='{8}', -- Rank = 0, AllAttractionRank=5,AllInterestRank=6, Price = 1, Distance = 2, NewReview = 3, ValuedFirst = 4 ,packageFirstThenDistance=10
				  --
	@orderDirect  ='{9}', --1 : OrderBy.Desc   0: OrderBy.Asc
	@nearByPOI  ='{10}', -- POI点  用于按POI点由近到远排序， orderBy = 3时需提供
	@InitHotelList  ='{11}', --初始酒店集 ，如此刻附近酒店集
	                             --@needTotalCount BIT = 1, --是否需要返回总数
	@needFilterCol  ='{16}', --是否需要返回各属性值
	@needHotelID  ='{17}' , --是否需要返回酒店列表
	@OTAID  ='{12}' , --OTAID 价格按那家的排序  0：不按那家出， >0:按OTA给出的进行价格排序
	@lat  ='{13}',
	@lng ='{14}',
	@distance  ='{15}' 
                
             AroundCityID:{18}: hotelList:{19}",
                firstFilter, string.Join(",", filterCol), (int)p.MinPrice, (int)p.MaxPrice, p.CheckInDate, p.CheckOutDate, p.StartIndex, p.ReturnCount, p.SortType, p.SortDirection, p.NearbyPOIID, p.InitHotelList, PriceWithOTAID, p.Lat, p.Lng, p.Distance, p.NeedFilterCol, p.NeedHotelID, p.AroundCityID, string.Join(",", hidList)));

        }

        public MenuEntity InitHotelMenu(int districtID)
        {
            HotelSearchParas p = new HotelSearchParas();
            p.DistrictID = districtID;
            p.NeedFilterCol = true;
            p.ReturnCount = 0;
            p.CheckInDate = DateTime.Now;
            p.CheckOutDate = DateTime.Now.AddDays(1);
            var result = QueryHotel(p);
            if (result == null)
                return null;
            var menu = ParseHotelMenu(result.FilterCount, false, districtID);
            menu.Classes = GetHotelClassByDistrict(districtID);
            return menu;
        }

        public MenuEntity ParseHotelMenu(Dictionary<long, int> filterCount, bool channel = false, int districtID = 0)
        {
            List<FilterDicEntity> lfd = GetAllHotelFilterDic();

            var query = from l in lfd
                        join f in filterCount on l.Key equals f.Key
                        select new FilterDicEntity
                        {
                            ID = l.ID,
                            Name = l.Name,
                            Type = l.Type,
                            Num = f.Value
                        };

            MenuEntity me = new MenuEntity();
            if (channel)
            {
                me.DistrictIDs = (from f in filterCount
                                  where (int)(f.Key / HotelListFilterPrefix.BaseOffset) == (int)(HotelListFilterPrefix.DistrictID / HotelListFilterPrefix.BaseOffset)
                                  orderby f.Value descending
                                  select new FilterDicEntity
                                  {
                                      ID = (int)(f.Key % HotelListFilterPrefix.DistrictID),
                                      Num = f.Value,
                                      Key = f.Key,
                                      Type = (int)(HotelListFilterPrefix.DistrictID / HotelListFilterPrefix.BaseOffset)
                                  }).ToList();
            }
            else
            {
                me.Brands = (from b in query
                             where b.Type == (int)(HotelListFilterPrefix.Brand / HotelListFilterPrefix.BaseOffset)
                             orderby b.Num descending
                             select b).ToList();

                me.Zones = (from b in query
                            where b.Type == (int)(HotelListFilterPrefix.Zone / HotelListFilterPrefix.BaseOffset)
                            orderby b.Num descending
                            select b).ToList();

                me.Locations = (from b in query
                                where b.Type == (int)(HotelListFilterPrefix.Location / HotelListFilterPrefix.BaseOffset)
                                orderby b.Num descending
                                select b).ToList();

                me.Stars = (from b in query
                            where b.Type == (int)(HotelListFilterPrefix.Star / HotelListFilterPrefix.BaseOffset)
                            orderby b.ID descending
                            select b).ToList();

                me.Facilitys = (from b in query
                                where b.Type == (int)(HotelListFilterPrefix.Facility / HotelListFilterPrefix.BaseOffset)
                                orderby b.Num descending
                                select b).ToList();

                me.Classes = (from b in query
                              where b.Type == (int)(HotelListFilterPrefix.Class / HotelListFilterPrefix.BaseOffset)
                              select b).ToList();
                //me.Classes = GetHotelClassByDistrict(districtID);

                if (districtID > 0)
                {
                    me.FilterPrice = GetZhongdangStartPrice(districtID).Values.ToList();
                }
            }
            me.Tags = (from b in query
                       where b.Type == (int)(HotelListFilterPrefix.Tags / HotelListFilterPrefix.BaseOffset)
                       orderby b.Num descending
                       select b).ToList();
            return me;
        }

        /// <summary>
        /// 查询酒店的分类过滤数据
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public List<FilterDicEntity> GetHotelFilters(HotelSearchParas p)
        {
            QueryHotelResult qResult = QueryHotel(p);

            List<FilterDicEntity> lfd = GetAllHotelFilterDic();

            return (from f in qResult.FilterCount
                    join l in lfd on f.Key equals l.Key into temp
                    from tt in temp.DefaultIfEmpty()
                    select new FilterDicEntity
                    {
                        Key = f.Key,
                        ID = tt == null ? 0 : tt.ID,
                        Name = tt == null ? "" : tt.Name,
                        Type = tt == null ? 0 : tt.Type,
                        Num = f.Value
                    }).ToList();

        }

        public List<FilterDicEntity> GetQueryHotelFilters(HotelSearchParas p)
        {
            return memCacheHotel.GetData<List<FilterDicEntity>>(string.Format("{0}.{1}", p.Attraction, p.Type), QueryHotelMenuListCacheKey, () =>
            {
                HotelSearchParas hp = new HotelSearchParas();
                hp.Attraction = p.Attraction;
                hp.ClassID = p.ClassID;
                hp.NeedFilterCol = true;
                hp.NeedHotelID = false;
                hp.CheckInDate = hp.CheckOutDate = DateTime.Now;
                QueryHotelResult qResult = QueryHotel(hp);

                List<FilterDicEntity> lfd = GetAllHotelFilterDic();

                return (from f in qResult.FilterCount
                        join l in lfd on f.Key equals l.Key into temp
                        from tt in temp.DefaultIfEmpty()
                        select new FilterDicEntity
                        {
                            Key = f.Key,
                            ID = tt == null ? 0 : tt.ID,
                            Name = tt == null ? "" : tt.Name,
                            Type = tt == null ? 0 : tt.Type,
                            Num = f.Value
                        }).ToList();
            });

        }

        public WapMenuEntity QueryHotelWapMenu(HotelSearchParas p, int needTagLength)
        {

            List<FilterDicEntity> lts = memCacheHotel.GetData<List<FilterDicEntity>>(string.Format("{0}.{1}", p.Attraction, p.ClassID), QueryHotelTagListCacheKey, () =>
            {

                HotelSearchParas hp = new HotelSearchParas();
                hp.Attraction = p.Attraction;
                hp.NeedFilterCol = true;
                hp.NeedHotelID = false;
                QueryHotelResult qResult = QueryHotel(hp);


                List<FilterDicEntity> lfd = GetAllHotelFilterDic();

                var query = from f in qResult.FilterCount
                            join l in lfd on f.Key equals l.Key
                            select new FilterDicEntity
                            {
                                ID = l.ID,
                                Name = l.Name,
                                Type = l.Type,
                                Num = f.Value
                            };

                return (from b in query
                        where b.Type == (int)(HotelListFilterPrefix.Tags / HotelListFilterPrefix.BaseOffset)
                        orderby b.Num descending
                        select b).Take(needTagLength).ToList();
            });


            WapMenuEntity wapMenu = memCacheHotel.GetData<WapMenuEntity>(string.Format("{0}", p.Attraction), QueryHotelMenuListCacheKey, () =>
            {
                HotelSearchParas hp = new HotelSearchParas();
                hp.Attraction = p.Attraction;
                hp.NeedFilterCol = true;
                hp.NeedHotelID = false;
                QueryHotelResult qResult = QueryHotel(hp);

                List<FilterDicEntity> lfd = GetAllHotelFilterDic();

                var query = from f in qResult.FilterCount
                            join l in lfd on f.Key equals l.Key
                            select new FilterDicEntity
                            {
                                Key = f.Key,
                                ID = l.ID,
                                Name = l.Name,
                                Type = l.Type,
                                Num = f.Value
                            };

                WapMenuEntity wme = new WapMenuEntity();

                //wme.Brands = (from b in query
                //             where b.Type == (int)(HotelListFilterPrefix.Brand / HotelListFilterPrefix.BaseOffset)
                //             orderby b.Num descending 
                //             select b).ToList();

                //wme.Zones = (from b in query
                //             where b.Type == (int)(HotelListFilterPrefix.Zone / HotelListFilterPrefix.BaseOffset)
                //             orderby b.Num descending
                //             select b).ToList();

                //wme.Locations = (from b in query
                //             where b.Type == (int)(HotelListFilterPrefix.Location / HotelListFilterPrefix.BaseOffset)
                //             orderby b.Num descending
                //             select b).ToList();

                if (qResult.FilterCount.ContainsKey(HotelListFilterPrefix.DistrictID + p.DistrictID))
                {
                    wme.TotalNum = qResult.FilterCount[HotelListFilterPrefix.DistrictID + p.DistrictID];
                }

                if (qResult.FilterCount.ContainsKey(HotelListFilterPrefix.Valued))
                {
                    wme.ValuedNum = qResult.FilterCount[HotelListFilterPrefix.Valued];
                }

                if (qResult.FilterCount.ContainsKey(HotelListFilterPrefix.Class + (int)HotelClassType.Resort))
                {
                    wme.ResortNum = qResult.FilterCount[HotelListFilterPrefix.Class + (int)HotelClassType.Resort];
                }

                if (qResult.FilterCount.ContainsKey(HotelListFilterPrefix.Class + (int)HotelClassType.Boutique))
                {
                    wme.BoutiqueNum = qResult.FilterCount[HotelListFilterPrefix.Class + (int)HotelClassType.Boutique];
                }

                wme.filterPrice = GetZhongdangStartPrice(p.DistrictID);

                wme.PriceSectionMinPrice = GetZhongdangPriceSection(p.DistrictID)[0];
                wme.PriceSectionMaxPrice = GetZhongdangPriceSection(p.DistrictID)[1];
                return wme;
            });



            wapMenu.Tags = lts;

            return wapMenu;
        }

        public List<int> GetZhongdangPriceSection(int DistrictID)
        {

            return memCacheHotel.GetData<List<int>>(string.Format("PriceSection:{0}_{1}", CacheHotelVer, DistrictID), () =>
            {
                ZhongDangPriceSectionEntity zdp = HotelDAL.GetZhongDangPriceSection(DistrictID);

                List<int> ps = new List<int>();
                if (zdp == null)
                {
                    ps.Add(200);
                    ps.Add(1000);
                }
                else if (zdp.MinPrice > 0)
                {
                    ps.Add((int)zdp.MinPrice);
                    ps.Add((int)zdp.MaxPrice);

                }
                else
                {
                    ps.Add(zdp.InChina ? 200 : 400);
                    ps.Add(zdp.InChina ? 1000 : 1400);
                }
                return ps;
            });
        }

        public Dictionary<int, string> GetZhongdangStartPrice(int DistrictID)
        {
            List<int> ps = GetZhongdangPriceSection(DistrictID);
            return GenPriceSection(ps[0], ps[1]);
        }

        public Dictionary<int, string> GenPriceSection(int minp, int maxp)
        {
            Dictionary<int, string> dp = new Dictionary<int, string>();
            List<int> priceS = new List<int> { 300, 500, 700, 1000, 1400 };

            int section = 0;

            foreach (int ps in priceS)
            {
                if (minp > ps) continue; //找到起始点
                if (ps - minp < 100) continue; //价差大于100
                section++;
                if (maxp > ps)
                {
                    if (maxp - ps < 100)
                    {
                        dp.Add(section, string.Format("{0}|{1}", minp, maxp));
                        break;
                    }
                    else
                    {
                        dp.Add(section, string.Format("{0}|{1}", minp, ps == 1400 ? maxp : ps //如果中档上限超过1400，则取中档上限
                               ));
                        minp = ps;
                    }
                }
                else
                {
                    dp.Add(section, string.Format("{0}|{1}", minp, maxp));
                    break;

                }

            }


            return dp;
        }

        public List<FilterDicEntity> GetAllHotelFilterDic()
        {
            return memCacheHotel.GetData<List<FilterDicEntity>>("AllHotelFilterDic1" + CacheHotelVer, () =>
            {
                return HotelDAL.GetAllHotelFilterDic();
            });
        }

        public List<HotelReviewEntity> GetReviewStatus(int hotelID, long userID)
        {
            return HotelDAL.GetReviewStatus(hotelID, userID);
        }

        public List<ListHotelItem2> GetCollectHotelList(List<int> hotelIdList)
        {
            List<ListHotelItem2> list = null;
            if (hotelIdList != null && hotelIdList.Count != 0)
            {

                List<SimpleHotelItem> simpleHotelItems = GetSimpleHotels(hotelIdList);
                list = TransToListHotelItem(simpleHotelItems, 0);
                return list;
            }
            else
            {
                return null;
            }
        }
        //public List<PackageEntity> GetPackegeListByHotelIDs(List<int> hotelIDs)
        //{
        //    List<PackageEntity> list = new List<PackageEntity>();
        //    if (hotelIDs != null && hotelIDs.Count != 0)
        //    {
        //        List<PackageEntity> PackageEntityList =HotelDAL.GetPackegeListByHotelIDs(hotelIDs);
        //        return list;
        //    }
        //    return list;
        //}

        public int UpdateInspectorRefHotelState()
        {
            var parameters = new DBParameterCollection();
            return HotelDAL.UpdateInspectorRefHotelState();
        }

        public List<InspectorHotel> GetInspectorHotelByHotelId(int hotelId)
        {
            List<InspectorHotel> list = HotelDAL.GetInspectorHotelByHotelId(hotelId);
            return list != null ? list : new List<InspectorHotel>();
        }

        public int InsertOrUpdatePoints(PointsEntity point)
        {
            if (point != null && point.TotalPoint == 0 && point.ID == 0 && (point.TypeID > 0 || !string.IsNullOrEmpty(point.TypeCode)))
            {
                List<PointsTypeDefineEntity> ptdeList = GetPointsTypeDefineList(point.TypeCode, point.TypeID);
                point.TotalPoint = point.LeavePoint = ptdeList.Count > 0 ? ptdeList[0].Amount : 0;
            }
            return HotelDAL.InsertOrUpdatePoints(point);
        }

        public List<PointsTypeDefineEntity> GetPointsTypeDefineList(string code, int type)
        {
            return HotelDAL.GetPointsTypeDefineList(code, type);
        }

        public int DeletePoints(int id)
        {
            return HotelDAL.DeletePoints(id);
        }

        public int InsertOrUpdatePointsConsume(PointsConsumeEntity point)
        {
            return HotelDAL.InsertOrUpdatePointsConsume(point);
        }

        public List<PointsEntity> GetPointsEntity(long userId)
        {
            return HotelDAL.GetPointsEntity(userId);
        }
        public List<PointsEntity> GetExpirePointsEntity(DateTime startTime, DateTime endTime, string userids = "")
        {
            return HotelDAL.GetExpirePointsEntity(startTime, endTime, userids);
        }
        public List<PointsEntity> GetPointsEntityByToDayAndTypeId(int typeId)
        {
            return HotelDAL.GetPointsEntityByToDayAndTypeId(typeId);
        }
        public List<ExpirePointsEntity> GetExpirePoints(DateTime startTime, DateTime endTime)
        {
            return HotelDAL.GetExpirePoints(startTime, endTime);
        }

        public List<PointsConsumeEntity> GetPointsConsumeEntity(long userId)
        {
            return HotelDAL.GetPointsConsumeEntity(userId);
        }

        public List<SourceIDAndObjectNameEntity> GetObjectNamesByTypeCode(List<long> ids, string typeCode, int typeID)
        {
            List<SourceIDAndObjectNameEntity> list = new List<SourceIDAndObjectNameEntity>();//ToDo
            if (typeID == 100)
            {
                return list;
            }
            switch (typeCode)
            {
                case "zeta":
                    return list;
                case "regist":
                    return list;
                case "zmjd50":
                    return list;
                case "sharecomment":
                    return HotelDAL.GetObjectNamesByCommentIDs(ids);
                case "writecomment":
                    return HotelDAL.GetObjectNamesByCommentIDs(ids);
                case "toprecommend":
                    return HotelDAL.GetObjectNamesByCommentIDs(ids);
                case "freeinspect":
                    return HotelDAL.GetObjectNamesByInspectorRefHotelIDs(ids);
                case "cancelexchange":
                    return HotelDAL.GetObjectNamesByInspectorRefHotelIDs(ids);
                case "orderpay":
                    return list;
                default:
                    return list;
            }
        }

        public int GetAvailablePointByUserID(long userID, int typeID)
        {
            List<PointsEntity> list = HotelDAL.GetPointsEntity(userID).FindAll(i => typeID == 0 || i.TypeID == typeID);
            if (list != null && list.Count != 0)
            {
                return list.Sum(i => i.LeavePoint);
            }
            return 0;
        }

        public PointsEntity GetPointsByIDOrTypeIDAndBusinessID(int id, int typeID, int businessID)
        {
            return HotelDAL.GetPointsByIDOrTypeIDAndBusinessID(id, typeID, businessID);
        }

        public List<PointsEntity> GetPointslistNumByUserIdAndTypeId(long userId, int typeId)
        {
            return HotelDAL.GetPointslistNumByUserIdAndTypeId(userId, typeId);
        }

        public int UpdateInspectorStateByPointsNum()
        {
            return HotelDAL.UpdateInspectorStateByPointsNum();
        }

        public List<ListHotelItem3> GetListHotelItem3List(List<int> hotelIDs)
        {
            List<ListHotelItem3> list = null;
            if (hotelIDs != null && hotelIDs.Count != 0)
            {
                List<SimpleHotelItem> simpleHotelItems = GetSimpleHotels(hotelIDs);
                list = TransToListHotelItem3(simpleHotelItems, 0);
                return list;
            }
            else
            {
                return new List<ListHotelItem3>();
            }
        }

        public List<ListHotelItem3> GetUserRecommendHotelList(int hotelID, long userID, int interestID, int maxCount = 3, int distance = 300000)
        {
            return memCacheHotel.GetData<List<ListHotelItem3>>(string.Format("{0}:{1}:{2}:{3}:{4}", hotelID, userID, interestID, maxCount, distance), UserRecommendHotelListCacheKey, () =>
            {
                return GenUserRecommendHotelList(hotelID, userID, interestID, maxCount, distance);
            });
        }

        public List<ListHotelItem3> GenUserRecommendHotelList(int hotelID, long userID, int interestID, int maxCount = 3, int distance = 300000)
        {
            List<int> hotelIDs = HotelDAL.Query_InterestHotel(hotelID, interestID, userID, distance);
            List<ListHotelItem3> list = new List<ListHotelItem3>();
            if (hotelIDs.Count != 0)
            {
                //只提取maxCount个酒店信息
                List<SimpleHotelItem> simpleHotelItems = GetSimpleHotels(hotelIDs.Where(id => id != hotelID).Take(maxCount).ToList());
                list = TransToListHotelItem3(simpleHotelItems, 0);//取出的内容和interestID无关 因此传0
            }
            return list;
        }

        #region Top 20 套餐排序
        public List<TopNPackagesEntity> GetTopNPackagesEntityList(TopNPackageSearchParam param, out int count)
        {
            return HotelDAL.GetTopNPackagesEntityList(param, out count);
        }
        public List<TopNPackagesEntity> GetTopNPackagesAddSearchEntityList(TopNPackageSearchParam param, out int count)
        {
            return HotelDAL.GetTopNPackagesAddSearchEntityList(param, out count);
        }
        
        public int UpdateTopNPackage(TopNPackagesEntity tnpe)
        {
            if (tnpe.PID > 0)
            {
                //根据套餐ID 获得酒店ID 极其对应的intro信息
                PackageEntity pe = HotelDAL.GetPackage(0, tnpe.PID).FirstOrDefault();
                tnpe.RecomemndWord = string.IsNullOrWhiteSpace(tnpe.RecomemndWord) ? GenHotelIntroStr(pe.HotelID) : tnpe.RecomemndWord.Trim();
                return HotelDAL.InsertOrUpdateTopNPackage(tnpe);
            }
            else if (tnpe.HotelID > 0)
            {
                tnpe.RecomemndWord = string.IsNullOrWhiteSpace(tnpe.RecomemndWord) ? GenHotelIntroStr(tnpe.HotelID) : tnpe.RecomemndWord.Trim();
                return HotelDAL.InsertOrUpdateTopNPackage(tnpe);
            }
            else
            {
                return 1;
            }
        }

        public string GenHotelIntroStr(int hotelID, int type = 1)
        {
            Hotel3Entity hotelIntro = HotelDAL.GetHotel3(hotelID).FirstOrDefault(_ => _.Type == type);//只要类型为1的Hotel3
            string introStr = "";
            if (hotelIntro != null && !string.IsNullOrWhiteSpace(hotelIntro.items))
            {
                StringReader reader = new StringReader(hotelIntro.items);
                DataSet set = new DataSet();
                set.ReadXml(reader);
                if (set.Tables.Count > 0)
                {
                    foreach (DataRow dr in set.Tables[0].Rows)
                    {
                        introStr += dr["content"].ToString();//+ (dr["Image"].ToString() == "" ? "" : "IMGURL:" + dr["Image"].ToString())
                    }
                }
            }
            return introStr;
        }

        public List<TopNPackageItem> GetPackageItemList(List<int> pids)
        {
            if (pids == null || pids.Count == 0)
            {
                return new List<TopNPackageItem>();
            }
            else
            {
                //List<TypeAndPrice> priceList = HotelDAL.GetPackageTypeAndPriceList(pids);
                List<TopNPackageItem> list = HotelDAL.GetTopNPackageContent(pids);
                if (list == null || list.Count == 0)
                {
                    return new List<TopNPackageItem>();
                }

                var currentDate = DateTime.Now.Date;
                //var priceCalendar = HotelDAL.GetHotelPackageFirstAvailablePriceByPIds(pids, currentDate, null);

                var priceCalendar = HotelDAL.GetHotelPackageMinPriceByPIds(pids, currentDate, null) ?? new List<HotelTop1PackageInfoEntity>();

                foreach (var temp in list)
                {
                    var hotelId = temp.HotelID;
                    var pId = temp.PackageID;

                    var price = priceCalendar.FirstOrDefault(_ => _.PID == pId);
                    if (price != null)
                    {
                        temp.PackagePrice = new List<TypeAndPrice>(){
                            new TypeAndPrice(){
                                PID = pId,
                                AddInfo = "",
                                Price = price.Price,
                                Type = 0,
                                TypeName = ""
                            },
                            new TypeAndPrice(){
                                PID = pId,
                                AddInfo = "",
                                Price = price.VIPPrice,
                                Type = -1,
                                TypeName = ""
                            }
                        };
                    }
                    else
                    {
                        temp.PackagePrice = new List<TypeAndPrice>();
                    }

                    #region old method for get daily and weekend price of a package
                    //var packageCalendar = GenHotelPackageCalendar(hotelId, currentDate, pId, 90, HotelServiceEnums.PricePolicyType.VIP).FindAll(_ => _.SellState == 1);//找出该范围内所有可用的日历价格

                    //var normalPriceList = packageCalendar.FindAll(_ => _.NormalPrice > 0);
                    //var sellPriceList = packageCalendar.FindAll(_ => _.SellPrice > 0);

                    //temp.PackagePrice = new List<TypeAndPrice>(){
                    //    new TypeAndPrice(){
                    //        PID = pId,
                    //        AddInfo = "",
                    //        Price = normalPriceList.Any() ? normalPriceList.Min(_=>_.NormalPrice) : 0,
                    //        Type = 0,
                    //        TypeName = ""
                    //    },
                    //    new TypeAndPrice(){
                    //        PID = pId,
                    //        AddInfo = "",
                    //        Price = sellPriceList.Any() ? sellPriceList.Min(_=>_.SellPrice) : 0,
                    //        Type = -1,
                    //        TypeName = ""
                    //    }
                    //};
                    #endregion
                }
                return list;
            }
        }

        public List<TopNPackageItem> GetPackageItemList2(List<int> pids, DateTime? currentDate, bool isOnlyOnlinePackage = true)
        {
            if (pids == null || pids.Count == 0)
            {
                return new List<TopNPackageItem>();
            }
            else
            {
                //List<TypeAndPrice> priceList = HotelDAL.GetPackageTypeAndPriceList(pids);
                List<TopNPackageItem> list = isOnlyOnlinePackage ? HotelDAL.GetTopNPackageContent(pids) : HotelDAL.GetPackageContentNofilterPackageState(pids);
                if (list == null || list.Count == 0)
                {
                    return new List<TopNPackageItem>();
                }

                //var currentDate = DateTime.Now.Date;
                //var priceCalendar = HotelDAL.GetHotelPackageFirstAvailablePriceByPIds(pids, currentDate, null) ?? new List<HotelTop1PackageInfoEntity>();
                var priceCalendar = HotelDAL.GetHotelPackageMinPriceByPIds(pids, currentDate, null) ?? new List<HotelTop1PackageInfoEntity>();

                foreach (var temp in list)
                {
                    var hotelId = temp.HotelID;
                    var pId = temp.PackageID;

                    var price = priceCalendar.FirstOrDefault(_ => _.PID == pId);
                    if (price != null)
                    {
                        temp.PackagePrice = new List<TypeAndPrice>(){
                            new TypeAndPrice(){
                                PID = pId,
                                AddInfo = "",
                                Price = price.Price,
                                Type = 0,
                                TypeName = "",
                                NowPriceDay = Convert.ToDateTime(price.DT)
                            },
                            new TypeAndPrice(){
                                PID = pId,
                                AddInfo = "",
                                Price = price.VIPPrice,
                                Type = -1,
                                TypeName = "",
                                NowPriceDay = Convert.ToDateTime(price.DT)
                            }
                        };
                    }
                    else
                    {
                        temp.PackagePrice = new List<TypeAndPrice>();
                    }
                }
                return list;
            }
        }

        

        public List<AlbumPackageSimpleEntity> GetTopNPackageScreenList(int albumsID)
        {
            return HotelDAL.GetTopNPackageScreenList(albumsID);
        }
        public List<TopNPackageItem> GetTopNPackageList(bool isValid, int start, int count, int albumsId = 0, bool isShowVipFirstBuypackage = true, bool isNeedNotSale = false, string dateStr = "", int gotoDistrictID = 0, int startDistrictID = 0)
        {
            var albumsEntity = albumsId > 0 ? HotelDAL.GetOnePackageAlbums(albumsId) : new PackageAlbumsEntity();

            TopNPackageSearchParam param = new TopNPackageSearchParam()
            {
                Filter = new TopNPackageSearchFilter()
                {
                    IsValid = true,
                    NeedRankRange = 0,
                    AlbumsID = albumsId,
                    NeedNotSale = isNeedNotSale,
                    VipFirstBuyPackage = isShowVipFirstBuypackage ? 1 : 0,
                    DateStr = dateStr,
                    GoToDistrictId = gotoDistrictID,
                    StartDistrictId = startDistrictID
                },
                Start = start,
                PageSize = count,
                Sort = new Dictionary<string, string>()
            };
            int totalCount = 0;
            //2酒店专辑 和 1套餐专辑有区别
            List<TopNPackagesEntity> list = albumsEntity.Type == 2 ? HotelDAL.GetTopNPackagesEntityList4HotelAlbums(param, out totalCount) : HotelDAL.GetTopNPackagesEntityList(param, out totalCount);

            List<int> pids = list.FindAll(_ => _.PID > 0).Select(_ => _.PID).ToList();
            var currentDate = DateTime.Now.Date;
            if (albumsId == 13)
            {
                currentDate = DateTime.Parse("2016-10-01");
            }
            var packageList = pids.Any() ? GetPackageItemList2(pids, null) : new List<TopNPackageItem>();
            var packageList20 = new List<TopNPackageItem>();
            foreach (var temp in list)
            {
                var item = packageList.Any() ? packageList.Find(_ => temp.PID == _.PackageID) : new TopNPackageItem();
                item.RecomemndWord = temp.RecomemndWord;
                item.RecomemndWord2 = temp.RecomemndWord2;
                item.RecomendPicShortNames = temp.RecomendPicShortNames;
                item.RecomendPicShortNames2 = temp.RecomendPicShortNames2;
                item.Title = temp.Title;
                item.MarketPrice = temp.MarketPrice;
                item.CoverPicSUrl = temp.CoverPicSUrl;
                item.AlbumsID = temp.AlbumsID;
                item.AlbumsName = temp.AlbumsName;
                item.PackageName = temp.PackageName;
                item.ForVIPFirstBuy = temp.ForVIPFirstBuy;
                item.VIPFirstPayDiscount = temp.VIPFirstPayDiscount;
                item.DateSelectType = temp.DateSelectType;

                item.DayLimitMin = 1;
                try
                {
                    //根据酒店和套餐获取其最少预订天数
                    var packagePrice = new List<PackageInfoEntity>() { new HotelService().GetHotelPackageByCode(item.HotelID, "", temp.PID) };
                    if (packagePrice != null)
                    {
                        var _min = packagePrice.Min(p => p.packageBase.DayLimitMin);
                        if (_min > 0) item.DayLimitMin = _min;
                    }
                }
                catch (Exception ex) { }

                var hotelItem = GetHotel(item.HotelID);//内部memcached缓存
                item.HotelScore = hotelItem.Score;
                item.HotelReviewCount = hotelItem.ReviewCount;
                item.DistrictId = hotelItem.DistrictId;
                item.DistrictName = hotelItem.DistrictName;
                item.DistrictEName = hotelItem.DistrictEName;
                item.ProvinceName = (hotelItem.ProvinceName == "中国" ? hotelItem.DistrictName : hotelItem.ProvinceName);
                item.InChina = hotelItem.InChina;
                packageList20.Add(item);
            }
            return packageList20;
        }
        /// <summary>
        /// 获取分组套餐专辑列表,Package.PackageGroupName相同且不为空，只取价格最低的套餐
        /// </summary>
        /// <param name="isValid"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <param name="albumsId"></param>
        /// <param name="isShowVipFirstBuypackage"></param>
        /// <param name="isNeedNotSale"></param>
        /// <param name="dateStr"></param>
        /// <param name="gotoDistrictID"></param>
        /// <param name="startDistrictID"></param>
        /// <returns></returns>
        public List<TopNPackageItem> GetTopNGroupPackageList(bool isValid, int start, int count, int albumsId = 0, bool isShowVipFirstBuypackage = true, bool isNeedNotSale = false, string dateStr = "", int gotoDistrictID = 0, int startDistrictID = 0, int pid=0)
        {
            var albumsEntity = albumsId > 0 ? HotelDAL.GetOnePackageAlbums(albumsId) : new PackageAlbumsEntity();

            TopNPackageSearchParam param = new TopNPackageSearchParam()
            {
                Filter = new TopNPackageSearchFilter()
                {
                    IsValid = true,
                    NeedRankRange = 0,
                    AlbumsID = albumsId,
                    NeedNotSale = isNeedNotSale,
                    VipFirstBuyPackage = isShowVipFirstBuypackage ? 1 : 0,
                    DateStr = dateStr,
                    GoToDistrictId = gotoDistrictID,
                    StartDistrictId = startDistrictID,
                    PackageID=pid
                },
                Start = start,
                PageSize = count,
                Sort = new Dictionary<string, string>()
            };
            int totalCount = 0;
            //2酒店专辑 和 1套餐专辑有区别
            List<TopNPackagesEntity> list = albumsEntity.Type == 2 ? HotelDAL.GetTopNGroupPackagesEntityList4HotelAlbums(param, out totalCount) : HotelDAL.GetTopNGroupPackagesEntityList(param, out totalCount);

            List<int> pids = list.FindAll(_ => _.PID > 0).Select(_ => _.PID).ToList();
            var currentDate = DateTime.Now.Date;
            if (albumsId == 13)
            {
                currentDate = DateTime.Parse("2016-10-01");
            }
            var packageList = pids.Any() ? GetPackageItemList2(pids, currentDate) : new List<TopNPackageItem>();
            var packageList20 = new List<TopNPackageItem>();
            foreach (var temp in list)
            {
                var item = packageList.Any() ? packageList.Find(_ => temp.PID == _.PackageID) : new TopNPackageItem();
                item.RecomemndWord = temp.RecomemndWord;
                item.RecomemndWord2 = temp.RecomemndWord2;
                item.RecomendPicShortNames = temp.RecomendPicShortNames;
                item.RecomendPicShortNames2 = temp.RecomendPicShortNames2;
                item.Title = temp.Title;
                item.MarketPrice = temp.MarketPrice;
                item.CoverPicSUrl = temp.CoverPicSUrl;
                item.AlbumsID = temp.AlbumsID;
                item.AlbumsName = temp.AlbumsName;
                item.PackageName = temp.PackageName;
                item.ForVIPFirstBuy = temp.ForVIPFirstBuy;
                item.VIPFirstPayDiscount = temp.VIPFirstPayDiscount;
                item.DateSelectType = temp.DateSelectType;
                item.Rank = temp.Rank;
                item.DayLimitMin = 1;
                item.PackageGroupName = temp.PackageGroupName;
                try
                {
                    //根据酒店和套餐获取其最少预订天数
                    var packagePrice = new List<PackageInfoEntity>() { new HotelService().GetHotelPackageByCode(item.HotelID, "", temp.PID) };
                    if (packagePrice != null)
                    {
                        var _min = packagePrice.Min(p => p.packageBase.DayLimitMin);
                        if (_min > 0) item.DayLimitMin = _min;
                    }
                }
                catch (Exception ex) { }
                item.StartDistrictId = temp.StartDistrictId;//出发地
                item.StartDistrictName = temp.StartDistrictName;

                var hotelItem = GetHotel(item.HotelID);//内部memcached缓存
                if (hotelItem != null)
                {
                    item.HotelScore = hotelItem.Score;
                    item.HotelReviewCount = hotelItem.ReviewCount;
                    item.DistrictId = hotelItem.DistrictId;
                    item.DistrictName = hotelItem.DistrictName;
                    item.DistrictEName = hotelItem.DistrictEName;
                    item.ProvinceName = (hotelItem.ProvinceName == "中国" ? hotelItem.DistrictName : hotelItem.ProvinceName);
                    item.InChina = hotelItem.InChina;
                }
                packageList20.Add(item);
            }
            return packageList20;
        }
        public List<TopNPackageItem> GetTopNPackageListByDistrictIds(bool isValid, int start, int count, int albumsId = 0, string districtIds = "", bool isShowVipFirstBuypackage = true, string dateStr = "", int gotoDistrictID = 0)
        { 
        var albumsEntity = albumsId > 0 ? HotelDAL.GetOnePackageAlbums(albumsId) : new PackageAlbumsEntity();

            TopNPackageSearchParam param = new TopNPackageSearchParam()
            {
                Filter = new TopNPackageSearchFilter()
                {
                    IsValid = true,
                    NeedRankRange = 0,
                    AlbumsID = albumsId,
                    VipFirstBuyPackage = isShowVipFirstBuypackage ? 1 : 0,
                    DistrictIds = districtIds,
                    DateStr = dateStr,
                    GoToDistrictId = gotoDistrictID
                },
                Start = start,
                PageSize = count,
                Sort = new Dictionary<string, string>()
            };
            int totalCount = 0;
            //2酒店专辑 和 1套餐专辑有区别
            List<TopNPackagesEntity> list = albumsEntity.Type == 2 ? HotelDAL.GetTopNPackagesEntityList4HotelAlbums(param, out totalCount) : HotelDAL.GetTopNPackagesEntityListByDistrictIds(param, out totalCount);

            List<int> pids = list.FindAll(_ => _.PID > 0).Select(_ => _.PID).ToList();
            var currentDate = DateTime.Now.Date;
            if (albumsId == 13)
            {
                currentDate = DateTime.Parse("2016-10-01");
            }
            var packageList = pids.Any() ? GetPackageItemList2(pids, currentDate) : new List<TopNPackageItem>();
            var packageList20 = new List<TopNPackageItem>();
            foreach (var temp in list)
            {
                var item = packageList.Any() ? packageList.Find(_ => temp.PID == _.PackageID) : new TopNPackageItem();
                item.RecomemndWord = temp.RecomemndWord;
                item.RecomemndWord2 = temp.RecomemndWord2;
                item.RecomendPicShortNames = temp.RecomendPicShortNames;
                item.RecomendPicShortNames2 = temp.RecomendPicShortNames2;
                item.Title = temp.Title;
                item.MarketPrice = temp.MarketPrice;
                item.CoverPicSUrl = temp.CoverPicSUrl;
                item.AlbumsID = temp.AlbumsID;
                item.AlbumsName = temp.AlbumsName;
                item.PackageName = temp.PackageName;
                item.ForVIPFirstBuy = temp.ForVIPFirstBuy;
                item.VIPFirstPayDiscount = temp.VIPFirstPayDiscount;
                item.DateSelectType = temp.DateSelectType;

                item.DayLimitMin = 1;
                try
                {
                    //根据酒店和套餐获取其最少预订天数
                    var packagePrice = new List<PackageInfoEntity>() { new HotelService().GetHotelPackageByCode(item.HotelID, "", temp.PID) };
                    if (packagePrice != null)
                    {
                        var _min = packagePrice.Min(p => p.packageBase.DayLimitMin);
                        if (_min > 0) item.DayLimitMin = _min;
                    }
                }
                catch (Exception ex) { }

                var hotelItem = GetHotel(item.HotelID);//内部memcached缓存
                item.HotelScore = hotelItem.Score;
                item.HotelReviewCount = hotelItem.ReviewCount;
                item.DistrictId = hotelItem.DistrictId;
                item.DistrictName = hotelItem.DistrictName;
                item.DistrictEName = hotelItem.DistrictEName;
                item.ProvinceName = (hotelItem.ProvinceName == "中国" ? hotelItem.DistrictName : hotelItem.ProvinceName);
                item.InChina = hotelItem.InChina;
                packageList20.Add(item);
            }
            return packageList20;
        }
        /// <summary>
        /// 根据目的获取分组套餐专辑列表,Package.PackageGroupName相同且不为空，只取价格最低的套餐
        /// </summary>
        /// <param name="isValid"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <param name="albumsId"></param>
        /// <param name="DistrictIds"></param>
        /// <returns></returns>
        public List<TopNPackageItem> GetTopNGroupPackageListByDistrictIds(bool isValid, int start, int count, int albumsId = 0, string districtIds = "", bool isShowVipFirstBuypackage = true, string dateStr = "", int gotoDistrictID = 0, int startDistrictID = 0)
        {
            var albumsEntity = albumsId > 0 ? HotelDAL.GetOnePackageAlbums(albumsId) : new PackageAlbumsEntity();

            TopNPackageSearchParam param = new TopNPackageSearchParam()
            {
                Filter = new TopNPackageSearchFilter()
                {
                    IsValid = true,
                    NeedRankRange = 0,
                    AlbumsID = albumsId,
                    VipFirstBuyPackage = isShowVipFirstBuypackage ? 1 : 0,
                    DistrictIds = districtIds,
                    DateStr = dateStr,
                    GoToDistrictId = gotoDistrictID,
                    StartDistrictId = startDistrictID
                },
                Start = start,
                PageSize = count,
                Sort = new Dictionary<string, string>()
            };
            int totalCount = 0;
            //2酒店专辑 和 1套餐专辑有区别
            List<TopNPackagesEntity> list = albumsEntity.Type == 2 ? HotelDAL.GetTopNGroupPackagesEntityList4HotelAlbums(param, out totalCount) : HotelDAL.GetTopNPackagesEntityListByDistrictIds(param, out totalCount);

            List<int> pids = list.FindAll(_ => _.PID > 0).Select(_ => _.PID).ToList();
            var currentDate = DateTime.Now.Date;
            if (albumsId == 13)
            {
                currentDate = DateTime.Parse("2016-10-01");
            }
            var packageList = pids.Any() ? GetPackageItemList2(pids, currentDate) : new List<TopNPackageItem>();
            var packageList20 = new List<TopNPackageItem>();
            foreach (var temp in list)
            {
                var item = packageList.Any() ? packageList.Find(_ => temp.PID == _.PackageID) : new TopNPackageItem();
                item.RecomemndWord = temp.RecomemndWord;
                item.RecomemndWord2 = temp.RecomemndWord2;
                item.RecomendPicShortNames = temp.RecomendPicShortNames;
                item.RecomendPicShortNames2 = temp.RecomendPicShortNames2;
                item.Title = temp.Title;
                item.MarketPrice = temp.MarketPrice;
                item.CoverPicSUrl = temp.CoverPicSUrl;
                item.AlbumsID = temp.AlbumsID;
                item.AlbumsName = temp.AlbumsName;
                item.PackageName = temp.PackageName;
                item.ForVIPFirstBuy = temp.ForVIPFirstBuy;
                item.VIPFirstPayDiscount = temp.VIPFirstPayDiscount;
                item.DateSelectType = temp.DateSelectType;
                item.Rank = temp.Rank;
                item.DayLimitMin = 1;
                try
                {
                    //根据酒店和套餐获取其最少预订天数
                    var packagePrice = new List<PackageInfoEntity>() { new HotelService().GetHotelPackageByCode(item.HotelID, "", temp.PID) };
                    if (packagePrice != null)
                    {
                        var _min = packagePrice.Min(p => p.packageBase.DayLimitMin);
                        if (_min > 0) item.DayLimitMin = _min;
                    }
                }
                catch (Exception ex) { }
                item.StartDistrictId = temp.StartDistrictId;
                item.StartDistrictName = temp.StartDistrictName;

                var hotelItem = GetHotel(item.HotelID);//内部memcached缓存
                item.HotelScore = hotelItem.Score;
                item.HotelReviewCount = hotelItem.ReviewCount;
                item.DistrictId = hotelItem.DistrictId;
                item.DistrictName = hotelItem.DistrictName;
                item.DistrictEName = hotelItem.DistrictEName;
                item.ProvinceName = (hotelItem.ProvinceName == "中国" ? hotelItem.DistrictName : hotelItem.ProvinceName);
                item.InChina = hotelItem.InChina;
                packageList20.Add(item);
            }
            return packageList20;
        }
        public List<TopNPackageItem> GetTopNPackageAddSearchList(bool isValid, int start, int count, int albumsId = 0, float lat = 0, float lng = 0, int geoScopeType = 0, int districtID = 0,bool isShowVipFirstBuypackage = true)
        {
            var albumsEntity = albumsId > 0 ? HotelDAL.GetOnePackageAlbums(albumsId) : new PackageAlbumsEntity();

            TopNPackageSearchParam param = new TopNPackageSearchParam()
            {
                Filter = new TopNPackageSearchFilter()
                {
                    IsValid = true,
                    NeedRankRange = 0,
                    AlbumsID = albumsId,
                    VipFirstBuyPackage = isShowVipFirstBuypackage ? 1 : 0
                },
                Start = start,
                PageSize = count,
                lat = lat,
                lng = lng,
                districtID = districtID,
                geoScopeType = geoScopeType,
                Sort = new Dictionary<string, string>()
            };
            int totalCount = 0;
            //LogHelper.WriteLog(string.Format("lat：{0}，lng：{1}，geoScopeType:{2},districtID:{3},AlbumsID：{4}，GroupNo：{5}，HotelID:{6},HotelName:{7},IsValid：{8}，NeedRankRange：{9}，PackageID:{10},PackageName:{11},Type{12}",
            //    param.lat,param.lng,param.geoScopeType,param.districtID, param.Filter.AlbumsID, param.Filter.GroupNo, param.Filter.HotelID, param.Filter.HotelName, param.Filter.IsValid, param.Filter.NeedRankRange, param.Filter.PackageID, param.Filter.PackageName, param.Filter.Type));
            //2酒店专辑 和 1套餐专辑有区别
            List<TopNPackagesEntity> list = albumsEntity.Type == 2 ? HotelDAL.GetTopNPackagesEntityList4HotelAlbums(param, out totalCount) : HotelDAL.GetTopNPackagesAddSearchEntityList(param, out totalCount);
            //LogHelper.WriteLog("totalCount" + totalCount);
            List<int> pids = list.FindAll(_ => _.PID > 0).Select(_ => _.PID).ToList();
            var currentDate = DateTime.Now.Date;
            if (albumsId == 13)
            {
                currentDate = DateTime.Parse("2016-10-01");
            }
            var packageList = pids.Any() ? GetPackageItemList2(pids, currentDate) : new List<TopNPackageItem>();
            var packageList20 = new List<TopNPackageItem>();
            foreach (var temp in list)
            {
                var item = packageList.Any() ? packageList.Find(_ => temp.PID == _.PackageID) : new TopNPackageItem();
                item.RecomemndWord = temp.RecomemndWord;
                item.RecomemndWord2 = temp.RecomemndWord2;
                item.RecomendPicShortNames = temp.RecomendPicShortNames;
                item.RecomendPicShortNames2 = temp.RecomendPicShortNames2;
                item.Title = temp.Title;
                item.MarketPrice = temp.MarketPrice;
                item.CoverPicSUrl = temp.CoverPicSUrl;
                item.AlbumsID = temp.AlbumsID;
                item.AlbumsName = temp.AlbumsName;
                item.PackageName = temp.PackageName;
                item.ForVIPFirstBuy = temp.ForVIPFirstBuy;
                item.VIPFirstPayDiscount = temp.VIPFirstPayDiscount;
                item.DateSelectType = temp.DateSelectType;

                item.DayLimitMin = 1;
                try
                {
                    //根据酒店和套餐获取其最少预订天数
                    var packagePrice = new List<PackageInfoEntity>() { new HotelService().GetHotelPackageByCode(item.HotelID, "", temp.PID) };
                    if (packagePrice != null)
                    {
                        var _min = packagePrice.Min(p => p.packageBase.DayLimitMin);
                        if (_min > 0) item.DayLimitMin = _min;
                    }
                }
                catch (Exception ex) { }

                var hotelItem = GetHotel(item.HotelID);//内部memcached缓存
                item.HotelScore = hotelItem.Score;
                item.HotelReviewCount = hotelItem.ReviewCount;
                item.DistrictId = hotelItem.DistrictId;
                item.DistrictName = hotelItem.DistrictName;
                item.DistrictEName = hotelItem.DistrictEName;
                item.ProvinceName = (hotelItem.ProvinceName == "中国" ? hotelItem.DistrictName : hotelItem.ProvinceName);
                item.InChina = hotelItem.InChina;
                packageList20.Add(item);
            }
            return packageList20;
        }

        public List<HotelDestnInfo> GetHotelDestnInfo(int albumsID)
        {
            return HotelDAL.GetHotelDestnInfo(albumsID);
        }

        public List<HotelDestnInfo> GetHotelDestWithIn(int albumsID,float lat, float lng)
        {
            return HotelDAL.GetHotelDestWithIn(albumsID, lat, lng);
        }

        public int UpdateTopNPackageBatch(bool IsValid, long Updator)
        {
            return HotelDAL.UpdateTopNPackageBatch(IsValid, Updator);
        }

        public int DeleteTopNPackage(int id)
        {
            return HotelDAL.DeleteTopNPackage(id);
        }

        public List<TypeAndPrice> GetPackageTypeAndPriceList(List<int> pids)
        {
            return HotelDAL.GetPackageTypeAndPriceList(pids);
        }

        /// <summary>
        /// 计算两个地区之间的距离  单位米
        /// </summary>
        /// <param name="districtID1"></param>
        /// <param name="districtID2"></param>
        /// <returns></returns>
        public int CalculateDistrictDistance(int districtID1, int districtID2)
        {
            return HotelDAL.CalculateDistrictDistance(districtID1, districtID2);
        }

        public int CalculateUserDistrictDistance(double userLat, double userLon, int districtID)
        {
            return HotelDAL.CalculateUserDistrictDistance(userLat, userLon, districtID);
        }

        public List<ArounDistrictEntity> CalculateNearDistrictByDistance(int districtID, float lat, float lon, int distance = 300000)
        {
            return HotelDAL.CalculateNearDistrictByDistance(districtID, lat, lon, distance);
        }
        #endregion

        /// <summary>
        /// 批量获取酒店列表过滤标签名称ID
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public List<FilterDicEntity> GetHotelListFilterTagInfos(SearchHotelListFilterTagInfoParam param)
        {
            return HotelDAL.GetHotelListFilterTagInfos(param);
        }

        /// <summary>
        /// 由名称生成一条客户添加酒店的新纪录 如果名称存在则返回记录的ID
        /// 如果commentID不为0那么新增一条点评记录与用户添加酒店的对应关系
        /// </summary>
        /// <param name="hotelName"></param>
        /// <param name="commentID"></param>
        /// <returns></returns>
        public int InsertUserAddHotels(string hotelName, long userID, int commentID = 0)
        {
            //先提交待添加酒店记录
            int addHotelID = HotelDAL.InsertUserAddHotels(hotelName, userID);
            if (addHotelID > 0 && commentID > 0)
            {
                //如果待添加酒店Okay且点评ID不等于0 那么再加一条关联记录
                HotelDAL.InsertUserAddHotelCommentRel(addHotelID, commentID);
            }
            return addHotelID;
        }

        /// <summary>
        /// 由点评ID获得新增酒店的名称
        /// </summary>
        /// <param name="commentID"></param>
        /// <returns></returns>
        public List<CommentAddHotelEntity> GetUserAddHotelByComment(int commentID, long userID = 0)
        {
            return HotelDAL.GetUserAddHotelByComment(commentID, userID);
        }

        /// <summary>
        /// 酒店名称 用户ID 验证是否提交过该名称的点评 防止重复点评
        /// </summary>
        /// <param name="hotelName"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public int GetUserAddHotelForComment(string hotelName, long userID)
        {
            return HotelDAL.GetUserAddHotelForComment(hotelName, userID);
        }

        /// <summary>
        /// 更新酒店坐标数据集合
        /// </summary>
        /// <param name="zoneID"></param>
        /// <returns></returns>
        public int UpdateHotelDistrictZoneRel(int zoneID)
        {
            if (zoneID > 0)
            {
                string ids = HotelDAL.UpdateHotelDistrictZoneRel(0, zoneID);
                HotelDAL.UpdateHotelFilter(0, zoneID, 18, string.IsNullOrWhiteSpace(ids) ? new List<int>() : ids.Split(',').Select(_ => int.Parse(_)));
            }
            return 0;
        }

        /// <summary>
        /// 绑定区域和酒店
        /// </summary>
        /// <param name="districtZoneID"></param>
        /// <param name="districtZoneName"></param>
        /// <param name="bindType"></param>
        /// <returns></returns>
        public int BindZoneHotelRel(int zoneID, string districtZoneName, BindZoneHotelRelType bindType)
        {
            if (zoneID > 0)
            {
                string hotelIds = "";
                if (bindType == BindZoneHotelRelType.OnlyZMJD)
                {
                    hotelIds = HotelDAL.UpdateHotelDistrictZoneRel(0, zoneID);//先删除之前的关联关系 再加入
                }
                else if (bindType == BindZoneHotelRelType.OnlyCtrip)
                {
                    hotelIds = HotelDAL.GetDistrictZoneMapOtherZoneHotel(zoneID);
                    HotelDAL.InsertHotelDistrictZoneRel(zoneID, hotelIds, true);
                }
                else if (bindType == BindZoneHotelRelType.BothZMJDAndCtrip)
                {
                    hotelIds = HotelDAL.UpdateHotelDistrictZoneRel(0, zoneID);//先删除之前的关联关系 再根据画的区域计算区域内酒店 返回涉及酒店字符串
                    string xcHotelIds = HotelDAL.GetDistrictZoneMapOtherZoneHotel(zoneID);
                    //合并区域计算出的酒店ID和xc酒店
                    if (string.IsNullOrWhiteSpace(hotelIds) && string.IsNullOrWhiteSpace(xcHotelIds))
                    {
                        hotelIds = "";
                    }
                    else if (string.IsNullOrWhiteSpace(hotelIds) && !string.IsNullOrWhiteSpace(xcHotelIds))
                    {
                        hotelIds = xcHotelIds;
                    }
                    else if (!string.IsNullOrWhiteSpace(hotelIds) && !string.IsNullOrWhiteSpace(xcHotelIds))
                    {
                        hotelIds += "," + xcHotelIds;
                        IEnumerable<string> ids = hotelIds.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Distinct();//求并集
                        hotelIds = string.Join(",", ids);
                    }
                    HotelDAL.InsertHotelDistrictZoneRel(zoneID, hotelIds, true);
                }
                HotelDAL.UpdateHotelFilter(0, zoneID, 18, string.IsNullOrWhiteSpace(hotelIds) ? new List<int>() : hotelIds.Split(',').Select(_ => int.Parse(_)));//快速更新
            }
            else
            {
                return 1;
            }
            return 0;
        }

        /// <summary>
        /// 获得酒店的设施集合
        /// </summary>
        /// <param name="hotelID"></param>
        /// <returns></returns>
        public List<HotelFacilityEntity> GetHotelFacilitysByHotelID(int hotelID)
        {
            return HotelDAL.GetHotelFacilitysByHotelID(hotelID);
        }

        /// <summary>
        /// 主题特色关联酒店数量（默认按酒店数量降序排列）
        /// </summary>
        /// <param name="interestType"></param>
        /// <returns></returns>
        public IEnumerable<InterestHotelCountEntity> GetInterestHotelCountList(int interestType)
        {
            return HotelDAL.GetInterestHotelCountList(interestType);
        }

        /// <summary>
        /// 批量获取酒店基本信息
        /// </summary>
        /// <param name="hotelIDs"></param>
        /// <returns></returns>
        public IEnumerable<HotelBasicInfo> GetHotelBasicInfoList(IEnumerable<int> hotelIDs)
        {
            return HotelDAL.GetHotelBasicInfoList(hotelIDs);
        }

        public List<PointsEntity> GetPointListByTypeIDAndUserID(int typeid, IEnumerable<long> ffRelIds)
        {
            return HotelDAL.GetPointListByTypeIDAndUserID(typeid, ffRelIds);
        }

        public List<PointsEntity> GetPointListByTypeIDAndBusinessID(int typeid, IEnumerable<long> ffRelIds)
        {
            return HotelDAL.GetPointListByTypeIDAndBusinessID(typeid, ffRelIds);
        }
        /// <summary>
        /// 酒店相关主题 已发布的
        /// </summary>
        /// <param name="hotelID"></param>
        /// <returns></returns>
        public IEnumerable<InterestEntity> GetInterestListByHotel(int hotelID)
        {
            return HotelDAL.GetInterestListByHotel(hotelID);
        }

        /// <summary>
        /// 回收哪类积分 默认所有过期的积分
        /// </summary>
        /// <param name="typeID"></param>
        /// <returns></returns>
        public int PointsRecycle(int typeID = 0)
        {
            //取出所有过期的且剩余积分大于0的积分记录
            IEnumerable<PointsEntity> pointsList = HotelDAL.GetExpiredPoints(typeID);
            //然后清空剩余积分 将清掉的剩余积分 作为回收类型插入到消费记录里面
            foreach (PointsEntity pe in pointsList)
            {
                int leftPoints = pe.LeavePoint;
                pe.LeavePoint = 0;
                HotelDAL.InsertOrUpdatePoints(pe);//更新积分数量

                PointsConsumeEntity pce = new PointsConsumeEntity()
                {
                    State = 1,
                    BusinessID = pe.ID,
                    TypeID = 8,
                    ConsumePoint = leftPoints,
                    UserID = pe.UserID,
                    ID = 0
                };//回收应该和消费积分一致
                HotelDAL.InsertOrUpdatePointsConsume(pce);//更新积分消费记录
            }
            return 0;
        }

        /// <summary>
        /// 获得需要提醒的品鉴师写点评
        /// </summary>
        /// <param name="maxDay"></param>
        /// <returns></returns>
        public IEnumerable<InspectorRefHotel> GetNeedWriteCommentInspectorRefHotel(int maxDay = 7)
        {
            return HotelDAL.GetNeedWriteCommentInspectorRefHotel(maxDay);
        }

        /// <summary>
        /// 更新指定的品鉴酒店提醒写点评状态
        /// </summary>
        /// <param name="inspectorRefHotelIDs"></param>
        /// <returns></returns>
        public int UpdateNoticeWriteCommentState4InspectorRefHotel(IEnumerable<int> inspectorRefHotelIDs)
        {
            foreach (var ehID in inspectorRefHotelIDs)
            {
                HotelDAL.UpdateInspectorRefHotel4Comment(ehID, 0, true);
            }
            return 0;
        }

        public HotelMapBasicInfo GetHotelMapInfo(int hotelID)
        {
            return HotelDAL.GetHotelMapInfo(hotelID);
        }

        /// <summary>
        /// 获取用户某个状态的品鉴记录
        /// 如果state=0则是全部状态的品鉴记录 1.待审核 2.审核通过 3.审核不通过
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public IEnumerable<InspectorRefHotel> GetInspectorRefHotelByUserID(long userID, int state = 0)
        {
            return HotelDAL.GetInspectorRefHotelByUserID(userID, state);
        }

        /// <summary>
        /// 获取地区对应攻略目的地的照片
        /// </summary>
        /// <param name="districtId"></param>
        /// <returns></returns>
        public string GetDistrictZonePicSUrl(int districtId)
        {
            return HotelDAL.GetDistrictZonePicSUrl(districtId);
        }

        #region 微信酒店列表

        public QueryHotelResult3 QueryHotelByDistrictInterest(int districtId, int interestId = 0)
        {
            QueryHotelResult qhr = new QueryHotelResult();

            List<int> hidList = new List<int>();
            if (interestId > 0)
            {
                if (districtId > 0)
                {
                    hidList = HotelDAL.GetHotelListByDistrictInterest(districtId, interestId);
                }
                else
                {
                    hidList = HotelDAL.GetHotelListByInterest(interestId);
                }
            }
            else
            {
                hidList = HotelDAL.GetHotelListByDistrict(districtId);
            }

            qhr.HotelList = GetSimpleHotels(hidList);

            QueryHotelResult3 q = new QueryHotelResult3();
            q.FilterCount = qhr.FilterCount;
            q.TotalCount = qhr.TotalCount;
            q.HotelList = TransToListHotelItemWeixin(qhr.HotelList, interestId);

            return q;
        }

        public QueryHotelResult3 QueryHotelByHids(string hotelids)
        {
            QueryHotelResult qhr = new QueryHotelResult();

            if (hotelids.Contains(" ")) hotelids = hotelids.Replace(" ", ",");
            else if (hotelids.Contains("\n")) hotelids = hotelids.Replace("\n", ",");

            List<int> hidList = hotelids.Split(',').ToList().Select(hid => Convert.ToInt32(hid)).ToList();

            qhr.HotelList = GetSimpleHotels(hidList);

            QueryHotelResult3 q = new QueryHotelResult3();
            q.FilterCount = qhr.FilterCount;
            q.TotalCount = qhr.TotalCount;
            q.HotelList = TransToListHotelItemWeixin(qhr.HotelList, 0);

            return q;
        }

        public List<string> GetProvinceListByInterest(int interestId)
        {
            if (interestId > 0)
            {
                return memCacheHotel.GetData<List<string>>("GetProvinceListByInterest_" + interestId, () => { return HotelDAL.GetProvinceListByInterest(interestId); });
            }
            else if (interestId < 0)
            {
                return memCacheHotel.GetData<List<string>>("GetProvinceListUnChina__" + interestId, () => { return HotelDAL.GetProvinceListUnChina(); });
            }
            return memCacheHotel.GetData<List<string>>("GetProvinceListInChina__" + interestId, () => { return HotelDAL.GetProvinceListInChina(); });
        }

        #endregion

        /// <summary>
        /// 插入浏览记录
        /// </summary>
        /// <param name="cbre"></param>
        /// <returns></returns>
        public int InsertBrowsingRecord(BrowsingRecordEntity browsing)
        {
            return HotelDAL.InsertBrowsingRecord(browsing);
        }

        public int InsertSearchRecord(SearchRecordEntity search)
        {
            return HotelDAL.InsertSearchRecord(search);
        }

        /// <summary>
        /// 插入分享记录
        /// </summary>
        /// <param name="csre"></param>
        /// <returns></returns>
        public int InsertShareRecord(ShareRecordEntity share)
        {
            return HotelDAL.InsertShareRecord(share);
        }

        /// <summary>
        /// 获取分享记录
        /// </summary>
        /// <param name="csre"></param>
        /// <returns></returns>
        public List<ShareRecordEntity> GetShareRecordList(CommonRecordQueryParam param)
        {
            return HotelDAL.GetShareRecordList(param);
        }

        /// <summary>
        /// 获取点评各渠道的浏览数量
        /// </summary>
        /// <param name="commentId"></param>
        /// <param name="terminalTypeArray"></param>
        /// <returns></returns>
        public int GetBrowseringCountOneComment(int commentId, int[] terminalTypeArray)
        {
            return HotelDAL.GetBrowseringCountOneComment(commentId, terminalTypeArray);
        }

        /// <summary>
        /// 获取某个地区热门搜索Tag
        /// </summary>
        /// <param name="districtId"></param>
        /// <returns></returns>
        public List<DistrictHotFilterTagEntity> GetDistrictHotFilterTagList(int districtId)
        {
            return HotelDAL.GetDistrictHotFilterTagList(districtId);
        }

        /// <summary>
        /// 获取某个地区热门搜索Tag
        /// </summary>
        /// <param name="districtId"></param>
        /// <returns></returns>
        public int UpsertDistrictHotFilterTagList(IEnumerable<DistrictHotFilterTagEntity> hotTags)
        {
            if (hotTags == null || hotTags.Count() == 0)
            {
                return 1;
            }
            foreach (var hotTag in hotTags)
            {
                HotelDAL.UpsertDistrictHotFilterTag(hotTag);
            }
            return 0;
        }

        public InterestEntity GetOneInterestEntity(int Id)
        {
            return HotelDAL.GetOneInterestEntity(Id);
        }

        public int UpdateCommentForOrderId(long orderId, int commentId)
        {
            return HotelDAL.UpdateCommentForOrderId(orderId, commentId);
        }

        /// <summary>
        /// 查询指定酒店的基于OTA数据的房态数据
        /// </summary>
        /// <param name="hotelId"></param>
        /// <returns></returns>
        public List<OtaRoomBedState> GetOtaRoomBedStateByHid(int hotelId)
        {
            return HotelDAL.GetOtaRoomBedStateByHid(hotelId);
        }

        /// <summary>
        /// 获取有合作的酒店的房态汇总信息
        /// </summary>
        /// <returns></returns>
        public List<OtaHotelRoomState> GetOtaHotelRoomStates()
        {
            return HotelDAL.GetOtaHotelRoomStates();
        }

        #region 各种优惠套餐
        public List<CanSellCheapHotelPackageEntity> GetCheapHotelPackage4Botao(DateTime startTime)
        {
            var result = HotelDAL.GetCheapHotelPackage4Botao(startTime);
            if (result != null && result.Count != 0)
            {
                result.ForEach((_) =>
                {
                    _.CheapPrice = _.Price;//_.Price - 5; 
                });//2016.06.22 wwb 博涛不在处理价格
            }
            return result;
        }
        #endregion

        #region 批量获取房型列表
        /// <summary>
        /// 获取房间列表
        /// </summary>
        /// <param name="roomIds"></param>
        /// <returns></returns>
        public List<PRoomInfoEntity> GetPRoomInfoEntityList(IEnumerable<int> roomIds)
        {
            return HotelDAL.GetPRoomInfoEntityList(roomIds);
        }
        #endregion

        /// <summary>
        /// 获取微信活动列表
        /// </summary>
        /// <returns></returns>
        public List<ActiveRuleGroupEntity> GetWXActiveRuleGroupList(int id)
        {
            return HotelDAL.GetWXActiveRuleGroupList(id);
        }
        /// <summary>
        /// 获取微信活动列表详情
        /// </summary>
        /// <returns></returns>
        public List<ActiveRuleExEntity> GetWXActiveRuleExList(int groupId)
        {
            return HotelDAL.GetWXActiveRuleExList(groupId);
        }

        public List<SimpleHotelEntity> GetHotelPackageDistrictInfo(double lat = 0, double lng = 0, int geoScopeType = 0)
        {
            return HotelDAL.GetHotelPackageDistrictInfo(lat, lng, geoScopeType);
        }
        /// <summary>
        /// 根据地区获取套餐信息
        /// </summary>
        /// <param name="distictId"></param>
        /// <param name="checkIn"></param>
        /// <param name="checkOut"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<PackageItemEntity> GetHotelPackageByDistrictId(int distictId, DateTime checkIn, DateTime checkOut, int pageIndex, int pageSize, out int totalCount)
        {
            List<PackageItemEntity> packageList = HotelDAL.GetHotelPackageByDistrictId(distictId, checkIn, checkOut, pageIndex, pageSize, out totalCount);
            List<int> pids = packageList.FindAll(_ => _.PackageID > 0).Select(_ => _.PackageID).ToList();
            var currentDate = System.DateTime.Now;
            //var  listItem = pids.Any() ? GetPackageItemList2(pids, currentDate) : new List<TopNPackageItem>();
            //var priceCalendar = HotelDAL.GetHotelPackageFirstAvailablePriceByPIds(pids, currentDate, null) ?? new List<HotelTop1PackageInfoEntity>();

            var priceCalendar = HotelDAL.GetHotelPackageMinPriceByPIds(pids, currentDate, null) ?? new List<HotelTop1PackageInfoEntity>();
            foreach (var item in packageList)
            {
                var price = priceCalendar.FirstOrDefault(_ => _.PID == item.PackageID);
                if (price != null)
                {
                    item.PackagePrice = new List<TypeAndPrice>(){
                            new TypeAndPrice(){
                                PID = item.PackageID,
                                AddInfo = "",
                                Price = price.Price,
                                Type = 0,
                                TypeName = ""
                            },
                            new TypeAndPrice(){
                                PID = item.PackageID,
                                AddInfo = "",
                                Price = price.VIPPrice,
                                Type = -1,
                                TypeName = ""
                            }
                        };
                }
                else
                {
                    item.PackagePrice = new List<TypeAndPrice>();
                }
                //item.PackagePrice = listItem.Find(_ => _.PackageID == item.PackageID).PackagePrice;
            }
            return packageList;
        }


        /// <summary>
        /// 获取酒店携程价显示策略
        /// </summary>
        /// <param name="hotelid"></param>
        /// <returns>0:可以显示最低价  1：不显示最低价 </returns>
        public int GetHotelCtripPricePolicy(int hotelid)
        {
            object obj =  memCacheHotel.Get( HotelCtripPricePolicyCacheKey  + ";" + hotelid.ToString());
            if(obj == null )
            {
                var p  =  HotelDAL.GetHotelCtripPricePolicy(hotelid) ;
                memCacheHotel.Set(HotelCtripPricePolicyCacheKey + ";" + hotelid.ToString(), p);
                return p;
            }
            else
            {
                return Convert.ToInt16(obj);
            }
        }
        public List<TravelPersonEntity> GetBookUserDateInfoByExchangCouponId(int ExchangCouponId)
        {
            return HotelDAL.GetBookUserDateInfoByExchangCouponId(ExchangCouponId);
        }

        public RetailHotelEntity GetRetailHotelList(int packageType, int sort, string searchWord, int start, int count)
        {
            RetailHotelEntity result = new RetailHotelEntity();
            int totalCount = HotelDAL.GetRetailHotelListCount(packageType, searchWord);
            List<RetailHotel> list =  HotelDAL.GetRetailHotelList(packageType, sort, searchWord, start, count);
            result.TotalCount = totalCount;
            result.list = list;
            return result;
        }

        public RetailHotelInfoEntity GetRetailHotelInfo(int hotelId)
        {
            RetailHotelInfoEntity model = new RetailHotelInfoEntity();
            RetailHotel hotelModel = HotelDAL.GetHotelChannel(hotelId);
            model.HotelId = hotelModel.HotelId;
            model.HotelName = hotelModel.HotelName;
            model.OriHotelId = hotelModel.OriHotelId;
            List<RetailPackageEntity> packageList = HotelDAL.GetRetailPackageList(hotelId);
            GenPackageMinPriceAndTime(packageList);
            model.RetailPackageList = packageList;
            return model;
        }

        public void GenPackageMinPriceAndTime(List<RetailPackageEntity> list)
        {
            string pids = string.Join(",", list.Select(_ => _.PID).ToList());
            List<PRateEntity> prateList = HotelDAL.GetPrateListByPids(pids);
            foreach (PRateEntity item in prateList)
            {
                RetailPackageEntity retailPackage = list.Where(_ => _.PID == item.PID).First();
                MinPriceAndDateEntity model = new MinPriceAndDateEntity();
                retailPackage.VipPrice = item.ManualVIPPrice == -1 ? decimal.Parse(item.Price.ToString()) : decimal.Parse(item.ManualVIPPrice.ToString());
                retailPackage.NotVipPrice = decimal.Parse(item.Price.ToString());
                retailPackage.ManualCommission = item.ManualCommission == -1 ? item.AutoCommission : item.ManualCommission;
                retailPackage.AutoCommission = item.AutoCommission;
               
                switch (item.Type)
                { 
                    case 0:
                        retailPackage.CheckIn = DateTime.Now.AddDays(1);
                        if (retailPackage.CheckIn.DayOfWeek == DayOfWeek.Friday)
                        {
                            retailPackage.CheckIn = retailPackage.CheckIn.AddDays(2);
                            break;
                        }
                        else if (retailPackage.CheckIn.DayOfWeek == DayOfWeek.Saturday)
                        {
                            retailPackage.CheckIn = retailPackage.CheckIn.AddDays(1);
                            break;
                        }
                        else
                        {
                            break;
                        }
                    case 1:
                        retailPackage.CheckIn = GetWeekDate(DateTime.Now, DayOfWeek.Monday);
                        break;
                    case 2:
                        retailPackage.CheckIn = GetWeekDate(DateTime.Now, DayOfWeek.Tuesday);
                        break;
                    case 3:
                        retailPackage.CheckIn = GetWeekDate(DateTime.Now, DayOfWeek.Wednesday);
                        break;
                    case 4:
                        retailPackage.CheckIn = GetWeekDate(DateTime.Now, DayOfWeek.Thursday);
                        break;
                    case 5:
                        retailPackage.CheckIn = GetWeekDate(DateTime.Now, DayOfWeek.Friday);
                        break;
                    case 6:
                        retailPackage.CheckIn = GetWeekDate(DateTime.Now, DayOfWeek.Saturday);
                        break;
                    case 7:
                        retailPackage.CheckIn = GetWeekDate(DateTime.Now, DayOfWeek.Sunday);
                        break;
                    case 8:
                        retailPackage.CheckIn = DateTime.Parse(item.Date.ToString());
                        break;
                }
                retailPackage.CheckOut = retailPackage.CheckIn.AddDays(1);
                
            }
            //return result;
        }

        public DateTime GetWeekDate(DateTime d, DayOfWeek w)
        {
            int i = d.DayOfWeek - w;
            if (i == -1) i = 6;// i值 > = 0 ，因为枚举原因，Sunday排在最前，此时Sunday-Monday=-1，必须+7=6。   
            TimeSpan ts = new TimeSpan(i, 0, 0, 0);
            DateTime date = DateTime.Now.Subtract(ts);
            if (date.ToShortDateString() == DateTime.Now.ToShortDateString() || date < DateTime.Now)
            {
                int i1 = d.AddDays(7).DayOfWeek - w;
                if (i1 == -1) i = 6;  
                TimeSpan ts1 = new TimeSpan(i1, 0, 0, 0);
                date = DateTime.Now.Subtract(ts1);
            }
            return date;
        }


        //public RetailPackageEntity GetRetailHotelInfo(int pid, long cid)
        //{
        //    RetailPackageEntity rp = new RetailPackageEntity();
        //    rp = HotelDAL.GetRetailPackageInfo(pid, cid);
        //    List<RetailPackageEntity> list = new List<RetailPackageEntity>();
        //    list.Add(rp);
        //    GenPackageMinPriceAndTime(list);
        //    return rp;
        //}

        public List<TopNPackagesEntity> GetTopNPackagesListByAlbumIdOrPID(int albumId = 0, int pid = 0)
        {
            return HotelDAL.GetTopNPackagesListByAlbumIdOrPID(albumId, pid);
        }
        public int UpdateTopNPackageTitle(int pid, string title)
        {
            return HotelDAL.UpdateTopNPackageTitle(pid, title);
        }
    }
}