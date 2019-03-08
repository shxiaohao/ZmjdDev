using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    [Serializable]
    [DataContract]
    public sealed class EasternAirLinesRecord
    {

        #region 映射字段
        [DataMember]
        private int id { get; set; }

        [DataMember]
        private string accountno { get; set; }

        [DataMember]
        private int accounttype { get; set; }

        [DataMember]
        private string othername { get; set; }

        [DataMember]
        private Double points { get; set; }

        [DataMember]
        private long userid { get; set; }

        [DataMember]
        private DateTime consumedate { get; set; }

        [DataMember]
        private DateTime createtime { get; set; }

        [DataMember]
        private int dealstate { get; set; }

        [DataMember]
        private int states { get; set; }

        [DataMember]
        private int? sourcetype { get; set; }

        [DataMember]
        private DateTime? updatetime { get; set; }

        [DataMember]
        private string description { get; set; }

        [DataMember]
        private string batchid { get; set; }

        [DataMember]
        private long businessid { get; set; }
        #endregion
    }
}
