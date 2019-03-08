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
    public class HotelPriceSlotListener : BaseJob
    {
        static string hps_queueKey = "HotelPriceSlotQueue2";

        static int whilenum = 1;

        //public static string HotelDbConn = "Data Source=192.168.1.113;Initial Catalog=HotelDB;Persist Security Info=True;User ID=sa;Password=password01!";
        public static string HotelDbConn = "Server=rdss9am4323erjo9qjsx.sqlserver.rds.aliyuncs.com,3433;UID=appuser;password=C1g8oC__JcW_C1g8oC__JcW_;database=HotelDB;";

        public HotelPriceSlotListener()
            : base("HotelPriceSlotListener")
        {
            Log(string.Format("Start Job [HotelPriceSlotListener]"));

            try
            {
                RunJob();
            }
            catch (Exception ex)
            {
                Log("Job [HotelPriceSlotListener] Error:" + ex.Message);
            }

            Log(string.Format("Stop Job [HotelPriceSlotListener]"));
        }

        private void RunJob()
        {
            ////处理该酒店
            //HJD.AccessServiceTask.Job.Search.HotelPriceSlotEngine.GenHotelSpecifyDaysSlot(524028, DateTime.Parse("2017-08-16"));
            //return;

            //HJD.AccessServiceTask.Job.Search.HotelPriceSlotEngine.GenHotelMoreDaysSlot(907362, 3);
            //return;

            //TestAdd();
            //return;

            //开启即时处理机制
            InstantAction();
        }

        #region 即时处理

        #region Test Demo

        private void TestAdd()
        {
            AddPriceSlot(597230);
        }

        static IModel hps_Channel;
        static IConnection hps_connection;
        static int hps_initChannel = 0;
        static string hps_queueKey2 = "HotelPriceSlotQueue";

        public IModel GetHpsChannel
        {
            get
            {
                if (hps_initChannel == 0)
                {
                    var factory = new ConnectionFactory();

                    factory.HostName = "121.40.252.219";
                    factory.UserName = "zmjd";
                    factory.Password = "zmjdpassword01!";

                    hps_connection = factory.CreateConnection();
                    hps_Channel = hps_connection.CreateModel();
                    hps_Channel.QueueDeclare(hps_queueKey2, false, false, false, null);

                    hps_initChannel = 1;
                }
                return hps_Channel;
            }
        }

        public void AddPriceSlot(int hotelid)
        {
            try
            {
                GetHpsChannel.BasicPublish("", hps_queueKey2, null, Encoding.UTF8.GetBytes(hotelid.ToString()));
            }
            catch (Exception ex)
            {

            }
        }

        #endregion

        private void InstantAction()
        {
            Console.ForegroundColor = ConsoleColor.Green;

            DBHelper.HotelDbConn = HotelDbConn;

            var factory = new ConnectionFactory();
            factory.HostName = Config.RabbitmqHostName;
            factory.UserName = Config.RabbitmqUserName;
            factory.Password = Config.RabbitmqPassword;

            Log(string.Format("[{0}] 即时处理的实时程序已经开启..", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.ff")));
            Console.WriteLine(string.Format("[{0}] 即时处理的实时程序已经开启..", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.ff")));
            
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(hps_queueKey, false, false, false, null);

                    var consumer = new QueueingBasicConsumer(channel);
                    channel.BasicConsume(hps_queueKey, false, consumer);
                    channel.BasicQos(0, 1, false);

                    var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                    while (ea.Body.Length > 0)
                    {
                        try
                        {
                            //得到任务队列
                            var item = ea.Body;
                            var hotelInfoStr = Encoding.UTF8.GetString(item);
                            var hotelInfoList = hotelInfoStr.Split(',');

                            //解析出酒店ID & 入住日期
                            var hotelid = Convert.ToInt32(hotelInfoList[0]);
                            var checkIn = DateTime.Now.Date.AddDays(1);
                            var refMoreDate = false;
                            if (hotelInfoList.Length > 1 && !string.IsNullOrEmpty(hotelInfoList[1]))
                            {
                                checkIn = DateTime.Parse(hotelInfoList[1]);
                                Console.WriteLine("【任务指定了入住日】" + checkIn);

                                if (hotelInfoList.Length > 2 && !string.IsNullOrEmpty(hotelInfoList[2]))
                                {
                                    refMoreDate = hotelInfoList[2] == "1";
                                    if (refMoreDate) Console.WriteLine("【需要更新更多日期】");
                                }
                            }

                            Log(string.Format("[{0}] Have New HotelId: {1}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.ff"), hotelid));
                            Console.WriteLine(string.Format("[{0}] Have New HotelId: {1}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.ff"), hotelid));

                            //处理该酒店
                            HJD.AccessServiceTask.Job.Search.HotelPriceSlotEngine.GenHotelSpecifyDaysSlot(hotelid, checkIn);

                            Log(string.Format("[{0}] Hotel Is OK: {1}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.ff"), hotelid));
                            Console.WriteLine(string.Format("[{0}] Hotel Is OK: {1}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.ff"), hotelid));

                            if (refMoreDate)
                            {
                                Log("【需要更新更多日期】");
                                Console.WriteLine("【需要更新更多日期】");

                                var startDate = DateTime.Now.Date.AddDays(1);
                                var endDate = startDate.AddDays(60);

                                while (startDate < endDate)
                                {
                                    if (startDate != checkIn)
                                    {
                                        Log("【处理更多日期】：" + hotelid + "：" + startDate);
                                        Console.WriteLine("【处理更多日期】：" + hotelid + "：" + startDate);

                                        HJD.AccessServiceTask.Job.Search.HotelPriceSlotEngine.GenHotelSpecifyDaysSlot(hotelid, startDate);
                                    }

                                    startDate = startDate.AddDays(1);
                                }

                                Log("【更多日期处理结束】");
                                Console.WriteLine("【更多日期处理结束】");
                            }

                        }
                        catch (Exception err)
                        {
                            Log(string.Format("[{0}] Gen Hotel Err:{1}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.ff"), err.Message));
                            Console.WriteLine(string.Format("[{0}] Gen Hotel Err:{1}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.ff"), err.Message));
                        }

                        channel.BasicAck(ea.DeliveryTag, false);

                        Console.WriteLine(string.Format("-_- 等待任务中 .. "));
                        ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                    }
                }
            }

        }

        #endregion
    }
}
