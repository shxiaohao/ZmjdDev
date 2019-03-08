using System.Collections.Generic;

namespace HJDAPI.Models
{
    public class QuickSearchIdsResult
    {
        public List<int> HotelIds { get; set; }
        public List<int> SightIds { get; set; }
        public List<int> QaIds { get; set; }
        public List<int> AllIds { get; set; }
    }
}