using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WHotelSite.Models;

namespace WHotelSite.ViewModels
{
    public class InterestListSelectViewModel
    {
        public List<InterestModel> Themes { get; set; }
        public List<InterestModel> Sights { get; set; }
    }
}