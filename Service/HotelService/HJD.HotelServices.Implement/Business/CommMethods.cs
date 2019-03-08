using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJD.HotelServices.Implement.Business
{
    public class CommMethods
    {
        public static int GetNightCount(DateTime CheckIn, DateTime CheckOut)
        {
            TimeSpan ts1 = new TimeSpan(CheckIn.Date.Ticks);
            TimeSpan ts2 = new TimeSpan(CheckOut.Date.Ticks);
            TimeSpan ts = ts1.Subtract(ts2).Duration();
            return ts.Days;
        }
    }
}
