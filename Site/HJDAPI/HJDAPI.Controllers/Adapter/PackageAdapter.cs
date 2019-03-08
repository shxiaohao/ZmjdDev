using HJD.Framework.Interface;
using HJD.Framework.WCF;
using HJD.HotelPrice.Contract;
using HJD.HotelPrice.Contract.DataContract;
using HJD.HotelServices.Contracts;
using HJDAPI.Controllers.Common;
using HJDAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WH.DataAccess.ActivityDB.Item;

namespace HJDAPI.Controllers.Adapter
{
    public class PackageAdapter
    {

        public static ICacheProvider LocalCache = CacheManagerFactory.Create("DynamicCacheForType2");
        //   public static IHotelService HotelService = ServiceProxyFactory.Create<IHotelService>("BasicHttpBinding_IHotelService");
        //   public static IHotelPriceService PriceService = ServiceProxyFactory.Create<IHotelPriceService>("BasicHttpBinding_IHotelPriceService");

        public static List<SpecialDealPackageEntity> GetSpecialDealPackage()
        {
            List<SpecialDealPackageEntity> sl = ServiceList.HotelService.GetSpecialDealPackage();
            foreach (var s in sl)
            {
                //获取第一个可销售日期的数据
                List<PDayItem> dl = GetHotelPackageCalendar30(s.HotelId, DateTime.Now, s.ID);
                foreach (var d in dl)
                {
                    if (d.SellState == 1)
                    {
                        s.CheckDate = d.Day;
                        s.RooCount = d.MaxSealCount;

                        s.PicUrl = PhotoAdapter.GenHotelPicUrl(s.SURL, HJDAPI.Common.Helpers.Enums.AppPhotoSize.appview);

                        HotelPrice2 hp = PriceAdapter.GetHotelPackageList(s.HotelId, s.CheckDate.ToString("yyyy-MM-dd"), s.CheckDate.AddDays(1).ToString("yyyy-MM-dd"), "web");
                        s.Price = hp.Packages.Where(p => p.packageBase.ID == s.ID).First().Price;

                        break;
                    }
                }
            }
            return sl;
        }
        public static List<CanSaleHotelInfoEntity> GetAllCanSellPackage(DateTime startDate, DateTime endDate, string tag = "")
        {
            List<CanSaleHotelInfoEntity> hl = ServiceList.HotelService.GetAllCanSellPackage(startDate, endDate, tag);
            SetHolidayHotRisePriceProperty(hl, startDate);
            return hl;
        }

        /// <summary>
        /// 设置酒店不涨价字段值
        /// </summary>
        /// <param name="hotelList"></param>
        public static void SetHolidayHotRisePriceProperty(List<CanSaleHotelInfoEntity> hotelList, DateTime startDate)
        {
            List<HolidayNotRiseItem> eList = LocalCache.GetData<List<HolidayNotRiseItem>>("AllHolidayNotRiseItemCacheKey" + startDate.ToShortDateString(), () =>
                   {
                       return ServiceList.ActivityService.GetAllHolidayNotRiseItem().Where(r => r.DT == startDate.Date).ToList();
                   }
                );

            hotelList.ForEach(h =>
                {
                    //IsFrontAddPay 是否前台补差价标识 true:不涨 
                    h.IsNotRisePrice = eList.Where(r => r.HotelID == h.HotelId || h.IsFrontAddPay == true).Count() > 0;
                }
            );

        }

        public List<SimplePackageEntity> GetSimplePackageInfo(string packageIDs)
        {
            return ServiceList.HotelService.GetSimplePackageInfo(packageIDs);
        }

        //public List<PackageInfoEntity> GetJLTourCanSellHotelPackages(int hotelid, DateTime startDate, out DateTime CheckIn, out DateTime CheckOut)
        //{
        //    CheckIn = DateTime.Now.Date;
        //    CheckOut = DateTime.Now.Date.AddDays(1);
        //        List<PDayItem> pdis = HotelService.GetJLHotelPackageCalendar(hotelid, DateTime.Now.Date);


        //        for (int i = 0; i < pdis.Count(); i++)
        //        {
        //            if (pdis[i].Day < startDate) continue;

        //            if (pdis[i].SellState > 1) continue;

        //            CheckIn = pdis[i].Day;
        //            CheckOut = CheckIn.AddDays(1);
        //            break;
        //        }

