using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HJD.AccessService.Contract;
using System.ServiceModel;
using System.Configuration;
using HJD.Framework.Interface;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using HJD.AccessService.Implement.Helper;
using System.Text.RegularExpressions;
using HJD.AccessService.Implement;
using HJD.AccessService.Contract.Model;
using RabbitMQ.Client;
using HJD.AccessService.Implement.Entity;
using HJD.AccessService.Contract.Params;
using RabbitMQ.Client.Events;
using HJD.Search.CommonLibrary.Engine;
using HJD.Search.CommonLibrary.Model;
using Newtonsoft.Json;

namespace HJD.AccessService.Implement
{
    /// <summary>
    /// 数据记录与分析
    /// </summary>
    public partial class AccessServices : IAccessService
    {
        #region 索引操作的队列实例

        static IModel mIndexChannel;
        static IConnection indexConnection;
        static int initIndexChannel = 0;
        static string indexQueueKey = "ManagerIndexQueue";

        public IModel GetIndexChannel
        {
            get
            {
                if (initIndexChannel == 0)
                {
                    var factory = new ConnectionFactory();

                    factory.HostName = Config.RabbitmqHostName;
                    factory.UserName = Config.RabbitmqUserName;
                    factory.Password = Config.RabbitmqPassword;

                    indexConnection = factory.CreateConnection();
                    mIndexChannel = indexConnection.CreateModel();
                    mIndexChannel.QueueDeclare(indexQueueKey, false, false, false, null);

                    initIndexChannel = 1;
                }
                return mIndexChannel;
            }
        }

        #endregion

        #region 索引相关操作

        /// <summary>
        /// 向索引队列中安排一条需要添加/更新的doc
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        public void AddIndexDocument(string id, SearchType type) 
        {
            try
            {
                //IndexJob job = new IndexJob();
                //job.Id = id;
                //job.JobType = IndexJobType.Add;
                //job.IndexType = type;

                //var jobStr = JsonConvert.SerializeObject(job);

                //GetIndexChannel.BasicPublish("", indexQueueKey, null, Encoding.UTF8.GetBytes(jobStr));

                //File.WriteAllText(string.Format(@"D:\Log\AccessService\AddIndexDocument_Log_{0}.txt", DateTime.Now.ToString("yyyyMMdd")), string.Format("【{0}】{1}", DateTime.Now, jobStr));

                HotelHelper.UpdateHotelEnabled(Convert.ToInt32(id));
            }
            catch (Exception ex)
            {
                File.WriteAllText(string.Format(@"D:\Log\AccessService\AddIndexDocument_Log_{0}.txt", DateTime.Now.ToString("yyyyMMdd")), string.Format("【{0}】{1}", DateTime.Now, ex.Message));
            }
        }

        /// <summary>
        /// 向索引队列中安排一条需要删除的doc
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        public void RemoveIndexDocument(string id, SearchType type) 
        {
            try
            {
                //IndexJob job = new IndexJob();
                //job.Id = id;
                //job.JobType = IndexJobType.Remove;
                //job.IndexType = type;

                //var jobStr = JsonConvert.SerializeObject(job);

                //GetIndexChannel.BasicPublish("", indexQueueKey, null, Encoding.UTF8.GetBytes(jobStr));

                HotelHelper.UpdateHotelEnabled(Convert.ToInt32(id));
            }
            catch (Exception ex)
            {

            }
        }

        #endregion

        #region 读取队列中的索引操作记录（TEST）

        public void ReaderIndexManagerQueue()
        {
            var factory = new ConnectionFactory();
            factory.HostName = Config.RabbitmqHostName;
            factory.UserName = Config.RabbitmqUserName;
            factory.Password = Config.RabbitmqPassword;

            using (var indexConnection = factory.CreateConnection())
            {
                using (var channel = indexConnection.CreateModel())
                {
                    channel.QueueDeclare(indexQueueKey, false, false, false, null);

                    var consumer = new QueueingBasicConsumer(channel);
                    channel.BasicConsume(indexQueueKey, true, consumer);

                    var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                    while (ea.Body.Length > 0)
                    {
                        try
                        {
                            var body = ea.Body;
                            var message = Encoding.UTF8.GetString(body);

                            var indexJob = JsonConvert.DeserializeObject<IndexJob>(message);
                        }
                        catch (Exception err)
                        {

                        }

                        ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                    }
                }
            }

        }

        #endregion
    


    }
}
