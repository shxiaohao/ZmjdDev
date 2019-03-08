using HJD.AccessServiceTask.Job;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJD.AccessServiceTask
{
    class Program
    {
        static void Main(string[] args)
        {
            BaseJob baseJob = new BaseJob();

            var jobName = "DownLoadWexinUserInfoJob";
            if (args != null && args.Length > 0) { jobName = args[0]; }
            if (jobName == "")
            {
                BaseJob.Log("没有指明要执行的任务名称");
                return;
            }

            Console.WriteLine(string.Format("{0} Start Run {1}", DateTime.Now.ToString(), jobName));
            RunJob(jobName);
        }

        static void RunJob(string jobName)
        {
            BaseJob baseJob = new BaseJob();

            try
            {
                Type type = typeof(BaseJob);
                type.GetMethod(jobName).Invoke(baseJob, new object[0] { });
            }
            catch (Exception ex)
            {
                BaseJob.Log("Job Error:" + ex.Message);
                BaseJob.Log(string.Format("Stop Job [{0}]", jobName));
            }
        }
    }
}
