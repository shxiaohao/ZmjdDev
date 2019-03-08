using HJD.CouponService.Contracts.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    [DataContract]
    public class AlbumSkuListEntity
    {
        [DataMember]
        public int TotalCount { get; set; }

        [DataMember]
        public List<SKUAlbumEntity> SKUList { get; set; }
    }
}
