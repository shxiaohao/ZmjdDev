using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WHotelSite.Models
{
    public class HotPackageModel
    {
        public string PicUrl { get; set; }

        public int Pid { get; set; }

        public int HotelId { get; set; }

        public string HotelName { get; set; }

        public Decimal ReviewScore { get; set; }

        public int ReviewCount { get; set; }

        public string Brief { get; set; }

        public int MinPrice { get; set; }

        public string PicSUrl { get; set; }
    }

    public class BindCardEntityEx : Pay.Models.BindCardEntity
    {
        public string PayUrl { get; set; }
    }
}