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
    /// PRateMatchOta
    /// </summary>
    public class PRateMatchOtaEntity
    {
        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public int PID { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public DateTime Date { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public int PriceSource { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public int Price { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public int SellState { get; set; }
    }
}
