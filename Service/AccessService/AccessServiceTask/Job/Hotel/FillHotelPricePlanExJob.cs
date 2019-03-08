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
    /// 
    /// </summary>
    public class FillHotelPricePlanExJob : BaseJob
    {
        static int ChannelId = 103;

        //public static string HotelDbConn = "Data Source=192.168.1.113;Initial Catalog=HotelDB;Persist Security Info=True;User ID=sa;Password=password01!";
        public static string HotelDbConn = "Server=rdss9am4323erjo9qjsx.sqlserver.rds.aliyuncs.com,3433;UID=appuser;password=C1g8oC__JcW_C1g8oC__JcW_;database=HotelDB;";

        public FillHotelPricePlanExJob()
            : base("FillHotelPricePlanExJob")
        {
            Log(string.Format("Start Job [FillHotelPricePlanExJob]"));

            try
            {
                RunJob();
            }
            catch (Exception ex)
            {
                Log("Job [FillHotelPricePlanExJob] Error:" + ex.Message);
            }

            Log(string.Format("Stop Job [FillHotelPricePlanExJob]"));
        }

        private void RunJob()
        {
            DBHelper.HotelDbConn = HotelDbConn;

            int sleepCount = 0;

            //首先获取包含空PricePlanEx信息的酒店
            var hids = HotelHelper.GetHaveNullPricePlanExHids(ChannelId);
            if (hids != null && hids.Count > 0)
            {
                Console.WriteLine("GetHaveNullPricePlanExHids Count:" + hids.Count);
                Log("GetHaveNullPricePlanExHids Count:" + hids.Count);

                for (int hnum = 0; hnum < 100; hnum++)
                {
                    var hotelid = hids[hnum];

                    Console.WriteLine("【HotelId】" + hotelid);
                    Log("【HotelId】" + hotelid);

                    //获取当前酒店的所有包含空PricePlanEx的日期
                    var dateList = HotelHelper.GetHaveNullPricePlanExDateListByHid(ChannelId, hotelid);
                    if (dateList != null && dateList.Count > 0)
                    {
                        Console.WriteLine("DateList Count:" + dateList.Count);
                        Log("DateList Count:" + dateList.Count);

                        for (int dnum = 0; dnum < dateList.Count; dnum++)
                        {
                            var checkIn = dateList[dnum];

                            Log(string.Format("[{0}] Have HotelId: {1} {2}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.ff"), hotelid, checkIn));
                            Console.WriteLine(string.Format("[{0}] Have HotelId: {1} {2}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.ff"), hotelid, checkIn));

                            //处理该酒店
                            HJD.AccessServiceTask.Job.Search.HotelPriceSlotEngine.GenHotelSpecifyDaysSlot(hotelid, checkIn);

                            Log(string.Format("[{0}] Hotel Is OK: {1}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.ff"), hotelid));
                            Console.WriteLine(string.Format("[{0}] Hotel Is OK: {1}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.ff"), hotelid));

                            sleepCount++;
                            if (sleepCount >= 100)
                            {
                                sleepCount = 0;
                                Console.WriteLine("抓取100次，休息3分钟");
                                Log("抓取100次，休息3分钟");

                                Thread.Sleep(1000 * 60 * 3);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine(string.Format("DateList is null: {0}", hotelid));
                        Log(string.Format("DateList is null: {0}", hotelid));
                    }
                }
            }
            else
            {
                Console.WriteLine("GetHaveNullPricePlanExHids is null or count = 0");
                Log("GetHaveNullPricePlanExHids is null or count = 0");
            }
        }
    }
}
