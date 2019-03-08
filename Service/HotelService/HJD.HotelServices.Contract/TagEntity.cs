using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using HJD.Framework.Entity;

namespace HJD.HotelServices.Contracts
{
    /// <summary>
    /// 特色标签For酒店列表查询参数 | For酒店详情页
    /// </summary>
    [Serializable]
    [DataContract]
    [DefaultColumn]
    public class TagEntity
    {
        /// <summary>
        /// 特色标签编号
        /// </summary>
        [DataMemberAttribute()]
        public int TagID { get; set; }

        /// <summary>
        /// 特色标签名称
        /// </summary>
        [DataMemberAttribute()]
        public string Name { get; set; }

        /// <summary>
        /// 该目的地下包含此标签的酒店数 | 当前酒店提到该标签次数
        /// </summary>
        [DataMemberAttribute()]
        public int Num { get; set; }

    }
}
