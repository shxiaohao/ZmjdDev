using HJD.Contracts;
using HJD.Entity;
using HJD.Framework.WCF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Controllers.Adapter
{
    public class JobAssistantAdapter
    {


        public static IJobAssistantService jobAssistantService = ServiceProxyFactory.Create<IJobAssistantService>("IJobAssistantService");

        public static void SetParameter(string jobName, string pConfigKey, string pConfigValue)
        {
            jobAssistantService.SetParameter(jobName, pConfigKey, pConfigValue);
        }
        public static List<JobAssistantParameterEntity> GetParameter(string jobName)
        {
            return jobAssistantService.GetParameter(jobName);
        }
    }
}
