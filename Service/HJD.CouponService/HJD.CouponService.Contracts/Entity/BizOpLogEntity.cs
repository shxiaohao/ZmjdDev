using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HJD.CouponService.Contracts.Entity
{
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public sealed class BizOpLogEntity
    {
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string OperatorUserName { get; set; }

        #region 映射字段
        private Int64 id = 0;

        private Int64 operatoruserid = 0;

        private DateTime opdatetime = System.DateTime.Parse("1900-1-1");

        private OpLogBizType biztype = 0;

        private Int64? bizid = 0;

        private String bizidstr = "";

        private Int32 optype = 0;

        private String opcontent = "";
        #endregion

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int64 ID
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
        public Int64 OperatorUserID
        {
            get
            {
                return this.operatoruserid;
            }
            set
            {
                this.operatoruserid = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public DateTime OpDateTime
        {
            get
            {
                return this.opdatetime;
            }
            set
            {
                this.opdatetime = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public OpLogBizType BizType
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

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int64? BizID
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
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public String BizIDStr
        {
            get
            {
                return this.bizidstr;
            }
            set
            {
                this.bizidstr = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32 OpType
        {
            get
            {
                return this.optype;
            }
            set
            {
                this.optype = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public String OpContent
        {
            get
            {
                return this.opcontent;
            }
            set
            {
                this.opcontent = value;
            }
        }
    }
}