        //    //按是否可销售、价格来排序
        //        return HotelService.GetJLHotelPackages(hotelid, CheckIn, CheckOut).OrderBy(p => p.SellState).ThenBy(p => p.Price).ToList();
        //}

        public List<PackageInfoEntity> GetHotelPackages(int hotelid, DateTime CheckIn, DateTime CheckOut, int terminalType = 1, HotelServiceEnums.PricePolicyType ppt = HotelServiceEnums.PricePolicyType.Default, int pid = 0)
        {
            return ServiceList.HotelService.GetHotelPackageList(hotelid, CheckIn, CheckOut, terminalType, ppt, pid);
        }

        public List<PackageInfoEntity> GetHotelPackagesInfo(int hotelid, DateTime CheckIn, DateTime CheckOut, int terminalType = 1, HotelServiceEnums.PricePolicyType ppt = HotelServiceEnums.PricePolicyType.Default, int pid = 0)
        {
            return ServiceList.HotelService.GetHotelPackageListInfo(hotelid, CheckIn, CheckOut, terminalType, ppt, pid);
        }

        public static PackageInfoEntity GetFirstVIPPackageByPackageId(int pid, DateTime checkIn, DateTime checkOut)
        {
            return ServiceList.HotelService.GetFirstVipPackageByPackageId(pid, checkIn, checkOut);
        }
        /// <summary>
        /// 根据酒店ID & 套餐Code 获取指定套餐
        /// </summary>
        /// <param name="hotelid">酒店ID</param>
        /// <param name="code">套餐Code</param>
        /// <returns>套餐</returns>
        public PackageInfoEntity GetHotelPackageByCode(int hotelid, string code, int pid)
        {
            return ServiceList.HotelService.GetHotelPackageByCode(hotelid, code, pid);
        }

        /// <summary>
        /// 获取最后取消日期
        /// </summary>
        /// <param name="p"></param>
        /// <param name="CheckIn"></param>
        /// <param name="CheckOut"></param>
        /// <returns></returns>
        //private DateTime GetLastCancelTime(PackageInfoEntity p, DateTime CheckIn, DateTime CheckOut)
        //{
        //    CheckIn = CheckIn.Date;
        //    CheckOut = CheckOut.Date;
        //    DateTime minCancelDate = DateTime.Now.AddMonths(1);

        //    DateTime cancelDate = new DateTime();

        //    PRateEntity r = new PRateEntity();

        //    for( DateTime curDate = CheckIn; curDate < CheckOut;curDate = curDate.AddDays(1) )
        //    {
        //       r =  GetPackageRateByDate(p, curDate);

        //       cancelDate = curDate.AddDays(-r.CancelDayLimit).Date.AddHours(r.CancelTimeLimitHour).AddMinutes(r.CancelTimeLinitMinute);

        //       if (cancelDate < minCancelDate)
        //           minCancelDate = cancelDate;
        //    }

        //    return minCancelDate;
        //}

        public List<PackageInfoEntity> GetCanSellHotelPackages(int hotelid, DateTime startDate, DateTime CheckIn, DateTime CheckOut, HotelServiceEnums.PricePolicyType ppt = HotelServiceEnums.PricePolicyType.Default)
        {
            //2016-07-06 没有看出直接把checkIn和checkOut设置为今天和明天的用处
            //CheckIn = DateTime.Now.Date;
            //CheckOut = DateTime.Now.Date.AddDays(1);

            List<PackageInfoEntity> pis = GetHotelPackages(hotelid, CheckIn, CheckOut, 1, ppt);
            if (pis.Count > 0)
            {
                //按是否可销售、价格来排序
                return pis.OrderBy(p => p.SellState).ThenBy(p => p.Price).ToList();
            }
            else
            {
                return new List<PackageInfoEntity>();
            }
        }

