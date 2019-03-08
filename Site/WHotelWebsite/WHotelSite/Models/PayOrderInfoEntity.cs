using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WHotelSite.Models
{
    public class PayOrderInfoEntity
    {
        public string payChannelType { get; set; }

        public long UserID { get; set; }
        public long OrderID { get; set; }
        public int OrderState { get; set; }

        public string MobilePhone { get; set; }
        public long CID { get; set; }
        public string payUrl { get; set; }
        public string completeUrl { get; set; }
        public string cancelUrl { get; set; }
        public string orderName { get; set; }
        public Decimal orderPrice { get; set; }
        public Decimal orderPoints { get; set; }

        public string productUrl { get; set; }
        public string successIdentifier { get; set; }//第三方支付成功URL
        public string failedIdentifier { get; set; }//第三方支付失败URL


    }
}