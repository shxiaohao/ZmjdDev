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
    /// PackageNotMatchOta
    /// </summary>
    public class PackageNotMatchOta
    {
        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public int IDX { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public int HotelID { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public int PID { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public string Code { get; set; }

        [DataMemberAttribute()]
        [DBColumn(IsOptional = true)]
        public string Brief { get; set; }
    }
}
