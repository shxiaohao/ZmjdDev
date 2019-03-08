using HJD.CouponService.Contracts.Entity;
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
    public class GetRetailProductListResult
    {
        [DataMember]
        public int pageSize { get; set; }
        [DataMember]
        public int start { get; set; }
        [DataMember]
        public int totalCount { get; set; }

        [DataMember]
        public List<int> ParamType { get; set; }

        [DataMember]
        public List<CouponActivityRetailEntity> list { get; set; }

    }
}
