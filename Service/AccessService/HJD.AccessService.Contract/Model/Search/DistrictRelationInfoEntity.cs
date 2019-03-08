using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJD.AccessService.Contract.Model
{
    [Serializable]
    [DataContract]
    /// <summary>
    /// 目的地相关信息数据模型
    /// </summary>
    public class DistrictRelationInfoEntity
    {
        [DataMemberAttribute()]
        public int DistrictId;

        [DataMemberAttribute()]
        public int HotelCount;

        [DataMemberAttribute()]
        public int InterestId;
    }
}
