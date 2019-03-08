using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HJD.Framework.Entity;

namespace HJD.HotelServices.Implement.Entity
{
    [DefaultColumn]
    public class HotelOTAPrice
    {
        [DBColumn(IsOptional = true)]
        public int HotelId { get; set; }

        [DBColumn(IsOptional = true)]
        public Int16 ChannelID { get; set; }

       [DBColumn(IsOptional = true)]
        public decimal Price { get; set; }
    }
}