        public List<PackageInfoEntity> GetJLCanSellHotelPackages(int hotelid, DateTime startDate, out DateTime CheckIn, out DateTime CheckOut, HotelServiceEnums.PricePolicyType ppt)
        {
            List<PackageInfoEntity> pis = ServiceList.HotelService.GetJLHotelPackages(hotelid, startDate, startDate.AddDays(1), ppt);
            CheckIn = DateTime.Now.Date;
            CheckOut = DateTime.Now.Date;
            if (pis.Count > 0)
            {
                List<PDayItem> pdis = ServiceList.HotelService.GetJLHotelPackageCalendar(hotelid, DateTime.Now.Date, ppt);


                for (int i = 0; i < pdis.Count(); i++)
                {
                    if (pdis[i].Day < startDate) continue;

                    if (pdis[i].SellState > 1) continue;

                    CheckIn = pdis[i].Day;
                    int minSellLimit = pis.Min(p => p.packageBase.DayLimitMin);
                    minSellLimit = minSellLimit == 0 ? 1 : minSellLimit;
                    CheckOut = CheckIn.AddDays(minSellLimit);
                    break;
                }

                pis = ServiceList.HotelService.GetJLHotelPackages(hotelid, CheckIn, CheckOut, ppt);

            }

            //按是否可销售、价格来排序
            return pis.OrderBy(p => p.SellState).ThenBy(p => p.Price).ToList();
        }

        //private void SetRoomOptions(PackageInfoEntity p, DateTime CheckIn)
        //{
        //    List<PRoomOptionsEntity> Items = p.RoomOptions.Where(r => r.DateType == 8 && r.Date == CheckIn).ToList();

        //    if (Items == null || Items.Count == 0) //如果不是指定日期条目，取指定星期条目
        //    {
        //        int weekday = (int)CheckIn.DayOfWeek;
        //        weekday = weekday == 0 ? 7 : weekday; //星期日=0，需要处理。 数据库定义中0为任意时间
        //        Items = p.RoomOptions.Where( r=>r.DateType == weekday).ToList();

        //        if (Items == null || Items.Count == 0)//取平日价格
        //            Items = p.RoomOptions.Where(r => r.DateType == 0).ToList();

        //    }

        //    if (Items.Count > 0)
        //    {
        //        p.Room.Options = Items.First().Options;
        //        p.Room.DefaultOption = Items.First().DefaultOption;
        //    }

        //}

        /// <summary>
        /// 判断套餐是所选日期是否可以销售
        /// </summary>
        /// <param name="pdis"></param>
        /// <param name="p"></param>
        /// <param name="CheckIn"></param>
        /// <param name="CheckOut"></param>
        /// <returns>1: 可以销售 2：不可销售 3：日期限止，不能销售</returns>
        //private int CheckPackageSealState(List<PDayItem> pdis, PackageEntity package, DateTime CheckIn, DateTime CheckOut)
        //{
        //    int intResult = 1;
        //    if (package.StartDate > CheckIn)
        //    {
        //        intResult = 2;
        //    }
        //    else if (pdis.Count == 0)
        //    {
        //        intResult = 2;
        //    }
        //    else
        //    {
        //        int PID = package.ID;
        //        int startIndex = CommMethods.GetNightCount(pdis.First().Day, CheckIn);
        //        int nightCount = CommMethods.GetNightCount(CheckIn, CheckOut);


        //        package.PackageCount = 10000;
        //        for (int i = 0; i < nightCount; i++)
        //        {
        //            PDayItem di = pdis[startIndex + i];
        //            if (di.PItems.Where(p => p.PID == PID).Count() > 0)
        //            {
        //                PDayPItem pitem = di.PItems.Where(p => p.PID == PID).First();
        //                int canSellCount = pitem.MaxSealCount - pitem.SoldCount;
        //                if (canSellCount <= 0)
        //                {
        //                    intResult = 2;
        //                    package.PackageCount = 0;
        //                    break;
        //                }
        //                //get the min canSellCount
        //                package.PackageCount = package.PackageCount > canSellCount ? canSellCount : package.PackageCount;
        //            }
        //            else
        //            {
        //                intResult = 2;
        //                package.PackageCount = 0;
        //                break;
        //            }
        //        }

        //        //处理套餐间夜限止
        //        if ((package.DayLimitMin > 0 && nightCount < package.DayLimitMin)
        //             || (package.DayLimitMax > 0 && nightCount > package.DayLimitMax))
        //        {
        //            intResult = 3;
        //        }

        //    }
        //    return intResult;
        //}

