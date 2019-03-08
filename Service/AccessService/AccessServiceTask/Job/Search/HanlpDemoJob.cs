using HJD.AccessService.Contract.Model;
using HJD.AccessService.Implement;
using HJD.AccessService.Implement.Helper;
using HJD.AccessServiceTask.Entity;
using HJD.AccessServiceTask.Job.Helper;
using HJDAPI.APIProxy;
using HtmlAgilityPack;
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
using HJD.Search.CommonLibrary.Engine;

namespace HJD.AccessServiceTask.Job.Search
{
    /// <summary>
    /// 生成酒店相关索引库
    /// </summary>
    public class HanlpDemoJob : BaseJob
    {
        public HanlpDemoJob()
            : base("HanlpDemoJob")
        {
            Log(string.Format("Start Job [HanlpDemoJob]"));

            try
            {
                RunJob();
            }
            catch (Exception ex)
            {
                Log("Job [HanlpDemoJob] Error:" + ex.Message);
            }


            Log(string.Format("Stop Job [HanlpDemoJob]"));
        }

        private void RunJob()
        {
            HelloJava();
        }

        public void HelloJava()
        {

            NLPEngine.HanlpDemo("苏州凯宾斯基度假酒店周末有房吗?");


            //List list = HanLP.segment("你好java");

            //Suggester suggester = new Suggester();
            //suggester.addSentence("苏州御庭酒店现在有房间");

            //var result = suggester.suggest("空房", 1);

        }
    }
}
