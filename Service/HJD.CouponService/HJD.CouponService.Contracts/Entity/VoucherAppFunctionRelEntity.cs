using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HJD.CouponService.Contracts.Entity
{
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public class VoucherAppFunctionRelEntity
    {
        #region 映射字段
        private Int32 voucherdefineid = 0;

        private Int32 appbiztypeid = 0;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32 VoucherDefineID
        {
            get
            {
                return this.voucherdefineid;
            }
            set
            {
                this.voucherdefineid = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32 AppBizTypeID
        {
            get
            {
                return this.appbiztypeid;
            }
            set
            {
                this.appbiztypeid = value;
            }
        }

    }
}