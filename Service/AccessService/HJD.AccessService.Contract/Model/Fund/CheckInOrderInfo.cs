using HJD.Framework.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJD.AccessService.Contract.Model.Fund
{
    [Serializable]
    [DataContract]
    /// <summary>
    /// 
    /// </summary>
    public class CheckInOrderInfo
    {
        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public Int32 Id { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public Int64 OrderID { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public decimal Amount { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public DateTime SubmitDate { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public DateTime CheckIn { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public Int32 NightCount { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public Int64 UserID { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public Int32 HotelID { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public string HotelName { get; set; }
    }
}
