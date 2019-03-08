using HJD.AccessService.Contract.Model;
using HJD.AccessService.Contract.Params;
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

namespace HJD.AccessServiceTask.Job
{
    /// <summary>
    /// 下载微信多客服会话消息
    /// </summary>
    public class DownloadWeixinChatRecoedJob : BaseJob
    {
        static string RecordTxtFileName = "record_{0}.txt";

        public DownloadWeixinChatRecoedJob()
            : base("DownloadWeixinChatRecoedJob")
        {
            Log(string.Format("Start Job [DownloadWeixinChatRecoedJob]"));

            try
            {
                RunJob();
            }
            catch (Exception ex)
            {
                Log("Job [DownloadWeixinChatRecoedJob] Error:" + ex.Message);
            }


            Log(string.Format("Stop Job [DownloadWeixinChatRecoedJob]"));
        }

        private void RunJob()
        {
            var weixinApi = new Weixin();

            var getDate = DateTime.Now.AddDays(-10).Date;//DateTime.Parse("2015-04-01");
            while (getDate < DateTime.Now.Date)
            {
                Log("GetDate:" + getDate);
                Console.WriteLine("GetDate:" + getDate);

                var wrp = new WeixinChatRecordParams
                {
                    starttime = TimeHelper.ConvertDateTimeInt(getDate),
                    endtime = TimeHelper.ConvertDateTimeInt(getDate.AddDays(1).AddSeconds(-1)),
                    openid = "",
                    pagesize = 1000,
                    pageindex = 1
                };

                var chatRecordResult = weixinApi.GetWeixinChatRecord(wrp);
                if (chatRecordResult != null && chatRecordResult.recordlist != null && chatRecordResult.recordlist.Count > 0)
                {
                    Log("Record Count:" + chatRecordResult.recordlist.Count);
                    Console.WriteLine("Record Count:" + chatRecordResult.recordlist.Count);

                    for (int i = 0; i < chatRecordResult.recordlist.Count; i++)
                    {
                        var record = chatRecordResult.recordlist[i];
                        var recordString = HJD.AccessService.Implement.Helper.WeixinHelper.FormatWeixinChatRecord(record);
                        WriteRecordToText(recordString, getDate);
                    }
                }
                else
                {
                    Log(string.Format("Record Is Null Or Count Is 0 [{0}]", getDate));
                    Console.WriteLine(string.Format("Record Is Null Or Count Is 0 [{0}]", getDate));
                }

                getDate = getDate.AddDays(1);
            }
        }

        /// <summary>
        /// 写行为String到Txt日志文件
        /// </summary>
        /// <param name="behaviorStr"></param>
        private void WriteRecordToText(string recordStr, DateTime getDate)
        {
            var filePath = Config.WeixinChatRecordTxtDownloadPath + string.Format(RecordTxtFileName, getDate.ToString("yyyyMMdd"));

            try
            {
                System.IO.File.AppendAllText(filePath, "\r\n" + recordStr, Encoding.Default);
                //DownloadHelper.WriteInfo(filePath, behaviorStr);
            }
            catch (Exception ex)
            {
                Log("WriteBehaviorToText Err: " + ex.Message);
            }
            
        }
    }
}
