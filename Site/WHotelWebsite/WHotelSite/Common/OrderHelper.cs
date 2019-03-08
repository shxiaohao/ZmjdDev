using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WHotelSite.Common
{
    public class OrderHelper
    {
        /// <summary>
        /// 判断订单号是酒店订单号还是通用订单号
        /// </summary>
        /// <param name="orderid"></param>
        /// <returns></returns>
        public static bool IsHotelOrder(long orderid)
        {
            return orderid > 100000000; 
        }
    }
}