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
    public class OtaServiceHelper
    {
        #region CheckRoomBedState

        static IModel chs_Channel;
        static IConnection chs_connection;
        static int chs_initChannel = 0;
        static string chs_queueKey = "CheckRoomBedState";

        public IModel GetHpsChannel
        {
            get
            {
                if (chs_initChannel == 0)
                {
                    var factory = new ConnectionFactory();

                    factory.HostName = Config.RabbitmqHostName;
                    factory.UserName = Config.RabbitmqUserName;
                    factory.Password = Config.RabbitmqPassword;

                    chs_connection = factory.CreateConnection();
                    chs_Channel = chs_connection.CreateModel();
                    chs_Channel.QueueDeclare(chs_queueKey, false, false, false, null);

                    chs_initChannel = 1;
                }
                return chs_Channel;
            }
        }

        public void AddCheckHotelRoomStateTask(int hotelid, DateTime date)
        {
            try
            {
                var taskStr = string.Format("{0},{1}", hotelid, date.ToString("yyyy-MM-dd"));

                GetHpsChannel.BasicPublish("", chs_queueKey, null, Encoding.UTF8.GetBytes(taskStr));
            }
            catch (Exception ex)
            {

            }
        }

        #endregion
    }
}
