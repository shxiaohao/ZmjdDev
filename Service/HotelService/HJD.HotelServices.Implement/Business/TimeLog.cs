using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJD.HotelServices.Implement.Business
{
    public class TimeLog:LogHelper
    {
        long mMaxMilliseconds = 200;
        long mTotalMilliseconds = 0;
        string mFunctionName = "";
        public string logPreString = "";
        Stopwatch sw = new Stopwatch();
        StringBuilder sb = new StringBuilder();
        TimeLog mParentLog = null;
        public TimeLog(string functionName, long maxMilliseconds, TimeLog parentLog = null)
        {
            mMaxMilliseconds = maxMilliseconds;
            mFunctionName = functionName;
            mParentLog = parentLog;
            if (parentLog != null)
            {
                logPreString = parentLog.logPreString + "   .";
            }
            sb.AppendLine(string.Format("{0} Start:{1}", logPreString + functionName, DateTime.Now));
            sw.Start();
        }

        public void AddLog(string nodeName)
        {
            sw.Stop();
            mTotalMilliseconds += sw.ElapsedMilliseconds;
            sb.AppendLine(string.Format("{0}:{1}", logPreString + nodeName, sw.ElapsedMilliseconds));
            sw.Restart();
        }

        public void AddChildLog(string msg)
        {
            sb.AppendLine(msg);
        }

        public void Finish()
        {
            sw.Stop();
            sb.AppendLine(string.Format("{2}Finished. Total MilliSeconds:{0}  {1}", mTotalMilliseconds, DateTime.Now.ToString(), logPreString));
            if (mParentLog != null)
            {
                mParentLog.AddChildLog(sb.ToString());
            }
            else if (mTotalMilliseconds > mMaxMilliseconds)
            {
                WriteLog(sb.ToString());
            }
        }
    }
}
