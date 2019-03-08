using HJD.AccessService.Contract;
using HJD.AccessService.Contract.Model;
using HJD.AccessService.Contract.Model.Hotel;
using HJD.AccessService.Implement;
using HJD.AccessService.Implement.Helper;
using HJD.AccessServiceTask.Entity;
using HJD.AccessServiceTask.Helper;
using HJD.AccessServiceTask.Job.Helper;
using HJD.HotelPrice.Contract.DataContract;
using HJD.HotelServices.Contracts;
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
    public class HotelPriceSlotEngine : BaseJob
    {
        static string hps_queueKey = "HotelPriceSlotQueue";

        static int whilenum = 1;

        //public static string HotelDbConn = "Data Source=192.168.1.113;Initial Catalog=HotelDB;Persist Security Info=True;User ID=sa;Password=password01!";
        public static string HotelDbConn = "Server=rdss9am4323erjo9qjsx.sqlserver.rds.aliyuncs.com,3433;UID=appuser;password=C1g8oC__JcW_C1g8oC__JcW_;database=HotelDB;";

        public HotelPriceSlotEngine()
            : base("HotelPriceSlotEngine")
        {
            Log(string.Format("Start Job [HotelPriceSlotEngine]"));

            try
            {
                RunJob();
            }
            catch (Exception ex)
            {
                Log("Job [HotelPriceSlotEngine] Error:" + ex.Message);
            }

            Log(string.Format("Stop Job [HotelPriceSlotEngine]"));
        }

        private void RunJob()
        {
            //执行常规的周期处理机制
            GeneralAction(true);

            Console.WriteLine(string.Format("[{0}] 第 {1} 轮的处理结束", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.ff"), whilenum));
            Log(string.Format("[{0}] 第 {1} 轮的处理结束", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.ff"), whilenum));
        }

        #region 周期处理

        private void GeneralAction(object state)
        {
            DBHelper.HotelDbConn = HotelDbConn;

            var mailResult = "";

            Log(string.Format("[{0}] 周期处理程序已经开启..", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.ff")));
            Console.WriteLine(string.Format("[{0}] 周期处理程序已经开启..", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.ff")));
            mailResult += (string.Format("[{0}] 周期处理程序已经开启..", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.ff")) + "<br />");

            //首先拿到所有需要统计价格段的酒店
            List<int> hotelList = GetGeneralHotelList();

            Log(string.Format("[{0}] 需要统计价格段的酒店数量：{1}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.ff"), hotelList.Count));
            Console.WriteLine(string.Format("[{0}] 需要统计价格段的酒店数量：{1}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.ff"), hotelList.Count));
            mailResult += (string.Format("[{0}] 需要统计价格段的酒店数量：{1}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.ff"), hotelList.Count) + "<br />");

            //处理每一个酒店
            for (int hnum = 0; hnum < hotelList.Count; hnum++)
            {
                var hotelid = hotelList[hnum];

                GenHotelMoreDaysSlot(hotelid, 30);
            }

            try
            {
                //所有的酒店统计好以后，做一次过期清理（将小于今天的HotelPriceSlot数据删除）
                var clear = HotelHelper.ClearPricePlan(DateTime.Now);

                Log(string.Format("[{0}] 删除小于今天的Slot数据:{1}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.ff"), clear));
                Console.WriteLine(string.Format("[{0}] 删除小于今天的Slot数据:{1}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.ff"), clear));
                mailResult += (string.Format("[{0}] 删除小于今天的Slot数据:{1}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.ff"), clear) + "<br />");
            }
            catch (Exception ex)
            {
                Log(string.Format("[{0}] 删除Slot数据错误: {1}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.ff"), ex.Message));
                Console.WriteLine(string.Format("[{0}] 删除Slot数据错误: {1}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.ff"), ex.Message));
                mailResult += (string.Format("[{0}] 删除Slot数据错误: {1}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.ff"), ex.Message) + "<br />");
            }

            #region 执行完的邮件报告（暂先不发送）

            ////任务执行完，发送邮件报告结果
            //try
            //{
            //    SendResult(mailResult);

            //    Log(string.Format("[{0}] 邮件已经发送", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.ff")));
            //    Console.WriteLine(string.Format("[{0}] 邮件已经发送", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.ff")));
            //}
            //catch (Exception ex)
            //{
            //    Log(string.Format("[{0}] 发送邮件错误: {1}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.ff"), ex.Message));
            //    Console.WriteLine(string.Format("[{0}] 发送邮件错误: {1}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.ff"), ex.Message));
            //}

            #endregion
        }

        private List<int> GetGeneralHotelList()
        {
            var hotelList = new List<int>();
            hotelList.Add(597230);
            hotelList.AddRange(HotelHelper.GetHotelListForPriceSlot());

            return hotelList;
        }

        #endregion

        #region Public

        /// <summary>
        /// 处理指定酒店指定天数的价格段数据
        /// </summary>
        /// <param name="hotelid"></param>
        public static void GenHotelMoreDaysSlot(int hotelid, int days = 15)
        {
            Console.WriteLine("Gen Hotel:" + hotelid);
            Log("Gen Hotel:" + hotelid);

            //查询起始日期
            var startTime = DateTime.Now.Date;

            //处理每一天
            for (int dnum = 0; dnum < days; dnum++)
            {
                var night = startTime.AddDays(dnum);

                //得到酒店的实时查询套餐
                var price = GetHotelOnlinePackages(hotelid, night);

                //处理当前酒店当前Night的价格段
                GenPriceSlot(hotelid, night, price);

                Console.WriteLine("Gen Price Slot Ok:" + hotelid + " 【" + night + "】");
                Log("Gen Price Slot Ok:" + hotelid + " 【" + night + "】");
            }
        }

        /// <summary>
        /// 处理指定酒店指定日期的价格段数据
        /// </summary>
        /// <param name="hotelid"></param>
        /// <param name="night"></param>
        public static void GenHotelSpecifyDaysSlot(int hotelid, DateTime night)
        {
            Console.WriteLine(string.Format("Gen【{0}】【{1}】", hotelid, night));
            Log(string.Format("Gen【{0}】【{1}】", hotelid, night));

            if (night < DateTime.Now.Date)
            {
                Console.WriteLine(string.Format("日期小于今天，不予处理"));
                Log(string.Format("日期小于今天，不予处理"));
                return;
            }

            //得到酒店的实时查询套餐
            var price = GetHotelOnlinePackages(hotelid, night);

            //处理当前酒店当前Night的价格段
            GenPriceSlot(hotelid, night, price);

            Console.WriteLine("Gen Price Slot Ok:" + hotelid + " 【" + night + "】");
            Log("Gen Price Slot Ok:" + hotelid + " 【" + night + "】");
        }

        /// <summary>
        /// 获取指定酒店的指定入住日的套餐信息
        /// </summary>
        /// <param name="hotelId"></param>
        /// <param name="night"></param>
        /// <returns></returns>
        public static HotelPrice2 GetHotelOnlinePackages(int hotelId, DateTime night)
        {
            var price = new HotelPrice2() { HotelID = hotelId, Packages = new List<HotelServices.Contracts.PackageInfoEntity>() };
            
            var checkIn = night.ToString("yyyy-MM-dd");
            var checkOut = night.AddDays(1).ToString("yyyy-MM-dd");

            try
            {
                price = Price.Get32(hotelId, checkIn, checkOut, "wap", 0);

                //检查是否需要重新查询
                if (price != null && price.Packages != null)
                {
                    //如果当前酒店DayLimitMin>1并且存在zmjd套餐但没有可售的，则更换日期再查一次
                    var where1 = price.DayLimitMin > 1 && price.Packages.Exists(p => p.PackageType == 1) && !price.Packages.Exists(p => p.SellState == 1 && p.PackageType == 1 && !p.IsNotSale);

                    //或者当前酒店DayLimitMin==0&&DayLimitMax>1并且存在zmjd套餐但没有可售的，则更换日期再查一次
                    var where2 = price.DayLimitMin <= 1 && price.DayLimitMax > 1 && price.Packages.Exists(p => p.PackageType == 1) && !price.Packages.Exists(p => p.SellState == 1 && p.PackageType == 1 && !p.IsNotSale);

                    //重新查询
                    if (where1)
                    {
                        checkOut = night.AddDays(price.DayLimitMin).ToString("yyyy-MM-dd");
                        price = Price.Get32(hotelId, checkIn, checkOut, "wap", 0);
                    }
                    else if (where2)
                    {
                        checkOut = night.AddDays(price.DayLimitMax).ToString("yyyy-MM-dd");
                        price = Price.Get32(hotelId, checkIn, checkOut, "wap", 0);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Hotel " + hotelId + " Price.Get32 Err:" + ex.Message);
                Log("Hotel " + hotelId + " Price.Get32 Err:" + ex.Message);
            }

            return price;
        }

        /// <summary>
        /// 生成指定酒店指定入住日的的套餐，所包含的价格段
        /// </summary>
        /// <param name="hotelId"></param>
        /// <param name="night"></param>
        /// <param name="price"></param>
        public static void GenPriceSlot(int hotelId, DateTime night, HotelPrice2 price)
        {
            //酒店价格段初始对象(价格段主要用于酒店列表页的价格段搜索使用)
            var hotelPriceSlot = new HotelPriceSlot { HotelId = hotelId, Night = night, MinPrice = 0, MaxPrice = 0, ChannelId = 0, Prices = "", SellState = 0, CreateTime = DateTime.Now, UpdateTime = DateTime.Now };

            //主要用于存储包含爆款在内的列表价信息 HotelMinPrice
            HotelMinPriceEntity hotelMinPrice = new HotelMinPriceEntity();

            //主要用于存储不包含爆款在内的列表价信息 HotelMinPrice
            HotelMinPriceEntity hotelMinPrice2 = new HotelMinPriceEntity();

            //NightCount
            TimeSpan d3 = price.CheckOut.Subtract(price.CheckIn);
            var nightCount = d3.Days > 0 ? d3.Days : 1;

            var pkCode = "";
            var pkBrief = "";
            var vipPrice = 0;
            var onlinePackages = new List<PackageInfoEntity>();
            var onlinePackages2 = new List<PackageInfoEntity>();

            #region 存储每个价格段最低价的字典

            Dictionary<string, int> slotMinPriceDic = new Dictionary<string, int>();
            slotMinPriceDic["0_400"] = 0;
            slotMinPriceDic["0_600"] = 0;
            slotMinPriceDic["0_800"] = 0;
            slotMinPriceDic["0_1000"] = 0;
            slotMinPriceDic["0_1500"] = 0;
            slotMinPriceDic["0_2000"] = 0;
            slotMinPriceDic["0_0"] = 0;
            slotMinPriceDic["400_600"] = 0;
            slotMinPriceDic["400_800"] = 0;
            slotMinPriceDic["400_1000"] = 0;
            slotMinPriceDic["400_1500"] = 0;
            slotMinPriceDic["400_2000"] = 0;
            slotMinPriceDic["400_0"] = 0;
            slotMinPriceDic["600_800"] = 0;
            slotMinPriceDic["600_1000"] = 0;
            slotMinPriceDic["600_1500"] = 0;
            slotMinPriceDic["600_2000"] = 0;
            slotMinPriceDic["600_0"] = 0;
            slotMinPriceDic["800_1000"] = 0;
            slotMinPriceDic["800_1500"] = 0;
            slotMinPriceDic["800_2000"] = 0;
            slotMinPriceDic["800_0"] = 0;
            slotMinPriceDic["1000_1500"] = 0;
            slotMinPriceDic["1000_2000"] = 0;
            slotMinPriceDic["1000_0"] = 0;
            slotMinPriceDic["1500_2000"] = 0;
            slotMinPriceDic["1500_0"] = 0;
            slotMinPriceDic["2000_0"] = 0;

            #endregion

            #region 酒店有套餐的时候通过套餐统计价格段

            if (price != null && price.Packages != null)
            {
                var haveZmjd = false;
                var haveOta = false;

                //存储包含爆款在内的列表价
                var minPackageInfo = new PackageInfoEntity();

                //存储不包含爆款的列表价
                var minPackageInfo2 = new PackageInfoEntity();

                //首先判断是否存在可售的套餐
                if (price.Packages.Exists(p => p.SellState == 1 && !p.IsNotSale))
                {
                    //if (price.DayLimitMin > 1)
                    //{
                    //    nightCount = price.DayLimitMin;
                    //}

                    Console.WriteLine("有可售套餐，NightCount：" + nightCount);

                    #region 1 【PricePlan】考虑爆款套餐的处理方式

                    #region a 存在爆款

                    //存在可售爆款
                    if (price.Packages.Exists(p => p.SellState == 1 && !p.IsNotSale && p.packageBase != null && p.packageBase.ForVIPFirstBuy && p.VIPPrice > 0))
                    {
                        Console.WriteLine(string.Format("【这家有爆款】【{0}】", hotelId));

                        //则找出爆款中VIP价格最便宜的
                        onlinePackages = price.Packages.Where(p => p.SellState == 1 && !p.IsNotSale && p.packageBase != null && p.packageBase.ForVIPFirstBuy && p.VIPPrice > 0).OrderBy(p => p.VIPPrice).ToList();

                        //存在zmjd的套餐
                        if (onlinePackages.Exists(pk => pk.PackageType == 1))
                        {
                            onlinePackages = onlinePackages.Where(p => p.PackageType == 1).OrderBy(p => p.VIPPrice).ToList();

                            #region 存在zmjd套餐时，则清空这一天之前ota的列表价

                            haveZmjd = true;

                            //try
                            //{
                            //    var _dzmjd = HotelHelper.ClearPricePlanAndPriceSlot(hotelId, 103, night);
                            //    Console.WriteLine(string.Format(" -_- 【ota】删除酒店{0} 日期{1}的下线套餐列表价", hotelId, night));
                            //}
                            //catch (Exception ex)
                            //{
                            //    Console.WriteLine("清空当前日期的ota列表价错误：" + ex.Message);
                            //}

                            #endregion
                        }
                        else
                        {
                            #region 当没有zmjd可售套餐时，清空这一天的所有旧的zmjd的列表价

                            //标识存在ota
                            if (onlinePackages.Exists(pk => pk.PackageType == 4 || pk.PackageType == 5 || pk.PackageType == 6))
                            {
                                onlinePackages = onlinePackages.Where(pk => pk.PackageType == 4 || pk.PackageType == 5 || pk.PackageType == 6).OrderBy(p => p.NotVIPPrice).ToList();

                                haveOta = true;
                            }

                            //try
                            //{
                            //    var _dzmjd = HotelHelper.ClearPricePlanAndPriceSlot(hotelId, 100, night);
                            //    Console.WriteLine(string.Format(" -_- 【zmjd】删除酒店{0} 日期{1}的下线套餐列表价", hotelId, night));
                            //}
                            //catch (Exception ex)
                            //{
                            //    Console.WriteLine("清空当前日期的zmjd列表价错误：" + ex.Message);
                            //}

                            #endregion
                        }
                    }

                    #endregion

                    #region b 不存在爆款

                    else
                    {
                        Console.WriteLine(string.Format("【没有爆款】【{0}】", hotelId));

                        onlinePackages = price.Packages.Where(p => p.SellState == 1 && !p.IsNotSale).OrderBy(p => p.NotVIPPrice).ToList();

                        //存在zmjd的套餐
                        if (onlinePackages.Exists(pk => pk.PackageType == 1))
                        {
                            onlinePackages = onlinePackages.Where(p => p.PackageType == 1).OrderBy(p => p.NotVIPPrice).ToList();

                            #region 存在zmjd套餐时，则清空这一天之前ota的列表价

                            haveZmjd = true;

                            //try
                            //{
                            //    var _dzmjd = HotelHelper.ClearPricePlanAndPriceSlot(hotelId, 103, night);
                            //    Console.WriteLine(string.Format(" -_- 【ota】删除酒店{0} 日期{1}的下线套餐列表价", hotelId, night));
                            //}
                            //catch (Exception ex)
                            //{
                            //    Console.WriteLine("清空当前日期的ota列表价错误：" + ex.Message);
                            //}

                            #endregion
                        }
                        else
                        {
                            #region 当没有zmjd可售套餐时，清空这一天的所有旧的zmjd的列表价

                            //标识存在ota
                            if (onlinePackages.Exists(pk => pk.PackageType == 4 || pk.PackageType == 5 || pk.PackageType == 6))
                            {
                                onlinePackages = onlinePackages.Where(pk => pk.PackageType == 4 || pk.PackageType == 5 || pk.PackageType == 6).OrderBy(p => p.NotVIPPrice).ToList();

                                haveOta = true;
                            }

                            //try
                            //{
                            //    var _dzmjd = HotelHelper.ClearPricePlanAndPriceSlot(hotelId, 100, night);
                            //    Console.WriteLine(string.Format(" -_- 【zmjd】删除酒店{0} 日期{1}的下线套餐列表价", hotelId, night));
                            //}
                            //catch (Exception ex)
                            //{
                            //    Console.WriteLine("清空当前日期的zmjd列表价错误：" + ex.Message);
                            //}

                            #endregion
                        }
                    }

                    #endregion

                    #endregion

                    #region 2 【HotelMinPrice】不考虑爆款套餐的处理方式

                    onlinePackages2 = price.Packages.Where(p => p.SellState == 1 && !p.IsNotSale && !p.packageBase.ForVIPFirstBuy).OrderBy(p => p.NotVIPPrice).ToList();

                    //存在zmjd的套餐
                    if (onlinePackages2.Exists(pk => pk.PackageType == 1))
                    {
                        onlinePackages2 = onlinePackages2.Where(p => p.PackageType == 1 && !p.packageBase.ForVIPFirstBuy).OrderBy(p => p.NotVIPPrice).ToList();

                        #region 存在zmjd套餐时，则清空这一天之前ota的列表价

                        haveZmjd = true;

                        //try
                        //{
                        //    var _dzmjd = HotelHelper.ClearPricePlanAndPriceSlot(hotelId, 103, night);
                        //    Console.WriteLine(string.Format(" -_- 【ota】删除酒店{0} 日期{1}的下线套餐列表价", hotelId, night));
                        //}
                        //catch (Exception ex)
                        //{
                        //    Console.WriteLine("清空当前日期的ota列表价错误：" + ex.Message);
                        //}

                        #endregion
                    }
                    else
                    {
                        #region 当没有zmjd可售套餐时，清空这一天的所有旧的zmjd的列表价

                        //标识存在ota
                        if (onlinePackages2.Exists(pk => pk.PackageType == 4 || pk.PackageType == 5 || pk.PackageType == 6))
                        {
                            haveOta = true;
                        }

                        //try
                        //{
                        //    var _dzmjd = HotelHelper.ClearPricePlanAndPriceSlot(hotelId, 100, night);
                        //    Console.WriteLine(string.Format(" -_- 【zmjd】删除酒店{0} 日期{1}的下线套餐列表价", hotelId, night));
                        //}
                        //catch (Exception ex)
                        //{
                        //    Console.WriteLine("清空当前日期的zmjd列表价错误：" + ex.Message);
                        //}

                        #endregion
                    }

                    #endregion

                    #region 3 【zmjd clear】不存在zmjd套餐，清空当前旧的zmjd套餐

                    if (!haveZmjd)
                    {
                        try
                        {
                            var _dzmjd = HotelHelper.ClearPricePlanAndPriceSlot(hotelId, 100, night);
                            Console.WriteLine(string.Format(" -_- 【zmjd】删除酒店{0} 日期{1}的下线套餐列表价", hotelId, night));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("清空当前日期的zmjd列表价错误：" + ex.Message);
                        }
                    }

                    #endregion

                    #region 4 【ota clear】不存在ota套餐，清空当前旧的ota套餐

                    if (!haveOta)
                    {
                        try
                        {
                            var _dota = HotelHelper.ClearPricePlanAndPriceSlot(hotelId, 103, night);
                            Console.WriteLine(string.Format(" -_- 【ota】删除酒店{0} 日期{1}的下线套餐列表价", hotelId, night));
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("清空当前日期的ota列表价错误：" + ex.Message);
                        }
                    }

                    #endregion
                }
                else
                { 
                    //没有可售套餐
                    //onlinePackages = price.Packages.OrderBy(p => p.NotVIPPrice).ToList();
                    //onlinePackages2 = price.Packages.OrderBy(p => p.NotVIPPrice).ToList();

                    //什么套餐都没有的时候，清空这一天的所有列表价..
                    Console.WriteLine("【注意注意】这一天什么套餐都没有，要全清空！");

                    #region 【all ota clear】清空这一天所有旧的ota列表价

                    try
                    {
                        var _dota = HotelHelper.ClearPricePlanAndPriceSlot(hotelId, 103, night);
                        Console.WriteLine(string.Format(" -_- 【1 ota】删除酒店{0} 日期{1}的下线套餐列表价", hotelId, night));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("清空当前日期的ota列表价错误：" + ex.Message);
                    }

                    #endregion

                    #region 【all zmjd clear】清空这一天的所有旧的zmjd的列表价

                    try
                    {
                        var _dzmjd = HotelHelper.ClearPricePlanAndPriceSlot(hotelId, 100, night);
                        Console.WriteLine(string.Format(" -_- 【2 zmjd】删除酒店{0} 日期{1}的下线套餐列表价", hotelId, night));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("清空当前日期的zmjd列表价错误：" + ex.Message);
                    }

                    #endregion
                }

                #region 【PricePlan Result】拿到有效的列表价list，计算出一个最终可用的列表价（PricePlan）和价格段数据（HotelPriceSlot）

                if (onlinePackages != null && onlinePackages.Count > 0)
                {
                    var priceSlotList = new List<int>();
                    var prices = "";

                    for (int pnum = 0; pnum < onlinePackages.Count; pnum++)
                    {
                        var package = onlinePackages[pnum];
                        
                        //根据当前套餐价格设置其所包含在的价格段
                        SetPriceSlotByPrice(package.NotVIPPrice, hotelPriceSlot, ref slotMinPriceDic);

                        ////记录下原始价格，可追溯吧
                        //if (!string.IsNullOrEmpty(prices)) prices += ",";
                        //prices += package.Price;
                    }

                    //找出最便宜的一个套餐
                    minPackageInfo = onlinePackages.OrderBy(o => o.NotVIPPrice).First();
                    if (minPackageInfo != null)
                    {
                        //min
                        hotelPriceSlot.MinPrice = minPackageInfo.NotVIPPrice;
                    }

                    //max price
                    hotelPriceSlot.MaxPrice = onlinePackages.Max(o => o.NotVIPPrice);

                    //channel
                    hotelPriceSlot.ChannelId = minPackageInfo.PackageType;

                    switch (hotelPriceSlot.ChannelId)
                    {
                        case 1: hotelPriceSlot.ChannelId = 100; break;
                        case 2: hotelPriceSlot.ChannelId = 2; break;
                        case 3: hotelPriceSlot.ChannelId = 102; break;
                        case 4:
                        case 5:
                        case 6: hotelPriceSlot.ChannelId = 103; break;
                        default: hotelPriceSlot.ChannelId = 100; break;
                    }

                    Console.WriteLine(string.Format("onlinePackages[0].PackageType:{0} ChannelId:{1}", onlinePackages[0].PackageType, hotelPriceSlot.ChannelId));

                    hotelPriceSlot.Prices = prices;
                    hotelPriceSlot.SellState = 1;

                    vipPrice = minPackageInfo.VIPPrice;
                    pkCode = minPackageInfo.packageBase.Code;
                    pkBrief = minPackageInfo.packageBase.Brief ?? "";
                    try
                    {
                        if (string.IsNullOrEmpty(pkBrief) || string.IsNullOrEmpty(pkBrief.Trim()))
                        {
                            pkBrief = minPackageInfo.DailyItems.First().Items.FindAll(_ => _.Type == 1).First().Description;
                        }
                    }
                    catch (Exception e)
                    {

                    }

                    //赋值hotelMinPrice对象，用于存储 HotelMinPrice 表（如果不存在爆款，则不需要赋值）
                    if (minPackageInfo.packageBase.ForVIPFirstBuy)
                    {
                        hotelMinPrice.HotelID = hotelId;
                        hotelMinPrice.ChannelID = hotelPriceSlot.ChannelId;
                        hotelMinPrice.Date = hotelPriceSlot.Night.Date;
                        hotelMinPrice.NightCount = nightCount;
                        hotelMinPrice.Price = hotelPriceSlot.MinPrice;
                        hotelMinPrice.VipPrice = vipPrice;
                        hotelMinPrice.PID = (hotelMinPrice.ChannelID == 100 ? minPackageInfo.packageBase.ID : 0);   //目前只有zmjd的套餐才会记录PID（暂时其他平台的pid没有什么可用价值）
                        hotelMinPrice.Name = pkCode;
                        hotelMinPrice.Brief = pkBrief;
                        hotelMinPrice.Type = (minPackageInfo.packageBase.ForVIPFirstBuy ? 1 : 0);   //新VIP专享的套餐标识1
                        hotelMinPrice.UpdateTime = DateTime.Now.Date;   
                    }
                }

                #endregion

                #region 【HotelMinPrice Result】计算最终的HotelMinPrice 2

                if (onlinePackages2 != null && onlinePackages2.Count > 0)
                {
                    //找出最便宜的一个套餐
                    minPackageInfo2 = onlinePackages2.OrderBy(o => o.NotVIPPrice).First();

                    //赋值hotelMinPrice对象，用于存储 HotelMinPrice 表
                    hotelMinPrice2.HotelID = hotelId;
                    switch (minPackageInfo2.PackageType)
                    {
                        case 1: hotelMinPrice2.ChannelID = 100; break;
                        case 2: hotelMinPrice2.ChannelID = 2; break;
                        case 3: hotelMinPrice2.ChannelID = 102; break;
                        case 4:
                        case 5:
                        case 6: hotelMinPrice2.ChannelID = 103; break;
                        default: hotelMinPrice2.ChannelID = 100; break;
                    }
                    hotelMinPrice2.Date = night.Date;
                    hotelMinPrice2.NightCount = nightCount;
                    hotelMinPrice2.Price = minPackageInfo2.NotVIPPrice;
                    hotelMinPrice2.VipPrice = minPackageInfo2.VIPPrice;
                    hotelMinPrice2.PID = (hotelMinPrice2.ChannelID == 100 ? minPackageInfo2.packageBase.ID : 0);   //目前只有zmjd的套餐才会记录PID（暂时其他平台的pid没有什么可用价值）
                    hotelMinPrice2.Name = minPackageInfo2.packageBase.Code;
                    hotelMinPrice2.Brief = minPackageInfo2.packageBase.Brief ?? "";
                    hotelMinPrice2.Type = (minPackageInfo2.packageBase.ForVIPFirstBuy ? 1 : 0);   //新VIP专享的套餐标识1
                    hotelMinPrice2.UpdateTime = DateTime.Now.Date;
                }

                #endregion
            }

            #endregion

            #region 没有套餐的，检查是否有OtaList，有则通过Ota的价格统计价格段

            if (onlinePackages.Count <= 0 && price.OTAList != null && price.OTAList.Count > 0)
            {
                Console.WriteLine("【Use OTAList】" + hotelId + " " + night);

                var priceSlotList = new List<int>();
                var prices = "";

                for (int onum = 0; onum < price.OTAList.Count; onum++)
                {
                    var ota = price.OTAList[onum];

                    //根据当前套餐价格设置其所包含在的价格段
                    SetPriceSlotByPrice((int)ota.Price, hotelPriceSlot, ref slotMinPriceDic);

                    ////记录下原始价格，可追溯吧
                    //if (!string.IsNullOrEmpty(prices)) prices += ",";
                    //prices += (int)ota.Price;
                }

                //找出最便宜的一个套餐
                var minOta = price.OTAList.OrderBy(o => o.Price).First();
                if (minOta != null)
                {
                    //min
                    hotelPriceSlot.MinPrice = (int)minOta.Price;
                }

                //max price
                hotelPriceSlot.MaxPrice = (int)price.OTAList.Max(o => o.Price);

                //channel
                hotelPriceSlot.ChannelId = minOta.ChannelID;

                hotelPriceSlot.Prices = prices;
                hotelPriceSlot.SellState = 1;

                pkCode = minOta.PriceName;
                pkBrief = minOta.PriceBrief;
            }

            #endregion

            //价格段的原始结构数据
            var pricesDicStr = ""; 
            try { pricesDicStr = JsonConvert.SerializeObject(slotMinPriceDic); } catch (Exception ex) { }
            hotelPriceSlot.Prices = pricesDicStr;

            //更新该酒店的HotelPriceSlot
            var add = HotelHelper.AddPriceSlot(hotelPriceSlot);

            //更新这一天的PricePlan以及列表扩展表
            if(hotelPriceSlot.MinPrice > 0) 
            {
                UpdatePricePlan(hotelPriceSlot, nightCount, vipPrice, pkCode, pkBrief);
            }

            //更新HotelMinPrice(可能包含新VIP专享套餐列表价)
            if (hotelMinPrice.Price > 0)
            {
                UpdateHotelMinPrice(hotelMinPrice);
            }

            //更新HotelMinPrice(不会包含新VIP专享的)
            if (hotelMinPrice2.Price > 0)
            {
                UpdateHotelMinPrice(hotelMinPrice2);
            }
        }

        /// <summary>
        /// 处理指定价格的价格段包含
        /// </summary>
        /// <param name="price"></param>
        /// <param name="priceSlot"></param>
        public static void SetPriceSlotByPrice(int price, HotelPriceSlot priceSlot, ref Dictionary<string, int> slotMinPriceDic)
        {
            var maxPrice = int.MaxValue;

            //0-99999999
            //0-2000
            //0-1500
            //0-1000
            //0-500
            //500-99999999
            //500-2000
            //500-1500
            //500-1000
            //1000-99999999
            //1000-2000
            //1000-1500
            //1500-99999999
            //1500-2000
            //2000-99999999

            if (price >= 0 && price <= 400) { priceSlot.Slot_0_400 = true; if (slotMinPriceDic["0_400"] == 0 || price < slotMinPriceDic["0_400"]) { slotMinPriceDic["0_400"] = price; } }
            if (price >= 0 && price <= 600) { priceSlot.Slot_0_600 = true; if (slotMinPriceDic["0_600"] == 0 || price < slotMinPriceDic["0_600"]) { slotMinPriceDic["0_600"] = price; } }
            if (price >= 0 && price <= 800) { priceSlot.Slot_0_800 = true; if (slotMinPriceDic["0_800"] == 0 || price < slotMinPriceDic["0_800"]) { slotMinPriceDic["0_800"] = price; } }
            if (price >= 0 && price <= 1000) { priceSlot.Slot_0_1000 = true; if (slotMinPriceDic["0_1000"] == 0 || price < slotMinPriceDic["0_1000"]) { slotMinPriceDic["0_1000"] = price; } }
            if (price >= 0 && price <= 1500) { priceSlot.Slot_0_1500 = true; if (slotMinPriceDic["0_1500"] == 0 || price < slotMinPriceDic["0_1500"]) { slotMinPriceDic["0_1500"] = price; } }
            if (price >= 0 && price <= 2000) { priceSlot.Slot_0_2000 = true; if (slotMinPriceDic["0_2000"] == 0 || price < slotMinPriceDic["0_2000"]) { slotMinPriceDic["0_2000"] = price; } }
            if (price >= 0 && price <= maxPrice) { priceSlot.Slot_0_0 = true; if (slotMinPriceDic["0_0"] == 0 || price < slotMinPriceDic["0_0"]) { slotMinPriceDic["0_0"] = price; } }

            if (price >= 400 && price <= 600) { priceSlot.Slot_400_600 = true; if (slotMinPriceDic["400_600"] == 0 || price < slotMinPriceDic["400_600"]) { slotMinPriceDic["400_600"] = price; } }
            if (price >= 400 && price <= 800) { priceSlot.Slot_400_800 = true; if (slotMinPriceDic["400_800"] == 0 || price < slotMinPriceDic["400_800"]) { slotMinPriceDic["400_800"] = price; } }
            if (price >= 400 && price <= 1000) { priceSlot.Slot_400_1000 = true; if (slotMinPriceDic["400_1000"] == 0 || price < slotMinPriceDic["400_1000"]) { slotMinPriceDic["400_1000"] = price; } }
            if (price >= 400 && price <= 1500) { priceSlot.Slot_400_1500 = true; if (slotMinPriceDic["400_1500"] == 0 || price < slotMinPriceDic["400_1500"]) { slotMinPriceDic["400_1500"] = price; } }
            if (price >= 400 && price <= 2000) { priceSlot.Slot_400_2000 = true; if (slotMinPriceDic["400_2000"] == 0 || price < slotMinPriceDic["400_2000"]) { slotMinPriceDic["400_2000"] = price; } }
            if (price >= 400 && price <= maxPrice) { priceSlot.Slot_400_0 = true; if (slotMinPriceDic["400_0"] == 0 || price < slotMinPriceDic["400_0"]) { slotMinPriceDic["400_0"] = price; } }

            if (price >= 600 && price <= 800) { priceSlot.Slot_600_800 = true; if (slotMinPriceDic["600_800"] == 0 || price < slotMinPriceDic["600_800"]) { slotMinPriceDic["600_800"] = price; } }
            if (price >= 600 && price <= 1000) { priceSlot.Slot_600_1000 = true; if (slotMinPriceDic["600_1000"] == 0 || price < slotMinPriceDic["600_1000"]) { slotMinPriceDic["600_1000"] = price; } }
            if (price >= 600 && price <= 1500) { priceSlot.Slot_600_1500 = true; if (slotMinPriceDic["600_1500"] == 0 || price < slotMinPriceDic["600_1500"]) { slotMinPriceDic["600_1500"] = price; } }
            if (price >= 600 && price <= 2000) { priceSlot.Slot_600_2000 = true; if (slotMinPriceDic["600_2000"] == 0 || price < slotMinPriceDic["600_2000"]) { slotMinPriceDic["600_2000"] = price; } }
            if (price >= 600 && price <= maxPrice) { priceSlot.Slot_600_0 = true; if (slotMinPriceDic["600_0"] == 0 || price < slotMinPriceDic["600_0"]) { slotMinPriceDic["600_0"] = price; } }

            if (price >= 800 && price <= 1000) { priceSlot.Slot_800_1000 = true; if (slotMinPriceDic["800_1000"] == 0 || price < slotMinPriceDic["800_1000"]) { slotMinPriceDic["800_1000"] = price; } }
            if (price >= 800 && price <= 1500) { priceSlot.Slot_800_1500 = true; if (slotMinPriceDic["800_1500"] == 0 || price < slotMinPriceDic["800_1500"]) { slotMinPriceDic["800_1500"] = price; } }
            if (price >= 800 && price <= 2000) { priceSlot.Slot_800_2000 = true; if (slotMinPriceDic["800_2000"] == 0 || price < slotMinPriceDic["800_2000"]) { slotMinPriceDic["800_2000"] = price; } }
            if (price >= 800 && price <= maxPrice) { priceSlot.Slot_800_0 = true; if (slotMinPriceDic["800_0"] == 0 || price < slotMinPriceDic["800_0"]) { slotMinPriceDic["800_0"] = price; } }

            if (price >= 1000 && price <= 1500) { priceSlot.Slot_1000_1500 = true; if (slotMinPriceDic["1000_1500"] == 0 || price < slotMinPriceDic["1000_1500"]) { slotMinPriceDic["1000_1500"] = price; } }
            if (price >= 1000 && price <= 2000) { priceSlot.Slot_1000_2000 = true; if (slotMinPriceDic["1000_2000"] == 0 || price < slotMinPriceDic["1000_2000"]) { slotMinPriceDic["1000_2000"] = price; } }
            if (price >= 1000 && price <= maxPrice) { priceSlot.Slot_1000_0 = true; if (slotMinPriceDic["1000_0"] == 0 || price < slotMinPriceDic["1000_0"]) { slotMinPriceDic["1000_0"] = price; } }

            if (price >= 1500 && price <= 2000) { priceSlot.Slot_1500_2000 = true; if (slotMinPriceDic["1500_2000"] == 0 || price < slotMinPriceDic["1500_2000"]) { slotMinPriceDic["1500_2000"] = price; } }
            if (price >= 1500 && price <= maxPrice) { priceSlot.Slot_1500_0 = true; if (slotMinPriceDic["1500_0"] == 0 || price < slotMinPriceDic["1500_0"]) { slotMinPriceDic["1500_0"] = price; } }

            if (price >= 2000 && price <= maxPrice) { priceSlot.Slot_2000_0 = true; if (slotMinPriceDic["2000_0"] == 0 || price < slotMinPriceDic["2000_0"]) { slotMinPriceDic["2000_0"] = price; } }
        }

        /// <summary>
        /// 更新PricePlan表
        /// </summary>
        /// <param name="priceSlot"></param>
        public static void UpdatePricePlan(HotelPriceSlot priceSlot, int nightCount, int vipPrice, string code, string brief)
        {
            try
            {
                var update = HotelHelper.UpdatePricePlan(priceSlot, nightCount, vipPrice);
                Console.WriteLine(string.Format("UpdatePricePlan: [{0}] {1} {2} {3} {4} {5}", update, priceSlot.HotelId, priceSlot.Night.Date, priceSlot.MinPrice, vipPrice, priceSlot.ChannelId));
            }
            catch (Exception ex)
            {
                Log("UpdatePricePlan [" + priceSlot.HotelId + "][" + priceSlot.Night.Date + "][" + priceSlot.MinPrice + "] Error:" + ex.Message);
                Console.WriteLine("UpdatePricePlan [" + priceSlot.HotelId + "][" + priceSlot.Night.Date + "][" + priceSlot.MinPrice + "] Error:" + ex.Message);
            }

            try
            {
                var update2 = HotelHelper.UpdatePricePlanEx(priceSlot, code, brief);
                Console.WriteLine(string.Format("UpdatePricePlanEx: [{0}] {1} {2} {3} {4} {5}", update2, priceSlot.HotelId, priceSlot.Night.Date, priceSlot.ChannelId, code, brief));
            }
            catch (Exception ex)
            {
                Log("UpdatePricePlanEx [" + priceSlot.HotelId + "][" + priceSlot.Night.Date + "][" + code + "][" + brief + "] Error:" + ex.Message);
                Console.WriteLine("UpdatePricePlanEx [" + priceSlot.HotelId + "][" + priceSlot.Night.Date + "][" + code + "][" + brief + "] Error:" + ex.Message);
            }
        }

        //更新HotelMinPrice表
        public static void UpdateHotelMinPrice(HotelMinPriceEntity hotelMinPrice) 
        {
            try
            {
                var update = HotelHelper.UpdateHotelMinPrice(hotelMinPrice);
                Console.WriteLine("执行了 UpdateHotelMinPrice：" + hotelMinPrice.HotelID + " Date:" + hotelMinPrice.Date + " Price:" + hotelMinPrice.Price + " VipPrice:" + hotelMinPrice.VipPrice);
            }
            catch (Exception ex)
            {
                Console.WriteLine("UpdateHotelMinPrice错误：" + hotelMinPrice.HotelID + " Date:" + hotelMinPrice.Date + " Price:" + hotelMinPrice.Price + " VipPrice:" + hotelMinPrice.VipPrice);
            }
        }

        public static void SendResult(string result)
        {
            //发送结果
            #region send

            var subject = string.Format("{0} 【HotelPriceSlot】Send Result", DateTime.Now.ToString("yyyy.MM.dd"));
            var body = result;
            var from = "haoy@zmjiudian.com";
            var toList = new List<string>();
            toList.Add("haoy@zmjiudian.com");

            //send
            //var send = MailHelper.SendMail(from, "kevincai@zmjiudian.com,haoy@zmjiudian.com", subject, body, true, files);
            var send = MailHelper.SendMail(from, toList, subject, body, true);
            if (send)
            {
                Log("SendResult Mail OK!");
            }
            else
            {
                Log("SendResult Mail Fail");
            }

            #endregion
        }

        #region debug、test

        //测试
        public int GetPriceNum(int price)
        {
            var num = 1;

            var maxPrice = int.MaxValue;

            if (price >= maxPrice)
            {
                num = 6;
            }
            else if (price >= 2000 && price < maxPrice)
            {
                num = 5;
            }
            else if (price >= 1500 && price < 2000)
            {
                num = 4;
            }
            else if (price >= 1000 && price < 1500)
            {
                num = 3;
            }
            else if (price >= 500 && price < 1000)
            {
                num = 2;
            }
            else if (price >= 0 && price < 500)
            {
                num = 1;
            }

            return num;
        }

        //测试
        public int GetPriceSlotByPrice(int price)
        {
            var slot = -1;

            var maxPrice = int.MaxValue;

            if (price >= 2000 && price <= maxPrice)
            {
                slot = 2000;
            }
            else if (price >= 1500 && price <= 2000)
            {
                slot = 1500;
            }
            else if (price >= 1000 && price <= 1500)
            {
                slot = 1000;
            }
            else if (price >= 500 && price <= 1000)
            {
                slot = 500;
            }
            else if (price > 0 && price <= 500)
            {
                slot = 1;
            }

            return slot;
        }

        //测试
        public string GetPriceSlotByPriceThink_(int price)
        {
            var slot = "";

            List<string> list = new List<string>();
            list.Add("0-99999999");
            list.Add("0-2000");
            list.Add("0-1500");
            list.Add("0-1000");
            list.Add("0-500");
            list.Add("500-99999999");
            list.Add("500-2000");
            list.Add("500-1500");
            list.Add("500-1000");
            list.Add("1000-99999999");
            list.Add("1000-2000");
            list.Add("1000-1500");
            list.Add("1500-99999999");
            list.Add("1500-2000");
            list.Add("2000-99999999");

            /*

            1)起始值   2）范围值 
            
            1-5           0-99999999
            1-4           0-2000
            1-3           0-1500
            1-2           0-1000
            1-1           0-500
            
            2-5           500-99999999
            2-4           500-2000
            2-3           500-1500
            2-2           500-1000
            
            3-5           1000-99999999
            3-4           1000-2000
            3-3           1000-1500

            4-5           1500-99999999
            4-4           1500-2000
            
            5-5           2000-99999999

             * 
             * 
             * -------起始（<=）
             * 1    0 
             * 2    500 
             * 3    1000 
             * 4    1500 
             * 5    2000
             * 
             * -------结束 （>=）
             * 1    500 
             * 2    1000 
             * 3    1500 
             * 4    2000 
             * 5    999999
             * 
             
            */

            return slot;
        }

        #endregion

        #endregion
    }
}
