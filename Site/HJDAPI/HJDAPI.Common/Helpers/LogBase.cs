using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Common.Helpers
{
    public class LogBase
    {
        private static string LogFilePath = @"D:\Log\API\Log_{0}.txt";

        public static bool WriteLog(string errMsg)
        {
            try
            {
                File.AppendAllText(string.Format(LogFilePath, DateTime.Now.ToString("MM_dd")), string.Format("{0}\r\n{1}\r\n", DateTime.Now, errMsg));
            }
            catch { }
            return true;
        }

        private static string AppErrorLogFilePath = @"D:\Log\API\Log_AppError_{0}.txt";

        public static bool WriteAppErrorLog(string errMsg)
        {
            try
            {
                File.AppendAllText(string.Format(AppErrorLogFilePath, DateTime.Now.ToString("MM_dd")), string.Format("{0}\r\n{1}\r\n", DateTime.Now, errMsg));
            }
            catch { }
            return true;
        }


        private static string MagiCallLogFilePath = @"D:\Log\API\MagiCallLog_{0}_{1}.txt";

        public static bool WriteMagiCallLog(string type, string errMsg)
        {
            try
            {
                File.AppendAllText(string.Format(MagiCallLogFilePath, type, DateTime.Now.ToString("MM_dd")), string.Format("{0}\r\n{1}\r\n", DateTime.Now, errMsg));
            }
            catch { }
            return true;
        }
    }
}
