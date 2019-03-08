using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HJD.CouponService.Contracts.Entity
{
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public class RetailUrlEntity
    {  
        #region 映射字段
        private Int32 idx = 0;
        
        private Int32 retailid = 0;
        
        private Int64 cid = 0;
        
        private String resourceurl = "";
        
        private String shortresourceurl = "";
        
        private String producturl = "";
        
        private String shortproducturl = "";
        
        private DateTime createtime = System.DateTime.Parse("1900-1-1");

        private Int32 skuid = 0;

        private Int32 pid = 0;
        #endregion
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32 PID
        {
            get
            {
                return this.pid;
            }
            set
            {
                this.pid = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
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
        public Int32 RetailID
        {
            get
            {
                return this.retailid;
            }
            set
            {
                this.retailid = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public Int64 CID
        {
            get
            {
                return this.cid;
            }
            set
            {
                this.cid = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public String ResourceURL
        {
            get
            {
                return this.resourceurl;
            }
            set
            {
                this.resourceurl = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public String ShortResourceURL
        {
            get
            {
                return this.shortresourceurl;
            }
            set
            {
                this.shortresourceurl = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public String ProductUrl
        {
            get
            {
                return this.producturl;
            }
            set
            {
                this.producturl = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public String ShortProductUrl
        {
            get
            {
                return this.shortproducturl;
            }
            set
            {
                this.shortproducturl = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
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
