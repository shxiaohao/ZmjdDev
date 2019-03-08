using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HJD.CouponService.Contracts.Entity
{
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public class UserCouponItemEntity
    {
        #region 映射字段
        private Int32 idx = 0;

        private Int32 coupondefineid = 0;

        private Int64 userid = 0;

        private DateTime expireddate = System.DateTime.Parse("1900-1-1");

        private Decimal restamount = 0m;

        private Int32 state = 0;

        private DateTime createtime = System.DateTime.Parse("1900-1-1");

        private Int64 creator = 0;

        private DateTime startdate = System.DateTime.Parse("1900-1-1");
        #endregion

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public DateTime StartDate
        {
            get
            {
                return this.startdate;
            }
            set
            {
                this.startdate = value;
            }
        }


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
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int64 UserID
        {
            get
            {
                return this.userid;
            }
            set
            {
                this.userid = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public DateTime ExpiredDate
        {
            get
            {
                return this.expireddate;
            }
            set
            {
                this.expireddate = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Decimal RestAmount
        {
            get
            {
                return this.restamount;
            }
            set
            {
                this.restamount = value;
            }
        }

        /// <summary>
        /// 0：记录  1：已使用 2：过期 3:取消
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32 State
        {
            get
            {
                return this.state;
            }
            set
            {
                this.state = value;
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

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int64 Creator
        {
            get
            {
                return this.creator;
            }
            set
            {
                this.creator = value;
            }
        }

    }
}