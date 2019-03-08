using HJD.AccountServices.Contracts;
using HJD.AccountServices.Entity;
using HJD.CouponService.Contracts.Entity;
using HJD.Framework.WCF;
using HJD.HotelPrice.Contract;
using HJD.HotelPrice.Contract.DataContract;
using HJD.HotelPrice.Contract.DataContract.Order;
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
    public class PriceAdapter
    {
        public static IHotelService HotelService = ServiceProxyFactory.Create<IHotelService>("BasicHttpBinding_IHotelService");
        public static IHotelPriceService PriceService = ServiceProxyFactory.Create<IHotelPriceService>("BasicHttpBinding_IHotelPriceService");

        public static Models.HotelPrice Get(int id, string checkIn, string checkOut, string sType)
        {
            DateTime arrivalTime = DateTime.Now;
            DateTime departureTime = arrivalTime.AddDays(1);

            try
            {
                arrivalTime = DateTime.Parse(checkIn);
                departureTime = DateTime.Parse(checkOut);
                if (departureTime < arrivalTime.AddDays(1)) departureTime = arrivalTime.AddDays(1);
            }
            catch { }



            List<HotelPriceEntity> hpl = PriceService.QueryHotelListPrice(new List<int> { id }, arrivalTime, departureTime);
            List<OTAInfo> otaInfos = HotelHelper.GetHotelOTAInfo(id, sType);

            if (hpl.Count > 0 && hpl[0].PriceList.Count > 0)
            {
                foreach (HotelOTAPrice hpe in hpl[0].PriceList)
                {
                    OTAInfo toi = otaInfos.Where(o => o.ChannelID == hpe.ChannelID).FirstOrDefault();
                    if (toi != null && toi.ChannelID > 0) toi.Price = Decimal.Round(hpe.Price, 0);
                }
            }

            Models.HotelPrice hp = new Models.HotelPrice();

            ////有价格的情况下，价格最低优先
            ////合作OTA有四家，同等价格下，优先显示次序为携程、艺龙、住哪儿、同程
            hp.OTAList = otaInfos.OrderBy(o => o.Price == 0 ? 10000 : o.Price).ThenBy(o => o.ChannelID == 2 ? 1 : (o.ChannelID == 6 ? 2 : (o.ChannelID == 11 ? 3 : (o.ChannelID == 6 ? 4 : 100)))).ToList();

            hp.HotelID = id;
            hp.Name = HotelService.GetHotel(id).Name;

            if (hp.OTAList.Where(o => o.ChannelID == 102).Count() > 0)
            {
                hp.OTAList.Remove(hp.OTAList.Where(o => o.ChannelID == 102).First());
            }

            return hp;
        }

        public static HotelPrice2 Get2(int id, string checkIn, string checkOut, string sType)
        {
            DateTime arrivalTime = CommMethods.GetDefaultCheckIn();
            DateTime departureTime = arrivalTime.AddDays(1);

            if (checkIn != "")
            {
                try
                {
                    arrivalTime = DateTime.Parse(checkIn);
                    departureTime = DateTime.Parse(checkOut);
                    if (departureTime < arrivalTime.AddDays(1)) departureTime = arrivalTime.AddDays(1);
                }
                catch { }
            }

            HotelPrice2 hp = new HotelPrice2();

            List<PackageInfoEntity> packagePrice = new PackageAdapter().GetHotelPackages(id, arrivalTime, departureTime);
            SetDayLimit(hp, packagePrice);


            //如果缺省日期没有套餐可买，则寻找一个最近有套餐可以销售的日期
            if (checkIn == "0" && packagePrice.Count > 0 && packagePrice.Count == packagePrice.Where(p => p.SellState > 1).Count())
            {
                packagePrice = new PackageAdapter().GetCanSellHotelPackages(id, arrivalTime, arrivalTime, departureTime);
                SetDayLimit(hp, packagePrice);
            }

            List<OTAInfo2> otaInfos2 = new List<OTAInfo2>();

            if (packagePrice.Count == 0)
            {
                List<OTAInfo> otaInfos = HotelHelper.GetHotelOTAInfo(id, sType);

                List<HotelPriceInfo2> hpl = PriceService.QueryHotelRealTimePrice2(id, arrivalTime, CommMethods.GetNightCount(arrivalTime, departureTime));

                otaInfos2 = (from o in otaInfos
                             join p in hpl on o.ChannelID equals p.ChannelID into temp
                             from hpi in temp.DefaultIfEmpty()
                             select new OTAInfo2
                             {
                                 AccessURL = o.AccessURL,
                                 ChannelID = o.ChannelID,
                                 Name = o.Name,
                                 OTAHotelID = o.OTAHotelID,
                                 Price = hpi == null ? 0 : hpi.MinPrice,
                                 CanSyncPrice = o.CanSyncPrice,
                                 RoomRates = GenRoomRates(hpi)
                             }).ToList();


                if (hpl.Count > 0 && hpl.Count > 0)
                {
                    foreach (HotelPriceInfo2 hpe in hpl)
                    {
                        OTAInfo toi = otaInfos.Where(o => o.ChannelID == hpe.ChannelID).FirstOrDefault();
                        if (toi != null && toi.ChannelID > 0) toi.Price = Decimal.Round(hpe.MinPrice, 0);
                    }
                }

                otaInfos2 = otaInfos2.OrderBy(o => o.Price == 0 ? 10000 : o.Price).ThenBy(o => o.ChannelID == 2 ? 1 : (o.ChannelID == 6 ? 2 : (o.ChannelID == 11 ? 3 : (o.ChannelID == 6 ? 4 : 100)))).ToList();

                if (otaInfos2.Where(o => o.ChannelID == 102).Count() > 0)
                {
                    otaInfos2.Remove(otaInfos2.Where(o => o.ChannelID == 102).First());
                }
            }

            ////有价格的情况下，价格最低优先
            ////合作OTA有四家，同等价格下，优先显示次序为携程、艺龙、住哪儿、同程
            hp.OTAList = otaInfos2;
            hp.HotelID = id;
            hp.Name = HotelService.GetHotel(id).Name;
            hp.CheckIn = arrivalTime;
            hp.CheckOut = departureTime;
            hp.Packages = packagePrice;


            return hp;
        }

        //private static  List<PackageInfoEntity> MerageDailyItems(List<PackageInfoEntity> ps)
        //{
        //    foreach (PackageInfoEntity p in ps)
        //    {
        //        if (p.DailyItems.Count > 1)
        //        {
        //                   p.DailyItems.RemoveRange(1, p.DailyItems.Count - 1);
        //        }
        //    }
        //    return ps;
        //}

        /// <summary>
        /// 根据套餐是否选用了包房供应商 进行了判断和更新
        /// </summary>
        /// <param name="hotelid"></param>
        /// <param name="CheckIn"></param>
        /// <param name="CheckOut"></param>
        /// <param name="pid"></param>
        /// <param name="roomCount"></param>
        /// <returns></returns>
        public static PackageRateEntity GetHotelOnePackageRate(int hotelid, DateTime CheckIn, DateTime CheckOut, int pid, string note, int roomCount, int RoomSupplierID, HotelServiceEnums.PricePolicyType ppt)
        {
            List<PackageRateEntity> rateList = HotelService.GetHotelPackageRateList(hotelid, CheckIn, CheckOut, ppt, pid: pid);
           
            //取消自动修改供应商规则
            //bool bIsPackRoomSupplier = HotelService.IsPackRoomSupplierByHotelID_PID(hotelid, pid);
            //if (bIsPackRoomSupplier)
            //{
            //    List<int> bedCountList = HotelService.ParseBedCountWithNote(pid, note, roomCount);
            //    List<PackRoomBedStateEntity20> packRoomBedList = HotelService.GetCanUsePackRoomSupplier(hotelid, CheckIn, (int)Math.Floor((CheckOut - CheckIn).TotalDays), pid, bedCountList[0], bedCountList[1], RoomSupplierID);
            //    if (packRoomBedList != null && packRoomBedList.Count != 0)
            //    {
            //        foreach (var pr in rateList)
            //        {
            //            foreach (var p in pr.DailyList)
            //            {
            //                var packRoomSupplier = packRoomBedList.First(_ => _.Date == p.Day);
            //                int price = packRoomSupplier.PackRoomPrice / 100;
            //                int roomSupplierID = packRoomSupplier.PackRoomSupplierID;
            //                foreach (var d in p.ItemList)
            //                {
            //                    //等于1是房间产品
            //                    if (d.SupplierType == 1)
            //                    {
            //                        d.SettlePrice = price;
            //                        d.ProductID = roomSupplierID;//ProductID对应供应商ID
            //                        break;
            //                    }
            //                }
            //                p.SettlePrice = p.ItemList.Sum(_ => _.SettlePrice);
            //            }
            //            pr.SettlePrice = pr.DailyList.Sum(_ => _.SettlePrice);
            //        }
            //    }
            //}

            return rateList.Where(r => r.PID == pid).First();
        }

        public static PackageRateEntity GetCtripHotelOnePackageRate(int hotelid, DateTime CheckIn, DateTime CheckOut, int pid, HJD.HotelServices.Contracts.HotelServiceEnums.PackageType packageType, string note, int roomCount, int RoomSupplierID, HotelServiceEnums.PricePolicyType ppt)
        {
            return HotelService.GetCtripHotelPackageRateList(hotelid, pid, packageType, CheckIn, CheckOut, ppt);
        }

        public static HotelPrice2 GetHotelPackageList(int id, string checkIn, string checkOut, string sType, string appVer = null, long userId = 0, bool needNotSalePackage = true,int pid = 0)
        {
            return GetHotelPackageList(id, checkIn, checkOut, sType, true, appVer, userId, null, needNotSalePackage: needNotSalePackage, pid: pid);
        }

        /// <summary>
        /// 获取指定HotelId、入住日期、离店日期的酒店套餐
        /// </summary>
        /// <param name="id">酒店ID</param>
        /// <param name="checkIn">入住日期</param>
        /// <param name="checkOut">离店日期</param>
        /// <param name="sType"></param>
        /// <param name="updatePrice">是否需要触发更新列表价(默认需要)</param>
        /// <returns></returns>
        public static HotelPrice2 Get3(int id, string checkIn, string checkOut, string sType, bool updatePrice, string appVer = null, long userId = 0)
        {
            //StringBuilder sb = new StringBuilder();
            //sb.AppendLine(string.Format(" Get3 :id:{0}, checkIn:{1}, checkOut:{2}, sType:{3}, updatePrice:{4},  appVer:{5}, userId:{6}", id, checkIn, checkOut, sType, updatePrice, appVer, userId));
            bool isAppV42 = !string.IsNullOrWhiteSpace(appVer) && appVer.CompareTo("4.2") >= 0 ? true : false;//old appVer friendly

            int terminalType = 0; if (sType == "www") terminalType = 3;//Web Access

            HJDAPI.Common.Helpers.Enums.CustomerType customerType = AccountAdapter.GetCustomerType(userId);
            HotelServiceEnums.PricePolicyType ppt = PriceAdapter.TransCustomerType2PricePolicyType(customerType);

            DateTime arrivalTime = CommMethods.GetDefaultCheckIn();
            DateTime departureTime = arrivalTime.AddDays(1);

            if (checkIn != "" && checkIn != "0")
            {
                try
                {
                    arrivalTime = DateTime.Parse(checkIn);
                    departureTime = DateTime.Parse(checkOut);
                    if (departureTime < arrivalTime.AddDays(1)) departureTime = arrivalTime.AddDays(1);
                }
                catch (Exception ex)
                {

                }
            }

            HotelPrice2 hp = new HotelPrice2();
            List<OTAInfo2> otaInfos2 = new List<OTAInfo2>();
            List<PackageInfoEntity> packagePrice = new List<PackageInfoEntity>();

            //标识是否包含zmjd套餐
            var haveZmjdPackage = false;

            #region 优先读取zmjd套餐
            try
            {
                //genZMJDHotelPackages(id, arrivalTime, departureTime, terminalType, ppt, checkIn, ref packagePrice);//获取zmjd套餐

                #region 获取zmjd套餐 old
                packagePrice = new PackageAdapter().GetHotelPackages(id, arrivalTime, departureTime, terminalType, ppt);//获取zmjd套餐
                //SetDayLimit(hp, packagePrice);

                //如果缺省日期没有套餐可买，则寻找一个最近有套餐可以销售的日期
                if (checkIn == "0" && packagePrice.Count > 0 && packagePrice.Count == packagePrice.Where(p => p.SellState > 1).Count())
                {
                    packagePrice = new PackageAdapter().GetCanSellHotelPackages(id, arrivalTime, arrivalTime, departureTime, ppt);
                    //SetDayLimit(hp, packagePrice);
                }
                #endregion
            }
            catch (Exception ex)
            {
                Log.WriteLog("PriceAdapter.Get3:[GetHotelPackages]处出现异常:" + ex.Message + "|" + ex.StackTrace + string.Format("|参数id:{0},checkIn:{1},checkOut:{2},sType={3}", id, checkIn, checkOut, sType));
            }

            haveZmjdPackage = packagePrice.Exists(p => p.SellState == 1);

            #endregion

            try
            {
                //genNotZMJDHotelPackages(id, arrivalTime, departureTime, ppt, checkIn, updatePrice, sType, haveZmjdPackage, isAppV42, ref packagePrice, ref otaInfos2);

                #region 获取OTA渠道套餐(如深捷旅/携程/Booking等) old

                //如果入住日小于今天，则不做任何OTA查询
                if (arrivalTime >= DateTime.Now.Date)
                {
                    //是否需要读取Booking（目前甲米等地酒店使用Booking跳转）
                    var useBooking = PriceService.UseBooking(id);

                    #region 不转向Booking

                    if (!useBooking)
                    {
                        #region 追加读取ctrip套餐

                        List<PackageInfoEntity> ctripPackages = new List<PackageInfoEntity>();
                        if (isAppV42)
                        {
                            //有zmjd套餐则通过api获取ctrip套餐（提高效率）
                            if (haveZmjdPackage)
                            {
                                //sb.AppendLine(" HotelService.GetCtripHotelPackagesForApiV42");
                                ctripPackages = HotelService.GetCtripHotelPackagesForApiV42(id, arrivalTime, departureTime, updatePrice, ppt);
                            }
                            //否则使用页面抓取获取ctrip套餐
                            else
                            {
                                //sb.AppendLine(" HotelService.GetCtripHotelPackagesForApiV42");
                                ctripPackages = HotelService.GetCtripHotelPackagesForApiV42(id, arrivalTime, departureTime, updatePrice, ppt);
                                //sb.AppendLine(" HotelService.GetCtripHotelPackagesV42");
                                //ctripPackages = HotelService.GetCtripHotelPackagesV42(id, arrivalTime, departureTime, updatePrice, ppt);
                            }
                        }
                        else
                        {
                            //sb.AppendLine(" HotelService.GetCtripHotelPackages");
                            ctripPackages = HotelService.GetCtripHotelPackages(id, arrivalTime, departureTime, updatePrice, ppt);
                        }

                        //如果获取到ctrip套餐则将其追加至zmjd套餐后面
                        if (ctripPackages != null && ctripPackages.Count > 0)
                        {
                            //sb.AppendLine(" 如果获取到ctrip套餐则将其追加至zmjd套餐后面");
                            packagePrice.InsertRange(packagePrice.Count(), ctripPackages);
                        }
                        else
                        {
                            //当没有zmjd套餐 & 默认日期进来的，则去查询最近一天有套餐的日期
                            //sb.AppendLine(" 当没有zmjd套餐 & 默认日期进来的，则去查询最近一天有套餐的日期");

                            if (checkIn == "0" && packagePrice.Count == 0)
                            {
                                var ds = HotelService.GenCtripHotelPackageCalendar(id, arrivalTime);
                                if (ds != null && ds.Count > 0 && ds.Exists(d => d.Day > arrivalTime && d.SellState == 1))
                                {
                                    var callSellDate = ds.Where(d => d.Day > arrivalTime && d.SellState == 1).OrderBy(d => d.Day).ToList().First();
                                    arrivalTime = callSellDate.Day;
                                    departureTime = arrivalTime.AddDays(1);
                                    packagePrice =
                                        isAppV42
                                        ? (haveZmjdPackage ? HotelService.GetCtripHotelPackagesForApiV42(id, arrivalTime, departureTime, updatePrice, ppt) : HotelService.GetCtripHotelPackagesForApiV42(id, arrivalTime, departureTime, updatePrice, ppt)) //HotelService.GetCtripHotelPackagesV42(id, arrivalTime, departureTime, updatePrice, ppt))
                                            : HotelService.GetCtripHotelPackages(id, arrivalTime, departureTime, updatePrice, ppt);
                                }
                            }
                        }

                        //SetDayLimit(hp, packagePrice);

                        #endregion

                        #region 没有zmjd 没有ctrip 读取深捷旅

                        if (packagePrice.Count == 0)
                        {
                            List<PackageInfoEntity> JLPackages = HotelService.GetJLHotelPackages(id, arrivalTime, departureTime, ppt);
                            if (JLPackages.Count() > 0)
                            {
                                //如果是指定日，但指定日的套餐都无价，同时酒店套餐为空的话，那么不加入套餐记录
                                if (checkIn != "0" && packagePrice.Count() == 0 && JLPackages.Where(p => p.SellState == 1).Count() == 0)
                                {
                                    //packagePrice = new PackageAdapter().GetJLCanSellHotelPackages(id, arrivalTime, out  arrivalTime, out  departureTime);
                                    //SetDayLimit(hp, packagePrice);
                                }
                                else
                                {
                                    packagePrice.InsertRange(packagePrice.Count(), JLPackages);

                                    if (checkIn == "0" && packagePrice.Count > 0 && packagePrice.Count == packagePrice.Where(p => p.SellState > 1).Count())
                                    {
                                        packagePrice = new PackageAdapter().GetJLCanSellHotelPackages(id, arrivalTime, out  arrivalTime, out  departureTime, ppt);
                                        //SetDayLimit(hp, packagePrice);
                                    }
                                }
                            }
                        }

                        #endregion
                    }

                    #endregion

                    #region 跳转至Booking

                    else
                    {
                        if (packagePrice.Count == 0)
                        {

                            //使用Booking前首先判断捷旅有没有数据，捷旅有套餐也要先用捷旅的，然后再说Booking
                            List<PackageInfoEntity> JLPackages = HotelService.GetJLHotelPackages(id, arrivalTime, departureTime);
                            if (JLPackages.Count() > 0)
                            {
                                //如果是指定日，但指定日的套餐都无价，同时酒店套餐为空的话，那么不加入套餐记录
                                if (checkIn != "0" && packagePrice.Count() == 0 && JLPackages.Where(p => p.SellState == 1).Count() == 0)
                                {
                                    //packagePrice = new PackageAdapter().GetJLCanSellHotelPackages(id, arrivalTime, out  arrivalTime, out  departureTime);
                                    //SetDayLimit(hp, packagePrice);
                                }
                                else
                                {
                                    packagePrice.InsertRange(packagePrice.Count(), JLPackages);

                                    if (checkIn == "0" && packagePrice.Count > 0 && packagePrice.Count == packagePrice.Where(p => p.SellState > 1).Count())
                                    {
                                        packagePrice = new PackageAdapter().GetJLCanSellHotelPackages(id, arrivalTime, out  arrivalTime, out  departureTime, ppt);
                                        //SetDayLimit(hp, packagePrice);
                                    }
                                }
                            }

                            #region 深捷旅也没有套餐的时候，则使用Booking跳转

                            if (packagePrice.Count == 0)
                            {
                                List<OTAInfo> otaInfos = HotelHelper.GetHotelOTAInfo(id, sType);

                                List<HotelPriceInfo2> hpl_new = new List<HotelPriceInfo2>();
                                List<HotelPriceInfo2> hpl = PriceService.QueryHotelPricePlan(id, arrivalTime);

                                if (hpl.Count > 0 && hpl.Count > 0)
                                {
                                    foreach (HotelPriceInfo2 hpe in hpl)
                                    {
                                        foreach (var toi in otaInfos)
                                        {
                                            if (toi != null && toi.ChannelID == hpe.ChannelID && toi.ChannelID > 0)
                                            {
                                                toi.Price = Decimal.Round(hpe.MinPrice, 0);
                                                //toi.PriceName = hpe.Name + "一卧室公寓";
                                                //toi.PriceBrief = hpe.Brief + "含早";
                                                toi.PriceName = hpe.Name;
                                                toi.PriceBrief = hpe.Brief;

                                                //一个渠道只需要拿取一个列表价
                                                if (!hpl_new.Exists(h => h.ChannelID == toi.ChannelID))
                                                {
                                                    hpl_new.Add(hpe);
                                                }

                                                break;
                                            }
                                        }
                                    }
                                }

                                var otaTransferUrlFormat = Configs.WWWURL + "/hotel/OtaTransfer?hotelId={0}&checkIn={1}&checkOut={2}";
                                if (!otaTransferUrlFormat.ToLower().Trim().StartsWith("http")) otaTransferUrlFormat = "http://" + otaTransferUrlFormat;

                                otaInfos2 = (from o in otaInfos
                                             join p in hpl_new on o.ChannelID equals p.ChannelID into temp
                                             from hpi in temp.DefaultIfEmpty()
                                             select new OTAInfo2
                                             {
                                                 AccessURL = o.AccessURL,
                                                 ChannelID = o.ChannelID,
                                                 Name = o.Name,
                                                 OTAHotelID = o.OTAHotelID,
                                                 Price = hpi == null ? 0 : hpi.MinPrice,
                                                 CanSyncPrice = o.CanSyncPrice,
                                                 RoomRates = GenRoomRates(hpi),
                                                 PriceName = o.PriceName,
                                                 PriceBrief = o.PriceBrief,
                                                 OtaTransferURL = string.Format(otaTransferUrlFormat, id, "0", "0")
                                             }).ToList();

                                otaInfos2 = otaInfos2.Where(o => o.ChannelID != 102 && o.Price > 0).OrderBy(o => o.ChannelID == 102 ? 0 : 1).ThenBy(o => o.Price == 0 ? 10000 : o.Price).ThenBy(o => o.ChannelID == 102 ? 0 : (o.ChannelID == 2 ? 1 : (o.ChannelID == 6 ? 2 : (o.ChannelID == 11 ? 3 : (o.ChannelID == 6 ? 4 : 100))))).ToList();

                                if (otaInfos2.Count() > 0 && otaInfos2.First().ChannelID == 102 && otaInfos2.First().Price == 0) otaInfos2.RemoveAt(0);

                                //价格升序
                                if (otaInfos2 != null && otaInfos2.Count > 0 && otaInfos2.Exists(ota => ota.Price > 0))
                                {
                                    otaInfos2 = new List<OTAInfo2> { otaInfos2.Where(ota => ota.Price > 0).OrderBy(ota => ota.Price).First() };
                                }
                            }

                            #endregion

                        }
                    }

                    #endregion
                }

                #endregion

            }
            catch (Exception ex)
            {
                Log.WriteLog("PriceAdapter.Get3出现异常:" + ex.Message + "|" + ex.StackTrace + string.Format("|参数id:{0},checkIn:{1},checkOut:{2},sType={3}", id, checkIn, checkOut, sType));
            }

            ////有价格的情况下，价格最低优先 合作OTA有四家，同等价格下，优先显示次序为携程、艺龙、住哪儿、同程
            hp.OTAList = otaInfos2;
            hp.HotelID = id;
            HotelItem tempHotel = HotelService.GetHotel(id);
            hp.Name = tempHotel != null ? tempHotel.Name : "";//2015-06-17 wwb 拆分HotelService.GetHotel(id).Name成两句
            hp.CheckIn = arrivalTime;
            hp.CheckOut = departureTime;
            hp.Packages = packagePrice.OrderBy(p => p.SellState).ThenBy(p => p.PackageType).ToList();

            SetDayLimit(hp, packagePrice);//设置套餐起始售卖日期

            AddMorePackageInfo(hp.Packages);

            genPackageLabelAndChannels(hp.Packages, customerType);//取消政策 优惠类型等标签
            genPackageLabelAndChannels(hp.JLPackages, customerType);//取消政策 优惠类型等标签

            if (hp.Packages != null && hp.Packages.Any())
            {
                hp.Packages.ForEach(_ =>
                {
                    _.CancelPolicy = PriceAdapter.GenPackageCancelPolicy(_.Items);//获取退款政策

                    //补iOS 4.4版本取错label数量的问题
                    if (sType.Equals("ios") && !string.IsNullOrWhiteSpace(appVer) && appVer.CompareTo("4.5") < 0 && _.PayLabelUrls.Any())
                    {
                        _.PayLabelUrls.Add(_.PayLabelUrls.Last());
                    }

                    _.PriceAfterDeduct = _.Price - _.Rebate;
                    _.BenefitPolicy = _.CanUseCashCoupon > 0 ? string.Format("可用券{0}", _.CanUseCashCoupon) : _.Rebate > 0 ? string.Format("{0}返{1}", _.Price, _.Rebate) : "";
                    _.RefundPolicy = _.Rebate > 0 ? "离店后返住基金" : "";

                    if (AccountAdapter.IsVIPCustomer(customerType))
                    {
                        _.VIPPrice = 0;
                    }
                });
            }
            //Log.WriteLog( sb.ToString());
            return hp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hotelID"></param>
        /// <param name="checkIn"></param>
        /// <param name="checkOut"></param>
        /// <param name="sType"></param>
        /// <param name="updatePrice"></param>
        /// <param name="appVer"></param>
        /// <param name="userId"></param>
        /// <param name="onlyNotZMJD">用来控制全部显示（zmjd 非zmjd套餐） 部分显示 zmjd或非zmjd套餐</param>
        /// <returns></returns>
        public static HotelPrice2 GetHotelPackageList(int hotelID, string checkIn, string checkOut, string sType, bool updatePrice, string appVer = null, long userId = 0, bool? onlyNotZMJD = true, bool needNotSalePackage = true,int pid = 0)
        {
            //用来在基本线上控制是否显示携程价格 即便onlyNotZMJD=true或null 若showCtripPrice=false 也不能显示
            var showCtripPriceDic = HotelService.CanShowCtripPrice(new int[] { hotelID });
            bool showCtripPrice = showCtripPriceDic.ContainsKey(hotelID) ? showCtripPriceDic[hotelID] : true;

            bool isAppV42 = !string.IsNullOrWhiteSpace(appVer) && appVer.CompareTo("4.2") >= 0 ? true : false;//old appVer friendly

            int terminalType = 0; if (sType == "www") terminalType = 3;//Web Access

            HJDAPI.Common.Helpers.Enums.CustomerType customerType = AccountAdapter.GetCustomerType(userId);
            HotelServiceEnums.PricePolicyType ppt = PriceAdapter.TransCustomerType2PricePolicyType(customerType);

            DateTime arrivalTime ;
            DateTime departureTime ;
            DateTime.TryParse(checkIn, out  arrivalTime);
            DateTime.TryParse(checkOut, out  departureTime);
            if (arrivalTime < DateTime.Now.Date)
            {
                arrivalTime = CommMethods.GetDefaultCheckIn();
            }

            if (departureTime < arrivalTime.AddDays(1))
            {
                departureTime = arrivalTime.AddDays(1);
            }

            HotelPrice2 hp = new HotelPrice2();
            List<OTAInfo2> otaInfos2 = new List<OTAInfo2>();
            List<PackageInfoEntity> packagePrice = new List<PackageInfoEntity>();

            //标识是否包含zmjd套餐
            var haveZmjdPackage = false;
            try
            {
                //默认 或者 不需要非周末酒店套餐时 取周末酒店套餐
                if (onlyNotZMJD == null || (onlyNotZMJD.HasValue && !(bool)onlyNotZMJD))
                {
                    genZMJDHotelPackages(hotelID, arrivalTime, departureTime, terminalType, ppt, checkIn, ref packagePrice, pid);//获取zmjd套餐
                }
            }
            catch (Exception ex)
            {
                Log.WriteLog("PriceAdapter.Get3:[GetHotelPackages]处出现异常:" + ex.Message + "|" + ex.StackTrace + string.Format("|参数id:{0},checkIn:{1},checkOut:{2},sType={3}", hotelID, checkIn, checkOut, sType));
            }

            ////过滤掉不售卖套餐 和 机酒游轮（package.WHPackageType = 1,2)
            //暂不过滤，前端Book页也调用 了这个方法
            //if (needNotSalePackage == false)
            //{
            //    packagePrice = packagePrice.Where(_ => _.IsNotSale == false && _.packageBase.WHPackageType == 0 ).ToList();
            //}

            //没有zmjd套餐且该酒店允许显示携程套餐则自动取其他套餐  可能会影响进入套餐列表的速度
            if ((packagePrice == null || !packagePrice.Any() || packagePrice.Count == 0 || packagePrice.Where(_ => _.IsNotSale == false).Count() == 0) && showCtripPrice)
            {
                onlyNotZMJD = true;
            }
            else
            {
                haveZmjdPackage = packagePrice.Exists(p => p.SellState == 1);
            }

            //Log.WriteLog("packagePrice：" + packagePrice.Count + "(onlyNotZMJD.HasValue && (bool)onlyNotZMJD)：" + (onlyNotZMJD.HasValue && (bool)onlyNotZMJD).ToString() + "  onlyNotZMJD :" + (onlyNotZMJD == null).ToString() + " !haveZmjdPackage  " + !haveZmjdPackage + "  checkIn " + checkIn);
            if (showCtripPrice)
            {
                try
                {
                    if (onlyNotZMJD == null || (onlyNotZMJD.HasValue && (bool)onlyNotZMJD))//|| !haveZmjdPackage
                    {
                        genNotZMJDHotelPackages(hotelID, arrivalTime, departureTime, ppt, checkIn, updatePrice, sType, haveZmjdPackage, isAppV42, ref packagePrice, ref otaInfos2);//同步取非周末酒店的价格
                    }
                    else
                    {
                        //异步取周末酒店的价格
                        var asyGetNotZMJDPackageTask = Task.Factory.StartNew(() =>
                        {
                            genNotZMJDHotelPackages(hotelID, arrivalTime, departureTime, ppt, checkIn, updatePrice, sType, haveZmjdPackage, isAppV42, ref packagePrice, ref otaInfos2);//同步取非周末酒店的价格
                            Log.WriteLog(string.Format("asyGetNotZMJDPackageTask已经执行，hotelID:{0}", hotelID));
                        });
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteLog("PriceAdapter.Get3出现异常:" + ex.Message + "|" + ex.StackTrace + string.Format("|参数id:{0},checkIn:{1},checkOut:{2},sType={3}", hotelID, checkIn, checkOut, sType));
                }
            }

            ////有价格的情况下，价格最低优先 合作OTA有四家，同等价格下，优先显示次序为携程、艺龙、住哪儿、同程
            hp.OTAList = otaInfos2;
            hp.HotelID = hotelID;
            HotelItem tempHotel = HotelService.GetHotel(hotelID);
            hp.Name = tempHotel != null ? tempHotel.Name : "";//2015-06-17 wwb 拆分HotelService.GetHotel(id).Name成两句
            //把港澳台 归为海外是 需要用户填写英文名称    政治无关  ---2018-10-29
            hp.InChina = (tempHotel.DistrictName == "香港" || tempHotel.DistrictName == "澳门" || tempHotel.DistrictName == "台湾") ? false : tempHotel.InChina;
            hp.CheckIn = arrivalTime;
            hp.CheckOut = departureTime;
            hp.Packages = packagePrice.OrderBy(p => p.SellState).ThenBy(p => p.PackageType).ToList();

            SetDayLimit(hp, packagePrice);//设置套餐起始售卖日期

            AddMorePackageInfo(hp.Packages);

            genPackageLabelAndChannels(hp.Packages, customerType);//取消政策 优惠类型等标签
            genPackageLabelAndChannels(hp.JLPackages, customerType);//取消政策 优惠类型等标签

            if (userId > 0)
            {
                GenPackageUpGradeInfo(hp.Packages, userId, customerType); //获取专辑升级
            }

            if (hp.Packages != null && hp.Packages.Any())
            {
                hp.Packages.ForEach(_ =>
                {
                    _.CancelPolicy = PriceAdapter.GenPackageCancelPolicy(_.Items);//获取退款政策

                    //补iOS 4.4版本取错label数量的问题
                    if (sType.Equals("ios") && !string.IsNullOrWhiteSpace(appVer) && appVer.CompareTo("4.5") < 0 && _.PayLabelUrls.Any())
                    {
                        _.PayLabelUrls.Add(_.PayLabelUrls.Last());
                    }

                    _.PriceAfterDeduct = _.Price - _.Rebate;
                    _.BenefitPolicy = _.CanUseCashCoupon > 0 ? string.Format("可用券{0}", _.CanUseCashCoupon) : _.Rebate > 0 ? string.Format("{0}返{1}", _.Price, _.Rebate) : "";
                    _.RefundPolicy = _.Rebate > 0 ? "离店后返住基金" : "";

                });
            }
            return hp;
        }

        private static void genZMJDHotelPackages(int id, DateTime arrivalTime, DateTime departureTime,
            int terminalType, HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType ppt, string checkIn, ref List<PackageInfoEntity> packagePrice,int pid = 0)
        {
            packagePrice = new PackageAdapter().GetHotelPackages(id, arrivalTime, departureTime, terminalType, ppt, pid);//获取zmjd套餐

            //如果缺省日期没有套餐可买，则寻找一个最近有套餐可以销售的日期
            if (checkIn == "0" && packagePrice.Count > 0 && packagePrice.Count == packagePrice.Where(p => p.SellState > 1).Count())
            {
                packagePrice = new PackageAdapter().GetCanSellHotelPackages(id, arrivalTime, arrivalTime, departureTime, ppt);
            }
        }

        private static void genNotZMJDHotelPackages(int id, DateTime arrivalTime, DateTime departureTime, HJD.HotelServices.Contracts.HotelServiceEnums.PricePolicyType ppt,
            string checkIn, bool updatePrice, string sType, bool haveZmjdPackage, bool isAppV42, ref List<PackageInfoEntity> packagePrice, ref List<OTAInfo2> otaInfos2)
        {
            #region 获取OTA渠道套餐(如深捷旅/携程/Booking等)

            //如果入住日小于今天，则不做任何OTA查询
            if (arrivalTime >= DateTime.Now.Date)
            {
                //改成走携程海外吧 //是否需要读取Booking（目前甲米等地酒店使用Booking跳转）
                var useBooking = false;//  PriceService.UseBooking(id);

                #region 不转向Booking

                if (!useBooking)
                {
                    #region 追加读取ctrip套餐

                    List<PackageInfoEntity> ctripPackages = new List<PackageInfoEntity>();
                    if (isAppV42)
                    {
                        //有zmjd套餐则通过api获取ctrip套餐（提高效率）
                        if (haveZmjdPackage)
                        {
                            //sb.AppendLine(" HotelService.GetCtripHotelPackagesForApiV42");
                            ctripPackages = HotelService.GetCtripHotelPackagesForApiV42(id, arrivalTime, departureTime, updatePrice, ppt);
                        }
                        //否则使用页面抓取获取ctrip套餐
                        else
                        {
                            //sb.AppendLine(" HotelService.GetCtripHotelPackagesForApiV42");
                            ctripPackages = HotelService.GetCtripHotelPackagesForApiV42(id, arrivalTime, departureTime, updatePrice, ppt);
                            //sb.AppendLine(" HotelService.GetCtripHotelPackagesV42");
                            //ctripPackages = HotelService.GetCtripHotelPackagesV42(id, arrivalTime, departureTime, updatePrice, ppt);
                        }
                    }
                    else
                    {
                        //sb.AppendLine(" HotelService.GetCtripHotelPackages");
                        ctripPackages = HotelService.GetCtripHotelPackages(id, arrivalTime, departureTime, updatePrice, ppt);
                    }

                    //如果获取到ctrip套餐则将其追加至zmjd套餐后面
                    if (ctripPackages != null && ctripPackages.Count > 0)
                    {
                        //ctripPackages.ForEach(_ =>
                        //{
                        //    _.CustomerAskPrice.Text = "获取更低价";
                        //    _.CustomerAskPrice.ActionUrl = "http://www.zmjiudian.com/MagiCall/MagiCallClient?arrivalTime=" + arrivalTime + "&departureTime=" + departureTime + "&RoomCode=" + _.Room.RoomCode + "&Price=" + _.Price + "&VIPPrice=" + _.VIPPrice + "&hotelId=" + _.packageBase.HotelID + "&userid={userid}";
                        //    // arrivalTime,departureTime,_.Room.RoomCode,_.Price,_.VIPPrice,_.packageBase.HotelID);
                        //});
                        //sb.AppendLine(" 如果获取到ctrip套餐则将其追加至zmjd套餐后面");
                      //  Log.WriteLog("ctripPackages" + ctripPackages.Count().ToString() + "  " + ctripPackages.FirstOrDefault() != null ? ctripPackages.FirstOrDefault().CustomerAskPrice.Text : "wu");
                        packagePrice.InsertRange(packagePrice.Count(), ctripPackages);
                    }
                    else
                    {
                        //当没有zmjd套餐 & 默认日期进来的，则去查询最近一天有套餐的日期
                        //sb.AppendLine(" 当没有zmjd套餐 & 默认日期进来的，则去查询最近一天有套餐的日期");

                        if (checkIn == "0" && packagePrice.Count == 0)
                        {
                            var ds = HotelService.GenCtripHotelPackageCalendar(id, arrivalTime);
                            if (ds != null && ds.Count > 0 && ds.Exists(d => d.Day > arrivalTime && d.SellState == 1))
                            {
                                var callSellDate = ds.Where(d => d.Day > arrivalTime && d.SellState == 1).OrderBy(d => d.Day).ToList().First();
                                arrivalTime = callSellDate.Day;
                                departureTime = arrivalTime.AddDays(1);
                                packagePrice =
                                    isAppV42
                                    ? (haveZmjdPackage ? HotelService.GetCtripHotelPackagesForApiV42(id, arrivalTime, departureTime, updatePrice, ppt) : HotelService.GetCtripHotelPackagesForApiV42(id, arrivalTime, departureTime, updatePrice, ppt)) //HotelService.GetCtripHotelPackagesV42(id, arrivalTime, departureTime, updatePrice, ppt))
                                        : HotelService.GetCtripHotelPackages(id, arrivalTime, departureTime, updatePrice, ppt);
                            }
                        }
                    }

                    //SetDayLimit(hp, packagePrice);

                    #endregion

                    #region 没有zmjd 没有ctrip 读取深捷旅

                    if (packagePrice.Count == 0)
                    {
                        List<PackageInfoEntity> JLPackages = HotelService.GetJLHotelPackages(id, arrivalTime, departureTime, ppt);
                        if (JLPackages.Count() > 0)
                        {
                            //如果是指定日，但指定日的套餐都无价，同时酒店套餐为空的话，那么不加入套餐记录
                            if (checkIn != "0" && packagePrice.Count() == 0 && JLPackages.Where(p => p.SellState == 1).Count() == 0)
                            {
                                //packagePrice = new PackageAdapter().GetJLCanSellHotelPackages(id, arrivalTime, out  arrivalTime, out  departureTime);
                                //SetDayLimit(hp, packagePrice);
                            }
                            else
                            {
                                packagePrice.InsertRange(packagePrice.Count(), JLPackages);

                                if (checkIn == "0" && packagePrice.Count > 0 && packagePrice.Count == packagePrice.Where(p => p.SellState > 1).Count())
                                {
                                    packagePrice = new PackageAdapter().GetJLCanSellHotelPackages(id, arrivalTime, out  arrivalTime, out  departureTime, ppt);
                                    //SetDayLimit(hp, packagePrice);
                                }
                            }
                        }
                    }

                    #endregion
                }

                #endregion

                #region 跳转至Booking

                else
                {
                    if (packagePrice.Count == 0)
                    {

                        //使用Booking前首先判断捷旅有没有数据，捷旅有套餐也要先用捷旅的，然后再说Booking
                        List<PackageInfoEntity> JLPackages = HotelService.GetJLHotelPackages(id, arrivalTime, departureTime);
                        if (JLPackages.Count() > 0)
                        {
                            //如果是指定日，但指定日的套餐都无价，同时酒店套餐为空的话，那么不加入套餐记录
                            if (checkIn != "0" && packagePrice.Count() == 0 && JLPackages.Where(p => p.SellState == 1).Count() == 0)
                            {
                                //packagePrice = new PackageAdapter().GetJLCanSellHotelPackages(id, arrivalTime, out  arrivalTime, out  departureTime);
                                //SetDayLimit(hp, packagePrice);
                            }
                            else
                            {
                                packagePrice.InsertRange(packagePrice.Count(), JLPackages);

                                if (checkIn == "0" && packagePrice.Count > 0 && packagePrice.Count == packagePrice.Where(p => p.SellState > 1).Count())
                                {
                                    packagePrice = new PackageAdapter().GetJLCanSellHotelPackages(id, arrivalTime, out  arrivalTime, out  departureTime, ppt);
                                    //SetDayLimit(hp, packagePrice);
                                }
                            }
                        }

                        #region 深捷旅也没有套餐的时候，则使用Booking跳转

                        if (packagePrice.Count == 0)
                        {
                            List<OTAInfo> otaInfos = HotelHelper.GetHotelOTAInfo(id, sType);

                            List<HotelPriceInfo2> hpl_new = new List<HotelPriceInfo2>();
                            List<HotelPriceInfo2> hpl = PriceService.QueryHotelPricePlan(id, arrivalTime);

                            if (hpl.Count > 0 && hpl.Count > 0)
                            {
                                foreach (HotelPriceInfo2 hpe in hpl)
                                {
                                    foreach (var toi in otaInfos)
                                    {
                                        if (toi != null && toi.ChannelID == hpe.ChannelID && toi.ChannelID > 0)
                                        {
                                            toi.Price = Decimal.Round(hpe.MinPrice, 0);
                                            //toi.PriceName = hpe.Name + "一卧室公寓";
                                            //toi.PriceBrief = hpe.Brief + "含早";
                                            toi.PriceName = hpe.Name;
                                            toi.PriceBrief = hpe.Brief;

                                            //一个渠道只需要拿取一个列表价
                                            if (!hpl_new.Exists(h => h.ChannelID == toi.ChannelID))
                                            {
                                                hpl_new.Add(hpe);
                                            }

                                            break;
                                        }
                                    }
                                }
                            }

                            var otaTransferUrlFormat = Configs.WWWURL + "/hotel/OtaTransfer?hotelId={0}&checkIn={1}&checkOut={2}";
                            if (!otaTransferUrlFormat.ToLower().Trim().StartsWith("http")) otaTransferUrlFormat = "http://" + otaTransferUrlFormat;

                            otaInfos2 = (from o in otaInfos
                                         join p in hpl_new on o.ChannelID equals p.ChannelID into temp
                                         from hpi in temp.DefaultIfEmpty()
                                         select new OTAInfo2
                                         {
                                             AccessURL = o.AccessURL,
                                             ChannelID = o.ChannelID,
                                             Name = o.Name,
                                             OTAHotelID = o.OTAHotelID,
                                             Price = hpi == null ? 0 : hpi.MinPrice,
                                             CanSyncPrice = o.CanSyncPrice,
                                             RoomRates = GenRoomRates(hpi),
                                             PriceName = o.PriceName,
                                             PriceBrief = o.PriceBrief,
                                             OtaTransferURL = string.Format(otaTransferUrlFormat, id, "0", "0")
                                         }).ToList();

                            otaInfos2 = otaInfos2.Where(o => o.ChannelID != 102 && o.Price > 0).OrderBy(o => o.ChannelID == 102 ? 0 : 1).ThenBy(o => o.Price == 0 ? 10000 : o.Price).ThenBy(o => o.ChannelID == 102 ? 0 : (o.ChannelID == 2 ? 1 : (o.ChannelID == 6 ? 2 : (o.ChannelID == 11 ? 3 : (o.ChannelID == 6 ? 4 : 100))))).ToList();

                            if (otaInfos2.Count() > 0 && otaInfos2.First().ChannelID == 102 && otaInfos2.First().Price == 0) otaInfos2.RemoveAt(0);

                            //价格升序
                            if (otaInfos2 != null && otaInfos2.Count > 0 && otaInfos2.Exists(ota => ota.Price > 0))
                            {
                                otaInfos2 = new List<OTAInfo2> { otaInfos2.Where(ota => ota.Price > 0).OrderBy(ota => ota.Price).First() };
                            }
                        }

                        #endregion

                    }
                }

                #endregion
            }

            #endregion
        }


        /// <summary>
        /// 查询酒店指定日期的套餐列表（目前该方法主要用于酒店列表价抓取服务中）
        /// </summary>
        /// <param name="id"></param>
        /// <param name="checkIn"></param>
        /// <param name="checkOut"></param>
        /// <param name="sType"></param>
        /// <param name="updatePrice"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static HotelPrice2 Get32(int id, string checkIn, string checkOut, string sType, bool updatePrice, long userId = 0)
        {
            HotelPrice2 hp = new HotelPrice2();
            try
            {
                //用来在基本线上控制是否显示携程价格 即便onlyNotZMJD=true或null 若showCtripPrice=false 也不能显示
                var showCtripPriceDic = HotelService.CanShowCtripPrice(new int[] { id });
                bool showCtripPrice = showCtripPriceDic.ContainsKey(id) ? showCtripPriceDic[id] : true;

                DateTime arrivalTime = CommMethods.GetDefaultCheckIn();
                DateTime departureTime = arrivalTime.AddDays(1);

                if (checkIn != "" && checkIn != "0")
                {
                    try
                    {
                        arrivalTime = DateTime.Parse(checkIn);
                        departureTime = DateTime.Parse(checkOut);
                        if (departureTime < arrivalTime.AddDays(1)) departureTime = arrivalTime.AddDays(1);
                    }
                    catch
                    {

                    }
                }

               List<OTAInfo2> otaInfos2 = new List<OTAInfo2>();
                List<PackageInfoEntity> packagePrice = new List<PackageInfoEntity>();
                HJDAPI.Common.Helpers.Enums.CustomerType customerType = AccountAdapter.GetCustomerType(userId);
                HotelServiceEnums.PricePolicyType ppt = PriceAdapter.TransCustomerType2PricePolicyType(customerType);

                //标识是否包含zmjd套餐
                var haveZmjdPackage = false;

                #region 优先读取zmjd套餐

                try
                {
                    //Web Access
                    int terminalType = 0; if (sType == "www") terminalType = 3;

                    //获取zmjd套餐
                    packagePrice = new PackageAdapter().GetHotelPackages(id, arrivalTime, departureTime, terminalType, ppt);
                    SetDayLimit(hp, packagePrice);

                    //如果缺省日期没有套餐可买，则寻找一个最近有套餐可以销售的日期
                    if (checkIn == "0" && packagePrice.Count > 0 && packagePrice.Count == packagePrice.Where(p => p.SellState > 1).Count())
                    {
                        packagePrice = new PackageAdapter().GetCanSellHotelPackages(id, arrivalTime, arrivalTime, departureTime, ppt);
                        SetDayLimit(hp, packagePrice);
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteLog("PriceAdapter.Get32:[GetHotelPackages]处出现异常:" + ex.Message + "|" + ex.StackTrace + string.Format("|参数id:{0},checkIn:{1},checkOut:{2},sType={3}", id, checkIn, checkOut, sType));
                }

                haveZmjdPackage = packagePrice.Exists(p => p.SellState == 1 && !p.IsNotSale);

                #endregion

                //如果入住日小于今天，则不做任何OTA查询
                if (arrivalTime >= DateTime.Now.Date)
                {
                    //是否需要读取Booking（目前甲米等地酒店使用Booking跳转）
                    var useBooking = PriceService.UseBooking(id);

                    #region 不转向Booking

                    if (!useBooking)
                    {
                        #region 追加读取ctrip套餐

                        if (showCtripPrice)
                        {
                            var ctripPackages = HotelService.GetCtripHotelPackagesForApiV42(id, arrivalTime, departureTime, updatePrice, ppt);

                            //如果获取到ctrip套餐则将其追加至zmjd套餐后面
                            if (ctripPackages != null && ctripPackages.Count > 0)
                            {
                                packagePrice.InsertRange(packagePrice.Count(), ctripPackages);
                            }
                            else
                            {
                                //当没有zmjd套餐 & 默认日期进来的，则去查询最近一天有套餐的日期
                                if (checkIn == "0" && packagePrice.Count == 0)
                                {
                                    var ds = HotelService.GenCtripHotelPackageCalendar(id, arrivalTime);
                                    if (ds != null && ds.Count > 0 && ds.Exists(d => d.Day > arrivalTime && d.SellState == 1))
                                    {
                                        var callSellDate = ds.Where(d => d.Day > arrivalTime && d.SellState == 1).OrderBy(d => d.Day).ToList().First();
                                        arrivalTime = callSellDate.Day;
                                        departureTime = arrivalTime.AddDays(1);
                                        packagePrice = HotelService.GetCtripHotelPackagesForApiV42(id, arrivalTime, departureTime, updatePrice, ppt);
                                    }
                                }
                            }

                            SetDayLimit(hp, packagePrice);   
                        }

                        #endregion

                        #region 没有zmjd 没有ctrip 读取深捷旅

                        if (packagePrice.Count == 0)
                        {
                            List<PackageInfoEntity> JLPackages = HotelService.GetJLHotelPackages(id, arrivalTime, departureTime, ppt);
                            if (JLPackages.Count() > 0)
                            {
                                //如果是指定日，但指定日的套餐都无价，同时酒店套餐为空的话，那么不加入套餐记录
                                if (checkIn != "0" && packagePrice.Count() == 0 && JLPackages.Where(p => p.SellState == 1).Count() == 0)
                                {
                                    //packagePrice = new PackageAdapter().GetJLCanSellHotelPackages(id, arrivalTime, out  arrivalTime, out  departureTime);
                                    //SetDayLimit(hp, packagePrice);
                                }
                                else
                                {
                                    packagePrice.InsertRange(packagePrice.Count(), JLPackages);

                                    if (checkIn == "0" && packagePrice.Count > 0 && packagePrice.Count == packagePrice.Where(p => p.SellState > 1).Count())
                                    {
                                        packagePrice = new PackageAdapter().GetJLCanSellHotelPackages(id, arrivalTime, out  arrivalTime, out  departureTime, ppt);
                                        SetDayLimit(hp, packagePrice);
                                    }
                                }
                            }
                        }

                        #endregion
                    }

                    #endregion
                }

                hp.OTAList = otaInfos2;
                hp.HotelID = id;
                HotelItem tempHotel = HotelService.GetHotel(id);
                hp.Name = tempHotel != null ? tempHotel.Name : "";//2015-06-17 wwb 拆分HotelService.GetHotel(id).Name成两句
                hp.CheckIn = arrivalTime;
                hp.CheckOut = departureTime;
                hp.Packages = packagePrice.Where(p => p.IsNotSale == false).OrderBy(p => p.SellState).ThenBy(p => p.PackageType).ToList();
            }
            catch
            {

            }
            return hp;
        }

        public static string GenPackageCancelPolicy(IEnumerable<HJD.HotelServices.Contracts.PItemEntity> items)
        {
            var result = new HJD.HotelServices.Contracts.PItemEntity();
            if (items != null && items.Any())
            {
                result = items.FirstOrDefault(_ => _.ItemCode.Equals("取消政策"));
            }
            return result == null ? "" : result.Description;
        }

        public static HotelServiceEnums.PricePolicyType TransCustomerType2PricePolicyType(HJDAPI.Common.Helpers.Enums.CustomerType customerType)
        {
            HotelServiceEnums.PricePolicyType ppt = HotelServiceEnums.PricePolicyType.Default;
            switch (customerType)
            {
                case HJDAPI.Common.Helpers.Enums.CustomerType.botao:
                    ppt = HotelServiceEnums.PricePolicyType.Botao;
                    break;
                case HJDAPI.Common.Helpers.Enums.CustomerType.vip:
                case HJDAPI.Common.Helpers.Enums.CustomerType.vip199:
                case HJDAPI.Common.Helpers.Enums.CustomerType.vip199nr:
                case HJDAPI.Common.Helpers.Enums.CustomerType.vip3M:
                case HJDAPI.Common.Helpers.Enums.CustomerType.vip6M:
                case HJDAPI.Common.Helpers.Enums.CustomerType.vip599:
                    ppt = HotelServiceEnums.PricePolicyType.VIP;
                    break;
            }

            return ppt;
        }

        //private static void genHotelPackagePriceAndCashCoupon4BoTao(List<PackageInfoEntity> packages)
        //{
        //    if (packages != null && packages.Count != 0)
        //    {
        //        packages = packages.FindAll(_ => _.PayType == 3);//预付
        //        if (packages != null && packages.Count > 0)
        //        {
        //            packages.ForEach(_ =>
        //            {
        //                _.Rates = _.Rates.FindAll(r => r.PayType == 3);
        //                _.DailyItems = _.DailyItems.FindAll(r => r.PayType == 3);
        //                _.DailyItems.ForEach(s =>
        //                {
        //                    s.Price -= 5;//铂韬用户一致减去5元 待定
        //                    s.CanUseCashCoupon = 0;//博涛用户不能用券

        //                    //s.Price -= s.CanUseCashCoupon == 0 ? 20 : s.CanUseCashCoupon;
        //                    //s.CanUseCashCoupon = s.CanUseCashCouponForBoTao;
        //                });

        //                _.Price -= 5 * _.DailyItems.Count;
        //                _.CanUseCashCoupon = 0;//博涛用户 不能用券

        //                //_.Price -= _.CanUseCashCoupon == 0 ? 20 * _.DailyItems.Count : _.CanUseCashCoupon;//很多天的sum
        //                //_.CanUseCashCoupon = _.CanUseCashCouponForBoTao;//很多天的sum
        //            });
        //        }
        //    }
        //}

        private static void genPackageLabelAndChannels(List<PackageInfoEntity> packages, HJDAPI.Common.Helpers.Enums.CustomerType customerType)
        {
            if (packages != null && packages.Count != 0)
            {
                packages.ForEach(_ =>
                {
                    var cancelPolicy = _.LastCancelTime <= DateTime.Now ? 1 : 2;
                    _.PayChannels = OrderAdapter.genOrderPayChannels(customerType, _.PayType, "");
                    _.PayLabelUrls = OrderAdapter.genOrderPayLabelUrls(customerType, _.PayType, cancelPolicy,_.packageBase.ForVIPFirstBuy,_.PackageType,_.packageBase.DayLimitMin);


                    _.DailyItems.ForEach(d =>
                    {
                        d.PayChannels = OrderAdapter.genOrderPayChannels(customerType, d.PayType,"");
                        d.PayLabelUrls = OrderAdapter.genOrderPayLabelUrls(customerType, d.PayType, cancelPolicy, _.packageBase.ForVIPFirstBuy, _.PackageType, _.packageBase.DayLimitMin);
                    });
                });
            }
        }

        /// <summary>
        /// 获取指定酒店列表价
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="id">酒店ID</param>
        /// <param name="checkIn">ref返回值，返回最终列表价对应的日期</param>
        /// <param name="pricePlan"></param>
        /// <param name="otaList2"></param>
        /// <param name="fixDate">是否指定日期(默认不指定)</param>
        public static void GetPricePlanAndOtaList(long userId, int id, ref DateTime checkIn, ref HotelMinPriceEntity hotelMinPrice, ref List<OTAInfo2> otaList2, bool fixDate = false)
        {
            //otalist
            otaList2 = new List<OTAInfo2>();

            //查询日期（没有指定日期则从明天开始查询最低列表价 | 指定了日期，则从指定日期开始查询所有列表价）
            var arrivalTime = fixDate ? checkIn : CommMethods.GetDefaultCheckIn();

            //获取当前用户遵循的列表价类型
            var minPriceTypes = GetHotelMinPriceTypeByUser(userId);

            //获取HotelMinPrice列表价
            var hotelMinPriceList = PriceService.GetHotelMinPriceList(new int[] { id }, arrivalTime) ?? new List<HotelMinPriceEntity>();
            var pricePlanList = new List<PricePlanEx>();

            if (hotelMinPriceList == null || hotelMinPriceList.Count == 0)
            {
                //获取PricePlan列表价
                pricePlanList = PriceService.GetPricePlanExList(new int[] { id }, arrivalTime) ?? new List<PricePlanEx>();   
            }

            //统计列表价
            hotelMinPrice = GetHotelPricePlan(id, pricePlanList, hotelMinPriceList, minPriceTypes, arrivalTime, fixDate);

            //拿到可用的列表价，更新CheckIn为当前列表价的日期
            if (hotelMinPrice.Price > 0)
            {
                checkIn = hotelMinPrice.Date;
            }

            #region 如果最终列表价 非zmjd、非深捷旅、非ctrip，则检查是否需要跳转Booking or agoda

            var checkChannelIdList = new List<int> { 0, 2, 100, 102, 103 };
            if (!checkChannelIdList.Contains(hotelMinPrice.ChannelID))
            {
                var pricePlan = new PricePlanEx();

                //是否需要读取Booking（目前甲米等地酒店使用Booking跳转）
                var useBooking = PriceService.UseBooking(id);

                #region Use Booking

                if (useBooking)
                {
                    #region 读取OtaList

                    List<OTAInfo> otaInfos = HotelHelper.GetHotelOTAInfo(id, "ios");

                    List<HotelPriceInfo2> hpl_new = new List<HotelPriceInfo2>();
                    List<HotelPriceInfo2> hpl = PriceService.QueryHotelPricePlan(id, arrivalTime);
                    if (hpl.Count > 0 && hpl.Count > 0)
                    {
                        foreach (HotelPriceInfo2 hpe in hpl)
                        {
                            foreach (var toi in otaInfos)
                            {
                                if (toi != null && toi.ChannelID == hpe.ChannelID && toi.ChannelID > 0)
                                {
                                    toi.Price = Decimal.Round(hpe.MinPrice, 0);
                                    toi.PriceName = hpe.Name;
                                    toi.PriceBrief = hpe.Brief;

                                    //一个渠道只需要拿取一个列表价
                                    if (!hpl_new.Exists(h => h.ChannelID == toi.ChannelID))
                                    {
                                        hpl_new.Add(hpe);
                                    }

                                    break;
                                }
                            }
                        }
                    }

                    var otaTransferUrlFormat = Configs.WWWURL + "/hotel/OtaTransfer?hotelId={0}&checkIn={1}&checkOut={2}";
                    if (!otaTransferUrlFormat.ToLower().Trim().StartsWith("http")) otaTransferUrlFormat = "http://" + otaTransferUrlFormat;

                    otaList2 = (from o in otaInfos
                                join p in hpl_new on o.ChannelID equals p.ChannelID into temp
                                from hpi in temp.DefaultIfEmpty()
                                select new OTAInfo2
                                {
                                    AccessURL = o.AccessURL,
                                    ChannelID = o.ChannelID,
                                    Name = o.Name,
                                    OTAHotelID = o.OTAHotelID,
                                    Price = hpi == null ? 0 : hpi.MinPrice,
                                    CanSyncPrice = o.CanSyncPrice,
                                    RoomRates = GenRoomRates(hpi),
                                    PriceName = o.PriceName,
                                    PriceBrief = o.PriceBrief,
                                    OtaTransferURL = string.Format(otaTransferUrlFormat, id, "0", "0")
                                }).ToList();

                    otaList2 = otaList2.Where(o => o.ChannelID != 102).OrderBy(o => o.ChannelID == 102 ? 0 : 1).ThenBy(o => o.Price == 0 ? 10000 : o.Price).ThenBy(o => o.ChannelID == 102 ? 0 : (o.ChannelID == 2 ? 1 : (o.ChannelID == 6 ? 2 : (o.ChannelID == 11 ? 3 : (o.ChannelID == 6 ? 4 : 100))))).ToList();

                    if (otaList2.Count() > 0 && otaList2.First().ChannelID == 102 && otaList2.First().Price == 0) otaList2.RemoveAt(0);

                    //价格升序
                    if (otaList2 != null && otaList2.Count > 0)
                    {
                        otaList2 = otaList2.Where(ota => ota.Price > 0).OrderBy(ota => ota.Price).ToList();
                    }

                    #endregion

                    #region 读取ota的列表价 2015-08-31 Haoy

                    if (pricePlanList.Exists(p => !checkChannelIdList.Contains(p.ChannelID)))
                    {
                        pricePlan = pricePlanList.Where(p => !checkChannelIdList.Contains(p.ChannelID)).OrderBy(p => p.Date).First();
                    }
                    else
                    {
                        if (otaList2 != null && otaList2.Count > 0)
                        {
                            var firstOta = otaList2[0];
                            pricePlan = new PricePlanEx
                            {
                                HotelID = id,
                                Date = arrivalTime,
                                ChannelID = firstOta.ChannelID,
                                Price = (int)firstOta.Price,
                                Name = firstOta.PriceName,
                                Brief = firstOta.PriceBrief
                            };
                        }
                    }

                    //Ota的返回一个价格为0的列表价，这样前台就不会显示套餐，而是直接解析OtaList
                    pricePlan.Price = 0;

                    #endregion

                    //更新CheckIn为当前OTA的价格
                    checkIn = arrivalTime;
                }

                #endregion
            }

            #endregion
        }

        /// <summary>
        /// 获取指定酒店的列表价 2016.11.09 haoy 【目前最新版都用该方法获取基础列表价，如果后期变更，请修改注释说明清楚 haoy 2016.12.13】
        /// </summary>
        /// <param name="hotelId">指定酒店ID</param>
        /// <param name="pricePlanList">包含该酒店所有列表价的价格集合（PricePlan&PricePlanEx表）</param>
        /// <param name="hotelMinPriceList">包含该酒店所有列表价的集合（HotelMinPrice表）</param>
        /// <param name="minPriceTypes">指定遵循的起价类型集合</param>
        /// <param name="arrivalTime">列表价查询起始日期</param>
        /// <param name="fixDate">是否指定日期</param>
        /// <returns></returns>
        public static HotelMinPriceEntity GetHotelPricePlan(int hotelId, List<PricePlanEx> pricePlanList, List<HotelMinPriceEntity> hotelMinPriceList, List<HotelMinPriceType> minPriceTypes, DateTime arrivalTime, bool fixDate)
        {
            //Log.WriteLog("minPriceTypes：" + minPriceTypes.Count);

            var hotelMinPrice = new HotelMinPriceEntity { HotelID = hotelId, Price = 0, VipPrice = 0, Name = "", Brief = "", ChannelID = 0, PID = 0, Type = 0 };

            //目前列表价可能包含的几种ChannelID类型
            var zmjdChannelIdList = new List<int> { 1, 100 };
            var checkChannelIdList = new List<int> { 0, 2, 100, 102, 103 };

            //60天后的日期
            var date60 = DateTime.Now.Date.AddDays(59);

            //初始保障
            pricePlanList = pricePlanList ?? new List<PricePlanEx>();
            hotelMinPriceList = hotelMinPriceList ?? new List<HotelMinPriceEntity>();

            //只取60内最低价
            if (hotelMinPriceList.Count > 0) { hotelMinPriceList = hotelMinPriceList.Where(_ => _.Date <= date60 && _.HotelID == hotelId && _.Price > 0 && minPriceTypes.Contains((HotelMinPriceType)_.Type)).ToList(); }
            if (pricePlanList.Count > 0) { pricePlanList = pricePlanList.Where(_ => _.Date <= date60 && _.HotelID == hotelId && _.Price > 0).ToList(); }
            
            try
            {
                #region 存在HotelMinPrice

                if (hotelMinPriceList.Count > 0)
                {
                    //【1】如果指定了入住日期，则只筛选这一天的价格
                    if (fixDate && hotelMinPriceList.Exists(p => p.Date == arrivalTime))
                    {
                        hotelMinPriceList = hotelMinPriceList.Where(p => p.Date == arrivalTime).ToList();
                    }

                    //【2】如果HotelMinPriceType允许包含了1（新VIP专享）的列表价类型，则优先筛选1的列表价
                    if (minPriceTypes.Contains(HotelMinPriceType.NewVip) && hotelMinPriceList.Exists(p => p.Type == 1))
                    {
                        hotelMinPriceList = hotelMinPriceList.Where(p => p.Type == 1).ToList();
                    }

                    //【3】如果存在zmjd套餐，则只取它们之间的列表价
                    if (hotelMinPriceList.Exists(p => zmjdChannelIdList.Contains(p.ChannelID)))
                    {
                        hotelMinPriceList = hotelMinPriceList.Where(p => zmjdChannelIdList.Contains(p.ChannelID)).ToList();
                    }

                    //【4】如果存在其他ota（如ctrip）的套餐，则只取它们之间的列表价
                    else if (hotelMinPriceList.Exists(p => checkChannelIdList.Contains(p.ChannelID)))
                    {
                        hotelMinPriceList = hotelMinPriceList.Where(p => checkChannelIdList.Contains(p.ChannelID)).ToList();
                    }

                    //【5】默认最便宜的价格
                    hotelMinPrice = hotelMinPriceList.OrderBy(p => p.Price).First();

                    //生成更新列表价的任务队列
                    if (hotelMinPrice != null && hotelMinPrice.Price > 0)
                    {
                        HotelAdapter.PublishUpdatePriceSlotTask(hotelId, hotelMinPrice.Date.ToString("yyyy-MM-dd"));
                    }
                }

                #endregion

                #region 存在PricePlan

                else if (pricePlanList.Count > 0)
                {
                    var pricePlan = new PricePlanEx();

                    //【1】如果指定了入住日期，则只筛选这一天的价格
                    if (fixDate && pricePlanList.Exists(p => p.Date == arrivalTime))
                    {
                        pricePlanList = pricePlanList.Where(p => p.Date == arrivalTime).ToList();
                    }

                    //【2】如果存在zmjd套餐，则只取它们之间的列表价
                    if (pricePlanList.Exists(p => zmjdChannelIdList.Contains(p.ChannelID)))
                    {
                        pricePlanList = pricePlanList.Where(p => zmjdChannelIdList.Contains(p.ChannelID)).ToList();
                    }

                    //【3】如果存在其他ota（如ctrip）的套餐，则只取它们之间的列表价
                    else if (pricePlanList.Exists(p => checkChannelIdList.Contains(p.ChannelID)))
                    {
                        pricePlanList = pricePlanList.Where(p => checkChannelIdList.Contains(p.ChannelID)).ToList();
                    }

                    //【4】默认最便宜的价格
                    pricePlan = pricePlanList.OrderBy(p => p.Price).First();

                    //生成更新列表价的任务队列
                    if (pricePlan != null && pricePlan.Price > 0)
                    {
                        HotelAdapter.PublishUpdatePriceSlotTask(hotelId, pricePlan.Date.ToString("yyyy-MM-dd"));
                    }

                    hotelMinPrice.Price = pricePlan.Price;
                    hotelMinPrice.VipPrice = pricePlan.VipPrice;
                    hotelMinPrice.Name = pricePlan.Name;
                    hotelMinPrice.Brief = pricePlan.Brief;
                    hotelMinPrice.Date = pricePlan.Date;
                    hotelMinPrice.NightCount = pricePlan.NightCount;
                    hotelMinPrice.ChannelID = pricePlan.ChannelID;
                }

                #endregion

                #region 没有拿到列表价的，读取BasePrice

                else
                {
                    var hotelBasePriceList = QueryHotelBasePrice(new List<int>() { hotelId }).FindAll(_ => _.MinPrice > 0);
                    if (hotelBasePriceList != null && hotelBasePriceList.Count > 0 && hotelBasePriceList[0].MinPrice > 0)
                    {
                        var _checkIn = fixDate ? arrivalTime : CommMethods.GetDefaultCheckIn();

                        hotelMinPrice = new HotelMinPriceEntity { HotelID = hotelId, Price = Convert.ToInt32(hotelBasePriceList[0].MinPrice), ChannelID = 100, Date = _checkIn };
                    }
                }

                #endregion
            }
            catch (Exception e)
            {
                Log.WriteLog("GetHotelPricePlan " + e);
            }

            return hotelMinPrice;
        }

        /// <summary>
        /// 获取指定用户遵循的酒店列表价的类型
        /// </summary>
        /// <param name="userId">zmjd用户ID</param>
        /// <returns></returns>
        public static List<HotelMinPriceType> GetHotelMinPriceTypeByUser(long userId)
        {
            var minPriceTypes = new List<HotelMinPriceType> { HotelMinPriceType.Default, HotelMinPriceType.NewVip };

            if (userId > 0)
            {
                //minPriceTypes = new List<HotelMinPriceType> { HotelMinPriceType.Default };

                //获取用户是否VIP
                var isVip = AccountAdapter.IsVIPCustomer(AccountAdapter.GetCustomerType(userId));

                //获取用户权限
                var privList = PrivilegeAdapter.GetAllPrivilegeByUserId(userId) ?? new List<UserPrivilegeRel>();

                //非VIP或者新VIP（有爆款权限的VIP），才能看到所有类型的列表价
                if (!isVip || (isVip && privList.Exists(_ => _.PrivID == 2010)))
                {
                    minPriceTypes = new List<HotelMinPriceType> { HotelMinPriceType.Default, HotelMinPriceType.NewVip };
                }
                else
                { 
                    //如果是VIP，但没有了爆款权限，则不显示1的列表价（新VIP专享列表价）
                    minPriceTypes = new List<HotelMinPriceType> { HotelMinPriceType.Default };
                }
            }

            return minPriceTypes;
        }

        /// <summary>
        /// 获取指定酒店套餐的多天日历价格
        /// </summary>
        /// <param name="hotelid"></param>
        /// <param name="pid"></param>
        /// <param name="userType"></param>
        /// <param name="nightCount"></param>
        /// <param name="totalPrice"></param>
        /// <param name="totalVipPrice"></param>
        /// <returns></returns>
        public static void GetManyDaysPackagePriceCached(int hotelid, int pid, HJDAPI.Common.Helpers.Enums.CustomerType userType, int nightCount, out int totalPrice, out int totalVipPrice) 
        {
            totalPrice = 0;
            totalVipPrice = 0;

            try
            {
                var startDate = CommMethods.GetDefaultCheckIn();

                var list = PackageAdapter.GetHotelPackageCalendarCachedWithCustomerType(hotelid, startDate, userType, pid: pid);
                if (list != null && list.Count > 0 && list.Exists(_ => _.SellPrice > 0))
                {
                    var firstDayObj = list.First(d => d.SellState == 1);

                    var _startDate = firstDayObj.Day;
                    for (int i = 0; i < nightCount; i++)
                    {
                        var _calendarItem = list.Find(_ => _.Day == _startDate.AddDays(i));

                        //normal total price
                        totalPrice += _calendarItem.NormalPrice;

                        //vip total price
                        totalVipPrice += _calendarItem.VipPrice;
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        /// <summary>
        /// 获取指定酒店套餐的多天日历价格
        /// </summary>
        /// <param name="hotelid"></param>
        /// <param name="pid"></param>
        /// <param name="userType"></param>
        /// <param name="nightCount"></param>
        /// <param name="totalPrice"></param>
        /// <param name="totalVipPrice"></param>
        /// <returns></returns>
        public static void GetManyDaysPackagePriceCached(int hotelid, int pid, HJDAPI.Common.Helpers.Enums.CustomerType userType, int nightCount, out int totalPrice, out int totalVipPrice, out DateTime startTime, out decimal totalMamuCommission, out decimal totalAutoCommission)
        {
            totalPrice = 0;
            totalVipPrice = 0;
            startTime = DateTime.Now;
            totalMamuCommission = 0m;
            totalAutoCommission = 0m;
            
            try
            {
                var startDate = CommMethods.GetDefaultCheckIn();

                var list = PackageAdapter.GetHotelPackageCalendarCachedWithCustomerType(hotelid, startDate, userType, pid: pid);
                if (list != null && list.Count > 0 && list.Exists(_ => _.SellPrice > 0))
                {
                    var firstDayObj = list.First(d => d.SellState == 1);
                    startTime = firstDayObj.Day;
                    var _startDate = firstDayObj.Day;
                    for (int i = 0; i < nightCount; i++)
                    {
                        var _calendarItem = list.Find(_ => _.Day == _startDate.AddDays(i));

                        //normal total price
                        totalPrice += _calendarItem.NormalPrice;

                        //vip total price
                        totalVipPrice += _calendarItem.VipPrice;

                        totalAutoCommission += _calendarItem.AutoCommission;
                        totalMamuCommission += _calendarItem.ManualCommission;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        /// <summary>
        /// 获取指定酒店、入住日期的酒店套餐（改套餐没有套餐信息，只用来获取OtaList）
        /// </summary>
        /// <param name="id"></param>
        /// <param name="checkIn"></param>
        /// <param name="checkOut"></param>
        /// <param name="sType"></param>
        /// <param name="needCache"></param>
        /// <returns></returns>
        public static HotelPrice2 GetOtaList(int id, string checkIn, string checkOut, string sType)
        {
            DateTime arrivalTime = CommMethods.GetDefaultCheckIn();
            DateTime departureTime = arrivalTime.AddDays(1);

            if (checkIn != "")
            {
                try
                {
                    arrivalTime = DateTime.Parse(checkIn);
                    departureTime = DateTime.Parse(checkOut);
                    if (departureTime < arrivalTime.AddDays(1)) departureTime = arrivalTime.AddDays(1);
                }
                catch
                {
                    //arrivalTime = departureTime = DateTime.MinValue;
                }
            }

            HotelPrice2 hp = new HotelPrice2();
            List<OTAInfo2> otaInfos2 = new List<OTAInfo2>();
            List<PackageInfoEntity> packagePrice = new List<PackageInfoEntity>();

            try
            {
                List<OTAInfo> otaInfos = HotelHelper.GetHotelOTAInfo(id, sType);

                List<HotelPriceInfo2> hpl_new = new List<HotelPriceInfo2>();
                List<HotelPriceInfo2> hpl = PriceService.QueryHotelPricePlan(id, arrivalTime);

                if (hpl.Count > 0 && hpl.Count > 0)
                {
                    foreach (HotelPriceInfo2 hpe in hpl)
                    {
                        foreach (var toi in otaInfos)
                        {
                            if (toi != null && toi.ChannelID == hpe.ChannelID && toi.ChannelID > 0)
                            {
                                toi.Price = Decimal.Round(hpe.MinPrice, 0);
                                //toi.PriceName = hpe.Name + "一卧室公寓";
                                //toi.PriceBrief = hpe.Brief + "含早";
                                toi.PriceName = hpe.Name;
                                toi.PriceBrief = hpe.Brief;

                                //一个渠道只需要拿取一个列表价
                                if (!hpl_new.Exists(h => h.ChannelID == toi.ChannelID))
                                {
                                    hpl_new.Add(hpe);
                                }

                                break;
                            }
                        }
                    }
                }

                var otaTransferUrlFormat = Configs.WWWURL + "/hotel/OtaTransfer?hotelId={0}&checkIn={1}&checkOut={2}";
                if (!otaTransferUrlFormat.ToLower().Trim().StartsWith("http")) otaTransferUrlFormat = "http://" + otaTransferUrlFormat;

                otaInfos2 = (from o in otaInfos
                             join p in hpl_new on o.ChannelID equals p.ChannelID into temp
                             from hpi in temp.DefaultIfEmpty()
                             select new OTAInfo2
                             {
                                 AccessURL = o.AccessURL,
                                 ChannelID = o.ChannelID,
                                 Name = o.Name,
                                 OTAHotelID = o.OTAHotelID,
                                 Price = hpi == null ? 0 : hpi.MinPrice,
                                 CanSyncPrice = o.CanSyncPrice,
                                 RoomRates = GenRoomRates(hpi),
                                 PriceName = o.PriceName,
                                 PriceBrief = o.PriceBrief,
                                 OtaTransferURL = string.Format(otaTransferUrlFormat, id, "0", "0")
                             }).ToList();

                otaInfos2 = otaInfos2.Where(o => o.ChannelID != 102).OrderBy(o => o.ChannelID == 102 ? 0 : 1).ThenBy(o => o.Price == 0 ? 10000 : o.Price).ThenBy(o => o.ChannelID == 102 ? 0 : (o.ChannelID == 2 ? 1 : (o.ChannelID == 6 ? 2 : (o.ChannelID == 11 ? 3 : (o.ChannelID == 6 ? 4 : 100))))).ToList();

                if (otaInfos2.Count() > 0 && otaInfos2.First().ChannelID == 102 && otaInfos2.First().Price == 0) otaInfos2.RemoveAt(0);

                ////有价格的情况下，价格最低优先
                ////合作OTA有四家，同等价格下，优先显示次序为携程、艺龙、住哪儿、同程
                hp.OTAList = otaInfos2;
                hp.HotelID = id;
                HotelItem tempHotel = HotelService.GetHotel(id);
                hp.Name = tempHotel != null ? tempHotel.Name : "";//2015-06-17 wwb 拆分HotelService.GetHotel(id).Name成两句
                hp.CheckIn = arrivalTime;
                hp.CheckOut = departureTime;
            }
            catch (Exception ex)
            {
                Log.WriteLog("PriceAdapter.Get3出现异常:" + ex.Message + "|" + ex.StackTrace + string.Format("|参数id:{0},checkIn:{1},checkOut:{2},sType={3}", id, checkIn, checkOut, sType));
            }
            finally
            {

            }

            return hp;
        }

        /// <summary>
        /// 根据酒店ID & 套餐Code获取指定套餐信息
        /// </summary>
        /// <param name="hotelId">酒店ID</param>
        /// <param name="code">套餐Code</param>
        /// <returns></returns>
        public static HotelPrice2 Get4(int hotelId, string code, string sType, int pid)
        {
            HotelPrice2 hp = new HotelPrice2();
            DateTime arrivalTime = CommMethods.GetDefaultCheckIn();
            DateTime departureTime = arrivalTime.AddDays(1);

            PackageInfoEntity pinfoEntity = new PackageAdapter().GetHotelPackageByCode(hotelId, code, pid);
            if (pinfoEntity == null)
            {
                return null;
            }

            List<PackageInfoEntity> packagePrice = new List<PackageInfoEntity>();
            packagePrice.Add(pinfoEntity);
            SetDayLimit(hp, packagePrice);

            hp.OTAList = new List<OTAInfo2>();
            hp.HotelID = hotelId;
            hp.Name = HotelService.GetHotel(hotelId).Name;
            hp.CheckIn = arrivalTime;
            hp.CheckOut = departureTime;
            hp.Packages = packagePrice.OrderBy(p => p.SellState).ThenBy(p => p.PackageType).ToList();

            AddMorePackageInfo(hp.Packages);
            return hp;
        }

        static HJD.HotelServices.Contracts.PItemEntity pitem = new HJD.HotelServices.Contracts.PItemEntity()
        {
            Date = DateTime.Now,
            DateType = 0,
            Description = "订单支付成功后，还需工作人员与供应方再次确认。如无法确认，将全额退款至您的支付帐户中。",
            Type = 2,
            HotelID = 0,
            ID = 0,
            ItemCode = "notice",
            PID = 0,
            Price = 0
        };

        static HJD.HotelServices.Contracts.PItemEntity pitemIDCard = new HJD.HotelServices.Contracts.PItemEntity()
        {
            Date = DateTime.Now,
            DateType = 0,
            Description = "请带好个人有效身份证件办理入住",
            Type = 2,
            HotelID = 0,
            ID = -50,
            ItemCode = "有效身份证件",
            PID = 0,
            Price = 0
        };  
        
        static HJD.HotelServices.Contracts.PItemEntity pitemRelief = new HJD.HotelServices.Contracts.PItemEntity()
        {
            Date = DateTime.Now,
            DateType = 0,
            Description = "套餐内所包含活动及免费设施，如遇天气等原因会有所调整，具体请以入住当天酒店告知为准！",
            Type = 2,
            HotelID = 0,
            ID = -50,
            ItemCode = "免责",
            PID = 0,
            Price = 0
        };

        static HJD.HotelServices.Contracts.PItemEntity pitemPassportAttention = new HJD.HotelServices.Contracts.PItemEntity()
        {
            Date = DateTime.Now,
            DateType = 0,
            Description = "请注意护照有效期需距出游归来日至少6个月以上有效期，如因证件问题导致无法出行，概不负责。",
            Type = 2,
            HotelID = 0,
            ID = -51,
            ItemCode = "护照有效期",
            PID = 0,
            Price = 0
        };

        static HJD.HotelServices.Contracts.PItemEntity RoomCheckInTimeNotice = new HJD.HotelServices.Contracts.PItemEntity()
        {
            Date = DateTime.Now,
            DateType = 0,
            Description = "周末及节假日入住，由于酒店房态紧张，入住时间有可能会延至下午3点之后，具体情况视酒店房态而定。",
            Type = 2,
            HotelID = 0,
            ID = 0,
            ItemCode = "notice",
            PID = 0,
            Price = 0
        };

        static PRoomOptionsEntity defaultRoomOption = new PRoomOptionsEntity()
        {
            Date = DateTime.Now,
            DateType = 0,
            DefaultOption = "无烟",
            ID = 0,
            Options = "无烟,吸烟",
            PID = 0,
            RoomID = 0
        };

        public static void AddMorePackageInfo(List<PackageInfoEntity> ps)
        {
            //增加 注意事项增加如下内容： 订单支付成功后，还需与酒店确认房态。如无法确认，将全额退款至您的支付帐户中。 

            foreach (PackageInfoEntity p in ps)
            {
                if (p.packageBase.Code != "限时抢购")
                {
                    DealSystemItem(p, "ID:", pitemIDCard);
                    DealSystemItem(p, "CO:", pitem);
                    DealSystemItem(p, "RO:", RoomCheckInTimeNotice);

                    if (p.Items != null && p.CardTypeList != null && p.CardTypeList.Contains("2")) //"护照"  海外订单设置套餐时如勾选证件需要护照的话，套餐注意事项需多默认规则：护照有效期大于6个月等
                    {
                        p.Items.Add(pitemPassportAttention);
                    }

                    p.Items.Add(pitemRelief);  //添加免责条款

                    //if (p.CanUseCashCoupon > 0)
                    //{

                    //    p.Items.Add(new HJD.HotelServices.Contracts.PItemEntity()
                    //                {
                    //                    Date = DateTime.Now,
                    //                    DateType = 0,
                    //                    Description = "<font style=\"color:red;\" color=\"red\" >从即日起至2015/12/31止，注册登录新版周末酒店APP即可享有￥50元现金券，本套餐可抵用￥" + p.CanUseCashCoupon.ToString() + "元现金券。</font>",
                    //                    Type = 2,
                    //                    HotelID = 0,
                    //                    ID = -50,
                    //                    ItemCode = "notice",
                    //                    PID = 0,
                    //                    Price = 0
                    //                });
                    //}
                }
                if (p.Room.Options.Length == 0)
                {
                    p.Room.DefaultOption = "无烟";
                    p.Room.Options = "无烟,吸烟";
                }
            }
        }

        public static void DealSystemItem(PackageInfoEntity p, string tag, HJD.HotelServices.Contracts.PItemEntity item)
        {
            var tempItem = p.Items.Where(i => i.Description.StartsWith(tag));
            if (tempItem.Count() > 0)
            {
                HJD.HotelServices.Contracts.PItemEntity fitem = tempItem.First();
                fitem.Description = fitem.Description.Substring(tag.Length).Trim();
                if (fitem.Description.Length == 0)
                {
                    p.Items.Remove(fitem);
                }
            }
            else
            {
                p.Items.Add(item);
            }
        }
        /// <summary>
        /// 获取酒店所有套餐最大和最小日期限止
        /// </summary>
        /// <param name="hp"></param>
        /// <param name="packagePrice"></param>
        public static void SetDayLimit(HotelPrice2 hp, List<PackageInfoEntity> packagePrice)
        {
            if (packagePrice.Count > 0)
            {
                hp.DayLimitMin = packagePrice.Min(p => p.packageBase.DayLimitMin);
                if (packagePrice.Min(p => p.packageBase.DayLimitMax) > 0)
                {
                    hp.DayLimitMax = packagePrice.Max(p => p.packageBase.DayLimitMax);
                }
            }
        }

        /// <summary>
        /// 生成前端需要的房型信息数据
        /// </summary>
        /// <param name="hp"></param>
        /// <returns></returns>
        private static List<Models.RoomRate2> GenRoomRates(HotelPriceInfo2 hp)
        {
            List<RoomRate2> rr = new List<Models.RoomRate2>();

            if (hp != null && hp.RoomRates != null)
            {
                foreach (RoomRate rate in hp.RoomRates)
                {
                    RoomInfoEntity ri = GetRoomInfo(hp.HotelId, hp.ChannelID, rate.RoomID);
                    if (ri == null) continue;
                    RoomRate2 r = new RoomRate2();
                    r.BackCount = rate.BackAmount;
                    r.GuaranteeType = rate.GuaranteeCode;
                    r.Name = ri.RoomName;
                    r.Pics = ri.Pics.Split(',').ToList();
                    r.Price = rate.Price;
                    r.RateID = rate.RateID;
                    r.RoomID = rate.RoomID;
                    r.DetailDes = GenRoomDetailDes(ri, rate);
                    r.Others = new List<String> { ri.Others };
                    r.ShortDes = GenRoomShortDes(ri, rate);
                    rr.Add(r);
                }
            }

            return rr;
        }

        private static List<string> GenRoomDetailDes(RoomInfoEntity ri, RoomRate rate)
        {
            List<string> sd = new List<string>();

            sd.Add("面积:" + ri.Area);
            sd.Add("楼层:" + ri.Floor);
            sd.Add(string.Format("床型:{0}{1}", HotelHelper.GetBedTypeName(ri.BedType ?? 0), (ri.BedSize == 0 ? "" : "(" + ri.BedSize + "米)")));
            sd.Add(HotelHelper.GetBroadbandType(ri.Broadband ?? 0));
            sd.Add(string.Format("可住{0}人", ri.StandardOccupancy ?? 0));
            sd.Add(ri.CanAddBed ? "可加床" : "不可加床");

            return sd; ;
        }

        private static List<string> GenRoomShortDes(RoomInfoEntity ri, RoomRate rate)
        {
            // 早餐 床
            List<string> sd = new List<string>();

            sd.Add(HotelHelper.GetBreakfastType(rate.NumberOfBreakfast));

            sd.Add(HotelHelper.GetBedTypeName(ri.BedType ?? 0));

            sd.Add(HotelHelper.GetBroadbandType(ri.Broadband ?? 0));

            return sd;
        }

        private static RoomInfoEntity GetRoomInfo(int hotelid, short channelID, int roomID)
        {
            List<RoomInfoEntity> rl = GetRoomInfo(hotelid);
            return rl.Where(r => r.ChannelID == channelID && r.RoomID == roomID).FirstOrDefault();
        }

        private static List<RoomInfoEntity> GetRoomInfo(int hotelid)
        {
            return HotelService.GetRoomInfo(hotelid);
        }

        public static void GenPackageUpGradeInfo(List<PackageInfoEntity> plist, long userID, HJDAPI.Common.Helpers.Enums.CustomerType ctype)
        {
            if (plist.Count == 0 || ctype != HJDAPI.Common.Helpers.Enums.CustomerType.vip599) return;

            StringBuilder sb = new StringBuilder(); 

            int hotelid = plist.First().packageBase.HotelID;

            int UpgradeCount = GetVIP599UserUpgradeCount(userID);

            sb.AppendFormat("GenPackageUpGradeInfo   userID：{0} hotelid:{1} UpgradeCount:{2}", userID, hotelid, UpgradeCount);


            if (UpgradeCount < 2)
            {
                //一个订单只能升级1间夜。如果用户选择了多晚，那么不显示可以升级，如果订单提交页选择了多间，那么不显示可以升级。

                List<RoomInfoEntity> roomList = GetRoomInfo(hotelid);

                bool isWHPackage = true; // plist.First().
                if (isWHPackage)
                {
                    foreach (var groupList in plist.GroupBy(p => p.packageBase.SerialNO))
                    {
                        foreach (var p in groupList)
                        {
                            var list = groupList.Where(item => item.Price > p.Price && item.Price - p.Price < 200 && item.Room.RoomGrade > p.Room.RoomGrade);

                            if (list.Count() > 0)
                            {
                                var upgradePackage = list.OrderBy(l => l.Price).First();

                                sb.AppendFormat(" curPackage:{0}  upgrade:{1}", p.packageBase.Code, upgradePackage.packageBase.Code);
                              
                                p.UpGradePackageInfo = new UpGradePackageInfoEntity
                                {
                                    LastCancelTime = upgradePackage.LastCancelTime,
                                    CancelPolicy = upgradePackage.CancelPolicy,
                                    PID = upgradePackage.packageBase.ID,
                                    PriceGap = upgradePackage.Price - p.Price,
                                    OrderDescription = "免费升级至" + upgradePackage.Room.RoomCode,
                                    ListDescription = "可免费升级至" + upgradePackage.Room.RoomCode
                                };
                            }
                        }
                    }
                }
                else
                {
                    foreach (var groupList in plist.GroupBy(p => p.packageBase.Brief)) //早餐政策。。。TODO
                    {
                        foreach (var p in groupList)
                        {
                            var list = groupList.Where(item => item.Price > p.Price && item.Price - p.Price < 200 && item.Room.RoomGrade > p.Room.RoomGrade);

                            if (list.Count() > 0)
                            {
                                var upgradePackage = list.OrderBy(l => l.Price).First();
                                p.UpGradePackageInfo = new UpGradePackageInfoEntity
                                {
                                    LastCancelTime = upgradePackage.LastCancelTime,
                                    CancelPolicy = upgradePackage.CancelPolicy,
                                    PID = upgradePackage.packageBase.ID,
                                    PriceGap = upgradePackage.Price - p.Price,
                                    OrderDescription = "免费升级至" + upgradePackage.Room.RoomCode,
                                    ListDescription = "可免费升级至" + upgradePackage.Room.RoomCode
                                };
                            }
                        }
                    }
                }

            }


           // Log.WriteLog(sb.ToString());

        }


        public static int GetVIP599UserUpgradeCount(long userID)
        {
            int activityID = 100398;  //VIP599 活动ID

            int UpgradeCount = 0;

            var list = CouponAdapter.GetExchangeCouponEntityListByActivityIDAndUserID(activityID, userID).Where(l => l.State == (int)ExchangeCouponState.paied).OrderByDescending(l => l.ID);

            if (list.Count() > 0)
            {
                UpgradeCount = list.First().UpgradeCount;
            }

            return UpgradeCount;
        }


        #region 获取酒店参考价
        /// <summary>
        /// 获取基本价
        /// </summary>
        /// <param name="hotelIds"></param>
        /// <returns></returns>
        public static List<HotelBasePriceEntity> QueryHotelBasePrice(List<int> hotelIds)
        {
            return PriceService.QueryHotelBasePrice(hotelIds);
        }
        #endregion

        #region 补充发票注意事项

        public static HJD.HotelServices.Contracts.PItemEntity GenOnePItemEntity(DateTime checkIn, int hotelId, int pId)
        {
            return new HJD.HotelServices.Contracts.PItemEntity()
            {
                Date = checkIn,
                SourceType = 0,
                DateType = 0,
                Description = "此套餐无法开具增值税专用发票",
                HotelID = hotelId,
                ID = 0,
                ItemCode = "发票政策",
                NotVIPPrice = 0,
                PID = pId,
                Price = 0,
                Type = 2,
                VIPPrice = 0
            };
        }
        #endregion

        public static PackageOrderInfo GetOrderInfo(long orderId)
        {
            return PriceService.GetOrderInfo(orderId);
        }
    }
}