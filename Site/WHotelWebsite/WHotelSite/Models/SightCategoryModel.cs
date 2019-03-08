using System.Collections.Generic;

namespace WHotelSite.Models
{
    public class SightCategoryModel
    {
        public int Id { get; set; }

        public List<int> InterestIds { get; set; }

        public string Name { get; set; }
    }
}