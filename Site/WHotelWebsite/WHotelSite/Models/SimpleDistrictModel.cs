using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WHotelSite.Models
{
    public class SimpleDistrictModel
    {
        public int DistrictId { get; set; }
        public string Name { get; set; }
        public string PinYin { get; set; }
        public string FirstLetter { get; set; }

        public double Lat { get; set; }

        public double Lng { get; set; }
    }
}