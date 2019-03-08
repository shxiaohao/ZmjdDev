using HJD.AccessServiceTask.Job.Acount;
using HJD.AccessServiceTask.Job.Fund;
using HJD.AccessServiceTask.Job.Search;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJD.AccessServiceTask.Job
{
    public class BaseJob
    {
        #region Base

        public BaseJob() { }
        public BaseJob(string job) { jobName = job; Console.Title = job; }
         
        private static string jobName = "BaseJob";
        private static string logFile = "";

        public static void Log(string log)
        { 
            Log(log, "");
        }
        public static void Log(string log, string addFileName)
        {
            try
            {
                var Log_Path = "";
                string[] commands = Environment.CommandLine.Replace("\"", "").Split(new char[] { ' ' });
                if (commands.Length > 0)
                {
                    Log_Path = Path.GetFullPath(commands[0]).Replace(Path.GetFileName(commands[0]), "");
                }

                var rootUrl = Environment.CurrentDirectory;
                //logFile = string.Format("d:\\LOG\\OtaData\\task\\{0}\\Log_{1}{2}.txt", jobName, DateTime.Now.ToString("yyyyMMdd"), (!string.IsNullOrEmpty(addFileName) ? "_" + addFileName : ""));
                logFile = string.Format(Log_Path + "Log\\Log_{1}{2}.txt", jobName, DateTime.Now.ToString("yyyyMMdd"), (!string.IsNullOrEmpty(addFileName) ? "_" + addFileName : ""));
                System.IO.File.AppendAllText(logFile, "\r\n" + string.Format("[{0}] ", DateTime.Now.ToString()) + log, Encoding.Default);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Log Error:" + ex.Message);
            }
        }

        #endregion

        #region All Job

        #region Acount

        public void DownloadBehaviorQueueJob()
        {
            new DownloadBehaviorQueueJob();
        }

        public void GenBehaviorCsvJob()
        {
            new GenBehaviorCsvJob();
        }

        public void GenUserRecommendRelJob()
        {
            new GenUserRecommendRelJob();
        }

        #endregion

        #region Fund

        public void GenUserFundJob()
        {
            new GenUserFundJob();
        }

        public void GenVipUserFundJob()
        {
            new GenVipUserFundJob();
        }

        #endregion

        #region Weixin

        public void DownloadWeixinChatRecoedJob()
        {
            new DownloadWeixinChatRecoedJob();
        }

        public void GenActiveLuckCodeListener()
        {
            new GenActiveLuckCodeListener();
        }

        public void GenActiveGemListener()
        {
            new GenActiveGemListener();
        }

        public void DownLoadWexinUserInfoJob()
        {
            new DownLoadWexinUserInfoJob();
        }

        public void DownLoadWexinMaterialJob()
        {
            new DownLoadWexinMaterialJob();
        }

        public void DownLoadWexinLiuwaMaterialJob()
        {
            new DownLoadWexinLiuwaMaterialJob();
        }

        public void SendServiceTemplateMsgSZ_Job()
        {
            new SendServiceTemplateMsgSZ_Job();
        }

        public void SendServiceTemplateMsgShanglvzm_Job()
        {
            new SendServiceTemplateMsgShanglvzm_Job();
        }

        public void SendServiceTemplateMsgLiuwa_Job()
        {
            new SendServiceTemplateMsgLiuwa_Job();
        }

        public void SendServiceTemplateMsgLiuwaWX_Job()
        {
            new SendServiceTemplateMsgLiuwaWX_Job();
        }

        public void SendRedPackJob()
        {
            new SendRedPackJob();
        }

        #endregion

        #region Search

        public void BuildHotelIndexJob()
        {
            new BuildHotelIndexJob();
        }

        public void IndexActionListener()
        {
            new IndexActionListener();
        }

        public void HotelIndexSourceListener()
        {
            new HotelIndexSourceListener();
        }

        public void HanlpDemoJob()
        {
            new HanlpDemoJob();
        }

        #endregion

        #region Qa Search

        public void BuildQaIndexJob()
        {
            new BuildQaIndexJob();
        }

        #endregion

        #region Hotel

        public void HotelPriceSlotEngine()
        {
            new HotelPriceSlotEngine();
        }

        public void HotelPriceSlotListener()
        {
            new HotelPriceSlotListener();
        }

        public void HotelPriceSlotListenerForBg()
        {
            new HotelPriceSlotListenerForBg();
        }

        public void ClearHotelPriceSlot()
        {
            new ClearHotelPriceSlot();
        }

        public void InitHotelBasePrice()
        {
            new InitHotelBasePrice();
        }

        public void FillHotelPricePlanExJob()
        {
            new FillHotelPricePlanExJob();
        }

        public void UpdateZmjdPackagePrice()
        {
            new UpdateZmjdPackagePrice();
        }

        #endregion

        #region Tool

        public void CustomSmsSendJob()
        {
            new CustomSmsSendJob();
        }

        #endregion

        #endregion
    }
}
