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
    /// 索引操作的监听程序
    /// </summary>
    public class IndexActionListener : BaseJob
    {
        static string IndexQueueKey = "ManagerIndexQueue";

        public IndexActionListener()
            : base("IndexActionListener")
        {
            Log(string.Format("Start Job [IndexActionListener]"));

            try
            {
                RunJob();
            }
            catch (Exception ex)
            {
                Log("Job [IndexActionListener] Error:" + ex.Message);
            }


            Log(string.Format("Stop Job [IndexActionListener]"));
        }

        private void RunJob()
        {
            IndexOn();
        }

        /// <summary>
        /// 索引任务执行
        /// </summary>
        private void IndexOn()
        {
            var factory = new ConnectionFactory();
            factory.HostName = Config.RabbitmqHostName;
            factory.UserName = Config.RabbitmqUserName;
            factory.Password = Config.RabbitmqPassword;

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(IndexQueueKey, false, false, false, null);

                    var consumer = new QueueingBasicConsumer(channel);
                    channel.BasicConsume(IndexQueueKey, true, consumer);

                    var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                    while (ea.Body.Length > 0)
                    {
                        try
                        {
                            var item = ea.Body;
                            var indexJobStr = Encoding.UTF8.GetString(item);
                            var indexJob = JsonConvert.DeserializeObject<IndexJob>(indexJobStr);

                            Log(string.Format("ID:{0}  IndexType:{1}   JobType:{2}", indexJob.Id, indexJob.IndexType.ToString(), indexJob.JobType.ToString()));

                            var indexEngine = new IndexEngine(indexJob.IndexType);
                            indexEngine.ProcessIndexJob(indexJob);
                        }
                        catch (Exception err)
                        {
                            Log("indexJob Err: " + err.Message);
                        }

                        ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                        //Task.Factory.StartNew

                        //RabbitMQ

                        //MqConfigDomFactory

                        //EventMessage
                    }
                }
            }
        } 
    }
}
