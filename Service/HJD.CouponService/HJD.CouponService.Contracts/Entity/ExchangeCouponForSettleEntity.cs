using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HJD.CouponService.Contracts.Entity
{
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    [HJD.Framework.Entity.DefaultColumnAttribute()]

    public class ExchangeCouponForSettleEntity
    {
        #region 映射字段
        private Int32 id = 0;

        private Int64 userid = 0;

        private String phonenum = "";

        private String exchangeno = "";

        private Int32 type = 0;

        private Int32 activitytype = 0;

        private Int32 activityid = 0;

        private Int32 state = 0;

        private Int64 exchangetargetid = 0;

        private DateTime createtime = System.DateTime.Parse("1900-1-1");

        private DateTime? canceltime = System.DateTime.Parse("1900-1-1");

        private DateTime? exchangetime = System.DateTime.Parse("1900-1-1");

        private Int32 payid = 0;

        private decimal price = 0;

        private Int64 updator = 0;

        private DateTime updatetime = System.DateTime.Parse("1900-1-1");

        private String addinfo = "";

        private Int32 upgradecount = 0;

        private Boolean hassendovertimemsg = false;

        private Boolean hassendroomalertmsg = false;

        private Int64 cid = 0;

        private Int32 skuid = 0;

        private Decimal settleprice = 0;

        private Int32 promotionid = 0;

        private Boolean hassettle = false;

        private String payaccount = "";

        private Int32 paytype = 0;

        private DateTime? exchangeusetime = System.DateTime.Parse("1900-1-1");

        private String operatorname = "";

        private DateTime? settletime = System.DateTime.Parse("1900-1-1");
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
        [HJD.Framework.Entity.DBColumnAttribute(DefaultValue = "((0))", IsOptional = true)]
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
        public String PhoneNum
        {
            get
            {
                return this.phonenum;
            }
            set
            {
                this.phonenum = value;
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
        public Int32 Type
        {
            get
            {
                return this.type;
            }
            set
            {
                this.type = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32 ActivityType
        {
            get
            {
                return this.activitytype;
            }
            set
            {
                this.activitytype = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32 ActivityID
        {
            get
            {
                return this.activityid;
            }
            set
            {
                this.activityid = value;
            }
        }

        /// <summary>
        /// 
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
        public Int64 ExchangeTargetID
        {
            get
            {
                return this.exchangetargetid;
            }
            set
            {
                this.exchangetargetid = value;
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
        public DateTime? CancelTime
        {
            get
            {
                return this.canceltime;
            }
            set
            {
                this.canceltime = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public DateTime? ExchangeTime
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

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32 PayID
        {
            get
            {
                return this.payid;
            }
            set
            {
                this.payid = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public decimal Price
        {
            get
            {
                return this.price;
            }
            set
            {
                this.price = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int64 Updator
        {
            get
            {
                return this.updator;
            }
            set
            {
                this.updator = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
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
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public String AddInfo
        {
            get
            {
                return this.addinfo;
            }
            set
            {
                this.addinfo = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(DefaultValue = "((0))", IsOptional = true)]
        public Int32 UpgradeCount
        {
            get
            {
                return this.upgradecount;
            }
            set
            {
                this.upgradecount = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Boolean HasSendOverTimeMsg
        {
            get
            {
                return this.hassendovertimemsg;
            }
            set
            {
                this.hassendovertimemsg = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Boolean HasSendRoomAlertMsg
        {
            get
            {
                return this.hassendroomalertmsg;
            }
            set
            {
                this.hassendroomalertmsg = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
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
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Decimal SettlePrice
        {
            get
            {
                return this.settleprice;
            }
            set
            {
                this.settleprice = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32 PromotionID
        {
            get
            {
                return this.promotionid;
            }
            set
            {
                this.promotionid = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Boolean HasSettle
        {
            get
            {
                return this.hassettle;
            }
            set
            {
                this.hassettle = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32 PayType
        {
            get
            {
                return this.paytype;
            }
            set
            {
                this.paytype = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public String PayAccount
        {
            get
            {
                return this.payaccount;
            }
            set
            {
                this.payaccount = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public DateTime? ExchangeUseTime
        {
            get
            {
                return this.exchangeusetime;
            }
            set
            {
                this.exchangeusetime = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public String OperatorName
        {
            get
            {
                return this.operatorname;
            }
            set
            {
                this.operatorname = value;
            }
        }
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public DateTime? SettleTime
        {
            get
            {
                return this.settletime;
            }
            set
            {
                this.settletime = value;
            }
        }
    }
}