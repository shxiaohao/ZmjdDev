using HJD.Framework.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJD.AccessService.Contract.Model.Hotel
{
    [Serializable]
    [DataContract]
    /// <summary>
    /// 酒店套餐
    /// </summary>
    public class HotelPackageEntity
    {
        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public Int64 Id { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public int HotelId { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public string Code { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public int RoomID { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public string Brief { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public string SerialNO { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public int PriceSource { get; set; }
    }
}
