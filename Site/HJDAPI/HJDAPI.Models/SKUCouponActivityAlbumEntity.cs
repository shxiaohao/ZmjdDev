using HJD.CouponService.Contracts.Entity;
using ProductService.Contracts.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    [DataContract]
    public class SKUCouponActivityAlbumEntity
    {
        public SKUCouponActivityAlbumEntity()
        {
            SKUCouponList = new List<SKUCouponActivityEntity>();
            AlbumName = "";
            Description = "";
        }


        [DataMember]
        public string AlbumName { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public int TotalCount { get; set; }
        //[DataMember]
        //public ProductAlbumsEntity ProductAlbum { get; set; }

        [DataMember]
        public List<SKUCouponActivityEntity> SKUCouponList { get; set; }


    }
}
