using HJD.Framework.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJD.HotelServices.Contracts
{
    [Serializable]
    [DataContract(IsReference = true)]
    [DefaultColumn]
    public class InspectorHotelSearchParam
    {
        [DataMember]
        [DBColumnAttribute(IsOptional = true)]
        public int PageIndex { get; set; }

        [DataMember]
        [DBColumnAttribute(IsOptional = true)]
        public int PageSize { get; set; }

        [DataMember]
        [DBColumnAttribute(IsOptional = true)]
        public InspectorHotelFilter Filter { get; set; }

        [DataMember]
        [DBColumnAttribute(IsOptional = true)]
        public Dictionary<string, string> Sort { get; set; }
    }

    [Serializable]
    [DataContract(IsReference = true)]
    [DefaultColumn]
    public class InspectorHotelFilter
    {
        [DataMember]
        [DBColumnAttribute(IsOptional = true)]
        public bool? IsValid { get; set; }

        [DataMember]
        [DBColumnAttribute(IsOptional = true)]
        public bool? IsExpired { get; set; }

        [DataMember]
        [DBColumnAttribute(IsOptional = true)]
        public DateTime? MaxLimitTime { get; set; }
    }
}