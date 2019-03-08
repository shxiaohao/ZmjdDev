using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HJD.CouponService.Contracts.Entity
{ 
    
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public sealed class CouponActivitySKURelEntity
    {
        
        #region 映射字段
        private Int32 idx = 0;
        
        private Int32 skuid = 0;
        
        private Int32 couponactivityid = 0;
        #endregion
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public Int32 IDX
        {
            get
            {
                return this.idx;
            }
            set
            {
                this.idx = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public Int32 SKUID
        {
            get
            {
                return this.skuid;
            }
            set
            {
                this.skuid = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public Int32 CouponActivityID
        {
            get
            {
                return this.couponactivityid;
            }
            set
            {
                this.couponactivityid = value;
            }
        }
    }
}
