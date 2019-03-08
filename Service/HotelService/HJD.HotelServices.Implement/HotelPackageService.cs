using HJD.Framework.Interface;
using HJD.Framework.WCF;
using HJD.HotelServices.Contracts;
using HJD.HotelServices.Implement;
using HJD.HotelServices.Implement.Business;
using HJD.HotelServices.Implement.Entity;
using HJD.HotelServices.Implement.Helper;
using HJD.HotelServices.Implement.Helper.Channel.Ctrip;
using HJD.OtaCrawlerService.Contract.Ctrip.Hotel;
using HJD.OtaCrawlerService.Contract.Params;
using HJD.OtaCrawlerServices.Contract;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HJD.HotelServices
{
    public partial class HotelService : IHotelService
    {
        private static IMemcacheProvider memCacheHotelCtripPackage = MemcacheManagerFactory.Create("HotelCtripPackageCache");
        private static IMemcacheProvider memCacheHolidays = MemcacheManagerFactory.Create("HolidaysCache");
        private static IMemcacheProvider memCacheCanlendar = MemcacheManagerFactory.Create("HolidaysCache");
        private static IMemcacheProvider memHotelPricePlanCache = MemcacheManagerFactory.Create("HotelPricePlanCache");
        private static IMemcacheProvider memCacheHotelPriceUpdate = MemcacheManagerFactory.Create("HotelPriceUpdateCache");
        public static IOtaCrawlerService otaCrawlerService = ServiceProxyFactory.Create<IOtaCrawlerService>("IOtaCrawlerService");

        public const int DEFAULT_ROOM_BED_COUNT = 200; //缺省的酒店每天的床数


        /// <summary>
        /// 这里添加方法注释
        /// </summary>
        public int AddTravelPerson(TravelPersonEntity travelperson)
        {
            return HotelDAL.AddTravelPerson(travelperson);
        }
        /// <summary>
        /// 这里添加方法注释
        /// </summary>
        public int UpdateTravelPerson(TravelPersonEntity travelperson)
        {
            return HotelDAL.UpdateTravelPerson(travelperson);
        }
        /// <summary>
        /// 这里添加方法注释
        /// </summary>
        public TravelPersonEntity GetTravelPersonById(int Id)
        {
            return HotelDAL.GetTravelPersonById(Id);
        }
        /// <summary>
        /// 这里添加方法注释
        /// </summary>
        public List<TravelPersonEntity> GetTravelPersonByUserId(long userId)
        {
            return HotelDAL.GetTravelPersonByUserId(userId);
        }

        public List<TravelPersonEntity> GetTravelPersonByIds(string Ids)
        {
            return HotelDAL.GetTravelPersonByIds(Ids);
        }

        /// <summary>
        /// 这里添加方法注释
        /// </summary>
        public bool DeleteTravelPerson(int id)
        {
            return HotelDAL.DeleteTravelPerson(id);
        }

        public List<ChannelInfoEntity> GetAllChannelInfoList()
        {
            return HotelDAL.GetAllChannelInfoList();
        }
     

        public List<HotelTop1PackageInfoEntity> GetHotelTop1PackageInfo(IEnumerable<int> HotelIDList, DateTime CheckIn, DateTime CheckOut)
        {
            return HotelDAL.GetHotelTop1PackageInfo(HotelIDList, CheckIn, CheckOut);
        }
        public List<SpecialDealPackageEntity> GetSpecialDealPackage()
        {
            return HotelDAL.GetSpecialDealPackages();
        }

        public List<CanSaleHotelInfoEntity> GetAllCanSellPackage(DateTime startDate, DateTime endDate, string tag = "")
        {
            //if (startDate.Date == DateTime.Parse("2017-01-27"))
            {
                try
                {
                    LogHelper.WriteLog(string.Format("start:【{0}】{1}", startDate, DateTime.Now));
                }
                catch (Exception ex)
                {
                    
                }
            }

            //return GenAllCanSellPackage(startDate, endDate, tag);
            string key = GenAllCanSellPackageCacheKey(startDate, endDate, tag); 
            return memCacheHolidayPackage.GetData<List<CanSaleHotelInfoEntity>>(key,
                () =>
                {
                        try
                        {
                            LogHelper.WriteLog(string.Format("GenAllCanSellPackage:{0} ", key));
                        }
                        catch (Exception ex)
                        {

                        } 

                    return GenAllCanSellPackage(startDate, endDate, tag);
                });
        }


        public List<CanSaleHotelInfoEntity> GenAllCanSellPackage(DateTime startDate, DateTime endDate, string tag, bool getOta = false)
        {
            List<CanSaleHotelInfoEntity> canSaleHotelInfoList = new List<CanSaleHotelInfoEntity>();


            if (tag.Length == 0) //如果不是找已标记的酒店，那么需要计算可捷旅酒店
            {
                canSaleHotelInfoList = HotelDAL.GetJLTourValidPackageHotel(startDate, endDate);
            }

            List<int> pHotelidList = HotelDAL.GetValidPackageHotelID(startDate, endDate, tag);

            //LogHelper.WriteLog( string.Format("{0}:{1}:{2}:{3}", startDate, endDate,tag ,string.Join(",", pHotelidList)));GetAllCanSellPackage20160930:20161001:1

            foreach (int hotelid in pHotelidList)
            {
                HotelItem hi = GetHotel(hotelid);

                for (DateTime curDate = startDate.Date; curDate <= endDate; curDate = curDate.AddDays(1))
                {
                    List<PackageInfoEntity> lp = new List<PackageInfoEntity>();

                    try
                    {
                        //查询该酒店的单天入住套餐
                        lp = GetHotelPackageList(hotelid, curDate, curDate.AddDays(1));
                    }
                    catch (Exception e)
                    {
                        LogHelper.WriteLog(e.Message);
                    }

                    PackageInfoEntity _packageEntity = null;
                    if (lp != null && lp.Count > 0)
                    {
                        if (lp.Where(p => p.SellState == 1).Count() > 0)
                        {
                            _packageEntity = lp.Where(p => p.SellState == 1).OrderBy(p => p.VIPPrice).First();
                        }
                        else
                        {
                            //如果单天查不到套餐，则查询是不是存在多天入住的套餐
                            var _firstPackage = lp[0];
                            if (_firstPackage.packageBase.DayLimitMin > 1)
                            {
                                //查询多天套餐
                                var lp2 = GetHotelPackageList(hotelid, curDate, curDate.AddDays(_firstPackage.packageBase.DayLimitMin));
                                if (lp2 != null && lp2.Count > 0 && lp2.Where(p => p.SellState == 1).Count() > 0)
                                {
                                    _packageEntity = lp2.Where(p => p.SellState == 1).OrderBy(p => p.VIPPrice).First();
                                }
                                else if (lp.Count > 1)
                                {
                                    _firstPackage = lp[1];
                                    if (_firstPackage.packageBase.DayLimitMin > 1)
                                    {
                                        //查询多天套餐
                                        var lp3 = GetHotelPackageList(hotelid, curDate, curDate.AddDays(_firstPackage.packageBase.DayLimitMin));
                                        if (lp3 != null && lp3.Count > 0 && lp3.Where(p => p.SellState == 1).Count() > 0)
                                        {
                                            _packageEntity = lp3.Where(p => p.SellState == 1).OrderBy(p => p.VIPPrice).First();
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (getOta && _packageEntity == null && lp.Where(p => p.SellState == 1).Count() == 0)
                    {
                        //LogHelper.WriteLog(string.Format("begin GetCtripHotelPackages:{0} {1} {2}", hotelid, curDate, curDate.AddDays(1)));
                        lp = GetCtripHotelPackagesForApiV42(hotelid, curDate, curDate.AddDays(1), false);
                        //LogHelper.WriteLog(string.Format("end GetCtripHotelPackages:{0} {1} {2}", hotelid, curDate, curDate.AddDays(1)));

                        if (lp != null && lp.Count > 0 && lp.Where(p => p.SellState == 1).Count() > 0)
                        {
                            _packageEntity = lp.Where(p => p.SellState == 1).OrderBy(p => p.Price).First();
                        }
                    }

                    if (_packageEntity != null)
                    {
                        CanSaleHotelInfoEntity can = new CanSaleHotelInfoEntity()
                        {
                            HotelId = hi.Id,
                            HotelName = hi.Name,
                            DistrictID = hi.DistrictId,
                            DistrictName = hi.DistrictName,
                            ProvinceName = (hi.ProvinceName == "中国" ? hi.DistrictName : hi.ProvinceName)
                        };
                        can.Night = curDate.Date;
                        can.DayLimitMin = (_packageEntity.packageBase.DayLimitMin > 0 ? _packageEntity.packageBase.DayLimitMin : 1);
                        can.Qtyable = 1;
                        can.PackageBrief = _packageEntity.packageBase.Brief;
                        can.PackageIsInBenefitArea = _packageEntity.packageBase.InBenefitArea;

                        //price info
                        can.Businessprice = 0;
                        can.VipPrice = 0;
                        if (_packageEntity.Price > 0)
                        {
                            can.Businessprice = _packageEntity.Price;
                            can.VipPrice = _packageEntity.VIPPrice;
                        }

                        canSaleHotelInfoList.Add(can);
                    }
                }
            }

            return canSaleHotelInfoList;
        }

        public string GenAllCanSellPackageCacheKey(DateTime startDate, DateTime endDate, string tag = "")
        {
            return string.Format("GetAllCanSellPackage{0},{1},{2}", startDate.Date.ToString("yyyy.MM.dd"), endDate.Date.ToString("yyyy.MM.dd"), tag);
        }

        public bool CreateAllCanSellPackage(DateTime startDate, DateTime endDate, string tag = "")
        {
            //LogHelper.WriteLog(string.Format("begin GetCtripHotelPackages:{0} ", startDate));

            string key = GenAllCanSellPackageCacheKey( startDate,  endDate,  tag );

            List<CanSaleHotelInfoEntity> list = GenAllCanSellPackage(startDate, endDate, tag, true);
            //return GenAllCanSellPackage(startDate, endDate, tag);

            try
            {
                LogHelper.WriteLog(string.Format("CreateAllCanSellPackage:{0} ", key));
            }
            catch (Exception ex)
            {
                
            }

            memCacheHolidayPackage.Remove(key);
            var _data = memCacheHolidayPackage.GetData<List<CanSaleHotelInfoEntity>>(key,
            () =>
            {
                return list;
            });

            //LogHelper.WriteLog(string.Format("end GetCtripHotelPackages:{0} ", startDate));

            return true;
        }


        public List<CanSaleHotelInfoEntity> OfflineGenAllCanSellPackage(DateTime startDate, DateTime endDate, string tag)
        {
            List<CanSaleHotelInfoEntity> canSaleHotelInfoList = new List<CanSaleHotelInfoEntity>();


            if (tag.Length == 0) //如果不是找已标记的酒店，那么需要计算可捷旅酒店
            {
                canSaleHotelInfoList = HotelDAL.GetJLTourValidPackageHotel(startDate, endDate);
            }

            List<int> pHotelidList = HotelDAL.GetValidPackageHotelID(startDate, endDate, tag);

            //   LogHelper.WriteLog( string.Format("{0}:{1}:{2}:{3}", startDate, endDate,tag ,string.Join(",", pHotelidList)));

            foreach (int hotelid in pHotelidList)
            {
                HotelItem hi = GetHotel(hotelid);

                for (DateTime curDate = startDate.Date; curDate <= endDate; curDate = curDate.AddDays(1))
                {
                    List<PackageInfoEntity> lp = new List<PackageInfoEntity>();

                    try
                    {
                        lp = GetHotelPackageList(hotelid, curDate, curDate.AddDays(1));

                        if (lp.Where(p => p.SellState == 1).Count() == 0)
                        {
                            //  LogHelper.WriteLog(string.Format("begin GetCtripHotelPackages:{0} {1} {2}",hotelid, curDate, curDate.AddDays(1)));
                            lp = GetCtripHotelPackages(hotelid, curDate, curDate.AddDays(1), false);
                            //   LogHelper.WriteLog(string.Format("end GetCtripHotelPackages:{0} {1} {2}",hotelid, curDate, curDate.AddDays(1)));
                        }
                    }
                    catch (Exception e)
                    {
                        LogHelper.WriteLog(e.Message);
                    }



                    if (lp.Count > 0)
                    {
                        var canSalePackages = lp.Where(p => p.SellState == 1);
                        if (canSalePackages.Count() > 0)
                        {

                            CanSaleHotelInfoEntity can = new CanSaleHotelInfoEntity()
                            {
                                HotelId = hi.Id,
                                HotelName = hi.Name,
                                DistrictID = hi.DistrictId
                            };
                            can.Night = curDate.Date;

                            can.Qtyable = 1;
                            can.Businessprice = canSalePackages.Min(p => p.Price);
                            can.PackageBrief = canSalePackages.OrderBy(p => p.Price).First().packageBase.Brief; 

                            canSaleHotelInfoList.Add(can);
                        }
                    }
                }
            }



            return canSaleHotelInfoList;
        }

        //private int canlendarLength = 60;  //日历返回的天数

        public List<SimplePackageEntity> GetSimplePackageInfo(string packageIDs)
        {
            return HotelDAL.GetSimplePackageInfo(packageIDs);
        }

        /// <summary>
        /// 从memcache获取套餐价格 10min钟缓存
        /// </summary>
        /// <param name="hotelid"></param>
        /// <param name="CheckIn"></param>
        /// <param name="CheckOut"></param>
        /// <param name="ppt"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        public List<PackageRateEntity> GetHotelPackageRateList(int hotelid, DateTime CheckIn, DateTime CheckOut, HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType ppt = HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType.Default, int pid = 0)
        {
            //return memCacheHotelPriceUpdate.GetData<List<PackageRateEntity>>(string.Format("GetHotelPackageRateList_{0}_{1}_{2}_{3}_{4}", hotelid, CheckIn.ToString("yyyy-MM-dd"), CheckOut.ToString("yyyy-MM-dd"), (int)ppt, pid),
            //    () =>
            //    {
            return GenHotelPackageRateList(hotelid, CheckIn, CheckOut, ppt, pid);
            //});
        }

        /// <summary>
        /// 获取一个酒店所有套餐在某天的价格, 返回价格以元计算。
        /// </summary>
        /// <param name="hotelid"></param>
        /// <param name="CheckIn"></param>
        /// <param name="CheckOut"></param>
        /// <returns></returns>
        public List<PackageRateEntity> GenHotelPackageRateList(int hotelid, DateTime CheckIn, DateTime CheckOut, HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType ppt = HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType.Default, int pid = 0)
        {
            CheckIn = CheckIn.Date;
            CheckOut = CheckOut.Date;
            List<PackageRateEntity> list = new List<PackageRateEntity>();
            //LogHelper.WriteLog("GenHotelPackageRateList" + CheckIn + "hotelid=" + hotelid + "   CheckOut=" + CheckOut);
            try
            {

                List<HotelPackageRateItemEntity> itemList = HotelDAL.GetHotelPackageRate(hotelid,pid);

                ////将元转换成分
                //foreach(var i in itemList)
                //{
                //    i.ActiveRebate *= 100;
                //    i.CanUseCashCoupon *= 100;
                //    i.CashCoupon *= 100;
                //    i.ManualActiveRebate *= 100;
                //    i.ManualCanUseCashCoupon *= 100;
                //    i.ManualCashCoupon *= 100;
                //    i.Price *= 100;
                //    i.Rebate *= 100;
                //    i.SettlePrice *= 100;
                //    i.ShowPrice *= 100;               
                //}

                var pidList = pid == 0 ? itemList.Select(o => o.PackageID).Distinct() : new int[1] { pid };
                foreach (int packageID in pidList)
                {
                    PackageRateEntity p = new PackageRateEntity { HotelID = hotelid, startDate = CheckIn, endDate = CheckOut, PID = packageID };//I

                    var pitemList = itemList.Where(o => o.PackageID == packageID);
                    var productList = pitemList.Select(o => new { o.SupplierType, o.SupplierID, o.PackageID, o.ProductID }).Distinct();

                    for (int i = 0; CheckIn.AddDays(i).Date < CheckOut.Date; i++)
                    {
                        var startDate = CheckIn.AddDays(i).Date;
                        var endDate = startDate.AddDays(1).Date;

                        //    PackageRateEntity p = new PackageRateEntity { HotelID = hotelid, startDate = startDate, endDate = endDate, PID = packageID };//I-1

                        PackageDailyRateEntity pdr = new PackageDailyRateEntity { Day = startDate };

                        foreach (var product in productList)
                        {
                            PackageRateItemEntity pr = new PackageRateItemEntity { ProductID = product.ProductID, SupplierID = product.SupplierID, SupplierType = product.SupplierType };

                            var productItemList = pitemList.Where(o => o.ProductID == pr.ProductID && o.SupplierID == pr.SupplierID && o.SupplierType == pr.SupplierType);

                            HotelPackageRateItemEntity rightItem = null;

                            //取指定日期条目
                            var specialDayItem = productItemList.Where(o => o.DateType == 8 && o.Date == pdr.Day);

                            if (specialDayItem.Count() > 0)
                            {
                                rightItem = specialDayItem.First();
                            }
                            else
                            {
                                int weekday = (int)pdr.Day.DayOfWeek;
                                weekday = weekday == 0 ? 7 : weekday; //星期日=0，需要处理。 数据库定义中0为任意时间
                                var weekdayItems = productItemList.Where(r => r.DateType == weekday);

                                if (weekdayItems.Count() > 0)
                                {
                                    rightItem = weekdayItems.First();
                                }
                                else
                                {
                                    var normaldayItems = productItemList.Where(r => r.DateType == 0);
                                    if (normaldayItems.Count() > 0)
                                        rightItem = normaldayItems.First();
                                }
                            }

                            if (rightItem != null)
                            {
                                //LogHelper.WriteLog("GenHotelPackageRateList  rightItem.AutoCommission=" + rightItem.AutoCommission + "  ManualCommission=" + rightItem.ManualCommission);
                                pr.Price = rightItem.Price;
                                pr.VIPPrice = rightItem.ManualVIPPrice == -1 ? rightItem.VIPPrice : rightItem.ManualVIPPrice;
                                pr.SettlePrice = rightItem.SettlePrice;
                                pr.Rebate = rightItem.Rebate;
                                pr.PayType = rightItem.PayType;
                                pr.ActiveRebate = rightItem.ManualActiveRebate == -1 ? rightItem.ActiveRebate : rightItem.ManualActiveRebate;
                                pr.CanUseCashCoupon = rightItem.ManualCanUseCashCoupon == -1 ? rightItem.CanUseCashCoupon : rightItem.ManualCanUseCashCoupon;
                                pr.CashCoupon = rightItem.ManualCashCoupon == -1 ? rightItem.CashCoupon : rightItem.ManualCashCoupon;
                                pr.CanUseCashCouponForBoTao = rightItem.ManualCanUseCashCoupon == -1 ? rightItem.CanUseCashCouponForBoTao : 0;
                                pr.AutoCommission = rightItem.AutoCommission;
                                pr.ManualCommission = rightItem.ManualCommission == -1 ? rightItem.AutoCommission : rightItem.ManualCommission;
                                pdr.ItemList.Add(pr);
                            }
                        }

                        pdr.Price = pdr.ItemList.Sum(o => o.Price);
                        pdr.VIPPrice = pdr.ItemList.Sum(o => o.VIPPrice);
                        pdr.SettlePrice = pdr.ItemList.Sum(o => o.SettlePrice);
                        pdr.Rebate = pdr.ItemList.Sum(o => o.Rebate);

                        pdr.AutoCommission = pdr.ItemList.Sum(o=>o.AutoCommission);
                        pdr.ManualCommission = pdr.ItemList.Sum(o => o.ManualCommission);
                        //LogHelper.WriteLog("GenHotelPackageRateList  pdr.AutoCommission=" + pdr.AutoCommission + " pdr ManualCommission=" + pdr.ManualCommission);
                        if (pdr.Rebate > 0) //如果有返现，那么不能用现金券
                        {
                            pdr.ActiveRebate = 0;
                            pdr.CanUseCashCoupon = 0;
                            pdr.CashCoupon = 0;
                            pdr.CanUseCashCouponForBoTao = 0;//加博涛
                        }
                        else
                        {
                            pdr.ActiveRebate = pdr.ItemList.Sum(o => o.ActiveRebate);
                            pdr.CanUseCashCoupon = pdr.ItemList.Sum(o => o.CanUseCashCoupon);
                            pdr.CashCoupon = pdr.ItemList.Sum(o => o.CashCoupon);
                            pdr.CanUseCashCouponForBoTao = pdr.ItemList.Sum(o => o.CanUseCashCouponForBoTao);
                        }
                        pdr.PayType = pdr.ItemList.Max(o => o.PayType);

                        p.DailyList.Add(pdr);

                        #region II-2
                        //p.Price = p.DailyList.Sum(o => o.Price);
                        //p.SettlePrice = p.DailyList.Sum(o => o.SettlePrice);
                        //p.Rebate = p.DailyList.Sum(o => o.Rebate);
                        //p.ActiveRebate = p.DailyList.Sum(o => o.ActiveRebate);
                        //p.CanUseCashCoupon = p.DailyList.Sum(o => o.CanUseCashCoupon);
                        //p.CanUseCashCouponForBoTao = p.DailyList.Sum(o => o.CanUseCashCouponForBoTao);
                        //p.CashCoupon = p.DailyList.Sum(o => o.CashCoupon);
                        //p.PayType = p.DailyList.Max(o => o.PayType);

                        //list.Add(p);
                        #endregion
                    }
                    #region II
                    p.Price = p.DailyList.Sum(o => o.Price);
                    p.VIPPrice = p.DailyList.Sum(o => o.VIPPrice);
                    p.SettlePrice = p.DailyList.Sum(o => o.SettlePrice);
                    p.Rebate = p.DailyList.Sum(o => o.Rebate);
                    p.ActiveRebate = p.DailyList.Sum(o => o.ActiveRebate);
                    p.CanUseCashCoupon = p.DailyList.Sum(o => o.CanUseCashCoupon);
                    p.CanUseCashCouponForBoTao = p.DailyList.Sum(o => o.CanUseCashCouponForBoTao);
                    p.CashCoupon = p.DailyList.Sum(o => o.CashCoupon);
                    p.PayType = p.DailyList.Count> 0? p.DailyList.Max(o => o.PayType) : 3;

                    p.AutoCommission = p.DailyList.Sum(o => o.AutoCommission);
                    p.ManualCommission = p.DailyList.Sum(o => o.ManualCommission);

                    //LogHelper.WriteLog("GenHotelPackageRateList  p.AutoCommission=" + p.AutoCommission + " p ManualCommission=" + p.ManualCommission);

                    list.Add(p);
                    #endregion
                }

                AddRatePricePolicy(list, ppt, CheckIn, HotelServiceEnums.PackageType.HotelPackage);
            }
            catch (Exception e)
            {
                LogHelper.WriteLog(string.Format("GetHotelPackageRateList Err：hotelid:{0}   CheckIn:{1}, CheckOut:{2} {3} {4}" , hotelid,CheckIn,  CheckOut , e.Message , e.StackTrace));
            }
            return list;
        }

        public bool IsPackRoomSupplierByHotelID_PID(int hotelID, int pid)
        {
            bool bResult = false;
            List<PackageEntity> PS = HotelDAL.GetPackage(hotelID, pid);
            if (PS.Count > 0)
            {
                bResult = PS.First().SupplierType == 2; //房间供应商类型是1代表普通房间供应商 2代表包房供应商
            }

            return bResult;

        }

        /// <summary>
        /// 订单对应包房供应商 房型和床型可用状态
        /// </summary>
        /// <param name="hotelID"></param>
        /// <param name="checkIn"></param>
        /// <param name="nightCount"></param>
        /// <param name="roomCount"></param>
        /// <param name="pid"></param>
        /// <param name="note"></param>
        /// <returns></returns>
        public List<PackRoomBedStateEntity20> GetCanUsePackRoomSupplier(int hotelID, DateTime checkIn, int nightCount, int pid, int BigBedCount, int TwinBedCount, int roomSupplierID = 0)
        {
            PackageEntity peInfo = null;
            List<PackageEntity> PS = HotelDAL.GetPackage(hotelID, pid);
            peInfo = PS.Count > 0 ? PS.First() : null;

            if (peInfo == null)
            {
                return new List<PackRoomBedStateEntity20>();
            }

            int RoomID = peInfo.RoomID;


            DateTime checkOut = checkIn.AddDays(nightCount);
            //计算该包房供应商销售掉房间的数量
            IEnumerable<RoomSouldCountEntity> soldPackRoomCount = GetHotelPackRoomSouldInfo(hotelID, checkIn, checkOut);//service方法内部给checkOut已经减了1天

            //获得包房供应商所有的数量
            List<PackRoomBedStateEntity20> rbs = HotelDAL.GetPackRoomBedState(hotelID, checkIn, checkOut.AddDays(-1));//床态不含checkOut那天

            List<PackRoomBedStateEntity20> supplierRoomCanUseNumber = new List<PackRoomBedStateEntity20>();

            List<int> packRoomSupplierIDList = rbs != null && rbs.Count > 0 ? rbs.Select(r => r.PackRoomSupplierID).Distinct().ToList() : new List<int>();
            List<int> NotEnoughSupplierIDList = new List<int>();
            foreach (var curRoomSupplierID in packRoomSupplierIDList)
            {

                if (roomSupplierID > 0 && roomSupplierID != curRoomSupplierID) continue;

                IEnumerable<RoomSouldCountEntity> supplierSoldPackRoomCount = soldPackRoomCount != null && soldPackRoomCount.Count() > 0 ?
              soldPackRoomCount.Where(_ => _.RoomSupplierID == curRoomSupplierID && _.RoomID == RoomID) : new List<RoomSouldCountEntity>();


                List<PackRoomBedStateEntity20> supplierRBS = rbs.FindAll(_ => _.PackRoomSupplierID == curRoomSupplierID && _.RoomID == RoomID);

                //计算checkIn到checkOut期间  包房数量是否充足 如果不充足则报警 充足则继续判断下一个日期
                for (DateTime curDate = checkIn; curDate < checkOut; curDate = curDate.AddDays(1).Date)
                {
                    var curSupplierRBS = supplierRBS != null && supplierRBS.Count > 0 ? supplierRBS.FindAll(_ => _.Date == curDate) : null;//当天一共的包房数量
                    if (curSupplierRBS != null && curSupplierRBS.Count != 0)
                    {
                        var curSoldCount = supplierSoldPackRoomCount.Count() > 0 ? supplierSoldPackRoomCount.Where(_ => _.Date == curDate).FirstOrDefault() : null;//当天已卖出

                        //卖出了包房 大床2 双床1 大床和双床可能都有 两者之和应该等于房间总数
                        int needBigBedCount = BigBedCount + (curSoldCount == null ? 0 : curSoldCount.BigBedSouldCount);
                        int needTwinBedCount = TwinBedCount + (curSoldCount == null ? 0 : curSoldCount.TwinBedSouldCount);

                        //int needCount = roomCount + (curSoldCount != null ? (BigBedCount > 0 ? curSoldCount.BigBedSouldCount: curSoldCount.TwinBedSouldCount) : 0);//对应床型已卖出的数量+当前预订的数量
                        if (needBigBedCount > 0)
                        {
                            foreach (var curSupplier in curSupplierRBS.FindAll(_ => _.BedTypeID == 2).OrderBy(_ => _.CreateTime))
                            {
                                needBigBedCount = needBigBedCount - curSupplier.Count;
                                if (needBigBedCount > 0)
                                {
                                    continue;
                                }
                                else
                                {
                                    PackRoomBedStateEntity20 temp = new PackRoomBedStateEntity20() { BedTypeID = curSupplier.BedTypeID, Count = curSupplier.Count, PackRoomPrice = curSupplier.PackRoomPrice, PackRoomSupplierID = curSupplier.PackRoomSupplierID, Date = curSupplier.Date };
                                    supplierRoomCanUseNumber.Add(temp);
                                    break;
                                }
                            }
                        }
                        if (needTwinBedCount > 0)
                        {
                            foreach (var curSupplier in curSupplierRBS.FindAll(_ => _.BedTypeID == 1).OrderBy(_ => _.CreateTime))
                            {
                                needTwinBedCount = needTwinBedCount - curSupplier.Count;
                                if (needTwinBedCount > 0)
                                {
                                    continue;
                                }
                                else
                                {
                                    PackRoomBedStateEntity20 temp = new PackRoomBedStateEntity20() { BedTypeID = curSupplier.BedTypeID, Count = curSupplier.Count, PackRoomPrice = curSupplier.PackRoomPrice, PackRoomSupplierID = curSupplier.PackRoomSupplierID, Date = curSupplier.Date };
                                    supplierRoomCanUseNumber.Add(temp);
                                    break;
                                }
                            }
                        }

                        if (needBigBedCount > 0 || needTwinBedCount > 0)
                        {
                            NotEnoughSupplierIDList.Add(curRoomSupplierID);
                            break;
                        }
                    }
                    else
                    {
                        NotEnoughSupplierIDList.Add(curRoomSupplierID);
                        break;
                    }
                }
            }

            if (NotEnoughSupplierIDList.Count == 0)
            {
                return supplierRoomCanUseNumber;
            }
            else if (supplierRoomCanUseNumber.Count > 0)
            {
                return supplierRoomCanUseNumber.FindAll(_ => !NotEnoughSupplierIDList.Exists(j => j == _.PackRoomSupplierID));
            }
            else
            {
                return supplierRoomCanUseNumber;
            }
        }

        /// <summary>
        /// 获得酒店套餐房间数量  考虑包房和非包房两种情况
        /// </summary>
        /// <param name="hotelid"></param>
        /// <param name="CheckIn"></param>
        /// <param name="CheckOut"></param>
        /// <returns></returns>
        public List<HotelPackageRoomBedCountEntity> GetHotelPackageRoomBedCount(int hotelid, DateTime CheckIn, DateTime CheckOut)
        {
            List<HotelPackageRoomBedCountEntity> rbdList = HotelDAL.GetHotelPackageRoomBedCount(hotelid, CheckIn, CheckOut.AddDays(-1));//一般的酒店数量


            //对于需要缺省控制床数量的酒店，需要用缺省数量来补足订数量控制列表
            List<PRoomInfoEntity> RoomsInfo = HotelDAL.GetPRoomInfo(hotelid) ;
            List<HotelPackageRoomBedCountEntity> morerbdList = new List<HotelPackageRoomBedCountEntity>();
            foreach (PRoomInfoEntity room in RoomsInfo)
            {
                if (room.DefaultBigBedCount < DEFAULT_ROOM_BED_COUNT || room.DefaultTwinBedCount < DEFAULT_ROOM_BED_COUNT)
                {
                    for (DateTime curDate = CheckIn.Date; curDate < CheckOut; curDate = curDate.AddDays(1).Date)
                    {
                        var rlist = rbdList.Where(r => r.RoomID == room.ID && r.Date == curDate);
                        if (rlist.Count() > 0)
                        {
                            HotelPackageRoomBedCountEntity t = rlist.First();

                            if (t.BigBed == DEFAULT_ROOM_BED_COUNT)
                            {
                                t.BigBed = room.DefaultBigBedCount;
                            }

                            if (t.TwinBed == DEFAULT_ROOM_BED_COUNT)
                            {
                                t.TwinBed = room.DefaultTwinBedCount;
                            }

                            t.RCount = t.BigBed + t.TwinBed;
                        }
                        else
                        {
                            morerbdList.Add(new HotelPackageRoomBedCountEntity
                            {
                                BigBed = room.DefaultBigBedCount,
                                Date = curDate,
                                RCount = room.DefaultBigBedCount + room.DefaultTwinBedCount,
                                RoomID = room.ID,
                                TwinBed = room.DefaultTwinBedCount
                            });
                        }
                    }
                }
            }

            if (morerbdList.Count > 0)
                rbdList.AddRange(morerbdList);

            rbdList = GetAllHotelPackageRoomBedCountEntity(rbdList, hotelid, CheckIn, CheckOut.AddDays(-1));//包房非包房总量

            return rbdList;
        }

        private List<HotelPackageRoomBedCountEntity> GetAllHotelPackageRoomBedCountEntity(List<HotelPackageRoomBedCountEntity> rbdList, int hotelid, DateTime startDate, DateTime endDate)
        {
            List<HotelPackageRoomBedCountEntity> packrbdList = HotelDAL.GetHotelPackRoomBedCount(hotelid, startDate, endDate);//包房的数量一起算

            if (packrbdList != null && packrbdList.Count > 0)
            {
                rbdList = (from i in rbdList select i).
                     Union(from j in packrbdList select j).
                     GroupBy(_ => new { _.Date, _.RoomID }).
                     Select(_ => new HotelPackageRoomBedCountEntity()
                     {
                         RoomID = _.Key.RoomID,
                         Date = _.Key.Date,
                         RCount = _.Sum(j => j.RCount),
                         BigBed = _.Sum(j => j.BigBed),
                         TwinBed = _.Sum(j => j.TwinBed)
                     }
                     ).ToList();
            }

            return rbdList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hotelid"></param>
        /// <param name="CheckIn"></param>
        /// <param name="CheckOut"></param>
        /// <param name="terminalType">访问终端类型 1:IOS, 2:Android 3:Web 4:Other</param>
        /// <returns></returns>
        public List<PackageInfoEntity> GetHotelPackageList(int hotelid, DateTime CheckIn, DateTime CheckOut, int terminalType = 1, HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType ppt = HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType.Default,int pid = 0)
        {
            //  LogHelper.WriteLog(string.Format("GetHotelPackageList:hotelid{0},  CheckIn:{1}, CheckOut:{2},  terminalType:{3}, PricePolicyType:{4}",hotelid,  CheckIn, CheckOut,   terminalType,  ppt.ToString() ));

            //TimeLog log = new TimeLog(string.Format("GetHotelPackageList pid：{0} hotelid:{1} startDate:{2} endDate:{3}", pid, hotelid, CheckIn, CheckOut), 1000, null);
            List<PackageInfoEntity> pis = GetHotelPackages(hotelid, pid, terminalType);
            //log.AddLog("GetHotelPackages");
            if (pis.Count > 0)
            {
                List<PDayItem> pdis = GetHotelPackageCalendar(hotelid, DateTime.Now.Date);
                //log.AddLog("GetHotelPackageCalendar");

                //获取酒店价格列表 价格以元为单位
                List<PackageRateEntity> rateList = GetHotelPackageRateList(hotelid, CheckIn, CheckOut, ppt, pid);
                //log.AddLog("GetHotelPackageRateList");

                //每个套餐床型控制的最小房型数
                List<HotelPackageRoomBedCountEntity> rbdList = GetHotelPackageRoomBedCount(hotelid, CheckIn, CheckOut);
                //log.AddLog("GetHotelPackageRoomBedCount");

                IEnumerable<RoomSouldCountEntity> rsList = GetHotelRoomSouldInfo(hotelid, CheckIn, CheckOut);
                //log.AddLog("GetHotelRoomSouldInfo");

                //int nightCount = (CheckOut - CheckIn).Days;

                int bigBedState = 0;
                int twinBedState = 0; //0:无  1：可售， 2： 不可售
                foreach (PackageInfoEntity p in pis)
                {

                    //if (pdis != null && pdis.Count > 0 && pdis.Exists(_ => _.SellPrice > 0))
                    //{
                    //    var firstDayObj = pdis.First(d => d.SellState == 1);
                    //    var _startDate = firstDayObj.Day;
                    //    for (int i = 0; i < nightCount; i++)
                    //    {
                    //        var _calendarItem = pdis.Find(_ => _.Day == _startDate.AddDays(i));
                    //        p.AutoCommission += _calendarItem.AutoCommission;
                    //        p.ManualCommission += _calendarItem.ManualCommission;
                    //    }
                    //}


                    //所选日期套餐价
                    // CalcPackagePrice(p, CheckIn, CheckOut);
                    SetPackagePrice(p, rateList);

                    //设置套餐项目内容
                    SetDailyItem(p, CheckIn, CheckOut);

                    //所选日期市场价
                    p.MarketPrice = GetMarketPrice(p, CheckIn, CheckOut);

                    //销售状态  1：可售  2：不可售 设置套餐数
                    p.SellState = CheckPackageSealState(pdis, p.packageBase, CheckIn, CheckOut, rbdList, rsList);

                    //设置最后取消日期
                    p.LastCancelTime = GetLastCancelTime(p, CheckIn, CheckOut);

                    p.Items.Add(GetLastCancelItem(p.LastCancelTime, CheckIn));

                    //处理返现条目
                    if (p.Rebate > 0)
                    {
                        p.Items.Add(GetRebateItem(p.Rebate));
                    }

                    //设置床态
                    SetRoomOptions(p, CheckIn);

                    //临时代码，去掉前端不要显示的内容。
                    for (int i = p.Items.Count - 1; i >= 0; i--)
                    {
                        if (p.Items[i].Type == 1 && p.Items[i].DateType > 0)
                            p.Items.RemoveAt(i);
                    }
                    //if (nightCount > 1)
                    //{
                    //    decimal _totalMamuCommission = 0m;
                    //    decimal _totalAutoCommission = 0m;
                    //    if (pdis != null && pdis.Count > 0 && pdis.Exists(_ => _.SellPrice > 0))
                    //    {
                    //        var firstDayObj = pdis.First(d => d.SellState == 1);
                    //        var _startDate = firstDayObj.Day;
                    //        for (int i = 0; i < nightCount; i++)
                    //        {
                    //            var _calendarItem = pdis.Find(_ => _.Day == _startDate.AddDays(i));
                    //            _totalAutoCommission += _calendarItem.AutoCommission;
                    //            _totalMamuCommission += _calendarItem.ManualCommission;
                    //        }
                    //    }
                    //    if (_totalAutoCommission == 0m) { p.AutoCommission = p.AutoCommission * nightCount; }
                    //    else { p.AutoCommission = _totalAutoCommission; }
                    //    if (_totalMamuCommission == 0m) { p.ManualCommission = p.ManualCommission * nightCount; }
                    //    else { p.ManualCommission = _totalMamuCommission; }
                    //}
                }
            }
            //log.AddLog("foreach");


            //按是否可销售、价格来排序
            var plist = pis.OrderBy(p => p.SellState).ThenBy(p => p.Price).ToList();
            //log.AddLog("OrderBy");

            plist = AddPricePolicy(plist, ppt, CheckIn, HotelServiceEnums.PackageType.HotelPackage);
            //log.AddLog("AddPricePolicy");
            //log.Finish();

            return plist;
        }


        public List<PackageInfoEntity> GetHotelPackageListInfo(int hotelid, DateTime CheckIn, DateTime CheckOut, int terminalType = 1, HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType ppt = HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType.Default, int pid = 0)
        {
            //TimeLog log = new TimeLog(string.Format("GetHotelPackageList pid：{0} hotelid:{1} startDate:{2} endDate:{3}", pid, hotelid, CheckIn, CheckOut), 1000, null);
            List<PackageInfoEntity> pisList = GetHotelPackages(hotelid, 0, terminalType);
            //log.AddLog("GetHotelPackages");
            List<PackageInfoEntity> pis = new List<PackageInfoEntity>();
            if (pid > 0)
            {
                pis = pisList.Where(_ => _.packageBase.ID == pid).ToList();
            }
            //log.AddLog("GetHotelPackagesWhere");
            if (pis.Count > 0)
            {
                //List<PDayItem> pdis = GetHotelPackageCalendar(hotelid, DateTime.Now.Date);
                //log.AddLog("GetHotelPackageCalendar");

                ////获取酒店价格列表 价格以元为单位
                //List<PackageRateEntity> rateList = GetHotelPackageRateList(hotelid, CheckIn, CheckOut, ppt, pid);
                //log.AddLog("GetHotelPackageRateList");

                //每个套餐床型控制的最小房型数
                List<HotelPackageRoomBedCountEntity> rbdList = GetHotelPackageRoomBedCount(hotelid, CheckIn, CheckOut);
                //log.AddLog("GetHotelPackageRoomBedCount");

                IEnumerable<RoomSouldCountEntity> rsList = GetHotelRoomSouldInfo(hotelid, CheckIn, CheckOut);
                //log.AddLog("GetHotelRoomSouldInfo");

                List<PDayItem> pdis = GenHotelPackageCalendarInfo(hotelid, DateTime.Now.Date, pisList, rsList);

                //log.AddLog("GenHotelPackageCalendarInfo");

                //int nightCount = (CheckOut - CheckIn).Days;
                //int bigBedState = 0;
                //int twinBedState = 0; //0:无  1：可售， 2： 不可售
                foreach (PackageInfoEntity p in pis)
                {
                    //SetPackagePrice(p, rateList);

                    //设置套餐项目内容
                    SetDailyItem(p, CheckIn, CheckOut);

                    //所选日期市场价
                    p.MarketPrice = GetMarketPrice(p, CheckIn, CheckOut);

                    //销售状态  1：可售  2：不可售 设置套餐数
                    p.SellState = CheckPackageSealState(pdis, p.packageBase, CheckIn, CheckOut, rbdList, rsList);

                    //设置最后取消日期
                    p.LastCancelTime = GetLastCancelTime(p, CheckIn, CheckOut);

                    p.Items.Add(GetLastCancelItem(p.LastCancelTime, CheckIn));

                    //处理返现条目
                    if (p.Rebate > 0)
                    {
                        p.Items.Add(GetRebateItem(p.Rebate));
                    }

                    //设置床态
                    SetRoomOptions(p, CheckIn);

                    //临时代码，去掉前端不要显示的内容。
                    for (int i = p.Items.Count - 1; i >= 0; i--)
                    {
                        if (p.Items[i].Type == 1 && p.Items[i].DateType > 0)
                            p.Items.RemoveAt(i);
                    }

                    //if (nightCount > 1)
                    //{
                    //    decimal _totalMamuCommission = 0m;
                    //    decimal _totalAutoCommission = 0m;
                    //    if (pdis != null && pdis.Count > 0 && pdis.Exists(_ => _.SellPrice > 0))
                    //    {
                    //        var firstDayObj = pdis.First(d => d.SellState == 1);
                    //        var _startDate = firstDayObj.Day;
                    //        for (int i = 0; i < nightCount; i++)
                    //        {
                    //            var _calendarItem = pdis.Find(_ => _.Day == _startDate.AddDays(i));
                    //            _totalAutoCommission += _calendarItem.AutoCommission;
                    //            _totalMamuCommission += _calendarItem.ManualCommission;
                    //        }
                    //    }
                    //    if (_totalAutoCommission == 0m) { p.AutoCommission = p.AutoCommission * nightCount; }
                    //    else { p.AutoCommission = _totalAutoCommission; }
                    //    if (_totalMamuCommission == 0m) { p.ManualCommission = p.ManualCommission * nightCount; }
                    //    else { p.ManualCommission = _totalMamuCommission; }
                    //}
                }
            }
            //log.AddLog("foreach");


            //按是否可销售、价格来排序
            var plist = pis.OrderBy(p => p.SellState).ThenBy(p => p.Price).ToList();
            //log.AddLog("OrderBy");

            plist = AddPricePolicy(plist, ppt, CheckIn, HotelServiceEnums.PackageType.HotelPackage);
            //log.AddLog("AddPricePolicy");
            //log.Finish();

            return plist;
        }


        public PackageInfoEntity GetFirstVipPackageByPackageId(int pid, DateTime checkIn, DateTime checkOut)
        {
            PackageInfoEntity pif = new PackageInfoEntity();
            pif.packageBase = HotelDAL.GetFirstVipPackageByPackageId(pid);
            if (pif.packageBase.ID > 0)
            {
                //获取酒店价格列表 价格以元为单位
                List<PackageRateEntity> rateList = GetHotelPackageRateList(pif.packageBase.HotelID, checkIn, checkOut, HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType.VIP, pif.packageBase.ID);


                SetPackagePrice(pif, rateList);
            }

            return pif;
        }
        /// <summary>
        /// 根据套餐价格策略，调整一个套餐每日的价格
        /// </summary>
        /// <param name="plist"></param>
        /// <param name="ppt"></param>
        /// <param name="CheckIn"></param>
        /// <param name="pt"></param>
        private void AddRatePricePolicy(List<PackageRateEntity> plist, HotelServiceEnums.PricePolicyType ppt, DateTime CheckIn, HJD.HotelServices.Contracts.HotelServiceEnums.PackageType pt)
        {
            // LogHelper.WriteLog(string.Format("AddRatePricePolicy: ppt:{0} packageType;{1}", ppt.ToString(), pt.ToString()));
            switch (pt)
            {
                case HotelServiceEnums.PackageType.HotelPackage:
                    switch (ppt)
                    {
                        case HotelServiceEnums.PricePolicyType.Botao:
                            //List<CanSellCheapHotelPackageEntity> cheapPackageForBotao = GetCheapHotelPackage4Botao(CheckIn);
                            PackagePricePolicy.UseBotaoPriceRatePolicyForWH(plist);
                            break;
                        case HotelServiceEnums.PricePolicyType.VIP:
                        case HotelServiceEnums.PricePolicyType.Default:
                            PackagePricePolicy.UseVIPPriceRatePolicyForWH(plist, ppt);
                            break;
                    }
                    break;
                case HotelServiceEnums.PackageType.CtripPackage:
                    switch (ppt)
                    {
                        case HotelServiceEnums.PricePolicyType.VIP:
                        case HotelServiceEnums.PricePolicyType.Default:
                            PackagePricePolicy.UseVIPPriceRatePolicyForCtrip(plist, ppt);
                            break;
                    }
                    break;
            }

        }

        /// <summary>
        /// 根据不同的套餐价格策略类型，过套餐进行调整
        /// </summary>
        /// <param name="plist"></param>
        /// <param name="ppt"></param>
        /// <param name="CheckIn"></param>
        /// <param name="pt"></param>
        private List<PackageInfoEntity> AddPricePolicy(List<PackageInfoEntity> plist, HotelServiceEnums.PricePolicyType ppt, DateTime CheckIn, HJD.HotelServices.Contracts.HotelServiceEnums.PackageType pt)
        {
            switch (pt)
            {
                case HotelServiceEnums.PackageType.HotelPackage:
                    switch (ppt)
                    {
                        case HotelServiceEnums.PricePolicyType.VIP:
                        case HotelServiceEnums.PricePolicyType.Default:
                            //plist = PackagePricePolicy.UseVIPPricePolicyForWH(plist, ppt);//去掉vip只能看到的套餐
                            break;
                        case HotelServiceEnums.PricePolicyType.Botao:
                            List<CanSellCheapHotelPackageEntity> cheapPackageForBotao = GetCheapHotelPackage4Botao(CheckIn);
                            plist = PackagePricePolicy.UseBotaoPricePolicyForWH(plist, cheapPackageForBotao);
                            break;
                    }
                    break;
                case HotelServiceEnums.PackageType.CtripPackage:
                    switch (ppt)
                    {
                        case HotelServiceEnums.PricePolicyType.VIP:
                        case HotelServiceEnums.PricePolicyType.Default:
                            plist = PackagePricePolicy.UseVIPPricePolicyForCtrip(plist, ppt);
                            break;
                        case HotelServiceEnums.PricePolicyType.Botao: //botao用户不能购买携程订单
                            plist.Clear();
                            break;
                    }
                    break;
            }

            plist.ForEach(p => p.packageBase.VIPFirstPayDiscount = 200);

            return plist;
        }

        /// <summary>
        /// 设置套餐项目
        /// </summary>
        /// <param name="p"></param>
        /// <param name="CheckInDate"></param>
        private void SetDailyItem(PackageInfoEntity p, DateTime CheckInDate, DateTime CheckOut)
        {
            for (DateTime start = CheckInDate.Date; start < CheckOut.Date; start = start.AddDays(1))
            {
                PackageDailyEntity dItem = new PackageDailyEntity();
                dItem.Day = start;
                dItem.Items = CalcPackageDailyItems(p, start);

                CalcPackagePrice(dItem, p, start);

                p.DailyItems.Add(dItem);
            }
        }

        private void SetPackagePrice(PackageInfoEntity p, List<PackageRateEntity> rateList)
        {

            PackageRateEntity rate = rateList.Where(r => r.PID == p.packageBase.ID).First();
            LogHelper.WriteLog("Price " + rate.Price + "  ManualCommission" + rate.ManualCommission);

            p.Price = rate.Price;
            p.NotVIPPrice = rate.NotVIPPrice;
            p.VIPPrice = rate.VIPPrice;
            p.Rebate = rate.Rebate;
            p.ActiveRebate = rate.ActiveRebate;
            p.CashCoupon = rate.CashCoupon;
            p.CanUseCashCoupon = rate.CanUseCashCoupon;
            p.PayType = rate.PayType;
            p.CanUseCashCouponForBoTao = rate.CanUseCashCouponForBoTao;
            p.AutoCommission = rate.AutoCommission;
            p.ManualCommission = rate.ManualCommission;
        }

        /// <summary>
        /// 根据酒店ID & 套餐Code 获取指定套餐
        /// </summary>
        /// <param name="hotelid">酒店ID</param>
        /// <param name="code">套餐Code</param>
        /// <returns>套餐</returns>
        public PackageInfoEntity GetHotelPackageByCode(int hotelid, string code, int pid)
        {
            return memCacheHotelCtripPackage.GetData<PackageInfoEntity>(string.Format("GetHotelPackageByCode_{0}_{1}_{2}", hotelid, code, pid), () =>
            {
                return GenHotelPackageByCode( hotelid,  code,  pid);
            });
        }

        public PackageInfoEntity GenHotelPackageByCode(int hotelid, string code, int pid)
        {
            PackageEntity entity = new PackageEntity();

            List<PackageEntity> entityList = HotelDAL.GetHotelPackageByCode(hotelid, code, pid);
            if (entityList == null || entityList.Count <= 0)
            {
                return null;
            }

            entity = entityList[0];

            List<PRoomInfoEntity> Rooms = HotelDAL.GetPRoomInfoWithPID(hotelid);
            List<PRoomOptionsEntity> RoomOptions = HotelDAL.GetPRoomOptions(hotelid);

            List<PItemEntity> Items = HotelDAL.GetPItem(hotelid);
            List<PRateEntity> Rates = HotelDAL.GetPRate(hotelid);

            PackageInfoEntity pinfoEntity = new PackageInfoEntity()
            {
                packageBase = entity,
                Items = Items.Where(i => i.PID == entity.ID).ToList(),
                Rates = Rates.Where(i => i.PID == entity.ID).ToList(),
                Room = Rooms.Where(i => i.PID == entity.ID).FirstOrDefault(),
                RoomOptions = RoomOptions.Where(r => r.PID == entity.ID).ToList(),
                PackageType = (int)HotelServiceEnums.PackageType.HotelPackage
            };

            return pinfoEntity;
        }

        /// <summary>
        /// 计算酒店房间已卖数量
        /// </summary>
        /// <param name="hotelid"></param>
        /// <param name="checkIn"></param>
        /// <param name="checkOut"></param>
        /// <returns></returns>
        public List<RoomSouldCountEntity> GetHotelRoomSouldInfo(int hotelid, DateTime checkIn, DateTime checkOut)
        {
            return memCacheHotelCtripPackage.GetData<List<RoomSouldCountEntity>>(string.Format("{0}_{1}_{2}", hotelid, checkIn.Date, checkOut.Date), "GetHotelRoomSouldInfo", () =>
                { 
                return  GenHotelRoomSouldInfo(  hotelid,   checkIn,   checkOut);
                });
        }
  public List<RoomSouldCountEntity> GenHotelRoomSouldInfo(int hotelid, DateTime checkIn, DateTime checkOut)
  {
            int MaxNightCount = 5;// 假设用户最多提交5晚的订单

            checkIn = checkIn.Date;
            checkOut = checkOut.Date;

            DateTime startDate = checkIn.AddDays(-MaxNightCount).Date;
            DateTime endDate = checkOut.AddDays(-1).Date;

            List<RoomSouldItemEntity> rsList = HotelDAL.GetHotelRoomSouldInfo(hotelid, startDate, endDate);
            if (rsList == null || rsList.Count == 0)
            {
                return new List<RoomSouldCountEntity>();
            }

            List<RoomSouldItemEntity> tempList = new List<RoomSouldItemEntity>();

            DateTime checkDate = checkIn;
            DateTime tempCheckIN = DateTime.Now;
            int nightCount = 1;



            for (int i = 1; i < MaxNightCount; i++)
            {
                checkDate = checkIn.AddDays(-i);
                nightCount = i + 1;
                foreach (RoomSouldItemEntity r in rsList.Where(rs => rs.NightCount == nightCount && rs.CheckIn.Date >= checkDate))
                {
                    for (int j = 0; j < nightCount; j++)
                    {
                        tempCheckIN = r.CheckIn.AddDays(j).Date;
                        if (tempCheckIN >= checkIn && tempCheckIN < checkOut)
                        {
                            tempList.Add(new RoomSouldItemEntity() { RoomCount = r.RoomCount, NightCount = 1, CheckIn = tempCheckIN, RoomID = r.RoomID, TwinBed = r.TwinBed, BigBed = r.BigBed });
                        }
                    }
                }
            }

            if (tempList.Count() > 0)
            {
                rsList.AddRange(tempList);
            }

            IEnumerable<RoomSouldCountEntity> countList = (from rs in rsList
                                                           where rs.NightCount == 1 && rs.CheckIn.Date >= checkDate
                                                           group rs by new { rs.RoomID, rs.CheckIn } into g
                                                           select new RoomSouldCountEntity()
                                                           {
                                                               Date = g.Key.CheckIn,
                                                               RoomID = g.Key.RoomID,
                                                               RoomSupplierID = 0,
                                                               SouldCount = g.Sum(o => o.RoomCount),
                                                               BigBedSouldCount = g.Sum(o => o.BigBed),
                                                               TwinBedSouldCount = g.Sum(o => o.TwinBed)
                                                           });

            return countList.ToList();
        }

        /// <summary>
        /// 计算酒店房间已卖数量
        /// </summary>
        /// <param name="hotelid"></param>
        /// <param name="checkIn"></param>
        /// <param name="checkOut"></param>
        /// <returns></returns>
        public IEnumerable<RoomSouldCountEntity> GetHotelPackRoomSouldInfo(int hotelid, DateTime checkIn, DateTime checkOut)
        {
            int MaxNightCount = 5;// 假设用户最多提交5晚的订单

            checkIn = checkIn.Date;
            checkOut = checkOut.Date;

            DateTime startDate = checkIn.AddDays(-MaxNightCount).Date;
            DateTime endDate = checkOut.AddDays(-1).Date;

            List<RoomSouldItemEntity> rsList = HotelDAL.GetHotelRoomSouldInfo(hotelid, startDate, endDate);
            if (rsList == null || rsList.Count == 0)
            {
                return new List<RoomSouldCountEntity>();
            }

            List<RoomSouldItemEntity> tempList = new List<RoomSouldItemEntity>();

            DateTime checkDate = checkIn;
            DateTime tempCheckIN = DateTime.Now;
            int nightCount = 1;

            for (int i = 1; i < MaxNightCount; i++)
            {
                checkDate = checkIn.AddDays(-i);
                nightCount = i + 1;
                foreach (RoomSouldItemEntity r in rsList.Where(rs => rs.NightCount == nightCount && rs.CheckIn.Date >= checkDate))
                {
                    for (int j = 0; j < nightCount; j++)
                    {
                        tempCheckIN = r.CheckIn.AddDays(j).Date;
                        if (tempCheckIN >= checkIn && tempCheckIN < checkOut)
                        {
                            tempList.Add(new RoomSouldItemEntity() { RoomCount = r.RoomCount, NightCount = 1, CheckIn = tempCheckIN, RoomID = r.RoomID, TwinBed = r.TwinBed, BigBed = r.BigBed, RoomSupplierID = r.RoomSupplierID, RoomSupplierType = r.RoomSupplierType });
                        }
                    }
                }
            }

            if (tempList.Count() > 0)
            {
                rsList.AddRange(tempList);
            }

            IEnumerable<RoomSouldCountEntity> countList = (from rs in rsList
                                                           where rs.NightCount == 1 && rs.CheckIn.Date >= checkDate && rs.RoomSupplierType == 2
                                                           group rs by new { rs.RoomID, rs.CheckIn, rs.RoomSupplierID } into g
                                                           select new RoomSouldCountEntity()
                                                           {
                                                               Date = g.Key.CheckIn,
                                                               RoomID = g.Key.RoomID,
                                                               RoomSupplierID = g.Key.RoomSupplierID,
                                                               SouldCount = g.Sum(o => o.RoomCount),
                                                               BigBedSouldCount = g.Sum(o => o.BigBed),
                                                               TwinBedSouldCount = g.Sum(o => o.TwinBed)
                                                           });

            return countList;
        }

        private void SetRoomOptions(PackageInfoEntity p, DateTime CheckIn)
        {

            HotelRoomInfoEntity roomInfo = HotelDAL.GetOnePRoomInfoByRoomID(p.Room.ID);

            string bedOption = "";

            if (roomInfo.DefaultBedType == 0) //无（前端不显示）
            {
                bedOption = "";
            }
            else
            {
                if (roomInfo.HasBigBed > 0)
                {
                    if (roomInfo.IsCheckBedType)
                    {
                        bedOption = "大床";
                        if (p.packageBase.BigBedCount == 0)
                        {
                            bedOption = bedOption + "(满房)";
                        }
                    }
                    else
                    {
                        bedOption = "尽量大床";
                    }
                }

                if (roomInfo.HasTwinBed > 0)
                {
                    if (roomInfo.IsCheckBedType)
                    {
                        string pre = bedOption.Length > 0 ? "," : "";
                        bedOption = bedOption + pre + "双床";
                        if (p.packageBase.TwinBedCount == 0)
                        {
                            bedOption = bedOption + "(满房)";
                        }
                    }
                    else
                    {
                        string pre = bedOption.Length > 0 ? "," : "";
                        bedOption = bedOption + pre + "尽量双床";
                    }
                }
            }




            //List<PRoomOptionsEntity> Items = p.RoomOptions.Where(r => r.DateType == 8 && r.Date == CheckIn).ToList();

            //if (Items == null || Items.Count == 0) //如果不是指定日期条目，取指定星期条目
            //{
            //    int weekday = (int)CheckIn.DayOfWeek;
            //    weekday = weekday == 0 ? 7 : weekday; //星期日=0，需要处理。 数据库定义中0为任意时间
            //    Items = p.RoomOptions.Where(r => r.DateType == weekday).ToList();

            //    if (Items == null || Items.Count == 0)//取平日价格
            //        Items = p.RoomOptions.Where(r => r.DateType == 0).ToList();

            //}

            //if (Items.Count > 0)
            //{
            //    p.Room.Options = Items.First().Options;
            //    p.Room.DefaultOption = Items.First().DefaultOption;
            //}

            //缺省选项
            p.Room.Options = "无烟,吸烟";
            p.Room.DefaultOption = "无烟";

            if (roomInfo.DefaultBedType == 1)
            {
                if (roomInfo.IsCheckBedType)
                {
                    if (p.packageBase.TwinBedCount > 0)
                    {
                        p.Room.DefaultOption = "双床";
                    }
                    else if (p.packageBase.BigBedCount > 0)
                    {
                        p.Room.DefaultOption = "大床";
                    }
                }
                else
                {
                    if (p.packageBase.TwinBedCount > 0)
                    {
                        p.Room.DefaultOption = "尽量双床";
                    }
                    else if (p.packageBase.BigBedCount > 0)
                    {
                        p.Room.DefaultOption = "尽量大床";
                    }
                }
            }

            if (roomInfo.DefaultBedType == 2)
            {
                if (roomInfo.IsCheckBedType)
                {
                    if (p.packageBase.BigBedCount > 0)
                    {
                        p.Room.DefaultOption = "大床";
                    }
                    else if (p.packageBase.TwinBedCount > 0)
                    {
                        p.Room.DefaultOption = "双床";
                    }
                }
                else
                {
                    if (p.packageBase.TwinBedCount > 0)
                    {
                        p.Room.DefaultOption = "尽量大床";
                    }
                    else if (p.packageBase.BigBedCount > 0)
                    {
                        p.Room.DefaultOption = "尽量双床";
                    }
                }
            }

            p.Room.Options = bedOption + (p.Room.Options.Length > 0 ? "|" : "") + p.Room.Options;



        }

        /// <summary>
        /// 获取最后取消日期
        /// </summary>
        /// <param name="p"></param>
        /// <param name="CheckIn"></param>
        /// <param name="CheckOut"></param>
        /// <returns></returns>
        private DateTime GetLastCancelTime(PackageInfoEntity p, DateTime CheckIn, DateTime CheckOut)
        {
            CheckIn = CheckIn.Date;
            CheckOut = CheckOut.Date;
            DateTime minCancelDate = DateTime.Now.AddYears(1);

            DateTime cancelDate = new DateTime();

            PRateEntity r = new PRateEntity();

            for (DateTime curDate = CheckIn; curDate < CheckOut; curDate = curDate.AddDays(1))
            {
                r = GetPackageRateByDate(p, curDate);

                if (r.CancelDayLimit == -1)//即订即保
                {
                    minCancelDate = DateTime.Parse("2000-1-1");
                    break;
                }
                else
                {

                    cancelDate = curDate.AddDays(-r.CancelDayLimit).Date.AddHours(r.CancelTimeLimitHour).AddMinutes(r.CancelTimeLinitMinute);

                    if (cancelDate < minCancelDate)
                        minCancelDate = cancelDate;
                }
            }

            return minCancelDate;
        }
        /// <summary>
        /// 判断套餐是所选日期是否可以销售，同时设置房间数
        /// </summary>
        /// <param name="pdis"></param>
        /// <param name="p"></param>
        /// <param name="CheckIn"></param>
        /// <param name="CheckOut"></param>
        /// <returns>1: 可以销售 2：不可销售 3：日期限止，不能销售</returns>
        private int CheckPackageSealState(List<PDayItem> pdis, PackageEntity package, DateTime CheckIn, DateTime CheckOut, List<HotelPackageRoomBedCountEntity> rbdList, IEnumerable<RoomSouldCountEntity> rsList)
        {
            int intResult = 1;
            package.PackageCount = DEFAULT_ROOM_BED_COUNT;
            package.BigBedCount = DEFAULT_ROOM_BED_COUNT; // -1: 无大床   0：不可售   >0: 可售数理
            package.TwinBedCount = DEFAULT_ROOM_BED_COUNT;// -1: 无双床   0：不可售   >0: 可售数理

            if (package.EndDate.AddDays(1).Date < CheckOut.Date || package.StartDate.Date > CheckIn.Date) //如果不在套餐的销售日期内，则认为不可售
            {
                intResult = 2;

            }
            else
            {

                //判断由床态控制的房间数量  减去已卖出房量
                var rbds = rbdList.Where(r => r.RoomID == package.RoomID);

                if (rbds.Count() > 0)
                {
                    int maxCanSellBigBedCount = 0;
                    int maxCanSellTwinBedCount = 0;

                    for (DateTime curDate = CheckIn.Date; curDate < CheckOut; curDate = curDate.AddDays(1).Date)
                    {
                        var rbd = rbds.Where(r => r.Date == curDate);
                        if (rbd.Count() > 0)
                        {
                            var roomControlCount = rbd.FirstOrDefault();
                            int roomCount = roomControlCount.RCount;
                            int bigBedCount = roomControlCount.BigBed;
                            int twinBedCount = roomControlCount.TwinBed;


                            var rs = rsList.Where(r => r.RoomID == package.RoomID && r.Date == curDate);

                            if (rs.Count() > 0) //有已销售数据
                            {
                                var roomSouldCount = rs.FirstOrDefault();
                                int canSellRoomCount = IsNegativeThan0(roomCount - roomSouldCount.SouldCount);
                                int canSellBigBedCount = IsNegativeThan0(bigBedCount - roomSouldCount.BigBedSouldCount);
                                int canSellTwinBedCount = IsNegativeThan0(twinBedCount - roomSouldCount.TwinBedSouldCount);

                                SetPackageCount(package, canSellBigBedCount, canSellTwinBedCount);

                                maxCanSellBigBedCount = maxCanSellBigBedCount > canSellBigBedCount ? maxCanSellBigBedCount : canSellBigBedCount;
                                maxCanSellTwinBedCount = maxCanSellTwinBedCount > canSellTwinBedCount ? maxCanSellTwinBedCount : canSellTwinBedCount;

                            }
                            else //无已销售数据
                            {
                                SetPackageCount(package, bigBedCount, twinBedCount);

                                maxCanSellBigBedCount = maxCanSellBigBedCount > bigBedCount ? maxCanSellBigBedCount : bigBedCount;
                                maxCanSellTwinBedCount = maxCanSellTwinBedCount > twinBedCount ? maxCanSellTwinBedCount : twinBedCount;

                            }

                        }
                    }

                    package.TwinBedCount = maxCanSellTwinBedCount;
                    package.BigBedCount = maxCanSellBigBedCount;


                    if (package.PackageCount <= 0)
                    {
                        package.PackageCount = 0;
                        intResult = 2;
                    }
                }

                if (intResult == 1)
                {
                    intResult = CheckPackageSealStateByDailyItems(pdis, package, CheckIn, CheckOut);
                }

                if (intResult == 1)
                {
                    intResult = CheckPackageSealStateMore(package, CheckIn, CheckOut);
                }
            }
            return intResult;
        }

        private static void SetPackageCount(PackageEntity package, int canSellBigBedCount, int canSellTwinBedCount)
        {
            package.BigBedCount = package.BigBedCount > canSellBigBedCount ? canSellBigBedCount : package.BigBedCount;
            package.TwinBedCount = package.TwinBedCount > canSellTwinBedCount ? canSellTwinBedCount : package.TwinBedCount;
            package.PackageCount = package.BigBedCount + package.TwinBedCount;
        }

        private static int IsNegativeThan0(int Num)
        {
            return Num < 0 ? 0 : Num;
        }

        private int CheckPackageSealStateByDailyItems(List<PDayItem> pdis, PackageEntity package, DateTime CheckIn, DateTime CheckOut)
        {
            int intResult = 1;
            if (pdis.Count == 0)
            {
                intResult = 2;
            }
            else
            {
                int PID = package.ID;
                int startIndex = CommMethods.GetNightCount(pdis.First().Day, CheckIn);
                int nightCount = CommMethods.GetNightCount(CheckIn, CheckOut);


                // package.PackageCount = 10000;
                for (int i = 0; i < nightCount; i++)
                {
                    if (startIndex + i >= pdis.Count) continue;

                    PDayItem di = pdis[startIndex + i];
                    if (di.PItems.Where(p => p.PID == PID).Count() > 0)
                    {
                        PDayPItem pitem = di.PItems.Where(p => p.PID == PID).First();
                        int canSellCount = pitem.MaxSealCount - pitem.SoldCount;
                        if (canSellCount <= 0)
                        {
                            intResult = 2;
                            package.PackageCount = 0;
                            break;
                        }
                        //get the min canSellCount
                        package.PackageCount = package.PackageCount > canSellCount ? canSellCount : package.PackageCount;
                    }
                    else
                    {
                        intResult = 2;
                        package.PackageCount = 0;
                        break;
                    }
                }
            }
            return intResult;
        }

        private int CheckPackageSealStateMore(PackageEntity package, DateTime CheckIn, DateTime CheckOut)
        {
            int intResult = 1;

            if (package.StartDate > CheckIn)
            {
                intResult = 2;
            }
            else
            {
                //处理提前购买需求
                if (package.TimeAdvance > 0)
                {
                    DateTime CanOrderDay = DateTime.Now.AddMinutes(package.TimeAdvance).Date;

                    if (CheckIn < CanOrderDay)
                        intResult = 2;
                }

                if (intResult == 1)
                {
                    int PID = package.ID;
                    int nightCount = CommMethods.GetNightCount(CheckIn, CheckOut);


                    //处理套餐间夜限止
                    if ((package.DayLimitMin > 0 && nightCount < package.DayLimitMin)
                         || (package.DayLimitMax > 0 && nightCount > package.DayLimitMax))
                    {
                        intResult = 3;
                    }
                }

            }
            return intResult;
        }

        private int GetMarketPrice(PackageInfoEntity p, DateTime CheckIn, DateTime CheckOut)
        {
            return 1000;
        }

        //private void CalcPackagePrice(PackageInfoEntity p, DateTime CheckIn, DateTime CheckOut)
        //{
        //    int NightCount = CommMethods.GetNightCount(CheckIn, CheckOut);

        //    if (NightCount <= 0)
        //        return;

        //    int Price = 0;
        //    int Rebate = 0;
        //    int ActiveRebate = 0;
        //    int CashCoupon = 0;
        //    int CanUseCashCoupon = 0;
        //    int PayType = 0;

        //    for (int i = 0; i < NightCount; i++)
        //    {
        //        PackageDailyEntity dItem = new PackageDailyEntity();
        //        dItem.Day = CheckIn.AddDays(i);
        //        CalcPackagePrice(dItem, p, CheckIn.AddDays(i));
        //        dItem.Items = CalcPackageDailyItems(p, CheckIn.AddDays(i));
        //        Price += dItem.Price;
        //        Rebate += dItem.Rebate;
        //        ActiveRebate += dItem.ActiveRebate;
        //        CashCoupon += dItem.CashCoupon;
        //        CanUseCashCoupon += dItem.CanUseCashCoupon;
        //        PayType = PayType < dItem.PayType ? dItem.PayType : PayType;
        //        p.DailyItems.Add(dItem);
        //    }

        //    p.Price = Price;
        //    p.Rebate = Rebate;
        //    p.ActiveRebate = ActiveRebate;
        //    p.CashCoupon = CashCoupon;
        //    p.CanUseCashCoupon = CanUseCashCoupon;
        //    p.PayType = PayType;
        //}

        private List<PItemEntity> CalcPackageDailyItems(PackageInfoEntity p, DateTime CheckIn)
        {
            //取指定日期条目
            List<PItemEntity> Items = p.Items.Where(r => r.Type == 1 && r.DateType == 8 && r.Date == CheckIn).ToList();

            if (Items == null || Items.Count == 0) //如果不是指定日期条目，取指定星期条目
            {
                int weekday = (int)CheckIn.DayOfWeek;
                weekday = weekday == 0 ? 7 : weekday; //星期日=0，需要处理。 数据库定义中0为任意时间
                Items = p.Items.Where(r => r.Type == 1 && r.DateType == weekday).ToList();

                if (Items == null || Items.Count == 0)//取平日价格
                    Items = p.Items.Where(r => r.Type == 1 && r.DateType == 0).ToList();

            }

            return Items;
        }

        private void CalcPackagePrice(PackageDailyEntity dItem, PackageInfoEntity p, DateTime CheckIn)
        {
            //取指定日期价格
            PRateEntity rate = GetPackageRateByDate(p, CheckIn);

            dItem.Price = rate == null ? -1 : rate.Price ?? -1;
            dItem.Rebate = rate == null ? 0 : rate.Rebate;
            dItem.ActiveRebate = rate == null ? 0 : rate.ActiveRebate;
            dItem.CashCoupon = rate == null ? 0 : rate.CashCoupon;
            dItem.CanUseCashCoupon = rate == null ? 0 : rate.CanUseCashCoupon;
            dItem.PayType = rate == null ? 3 : rate.PayType;
            dItem.CanUseCashCouponForBoTao = rate == null ? 0 : rate.CanUseCashCouponForBoTao;
        }

        /// <summary>
        /// 获取套餐与指定日期相关的价格记录
        /// </summary>
        /// <param name="p"></param>
        /// <param name="CheckIn"></param>
        /// <returns></returns>
        public PRateEntity GetPackageRateByDate(PackageInfoEntity p, DateTime CheckIn)
        {  //取指定日期价格
            PRateEntity rate = p.Rates.Where(r => r.Type == 8 && r.Date == CheckIn.Date).FirstOrDefault();

            if (rate == null || rate.ID == 0) //如果不是指定日期价格，取指定星期价格
            {
                int weekday = (int)CheckIn.DayOfWeek;
                weekday = weekday == 0 ? 7 : weekday; //星期日=0，需要处理。 数据库定义中0为任意时间
                rate = p.Rates.Where(r => r.Type == weekday).FirstOrDefault();

                if (rate == null || rate.ID == 0)//如果不是指定星期价格，取平日价格
                {
                    if (p.Rates.Where(r => r.Type == 0).Count() > 0)
                    {
                        rate = p.Rates.Where(r => r.Type == 0).FirstOrDefault();
                    }
                    else //如果没有平日价，那么取最后一个价格
                    {
                        rate = p.Rates.Last();
                    }

                }

            }

            return rate;
        }
        public HotelContactPackageEntity GetHotelContactPackageByHotelIDAndPid(int hotelid, int pid)
        {
            //HotelContactPackageEntity h = new HotelContactPackageEntity();
            HotelContactPackageEntity hc = HotelDAL.GetHotelContactPackageByHotelIDAndPid(hotelid, pid);
            return hc == null ? new HotelContactPackageEntity() : hc;
            //h.BankInfo = hc.BankInfo;
            //h.Beach = hc.Beach;
            //h.BuffetSupperPrice = hc.BuffetSupperPrice;
            //h.BusSchedule = hc.BusSchedule;
            //h.CheckInSettleAdvanceDay = hc.CheckInSettleAdvanceDay;
            //h.ChildCare = hc.ChildCare;
            //h.ChildCareIsFree = hc.ChildCareIsFree;
            //h.ConfirmEmail = hc.ConfirmEmail;
            //h.ContactName = hc.ContactName;
            //h.Deposit = hc.Deposit;
            //h.Fax = hc.Fax;
            //h.Fax2 = hc.Fax2;
            //h.Fax2TimeEnd = hc.Fax2TimeEnd;
            //h.Fax2TimeStart = hc.Fax2TimeStart;
            //h.Financial = hc.Financial;
            //h.FinancialTel = hc.FinancialTel;
            //h.HasBus = hc.HasBus;
            //h.HighSpeedRailStation = hc.HighSpeedRailStation;
            //h.HotelID = hc.HotelID;
            //h.HotelPayType = hc.HotelPayType;
            //h.HSRailStationDistance = hc.HSRailStationDistance;
            //h.IndoorChildrenPlayground = hc.IndoorChildrenPlayground;
            //h.IndoorChildrenPool = hc.IndoorChildrenPool;
            //h.IndoorSwimmingPool = hc.IndoorSwimmingPool;
            //h.IndoorSwimmingPoolIsHengWen = hc.IndoorSwimmingPoolIsHengWen;
            //h.InvoiceType = hc.InvoiceType;
            //h.IsFestival = hc.IsFestival;
            //h.MonthSettleDay = hc.MonthSettleDay;
            //h.Note = hc.Note;
            //h.OutdoorChildrenPlayground = hc.OutdoorChildrenPlayground;
            //h.OutdoorChildrenPool = hc.OutdoorChildrenPool;
            //h.OutdoorChildrenPoolIsHengWen = hc.OutdoorChildrenPoolIsHengWen;
            //h.OutdoorSwimmingPool = hc.OutdoorSwimmingPool;
            //h.Priority = hc.Priority;
            //h.PrivateBeach = hc.PrivateBeach;
        }



        public List<PackageInfoEntity> GetHotelPackages(int hotelid, int pid = 0, int terminalType = 1)
        {
            return GenHotelPackages(hotelid, pid, terminalType);
            return memCacheHotelCtripPackage.GetData<List<PackageInfoEntity>>(string.Format("GetHotelPackages{0}_{1}_{2}", hotelid, pid, terminalType), () =>
                  {
                      return GenHotelPackages(hotelid, pid, terminalType);
                  });
        }

         public List<PackageInfoEntity> GenHotelPackages(int hotelid, int pid = 0, int terminalType = 1)
        {
            List<PackageInfoEntity> PIS = new List<PackageInfoEntity>();

            List<PackageEntity> PS = HotelDAL.GetPackage(hotelid, pid, terminalType);

            List<PRoomInfoEntity> Rooms = HotelDAL.GetPRoomInfoWithPID(hotelid, pid);
            List<PRoomOptionsEntity> RoomOptions = HotelDAL.GetPRoomOptions(hotelid, pid);

            List<PItemEntity> Items = HotelDAL.GetPItem(hotelid, pid);
            List<PRateEntity> Rates = HotelDAL.GetPRate(hotelid, pid);

            PItemEntity breakfastItem = new PItemEntity();
            PItemEntity BuffetItem = new PItemEntity();
            HotelContactEntity hc = HotelDAL.GetHotelContactByHotelID(hotelid);
            if (hc != null && !string.IsNullOrEmpty(hc.SelfBreakfastPrice) && hc.SelfBreakfastPrice.Length > 3)
            {
                breakfastItem = new PItemEntity
                {
                    Date = DateTime.Parse("1900-1-1"),
                    DateType = 0,
                    Description = "自助早餐参考价格：" + hc.SelfBreakfastPrice.Replace("\n", ""),
                    HotelID = hotelid,
                    ID = hotelid,
                    ItemCode = "自助早餐参考价格",
                    PID = 0,
                    Price = 0,
                    SourceType = 0,
                    Type = 2
                };
            }
            if (hc != null && !string.IsNullOrEmpty(hc.BuffetSupperPrice) && hc.BuffetSupperPrice.Length > 3)
            {
                BuffetItem = new PItemEntity
                {
                    Date = DateTime.Parse("1900-1-1"),
                    DateType = 0,
                    Description = "自助晚餐参考价格：" + hc.BuffetSupperPrice.Replace("\n", ""),
                    HotelID = hotelid,
                    ID = hotelid,
                    ItemCode = "自助晚餐参考价格",
                    PID = 0,
                    Price = 0,
                    SourceType = 0,
                    Type = 2
                };
            }
            //追加 酒店级别的注意事项
            List<PItemEntity> hotelItem=new List<PItemEntity>();
            if (hc != null && !string.IsNullOrWhiteSpace(hc.HotelItemNotice))
            {
                hotelItem = genHotelPItems(hc.HotelItemNotice, hotelid);
            }


            foreach (PackageEntity p in PS)
            {
                PackageInfoEntity pi = new PackageInfoEntity();

                pi.packageBase = p;

                pi.Items = genMultiPItems(Items.Where(i => i.PID == p.ID));

                //处理早餐政策
                var ZC = pi.Items.Where(i => i.Description.StartsWith("ZC:"));
                if (ZC.Count() > 0)
                {
                    ZC.First().Description = ZC.First().Description.Substring(3);
                }
                else if (breakfastItem.ID != 0)
                {
                    pi.Items.Add(breakfastItem);
                    pi.Items.Add(BuffetItem);
                }
                if (hotelItem.Count > 0)
                {
                    pi.Items.AddRange(hotelItem);
                }

                pi.Rates = Rates.Where(r => r.PID == p.ID).ToList();
                pi.Room = Rooms.Where(r => r.PID == p.ID).FirstOrDefault();
                pi.RoomOptions = RoomOptions.Where(r => r.PID == p.ID).ToList();

                //获取具体房型信息
                HotelRoomInfoEntity roomInfo = HotelDAL.GetOnePRoomInfoByRoomID(pi.Room.ID);
                if (roomInfo != null && string.IsNullOrEmpty(pi.Room.Options))
                {
                    var _defInfo = "无烟";
                    var _bedInfo = "";
                    var _otherInfo = "无烟,吸烟";
                    
                    if (roomInfo.HasBigBed > 0) 
                    {
                        _bedInfo += "大床";
                        _defInfo = "大床";
                    }

                    if (roomInfo.HasTwinBed > 0) 
                    {
                        if (!string.IsNullOrEmpty(_bedInfo)) _bedInfo += ",";
                        _bedInfo += "双床";

                        _defInfo = "双床";
                    }

                    if (!string.IsNullOrEmpty(_bedInfo)) _bedInfo += "|";

                    pi.Room.Options = (_bedInfo + _otherInfo);
                    pi.Room.DefaultOption = _defInfo;
                }

                pi.MinHotelPeople = p.MinHotelPeople;

                pi.MaxHotelPeople = p.MaxHotelPeople;
                //pi.CardTypeList = string.IsNullOrEmpty(p.CardType) ? p.CardType.Split(';').ToArray() : new string[] { };
                pi.CardTypeList = p.CardType.Split(';').ToList();

                pi.TravelPersonDescribe = p.TravelPersonDescribe;

                pi.PackageType = (int)HotelServiceEnums.PackageType.HotelPackage;

                pi.IsNotSale = p.IsNotSale;

                pi.CanUseCoupon = p.CanUseCoupon;

                pi.CustomBuyMax = p.CustomBuyMax;

                pi.CustomBuyMin = p.CustomBuyMin;

                pi.IsNeedDocumentInformation = ((p.WHPackageType == 2 || p.WHPackageType == 1) && p.MaxHotelPeople > 0) ? true : false;

                pi.CheckTimeShow = (p.WHPackageType == 2 || p.WHPackageType == 1) ? 1 : 0;

                if (p.WHPackageType == 1)
                {
                    pi.PurchaseNotes.ActionUrl = "http://www.zmjiudian.com/active/activepage?pageid=72";
                    pi.PurchaseNotes.Text = "周末酒店机酒套餐预定须知";
                    pi.PurchaseNotes.Description = "点击购买表示已阅读并同意";
                }

                PIS.Add(pi);

            }

            return PIS;
        }

        private List<PItemEntity> genMultiPItems(IEnumerable<PItemEntity> items)
        {
            var result = new List<PItemEntity>();
            if (items != null)
            {
                foreach (var item in items)
                {
                    //如果是SourceType=2的需要检查是不是有换行 换行就是多条记录
                    if (item.SourceType == 2 && !string.IsNullOrWhiteSpace(item.Description))
                    {
                        result.AddRange(item.Description.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(_ => new PItemEntity()
                        {
                            Date = item.Date,
                            DateType = item.DateType,
                            Description = _,
                            HotelID = item.HotelID,
                            ID = item.ID,
                            ItemCode = item.ItemCode,
                            NotVIPPrice = item.NotVIPPrice,
                            PID = item.PID,
                            Price = item.Price,
                            SourceType = item.SourceType,
                            Type = item.Type,
                            VIPPrice = item.VIPPrice
                        }));
                    }
                    else
                    {
                        result.Add(item);
                    }
                }
            }
            return result;
        }


        private List<PItemEntity> genHotelPItems(string hotelItemNotice,int hotelid)
        {
            var result = new List<PItemEntity>();
            if (!string.IsNullOrWhiteSpace(hotelItemNotice))
            {
                result.AddRange(hotelItemNotice.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).Select(_ => new PItemEntity()
                {
                    Date = System.DateTime.Now,
                    DateType = 0,
                    Description = _,
                    HotelID = hotelid,
                    ID = 0,
                    ItemCode = "",
                    NotVIPPrice = 0,
                    PID = 0,
                    Price = 0,
                    SourceType = 0,
                    Type = 2,
                    VIPPrice = 0
                }));
                //foreach (var item in items)
                //{
                //    //如果是SourceType=2的需要检查是不是有换行 换行就是多条记录
                //    if (item.SourceType == 2 && !string.IsNullOrWhiteSpace(item.Description))
                //    {
                        
                //    }
                //    else
                //    {
                //        result.Add(item);
                //    }
                //}
            }
            return result;
        }

        public PItemEntity GetLastCancelItem(DateTime lastCancelTime, DateTime CheckIn)
        {
            string lastCancelNotice = "";

            if (lastCancelTime.Year == 2000)
            {
                lastCancelNotice = "订单确认之后，不可更改或取消。";
            }
            else if (DateTime.Now < lastCancelTime)
            {
                lastCancelNotice = lastCancelTime.ToString("yyyy年MM月dd日") + (lastCancelTime.Hour < 12 ? "上午" : (lastCancelTime.Hour == 12 ? "中午" : "")) + lastCancelTime.Hour.ToString() + (lastCancelTime.Minute == 0 ? "点整" : "点" + lastCancelTime.Minute.ToString() + "分") + "之前取消预订，可无理由全额退款；过期不可取消或更改。";
            }
            else
            {
                int DayDiff = (int)(CheckIn - lastCancelTime.Date).TotalDays;

                lastCancelNotice = string.Format("订单确认后，入住日{0}中午12点之前，可无条件取消或更改。之后，不可取消或更改。",
                   (DayDiff == 0 ? "当天" : "前" + DayDiff.ToString() + "天")
                    );
            }
            return new PItemEntity()
             {
                 Type = 2,
                 Description = lastCancelNotice,
                 Date = DateTime.Now,
                 DateType = 0,
                 HotelID = 0,
                 ID = 0,
                 ItemCode = "取消政策",
                 PID = 0,
                 Price = 0
             };
        }

        public PItemEntity GetRebateItem(int rebate)
        {
            return new PItemEntity()
            {
                Type = 2,
                Description = string.Format("此订单可享受{0}元返现。通过“周末酒店”APP“我的钱包”申请返现，款项将会在1-5个工作日内直接返回到您的支付帐户中。", rebate),
                Date = DateTime.Now,
                DateType = 0,
                HotelID = 0,
                ID = 0,
                ItemCode = "返现",
                PID = 0,
                Price = 0
            };
        }

        /// <summary>
        /// 获取房型对应的可用价格条目
        /// </summary>
        /// <param name="rps"></param>
        /// <param name="roomtypeid"></param>
        /// <param name="checkIn"></param>
        /// <param name="checkOut"></param>
        /// <returns></returns>
        public IEnumerable<JLHotelRoomPriceEntity> GetFitPriceItem(IEnumerable<JLHotelRoomPriceEntity> rps, int ratetype, int Roomtypeid, DateTime checkIn, DateTime checkOut)
        {
            //同一时段内取allotmenttype 最大的可卖房 价格最低 并且是同一个供应商
            //6.27 现在改成取价格最低，且是同一供应商
            IEnumerable<JLHotelRoomPriceEntity> temprps = rps.Where(o => o.ratetype == ratetype && o.RoomtypeId == Roomtypeid && o.Ratetypename.IndexOf("票") < 0);

            int nightCount = GetNightCount(checkIn, checkOut);

            if (temprps.Count() < nightCount)//如果nightCount == 2 但只有一天的记录，那么这个套餐不能销售。
            {
                return new List<JLHotelRoomPriceEntity>();
            }

            if (temprps.Count() > 1)
            {

                //由日期 供应商、价格组成的矩阵  求价格都有，且最便宜的 一组。
                //     1    2     3 
                //s1  23    23   23
                //s2  20    25   28 
                //s3   15   0    25
                //同一供应商同一天对同一个房型的同一个价格类型还是会有不同报价


                Dictionary<int, List<JLHotelRoomPriceEntity>> mPriceItem = new Dictionary<int, List<JLHotelRoomPriceEntity>>();
                Dictionary<int, int> mPrice = new Dictionary<int, int>();
                int lastSupplierid = 0;
                for (int i = 0; i < nightCount; i++)
                {
                    lastSupplierid = 0;
                    foreach (JLHotelRoomPriceEntity t in temprps.Where(t => t.Night.Date == checkIn.AddDays(i).Date).OrderBy(o => o.Supplierid).OrderByDescending(o => o.allotmentType))
                    {
                        //一个供应商只取一个记录 JLTour的数据中会出现同一供应商同一天同一价格类型给出两个的价的情况。 只取最低的报价。。。 头疼。。
                        if (lastSupplierid > 0 && t.Supplierid == lastSupplierid) continue;

                        if (mPriceItem.ContainsKey(t.Supplierid))
                        {
                            mPriceItem[t.Supplierid].Add(t);
                            mPrice[t.Supplierid] += (int)t.Price;
                        }
                        else if (i == 0) //只加入第一天都有的
                        {
                            mPriceItem.Add(t.Supplierid, new List<JLHotelRoomPriceEntity>() { t });
                            mPrice.Add(t.Supplierid, (int)t.Price);
                        }
                        lastSupplierid = t.Supplierid;
                    }
                }

                int minSID = 0;
                foreach (int sid in mPrice.Keys)
                {
                    if (mPriceItem[sid].Count == nightCount)
                    {
                        if (minSID == 0)
                        {
                            minSID = sid;
                        }
                        else if (mPrice[minSID] > mPrice[sid])
                        {
                            minSID = sid;
                        }
                    }
                }

                if (minSID > 0)
                {
                    return mPriceItem[minSID];
                }
                else
                {
                    return new List<JLHotelRoomPriceEntity>();
                }

                //int maxAllotmentType = temprps.Max(m => m.allotmentType);
                //int minAllotmentType = temprps.Min(m => m.allotmentType);

                //int allotmentType = maxAllotmentType;
                //if (maxAllotmentType > minAllotmentType)
                //{
                //    //找到都有房的allotmentType值 
                //    int nightCount = GetNightCount(checkIn, checkOut);
                //    for (int a = maxAllotmentType; a >= minAllotmentType; a--)
                //    {
                //        if (temprps.Where(m => m.allotmentType == a && m.Qtyable >= 0).Count() == nightCount)
                //        {
                //            allotmentType = a;
                //            break;
                //        }
                //    }
                //}

                //int supplyid = temprps.Where(m => m.allotmentType == allotmentType).OrderBy(o => o.Price).First().Supplierid;

                //return temprps.Where(m => m.allotmentType == allotmentType && m.Supplierid == supplyid);

            }
            else
            {
                return temprps;
            }
        }

        public void SetCancelItemAndTime(IEnumerable<JLHotelRoomPriceEntity> rps, PackageInfoEntity pi, int pid)
        {
            PItemEntity pie1;

            if (rps.Where(o => o.Voidabletype == 13).Count() > 0) //13	即订即保
            {
                pi.LastCancelTime = DateTime.Now.Date;//设置最晚取时间为今天，表示即订即保  设计中应有一个字段来明确表示这个含义，不应是隐含的

                pie1 = new PItemEntity()
                {
                    Type = 2,
                    Description = "预订后不能取消。",
                    Date = DateTime.Now,
                    DateType = 0,
                    HotelID = 0,
                    ID = 0,
                    ItemCode = "取消政策",
                    PID = pid,
                    Price = 0
                };
            }
            else
            {

                pi.LastCancelTime = rps.Min(r => r.Night).Date.AddHours(12).AddMinutes(0);

                DateTime tempDate;
                foreach (JLHotelRoomPriceEntity jr in rps)
                {
                    string[] timePart = jr.Timeselect.Split(':');

                    int hour = 12;
                    int minute = 0;

                    if (timePart.Length == 2)
                    {
                        if (hour < 12)
                        {
                            hour = hour > 1 ? hour - 1 : hour;
                        }
                    }

                    tempDate = jr.Night.AddDays(-jr.Dayselect).Date.AddHours(hour).AddMinutes(minute);

                    if (tempDate < pi.LastCancelTime)
                        pi.LastCancelTime = tempDate;

                }

                pie1 = new PItemEntity()
                    {
                        Type = 2,
                        Description = string.Format("{0}之前取消预订，客人可无理由退款。",
                            pi.LastCancelTime.ToString("yyyy-MM-dd hh:mm")),
                        Date = DateTime.Now,
                        DateType = 0,
                        HotelID = 0,
                        ID = 0,
                        ItemCode = "取消政策",
                        PID = pid,
                        Price = 0
                    };
            }

            pi.Items.Add(pie1);

        }


        public void SetTopTownCancelItemAndTime(IEnumerable<TopTownHotelPriceEntity> rps, PackageInfoEntity pi, int pid)
        {
            PItemEntity pie1;

            if (rps.Where(o => o.RTStopCancelTime == 0).Count() > 0) //0	不可退
            {
                pi.LastCancelTime = DateTime.Now.Date;//设置最晚取时间为今天，表示即订即保  设计中应有一个字段来明确表示这个含义，不应是隐含的

                pie1 = new PItemEntity()
                {
                    Type = 2,
                    Description = "预订后不能取消。",
                    Date = DateTime.Now,
                    DateType = 0,
                    HotelID = 0,
                    ID = 0,
                    ItemCode = "取消政策",
                    PID = pid,
                    Price = 0
                };
            }
            else
            {

                pi.LastCancelTime = Convert.ToDateTime(rps.Min(r => r.EffectiveDate)).Date.AddHours(12).AddMinutes(0);

                foreach (TopTownHotelPriceEntity rp in rps)
                {
                    pi.LastCancelTime = rp.StopCancelDate;
                }

                pie1 = new PItemEntity()
                {
                    Type = 2,
                    Description = string.Format("{0}之前取消预订，客人可无理由退款。",
                        pi.LastCancelTime.ToString("yyyy-MM-dd hh:mm")),
                    Date = DateTime.Now,
                    DateType = 0,
                    HotelID = 0,
                    ID = 0,
                    ItemCode = "取消政策",
                    PID = pid,
                    Price = 0
                };
            }

            pi.Items.Add(pie1);

        }
        public List<PackageInfoEntity> GetTopTownPackages(int hotelid, DateTime checkIn, DateTime checkOut, HotelServiceEnums.PricePolicyType ppt = HotelServiceEnums.PricePolicyType.Default)
        {
            List<PackageInfoEntity> PIS = new List<PackageInfoEntity>();
            List<PackageEntity> PS = new List<PackageEntity>();

            List<TopTownHotelRoomEntity> rooms = HotelDAL.GetTopTownHotelRoom(hotelid);
            List<TopTownHotelPriceEntity> allRPS = HotelDAL.GetTopTownHotelRoomPrice(hotelid, checkIn, checkOut);


            foreach (TopTownHotelRoomEntity item in rooms)
            {
                IEnumerable<TopTownHotelPriceEntity> rps = allRPS.Where(p => p.RoomTypeCode == item.RoomTypeCode);
                //IEnumerable<int> RateTypeList = rps.Select(p => p.RateId).Distinct();
                List<IGrouping<string, TopTownHotelPriceEntity>> GroupbyRateCodeList = rps.GroupBy(_ => _.RatePlanCode).ToList();
                foreach (IGrouping<string, TopTownHotelPriceEntity> tpr in GroupbyRateCodeList)
                {
                    PackageInfoEntity pi = new PackageInfoEntity();
                    pi.Items = new List<PItemEntity>();
                    pi.Rates = new List<PRateEntity>();
                    pi.SellState = (int)PackageSellSate.canSell;
                    List<PackageDailyEntity> pdes = new List<PackageDailyEntity>();

                    int pid = tpr.FirstOrDefault().RateId;
                    PackageEntity p = new PackageEntity()
                    {
                        CanInvoice = true,
                        Code = item.RoomTypeName,
                        DayLimitMax = 0,
                        DayLimitMin = 0,
                        EndDate = DateTime.Now.AddYears(1),
                        HotelID = item.HotelId,
                        ID = pid,
                        IsValid = true,
                        PackageCount = 5,
                        RoomID = item.RoomId,
                        SoldCountSum = 1,
                        StartDate = DateTime.Now.AddDays(-10),
                        StartNum = 0,
                        TimeAdvance = 1
                    };

                    PRoomInfoEntity room = new PRoomInfoEntity()
                    {
                        DefaultOption = "",
                        Description = string.Format("{0}", item.RoomTypeName),
                        FestivalPrice = 1000,
                        HotelID = item.HotelId,
                        ID = item.RoomId,
                        Options = "",
                        PID = pid,
                        RelOTAID = 0,
                        RelOTARoomID = 0,
                        RoomCode = item.RoomTypeCode,
                        WeekDayPrice = 0,
                        WeekendDayPrice = 0
                    };
                    SetTopTownPackageRoomOptions(item, pi, pid, room);
                    SetTopTownPackageDailyItem(item, pi, pdes, pid, allRPS);
                    SetTopTownCancelItemAndTime(rps, pi, pid);
                    if (pdes.Count > 0)
                    {
                        pi.PackageType = (int)HotelServiceEnums.PackageType.JLPackage;

                        pi.DailyItems = pdes;
                        pi.packageBase = p;
                        pi.Room = room;

                        pi.PayType = pi.DailyItems.Max(day => day.PayType);  // 3: 预付    4：待确认后支付（ 用户先提交订单，客服确认有房后，再付款）

                        if (pi.SellState == (int)PackageSellSate.canSell)
                        {
                            pi.SellState = (CheckPackageSealStateMore(pi.packageBase, checkIn, checkOut) == 1) ? (int)PackageSellSate.canSell : (int)PackageSellSate.canNotSell;
                        }

                        PIS.Add(pi);
                    }
                }


            }

            var plist = PIS.OrderBy(p => p.SellState).ThenBy(p => p.Price).ToList();
            AddPricePolicy(plist, ppt, checkIn, HotelServiceEnums.PackageType.TopTownPackage);
            return plist;
            //return new List<PackageInfoEntity>();
        }


        public List<PackageInfoEntity> GetJLHotelPackages(int hotelid, DateTime checkIn, DateTime checkOut, HotelServiceEnums.PricePolicyType ppt = HotelServiceEnums.PricePolicyType.Default)
        {
            List<PackageInfoEntity> PIS = new List<PackageInfoEntity>();

            List<JLHotelRoomEntity> rooms = HotelDAL.GetJLHotelRoom(hotelid);
            List<JLHotelRoomPriceEntity> allRPS = HotelDAL.GetJLHotelRoomPrice(hotelid, checkIn, checkOut);

            List<PackageEntity> PS = new List<PackageEntity>();


            //按房型，价格类型打包套餐
            foreach (JLHotelRoomEntity r in rooms)
            {

                IEnumerable<JLHotelRoomPriceEntity> rps = allRPS.Where(p => p.RoomtypeId == r.Roomtypeid);

                IEnumerable<int> RateTypeList = rps.Select(p => p.ratetype).Distinct();

                foreach (int ratetype in RateTypeList)
                {
                    PackageInfoEntity pi = new PackageInfoEntity();
                    pi.Items = new List<PItemEntity>();
                    pi.Rates = new List<PRateEntity>();
                    pi.SellState = (int)PackageSellSate.canSell;
                    List<PackageDailyEntity> pdes = new List<PackageDailyEntity>();

                    int pid = r.Roomtypeid * 1000 + ratetype;
                    PackageEntity p = new PackageEntity()
                    {
                        CanInvoice = true,
                        Code = r.Namechn,
                        DayLimitMax = 0,
                        DayLimitMin = 0,
                        EndDate = DateTime.Now.AddYears(1),
                        HotelID = r.HotelId,
                        ID = pid,
                        IsValid = true,
                        PackageCount = 5,
                        RoomID = r.Roomtypeid,
                        SoldCountSum = 1,
                        StartDate = DateTime.Now.AddDays(-10),
                        StartNum = r.Roomtypeid % 10,
                        TimeAdvance = 0
                    };

                    PRoomInfoEntity room = new PRoomInfoEntity()
                    {
                        DefaultOption = "",
                        Description = string.Format("{0}{1}{2}", r.Namechn,
                        r.Bedtype == null ? "" : (" " + r.Bedtype + (r.Bedsize == "" ? "" : ("(" + r.Bedsize + ")"))),
                        r.Acreages.Trim().Length > 0 ? (" " + r.Acreages + "平米") : ""),
                        FestivalPrice = 1000,
                        HotelID = r.HotelId,
                        ID = r.Roomtypeid,
                        Options = "",
                        PID = pid,
                        RelOTAID = 0,
                        RelOTARoomID = 0,
                        RoomCode = r.Namechn,
                        WeekDayPrice = 0,
                        WeekendDayPrice = 0
                    };


                    IEnumerable<JLHotelRoomPriceEntity> temprps = GetFitPriceItem(rps, ratetype, r.Roomtypeid, checkIn, checkOut);

                    if (temprps.Count() > 0)
                    {
                        SetJLTourPackageDailyItem(r, pi, pdes, pid, temprps);
                        SetCancelItemAndTime(temprps, pi, pid);

                        JLHotelRoomPriceEntity firstRoomPrice = temprps.OrderBy(t => t.Night).First();

                        if (firstRoomPrice.termtype > 0)//条款类型:提前订房,指定日期前,连住晚数,指定时间段
                        {
                            switch (firstRoomPrice.termtype)
                            {
                                case 11://	提前预订
                                    //( 0:随时可预订 480=（24-16）*60 ：下午16点前可预订当天的 1920=（24*2-16）*60 下午16点前可预订明天的 )
                                    int advDay = firstRoomPrice.advancedays;
                                    int advHour = int.Parse(firstRoomPrice.advancetime.Split(':')[0]);//14:00 
                                    int advMin = int.Parse(firstRoomPrice.advancetime.Split(':')[1]);
                                    p.TimeAdvance = (24 * (advDay + 1) - advHour) * 60 - advMin;

                                    if (DateTime.Now > checkIn.AddDays(-advDay).Date.AddHours(advHour).AddMinutes(advMin))
                                    {
                                        pi.SellState = (int)PackageSellSate.canNotSell;
                                    }
                                    break;
                                case 12://	指定日期前订
                                    int advDay1 = Math.Abs(CommMethods.GetNightCount(firstRoomPrice.appointeddate, checkIn));
                                    int advHour1 = 18;
                                    int advMin1 = 0;

                                    p.TimeAdvance = (24 * (advDay1 + 1) - advHour1) * 60 - advMin1;
                                    if (DateTime.Now > firstRoomPrice.appointeddate)
                                    {
                                        pi.SellState = (int)PackageSellSate.canNotSell;
                                    }
                                    break;
                                case 13://	连住晚数
                                    p.DayLimitMin = firstRoomPrice.continuousdays;
                                    break;
                                case 14://	指定时间段能订
                                    p.StartDate = firstRoomPrice.beginday;
                                    p.EndDate = firstRoomPrice.endday;

                                    if (DateTime.Now < firstRoomPrice.beginday || DateTime.Now > firstRoomPrice.endday)
                                    {
                                        pi.SellState = (int)PackageSellSate.canNotSell;
                                    }
                                    break;
                            }

                        }

                    }


                    SetJLTourPackageRoomOptions(r, pi, pid, room);

                    if (pdes.Count > 0)
                    {
                        pi.PackageType = (int)HotelServiceEnums.PackageType.JLPackage;

                        pi.DailyItems = pdes;
                        pi.packageBase = p;
                        pi.Room = room;

                        pi.PayType = pi.DailyItems.Max(day => day.PayType);  // 3: 预付    4：待确认后支付（ 用户先提交订单，客服确认有房后，再付款）

                        if (pi.SellState == (int)PackageSellSate.canSell)
                        {
                            pi.SellState = (CheckPackageSealStateMore(pi.packageBase, checkIn, checkOut) == 1) ? (int)PackageSellSate.canSell : (int)PackageSellSate.canNotSell;
                        }

                        PIS.Add(pi);
                    }
                }
            }


            var plist = PIS.OrderBy(p => p.SellState).ThenBy(p => p.Price).ToList();
            AddPricePolicy(plist, ppt, checkIn, HotelServiceEnums.PackageType.JLPackage);
            return plist;
        }

        private static void SetJLTourPackageRoomOptions(JLHotelRoomEntity r, PackageInfoEntity pi, int pid, PRoomInfoEntity room)
        {
            List<PRoomOptionsEntity> RoomOptions = new List<PRoomOptionsEntity>();

            if (r.Bedtype != null)// && r.Bedtype.Split('/').Length > 1)
            {

                string options = r.Bedtype.Replace("/", ",");
                string DefaultOption = r.Bedtype.Split('/').Count() > 1 ? "双床" : r.Bedtype.Split('/')[0];

                room.Options = options;
                room.DefaultOption = DefaultOption;


                RoomOptions.Add(new PRoomOptionsEntity()
                {
                    Options = options,
                    Date = DateTime.Now,
                    DateType = 0,
                    DefaultOption = DefaultOption,
                    ID = 0,
                    PID = pid,
                    RoomID = r.Roomtypeid
                });

                pi.RoomOptions = RoomOptions;
            }
        }


        private static void SetTopTownPackageRoomOptions(TopTownHotelRoomEntity r, PackageInfoEntity pi, int pid, PRoomInfoEntity room)
        {
            List<PRoomOptionsEntity> RoomOptions = new List<PRoomOptionsEntity>();

            if (r.BedTypeName != null)// && r.Bedtype.Split('/').Length > 1)
            {

                string options = r.BedTypeName.Replace("/", ",");
                string DefaultOption = r.BedTypeName.Split('/').Count() > 1 ? "双床" : r.BedTypeName.Split('/')[0];

                room.Options = options;
                room.DefaultOption = DefaultOption;


                RoomOptions.Add(new PRoomOptionsEntity()
                {
                    Options = options,
                    Date = DateTime.Now,
                    DateType = 0,
                    DefaultOption = DefaultOption,
                    ID = 0,
                    PID = pid,
                    RoomID = r.RoomId
                });

                pi.RoomOptions = RoomOptions;
            }
        }

        private static void SetTopTownPackageDailyItem(TopTownHotelRoomEntity r, PackageInfoEntity pi, List<PackageDailyEntity> pdes, int pid, IEnumerable<TopTownHotelPriceEntity> temprps)
        {
            foreach (TopTownHotelPriceEntity rp in temprps)
            {
                PackageDailyEntity pde = new PackageDailyEntity();
                pde.Day = Convert.ToDateTime(rp.EffectiveDate);
                pde.Price = (int)rp.AmountAfterTax;
                pde.PayType = 3;// rp.Qtyable > 0 ? 3 : 4;//3.	预付  4.	待确认后支付（ 用户先提交订单，客服确认有房后，再付款）
                pde.Rebate = 0;//rp.Rebate;
                pde.ActiveRebate = 0; //rp.ActiveRebate;
                pde.CashCoupon = 0; // rp.CashCoupon;
                pde.CanUseCashCoupon = 0; // rp.CanUseCashCoupon;
                pde.Items = new List<PItemEntity>();


                //早餐项
                PItemEntity pie = new PItemEntity()
                {
                    Type = 1,
                    Description = rp.Breakfast == 0 ? "不含早" : rp.Breakfast == 1 ? "单早" : rp.Breakfast == 2 ? "双早" : rp.Breakfast == 3 ? "三早" : rp.Breakfast == 4 ? "四早" : rp.Breakfast == -2 ? "根据入住人数匹配早餐分数（注：最多2份）" : "",
                    Date = Convert.ToDateTime(rp.EffectiveDate),
                    DateType = 8,
                    HotelID = rp.HotelId,
                    ID = rp.RateId,
                    ItemCode =  rp.Breakfast == 0 ? "不含早" : rp.Breakfast == 1 ? "单早" : rp.Breakfast == 2 ? "双早" : rp.Breakfast == 3 ? "三早" : rp.Breakfast == 4 ? "四早" : rp.Breakfast == -2 ? "根据入住人数匹配早餐分数（注：最多2份）" : "",
                    PID = pid,
                    Price = 0
                };

                pde.Items.Add(pie);

                //条款类型:
                //11	提前预订
                //12	指定日期前订
                //13	连住晚数
                //14	指定时间段能订
                string termtypename = "需要提前1天预订";

                if (termtypename.Length > 0)
                {
                    PItemEntity termtypeItem = new PItemEntity()
                    {
                        Type = 1,
                        Description = termtypename,
                        Date = Convert.ToDateTime(rp.EffectiveDate),
                        DateType = 8,
                        HotelID = rp.HotelId,
                        ID = rp.RateId,
                        ItemCode = termtypename,
                        PID = pid,
                        Price = 0
                    };

                    pde.Items.Add(termtypeItem);
                }


                //if (rp.Qtyable < 0) //不可售
                //{
                //    pi.SellState = (int)PackageSellSate.canNotSell;
                //}

                pi.Price += (int)rp.AmountAfterTax;
                pi.Rebate += 0;
                pi.ActiveRebate += 0;
                pi.CashCoupon += 0;
                pi.CanUseCashCoupon +=0;

                pdes.Add(pde);
            }
        }


        private static void SetJLTourPackageDailyItem(JLHotelRoomEntity r, PackageInfoEntity pi, List<PackageDailyEntity> pdes, int pid, IEnumerable<JLHotelRoomPriceEntity> temprps)
        {
            foreach (JLHotelRoomPriceEntity rp in temprps)
            {
                PackageDailyEntity pde = new PackageDailyEntity();
                pde.Day = rp.Night.Date;
                pde.Price = (int)rp.Price;
                pde.PayType = rp.Qtyable > 0 ? 3 : 4;//3.	预付  4.	待确认后支付（ 用户先提交订单，客服确认有房后，再付款）
                pde.Rebate = rp.Rebate;
                pde.ActiveRebate = rp.ActiveRebate;
                pde.CashCoupon = rp.CashCoupon;
                pde.CanUseCashCoupon = rp.CanUseCashCoupon;
                pde.Items = new List<PItemEntity>();

                if (r.Remark2.Trim().Length > 0)
                {
                    pde.Items.Add(new PItemEntity()
                    {
                        Type = 1,
                        Description = r.Remark2.Trim(),
                        Date = DateTime.Now,
                        DateType = 0,
                        HotelID = 0,
                        ID = 0,
                        ItemCode = "Remark2",
                        PID = pid,
                        Price = 0
                    });
                }

                //早餐项
                PItemEntity pie = new PItemEntity()
                {
                    Type = 1,
                    Description = rp.Includebreakfastqty2,
                    Date = rp.Night,
                    DateType = 8,
                    HotelID = rp.HotelId,
                    ID = rp.ID,
                    ItemCode = rp.Includebreakfastqty2,
                    PID = pid,
                    Price = 0
                };

                pde.Items.Add(pie);

                //条款类型:
                //11	提前预订
                //12	指定日期前订
                //13	连住晚数
                //14	指定时间段能订
                string termtypename = "";
                switch (rp.termtype)
                {
                    case 11://	提前预订
                        termtypename = string.Format("需要提前{0}天{1}前预订", rp.advancedays, rp.advancetime);
                        break;
                    case 12://	指定日期前订
                        termtypename = string.Format("需在{0}前预订", rp.appointeddate);
                        break;
                    case 13://	连住晚数
                        termtypename = string.Format("需连住{0}晚", rp.continuousdays);
                        break;
                    case 14://	指定时间段能订
                        termtypename = string.Format("可预订时间段为{0}--{1}", rp.beginday, rp.endday);
                        break;
                }

                if (termtypename.Length > 0)
                {
                    PItemEntity termtypeItem = new PItemEntity()
                    {
                        Type = 1,
                        Description = termtypename,
                        Date = rp.Night,
                        DateType = 8,
                        HotelID = rp.HotelId,
                        ID = rp.ID,
                        ItemCode = termtypename,
                        PID = pid,
                        Price = 0
                    };

                    pde.Items.Add(termtypeItem);
                }


                if (rp.Qtyable < 0) //不可售
                {

                    pi.SellState = (int)PackageSellSate.canNotSell;
                }

                pi.Price += (int)rp.Price;
                pi.Rebate += rp.Rebate;
                pi.ActiveRebate += rp.ActiveRebate;
                pi.CashCoupon += rp.CashCoupon;
                pi.CanUseCashCoupon += rp.CanUseCashCoupon;

                pdes.Add(pde);
            }
        }

        /// <summary>
        /// 格式化套餐价格类型描述， 过滤掉“无早餐 无早餐”这样的类型
        /// </summary>
        /// <param name="priceTypeName"></param>
        /// <param name="breakfirstName"></param>
        /// <returns></returns>
        private static string formatPriceTypeDescription(string priceTypeName, string breakfirstName)
        {
            if (priceTypeName.Trim() == breakfirstName.Trim())
                return priceTypeName;
            else
                return priceTypeName + " " + breakfirstName;
        }

        private string CheckTimeSelected(string timeSelected)
        {
            string time = "12点";
            if (timeSelected.Trim() != "")
            {
                int hour = int.Parse(timeSelected.Split(':')[0]);
                if (hour <= 12)
                {
                    if (hour > 0)
                    {
                        time = string.Format("{0}点", hour - 1);
                    }
                }
            }

            return time;
        }

        public List<PDayItem> GetJLHotelPackageCalendar(int hotelid, DateTime startDate, HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType ppt = HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType.Default)
        {
            return GenJLHotelPackageCalendar(hotelid, startDate, ppt: ppt);

        }

        public int GenJLTourPackageID(JLHotelRoomPriceEntity rp)
        {
            return rp.RoomtypeId * 1000 + rp.ratetype;
        }

        public List<PDayItem> GenJLHotelPackageCalendar(int hotelid, DateTime startDate, int pid = 0, int canlendarLength = 360, HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType ppt = HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType.Default)
        {
            DateTime packageEndDate = startDate.AddDays(canlendarLength);
            DateTime endDate = startDate.AddDays(canlendarLength) < packageEndDate ? startDate.AddDays(canlendarLength) : packageEndDate;

            List<JLHotelRoomPriceEntity> rps = HotelDAL.GetJLHotelRoomPrice(hotelid, startDate, endDate);

            if (pid > 0) //只显示某个套餐的日历
            {
                for (int i = rps.Count - 1; i >= 0; i--)
                {
                    if (GenJLTourPackageID(rps[i]) != pid)
                        rps.RemoveAt(i);
                }
            }

            List<PDayItem> ds = new List<PDayItem>();
            if (rps.Count <= 0)
            {
                return ds;
            }

            for (int i = 0; i < canlendarLength; i++)
            {
                if (startDate.AddDays(i) > endDate) break;
                PDayItem d = new PDayItem();
                d.Day = startDate.AddDays(i).Date;
                d.MaxSealCount = 5;

                d.SellState = rps.Where(r => r.Night.Date == d.Day && r.Qtyable >= 0).Count() > 0 ? (int)PackageSellSate.canSell : (int)PackageSellSate.canNotSell;

                ds.Add(d);
            }

            List<PackageInfoEntity> ps = GetJLHotelPackages(hotelid, startDate, startDate.AddDays(1), ppt);


            DealTimeAdvance(ds, ps);

            return ds;
        }

        //处理有预订提前量要求的套餐         
        private static void DealTimeAdvance(List<PDayItem> ds, List<PackageInfoEntity> ps)
        {
            foreach (PackageInfoEntity p in ps.Where(p => p.packageBase.TimeAdvance > 0))
            {
                DateTime CanOrderDay = DateTime.Now.AddMinutes(p.packageBase.TimeAdvance).Date; ;

                foreach (PDayItem d in ds)
                {
                    PDayPItem pd = d.PItems.Where(dp => dp.PID == p.packageBase.ID).FirstOrDefault();
                    if (pd != null && pd.PID > 0)
                    {
                        if (pd.Day < CanOrderDay)
                        {
                            pd.SealState = 2;
                            d.SellState = d.PItems.Where(o => o.SealState == 1).Count() == 0 ? 2 : 1;//判断某天是否可销售
                        }
                        else
                            break;
                    }
                }

            }
        }

        public List<PDayItem> GetHotelPackageCalendar(int hotelid, DateTime startDate, HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType ppt = HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType.Default)
        {
            return GenHotelPackageCalendar(hotelid, startDate, ppt: ppt);

        }

        public List<PDayItem> GetHotelPackageCalendar30Cached(int hotelid, DateTime startDate, int pid = 0, int channelId = 0, HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType ppt = HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType.Default)
        {
            return GetHotelPackageCalendar30(hotelid, startDate, pid, channelId, ppt);
            return memCacheCanlendar.GetData<List<PDayItem>>(string.Format("{0}_{1}_{2}_{3}_{4}", hotelid, pid, startDate, channelId, (int)ppt), "PC30", () =>
                {
                    return GetHotelPackageCalendar30(  hotelid,   startDate,  pid  ,    channelId  ,  ppt  );
                });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hotelid"></param>
        /// <param name="startDate"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        public List<PDayItem> GetHotelPackageCalendar30(int hotelid, DateTime startDate, int pid = 0, int channelId = 0, HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType ppt = HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType.Default)
        {
            List<PDayItem> pl = new List<PDayItem>();
            try
            {
                if (pid > 0)
                {


                    if (channelId > 0)
                    {
                        switch (channelId)
                        {
                            //捷旅
                            case 3:
                                {
                                    channelId = 102;
                                    pl = GenJLHotelPackageCalendar(hotelid, startDate, pid, ppt: ppt);
                                    break;
                                }
                            //携程
                            case 2:
                            case 4:
                            case 5:
                            case 6:
                                {
                                    channelId = 103;
                                    pl = GenCtripHotelPackageCalendar(hotelid, startDate, pid, ppt: ppt);
                                    break;
                                }
                            //默认
                            default:
                                {
                                    channelId = 100;
                                    pl = GenHotelPackageCalendar(hotelid, startDate, pid, ppt: ppt);
                                    break;
                                }
                        }
                    }
                    else
                    {
                        pl = GenHotelPackageCalendar(hotelid, startDate, pid, ppt: ppt);
                        if (pl.Count() == 0)
                        {
                            //取携程
                            pl = GenCtripHotelPackageCalendar(hotelid, startDate, pid, ppt: ppt);

                            //携程没有则取捷旅的
                            if (pl.Count() == 0)
                            {
                                ////当没有设置需要携程套餐的情况下，才会先去查询捷旅的
                                //if (!NeedCtripPackage(hotelid))
                                {
                                    pl = GenJLHotelPackageCalendar(hotelid, startDate, pid, ppt: ppt);
                                }
                            }
                        }
                    }

                    //如果最后一天是可售，那么需要增加一天，这天不可售，以便用户可以选择最后一天下单
                    if (pl.Count > 0 && pl.Last().SellState == 1)
                    {
                        PDayItem ld = pl.Last();
                        PDayItem oneMoreDay = new PDayItem() { Day = ld.Day.AddDays(1), SellState = 2, MaxSealCount = 0, SoldCount = 0, PItems = ld.PItems };
                        pl.Add(oneMoreDay);
                    }

                }
                else
                {
                    pl = GenPureHotelPackageCalendar(startDate);
                }
            }
            catch (Exception e)
            {
                LogHelper.WriteLog("GetHotelPackageCalendar30 err：" + e.Message + e.StackTrace);
                pl = GenPureHotelPackageCalendar(startDate);
            }
            //晚上17点以后，不可预订当天入住订单
            if (startDate.Date == DateTime.Now.Date && DateTime.Now.Hour >= 17)
            {
                var today = pl.Where(p => p.Day.Date == DateTime.Now.Date);
                if (today.Count() > 0)
                {
                    today.First().SellState = (int)HJD.HotelServices.Contracts.HotelServiceEnums.SellState.cannotSell;
                }
            }

            return pl;
        }

        /// <summary>
        /// 获取酒套餐每日可售数量，合并床态控制信息
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public List<PCountEntity> GetPCountInfo(List<PackageInfoEntity> ps, int hotelid, DateTime startDate, DateTime endDate, int OnePID = 0)
        {
            List<PCountEntity> cs = HotelDAL.GetPCount(hotelid, OnePID);

            //每个套餐床型控制的最小房型数
            List<HotelPackageRoomBedCountEntity> nbl = GetHotelPackageRoomBedCount(hotelid, startDate, endDate);

            foreach (PackageInfoEntity p in ps)
            {

                int roomid = p.packageBase.RoomID;
                int pid = p.packageBase.ID;


                //确保有平日数据
                if (cs.Where(c => c.PID == pid && c.DateType == 0).Count() == 0)
                {
                    PCountEntity normal = new PCountEntity() { Count = DEFAULT_ROOM_BED_COUNT * 2, DateType = 0, Date = DateTime.Now, ID = roomid, PID = pid };
                    cs.Add(normal);
                }


                //房态中增加如果定义了周一到周六等的数据，那么也按最小的数据来写
                //如果有相同的定义，取最严的定义，也就是房间数量最少的定义
                int NormalDayCount = 100;
                var existN = cs.Where(c => c.DateType == 0 && c.PID == pid);
                if (existN.Count() > 0)
                {
                    NormalDayCount = existN.First().Count;
                }

                foreach (HotelPackageRoomBedCountEntity n in nbl.Where(n => n.RoomID == roomid))
                {
                    PCountEntity pc = new PCountEntity() { Count = n.RCount, Date = n.Date, DateType = 8, ID = roomid, PID = pid };

                    int weekday = GetDateWeekNo(n.Date);

                    //如果有相同的定义，取最严的定义，也就是房间数量最少的定义
                    var existC = cs.Where(c => c.DateType == 8 && c.Date == n.Date && c.PID == pid);
                    var existW = cs.Where(c => c.DateType == weekday && c.PID == pid);


                    if (existC.Count() > 0 && existW.Count() > 0)
                    {
                        if (pc.Count < existC.First().Count && pc.Count < existW.First().Count)
                        {
                            cs.Insert(0, pc);
                        }
                    }
                    else if (existC.Count() > 0)
                    {
                        if (pc.Count < existC.First().Count)
                        {
                            cs.Insert(0, pc);
                        }
                    }
                    else if (existW.Count() > 0)
                    {
                        if (pc.Count < existW.First().Count)
                        {
                            cs.Insert(0, pc);
                        }
                    }
                    else if (pc.Count < NormalDayCount)
                    {
                        cs.Insert(0, pc);
                    }

                }
            }

            return cs;
        }

        public int GetDateWeekNo(DateTime date)
        {
            int weekday = (int)date.DayOfWeek;
            weekday = weekday == 0 ? 7 : weekday; //星期日=0，需要处理。 数据库定义中0为任意时间
            return weekday;
        }


        public List<PDayItem> GenHotelPackageCalendarInfo(int hotelid, DateTime startDate, List<PackageInfoEntity> ps, IEnumerable<RoomSouldCountEntity> rsList, int pid = 0, int canlendarLength = 360, 
            HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType ppt = HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType.Default)
        {

            TimeLog log = new TimeLog(string.Format("GenHotelPackageCalendarInfo pid：{0} hotelid:{1} startDate:{2} endDate:{3}", pid, hotelid, startDate, canlendarLength), 1000, null);
            //List<PackageInfoEntity> ps = GetHotelPackages(hotelid, pid);

            log.AddLog("GetHotelPackages");
            if (ps.Count() == 0)
            {
                return new List<PDayItem>();
            }


            DateTime packageEndDate = ps.Max(p => p.packageBase.EndDate);
            DateTime endDate = startDate.AddDays(canlendarLength) < packageEndDate ? startDate.AddDays(canlendarLength) : packageEndDate.AddDays(1);

            //获取每天可销售房间数量
            List<PCountEntity> pCountList = GetPCountInfo(ps, hotelid, startDate, endDate, pid);
            log.AddLog("GetPCountInfo");

            //IEnumerable<RoomSouldCountEntity> rsList = GetHotelRoomSouldInfo(hotelid, startDate, endDate);
            //log.AddLog("GetHotelRoomSouldInfo");


            if (pid > 0) //只显示某个套餐的日历
            {
                for (int i = ps.Count - 1; i >= 0; i--)
                {
                    if (ps[i].packageBase.ID != pid)
                        ps.RemoveAt(i);
                }

                for (int i = pCountList.Count - 1; i >= 0; i--)
                {
                    if (pCountList[i].PID != pid)
                        pCountList.RemoveAt(i);
                }
            }

            log.AddLog("if (pid > 0)");
            List<PDayItem> ds = new List<PDayItem>();
            for (int i = 0; i < canlendarLength; i++)
            {
                if (startDate.AddDays(i) > endDate) break;
                PDayItem d = new PDayItem();
                d.Day = startDate.AddDays(i).Date;

                FillDailyItems(pCountList, ps, d, rsList);

                ds.Add(d);
            }
            log.AddLog("for (int i = 0");

            //设置日历每日价格 考虑不同用户的优惠价 普通用户无优惠  2016.05.25 wwb 测试问题
            genHotelPackageCalendarPriceList(pid, hotelid, ds, ppt);
            log.AddLog("genHotelPackageCalendarPriceList");

            //处理有预订提前量要求的套餐
            DealTimeAdvance(ds, ps);
            log.AddLog("DealTimeAdvance");

            log.Finish();
            return ds;
        }


        public List<PDayItem> GenHotelPackageCalendar(int hotelid, DateTime startDate, int pid = 0, int canlendarLength = 360, HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType ppt = HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType.Default)
        {

            //TimeLog log = new TimeLog(string.Format("GenHotelPackageCalendar pid：{0} hotelid:{1} startDate:{2} endDate:{3}", pid, hotelid, startDate, canlendarLength), 1000, null);
            List<PackageInfoEntity> ps = GetHotelPackages(hotelid, pid);

            //log.AddLog("GetHotelPackages");
            //  LogHelper.WriteLog(string.Format("GenHotelPackageCalendar:{0}:{1}:{2}", hotelid,pid,ps.Count()));

            if (ps.Count() == 0)//|| ps.Where(p => p.packageBase.IsValid == true).Count() == 0)
            {
                return new List<PDayItem>();
            }

            DateTime packageEndDate = ps.Max(p => p.packageBase.EndDate);

            //是否是套餐的结束售卖日期
            bool isPacakgeEndDate = startDate.AddDays(canlendarLength) < packageEndDate;

            DateTime endDate = isPacakgeEndDate ? startDate.AddDays(canlendarLength) : packageEndDate.AddDays(1);

            //获取每天可销售房间数量
            List<PCountEntity> pCountList = GetPCountInfo(ps, hotelid, startDate, endDate, pid);
            //log.AddLog("GetPCountInfo");

            IEnumerable<RoomSouldCountEntity> rsList = GetHotelRoomSouldInfo(hotelid, startDate, endDate);
            //log.AddLog("GetHotelRoomSouldInfo");


            if (pid > 0) //只显示某个套餐的日历
            {
                for (int i = ps.Count - 1; i >= 0; i--)
                {
                    if (ps[i].packageBase.ID != pid)
                        ps.RemoveAt(i);
                }

                for (int i = pCountList.Count - 1; i >= 0; i--)
                {
                    if (pCountList[i].PID != pid)
                        pCountList.RemoveAt(i);
                }
            }

            //log.AddLog("if (pid > 0)");
            List<PDayItem> ds = new List<PDayItem>();
            for (int i = 0; i < canlendarLength; i++)
            {
                if (startDate.AddDays(i) > endDate)
                {
                    break;
                }
                PDayItem d = new PDayItem();
                //PDayItem d = new PDayItem();
                d.Day = startDate.AddDays(i).Date;

                FillDailyItems(pCountList, ps, d, rsList);

                ds.Add(d);
            }
            //log.AddLog("for (int i = 0");

            try
            {
                //如果不是结束售卖日期 多加一天并设置为不可售，去价格时的逻辑是 小于endDate 否则endDate取不到价格
                if (isPacakgeEndDate)
                {
                    PDayItem d = new PDayItem();
                    d.Day = ds.Max(_ => _.Day).Date.AddDays(1).Date;
                    d.SellState = 2;
                    ds.Add(d);
                }
            }
            catch (Exception e)
            {
            }

            //设置日历每日价格 考虑不同用户的优惠价 普通用户无优惠  2016.05.25 wwb 测试问题
            genHotelPackageCalendarPriceList(pid, hotelid, ds, ppt);
            //log.AddLog("genHotelPackageCalendarPriceList");

            //处理有预订提前量要求的套餐
            DealTimeAdvance(ds, ps);
            //log.AddLog("DealTimeAdvance");

            //log.Finish();

            //foreach (PDayItem d in ds)
            //{
            //    foreach (PDayPItem dp in d.PItems)
            //    {
            //        if (dp.SealState == 0)
            //        {
            //            dp.SealState = dp.MaxSealCount > dp.SoldCount ? 1 : 2;//判断每个套餐是否可销售
            //        }
            //    }

            //    d.SellState = d.PItems.Where(p => p.SealState == 1).Count() == 0 ? 2 : 1;//判断某天是否可销售
            //}



            return ds;
        }

        private void genHotelPackageCalendarPriceList(int pid, int hotelid, IEnumerable<PDayItem> ds, HotelServiceEnums.PricePolicyType ppt)
        {
            if (pid * hotelid == 0 || ds == null || ds.Count() == 0)
            {
                return;
            }
            var packageRateList = GetHotelPackageRateList(hotelid, ds.Min(_ => _.Day), ds.Max(_ => _.Day), ppt, pid);
            if (packageRateList == null)
            {
                return;
            }
            else
            {
                var packageRate = GetHotelPackageRateList(hotelid, ds.Min(_ => _.Day), ds.Max(_ => _.Day), ppt, pid).FirstOrDefault();
                if (packageRate == null || packageRate.Price == 0)
                {
                    return;
                }

                foreach (var pDayItem in ds)
                {
                    foreach (var item in packageRate.DailyList)
                    {
                        if (pDayItem.Day.Date == item.Day.Date)
                        {
                            pDayItem.SellPrice = item.Price;
                            pDayItem.NormalPrice = item.NotVIPPrice;
                            pDayItem.VipPrice = item.VIPPrice;
                            pDayItem.AutoCommission = item.AutoCommission;
                            pDayItem.ManualCommission = item.ManualCommission;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        ///计算每天最多可以卖几份套餐
        /// </summary>
        /// <param name="cs"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        private void FillDailyItems(List<PCountEntity> cs, List<PackageInfoEntity> ps, PDayItem d, IEnumerable<RoomSouldCountEntity> rsList)
        {
            DateTime dateTime = d.Day.Date;

            int maxSealCount = 0;
            int souldCount = 0;
            PackageEntity pe = new PackageEntity();
            foreach (int pid in cs.Select(c => c.PID).Distinct())
            {
                pe = ps.Where(p => p.packageBase.ID == pid).FirstOrDefault().packageBase;
                if (pe.EndDate < dateTime || pe.StartDate > dateTime) continue;//如果套餐已结束，则不记入

                PDayPItem pitem = new PDayPItem();
                pitem.Day = dateTime;
                pitem.PID = pid;
                pitem.MaxSealCount = CalOnePackageMaxSealCount(cs, pid, dateTime);
                maxSealCount += pitem.MaxSealCount;

                var rs = rsList.Where(r => r.Date == dateTime && r.RoomID == pe.RoomID);
                if (rs.Count() > 0)
                {
                    pitem.SoldCount = rs.First().SouldCount;

                }
                else
                {
                    pitem.SoldCount = 0;
                }
                souldCount += pitem.SoldCount;
                pitem.SealState = pitem.SoldCount >= pitem.MaxSealCount ? 2 : 1;

                d.PItems.Add(pitem);

            }

            d.MaxSealCount = maxSealCount;
            d.SoldCount = souldCount;
            d.SellState = d.PItems.Where(i => i.SealState == 1).Count() == 0 ? 2 : 1;
        }

        private int CalOnePackageMaxSealCount(List<PCountEntity> cs, int pid, DateTime dateTime)
        {
            //取指定日期条目
            List<PCountEntity> Items = cs.Where(r => r.PID == pid && r.DateType == 8 && r.Date == dateTime).ToList();

            #region 如果不是指定日期条目，取指定节假日或星期条目

            if (Items == null || Items.Count == 0)
            {
                #region 优先获取节假日

                var holidayDateType = 10;

                //存在节假日设置
                if (cs.Exists(r => r.PID == pid && r.DateType == holidayDateType))
                {
                    //读取节假日
                    List<HJD.HotelManagementCenter.Domain.HolidayInfoEntity> holidayList = memCacheHotelCtripPackage.GetData<List<HJD.HotelManagementCenter.Domain.HolidayInfoEntity>>(string.Format("GetHolidays"),
                    () => { return HotelDAL.GetHolidays() ?? new List<HJD.HotelManagementCenter.Domain.HolidayInfoEntity>(); });

                    //筛选出中国的节假日
                    holidayList = holidayList.Where(h => h.CountryCode == "cn" && h.Holiday == 1).ToList();

                    //当前日期是节假日
                    if (holidayList.Exists(hd => hd.Day.Date == dateTime.Date))
                    {
                        Items = cs.Where(r => r.PID == pid && r.DateType == holidayDateType).ToList();
                    }
                }

                #endregion

                #region 在没有节假日的情况下，取星期条目

                //星期日=0，需要处理。 数据库定义中0为任意时间
                if (Items == null || Items.Count == 0)
                {
                    int weekday = (int)dateTime.DayOfWeek;
                    weekday = weekday == 0 ? 7 : weekday;
                    Items = cs.Where(r => r.PID == pid && r.DateType == weekday).ToList();
                }

                #endregion

                #region 星期条目也没有的情况下，取平日价格

                //取平日价格
                if (Items == null || Items.Count == 0)
                {
                    Items = cs.Where(r => r.PID == pid && r.DateType == 0).ToList();
                }

                #endregion

            }

            #endregion

            return Items.Count > 0 ? Items.First().Count : 0;
        }

        public static int GetNightCount(DateTime CheckIn, DateTime CheckOut)
        {
            TimeSpan ts1 = new TimeSpan(CheckIn.Date.Ticks);
            TimeSpan ts2 = new TimeSpan(CheckOut.Date.Ticks);
            TimeSpan ts = ts1.Subtract(ts2).Duration();
            return ts.Days;
        }

        public static int GetNightCount(Int32 CheckIn, DateTime CheckOut)
        {
            DateTime CheckInDate = DateTime.Parse(String.Format("{0}-{1}-{2}", CheckIn / 10000, (CheckIn % 10000) / 100, CheckIn % 100));
            return GetNightCount(CheckInDate, CheckOut);
        }

        private int GenIntDate(DateTime date)
        {
            return date.Year * 10000 + date.Month * 100 + date.Day;
        }

        #region 携程抓取套餐打包

        /// <summary>
        /// 检查指定酒店是否需要解析携程酒店套餐数据
        /// </summary>
        /// <param name="hotelid"></param>
        /// <returns></returns>
        public bool NeedCtripPackage(int hotelid)
        {
            var need = false;

            var hoteloriid = HotelDAL.GetHotelContactsFor3(hotelid);
            if (hoteloriid > 0)
            {
                need = true;
            }

            return need;
        }

        /// <summary>
        /// 【页面抓取】根据酒店和入住日期获取携程的酒店套餐信息(4.2版本开始使用 2016-01-11 haoy)
        /// </summary>
        /// <param name="hotelid"></param>
        /// <param name="checkIn"></param>
        /// <param name="checkOut"></param>
        /// <param name="updatePrice"></param>
        /// <returns></returns>
        public List<PackageInfoEntity> GetCtripHotelPackagesV42(int hotelid, DateTime checkIn, DateTime checkOut, bool updatePrice, HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType ppt = HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType.Default)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                #region 得到套餐

                //缓存key
                var cacheKey = string.Format("CtripPgV42{0}:{1}:{2}:{3}", hotelid, checkIn.Date, checkOut.Date, (int)ppt);

                sb.Append("GetCtripHotelPackagesV42:" + cacheKey);

                //这里有个10分钟的缓存机制
                var packages = memCacheHotelCtripPackage.GetData<List<PackageInfoEntity>>(cacheKey,
                () =>
                {
                    sb.Append("LoadCtripHotelPackagesV42");
                    return LoadCtripHotelPackagesV42(hotelid, checkIn, checkOut, ppt);
                });

                //如果没有拿到套餐，则清楚缓存，下次重新读取
                if (packages == null || packages.Count <= 0)
                {
                    //删除缓存
                    memCacheHotelCtripPackage.Remove(cacheKey);
                    sb.Append("将null放至缓存");
                    //将null放至缓存
                    var data = memCacheHotelCtripPackage.GetData<List<PackageInfoEntity>>(cacheKey,
                    () =>
                    {
                        return null;
                    });
                }

                #endregion

                #region 其它操作（如 触发更新酒店列表价）

                if (updatePrice)
                {
                    PublishUpdatePriceSlotTask(hotelid, checkIn);
                }

                #endregion
                //   LogHelper.WriteLog(sb.ToString());
                return packages;
            }
            catch (Exception err)
            {
                LogHelper.WriteLog(sb.ToString() + err.Message + err.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// 【对接API】根据酒店和入住日期获取携程的酒店套餐信息
        /// </summary>
        /// <param name="hotelid"></param>
        /// <param name="checkIn"></param>
        /// <param name="checkOut"></param>
        /// <param name="updatePrice"></param>
        /// <param name="ppt"></param>
        /// <returns></returns>
        public List<PackageInfoEntity> GetCtripHotelPackagesForApiV42(int hotelid, DateTime checkIn, DateTime checkOut, bool updatePrice, HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType ppt = HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType.Default)
        {
          //  LogHelper.WriteLog("GetCtripHotelPackagesForApiV42");
            #region 得到套餐
            //var packages = LoadCtripHotelPackagesV42(hotelid, checkIn, checkOut, ppt, 1);

            //缓存key
            var cacheKey = string.Format("GetCtripHotelPackagesForApiV42{0}:{1}:{2}:{3}", hotelid, checkIn.Date, checkOut.Date, (int)ppt);

            //这里有3分钟的缓存机制
            var packages = memCacheHotelCtripPackage.GetData<List<PackageInfoEntity>>(cacheKey,
            () =>
            {
                return LoadCtripHotelPackagesV42(hotelid, checkIn, checkOut, ppt, 1);
            });
           
            //如果没有拿到套餐，则清楚缓存，下次重新读取
            if (packages == null || packages.Count <= 0)
            {
                //删除缓存
                memCacheHotelCtripPackage.Remove(cacheKey);

                //将null放至缓存
                var data = memCacheHotelCtripPackage.GetData<List<PackageInfoEntity>>(cacheKey,
                () =>
                {
                    return null;
                });
            }

            #endregion

            #region 其它操作（如 触发更新酒店列表价）

            if (updatePrice)
            {
                PublishUpdatePriceSlotTask(hotelid, checkIn);
            }

            #endregion

            return packages;
        }

        public List<PackageInfoEntity> LoadCtripHotelPackagesV42(int hotelid, DateTime checkIn, DateTime checkOut, HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType ppt = HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType.Default, int crawlType = 0)
        {
            List<PackageInfoEntity> packageList = new List<PackageInfoEntity>();

            try
            {
                #region 根据酒店获得抓取的相关配置参数

                var channelId = 103;

                var otaPackageConfig = otaCrawlerService.GetPackageSourceConfigByHotelId(channelId, Convert.ToInt64(hotelid));

                if (otaPackageConfig == null) { return new List<PackageInfoEntity>(); }

                #endregion

                #region 抓取

                //抓取的携程套餐
                var cHotel = new CrawlerHotel();

                //当需要缓存的时候，才会去更新套餐数据 调取实时抓取，返回抓取对象
                switch (crawlType)
                {
                    //对接API
                    case 1:
                        {
                            cHotel = otaCrawlerService.Ctrip_CrawlHotelPackageForApiV42(otaPackageConfig, checkIn, checkOut, false);
                            break;
                        }
                    //页面抓取
                    default:
                        {
                            cHotel = otaCrawlerService.Ctrip_CrawlHotelPackageV42(otaPackageConfig, checkIn, checkOut, false);
                            break;
                        }
                }

                //补救措施，如果页面抓取没有拿到套餐，则走API返回
                if (crawlType == 0 && (cHotel == null || !cHotel.HasRoom || cHotel.HotelBaseRoomList == null || cHotel.HotelBaseRoomList.Count <= 0))
                {
                    cHotel = otaCrawlerService.Ctrip_CrawlHotelPackageForApiV42(otaPackageConfig, checkIn, checkOut, false);
                }

                #endregion

                #region 封装抓取对象为OTAPriceCtrip对象

                //将抓取对象转换为携程套餐实例集合
                var otaPriceList = CtripHotelPackageEngine.GetOtaCtripPriceByCrawlHotel(hotelid, otaPackageConfig.HotelOriId, cHotel);

                #endregion

                #region 插入抓取

                var insert = CtripHotelPackageEngine.InsertPackageList(hotelid, otaPackageConfig.HotelOriId, otaPriceList);

                #endregion

                #region 读取抓取

                //这里直接读取数据库中的套餐情况
                packageList = CtripHotelPackageEngine.GetPackageListV42(cHotel, checkIn, checkOut, otaPackageConfig, crawlType);

                #endregion
            }
            catch (Exception ex)
            {
                //File.AppendAllText(string.Format(@"D:\Log\HotelService\LoadCtripHotelPackagesV42_{0}_Log.txt", DateTime.Now.ToString("yyyyMMdd")), string.Format("{0}-{1} \r\n", "Err:" + ex.Message + "\r\n" + ex.StackTrace, DateTime.Now));
            }

            AddPricePolicy(packageList, ppt, checkIn, HotelServiceEnums.PackageType.CtripPackage);
            //LogHelper.WriteLog(string.Format("LoadCtripHotelPackagesV42:{0}", packageList.Count));

            //if (ppt == HotelServiceEnums.PricePolicyType.VIP)
            //{
            //    foreach (var pi in packageList)
            //    {
            //        pi.CustomerAskPrice.Text = "获取更低价>>";
            //        pi.CustomerAskPrice.Description = "联系顾问可能";
            //        var cancelPolicy = pi.LastCancelTime <= DateTime.Now ? 1 : 2;
            //        pi.CustomerAskPrice.ActionUrl = "http://www.zmjiudian.com/MagiCall/MagiCallClient?checkIn=" + checkIn + "&checkOut=" + checkOut + "&PCode=" + pi.packageBase.SerialNO + "&price=" + pi.NotVIPPrice + "&VIPPrice=" + pi.VIPPrice + "&hotelId=" + pi.packageBase.HotelID + "&payType=" + pi.PayType + "&cancelPolicy=" + cancelPolicy + "&userid={userid}&realuserid=1";
            //    }
            //}

          //  GenOneVIPFirstpayPackage(  hotelid,packageList);

            return packageList;
        }


        /// <summary>
        ///  将携程订单中最低价的套餐生成一个VIP首单
        /// </summary>
        /// <param name="packageList"></param>
        private void GenOneVIPFirstpayPackage(int hotelid , List<PackageInfoEntity> packageList)
        {
                 if (packageList.Count > 0)
                {
                    PackageInfoEntity newVIPPackage = new PackageInfoEntity();

                    var pList = packageList.Where(_ => _.Price - 205 > 0 && _.PayType == 3 );
                    if (pList.Count() > 0)
                    {
                        if (GetHotelCtripPricePolicy(hotelid) == 0)
                        {
                            newVIPPackage = ObjectCopier.Clone<PackageInfoEntity>(pList.OrderBy(_ => _.Price).First());

                            GenCtripPackage(newVIPPackage);
                            packageList.Add(newVIPPackage);
                        }
                    }
                }
        }

        private void GenCtripPackage(PackageInfoEntity vipPackage)
        {
        //    vipPackage.PackageType = (int)HotelServiceEnums.PackageType.HotelPackage;
            vipPackage.VIPPrice = vipPackage.NotVIPPrice - 200;
            vipPackage.Price = vipPackage.VIPPrice;
            vipPackage.packageBase.SerialNO = vipPackage.packageBase.SerialNO + "（新VIP专享）";
            vipPackage.packageBase.Brief = "*新VIP专享 " + vipPackage.packageBase.Brief;
            vipPackage.packageBase.Code = vipPackage.packageBase.Code + "（新VIP专享）";
            vipPackage.packageBase.ID = -vipPackage.packageBase.ID;
            vipPackage.Room.PID = -vipPackage.Room.PID;
            vipPackage.packageBase.ForVIPFirstBuy = true;
        }


        /// <summary>
        /// 获取对应携程酒店某个套餐的价格策略
        /// </summary>
        /// <param name="hotelid"></param>
        /// <param name="CheckIn"></param>
        /// <param name="CheckOut"></param>
        /// <param name="ppt"></param>
        /// <returns></returns>
        public PackageRateEntity GetCtripHotelPackageRateList(int hotelid, int pid, HJD.HotelServices.Contracts.HotelServiceEnums.PackageType packageType, DateTime CheckIn, DateTime CheckOut, HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType ppt = HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType.Default)
        {
            CheckIn = CheckIn.Date;
            CheckOut = CheckOut.Date;
            PackageRateEntity packageRate = new PackageRateEntity();

            CrawlerHotelRoom pricerate = null;
            CrawlerHotelBaseRoom baseRoom = null;
            var channelId = 103;


            try
            {

                //根据携程抓取套餐ID获取套餐对象
                //var roomId = HotelDAL.GetOtaRoomIdCtripByIdx(pid);
                var roomId = CtripHotelPackageEngine.GetOtaRoomIdCtripByIdx(pid>0?pid:-pid);

                //获取平台酒店的OTA套餐配置
                var otaPackageConfig = otaCrawlerService.GetPackageSourceConfigByHotelId(channelId, Convert.ToInt64(hotelid));

                //根据pid去实时查询携程的房态信息
                HotelRoomParams hotelRoomParams = new HotelRoomParams
                {
                    HotelID = otaPackageConfig.HotelOriId.ToString(),
                    MineHotelID = otaPackageConfig.HotelId.ToString(),
                    OtaPackageConfig = otaPackageConfig,
                    CheckIn = CheckIn.ToString("yyyy-MM-dd"),
                    CheckOut = CheckOut.ToString("yyyy-MM-dd"),
                    OtaType = HJD.OtaCrawlerService.Contract.OtaType.Ctrip,
                    RoomID = roomId
                };

                #region 拿到符合要求的一个房型信息

                var hotel = new CrawlerHotel();
                switch (packageType)
                {
                    case HotelServiceEnums.PackageType.CtripPackageByApi:
                        hotel = otaCrawlerService.GetCanSellHotelForApiV42(hotelRoomParams);
                        break;
                    default:
                        hotel = otaCrawlerService.GetCanSellHotelV42(hotelRoomParams);
                        break;
                }

                if (hotel != null && hotel.HotelBaseRoomList != null && hotel.HotelBaseRoomList.Count > 0)
                {
                    baseRoom = hotel.HotelBaseRoomList.Find(hr => hr.HotelRoomList != null && hr.HotelRoomList.Exists(r => r.CanSell == 1 && r.RoomID.ToLower().Trim() == roomId.ToLower().Trim()));
                    if (baseRoom != null && baseRoom.HotelRoomList != null && baseRoom.HotelRoomList.Count > 0)
                    {
                        var roomList = baseRoom.HotelRoomList.Where(r => r.CanSell == 1 && r.RoomID.ToLower().Trim() == roomId.ToLower().Trim()).ToList();

                        pricerate = roomList.First();
                    }
                }
                #endregion

                if (pricerate != null)
                {

                    if (pricerate.DetailPriceList == null || pricerate.DetailPriceList.Count == 0)
                    {
                        pricerate.DetailPriceList = new List<decimal> { pricerate.Price };
                    }

                    PackageRateEntity p = new PackageRateEntity { HotelID = hotelid, startDate = CheckIn, endDate = CheckOut, PID = pid };


                    decimal settleRate = 1;
                    if (packageType == HotelServiceEnums.PackageType.CtripPackageForHotel) // OrderType.CtripPackageForHotel
                    {
                        var hotelContacts = HotelDAL.GetHotelContacts(hotelid);

                        if (hotelContacts.SettleRate > 0)
                        {
                            settleRate = Convert.ToDecimal(1) - (Convert.ToDecimal(hotelContacts.SettleRate) / 100);
                        }
                    }


                    for (int i = 0; CheckIn.AddDays(i).Date < CheckOut.Date; i++)
                    {
                        PackageDailyRateEntity pdr = new PackageDailyRateEntity { Day = CheckIn.AddDays(i) };

                        PackageRateItemEntity pr = new PackageRateItemEntity
                        {
                            ProductID = 10,  //携程作为房间供应商的ID
                            SupplierID = hotelid,
                            SupplierType = (int)HJD.HotelServices.Contracts.HotelServiceEnums.SupplierType.RoomSupplier
                        };

                        pr.Price = (int)pricerate.DetailPriceList[i];
                        pr.SettlePrice = (int)(pr.Price * settleRate);
                        pr.Rebate = 0;
                        pr.PayType = HJD.HotelServices.Implement.Helper.Channel.Ctrip.CtripHotelPackageEngine.GetZmjdPayType(pricerate.PayType);
                        pr.ActiveRebate = 0;
                        pr.CanUseCashCoupon = 0;
                        pr.CashCoupon = 0;
                        pr.CanUseCashCouponForBoTao = 0;

                        pdr.ItemList.Add(pr);

                        pdr.Price = pdr.ItemList.Sum(o => o.Price);
                        pdr.SettlePrice = pdr.ItemList.Sum(o => o.SettlePrice);
                        pdr.Rebate = pdr.ItemList.Sum(o => o.Rebate);


                        pdr.ActiveRebate = 0;
                        pdr.CanUseCashCoupon = 0;
                        pdr.CashCoupon = 0;
                        pdr.CanUseCashCouponForBoTao = 0;//加博涛

                        //if (pdr.Rebate > 0) //如果有返现，那么不能用现金券
                        //{
                        //    pdr.ActiveRebate = 0;
                        //    pdr.CanUseCashCoupon = 0;
                        //    pdr.CashCoupon = 0;
                        //    pdr.CanUseCashCouponForBoTao = 0;//加博涛
                        //}
                        //else
                        //{
                        //    pdr.ActiveRebate = pdr.ItemList.Sum(o => o.ActiveRebate);
                        //    pdr.CanUseCashCoupon = pdr.ItemList.Sum(o => o.CanUseCashCoupon);
                        //    pdr.CashCoupon = pdr.ItemList.Sum(o => o.CashCoupon);
                        //    pdr.CanUseCashCouponForBoTao = pdr.ItemList.Sum(o => o.CanUseCashCouponForBoTao);
                        //}

                        pdr.PayType = pdr.ItemList.Max(o => o.PayType);
                        p.DailyList.Add(pdr);

                    }
                    p.Price = p.DailyList.Sum(o => o.Price);
                    p.SettlePrice = p.DailyList.Sum(o => o.SettlePrice);
                    p.Rebate = p.DailyList.Sum(o => o.Rebate);
                    p.ActiveRebate = p.DailyList.Sum(o => o.ActiveRebate);
                    p.CanUseCashCoupon = p.DailyList.Sum(o => o.CanUseCashCoupon);
                    p.CanUseCashCouponForBoTao = p.DailyList.Sum(o => o.CanUseCashCouponForBoTao);
                    p.CashCoupon = p.DailyList.Sum(o => o.CashCoupon);
                    p.PayType = p.DailyList.Max(o => o.PayType);

                    packageRate = p;

                    AddRatePricePolicy(new List<PackageRateEntity> { packageRate }, ppt, CheckIn, HotelServiceEnums.PackageType.CtripPackage);

                    LogHelper.WriteLog(string.Format("{0} {1} {2} {3}", packageRate.HotelID, ppt.ToString(), packageRate.PayType, packageRate.Rebate));
                }
            }
            catch (Exception e)
            {
                LogHelper.WriteLog(string.Format("GetCtripHotelPackageRateList ERR:{0} {1} {2} {3} {4} {5} {6}", hotelid, pid, packageType.ToString(), CheckIn, CheckOut, e.Message, e.StackTrace));
            }

            return packageRate;
        }


        /// <summary>
        /// 【4.2之前的抓取方式】根据酒店和入住日期获取携程的酒店套餐信息(4.2之前版本使用 2016-01-11 haoy)
        /// </summary>
        /// <param name="hotelid"></param>
        /// <param name="checkIn"></param>
        /// <param name="checkOut"></param>
        /// <param name="updatePrice"></param>
        /// <returns></returns>
        public List<PackageInfoEntity> GetCtripHotelPackages(int hotelid, DateTime checkIn, DateTime checkOut, bool updatePrice, HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType ppt = HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType.Default)
        {
              #region 得到套餐

            //这里有个10分钟的缓存机制
            var packages = memCacheHotelCtripPackage.GetData<List<PackageInfoEntity>>(string.Format("GetCtripHotelPackages{0}:{1}:{2}:{3}", hotelid, checkIn.Date, checkOut.Date, (int)ppt),
            () =>
            {
                return LoadCtripHotelPackages(hotelid, checkIn, checkOut, ppt);
            });

            #endregion

            #region 其它操作（如 触发更新酒店列表价）

            if (updatePrice)
            {
                PublishUpdatePriceSlotTask(hotelid, checkIn);
            }

            #endregion

            return packages;
        }

        public List<PackageInfoEntity> LoadCtripHotelPackages(int hotelid, DateTime checkIn, DateTime checkOut, HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType ppt = HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType.Default)
        {
            try
            {
                #region 根据酒店获得抓取的相关配置参数

                var channelId = 103;

                var otaPackageConfig = otaCrawlerService.GetPackageSourceConfigByHotelId(channelId, Convert.ToInt64(hotelid));

                if (otaPackageConfig == null) { return new List<PackageInfoEntity>(); }

                #endregion

                #region 抓取

                //抓取的携程套餐
                var cHotel = new CrawlerHotel();

                //当需要缓存的时候，才会去更新套餐数据
                //调取实时抓取，返回抓取对象
                cHotel = otaCrawlerService.Ctrip_CrawlHotelPackage(otaPackageConfig, checkIn, checkOut, false);

                #endregion

                #region 封装抓取对象为OTAPriceCtrip对象

                //将抓取对象转换为携程套餐实例集合
                var otaPriceList = CtripHotelPackageEngine.GetOtaCtripPriceByCrawlHotel(hotelid, otaPackageConfig.HotelOriId, cHotel);

                #endregion

                #region 插入抓取

                var insert = CtripHotelPackageEngine.InsertPackageList(hotelid, otaPackageConfig.HotelOriId, otaPriceList);

                #endregion

                #region 读取抓取

                //这里直接读取数据库中的套餐情况
                var packageList = CtripHotelPackageEngine.GetPackageList(cHotel, checkIn, checkOut, otaPackageConfig);

                #endregion


                AddPricePolicy(packageList, ppt, checkIn, HotelServiceEnums.PackageType.CtripPackage);

                //if (ppt == HotelServiceEnums.PricePolicyType.VIP)
                //{
                //    foreach (var pi in packageList)
                //    {
                //        pi.CustomerAskPrice.Text = "获取更低价>>";
                //        pi.CustomerAskPrice.Description = "联系顾问可能";
                //        var cancelPolicy = pi.LastCancelTime <= DateTime.Now ? 1 : 2;
                //        pi.CustomerAskPrice.ActionUrl = "http://www.zmjiudian.com/MagiCall/MagiCallClient?checkIn=" + checkIn + "&checkOut=" + checkOut + "&PCode=" + pi.packageBase.SerialNO + "&price=" + pi.NotVIPPrice + "&VIPPrice=" + pi.VIPPrice + "&hotelId=" + pi.packageBase.HotelID + "&payType=" + pi.PayType + "&cancelPolicy=" + cancelPolicy + "&userid={userid}&realuserid=1";
                //    }
                //}

                return packageList;
            }
            catch(Exception err)
            {
                LogHelper.WriteLog("LoadCtripHotelPackages ERR:" + err.Message + err.StackTrace);
                return new List<PackageInfoEntity>(); 
            }
        }

        /// <summary>
        /// 【触发更新列表价】触发更新指定酒店指定日期的列表价队列任务
        /// </summary>
        /// <param name="hotelid"></param>
        /// <param name="checkIn"></param>
        public bool PublishUpdatePriceSlotTask(int hotelid, DateTime checkIn)
        {
            ////实时拿过携程套餐后，触发更新该酒店的HotelPriceSlot
            //try
            //{
            //    new AccessServiceHelper().AddPriceSlot(hotelid, checkIn, false);
            //}
            //catch (Exception ex)
            //{

            //}

            //使用缓存原理来控制，同一个酒店3分钟内只更新一次
            var update = memCacheHotelPriceUpdate.GetData<string>(string.Format("HotelAddPriceSlot_{0}", hotelid),
            () =>
            {
                //获取是否需要更新更多日期
                var refMoreDate = false; var _ = memHotelPricePlanCache.GetData<string>(string.Format("HotelAddPriceSlot_RefMoreDate_{0}", hotelid), () => { refMoreDate = true; return ""; });

                //实时拿过携程套餐后，触发更新该酒店的HotelPriceSlot
                try
                {
                    new AccessServiceHelper().AddPriceSlot(hotelid, checkIn, refMoreDate);
                }
                catch (Exception ex)
                {

                }

                return "OK";
            });

            return true;
        }

        /// <summary>
        /// 【触发更新列表价】【直接触发&没有缓存时间】触发更新指定酒店指定日期的列表价队列任务
        /// </summary>
        /// <param name="hotelid"></param>
        /// <param name="checkIn"></param>
        /// <returns></returns>
        public bool QuickPublishPriceSlotTask(int hotelid, DateTime checkIn)
        {
            //实时拿过携程套餐后，触发更新该酒店的HotelPriceSlot
            try
            {
                //是否更新更多价格（目前是60天）
                var refMoreDate = true;

                new AccessServiceHelper().AddPriceSlot(hotelid, checkIn, refMoreDate);
            }
            catch (Exception ex)
            {

            }

            return true;
        }

        /// <summary>
        /// 【面向后台】【触发更新列表价】【直接触发&没有缓存时间】触发更新指定酒店指定日期的列表价队列任务
        /// </summary>
        /// <param name="hotelid"></param>
        /// <param name="checkIn"></param>
        /// <returns></returns>
        public bool QuickPublishPriceSlotTaskForBg(int hotelid, DateTime checkIn)
        {
            ////实时拿过携程套餐后，触发更新该酒店的HotelPriceSlot
            //try
            //{
            //    new AccessServiceHelper().AddPriceSlotForBg(hotelid, checkIn, false);
            //}
            //catch (Exception ex)
            //{

            //}

            //使用缓存原理来控制，同一个酒店3分钟内只更新一次
            var update = memCacheHotelPriceUpdate.GetData<string>(string.Format("HotelAddPriceSlotForBg_{0}", hotelid),
            () =>
            {
                //实时拿过携程套餐后，触发更新该酒店的HotelPriceSlot
                try
                {
                    //是否更新更多价格（目前是60天）
                    var refMoreDate = true;

                    new AccessServiceHelper().AddPriceSlotForBg(hotelid, checkIn, refMoreDate);
                }
                catch (Exception ex)
                {

                }

                return "OK";
            });

            return true;
        }

        /// <summary>
        /// 【触发更新列表价】触发更新指定酒店列表指定日期的列表价队列任务
        /// </summary>
        /// <param name="hotellist"></param>
        /// <param name="checkIn"></param>
        /// <returns></returns>
        public bool PublishUpdateMultiPriceSlotTask(List<int> hotellist, DateTime checkIn)
        {
            if (hotellist == null || hotellist.Count <= 0)
            {
                return false;
            }

            for (int i = 0; i < hotellist.Count; i++)
            {
                var hid = hotellist[i];
                try
                {
                    PublishUpdatePriceSlotTask(hid, checkIn);
                }
                catch (Exception ex)
                {

                }
            }

            return true;
        }

        /// <summary>
        /// 获取指定酒店的携程套餐的日历数据
        /// </summary>
        /// <param name="hotelid"></param>
        /// <param name="startDate"></param>
        /// <param name="pid"></param>
        /// <param name="canlendarLength">携程的日历目前初始显示180天的，前两个月的日历根据套餐可售状态显示，后面的几个月Open</param>
        /// <returns></returns>
        public List<PDayItem> GenCtripHotelPackageCalendar(int hotelid, DateTime startDate, int pid = 0, int canlendarLength = 360, HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType ppt = HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType.Default)
        {
            DateTime packageEndDate = startDate.AddDays(canlendarLength);
            DateTime endDate = startDate.AddDays(canlendarLength) < packageEndDate ? startDate.AddDays(canlendarLength) : packageEndDate;

            //当前酒店所有有套餐的日期
            var nightList = new List<CtripHotelRoomPriceNightEntity>();

            ////得到对应携程酒店ID
            //var hotelOriId = HotelDAL.GetCtrpHotelId(hotelid);
            //if (hotelOriId == 0)
            //{
            //    return new List<PDayItem>();
            //}

            //这里应该有一个完善的XC套餐日历管理模块，目前还没有，先开放日历
            //if (pid > 0)
            //{
            //    //只显示某个套餐的日历
            //    nightList = CtripDAL.GetCtripHotelRoomPriceNightWhereId(hotelOriId, pid.ToString());
            //}
            //else
            //{
            //    //得到当前酒店的PriceChannelID（2 表示忽略携程的可预付条件    3 表示需追加套餐可预付的条件）
            //    string priceChannelId = HotelDAL.GetPriceChannelIdByHotelId((long)hotelid);
            //    if (priceChannelId != "2")
            //    {
            //        //追加可预付条件
            //        nightList = CtripDAL.GetCtripHotelRoomPriceNight2(hotelOriId);
            //    }
            //    else
            //    {
            //        nightList = CtripDAL.GetCtripHotelRoomPriceNight(hotelOriId);
            //    }
            //}

            List<PDayItem> ds = new List<PDayItem>();
            for (int i = 0; i < canlendarLength; i++)
            {
                if (startDate.AddDays(i) > endDate) break;
                PDayItem d = new PDayItem();
                d.Day = startDate.AddDays(i).Date;
                d.MaxSealCount = 5;

                //FillDailyItems
                var pdayPItem = new PDayPItem
                {
                    Day = d.Day.Date,
                    MaxSealCount = 5,
                    SoldCount = 0
                };

                //两个月以后的日历Open
                if (i + 1 > nightList.Count)
                {
                    d.SellState = 1;
                    pdayPItem.PID = 0;
                }
                else
                {
                    d.SellState = nightList.Exists(n => n.Night.Date == d.Day) ? (int)PackageSellSate.canSell : (int)PackageSellSate.canNotSell;
                    if (d.SellState == (int)PackageSellSate.canSell)
                    {
                        pdayPItem.PID = Convert.ToInt32(nightList.Find(n => n.Night.Date == d.Day).ID);
                    }
                }

                d.PItems = new List<PDayPItem>();
                d.PItems.Add(pdayPItem);

                ds.Add(d);
            }

            return ds;
        }

        /// <summary>
        /// 返回一个纯的（不控制可售日)日历
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="canlendarLength"></param>
        /// <returns></returns>
        public List<PDayItem> GenPureHotelPackageCalendar(DateTime startDate, int canlendarLength = 360)
        {

            return memCacheCanlendar.GetData<List<PDayItem>>(string.Format("{0}_{1}", startDate.ToString("yyMMdd"), canlendarLength), "PurCanledar", () =>
            {
                DateTime packageEndDate = startDate.AddDays(canlendarLength);
                DateTime endDate = startDate.AddDays(canlendarLength) < packageEndDate ? startDate.AddDays(canlendarLength) : packageEndDate;

                List<PDayItem> ds = new List<PDayItem>();
                for (int i = 0; i < canlendarLength; i++)
                {
                    if (startDate.AddDays(i) > endDate) break;
                    PDayItem d = new PDayItem();
                    d.Day = startDate.AddDays(i).Date;
                    d.MaxSealCount = 5;

                    //FillDailyItems
                    var pdayPItem = new PDayPItem
                    {
                        Day = d.Day.Date,
                        MaxSealCount = 5,
                        SoldCount = 0,
                        PID = 0
                    };

                    d.SellState = 1;

                    d.PItems = new List<PDayPItem>();
                    d.PItems.Add(pdayPItem);

                    ds.Add(d);
                }

                return ds;
            });
        }

        public List<DateTime> GetHotlidays()
        {
            return memCacheHolidays.GetData<List<DateTime>>("HolidayList",()=>
                 {
                   return   HotelDAL.GetHolidays().Where(_ => _.Day.Year == DateTime.Now.Year).Select(_ => _.Day.Date).ToList();
                });
        }

        /// <summary>
        /// 获取指定房券的日历数据
        /// </summary>
        /// <param name="hvid">房券id</param>
        /// <param name="startDate">房券售卖开始时间</param>
        /// <param name="endDate">房券售卖结束时间</param>
        /// <returns></returns>
        public List<PDayItem> GenHotelVoucherCalendar(int hvid, DateTime startDate, DateTime endDate)
        {
            startDate = startDate.Date < DateTime.Now.Date ? DateTime.Now : startDate;
            List<PDayItem> ds = new List<PDayItem>();
            //获取房券使用日期
            List<VRateEntity> VRateList = new List<VRateEntity>();
            VRateList = HotelDAL.GetVRateByHVID(hvid); 
            List<DateTime> holidayList =  GetHotlidays();
            if (VRateList != null && VRateList.Count > 0)
            {
                bool isHaveType0 = true;
                if (VRateList.Where(_ => _.Type == 0).Count() > 0)
                {
                    isHaveType0 = true;
                }
                else
                {
                    isHaveType0 = false;
                }
                for (DateTime startTime = startDate.Date; startTime <= endDate.Date; startTime = startTime.AddDays(1))
                {
                    PDayItem d = new PDayItem();
                    d.Day = startTime;
                    if (isHaveType0 == true)
                    {
                        d.SellState = 1;
                    }
                    else
                    {
                        d.SellState = 0;
                    }
                    d.MaxSealCount = 1;
                    //节假日不可售
                    if (holidayList.Contains(startTime))
                    {
                        d.SellState = 0;
                    }
                    //周末不可售
                    if (startTime.DayOfWeek == DayOfWeek.Friday || startTime.DayOfWeek == DayOfWeek.Saturday)
                    {
                        d.SellState = 0;
                    }
                    foreach (VRateEntity item in VRateList.Where(_=>_.Integral!=0))
                    {
                        //指定日和类型为5、6(5、6即周五周六)可售。
                        if (item.Type == 5 && startTime.Date.DayOfWeek == DayOfWeek.Friday)
                        {
                            d.SellState = 1;
                        }
                        //指定日和类型为5、6(5、6即周五周六)可售。
                        if (item.Type == 6 && startTime.Date.DayOfWeek == DayOfWeek.Saturday)
                        {
                            d.SellState = 1;
                        }
                        //指定日和类型为7(周日)可售。
                        if (item.Type == 7 && startTime.Date.DayOfWeek == DayOfWeek.Sunday)
                        {
                            d.SellState = 1;
                        }
                    }
                    foreach (VRateEntity item in VRateList.Where(_ => _.Integral == 0))
                    {
                        //积分为0的也不可售
                        if (item.Date.Date == startTime)
                        {
                            d.SellState = 0;
                        }
                    }
                    
                    ds.Add(d);
                }
            }


            return ds;
        }



        public static void CtripPackageWriteLog(string msg)
        {
            //string logFile = @"D:\Log\HotelService\CtripPackageWriteLogV42_" + DateTime.Now.ToString("_MMdd") + ".txt";
            //File.AppendAllText(logFile, string.Format("{0}  {1} \r\n\r\n", msg, DateTime.Now));
        }

        #endregion

        /// <summary>
        /// 从HJD.PriceService处抄来 由订单备注及套餐对应房型 房型关联的床型 获得订单对应床型
        /// </summary>
        /// <param name="pID"></param>
        /// <param name="note"></param>
        /// <param name="roomCount"></param>
        /// <param name="BigBedCount"></param>
        /// <param name="TwinBedCount"></param>
        public List<int> ParseBedCountWithNote(int pID, string note, int roomCount)
        {
            int BigBedCount = 0;
            int TwinBedCount = 0;

            HotelRoomInfoEntity hri = HotelDAL.GetOnePRoomInfoByPID(pID);

            if (hri != null)
            {
                if (hri.HasBigBed > 0 && !(hri.HasTwinBed > 0)) //套餐房型只有大床
                {
                    BigBedCount = roomCount;
                }
                else if (hri.HasTwinBed > 0 && !(hri.HasBigBed > 0)) //套餐房型只有双床
                {
                    TwinBedCount = roomCount;
                }
            }
            else
            {
                Dictionary<string, string> numDic = new Dictionary<string, string>() { { "一", "1" } ,
                    { "二", "2" },
                    { "两", "2" },
                    { "三", "3" },
                    { "四", "4" },
                    { "五", "5" },
                    { "六", "6" },
                    { "七", "7" },
                    { "八", "8" },
                    { "九", "9" }};

                string newNote = note;
                foreach (string key in numDic.Keys)
                {
                    newNote = newNote.Replace(key, numDic[key]);
                }

                Regex BigBegreg = new Regex("(\\d)间?大床");
                Regex TwinBegreg = new Regex("(\\d)间?双床");

                Match m = BigBegreg.Match(newNote);

                if (m.Success)
                {
                    BigBedCount = int.Parse(m.Groups[1].ToString());
                }

                m = TwinBegreg.Match(newNote);

                if (m.Success)
                {
                    TwinBedCount = int.Parse(m.Groups[1].ToString());
                }

                if (TwinBedCount == 0 && BigBedCount == 0)
                {
                    if (newNote.IndexOf("双床") >= 0)
                    {
                        TwinBedCount = roomCount;
                    }
                    else if (newNote.IndexOf("大床") >= 0)
                    {
                        BigBedCount = roomCount;
                    }
                    else //如果备注中没有床的信息  那么缺省取双床， 如果房型与双床没有关联，那么取大床
                    {
                        if (hri == null) //如果没有信息 , 如捷旅的套餐
                        {
                            TwinBedCount = roomCount;
                        }
                        else
                        {
                            if (hri.HasTwinBed > 0)
                            {
                                TwinBedCount = roomCount;
                            }
                            else
                            {
                                BigBedCount = roomCount;
                            }
                        }
                    }
                }
                else
                {
                    if (TwinBedCount == 0)
                    {
                        TwinBedCount = roomCount - BigBedCount;
                    }
                    else if (BigBedCount == 0)
                    {
                        BigBedCount = roomCount - TwinBedCount;
                    }
                }
            }
            return new List<int> { BigBedCount, TwinBedCount };
        }

        /// <summary>
        /// 包房酒店列表
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public List<int> GetPackRoomHotelIdList(DateTime date)
        {
            return HotelDAL.GetPackRoomHotelIdList(date);
        }

        /// <summary>
        /// 获取与某个套餐ID相同序列的套餐列表
        /// </summary>
        /// <param name="pId"></param>
        /// <returns></returns>
        public List<SameSerialPackageEntity> GetSameSerialPackageEntityList(int pId)
        {
            return HotelDAL.GetSameSerialPackageEntityList(pId);
        }



        public List<SameSerialPackageEntity> GetSerialPackageItemListByPid(int pId, DateTime currentDate, int hotelId)
        {
            return GenSerialPackageItemListByPid(pId, currentDate, hotelId);
            //return memCacheHotelCtripPackage.GetData<List<SameSerialPackageEntity>>(string.Format("GetSerialPackageItemListByPid{0}_{1}_{2}", pId, currentDate, hotelId), () =>
            //{
            //    return GenSerialPackageItemListByPid(pId, currentDate, hotelId);
            //});
        }
        /// <summary>
        /// 获取与某个套餐id相同组号的套餐列表
        /// </summary>
        /// <param name="pId"></param>
        /// <param name="currentDate"></param>
        /// <returns></returns>
        public List<SameSerialPackageEntity> GenSerialPackageItemListByPid(int pId, DateTime currentDate, int hotelId)
        {
            List<SameSerialPackageEntity> groupSerialList = HotelDAL.GetSerialPackageItemListByPid(pId);
            List<int> pids = groupSerialList.Select(_ => _.pId).ToList();
            //List<TopNPackageItem> plist = new HotelService().GetPackageItemList2(pids, null);

            foreach (SameSerialPackageEntity item in groupSerialList)
            {
                decimal noVipPrice = 0;
                decimal vipPrice = 0;
                try
                {
                    List<PDayItem> pdayitemlist = GenHotelPackageCalendar(hotelId, currentDate, item.pId);
                    PDayItem pdayitem = pdayitemlist.Where(_ => _.SellState == 1).OrderBy(_ => _.SellPrice).FirstOrDefault();
                    if (pdayitem == null || pdayitem.SellPrice == 0)
                    {
                        pdayitem = pdayitemlist.Where(_ => _.SellPrice > 0).OrderBy(_ => _.SellPrice).FirstOrDefault();
                    }
                    noVipPrice = pdayitem.SellPrice;//非vip最低价 正常售价
                    vipPrice = pdayitem.VipPrice;//最低价 是vip价格
                    
                    //noVipPrice = plist.Where(_ => _.PackageID == item.pId).First().PackagePrice.Where(p => p.Type == 0).First().Price;//非vip最低价 正常售价
                    //vipPrice = plist.Where(_ => _.PackageID == item.pId).First().PackagePrice.Where(p => p.Type == -1).First().Price;//最低价 是vip价格
                }
                catch (Exception ex) {
                    LogHelper.WriteLog("获取与某个套餐id相同组号的套餐价格时报错  pid：" + item.pId + " error：" + ex);
                }

                item.NoVIPPrice = noVipPrice;
                item.VIPPrice = vipPrice;
            }

            return groupSerialList;
        }

        #region 套餐专辑列表

        /// <summary>
        /// 专辑列表
        /// </summary>
        /// <param name="param"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<PackageAlbumsEntity> GetPackageAlbumsList(TopNPackageSearchParam param, out int totalCount)
        {
            return HotelDAL.GetPackageAlbumsList(param, out totalCount);
        }

        public List<PackageAlbumsEntity> GetAllPackageAlbums()
        {
            return HotelDAL.GetAllPackageAlbums();
        }


        public List<PackageAlbumsEntity> GetPackageAlbumsByGroupNo(string groupNo)
        {
            return HotelDAL.GetPackageAlbumsByGroupNo(groupNo);
        }

        /// <summary>
        /// 单张专辑信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PackageAlbumsEntity GetOnePackageAlbums(int id)
        {
            return HotelDAL.GetOnePackageAlbums(id);
        }

        /// <summary>
        /// 删掉专辑
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int DelPackageAlbums(int id)
        {
            return HotelDAL.DelPackageAlbums(id);
        }

        /// <summary>
        /// 新增或更新专辑
        /// </summary>
        /// <param name="album"></param>
        /// <returns></returns>
        public int InsertOrUpdatePackageAlbums(PackageAlbumsEntity album)
        {
            return HotelDAL.InsertOrUpdatePackageAlbums(album);
        }

        /// <summary>
        /// 获取酒店专辑列表
        /// </summary>
        /// <param name="param"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<TopNPackagesEntity> GetTopNPackagesEntityList4HotelAlbums(TopNPackageSearchParam param, out int count)
        {
            return HotelDAL.GetTopNPackagesEntityList4HotelAlbums(param, out count);
        }

        #endregion

        #region 获取绑定的专辑
        /// <summary>
        /// 由酒店Id或套餐Id获得绑定的专辑
        /// </summary>
        /// <param name="hotelId"></param>
        /// <param name="pId"></param>
        /// <returns></returns>
        public List<RelPackageAlbumsEntity> GetRelPackageAlbums(IEnumerable<int> hotelIds, IEnumerable<int> pIds)
        {
            return HotelDAL.GetRelPackageAlbums(hotelIds, pIds);
        }
        #endregion

        /// <summary>
        /// 获取单个套餐的数据
        /// </summary>
        /// <param name="pId"></param>
        /// <returns></returns>
        public PackageEntity GetOnePackageEntity(int pId)
        {
            return HotelDAL.GetOnePackageEntity(pId);
        }

        /// <summary>
        /// 【触发检查指定酒店房态】
        /// </summary>
        /// <param name="hotelid"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public bool PublishCheckHotelRoomStateTask(int hotelid, DateTime date)
        {
            //实时拿过携程套餐后，触发更新该酒店的HotelPriceSlot
            try
            {
                new OtaServiceHelper().AddCheckHotelRoomStateTask(hotelid, date);
            }
            catch (Exception ex)
            {

            }

            return true;

            //使用缓存原理来控制，同一个酒店的同一个日期3分钟内只更新一次
            var update = memCacheHotelPriceUpdate.GetData<string>(string.Format("CheckRoomBedState_{0}_{1}", hotelid, date.ToString("yyyy-MM-dd")),
            () =>
            {
                //实时拿过携程套餐后，触发更新该酒店的HotelPriceSlot
                try
                {
                    new OtaServiceHelper().AddCheckHotelRoomStateTask(hotelid, date);
                }
                catch (Exception ex)
                {

                }

                return "OK";
            });

            return true;
        }

        public RetailPackageEntity GetRetailPackageInfo( DateTime CheckIn, DateTime CheckOut, int pid, long cid)
        {

            RetailPackageEntity rp = new RetailPackageEntity();
            rp = HotelDAL.GetRetailPackageInfo(pid, cid);
            if (rp != null || rp.PID > 0)
            {
                //获取酒店价格列表 价格以元为单位
                List<PackageRateEntity> rateList = GetHotelPackageRateList(rp.HotelId, CheckIn, CheckOut, HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType.Default, pid);

                PackageRateEntity rate = rateList.Where(r => r.PID == pid).First();
                rp.NotVipPrice = rate.NotVIPPrice;
                rp.VipPrice = rate.VIPPrice;
                rp.ManualCommission = rate.ManualCommission;
                rp.AutoCommission = rate.AutoCommission;
                rp.CheckIn = CheckIn;
                rp.CheckOut = CheckOut;

                try
                {
                    PackageInfoEntity pEntity = GetHotelPackages(rp.HotelId, rp.PID).FirstOrDefault();
                    if (pEntity != null)
                    {
                        //设置套餐项目内容
                        SetDailyItem(pEntity, CheckIn, CheckOut);
                        rp.PackageContent = pEntity.Items.FindAll(_ => _.Type == 1).Select(_ => _.Description).ToList();
                    }
                    rp.ShareDescription = pEntity.packageBase.ShareDescription;
                    rp.ShareTitle = pEntity.packageBase.ShareTitle;
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog("GetRetailPackageInfos" + ex);
                }
            }
            return rp;
        }
    }
}