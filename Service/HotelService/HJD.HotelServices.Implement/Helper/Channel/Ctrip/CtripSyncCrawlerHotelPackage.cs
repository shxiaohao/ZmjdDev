//using HJD.Framework.Interface;
//using HJD.Framework.WCF;
//using HJD.HotelServices.Contracts;
//using HJD.OtaCrawlerService.Contract.Ctrip.Hotel;
//using HJD.OtaCrawlerService.Contract.Params;
//using HJD.OtaCrawlerServices.Contract;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading;

//namespace HJD.HotelServices.Implement.Helper.Channel.Ctrip
//{
//    class CtripSyncCrawlerHotelPackage
//    {
//        public static IOtaCrawlerService otaCrawlerService = ServiceProxyFactory.Create<IOtaCrawlerService>("IOtaCrawlerService");
//        private static IMemcacheProvider memCacheHotelCtripPackage = MemcacheManagerFactory.Create("HotelCtripPackageCache");
//        public int HotelID;
//        public long HotelOriID;
//        public DateTime ArrivalTime;
//        public DateTime DepartureTime;
//        public bool IsAsyn;
//        public Thread Thread;
//        public List<CrawlerHotel> CHotelList;

//        public void Start()
//        {
//            Thread = new Thread(this.Go);
//            Thread.IsBackground = true;
//            Thread.Start();
//        }

//        public void Go()
//        {
//            try
//            {
//                CrawlerHotel();
//            }
//            catch (Exception ex)
//            {
//                writeLog("CrawlerHotel Error:" + ex.Message);
//            }
            
//        }

//        private void CrawlerHotel()
//        {
//            //入住天数
//            var nightCount = (int)(DepartureTime - ArrivalTime).TotalDays;

//            //考虑可能入住多天，那么同一个套餐需要insert/upodate多天
//            for (int i = 0; i < nightCount; i++)
//            {
//                var night = ArrivalTime.AddDays(i);

//                #region 实时抓取xc酒店套餐数据

//                //查询每一个酒店的固定日期内的套餐信息
//                HotelRoomParams hotelRoomParams = new HotelRoomParams
//                {
//                    HotelID = HotelOriID.ToString(),
//                    CheckIn = night.ToString("yyyy-MM-dd"),
//                    CheckOut = night.AddDays(1).ToString("yyyy-MM-dd"),
//                    OtaType = HJD.OtaCrawlerService.Contract.OtaType.Ctrip
//                };

//                var CHotel = otaCrawlerService.GetCanSellHotel(hotelRoomParams);

//                #endregion

//                #region 将抓取的数据插入数据库并作缓存更新等操作

//                try
//                {
//                    //然后将最新抓取的携程数据更新到数据库
//                    var insertRoomStatus = false;

//                    if (CHotel != null && CHotel.HasRoom && CHotel.HotelBaseRoomList != null && CHotel.HotelBaseRoomList.Count > 0)
//                    {
//                        var startTime = DateTime.Now.AddSeconds(-1);

//                        try
//                        {
//                            //遍历房型，然后insert
//                            foreach (var baseRoom in CHotel.HotelBaseRoomList)
//                            {
//                                var roomback = CtripDAL.InsertCtripRoom(baseRoom);

//                                //遍历房间，然后insert
//                                if (baseRoom.HotelRoomList != null)
//                                {
//                                    foreach (var room in baseRoom.HotelRoomList)
//                                    {
//                                        room.Night = night.Date;

//                                        var pricerateback = CtripDAL.InsertCtripPriceRate(room);
//                                        insertRoomStatus = true;
//                                    }
//                                }
//                            }
//                            insertRoomStatus = true;
//                        }
//                        catch (Exception ex)
//                        {
//                            insertRoomStatus = false;
//                        }

//                        //如果抓取的数据Insert/Update成功，则删除/过期该酒店本次Insert/Update之前的数据
//                        if (insertRoomStatus)
//                        {
//                            var del = otaCrawlerService.ExpiredHotelRoom(startTime, HotelOriID, DateTime.Parse(night.ToString("yyyy-MM-dd")));

//                            #region 更新缓存

//                            //异步抓取的，需要异步处理完以后，更新下缓存（实时的就不需要了，因为实时抓取的就已经是最新的了）
//                            if (IsAsyn)
//                            {
//                                #region 默认缓存处理

//                                //缓存key
//                                var cacheKey = string.Format("GetCtripHotelPackages{0}:{1}:{2}", HotelID, ArrivalTime.Date, DepartureTime.Date);

//                                //删除缓存
//                                memCacheHotelCtripPackage.Remove(cacheKey);

//                                //重新取出至缓存
//                                var data = memCacheHotelCtripPackage.GetData<List<PackageInfoEntity>>(cacheKey,
//                                () =>
//                                {
//                                    return HotelPackageHelper.GetPackageList(null, ArrivalTime, DepartureTime, (int)HotelOriID, HotelID);
//                                });

//                                #endregion

//                                #region 异步缓存处理

//                                try
//                                {
//                                    //异步缓存key
//                                    var asynCacheKey = string.Format("GetCtripHotelPackages{0}:{1}:{2}{3}", HotelID, ArrivalTime.Date, DepartureTime.Date, "isAsyn");

//                                    //删除缓存
//                                    memCacheHotelCtripPackage.Remove(asynCacheKey);
//                                }
//                                catch (Exception ex2)
//                                {

//                                }

//                                #endregion
//                            }

//                            #endregion
//                        }
//                    }
//                    else
//                    {
//                        //没有抓取到数据，则过期当天套餐数据
//                        var expricerate = otaCrawlerService.ExpiredHotelPriceRate(HotelOriID, DateTime.Parse(night.ToString("yyyy-MM-dd")));
//                    }
//                }
//                catch (Exception ex)
//                {
                    
//                }

//                #endregion

//                CHotelList.Add(CHotel);
//            }
//        }

//        string logFile = @"D:\Log\HotelService\hotelpack_sync" + DateTime.Now.ToString("_MMdd") + ".txt";

//         public void writeLog(string msg)
//         {
//             try
//             {
//                 //File.AppendAllText(logFile, string.Format("{0}  {1} \r\n\r\n", msg, DateTime.Now));
//             }
//             catch (Exception)
//             {
                 
//             }
//         }
//    }
//}
