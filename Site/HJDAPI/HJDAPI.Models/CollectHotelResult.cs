using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using HJD.HotelServices.Contracts;

namespace HJDAPI.Models
{
    [Serializable]
    [DataContract]
    public class CollectHotelResult
    {
        /// <summary>
        /// 收藏酒店总数量
        /// </summary>
        [DataMember]
        public int TotalCount { get; set; }

        /// <summary>
        /// 收藏酒店基本信息
        /// </summary>
        [DataMember]
        public List<CollectListItem> Hotels { get; set; }

        /// <summary>
        /// 同步信息
        /// </summary>
        [DataMember]
        public string Message { get; set; }

        /// <summary>
        /// 同步是否成功
        /// </summary>
        [DataMember]
        public int Success { get; set; }
    }
}
