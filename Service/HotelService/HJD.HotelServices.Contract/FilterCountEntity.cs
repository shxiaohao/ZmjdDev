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
    public class FilterCountEntity
    {
        /// <summary>
        /// 特色标签编号
        /// </summary>
        [DataMemberAttribute()]
        public int ID { get; set; }



        /// <summary>
        /// 特色标签名称
        /// </summary>
        [DataMemberAttribute()]
        public string Name { get; set; }


        [DataMemberAttribute()]
        public int Count { get; set; }

        [DataMemberAttribute()]
        public int Type { get; set; }
    }
}
