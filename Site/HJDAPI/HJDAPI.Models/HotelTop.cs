using System.Collections.Generic;
using HJD.DestServices.Contract;

namespace HJDAPI.Models
{
    public class HotelTop
    {
        public HJDWordKindsEntity Kind { get; set; }
        public List<HotelTopItem> Results { get; set; }
    }
}