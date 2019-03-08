using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJD.HotelServices.Implement.Helper
{
    class HotelTFTReviewCompare :IEqualityComparer<HotelTFTReviewItemEntity>
    {
        public bool Equals(HotelTFTReviewItemEntity x, HotelTFTReviewItemEntity y)
        {
            return x.HotelID == y.HotelID && x.TFTID == y.TFTID && x.Type == y.Type;
        }

        public int GetHashCode(HotelTFTReviewItemEntity obj)
        {
            return  obj == null?0: String.Format("{0}:{1}:{2}",obj.HotelID,obj.TFTID,obj.Type).GetHashCode();
        }
    }
}
