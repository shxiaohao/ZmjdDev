using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using HJD.Framework.Entity;

namespace HJD.HotelServices.Contracts
{
    public class ValuedHotelEntity : IComparable
    {
        [DataMember]
        [DBColumn]
        public double GoodReviewNum { get; set; }

        [DataMember]
        [DBColumn]
        public decimal AllReviewNum { get; set; }

        [DataMember]
        [DBColumn]
        public int HotelId { get; set; }

        [DataMember]
        [DBColumn]
        public string HotelName { get; set; }

        [DataMember]
        [DBColumn]
        public int DistrictId { get; set; }

        [DataMember]
        [DBColumn]
        public int Rank { get; set; }

        [DataMember]
        [DBColumn]
        public decimal Price { get; set; }

        [DataMember]
        [DBColumn]
        public decimal Score { get; set; }

        [DataMember]
        [DBColumn(IsOptional = true)]
        public double WritingRate { get; set; }

        [DataMember]
        [DBColumn(IsOptional = true)]
        public double RankRate { get; set; }

        [DataMember]
        [DBColumn(IsOptional = true)]
        public double FinalScore { get; set; }

        public int CompareTo(object obj)
        {
            if (obj is ValuedHotelEntity)
            {
                ValuedHotelEntity tempInfo = obj as ValuedHotelEntity;
                return tempInfo.FinalScore.CompareTo(this.FinalScore);
            }
            throw new NotImplementedException("obj is not a HotelInfo!");

        }
    }
}
