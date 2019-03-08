using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WHotelSite.Models
{
   public class DistrictSuggestModel
    {
        public string Group { get; set; }

        public List<SimpleDistrictModel> List { get; set; }
    }
}