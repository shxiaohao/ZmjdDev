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
    public class UsedProductCouponEntity
    {
        public UsedProductCouponEntity()
        {
            usedDetailProductCoupon = new List<UsedDetailProductCouponEntity>();
        }
        /// <summary>
        /// 当前验券量
        /// </summary>
        [DataMember]
        public int TotalCount { get; set; }

        /// <summary>
        /// 结算总金额
        /// </summary>
        [DataMember]
        public Decimal TotalAmount { get; set; }

        [DataMember]
        public List<UsedDetailProductCouponEntity> usedDetailProductCoupon { get; set; }
    }

    [Serializable]
    [DataContract]
    public class UsedDetailProductCouponEntity
    {
        [DataMember]
        public string SKUName { get; set; }

        [DataMember]
        public int Count { get; set; }

        [DataMember]
        public Decimal Amount { get; set; }
    }

    public class BookNoUsedExchangeCouponEntity
    {
        public BookNoUsedExchangeCouponEntity()
        {
            BookNoUsedList = new List<BookNoUsedExchangeCouponDetailEntity>();
        }
        /// <summary>
        /// 当前已预约未核销数量
        /// </summary>
        [DataMember]
        public int TotalCount { get; set; }

        public List<BookNoUsedExchangeCouponDetailEntity> BookNoUsedList { get; set; }
    }

    public class BookNoUsedExchangeCouponDetailEntity
    {
        [DataMember]
        public string SkuName { get; set; }
        /// <summary>
        /// 预约场次名称
        /// </summary>
        [DataMember]
        public string BookDetailName { get; set; }

        /// <summary>
        /// 预约数量
        /// </summary>
        [DataMember]
        public int PeopleCount { get; set; }
    }
}
