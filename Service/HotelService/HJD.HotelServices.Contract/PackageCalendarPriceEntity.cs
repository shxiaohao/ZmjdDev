using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJD.HotelServices.Contracts
{
    [DataContract]
    [Serializable]
    public class PackageCalendarPriceEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int ID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int HotelId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int PackageId { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int ChannelID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public DateTime Date { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int Price { get; set; }
    }
}
