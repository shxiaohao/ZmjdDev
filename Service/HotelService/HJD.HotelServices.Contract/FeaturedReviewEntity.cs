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
    public class FeaturedReviewEntity
    {
        /// <summary>
        /// 特色标签编号
        /// </summary>
        [DataMemberAttribute()]
        public int FID { get; set; }

        [DataMemberAttribute()]
        public int TID { get; set; }

        [DataMemberAttribute()]
        public int HotelID { get; set; }

        [DataMemberAttribute()]
        public int Writing { get; set; }

        //[DataMemberAttribute()]
        //public string aboutWords { get; set; }

    }
}
