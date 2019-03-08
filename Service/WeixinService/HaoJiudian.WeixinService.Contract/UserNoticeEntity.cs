using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJD.WeixinServices.Contract
{
    
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public sealed class UserNoticeEntity
    {
        
        #region 映射字段
        private Int64 idx = 0;
        
        private Int64 userid = 0;
        
        private Int32 noticetype = 0;
        
        private String message = "";
        
        private String action1url = "";
        
        private Boolean isvalid = false;
        
        private DateTime createtime = System.DateTime.Parse("1900-1-1");
        
        private DateTime updatetime = System.DateTime.Parse("1900-1-1");
        
        private Int32 actiontype = 0;
        #endregion
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public Int64 IDX
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
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public Int32 NoticeType
        {
            get
            {
                return this.noticetype;
            }
            set
            {
                this.noticetype = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public String Message
        {
            get
            {
                return this.message;
            }
            set
            {
                this.message = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public String Action1URL
        {
            get
            {
                return this.action1url;
            }
            set
            {
                this.action1url = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public Boolean IsValid
        {
            get
            {
                return this.isvalid;
            }
            set
            {
                this.isvalid = value;
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
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public DateTime UpdateTime
        {
            get
            {
                return this.updatetime;
            }
            set
            {
                this.updatetime = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public Int32 ActionType
        {
            get
            {
                return this.actiontype;
            }
            set
            {
                this.actiontype = value;
            }
        }
    }
}
