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
    public class BuyCouponCheckNumResult
    {
        [DataMember]
        public string Message { get; set; }

        ///// <summary>
        ///// 用户状态 0.没有购买过 1.购买过且没有超过限制 2.购买达到上限 3.锁定未支付(超过十分钟)两次
        ///// </summary>
        //[DataMember]
        //public int UserState { get; set; }

        /// <summary>
        /// 用户买的数量（提交的数量不包括已取消的数量）
        /// </summary>
        [DataMember]
        public int UserBuyNum { get; set; }

        /// <summary>
        /// 0.全部售出 1.还有未售出
        /// </summary>
        [DataMember]
        public int ActivityState { get; set; }

        /// <summary>
        /// 活动没卖出的数量中未被锁定的数量
        /// </summary>
        [DataMember]
        public int ActivityNoLockNum { get; set; }

        /// <summary>
        /// 活动没卖出的数量中被锁定的数量
        /// </summary>
        [DataMember]
        public int ActivityLockNum { get; set; }

        /// <summary>
        /// 活动总数量
        /// </summary>
        [DataMember]
        public int TotalNum { get; set; }

        /// <summary>
        /// 活动卖出的数量(不包括被锁定数量)
        /// </summary>
        [DataMember]
        public int SellNum { get; set; }

        /// <summary>
        /// 最多购买数量
        /// </summary>
        [DataMember]
        public int MaxBuyNum { get; set; }

        /// <summary>
        /// 最少购买数量
        /// </summary>
        [DataMember]
        public int MinBuyNum { get; set; }

        /// <summary>
        /// 来源ID 如套餐ID
        /// </summary>
        [DataMember]
        public long SourceID { get; set; }

        /// <summary>
        /// 活动价格字符串
        /// </summary>
        [DataMember]
        public string Price { get; set; }

        /// <summary>
        /// 购买数量起始值
        /// </summary>
        [DataMember]
        public int ManuSellNum { get; set; }

        /// <summary>
        /// 验证（活动开始）时间是否到了
        /// </summary>
        [DataMember]
        public DateTime EffectiveTime { get; set; }

        /// <summary>
        /// 验证（活动结束）时间是否到了
        /// </summary>
        [DataMember]
        public DateTime SaleEndDate { get; set; }
    }

     [Serializable]
    [DataContract]
    public class BuyCouponSuccessResult
    {
        [DataMember]
        public string ObjectName { get; set; }

        /// <summary>
        /// 用户状态 0.没有购买过 1.购买过且没有超过限制 2.购买达到上限 3.锁定未支付(超过十分钟)两次
        /// </summary>
        [DataMember]
        public List<string> CouponCodeList { get; set; }

        [DataMember]
        public int ActivityType { get; set; }
    }

     [Serializable]
    [DataContract]
    public class ActivityProductAndPrice
    {
        [DataMember]
        public string ObjectName { get; set; }

        /// <summary>
        /// 用户状态 0.没有购买过 1.购买过且没有超过限制 2.购买达到上限 3.锁定未支付(超过十分钟)两次
        /// </summary>
        [DataMember]
        public List<string> CouponCodeList { get; set; }

        [DataMember]
        public int ActivityType { get; set; }
    }

     [Serializable]
    [DataContract]
    public class CouponUserLockNumResult
    {
        /// <summary>
        /// 用户已锁定订单数： 已提交 + 已支付 + 已兑换 + 超时支付
        /// </summary>
        [DataMember]
        public Int32 HasLockedNum { get; set; }
         

        [DataMember]
        public Int32 OrderID { get; set; }

        [DataMember]
        public Int32 PayType { get; set; }

        /// <summary>
        /// 末支付数
        /// </summary>
        [DataMember]
        public Int32 NotPaiedCount { get; set; }
    }


     [Serializable]
    [DataContract]
    public class CouponLockNumResult
    {
      [DataMember]
       public Int32 ActivityLockNum { get; set; }         
    }
}