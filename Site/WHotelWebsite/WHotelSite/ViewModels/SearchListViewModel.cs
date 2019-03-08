using System.Collections.Generic;
using WHotelSite.Models;

namespace WHotelSite.ViewModels
{
    public class SearchListViewModel
    {
        public List<HotelModel> Hotels { get; set; }

        public string Keyword { get; set; }
    }
}