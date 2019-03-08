using HJD.Framework.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJD.HotelServices.Contracts
{
    [System.Runtime.Serialization.DataContract]
    [Serializable]
    public class CanSellCheapHotelPackageEntity
    {
        /// <summary>
        /// 酒店Id
        /// </summary>
        [System.Runtime.Serialization.DataMember]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public int HotelId { get; set; }

        /// <summary>
        /// 酒店名称
        /// </summary>
        [System.Runtime.Serialization.DataMember]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public string HotelName { get; set; }

        /// <summary>
        /// 套餐Id
        /// </summary>
        [System.Runtime.Serialization.DataMember]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public int PId { get; set; }

        /// <summary>
        /// 套餐名称
        /// </summary>
        [System.Runtime.Serialization.DataMember]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public string PName { get; set; }

        /// <summary>
        /// 简介
        /// </summary>
        [System.Runtime.Serialization.DataMember]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public string PBrief { get; set; }

        /// <summary>
        /// 过期日期
        /// </summary>
        [System.Runtime.Serialization.DataMember]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 价格日期类型
        /// </summary>
        [System.Runtime.Serialization.DataMember]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public int PriceType { get; set; }

        /// <summary>
        /// 价格日期
        /// </summary>
        [System.Runtime.Serialization.DataMember]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public DateTime PriceDate { get; set; }

        /// <summary>
        /// 套餐设置价格（单位元）
        /// </summary>
        [System.Runtime.Serialization.DataMember]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public int Price { get; set; }

        /// <summary>
        /// 套餐优惠售价（单位元）
        /// </summary>
        [System.Runtime.Serialization.DataMember]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public int CheapPrice { get; set; }

        /// <summary>
        /// 套餐市场售价（单位元）
        /// </summary>
        [System.Runtime.Serialization.DataMember]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public int MarketPrice { get; set; }

        /// <summary>
        /// 地区名称
        /// </summary>
        [System.Runtime.Serialization.DataMember]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public string DistrictName { get; set; }

        /// <summary>
        /// 利润（单位元）
        /// </summary>
        [System.Runtime.Serialization.DataMember]
        [DBColumnAttribute(DefaultValue = "0", IsOptional = true)]
        public decimal Profit { get; set; }
    }
}