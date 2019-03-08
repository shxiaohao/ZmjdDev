using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Runtime.Serialization;
using HJD.Framework.Entity;


namespace HJD.HotelServices
{

    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public sealed class NearbyHotelsEntity
    {
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32 POIID
        {
            get;
            set;
        }

     [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32 HotelID
        {
            get;
            set;
        }

     [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
     public Int32 Distance
     {
         get;
         set;
     }


    }
}
