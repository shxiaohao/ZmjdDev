using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HJD.CouponService.Contracts.Entity
{
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public class RedShareEntity
    {
        /// <summary>
        /// 已抢数量
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int GrabCount { get; set; }

        #region 映射字段
        private Int32 id = 0;

        private Decimal totalamount = 0m;

        private Int32 totalcount = 0;

        private Decimal maxamount = 0m;

        private Decimal minamount = 0m;

        private String redurl = "";

        private DateTime createtime = System.DateTime.Parse("1900-1-1");

        private Int64 createuser = 0;

        private String guid = "";

        private String sharetitle = "";

        private String sharedesc = "";
        #endregion

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public String ShareTitle
        {
            get
            {
                return this.sharetitle;
            }
            set
            {
                this.sharetitle = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public String ShareDesc
        {
            get
            {
                return this.sharedesc;
            }
            set
            {
                this.sharedesc = value;
            }
        }
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
        /// 总金额
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Decimal TotalAmount
        {
            get
            {
                return this.totalamount;
            }
            set
            {
                this.totalamount = value;
            }
        }

        /// <summary>
        /// 红包总数
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32 TotalCount
        {
            get
            {
                return this.totalcount;
            }
            set
            {
                this.totalcount = value;
            }
        }

        /// <summary>
        /// 最大金额
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Decimal MaxAmount
        {
            get
            {
                return this.maxamount;
            }
            set
            {
                this.maxamount = value;
            }
        }

        /// <summary>
        /// 最小金额
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Decimal MinAmount
        {
            get
            {
                return this.minamount;
            }
            set
            {
                this.minamount = value;
            }
        }

        /// <summary>
        /// 红包url
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public String RedUrl
        {
            get
            {
                return this.redurl;
            }
            set
            {
                this.redurl = value;
            }
        }

        /// <summary>
        /// 创建时间
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
        /// 创建人
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int64 CreateUser
        {
            get
            {
                return this.createuser;
            }
            set
            {
                this.createuser = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public String GUID
        {
            get
            {
                return this.guid;
            }
            set
            {
                this.guid = value;
            }
        }
    }
}