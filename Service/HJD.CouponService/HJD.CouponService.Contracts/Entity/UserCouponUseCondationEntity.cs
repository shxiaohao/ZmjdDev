using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HJD.CouponService.Contracts.Entity
{
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public class UserCouponUseCondationEntity
    {
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string BizName { get; set; }

        #region 映射字段
        private Int32 idx = 0;

        private Int32 coupondefineid = 0;

        private Int32 condationtype = 0;

        private Int32 intval = 0;

        private Int64 creater = 0;

        private DateTime createtime = System.DateTime.Parse("1900-1-1");
        #endregion

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
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
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32 CouponDefineID
        {
            get
            {
                return this.coupondefineid;
            }
            set
            {
                this.coupondefineid = value;
            }
        }

        /// <summary>
        /// 1:sku 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32 CondationType
        {
            get
            {
                return this.condationtype;
            }
            set
            {
                this.condationtype = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32 IntVal
        {
            get
            {
                return this.intval;
            }
            set
            {
                this.intval = value;
            }
        }

        /// <summary>
        /// 数值类型条件的取值
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int64 Creater
        {
            get
            {
                return this.creater;
            }
            set
            {
                this.creater = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public DateTime CreateTime
        {
            get
            {
                return this.createtime;
            }
            set
            {
                this.createtime = value;
            }
        }

    }
}