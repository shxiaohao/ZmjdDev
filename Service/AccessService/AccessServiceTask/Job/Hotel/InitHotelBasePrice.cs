using HJD.AccessService.Contract;
using HJD.AccessService.Contract.Model;
using HJD.AccessService.Contract.Model.Hotel;
using HJD.AccessService.Implement;
using HJD.AccessService.Implement.Helper;
using HJD.AccessServiceTask.Entity;
using HJD.AccessServiceTask.Helper;
using HJD.AccessServiceTask.Job.Helper;
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
    /// 初始所有酒店的HotelBasePrice
    /// </summary>
    public class InitHotelBasePrice : BaseJob
    {
        //public static string HotelDbConn = "Data Source=192.168.1.113;Initial Catalog=HotelDB;Persist Security Info=True;User ID=sa;Password=password01!";
        public static string HotelDbConn = "Server=rdss9am4323erjo9qjsx.sqlserver.rds.aliyuncs.com,3433;UID=appuser;password=C1g8oC__JcW_C1g8oC__JcW_;database=HotelDB;";

        public InitHotelBasePrice()
            : base("InitHotelBasePrice")
        {
            Log(string.Format("Start Job [InitHotelBasePrice]"));

            try
            {
                RunJob();
            }
            catch (Exception ex)
            {
                Log("Job [InitHotelBasePrice] Error:" + ex.Message);
            }

            Log(string.Format("Stop Job [InitHotelBasePrice]"));
        }

        private void RunJob()
        {
            //sql config
            DBHelper.HotelDbConn = HotelDbConn;

            //获取所有还没有HotelBasePrice的酒店数据
            var noBasePriceHotels = HotelHelper.GetNoBasePriceHotels();
            if (noBasePriceHotels != null && noBasePriceHotels.Count > 0)
            {
                Console.WriteLine("GetNoBasePriceHotels Count:" + noBasePriceHotels.Count);
                Log("GetNoBasePriceHotels Count:" + noBasePriceHotels.Count);

                for (int i = 0; i < noBasePriceHotels.Count; i++)
                {
                    var hid = noBasePriceHotels[i];
                    Console.WriteLine(string.Format("【{0}】", hid));
                    Log(string.Format("【{0}】", hid));

                    try
                    {
                        int minPrice = 0;

                        //获取最近的一个可售价格（目前最多查7天）
                        var startTime = DateTime.Now.AddDays(2);
                        for (int dnum = 0; dnum < 5; dnum++)
                        {
                            var night = startTime.AddDays(dnum);
                            Console.WriteLine(dnum + "> night:" + night.ToString("yyyy-MM-dd"));

                            var priceEntity = HJD.AccessServiceTask.Job.Search.HotelPriceSlotEngine.GetHotelOnlinePackages(hid, night);
                            if (priceEntity != null && priceEntity.Packages != null && priceEntity.Packages.Count > 0)
                            {
                                var packages = priceEntity.Packages.OrderBy(p => p.Price).ToList();
                                if (packages != null && packages.Count > 0) 
                                {
                                    //得到最低价格
                                    for (int pnum = 0; pnum < packages.Count; pnum++)
                                    {
                                        var _packageEntity = packages[pnum];
                                        if (_packageEntity.Price > 0)
                                        {
                                            minPrice = _packageEntity.Price;
                                            Console.WriteLine("拿到价格：" + minPrice);
                                            break;
                                        }
                                    }

                                    if (minPrice > 0)
                                    {
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("这天没价格，拜拜~");
                            }
                        }

                        if (minPrice > 0)
                        {
                            var add = HotelHelper.AddHotelBasePrice(hid, minPrice, 0, 2);
                            Console.WriteLine("AddHotelBasePrice:" + add);
                            Log("AddHotelBasePrice:" + add);
                        }
                        else
                        {
                            Console.WriteLine("没有得到最低价格");
                            Log("没有得到最低价格");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(string.Format("{0} Err:{1}", hid, ex.Message));
                        Log(string.Format("{0} Err:{1}", hid, ex.Message));
                    }
                }
            }
            else
            {
                Console.WriteLine("GetNoBasePriceHotels is null");
                Log("GetNoBasePriceHotels is null");
            }
        }
    }
}
