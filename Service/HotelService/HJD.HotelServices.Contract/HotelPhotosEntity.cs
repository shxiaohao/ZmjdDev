using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJD.HotelServices.Contracts
{
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public class HotelPhotosEntity
    {
        [System.Runtime.Serialization.DataMemberAttribute()]
        public String HotelName { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public int HotelId { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        public List<HotelPhotoEntity> HPList{get;set;}
    }
}
