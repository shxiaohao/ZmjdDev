using HJD.Framework.WCF;
using HJD.HotelServices.Contracts;
using HJD.HotelServices.Implement.Entity;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HJD.HotelServices.Implement.Helper
{
    public class AccessServiceHelper
    {
        #region HotelPriceSlot

        static IModel hps_Channel;
        static IConnection hps_connection;
        static int hps_initChannel = 0;
        static string hps_queueKey = "HotelPriceSlotQueue2";

        public IModel GetHpsChannel
        {
            get
            {
                if (hps_initChannel == 0)
                {
                    var factory = new ConnectionFactory();

                    factory.HostName = Config.RabbitmqHostName;
                    factory.UserName = Config.RabbitmqUserName;
                    factory.Password = Config.RabbitmqPassword;

                    hps_connection = factory.CreateConnection();
                    hps_Channel = hps_connection.CreateModel();
                    hps_Channel.QueueDeclare(hps_queueKey, false, false, false, null);

                    hps_initChannel = 1;
                }
                return hps_Channel;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hotelid">触发的酒店ID（zmjd酒店ID）</param>
        /// <param name="checkIn">更新的指定日期</param>
        /// <param name="refMoreDate">是否触发更新更多日期（一般为两个月）</param>
        public void AddPriceSlot(int hotelid, DateTime checkIn, bool refMoreDate = false)
        {
            try
            {
                var taskStr = string.Format("{0},{1},{2}", hotelid, checkIn.ToString("yyyy-MM-dd"), (refMoreDate ? "1" : "0"));

                GetHpsChannel.BasicPublish("", hps_queueKey, null, Encoding.UTF8.GetBytes(taskStr));
            }
            catch (Exception ex)
            {

            }
        }

        #endregion

        #region HotelPriceSlotForBg

        static IModel hps_ChannelForBg;
        static IConnection hps_connectionForBg;
        static int hps_initChannelForBg = 0;
        static string hps_queueKeyForBg = "HotelPriceSlotQueueForBg2";

        public IModel GetHpsChannelForBg
        {
            get
            {
                if (hps_initChannelForBg == 0)
                {
                    var factory = new ConnectionFactory();

                    factory.HostName = Config.RabbitmqHostName;
                    factory.UserName = Config.RabbitmqUserName;
                    factory.Password = Config.RabbitmqPassword;

                    hps_connectionForBg = factory.CreateConnection();
                    hps_ChannelForBg = hps_connectionForBg.CreateModel();
                    hps_ChannelForBg.QueueDeclare(hps_queueKeyForBg, false, false, false, null);

                    hps_initChannelForBg = 1;
                }
                return hps_ChannelForBg;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hotelid">触发的酒店ID（zmjd酒店ID）</param>
        /// <param name="checkIn">更新的指定日期</param>
        /// <param name="refMoreDate">是否触发更新更多日期（一般为两个月）</param>
        public void AddPriceSlotForBg(int hotelid, DateTime checkIn, bool refMoreDate = false)
        {
            try
            {
                //var _file = string.Format(@"D:\Log\HotelService\AddPriceSlotForBg_{0}.txt", DateTime.Now.ToString("yyyyMMdd"));
                //File.WriteAllText(_file, "--" + hotelid + "：" + checkIn + "--");

                var taskStr = string.Format("{0},{1},{2}", hotelid, checkIn.ToString("yyyy-MM-dd"), (refMoreDate ? "1" : "0"));

                GetHpsChannelForBg.BasicPublish("", hps_queueKeyForBg, null, Encoding.UTF8.GetBytes(taskStr));
            }
            catch (Exception ex)
            {

            }
        }

        #endregion
    }
}
