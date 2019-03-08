using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HJD.CouponService.Contracts.Entity
{
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public class ExchangeSettleModel
    {
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string OperatorUserName { get; set; }

        #region 映射字段
        private Int32 spuid = 0;

        private Int32 couponcount = 0;

        private decimal supplieramount = 0;

        private OpLogBizType biztype = 0;

        private String spuname = "";
        private String skuname = "";
        private String suppliername = "";
        #endregion

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32 SpuID
        {
            get
            {
                return this.spuid;
            }
            set
            {
                this.spuid = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32 CouponCount
        {
            get
            {
                return this.couponcount;
            }
            set
            {
                this.couponcount = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public decimal SupplierAmount
        {
            get
            {
                return this.supplieramount;
            }
            set
            {
                this.supplieramount = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public String SPUName
        {
            get
            {
                return this.spuname;
            }
            set
            {
                this.spuname = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public String SKUName
        {
            get
            {
                return this.skuname;
            }
            set
            {
                this.skuname = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public String SupplierName
        {
            get
            {
                return this.suppliername;
            }
            set
            {
                this.suppliername = value;
            }
        }
    }
}