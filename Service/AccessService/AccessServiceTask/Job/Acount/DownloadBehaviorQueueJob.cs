using HJD.AccessService.Implement;
using HJD.AccessService.Implement.Helper;
using HJD.AccessServiceTask.Entity;
using HJD.AccessServiceTask.Job.Helper;
using HJDAPI.APIProxy;
using HtmlAgilityPack;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace HJD.AccessServiceTask.Job.Acount
{
    /// <summary>
    /// 下载用户行为队列
    /// </summary>
    public class DownloadBehaviorQueueJob : BaseJob
    {
        static string QueueKey = "BehaviorQueue";
        static string BehaviorTxtFileName = "behavior_{0}.txt";

        public DownloadBehaviorQueueJob()
            : base("DownloadBehaviorQueueJob")
        {
            Log(string.Format("Start Job [DownloadBehaviorQueueJob]"));

            try
            {
                RunJob();
            }
            catch (Exception ex)
            {
                Log("Job [DownloadBehaviorQueueJob] Error:" + ex.Message);
            }


            Log(string.Format("Stop Job [DownloadBehaviorQueueJob]"));
        }

        private void RunJob()
        {
            var factory = new ConnectionFactory();
            factory.HostName = Config.RabbitmqHostName;
            factory.UserName = Config.RabbitmqUserName;
            factory.Password = Config.RabbitmqPassword;

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(QueueKey, false, false, false, null);

                    var consumer = new QueueingBasicConsumer(channel);
                    channel.BasicConsume(QueueKey, true, consumer);

                    var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                    while (ea.Body.Length > 0)
                    {
                        try
                        {
                            var item = ea.Body;
                            var behavior = Encoding.UTF8.GetString(item);

                            WriteBehaviorToText(behavior);
                        }
                        catch (Exception err)
                        {
                            Log("consumer.Queue.Dequeue Err: " + err.Message);
                        }

                        ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

                    }
                }
            }
        }

        /// <summary>
        /// 写行为String到Txt日志文件
        /// </summary>
        /// <param name="behaviorStr"></param>
        private void WriteBehaviorToText(string behaviorStr)
        {
            var filePath = Config.BehaviorTxtDownloadPath + string.Format(BehaviorTxtFileName, DateTime.Now.ToString("yyyyMMdd"));

            try
            {
                System.IO.File.AppendAllText(filePath, "\r\n" + behaviorStr, Encoding.UTF8);
                //DownloadHelper.WriteInfo(filePath, behaviorStr);
            }
            catch (Exception ex)
            {
                Log("WriteBehaviorToText Err: " + ex.Message);
            }
            
        }
    }
}
