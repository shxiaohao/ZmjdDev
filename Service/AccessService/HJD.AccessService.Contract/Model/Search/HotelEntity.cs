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
    /// 酒店基础模型
    /// </summary>
    public class HotelEntity
    {
        [DataMemberAttribute()]
        public int HotelId;

        [DataMemberAttribute()]
        public string HotelName;

        [DataMemberAttribute()]
        public string Enabled;

        [DataMemberAttribute()]
        public string TimeStampCol;
    }
}
