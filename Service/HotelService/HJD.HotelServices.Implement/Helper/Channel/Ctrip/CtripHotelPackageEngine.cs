using HJD.Framework.Interface;
using HJD.Framework.WCF;
using HJD.HotelServices.Contracts;
using HJD.HotelServices.Implement.Entity;
using HJD.OtaCrawlerService.Contract.Ctrip.Hotel;
using HJD.OtaCrawlerService.Contract.Params;
using HJD.OtaCrawlerServices.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HJD.HotelServices.Implement.Helper.Channel.Ctrip
{
    public class CtripHotelPackageEngine
    {
        private static IMemcacheProvider memCacheCtripRoomIdx = MemcacheManagerFactory.Create("CtripRoomIdxCache");
        public static string memCacheKeyCtripRoomId = "KeyCtripRoomId_{0}";
        public static string memCacheKeyCtripIdx = "KeyCtripIdx_{0}";

        static object lock1 = new object();

        #region 最新方式

        public static bool InsertPackageList(int hotelId, long hotelOriId, List<OTAPriceCtrip> otaPriceList)
        {
            //lock (lock1)
            {
                try
                {
                    //统计出所有日期，按照日期执行 删除 -> 插入
                    var roomIdList = otaPriceList.Select(p => p.RoomID).Distinct().ToList();

                    //按照日期进行删除/插入操作
                    for (int i = 0; i < roomIdList.Count; i++)
                    {
                        var roomId = roomIdList[i];
                        //var add = HotelDAL.InsertOtaPackageIdxCtrip(roomId);

                        //缓存roomid key
                        var _roomIdIdx = memCacheCtripRoomIdx.GetData<string>(string.Format(memCacheKeyCtripRoomId, roomId), () =>
                        {
                            var _idx = Convert.ToInt32(new Random().Next(0, 9999) + DateTime.Now.ToString("fffff")) + i;
                            return _idx.ToString();
                        });

                        //缓存idx key
                        var _roomId = memCacheCtripRoomIdx.GetData<string>(string.Format(memCacheKeyCtripIdx, _roomIdIdx), () =>
                        {
                            return roomId;
                        });
                    }

                    return true;
                }
                catch (Exception ex)
                {

                }
            }

            return false;
        }

        /// <summary>
        /// 根据携程RoomId（套餐ID）得到映射关系的Idx
        /// </summary>
        /// <param name="roomId"></param>
        /// <returns></returns>
        public static int GetPackageIdx(string roomId)
        {
            var _roomIdIdx = Convert.ToInt32(new Random().Next(0, 9999) + DateTime.Now.ToString("fffff"));

            try
            {
                _roomIdIdx = Convert.ToInt32(memCacheCtripRoomIdx.GetData<string>(string.Format(memCacheKeyCtripRoomId, roomId), () =>
                {
                    return new Random().Next(0, 9999) + DateTime.Now.ToString("fffff");
                }));
            }
            catch (Exception ex)
            {
                
            }

            return _roomIdIdx;

            //var pidx = HotelDAL.GetOtaPackageIdxCtrip(roomId);
            //return pidx;
        }

        /// <summary>
        /// 根据携程映射关系的Idx得到RoomId（套餐ID）
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static string GetOtaRoomIdCtripByIdx(int pid) 
        {
            var _roomId = "";

            try
            {
                _roomId = memCacheCtripRoomIdx.GetData<string>(string.Format(memCacheKeyCtripIdx, pid), () =>
                {
                    return "";
                });
            }
            catch (Exception ex)
            {

            }

            return _roomId;
        } 

        /// <summary>
        /// 查询指定酒店/指定入住&离店日期抓取的携程套餐信息(4.2及之后版本使用 2016-01-11 haoy)
        /// </summary>
        /// <param name="checkIn"></param>
        /// <param name="checkOut"></param>
        /// <param name="hotelOriId"></param>
        /// <param name="crawlTpe">0页面抓取 1对接api</param>
        /// <returns></returns>
        public static List<PackageInfoEntity> GetPackageListV42(CrawlerHotel crawlHotel, DateTime checkIn, DateTime checkOut, OTAPackageSourceConfig otaPackageConfig, int crawlTpe = 0)
        {
            var packageList = new List<PackageInfoEntity>();

            //入住天数
            var nightCount = (int)(checkOut - checkIn).TotalDays;

            //过滤抓取的XC套餐数据
            FilterSubRoomsV42(otaPackageConfig.SourceType, ref crawlHotel);

            if (crawlHotel != null && crawlHotel.HotelBaseRoomList != null)
            {
                var priceList = new List<OTAPriceCtrip>();

                #region 统计出每个房型中最优（按照固定策略）的一个套餐

                for (int roomNum = 0; roomNum < crawlHotel.HotelBaseRoomList.Count; roomNum++)
                {
                    var baseRoomEntity = crawlHotel.HotelBaseRoomList[roomNum];

                    foreach (var roomItem in baseRoomEntity.HotelRoomList)
                    {
                        if (roomItem.CanSell == 1)
                        {
                            var rateEntity = roomItem;

                            //ajax链接的方式抓取
                            //var sumPrice = Convert.ToInt32(rateEntity.Price) * nightCount;

                            //api的接口抓取(api抓取的方式，多天返回的金额是多天的总金额，所以不需要再乘以入住天数)
                            var sumPrice = Convert.ToInt32(rateEntity.Price);

                            //封装OTAPriceCtrip
                            var priceEntity = new OTAPriceCtrip
                            {
                                ID = rateEntity.ID,
                                Night = rateEntity.Night,
                                HotelID = otaPackageConfig.HotelId,
                                HotelOriId = otaPackageConfig.HotelOriId,
                                RoomTypeID = rateEntity.BaseRoomID,
                                RoomTypeName = baseRoomEntity.Name,
                                RoomID = rateEntity.RoomID,
                                RoomName = rateEntity.Name,

                                Price = Convert.ToInt32(rateEntity.Price),
                                SumPrice = sumPrice,
                                DetailPriceList = rateEntity.DetailPriceList,
                                CurrencyType = "",
                                Promotion = 0,
                                PromotionInfo = "",
                                ReturnCash = 0,
                                IsGift = rateEntity.IsGift,
                                GiftInfo = rateEntity.GiftTip,
                                BedType = (!string.IsNullOrEmpty(rateEntity.BedType) ? rateEntity.BedType : baseRoomEntity.BedType),    //baseRoomEntity.BedType, //
                                PackageBedType = rateEntity.BedType,
                                Area = baseRoomEntity.Area,
                                Floor = baseRoomEntity.Floor,
                                Breakfast = rateEntity.Breakfast,
                                Broadband = rateEntity.Broadband,

                                PayType = rateEntity.PayType,
                                Policy = rateEntity.Policy,
                                PolicyTip = rateEntity.PolicyTip,
                                PackageSummary = rateEntity.PackageSummary,
                                CreateTime = DateTime.Now
                            };
                            priceList.Add(priceEntity);
                        }
                    }
                }

                #endregion

                #region 根据抓取数据，生成ZM的套餐数据

                foreach (var otaPriceEntity in priceList)
                {
                    //多天套餐时，单天的明细价格必须对应入住天数
                    if (otaPriceEntity.DetailPriceList != null && otaPriceEntity.DetailPriceList.Count == nightCount)
                    {
                        //1:现付 2;担保 3:预付 4：待确认后支付（ 用户先提交订单，客服确认有房后，再付款）
                        var _payType = GetZmjdPayType(otaPriceEntity);

                        //不显示担保的套餐 2018.10.10 haoy
                        if (_payType != 2)
                        {
                            ////目前 如果是携程的套餐，今天入住的担保不显示
                            //if (checkIn.Date == DateTime.Now.Date && _payType == 2)
                            //{
                            //    continue;
                            //}

                            //将该房型下的所有套餐都加进来
                            PackageInfoEntity pi = GetPackageInfoByCtripRoom(Convert.ToInt32(otaPackageConfig.HotelId), otaPriceEntity, checkIn, nightCount);
                            pi.Price = otaPriceEntity.SumPrice;
                            pi.PackageType = crawlTpe == 1 ? (int)HotelServiceEnums.PackageType.CtripPackageByApi : (otaPackageConfig.SourceType != 2 ? (int)HotelServiceEnums.PackageType.CtripPackage : (int)HotelServiceEnums.PackageType.CtripPackageForHotel);
                            pi.PayType = _payType;
                            //pi.CustomerAskPrice.Text = "获取更低价";
                            var cancelPolicy = pi.LastCancelTime <= DateTime.Now ? 1 : 2;
                            //pi.CustomerAskPrice.ActionUrl = "http://www.zmjiudian.com/MagiCall/MagiCallClient?checkIn=" + checkIn + "&checkOut=" + checkOut + "&PCode=" + pi.packageBase.SerialNO + "&price=" + pi.Price + "&VIPPrice=" + pi.VIPPrice + "&hotelId=" + pi.packageBase.HotelID + "&payType=" + pi.PayType + "&cancelPolicy=" + cancelPolicy + "&userid={userid}&realuserid=1";
                            pi.NotShowVIPPrice = 1;

                            //pi.CanUseCoupon = _payType == 3; //携程预付套餐可用现金券

                            packageList.Add(pi);  
                        } 
                    }
                }

                #endregion
            }

            #region 分组排序

            Dictionary<string, int> orderByDic1 = new Dictionary<string, int>();
            packageList = packageList.OrderBy(p => p.Price).ToList();
            for (int pnum = 0; pnum < packageList.Count; pnum++)
            {
                if (!orderByDic1.ContainsKey(packageList[pnum].packageBase.Code))
                {
                    orderByDic1[packageList[pnum].packageBase.Code] = pnum;    
                }
            }

            //再排序，首先按照房型排序，再按照价格排序
            packageList = packageList.OrderBy(p => orderByDic1[p.packageBase.Code]).ThenBy(p => p.Price).ToList();

            #endregion

            return packageList;
        }

        /// <summary>
        /// 查询指定酒店/指定入住&离店日期抓取的携程套餐信息(4.2之前版本使用 2016-01-11 haoy)
        /// </summary>
        /// <param name="checkIn"></param>
        /// <param name="checkOut"></param>
        /// <param name="hotelOriId"></param>
        /// <returns></returns>
        public static List<PackageInfoEntity> GetPackageList(CrawlerHotel crawlHotel, DateTime checkIn, DateTime checkOut, OTAPackageSourceConfig otaPackageConfig)
        {
            var packageList = new List<PackageInfoEntity>();
            //var canSellRates = new List<OTAPriceCtrip>();
            //var roomMinPackageDic = new Dictionary<long, List<OTAPriceCtrip>>();

            //入住天数
            var nightCount = (int)(checkOut - checkIn).TotalDays;

            //过滤抓取的XC套餐数据
            FilterSubRooms(otaPackageConfig.SourceType, ref crawlHotel);

            if (crawlHotel != null && crawlHotel.HotelBaseRoomList != null)
            {
                var priceList = new List<OTAPriceCtrip>();

                #region 统计出每个房型中最优（按照固定策略）的一个套餐

                for (int roomNum = 0; roomNum < crawlHotel.HotelBaseRoomList.Count; roomNum++)
                {
                    var baseRoomEntity = crawlHotel.HotelBaseRoomList[roomNum];
                    var rateEntity = new CrawlerHotelRoom();

                    //取价格最低的策略过滤
                    var rateList = baseRoomEntity.HotelRoomList.OrderBy(r => r.Price).ToList();

                    //可售状态过滤
                    rateList = rateList.Where(r => r.CanSell == 1).ToList();

                    if (rateList != null && rateList.Count > 0)
                    {
                        //双早无早策略过滤
                        if (rateList.Exists(r => r.Breakfast != "无早" && r.Breakfast != "单早"))
                        {
                            rateEntity = rateList.Find(r => r.Breakfast != "无早" && r.Breakfast != "单早");
                        }
                        else
                        {
                            rateEntity = rateList.First();
                        }   

                        //ajax链接的方式抓取
                        //var sumPrice = Convert.ToInt32(rateEntity.Price) * nightCount;

                        //api的接口抓取(api抓取的方式，多天返回的金额是多天的总金额，所以不需要再乘以入住天数)
                        var sumPrice = Convert.ToInt32(rateEntity.Price);

                        //封装OTAPriceCtrip
                        var priceEntity = new OTAPriceCtrip
                        {
                            ID = rateEntity.ID,
                            Night = rateEntity.Night,
                            HotelID = otaPackageConfig.HotelId,
                            HotelOriId = otaPackageConfig.HotelOriId,
                            RoomTypeID = rateEntity.BaseRoomID,
                            RoomTypeName = baseRoomEntity.Name,
                            RoomID = rateEntity.RoomID,
                            RoomName = rateEntity.Name,

                            Price = Convert.ToInt32(rateEntity.Price),
                            SumPrice = sumPrice,
                            CurrencyType = "",
                            Promotion = 0,
                            PromotionInfo = "",
                            ReturnCash = 0,
                            IsGift = rateEntity.IsGift,
                            GiftInfo = rateEntity.GiftTip,
                            BedType = baseRoomEntity.BedType, //rateEntity.BedType,
                            Area = baseRoomEntity.Area,
                            Breakfast = rateEntity.Breakfast,
                            Broadband = rateEntity.Broadband,

                            PayType = rateEntity.PayType,
                            Policy = rateEntity.Policy,
                            PolicyTip = rateEntity.PolicyTip,
                            PackageSummary = rateEntity.PackageSummary,
                            CreateTime = DateTime.Now,
                        };
                        priceList.Add(priceEntity);   
                    }
                }

                #endregion

                #region 根据抓取数据，生成ZM的套餐数据

                foreach (var otaPriceEntity in priceList)
                {
                    //将该房型下的所有套餐都加进来
                    PackageInfoEntity pi = GetPackageInfoByCtripRoom(Convert.ToInt32(otaPackageConfig.HotelId), otaPriceEntity, checkIn, nightCount);
                    pi.Price = otaPriceEntity.SumPrice;
                    pi.PackageType = otaPackageConfig.SourceType != 2 ? (int)HotelServiceEnums.PackageType.CtripPackage : (int)HotelServiceEnums.PackageType.CtripPackageForHotel;
                    pi.PayType = 3;    // 3: 预付    4：待确认后支付（ 用户先提交订单，客服确认有房后，再付款）
                 //   pi.CustomerAskPrice.Text = "获取更低价";
                    //var cancelPolicy = pi.LastCancelTime <= DateTime.Now ? 1 : 2;
                  //  pi.CustomerAskPrice.ActionUrl = "http://www.zmjiudian.com/MagiCall/MagiCallClient?checkIn=" + checkIn + "&checkOut=" + checkOut + "&PCode=" + pi.packageBase.SerialNO + "&price=" + pi.Price + "&VIPPrice=" + pi.VIPPrice + "&hotelId=" + pi.packageBase.HotelID + "&payType=" + pi.PayType + "&cancelPolicy=" + cancelPolicy + "&userid={userid}&realuserid=1";
                    pi.NotShowVIPPrice = 1;
                    
                   // pi.CanUseCoupon = true; //携程套餐 默认都可以使用立减券  20171101

                    packageList.Add(pi);
                }

                #endregion   
            }

            //排序
            packageList = packageList.OrderBy(p => p.Price).ToList();

            return packageList;
        }

        /// <summary>
        /// 根据抓取酒店实例解析出携程套餐集合
        /// </summary>
        /// <param name="cHotelList"></param>
        /// <returns></returns>
        public static List<OTAPriceCtrip> GetOtaCtripPriceByCrawlHotel(int hotelId, long hotelOriId, CrawlerHotel cHotel)
        {
            var priceList = new List<OTAPriceCtrip>();

            if (cHotel != null && cHotel.HasRoom && cHotel.HotelBaseRoomList != null && cHotel.HotelBaseRoomList.Count > 0)
            {
                for (int i = 0; i < cHotel.HotelBaseRoomList.Count; i++)
                {
                    var baseRoom = cHotel.HotelBaseRoomList[i];
                    var priceRates = baseRoom.HotelRoomList;
                    for (int pNum = 0; pNum < priceRates.Count; pNum++)
                    {
                        var prate = priceRates[pNum];
                        var priceEntity = new OTAPriceCtrip
                        {
                            ID = prate.ID,
                            Night = prate.Night,
                            HotelID = hotelId,
                            HotelOriId = hotelOriId,
                            RoomTypeID = prate.BaseRoomID,
                            RoomTypeName = baseRoom.Name,
                            RoomID = prate.RoomID,
                            RoomName = prate.Name,

                            Price = Convert.ToInt32(prate.Price),
                            SumPrice = Convert.ToInt32(prate.Price),
                            CurrencyType = "",
                            Promotion = 0,
                            PromotionInfo = "",
                            ReturnCash = 0,
                            IsGift = prate.IsGift,
                            GiftInfo = prate.GiftTip,
                            BedType = baseRoom.BedType, //prate.BedType,
                            Area = baseRoom.Area,
                            Breakfast = prate.Breakfast,
                            Broadband = prate.Broadband,

                            PayType = prate.PayType,
                            Policy = prate.Policy,
                            PolicyTip = prate.PolicyTip,
                            PackageSummary = prate.PackageSummary,
                            CreateTime = DateTime.Now
                        };

                        #region 特殊处理

                        //如果是不是预付，并且存在返现，则显示返现前的价格 2016-03-16 haoy
                        var soldPrice = 0;
                        var discount = 0;
                        try
                        {
                            if (!prate.PayType.Contains("预付") && !string.IsNullOrEmpty(prate.SoldPrice) && !string.IsNullOrEmpty(prate.Discount))
                            {
                                soldPrice = Convert.ToInt32(prate.SoldPrice);
                                discount = Convert.ToInt32(prate.Discount);
                                if (soldPrice > 0 && discount > 0)
                                {
                                    priceEntity.Price = soldPrice;
                                    priceEntity.SumPrice = soldPrice;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            //File.AppendAllText(string.Format(@"D:\Log\HotelService\LoadCtripHotelPackagesV42_{0}_Log.txt", DateTime.Now.ToString("yyyyMMdd")), string.Format("{0}-{1} \r\n", "Err:" + ex.Message + "\r\n" + ex.StackTrace, DateTime.Now));
                        }

                        #endregion

                        priceList.Add(priceEntity);
                    }
                }
            }

            return priceList;
        }

        /// <summary>
        /// 对抓取的酒店套餐数据进行具体规则的过滤(4.2及之后版本使用)
        /// </summary>
        /// <param name="priceChannelId"></param>
        /// <param name="subRooms"></param>
        /// <returns></returns>
        public static void FilterSubRoomsV42(int priceChannelId, ref CrawlerHotel crawlHotel)
        {
            var roomList = new List<CrawlerHotelBaseRoom>();

            if (crawlHotel != null && crawlHotel.HotelBaseRoomList != null)
            {
                roomList = crawlHotel.HotelBaseRoomList;

                //过滤包含“间以上”、“钟点房”、“单人入住”
                roomList = roomList.Where(h => !h.Name.Contains("起订") && !h.Name.Contains("间以上") && !h.Name.Contains("间及以上") && !h.Name.Contains("钟点房") && !h.Name.Contains("单人入住") && !h.Name.Contains("准考证入住") && !h.Name.Contains("澳门居民身份证") && !h.Name.Contains("只限持有") && !h.Name.Contains("此价格仅限持有") && !h.Name.Contains("不含住宿") && !h.Name.Contains("外宾")).ToList();

                for (int i = 0; i < roomList.Count; i++)
                {
                    //!h.PayType.Contains("担保")  开放担保套餐 2016.10.26 haoy
                    roomList[i].HotelRoomList = roomList[i].HotelRoomList.Where(h => !h.Name.Contains("团购券") && !h.Name.Contains("起订") && !h.Name.Contains("间以上") && !h.Name.Contains("间及以上") && !h.Name.Contains("钟点房") && !h.Name.Contains("单人入住") && !h.Name.Contains("准考证入住") && !h.Name.Contains("澳门居民身份证") && !h.Name.Contains("只限持有") && !h.Name.Contains("此价格仅限持有") && !h.Name.Contains("不含住宿") && !h.Name.Contains("外宾")).ToList() ?? new List<CrawlerHotelRoom>();
                }

                crawlHotel.HotelBaseRoomList = roomList;
            }
        }

        /// <summary>
        /// 对抓取的酒店套餐数据进行具体规则的过滤(4.2之前版本使用)
        /// </summary>
        public static void FilterSubRooms(int priceChannelId, ref CrawlerHotel crawlHotel)
        {
            var roomList = new List<CrawlerHotelBaseRoom>();

            if (crawlHotel != null && crawlHotel.HotelBaseRoomList != null)
            {
                roomList = crawlHotel.HotelBaseRoomList;

                if (priceChannelId != 2)
                {
                    roomList = roomList.Where(rate => rate.HotelRoomList != null && rate.HotelRoomList.Exists(r => r.PayType == "预付")).ToList();   
                }

                //过滤包含“间以上”、“钟点房”、“单人入住”
                roomList = roomList.Where(h => !h.Name.Contains("起订") && !h.Name.Contains("间以上") && !h.Name.Contains("间及以上") && !h.Name.Contains("钟点房") && !h.Name.Contains("单人入住") && !h.Name.Contains("准考证入住") && !h.Name.Contains("澳门居民身份证") && !h.Name.Contains("只限持有") && !h.Name.Contains("此价格仅限持有") && !h.Name.Contains("不含住宿") && !h.Name.Contains("外宾")).ToList();

                for (int i = 0; i < roomList.Count; i++)
                {
                    //!h.PayType.Contains("担保")  开放担保套餐 2016.10.26 haoy
                    roomList[i].HotelRoomList = roomList[i].HotelRoomList.Where(h => !h.Name.Contains("团购券") && !h.Name.Contains("起订") && !h.Name.Contains("间以上") && !h.Name.Contains("间及以上") && !h.Name.Contains("钟点房") && !h.Name.Contains("单人入住") && !h.Name.Contains("准考证入住") && !h.Name.Contains("澳门居民身份证") && !h.Name.Contains("只限持有") && !h.Name.Contains("此价格仅限持有") && !h.Name.Contains("不含住宿") && !h.Name.Contains("外宾")).ToList() ?? new List<CrawlerHotelRoom>();
                }

                crawlHotel.HotelBaseRoomList = roomList;
            }
        }

        #endregion

        #region zmjiudian 套餐生成

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseRoom"></param>
        /// <param name="pricerate"></param>
        /// <returns></returns>
        private static PackageInfoEntity GetPackageInfoByCtripRoom(int hotelid, OTAPriceCtrip pricerate, DateTime night, int nightCount)
        {
            PackageInfoEntity pi = new PackageInfoEntity();
            pi.Items = new List<PItemEntity>();
            pi.Rates = new List<PRateEntity>();
            pi.SellState = (int)PackageSellSate.canSell;
            List<PackageDailyEntity> pdes = new List<PackageDailyEntity>();

            if (!string.IsNullOrEmpty(pricerate.Breakfast))
            {
                pricerate.Breakfast = pricerate.Breakfast.Replace("0早", "无早");   
            }

            int soldCount = Convert.ToInt32(pricerate.RoomTypeID) % 10;

            //根据携程套餐ID生成关联的IDX
            int pid = GetPackageIdx(pricerate.RoomID);

            PackageEntity p = new PackageEntity()
            {
                CanInvoice = true,
                Code = pricerate.RoomTypeName,
                Brief = string.Format("{0} {1}", pricerate.RoomTypeName, pricerate.Breakfast),
                DayLimitMax = 0,
                DayLimitMin = 0,
                EndDate = DateTime.Now.AddYears(1),
                HotelID = hotelid,
                ID = pid,
                IsValid = true,
                PackageCount = 5,
                RoomID = Convert.ToInt32(pricerate.RoomTypeID),
                SoldCountSum = soldCount,
                StartDate = night,
                StartNum = 0,
                TimeAdvance = 0,
                SerialNO = pricerate.RoomTypeName + " " + pricerate.PackageBedType + " " + pricerate.Breakfast
            };

            var roomBedType = GetCtripRoomBedType(pricerate.BedType);
            PRoomInfoEntity roomInfo = new PRoomInfoEntity()
            {
                DefaultOption = "",
                Description = string.Format("{0}{1}{2}{3}",
                pricerate.RoomTypeName,
                string.IsNullOrEmpty(roomBedType) ? "" : (" " + roomBedType),
                string.IsNullOrEmpty(pricerate.Floor) ? "" : (" " + pricerate.Floor),
                string.IsNullOrEmpty(pricerate.Area) ? "" : (" " + pricerate.Area.Replace("建筑面积：", ""))),
                FestivalPrice = 1000,
                HotelID = hotelid,
                ID = Convert.ToInt32(pricerate.RoomTypeID),
                Options = "",
                PID = pid,
                RelOTAID = 0,
                RelOTARoomID = 0,
                RoomCode = pricerate.RoomTypeName,
                WeekDayPrice = 0,
                WeekendDayPrice = 0
            };

            //set package deily items
            SetCtripPackageDailyItem(hotelid, pricerate, pi, pdes);

            //set room options
            SetCtripPackageRoomOptions(pricerate, roomBedType, pi, pid, roomInfo);

            //set package items
            SetCtrpPackageItems(hotelid, pricerate, pi, night);

            pi.packageBase = p;
            pi.Room = roomInfo;
            pi.DailyItems = pdes;

            //套餐金额要乘以入住天数
            pi.Price = pi.Price * nightCount;

            return pi;
        }

        /// <summary>
        /// 将携程的床型命名转换为我们的床型显示规范
        /// </summary>
        /// <param name="bedType"></param>
        /// <returns></returns>
        private static string GetCtripRoomBedType(string bedType)
        {
            //床型：双人床1.5米，1张或单人床1.1米，2张
            //大床1.5米/双床1.1米

            //床型信息：1张双人床(1.5m)/2张单人床(1.1m)

            bedType = bedType.Replace("床型信息：", "");
            bedType = bedType.Replace("床型：", "");
            bedType = bedType.Replace("或", "/").Replace("和", "/");
            bedType = bedType.Replace("双人", "大").Replace("单人", "双");

            var newBeds = bedType.Split('/');
            var bedTypeStr = "";
            foreach (var item in newBeds)
            {
                if (!string.IsNullOrEmpty(item.Trim()))
                {
                    if (!string.IsNullOrEmpty(bedTypeStr)) bedTypeStr += "/";

                    var bed = item.Split('，')[0];
                    if (bed.Contains("张"))
                    {
                        bed = bed.Split('张')[1];
                        bed = bed.Replace("cm", "厘米").Replace("m", "米");
                    }
                    bedTypeStr += bed;   
                }
            }

            return bedTypeStr;
        }

        private static void SetCtripPackageRoomOptions(OTAPriceCtrip pricerate, string bedType, PackageInfoEntity pi, int pid, PRoomInfoEntity roomInfo)
        {
            List<PRoomOptionsEntity> RoomOptions = new List<PRoomOptionsEntity>();

            if (!string.IsNullOrEmpty(bedType))
            {

                string options = bedType.Replace("/", ",");
                
                string DefaultOption = "";
                var bedTypeList = bedType.Split('/');
                DefaultOption = bedTypeList.Count() > 1 ? (bedTypeList[0].Contains("双") ? bedTypeList[0] : bedTypeList[1]) : bedTypeList[0];

                //携程套餐，大双床前面追加 尽量 20170324 haoy
                options = options.Replace("双床", "尽量双床").Replace("大床", "尽量大床");
                DefaultOption = DefaultOption.Replace("双床", "尽量双床").Replace("大床", "尽量大床");

                roomInfo.Options = options;
                roomInfo.DefaultOption = DefaultOption;


                RoomOptions.Add(new PRoomOptionsEntity()
                {
                    Options = options,
                    Date = DateTime.Now,
                    DateType = 0,
                    DefaultOption = DefaultOption,
                    ID = 0,
                    PID = pid,
                    RoomID = Convert.ToInt32(pricerate.RoomTypeID)
                });

                pi.RoomOptions = RoomOptions;
            }
        }

        private static void SetCtripPackageDailyItem(int hotelid, OTAPriceCtrip pricerate, PackageInfoEntity pi, List<PackageDailyEntity> pdes)
        {
            var priceList = new List<decimal> { pricerate.Price };
            if (pricerate.DetailPriceList != null && pricerate.DetailPriceList.Count > 0)
            {
                priceList = pricerate.DetailPriceList;
            }

            for (int priceNum = 0; priceNum < priceList.Count; priceNum++)
            {
                var priceValue = Convert.ToInt32(priceList[priceNum]);

                PackageDailyEntity pde = new PackageDailyEntity();
                pde.Day = pricerate.Night.Date;
                pde.Price = priceValue;
                pde.PayType = GetZmjdPayType(pricerate); //3;   //3.	预付  4.	待确认后支付（ 用户先提交订单，客服确认有房后，再付款）
                pde.Rebate = 0;
                pde.ActiveRebate = 0;
                pde.CashCoupon = 0;
                pde.CanUseCashCoupon = 0;
                pde.Items = new List<PItemEntity>();

                #region 早餐

                if (!string.IsNullOrEmpty(pricerate.Breakfast))
                {
                    PItemEntity Item = new PItemEntity
                    {
                        Date = DateTime.Parse("1900-1-1"),
                        DateType = 0,
                        Description = pricerate.Breakfast,
                        HotelID = hotelid,
                        ID = hotelid,
                        ItemCode = "早餐",
                        PID = 0,
                        Price = 0,
                        SourceType = 0,
                        Type = 1
                    };
                    pde.Items.Add(Item);
                    if (priceNum == 0) pi.Items.Add(Item);
                }

                #endregion

                #region 礼

                if (pricerate.IsGift == 1 && !string.IsNullOrEmpty(pricerate.GiftInfo))
                {
                    PItemEntity Item = new PItemEntity
                    {
                        Date = DateTime.Parse("1900-1-1"),
                        DateType = 0,
                        Description = pricerate.GiftInfo.Replace("携程", ""),
                        HotelID = hotelid,
                        ID = hotelid,
                        ItemCode = "礼",
                        PID = 0,
                        Price = 0,
                        SourceType = 0,
                        Type = 1
                    };
                    pde.Items.Add(Item);
                    if (priceNum == 0) pi.Items.Add(Item);
                }

                #endregion

                #region 套餐信息

                if (!string.IsNullOrEmpty(pricerate.PackageSummary))
                {
                    var summaryList = Regex.Split(pricerate.PackageSummary.Replace("●", "").Replace("携程", ""), "\r\n");
                    if (summaryList.Length > 1)
                    {
                        for (int i = 1; i < summaryList.Length; i++)
                        {
                            var summary = summaryList[i].Trim();
                            PItemEntity Item = new PItemEntity
                            {
                                Date = DateTime.Parse("1900-1-1"),
                                DateType = 0,
                                Description = summary,
                                HotelID = hotelid,
                                ID = hotelid,
                                ItemCode = "套餐说明",
                                PID = 0,
                                Price = 0,
                                SourceType = 0,
                                Type = 1
                            };
                            pde.Items.Add(Item);
                            if (priceNum == 0) pi.Items.Add(Item);
                        }
                    }
                }

                #endregion

                pi.Price += (int)pde.Price;
                pi.Rebate += pde.Rebate;
                pi.ActiveRebate += pde.ActiveRebate;
                pi.CashCoupon += pde.CashCoupon;
                pi.CanUseCashCoupon += pde.CanUseCashCoupon;

                pdes.Add(pde);
            }
        }

        private static void SetCtrpPackageItems(int hotelid, OTAPriceCtrip pricerate, PackageInfoEntity pi, DateTime night)
        {
            var payType = GetZmjdPayType(pricerate);

            var lastCancelTime = night;


            if (pricerate.Policy == null) { pricerate.Policy = ""; }
            if (pricerate.PolicyTip == null) { pricerate.PolicyTip = ""; }

            //取消政策
            var policyTip = "";
            switch (pricerate.Policy)
            {
                case "不可取消":
                case "不可取消立即确认":
                    { 
                        policyTip = "预订后不能取消或更改。"; 
                        lastCancelTime = DateTime.Now.Date.AddDays(-1); 
                        break; 
                    }
                case "限时取消":
                case "限时取消立即确认":
                    {
                        policyTip = pricerate.PolicyTip.Replace("携程", "工作人员");

                        //截取携程取消政策中的最晚取消时间
                        //例：如需取消、修改订单，请在2015-06-18 12:00前通知工作人员，否则我们将扣除您全额或部分房费。
                        try
                        {
                            //var policyTip1 = policyTip.Substring(policyTip.IndexOf("请在") + 2);
                            //var policyTip2 = policyTip1.Substring(0, policyTip1.IndexOf("前"));
                            //lastCancelTime = DateTime.Parse(policyTip2);

                            Regex r = new Regex("[0-9]{2,4}(-|/|年)[0-9]{1,2}(-|/|月)[0-9]{1,2} [0-9]{1,2}:[0-9]{1,2}");
                            var m = r.Match(policyTip);
                            if (!string.IsNullOrEmpty(m.Value))
                            {
                                lastCancelTime = DateTime.Parse(m.Value);
                            }
                        }
                        catch (Exception) { }

                        break;
                    }
                case "免费取消":
                case "免费取消立即确认":
                    { 
                        policyTip = string.Format("如需取消、修改订单，请在{0}前通知工作人员，否则我们将扣除您全额或部分房费。", lastCancelTime.ToString("yyyy-MM-dd 12:00"));
                        lastCancelTime = DateTime.Parse(lastCancelTime.ToString("yyyy-MM-dd 12:00"));
                        break; 
                    }
                default: 
                    { 
                        policyTip = pricerate.PolicyTip.Replace("携程", "");

                        //默认不能取消
                        if (string.IsNullOrEmpty(policyTip))
                        {
                            policyTip = "订单一经提交，不可取消、修改。若未按时入住，我们将扣除您全额或部分房费。";
                        }
                        lastCancelTime = DateTime.Now.Date.AddDays(-1);
                        break; 
                    }
            }

            //担保的套餐暂时显示 预付不可取消 政策
            if (payType == 2) { policyTip = "预订后不能取消或更改。"; lastCancelTime = DateTime.Now.Date.AddDays(-1); }

            pi.LastCancelTime = lastCancelTime;

            PItemEntity Item = new PItemEntity
            {
                Date = DateTime.Parse("1900-1-1"),
                DateType = 0,
                Description = policyTip,
                HotelID = hotelid,
                ID = hotelid,
                ItemCode = "取消政策",
                PID = 0,
                Price = 0,
                SourceType = 0,
                Type = 2
            };
            pi.Items.Add(Item);
        }

        /// <summary>
        /// 根据OTA的支付方式，转换为ZMJD的支付方式
        /// </summary>
        /// <param name="pricerate"></param>
        /// <returns></returns>
        public static int GetZmjdPayType(OTAPriceCtrip pricerate)
        {
            return GetZmjdPayType(pricerate.PayType);
        }

        public static int GetZmjdPayType(string  otaPayType)
        {
            var paytype = 3;

             otaPayType = otaPayType.ToLower().Trim();

            if (otaPayType.Contains("现付"))
            {
                paytype = 1;
            }
            else if (otaPayType.Contains("担保"))
            {
                paytype = 2;
            }
            else if (otaPayType.Contains("预付"))
            {
                paytype = 3;
            }

            return paytype;
        }

        #endregion

        #region Log 记录

        public static void WLog(string msg)
        {
            string logFile = @"D:\Log\HotelService\CtripHotelPackageEngine_" + DateTime.Now.ToString("_MMdd") + ".txt";
            File.AppendAllText(logFile, string.Format("{0}  {1} \r\n\r\n", msg, DateTime.Now));
        }

        #endregion

        #region 原始方式（暂注释）

        ///// <summary>
        ///// 查询指定酒店/指定入住&离店日期抓取的携程套餐信息
        ///// </summary>
        ///// <param name="checkIn"></param>
        ///// <param name="checkOut"></param>
        ///// <param name="hotelOriId"></param>
        ///// <returns></returns>
        //public static List<PackageInfoEntity> GetPackageList2(DateTime checkIn, DateTime checkOut, int hotelOriId, int hotelid)
        //{
        //    var packageList = new List<PackageInfoEntity>();
        //    var canSellRates = new List<CrawlerHotelRoomEx>();
        //    var roomMinPackageDic = new Dictionary<long, List<CrawlerHotelRoomEx>>();

        //    //查询出携程抓取的酒店套餐
        //    var hotelRates = CtripDAL.GetPriceRateByHotel((long)hotelOriId, checkIn, checkOut);

        //    //入住天数
        //    var nightCount = (int)(checkOut - checkIn).TotalDays;

        //    //得到当前酒店的PriceChannelID（2 表示忽略携程的可预付条件    3 表示需追加套餐可预付的条件）
        //    string priceChannelId = HotelDAL.GetPriceChannelIdByHotelId((long)hotelid);

        //    //过滤抓取的XC套餐数据
        //    FilterHotelRates(priceChannelId, ref hotelRates);

        //    if (hotelRates != null && hotelRates.Count > 0)
        //    {
        //        var matchRates = new List<CrawlerHotelRoomEx>();
        //        matchRates.AddRange(hotelRates);

        //        for (int i = 0; i < hotelRates.Count; i++)
        //        {
        //            var rate = hotelRates[i];

        //            //（有"套餐信息"的使用，我们暂时先不使用携程的套餐数据）
        //            var whereRates = matchRates.Where(r => r.BaseRoomID == rate.BaseRoomID && r.RoomID == rate.RoomID && string.IsNullOrEmpty(rate.PackageSummary)).ToList();

        //            //将满足入住天数的房型/套餐保留下来
        //            if (whereRates != null && whereRates.Count > 0 && whereRates.Count == nightCount
        //                && !canSellRates.Exists(cr => cr.BaseRoomID == rate.BaseRoomID && cr.RoomID == rate.RoomID))
        //            {
        //                //使用最便宜的一个套餐信息
        //                var sellRate = whereRates.OrderBy(wr => wr.Price).First();

        //                //套餐价格则是所有套餐的价格累加（可能每天的价格都是不一样的）
        //                sellRate.SumPrice = whereRates.Sum(wr => (int)wr.Price);

        //                if (!roomMinPackageDic.ContainsKey(sellRate.BaseRoomID)) roomMinPackageDic[sellRate.BaseRoomID] = new List<CrawlerHotelRoomEx>();
        //                roomMinPackageDic[sellRate.BaseRoomID].Add(sellRate);

        //                //标记已经遍历过的套餐
        //                canSellRates.Add(sellRate);
        //            }
        //        }

        //        #region 根据抓取数据，生成ZM的套餐数据

        //        foreach (var sellRates in roomMinPackageDic.Values)
        //        {
        //            CrawlerHotelRoomEx sellRate = new CrawlerHotelRoomEx();

        //            //如果存在双早的套餐，则找出双早中最便宜的，否则不追加该条件
        //            if (sellRates.Exists(s => s.Breakfast != "无早" && s.Breakfast != "单早"))
        //            {
        //                sellRate = sellRates.Where(s => s.Breakfast != "无早" && s.Breakfast != "单早").OrderBy(s => s.SumPrice).First();
        //            }
        //            else
        //            {
        //                sellRate = sellRates.OrderBy(s => s.SumPrice).First();
        //            }

        //            //将该房型下的所有套餐都加进来
        //            PackageInfoEntity pi = GetPackageInfoByCtripRoom(hotelid, sellRate, checkIn, nightCount);
        //            pi.Price = sellRate.SumPrice;
        //            pi.PackageType = priceChannelId != "2" ? (int)HotelServiceEnums.PackageType.CtripPackage : (int)HotelServiceEnums.PackageType.CtripPackageForHotel;
        //            pi.PayType = 3;    // 3: 预付    4：待确认后支付（ 用户先提交订单，客服确认有房后，再付款）
        //            packageList.Add(pi);
        //        }

        //        #endregion
        //    }

        //    //排序
        //    packageList = packageList.OrderBy(p => p.Price).ToList();

        //    return packageList;

        //    #region 注释
        //    //List<CrawlerHotelBaseRoom> rooms = CtripDAL.GetCtripHotelRoom((long)hotelOriId);

        //    ////遍历当前酒店所有抓取到的房型(目前只拿当前房型中价格最低的一个双早的项目到套餐)
        //    //for (int rmNum = 0; rmNum < rooms.Count; rmNum++)
        //    //{
        //    //    var baseRoom = rooms[rmNum];

        //    //    //按照入住和离店日期，获取每一天的套餐信息
        //    //    int day = 0;
        //    //    while (checkIn.AddDays(day) < checkOut)
        //    //    {
        //    //        //入住日
        //    //        var night = checkIn.AddDays(day);

        //    //        //获取当前房型下的所有项目
        //    //        var rates = CtripDAL.GetCtripHotelRoomPrice(baseRoom.BaseRoomID, night);

        //    //        //排除无早和单早
        //    //        var priceratelist = rates; //.Where(r => !r.Breakfast.Contains("无早") && !r.Breakfast.Contains("单早") && r.CanSell == 1 && r.PayType == "预付").ToList();
        //    //        if (priceratelist != null && priceratelist.Count > 0)
        //    //        {
        //    //            //找出其中价格最低的一个
        //    //            var pricerate = priceratelist.OrderBy(r => r.Price).ToList()[0];
        //    //            if (pricerate != null)
        //    //            {
        //    //                PackageInfoEntity pi = GetPackageInfoByCtripRoom(baseRoom, pricerate, checkIn, nightCount);
        //    //                PIS.Add(pi);
        //    //            }
        //    //        }

        //    //        day++;
        //    //    }
        //    //}
        //    #endregion

        //    #region 直接解析爬取对象（注释）

        //    ////携程酒店爬取对象
        //    //var hotel = new CrawlerHotel();

        //    //{
        //    //    writeLog("start 直接解析爬取对象");

        //    //    //便利每一个房型
        //    //    foreach (var baseRoom in hotel.HotelBaseRoomList)
        //    //    {
        //    //        //然后找出这个房型下面价格最便宜的一个 非无早&非单早&预付&可卖 的一个套餐
        //    //        if (baseRoom.HotelRoomList != null && baseRoom.HotelRoomList.Count > 0)
        //    //        {
        //    //            var priceratelist = baseRoom.HotelRoomList.Where(r => r.Breakfast != "无早" && r.Breakfast != "单早" && r.PayType == "预付" && r.CanSell == 1).ToList();
        //    //            if (priceratelist == null || priceratelist.Count <= 0) continue;
        //    //            var pricerate = priceratelist.OrderBy(r => r.Price).ToList()[0];

        //    //            PackageInfoEntity pi = GetPackageInfoByCtripRoom(baseRoom, pricerate, nightCount);
        //    //            PIS.Add(pi);
        //    //        }
        //    //    }

        //    //    writeLog("end 直接解析爬取对象");
        //    //}
        //    #endregion
        //}

        ///// <summary>
        ///// 对抓取的酒店套餐数据进行具体规则的过滤
        ///// </summary>
        ///// <param name="priceChannelId"></param>
        ///// <param name="hotelRates"></param>
        ///// <returns></returns>
        //private static void FilterHotelRates(string priceChannelId, ref List<CrawlerHotelRoomEx> hotelRates)
        //{
        //    //过滤过期数据
        //    hotelRates = hotelRates.Where(h => h.TimeStamp > DateTime.Now.Date).ToList();

        //    if (hotelRates != null && priceChannelId != "2")
        //    {
        //        hotelRates = hotelRates.Where(h => h.PayType == "预付").ToList();
        //    }

        //    //过滤掉包含“间以上”的套餐信息
        //    hotelRates = hotelRates.Where(h => !h.Name2.Contains("间以上")).ToList();
        //}

        ///// <summary>
        ///// 根据抓取酒店实例解析出套餐集合
        ///// </summary>
        ///// <param name="cHotel"></param>
        ///// <returns></returns>
        //private static List<CrawlerHotelRoomEx> GetHotelRateExListByCrawlHotel(List<CrawlerHotel> cHotelList)
        //{
        //    var hotelRates = new List<CrawlerHotelRoomEx>();

        //    for (int listNum = 0; listNum < cHotelList.Count; listNum++)
        //    {
        //        var cHotel = cHotelList[listNum];
        //        if (cHotel != null && cHotel.HasRoom && cHotel.HotelBaseRoomList != null && cHotel.HotelBaseRoomList.Count > 0)
        //        {
        //            for (int i = 0; i < cHotel.HotelBaseRoomList.Count; i++)
        //            {
        //                var baseRoom = cHotel.HotelBaseRoomList[i];
        //                var priceRates = baseRoom.HotelRoomList;
        //                for (int pNum = 0; pNum < priceRates.Count; pNum++)
        //                {
        //                    var prate = priceRates[pNum];
        //                    var hrate = new CrawlerHotelRoomEx
        //                    {
        //                        ID = prate.ID,
        //                        BaseRoomID = prate.BaseRoomID,
        //                        BedType = prate.BedType,
        //                        Breakfast = prate.Breakfast,
        //                        Broadband = prate.Broadband,
        //                        CanSell = prate.CanSell,
        //                        DelPrice = prate.DelPrice,
        //                        Discount = prate.Discount,
        //                        GiftTip = prate.GiftTip,
        //                        IsGift = prate.IsGift,
        //                        Name = prate.Name,
        //                        Night = prate.Night,
        //                        PackageSummary = prate.PackageSummary,
        //                        PayType = prate.PayType,
        //                        Policy = prate.Policy,
        //                        PolicyTip = prate.PolicyTip,
        //                        Price = prate.Price,
        //                        RoomID = prate.RoomID,
        //                        SoldPrice = prate.SoldPrice,
        //                        Stamp = prate.Stamp,
        //                        TimeStamp = DateTime.Now,

        //                        HotelID = baseRoom.HotelID,
        //                        Name2 = baseRoom.Name,
        //                        BedType2 = baseRoom.BedType,
        //                        Area = baseRoom.Area,
        //                        SumPrice = Convert.ToInt32(prate.Price)
        //                    };
        //                    hotelRates.Add(hrate);
        //                }
        //            }   
        //        }
        //    }

        //    hotelRates = hotelRates.OrderBy(hr => hr.BaseRoomID.ToString() + hr.RoomID).ToList();

        //    return hotelRates;
        //}

        #endregion
    }
}
