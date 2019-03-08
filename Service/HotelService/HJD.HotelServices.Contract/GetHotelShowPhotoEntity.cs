using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HJD.Framework.Entity;

namespace HJD.HotelServices.Contracts
{
    [SerializableAttribute()]
    [DefaultColumnAttribute()]
    public  class GetHotelShowPhotoEntity
    {
        public string Url { get; set; }

        public int HotelOriID { get; set; }

        public int HotelID { get; set; }

        [DBColumn(Ignore = true)]
        public string BaseUrl { get; set; }
    }
}