        private int GetMarketPrice(PackageInfoEntity p, DateTime CheckIn, DateTime CheckOut)
        {
            return 1000;
            //int NightCount = CommMethods.GetNightCount(CheckIn, CheckOut);

            //if (NightCount <= 0)
            //    return 0;

            //int Price = 0;

            //for (int i = 0; i < NightCount; i++)
            //{
            //    Price += GetMarketPrice(p, CheckIn.AddDays(i));
            //}

            //return Price ;

        }
        //private int GetMarketPrice(PackageInfoEntity p, DateTime CheckIn)
        //{
        //    int ItemPrice = p.Items.Where(i => i.Type == 1).Sum(i => i.Price) ?? 0; //价格类型

        //    int RoomPrice = 0; //房型价格
        //    //先取OTA的价格，如果没有OTA价格则取编辑录入的价格。
        //    if (p.Room.RelOTAID != null && p.Room.RelOTAID > 0) //先判断是否匹配了OTA房型
        //    {
        //        List<HotelPriceInfo2> hps = PriceService.QueryHotelRealTimePrice2(p.Room.HotelID ?? 0, CheckIn, 1);

        //        HotelPriceInfo2 hp = hps.Where(h => h.ChannelID == p.Room.RelOTAID).FirstOrDefault();

        //        if (hp != null && hp.ChannelID > 0) // 获取OTA对应房型价格 需要价格服务完成改造
        //        {
        //           RoomRate rr =  hp.RoomRates.Where(r => r.RoomID == p.Room.RelOTARoomID).FirstOrDefault();
        //           if (rr != null && rr.RoomID > 0)
        //               RoomPrice = (int) rr.Price;
        //        }
        //    }

        //    if (RoomPrice == 0)
        //    {
        //        if (CommMethods.IsFestival(CheckIn))
        //            RoomPrice = p.Room.FestivalPrice;
        //        else if (CommMethods.IsWeekend(CheckIn))
        //            RoomPrice = p.Room.WeekendDayPrice ?? 0;
        //        else
        //            RoomPrice = p.Room.WeekDayPrice ?? 0;
        //    }

        //    return ItemPrice + RoomPrice;
        //}



        //private void  CalcPackagePrice(PackageInfoEntity p, DateTime CheckIn, DateTime CheckOut)
        //{
        //    int NightCount = CommMethods.GetNightCount(CheckIn, CheckOut);

        //    if (NightCount <= 0)
        //        return ;

        //    int Price = 0;
        //    int Rebate = 0;
        //    int PayType = 0;

        //    for (int i = 0; i < NightCount; i++)
        //    {
        //        PackageDailyEntity dItem = new PackageDailyEntity();
        //        dItem.Day = CheckIn.AddDays(i);
        //        CalcPackagePrice(dItem,p, CheckIn.AddDays(i));
        //       dItem.Items = CalcPackageDailyItems(p, CheckIn.AddDays(i));
        //         Price += dItem.Price;
        //         Rebate += dItem.Rebate;
        //         PayType = PayType < dItem.PayType ? dItem.PayType : PayType;
        //        p.DailyItems.Add(dItem);
        //    }

        //    p.Price = Price ;
        //    p.Rebate = Rebate;
        //    p.PayType = PayType;
        //}

        //private List<PItemEntity> CalcPackageDailyItems(PackageInfoEntity p, DateTime CheckIn)
        //{
        //    //取指定日期条目
        //    List<PItemEntity> Items = p.Items.Where(r => r.Type == 1 && r.DateType == 8 && r.Date == CheckIn).ToList();

        //    if (Items == null || Items.Count == 0) //如果不是指定日期条目，取指定星期条目
        //    {
        //        int weekday = (int)CheckIn.DayOfWeek;
        //        weekday = weekday == 0 ? 7 : weekday; //星期日=0，需要处理。 数据库定义中0为任意时间
        //        Items = p.Items.Where(r => r.Type ==1 && r.DateType == weekday).ToList();

        //        if (Items == null || Items.Count == 0)//取平日价格
        //            Items = p.Items.Where(r => r.Type == 1 && r.DateType == 0).ToList();

        //    }

        //    return Items;
        //}

        //private void CalcPackagePrice(PackageDailyEntity dItem,PackageInfoEntity p, DateTime CheckIn)
        //{

        //    //取指定日期价格
        //    PRateEntity rate =GetPackageRateByDate(p,CheckIn);

        //    dItem.Price = rate == null? -1 : rate.Price ?? -1;
        //    dItem.Rebate = rate == null ? 0 : rate.Rebate;
        //    dItem.PayType = rate == null ? 3 : rate.PayType;

        //}

