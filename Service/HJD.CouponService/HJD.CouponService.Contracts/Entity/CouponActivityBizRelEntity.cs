using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HJD.CouponService.Contracts.Entity
{
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public class CouponActivityBizRelEntity
    {
        #region 映射字段
        private Int32 id = 0;

        private Int32 couponactivityid = 0;

        private Int32 bizid = 0;

        private Int32 biztype = 0;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32 ID
        {
            get
            {
                return this.id;
            }
            set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32 CouponActivityId
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

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32 BizID
        {
            get
            {
                return this.bizid;
            }
            set
            {
                this.bizid = value;
            }
        }

        /// <summary>
        /// 类型
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32 BizType
        {
            get
            {
                return this.biztype;
            }
            set
            {
                this.biztype = value;
            }
        }

    }
}