using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJD.HotelServices.Implement.Business
{
    public class LogHelper
    {
        private static string LogFilePath = @"D:\Log\HotelService\Log_{0}.txt";

        public static bool WriteLog(string errMsg)
        {
            try
            {
                File.AppendAllText(string.Format(LogFilePath, DateTime.Now.ToString("MM_dd")), string.Format("{0}\r\n{1}\r\n", DateTime.Now, errMsg));
            }
            catch(Exception e)
            {

            }
            return true;
        }
    }
}
