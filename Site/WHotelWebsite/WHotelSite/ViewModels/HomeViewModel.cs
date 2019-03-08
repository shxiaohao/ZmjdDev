using System.Collections.Generic;
using WHotelSite.Models;

namespace WHotelSite.ViewModels
{
    public class HomeViewModel
    {
        public int DistrictId { get; set; }
        public List<SimpleDistrictModel> Districts { get; set; }

        public HotPackageModel TopHotPackage { get; set; }

        public List<HotPackageModel> HotPackages { get; set; }

        public List<InterestModel> Themes { get; set; }

        public List<InterestModel> Sights { get; set; }
        public List<SightCategoryModel> SightCategories { get; set; }
    }
}