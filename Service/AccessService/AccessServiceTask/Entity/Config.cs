using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJD.AccessServiceTask.Entity
{
    public class Config
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

        public static string WeixinTemplateMsgTitle
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["WeixinTemplateMsgTitle"];
            }
        }

        public static string WeixinTemplateMsgName
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["WeixinTemplateMsgName"];
            }
        }

        public static string WeixinTemplateMsgDesc
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["WeixinTemplateMsgDesc"];
            }
        }

        public static string WeixinTemplateMsgDesc2
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["WeixinTemplateMsgDesc2"];
            }
        }

        public static string WeixinTemplateMsgLink
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["WeixinTemplateMsgLink"];
            }
        }

        public static string WeixinTemplateMsgRemark
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["WeixinTemplateMsgRemark"];
            }
        }

        public static string WeixinTemplateMsgTestOpenid
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["WeixinTemplateMsgTestOpenid"];
            }
        }
    }
}
