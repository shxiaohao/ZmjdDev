using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJD.HotelServices.Contracts
{

    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public sealed class PDayPItem
    {
        [System.Runtime.Serialization.DataMemberAttribute()]
        public Int32 PID{ get; set; } //套餐ID

        [System.Runtime.Serialization.DataMemberAttribute()]
        public DateTime Day { get; set; } //天

        [System.Runtime.Serialization.DataMemberAttribute()]
        public Int32 SealState { get; set; } // 可售卖套餐状态 0：不可卖 1：可卖

        [System.Runtime.Serialization.DataMemberAttribute()]
        public Int32 SoldCount { get; set; } // 已售卖套餐数


        [System.Runtime.Serialization.DataMemberAttribute()]
        public Int32 MaxSealCount { get; set; } //总可售卖套餐数
        
    }
}
