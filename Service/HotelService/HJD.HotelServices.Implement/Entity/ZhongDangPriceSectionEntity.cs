using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HJD.Framework.Entity;

namespace HJD.HotelServices.Implement.Entity
{
    [DefaultColumn]
    public class ZhongDangPriceSectionEntity
    {
        [DBColumn(IsOptional = true)]
        public int DistrictID { get; set; }

        [DBColumn(IsOptional = true)]
        public float MinPrice { get; set; }

       [DBColumn(IsOptional = true)]
        public float MaxPrice { get; set; }

       [DBColumn(IsOptional = true)]
       public bool InChina { get; set; }
    }
}
