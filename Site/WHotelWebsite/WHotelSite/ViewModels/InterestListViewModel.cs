using HJD.HotelServices.Contracts;
using System.Collections.Generic;
using WHotelSite.Models;

namespace WHotelSite.ViewModels
{
    public class InterestListViewModel
    {
        public int DistrictId { get; set; }

        public string Type { get; set; }
        public List<InterestModel> Interests { get; set; }
        public List<SightCategoryModel> SightCategories { get; set; }
        public List<InterestEntity> ThemeInterestList { get; set; }
    }
}