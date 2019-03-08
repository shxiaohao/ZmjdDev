using System;
using System.Runtime.Serialization;
using HJD.Framework.Entity;

namespace HJD.HotelServices.Contracts
{
    /// <summary>
    /// 酒店点评新接口日志
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    [DefaultColumn]
    
    public class HotelReviewWritingLog
    {

        #region 重要

        /// <summary>
        /// 驴评点评id
        /// </summary>
        [DataMember]
        [DBColumnAttribute(IsOptional = true)]
        public int Writing { get; set; }

        /// <summary>
        /// 携程的点评ID
        /// </summary>
        [DataMember]
        [DBColumnAttribute(IsOptional = true)]
        public int Ctripwriting { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        [DataMember]
        [DBColumnAttribute(IsOptional = true)]
        public int Orderid { get; set; }

        /// <summary>
        /// 点评类型（C用户在携程点评，L用户在驴评点评）
        /// </summary>
        [DataMember]
        [DBColumnAttribute(IsOptional = true)]
        public string Source { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        [DataMember]
        [DBColumnAttribute(IsOptional = true)]
        public string OrderStatus { get; set; }

        /// <summary>
        /// 照片下载状态
        /// </summary>
        [DataMember]
        [DBColumnAttribute(IsOptional = true)]
        public string Photostatus { get; set; }

        #endregion

    }
}



