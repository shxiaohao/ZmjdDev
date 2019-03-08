using HJD.AccessService.Contract;
using HJD.AccessService.Contract.Model;
using HJD.AccessService.Contract.Model.Hotel;
using HJD.AccessService.Implement;
using HJD.AccessService.Implement.Helper;
using HJD.AccessServiceTask.Entity;
using HJD.AccessServiceTask.Helper;
using HJD.AccessServiceTask.Job.Helper;
using HJD.HotelServices.Contracts;
using HJD.OtaCrawlerService.Contract.Ctrip.Hotel;
using HJD.Search.CommonLibrary;
using HJD.Search.CommonLibrary.Engine;
using HJD.Search.CommonLibrary.Model;
using HJDAPI.APIProxy;
using HJDAPI.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace HJD.AccessServiceTask.Job.Search
{
    /// <summary>
    /// 
    /// </summary>
    public class UpdateZmjdPackagePrice : BaseJob
    {
        static int ChannelId = 103;

        //public static string HotelDbConn = "Data Source=192.168.1.113;Initial Catalog=HotelDB;Persist Security Info=True;User ID=sa;Password=password01!";
        public static string HotelDbConn = "Server=rdss9am4323erjo9qjsx.sqlserver.rds.aliyuncs.com,3433;UID=appuser;password=C1g8oC__JcW_C1g8oC__JcW_;database=HotelDB;";

        public UpdateZmjdPackagePrice()
            : base("UpdateZmjdPackagePrice")
        {
            Log(string.Format("Start Job [UpdateZmjdPackagePrice]"));

            try
            {
                RunJob();
            }
            catch (Exception ex)
            {
                Log("Job [UpdateZmjdPackagePrice] Error:" + ex.Message);
            }

            Log(string.Format("Stop Job [UpdateZmjdPackagePrice]"));
        }

        private void RunJob()
        {
            DBHelper.HotelDbConn = HotelDbConn;

            //匹配价格从Ctrip
            MatchOtaForCtrip();
        }

        /// <summary>
        /// Ctrip Price Match
        /// </summary>
        private void MatchOtaForCtrip()
        {
            var priceSource = 1;

            //首先获取所有PriceSource=1的套餐信息
            var allPackageList = new List<HotelPackageEntity>();

            //allPackageList.Add(new HotelPackageEntity { Id = 10805, HotelId = 71513, Code = "会员专享", Brief = "洲际豪华房+双早", SerialNO = "会员专享", RoomID = 27191, PriceSource = 1 });

            allPackageList = HotelHelper.GetAllPackageByPriceSource(priceSource);

            if (allPackageList != null && allPackageList.Count > 0)
            {
                Console.WriteLine("allPackageList 待处理总数：" + allPackageList.Count);
                Log("allPackageList 待处理总数：" + allPackageList.Count);

                //根据酒店分组
                var hotelPackageGroupList = allPackageList.GroupBy(_ => _.HotelId);

                //首先遍历所有酒店
                foreach (var hotelPackageEntity in hotelPackageGroupList)
                {
                    var hotelId = hotelPackageEntity.Key;
                    var thisPackageList = hotelPackageEntity.ToList();

                    Console.WriteLine(string.Format("开始遍历酒店：{0}，套餐数量：{1}", hotelId, thisPackageList.Count));
                    Log(string.Format("开始遍历酒店：{0}，套餐数量：{1}", hotelId, thisPackageList.Count));

                    #region 遍历匹配当前酒店下120天的价格信息

                    var today = DateTime.Now.Date;
                    var days = 120;
                    for (int dayNum = 0; dayNum < days; dayNum++)
                    {
                        //获取指定酒店指定日期Ctrip这一天的套餐价格信息
                        var checkIn = today.AddDays(dayNum);
                        var checkOut = checkIn.AddDays(1);

                        Console.WriteLine(string.Format("第{0}天：{1}", (dayNum + 1), checkIn));
                        Log(string.Format("第{0}天：{1}", (dayNum + 1), checkIn));
                        
                        CrawlerHotel ctripHotel = new OtaCrawler().GetAllPackagesForCtripApi(hotelId, checkIn, checkOut);
                        
                        //遍历当前酒店下的套餐，并去匹配Ctrip下匹配的套餐价格信息
                        for (int pnum = 0; pnum < thisPackageList.Count; pnum++)
                        {
                            var thisPackageEntity = thisPackageList[pnum];

                            Console.WriteLine(string.Format("准备匹配更新套餐：{0} {1} 【{2}】", thisPackageEntity.Id, thisPackageEntity.Brief, thisPackageEntity.Code));
                            Log(string.Format("准备匹配更新套餐：{0} {1} 【{2}】", thisPackageEntity.Id, thisPackageEntity.Brief, thisPackageEntity.Code));

                            //是否抓取到ctrip数据
                            if (ctripHotel != null && ctripHotel.HotelBaseRoomList != null && ctripHotel.HotelBaseRoomList.Count > 0)
                            {
                                //找出匹配
                                var otaRoom = MatchCripPackageResult(ctripHotel, thisPackageEntity);
                                if (otaRoom != null)
                                {
                                    var prateMatchOta = new PRateMatchOtaEntity();
                                    prateMatchOta.PID = Convert.ToInt32(thisPackageEntity.Id);
                                    prateMatchOta.Date = checkIn;
                                    prateMatchOta.PriceSource = priceSource;
                                    prateMatchOta.Price = Convert.ToInt32(otaRoom.Price);
                                    prateMatchOta.SellState = otaRoom.CanSell;

                                    //exc sql
                                    var ins = HotelHelper.UpdatePRateMatchOta(prateMatchOta);

                                    Console.WriteLine(string.Format("更新PRateMatchOta完成 ins：{0}，pid：{1}，price：{2}，sellState：{3}", ins, prateMatchOta.PID, prateMatchOta.Price, prateMatchOta.SellState));
                                    Log(string.Format("更新PRateMatchOta完成 ins：{0}，pid：{1}，price：{2}，sellState：{3}", ins, prateMatchOta.PID, prateMatchOta.Price, prateMatchOta.SellState));
                                }
                                else
                                {
                                    //exc null
                                    var ins = HotelHelper.UpdatePRateMatchOta(new PRateMatchOtaEntity { PID = Convert.ToInt32(thisPackageEntity.Id), Date = checkIn, Price = 0, PriceSource = priceSource, SellState = 0 });

                                    //不能匹配的，则记录另一张表
                                    var notMatchOtaData = new PackageNotMatchOta() { HotelID = hotelId, PID = Convert.ToInt32(thisPackageEntity.Id), Code = thisPackageEntity.Code, Brief = thisPackageEntity.Brief };
                                    var insNotMatch = HotelHelper.UpdatePackageNotMatchOta(notMatchOtaData);

                                    Console.WriteLine(string.Format("没有找到匹配项，已记录PackageNotMatchOta：{0}", insNotMatch));
                                    Log(string.Format("没有找到匹配项，已记录PackageNotMatchOta：{0}", insNotMatch));
                                }
                            }
                            else
                            {
                                //exc null
                                var ins = HotelHelper.UpdatePRateMatchOta(new PRateMatchOtaEntity { PID = Convert.ToInt32(thisPackageEntity.Id), Date = checkIn, Price = 0, PriceSource = priceSource, SellState = 0 });

                                Console.WriteLine(string.Format("Ctrip这一天没有找到任何套餐价格信息：{0} CheckIn：{1}", hotelId, checkIn));
                                Log(string.Format("Ctrip这一天没有找到任何套餐价格信息：{0} CheckIn：{1}", hotelId, checkIn));
                            }
                        }
                    }

                    #endregion

                    //每遍历完一家酒店后，执行存储过程更新PRate
                    var _exec = HotelHelper.SP4_PRate_UpdateWithOTAPrice();
                    Console.WriteLine("----- 每遍历完一家酒店后，执行存储过程更新PRate -----");
                    Log("----- 每遍历完一家酒店后，执行存储过程更新PRate -----");
                    Console.WriteLine("SP4_PRate_UpdateWithOTAPrice：" + _exec);
                    Log("SP4_PRate_UpdateWithOTAPrice：" + _exec);
                    Console.WriteLine("-----------------------------------------------------");
                    Log("-----------------------------------------------------");
                }

            }
            else
            {
                Console.WriteLine("allPackageList is null or count <= 0");
                Log("allPackageList is null or count <= 0");
            }

        }

        /// <summary>
        /// 匹配
        /// </summary>
        /// <param name="ctripHotel"></param>
        /// <param name="hotelPackage"></param>
        /// <returns></returns>
        private CrawlerHotelRoom MatchCripPackageResult(CrawlerHotel ctripHotel, HotelPackageEntity hotelPackage)
        {
            //过滤抓取的XC套餐数据
            FilterSubRoomsV42(ref ctripHotel);

            //遍历当前ota酒店的所有房型，看是否有包含的
            foreach (var baseRoom in ctripHotel.HotelBaseRoomList)
            {
                //zmjd
                var oriKey = hotelPackage.Brief.ToLower().Trim();

                //找出所有匹配的最便宜的一个
                var matchRoomList = baseRoom.HotelRoomList.Where(_room => string.Format("{0}{1}", baseRoom.Name, (_room.Breakfast.Contains("无早") ? "" : "+" + _room.Breakfast)).ToLower().Trim() == oriKey).ToList();
                if (matchRoomList != null && matchRoomList.Count > 0)
                {
                    return matchRoomList.OrderBy(_room => _room.Price).First();
                }
                else
                {
                    //如果没有匹配，尝试通过其他匹配方法？
                    Console.WriteLine("no match");
                    //Log("no match");
                }
            }


            return null;
        }

        /// <summary>
        /// 过滤OTA部分不用的套餐
        /// </summary>
        /// <param name="crawlHotel"></param>
        public static void FilterSubRoomsV42(ref CrawlerHotel crawlHotel)
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
    }
}
