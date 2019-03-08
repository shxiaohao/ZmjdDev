using HJD.AccessService.Contract.Model;
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
    /// 生成用户行为日志的CSV文件
    /// </summary>
    public class GenBehaviorCsvJob : BaseJob
    {
        static string QueueKey = "BehaviorQueue";
        static string BehaviorTxtFileName = "behavior_{0}.Txt";
        static string BehaviorCsvFileName = "behavior_{0}.csv";
        static string GetAndSetDateStr = DateTime.Now.ToString("yyyyMMdd");

        public GenBehaviorCsvJob()
            : base("GenBehaviorCsvJob")
        {
            Log(string.Format("Start Job [GenBehaviorCsvJob]"));

            try
            {
                RunJob();
            }
            catch (Exception ex)
            {
                Log("Job [GenBehaviorCsvJob] Error:" + ex.Message);
            }


            Log(string.Format("Stop Job [GenBehaviorCsvJob]"));
        }

        private void RunJob()
        {
            //是否指定生成日期，不指定默认今天
            Console.WriteLine("请输入要生成的日期（如20170801），默认今天：");
            var dateStr = Console.ReadLine();
            if (!string.IsNullOrEmpty(dateStr) && dateStr.Length == 8)
            {
                GetAndSetDateStr = dateStr;
            }

            var rPath = Config.BehaviorTxtDownloadPath + string.Format(BehaviorTxtFileName, GetAndSetDateStr);
            Console.WriteLine("读取Txt：" + rPath);
            Log("读取Txt：" + rPath);

            var behaviorString = DownloadHelper.ReadInfo(rPath);
            if (!string.IsNullOrEmpty(behaviorString))
            {
                var objList = Regex.Split(behaviorString, "\r\n");
                if (objList != null)
                {
                    var behaviorList = new List<Behavior>();

                    foreach (var beStr in objList)
                    {
                        var behavior = BehaviorHelper.GetBehaviorByString(beStr);
                        behaviorList.Add(behavior);
                    }

                    Log("WriteBehaviorList Count:" + behaviorList.Count);
                    Console.WriteLine("WriteBehaviorList Count:" + behaviorList.Count);

                    WriteBehaviorListCsv(behaviorList);
                }
            }
            else
            {
                Log("Reader BehaviorTxtDownloadPath Is Null");
                Console.WriteLine("Reader BehaviorTxtDownloadPath Is Null");
            }
        }

        private void WriteBehaviorListCsv(List<Behavior> behaviorList)
        {
            var filePath = Config.BehaviorCsvDownloadPath + string.Format(BehaviorCsvFileName, GetAndSetDateStr);

            Console.WriteLine("输出Csv：" + filePath);
            Log("输出Csv：" + filePath);

            StreamWriter SW = new StreamWriter(filePath, false, Encoding.Default);

            try
            {
                StringBuilder str = new StringBuilder("ID,Page,Code,Event,Value,UserId,Phone,AppKey,IP,RecordLayer,ClientType,AppVer,RecordTime\r\n");

                string rowStr = "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}\r\n";

                //写数据行
                for (int i = 0; i < behaviorList.Count; i++)
                {
                    var behavior = behaviorList[i];
                    str.Append(string.Format(rowStr
                        , behavior.ID
                        , behavior.Page
                        , behavior.Code
                        , behavior.Event
                        , behavior.Value != null ? behavior.Value.Replace(",", "，") : ""
                        , behavior.UserId
                        , behavior.Phone
                        , behavior.AppKey
                        , behavior.IP
                        , behavior.RecordLayer
                        , behavior.ClientType
                        , behavior.AppVer
                        , behavior.RecordTime.ToString("yyyy-MM-dd HH:mm:ss.ff")
                        ));
                }

                SW.WriteLine(str.ToString());
                SW.Flush();
                SW.Close();
                return;
            }
            catch (Exception e)
            {
                SW.Flush();
                SW.Close();
            }
        }
    }
}
