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
    /// 酒店价格段
    /// </summary>
    public class HotelPriceSlot
    {
        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public Int64 Id { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public int HotelId { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public DateTime Night { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public bool Slot_0_400 { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public bool Slot_0_600 { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public bool Slot_0_800 { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public bool Slot_0_1000 { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public bool Slot_0_1500 { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public bool Slot_0_2000 { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public bool Slot_0_0 { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public bool Slot_400_600 { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public bool Slot_400_800 { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public bool Slot_400_1000 { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public bool Slot_400_1500 { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public bool Slot_400_2000 { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public bool Slot_400_0 { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public bool Slot_600_800 { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public bool Slot_600_1000 { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public bool Slot_600_1500 { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public bool Slot_600_2000 { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public bool Slot_600_0 { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public bool Slot_800_1000 { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public bool Slot_800_1500 { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public bool Slot_800_2000 { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public bool Slot_800_0 { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public bool Slot_1000_1500 { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public bool Slot_1000_2000 { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public bool Slot_1000_0 { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public bool Slot_1500_2000 { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public bool Slot_1500_0 { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public bool Slot_2000_0 { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public string Prices { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public int MinPrice { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public int MaxPrice { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public int ChannelId { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public int SellState { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public DateTime CreateTime { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public DateTime UpdateTime { get; set; }
    }
}
