using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJD.Search.CommonLibrary.Helper
{
    public class LogHelper
    {
        public static void WriteLog(string msg)
        {
            System.IO.File.AppendAllText(string.Format(@"D:\Log\AccessService\AccessLog_{0}.txt", DateTime.Now.ToString("MM_dd")), "\r\n=====" + msg);
    
        }

        public static void WriteConsole(string msg)
        {
            Console.WriteLine(string.Format("{0} {1}", msg, DateTime.Now));
        }
    }
}
