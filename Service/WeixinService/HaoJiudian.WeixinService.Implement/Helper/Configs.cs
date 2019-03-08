using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJD.WeixinService.Implement.Helper
{
    public class Configs
    {
        public static string WeiXinAPPID
        {
            get { return ConfigurationManager.AppSettings["WeiXinAPPID"]; }
        }
        public static string WeiXinSecret
        {
            get { return ConfigurationManager.AppSettings["WeiXinSecret"]; }
        }
    }

    public class RabbitmqConfig
    {

        public static string RabbitmqHostName
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["RabbitmqHostName"];
            }
        }

        public static string RabbitmqUserName
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["RabbitmqUserName"];
            }
        }

        public static string RabbitmqPassword
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["RabbitmqPassword"];
            }
        }

        public static string BehaviorTxtDownloadPath
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["BehaviorTxtDownloadPath"];
            }
        }

        public static string BehaviorCsvDownloadPath
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["BehaviorCsvDownloadPath"];
            }
        }

        public static string WeixinChatRecordTxtDownloadPath
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["WeixinChatRecordTxtDownloadPath"];
            }
        }

        public static string WeixinChatRecordCsvDownloadPath
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["WeixinChatRecordCsvDownloadPath"];
            }
        }
    }
}
