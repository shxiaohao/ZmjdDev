using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HJD.AccessServiceTask.Job.Helper
{
    public class TimeHelper
    {
        public static List<DateTime> GetWeekTimeList(DateTime startTime, int sumDays)
        {
            var weekTimeList = new List<DateTime>();

            var today = startTime;
            var twoMonthDay = today.AddDays(sumDays);
            DateTime fromTime = today;
            DateTime toTime = twoMonthDay;

            //TimeSpan得到fromTime和toTime的时间间隔
            TimeSpan ts = toTime.Subtract(fromTime);

            //获取两个日期间的总天数
            long countday = ts.Days;

            //循环用来扣除总天数中的双休日  
            for (int i = 0; i < countday; i++)
            {
                DateTime tempdt = fromTime.Date.AddDays(i + 1);
                if (tempdt.DayOfWeek == System.DayOfWeek.Friday || tempdt.DayOfWeek == System.DayOfWeek.Saturday)
                {
                    weekTimeList.Add(tempdt);
                }
            }

            return weekTimeList;
        }

        public static int ConvertDateTimeInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }
    }
}
