using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HJD.Framework.Entity;

namespace HJD.HotelServices
{
    [Serializable]
    [DefaultColumn]
    public class HotelPackageOrderEntity
    {

        [DBColumn(DefaultValue = "0")]
        public long ID { get; set; }

        [DBColumn(DefaultValue = "0")]
        public int PID { get; set; }

        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public DateTime CheckIn { get; set; }

        [DBColumn(DefaultValue = "0")]
        public int NightCount { get; set; }

        [DBColumn(DefaultValue = "0")]
        public int RoomCount { get; set; }

    }
}