        ///// <summary>
        ///// 获取套餐与指定日期相关的价格记录
        ///// </summary>
        ///// <param name="p"></param>
        ///// <param name="CheckIn"></param>
        ///// <returns></returns>
        //public PRateEntity GetPackageRateByDate(PackageInfoEntity p,  DateTime CheckIn)
        //{  //取指定日期价格
        //    PRateEntity rate = p.Rates.Where(r => r.Type == 8 && r.Date == CheckIn).FirstOrDefault();

        //    if (rate == null || rate.ID == 0) //如果不是指定日期价格，取指定星期价格
        //    {
        //        int weekday = (int)CheckIn.DayOfWeek;
        //        weekday = weekday == 0 ? 7 : weekday; //星期日=0，需要处理。 数据库定义中0为任意时间
        //        rate = p.Rates.Where(r => r.Type == weekday).FirstOrDefault();

        //        if (rate == null || rate.ID == 0)//如果不是指定星期价格，取平日价格
        //            rate = p.Rates.Where(r => r.Type == 0).FirstOrDefault();

        //    }

        //    return rate;
        //}

        /// <summary>
        /// 获取酒店套餐可销售状态
        /// </summary>
        /// <param name="hotelid"></param>
        /// <param name="startDate"></param>
        /// <returns></returns>
        public static List<PDayItem> GetHotelPackageCalendar(int hotelid, DateTime startDate)
        {
            return ServiceList.HotelService.GetHotelPackageCalendar(hotelid, startDate);
        }

        //public static List<PackageInfoEntity> GetHotelPackages(int hotelid, int pid = 0, int terminalType = 1)
        //{
        //    return HotelService.GetHotelPackages(hotelid, pid, terminalType);
        //}

        public static List<PDayItem> GetHotelPackageCalendar30(int hotelid, DateTime startDate, int pid = 0, long userId = 0, int ChannelID = 0)
        {
            HJDAPI.Common.Helpers.Enums.CustomerType customerType = AccountAdapter.GetCustomerType(userId);
            return GetHotelPackageCalendarWithCustomerType(hotelid, startDate, customerType, pid, ChannelID);
        }

        public static List<PDayItem> GetHotelPackageCalendarWithCustomerType(int hotelid, DateTime startDate, HJDAPI.Common.Helpers.Enums.CustomerType customerType, int pid = 0, int ChannelID = 0)
        {
            HotelServiceEnums.PricePolicyType ppt = PriceAdapter.TransCustomerType2PricePolicyType(customerType);
            return ServiceList.HotelService.GetHotelPackageCalendar30(hotelid, startDate, pid, ChannelID, ppt);
        }

        /// <summary>
        /// 缓存价格日历
        /// </summary>
        /// <param name="hotelid"></param>
        /// <param name="startDate"></param>
        /// <param name="customerType"></param>
        /// <param name="pid"></param>
        /// <param name="ChannelID"></param>
        /// <returns></returns>
        public static List<PDayItem> GetHotelPackageCalendarCachedWithCustomerType(int hotelid, DateTime startDate, HJDAPI.Common.Helpers.Enums.CustomerType customerType, int pid = 0, int ChannelID = 0)
        {
            HotelServiceEnums.PricePolicyType ppt = PriceAdapter.TransCustomerType2PricePolicyType(customerType);
            return ServiceList.HotelService.GetHotelPackageCalendar30Cached(hotelid, startDate, pid, ChannelID, ppt);
        }

        /// <summary>
        /// 得到房券可售日
        /// </summary>
        /// <param name="hvid">房券id</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns></returns>
        public static List<PDayItem> GetHotelVoucherCalendar(int hvid, DateTime startDate, DateTime endDate)
        {
            return ServiceList.HotelService.GenHotelVoucherCalendar(hvid, startDate, endDate);
        }
        /// <summary>
        /// 包房酒店列表
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static List<int> GetPackRoomHotelIdList(DateTime date)
        {
            return ServiceList.HotelService.GetPackRoomHotelIdList(date);
        }

        /// <summary>
        /// 获取单个套餐
        /// </summary>
        /// <param name="pId"></param>
        /// <returns></returns>
        public static HJD.HotelServices.Contracts.PackageEntity GetOnePackageEntity(int pId)
        {
            return ServiceList.HotelService.GetOnePackageEntity(pId);
        }
    }
}