﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJD.WeixinService.Implement.Helper
{
     public class LogHelper
    {
        private static string LogFilePath = @"D:\Log\API\Log_WinXinSvc_{0}.txt";

        public static bool WriteLog(string errMsg)
        {
            File.AppendAllText(string.Format(LogFilePath, DateTime.Now.ToString("MM_dd")), string.Format("{0}\r\n{1}\r\n", DateTime.Now, errMsg));
            return true;
        }
    }
}
