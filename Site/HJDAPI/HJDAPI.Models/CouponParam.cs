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
    public class CouponModelParam
    {
        [DataMember]
        public long id { get; set; }
        [DataMember]
        public string guid { get; set; }
        [DataMember]
        public long userId { get; set; }
        [DataMember]
        public long sourceId { get; set; }
        [DataMember]
        public int typeId { get; set; }

        /// <summary>
        /// 以分为单位
        /// </summary>
        [DataMember]
        public int couponAmount{ get; set; }
        [DataMember]
        public bool isBudget { get; set; }
        [DataMember]
        public int state { get; set; }
        [DataMember]
        public string phoneNo { get; set; }
    }

    [Serializable]
    [DataContract]
    public class ShareCommentCouponParam
    {
        [DataMember]
        public long userId { get; set; }
        [DataMember]
        public long sourceId { get; set; }
        [DataMember]
        public string phoneNo { get; set; }
    }

    public class PItemRelItemEntity
    {
        public int Sort { get; set; }
        public int HotelID { get; set; }

        public int PID { get; set; }

        public int dateType { get; set; }

        public DateTime date { get; set; }
        public string Description { get; set; }
        public int ID { get; set; }
        public string ItemCode { get; set; }
        public Decimal Price { get; set; }
        public int Type { get; set; }

        /// <summary>
        /// 项目产品类型，1： 酒店项目， 2：供应商项目
        /// </summary>
        public int SourceType { get; set; }
    }
}