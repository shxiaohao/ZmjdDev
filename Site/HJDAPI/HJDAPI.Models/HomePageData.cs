using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    public class HomePageData
    {
       public  HotPackageInfo topPackage { get; set; }
       public List<HotPackageInfo> Preferential { get; set; }

       public InterestModel2 InterestData { get; set; }
    }


    public class HotPackageInfo
    {
        public string PicURL { get; set; }

        public int PID { get; set; }

        public int HotelID { get; set; }

        public string HotelName { get; set; }

        public decimal ReviewScore { get; set; }

        public int ReviewCount { get; set; }

        public string Brief { get; set; }

        public int MinPrice { get; set; }

        public string PicSUrl { get; set; }
    }
}
