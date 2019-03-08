using HJD.AccessService.Contract;
using HJD.AccessService.Contract.Model;
using HJD.AccessService.Implement;
using HJD.AccessService.Implement.Helper;
using HJD.AccessServiceTask.Entity;
using HJD.AccessServiceTask.Job.Helper;
using HJD.Search.CommonLibrary;
using HJD.Search.CommonLibrary.Engine;
using HJD.Search.CommonLibrary.Model;
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
    /// 酒店索引库的酒店源数据监控
    /// </summary>
    public class HotelIndexSourceListener : BaseJob
    {
        public static string SearchIndexRootPath = System.Configuration.ConfigurationManager.AppSettings["SearchIndexRootPath"];

        private static string HotelIndexTimestampConfig = SearchIndexRootPath + @"\Config\Hotel\HotelLastTimestamp.txt";

        public HotelIndexSourceListener()
            : base("HotelIndexSourceListener")
        {
            Log(string.Format("Start Job [HotelIndexSourceListener]"));

            try
            {
                RunJob();
            }
            catch (Exception ex)
            {
                Log("Job [HotelIndexSourceListener] Error:" + ex.Message);
            }

            Log(string.Format("Stop Job [HotelIndexSourceListener]"));
        }

        private void RunJob()
        {
            while (true)
            {
                var hotels = GetUpdatedHotels();

                Console.WriteLine("updated hotels count:" + hotels.Count);
                Log("updated hotels count:" + hotels.Count);

                //去更新索引
                Go(hotels);

                Console.WriteLine("index update end");
                Log("index update end");

                //10分钟以后继续监测
                Thread.Sleep(60000);
            }
        }

        /// <summary>
        /// 获取最新更新的酒店数据
        /// </summary>
        /// <returns></returns>
        private List<HotelEntity> GetUpdatedHotels()
        {
            var lastTimestamp = DownloadHelper.ReadInfo(HotelIndexTimestampConfig);

            Console.WriteLine("lastTimestamp:" + lastTimestamp);
            Log("lastTimestamp:" + lastTimestamp);

            return HotelEngine.GetUpdatesHotels(lastTimestamp);
        }

        /// <summary>
        /// 根据最新酒店更新情况来更新索引
        /// </summary>
        private void Go(List<HotelEntity> hotels)
        {
            foreach (var hotel in hotels)
            {
                switch (hotel.Enabled.ToUpper())
                {
                    case "T":
                        {
                            new AccessExService().AddIndexDoc(hotel.HotelId.ToString(), SearchType.Hotel);

                            Log(string.Format("HotelID:{0}  HotelName:{1}  Enabled:{2}  TimestampCol:{3}", hotel.HotelId, hotel.HotelName, hotel.Enabled, hotel.TimeStampCol));
                            break;
                        }
                    case "F":
                        {
                            new AccessExService().RemoveIndexDoc(hotel.HotelId.ToString(), SearchType.Hotel);

                            Log(string.Format("HotelID:{0}  HotelName:{1}  Enabled:{2}  TimestampCol:{3}", hotel.HotelId, hotel.HotelName, hotel.Enabled, hotel.TimeStampCol));
                            break;
                        }
                }

                //记录最新的timestamp
                System.IO.File.WriteAllText(HotelIndexTimestampConfig, hotel.TimeStampCol, Encoding.Default);
            }
        } 
    }
}
