using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HJD.CouponService.Contracts.Entity
{
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public class VoucherItemsEntity
    {
        #region 映射字段
        private Int64 idx = 0;

        private Int32 voucherchannelid = 0;

        private String exchangeno = "";

        private DateTime createtime = System.DateTime.Parse("1900-1-1");

        private DateTime expiretime = System.DateTime.Parse("1900-1-1");

        private Int32 state = 0;

        private Int64 creator = 0;

        private Int64 userid = 0;

        private Int64 relbizid = 0;

        private Int32 relbiztype = 0;

        private DateTime exchangetime = System.DateTime.Parse("1900-1-1");

        private String channelname = "";

        private String channelcode = "";
        #endregion

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public String ChannelCode
        {
            get
            {
                return this.channelcode;
            }
            set
            {
                this.channelcode = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public String ChannelName
        {
            get
            {
                return this.channelname;
            }
            set
            {
                this.channelname = value;
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
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
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
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32 VoucherChannelID
        {
            get
            {
                return this.voucherchannelid;
            }
            set
            {
                this.voucherchannelid = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public String ExchangeNo
        {
            get
            {
                return this.exchangeno;
            }
            set
            {
                this.exchangeno = value;
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
        /// 过期时间
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public DateTime ExpireTime
        {
            get
            {
                return this.expiretime;
            }
            set
            {
                this.expiretime = value;
            }
        }

        /// <summary>
        /// 0:可用 1：已使用用 2：已过期  3：删除
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
        /// 兑换用户ID
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int64 Userid
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
        /// 业务ID
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int64 RelBizID
        {
            get
            {
                return this.relbizid;
            }
            set
            {
                this.relbizid = value;
            }
        }

        /// <summary>
        /// 业务类型
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32 RelBizType
        {
            get
            {
                return this.relbiztype;
            }
            set
            {
                this.relbiztype = value;
            }
        }

        /// <summary>
        /// 兑换时间
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public DateTime ExchangeTime
        {
            get
            {
                return this.exchangetime;
            }
            set
            {
                this.exchangetime = value;
            }
        }

    }
}