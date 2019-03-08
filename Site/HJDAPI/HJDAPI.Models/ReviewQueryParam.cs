using HJD.HotelServices.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    public class ReviewQueryParam
    {
        public int Hotel { get; set; }
        public int Start { get; set; }
        public int Count { get; set; }
        public RatingType RatingType { get; set; }
        public int InterestID { get; set; }
        public int TFTType { get; set; }
        public int TFTID { get; set; }
        public long UserID { get; set; }
    }
}
