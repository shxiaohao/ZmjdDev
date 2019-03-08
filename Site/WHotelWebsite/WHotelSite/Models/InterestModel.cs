using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WHotelSite.Models
{
    public class InterestModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Type { get; set; }

        public double GLat { get; set; }

        public double GLon { get; set; }

        public string ImageUrl { get; set; }

        public int HotelCount { get; set; }

        public string HotelList { get; set; }

        public string InterestPlaceIDs { get; set; }
    }
}