using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HJD.CouponService.Contracts.Entity
{
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public class UserCouponDefineEntity
    {

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32 UsedCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string PromotionSKUName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int PromotionSKUID { get; set; }
        /// <summary>
        /// 用户是否购买过该券
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public bool UserBuyState { get; set; }

        #region 映射字段
        private Int32 idx = 0;

        private Int32 type = 0;

        private String name = "";

        private String description = "";

        private Decimal requireamount = 0m;

        private Decimal discountamount = 0m;

        private Int32 state = 0;

        private Int64 creater = 0;

        private DateTime createtime = System.DateTime.Parse("1900-1-1");

        private DateTime validuntildate = System.DateTime.Parse("1900-1-1");

        private Int32 expirationday = 0;

        private Int32 expirationtype = 0;

        private DateTime startusedate = System.DateTime.Parse("1900-1-1");


        //private string tips = "";

        private bool isaccordingproduct = false;

        #endregion


        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public bool IsAccordingProduct
        {
            get
            {
                return this.isaccordingproduct;
            }
            set
            {
                this.isaccordingproduct = value;
            }
        }

        //[System.Runtime.Serialization.DataMemberAttribute()]
        //[HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        //public string Tips
        //{
        //    get
        //    {
        //        return this.tips;
        //    }
        //    set
        //    {
        //        this.tips = value;
        //    }
        //}

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public DateTime StartUseDate
        {
            get
            {
                return this.startusedate;
            }
            set
            {
                this.startusedate = value;
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
        /// 券类型，0：满减，1：立减
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
        public String Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public String Description
        {
            get
            {
                return this.description;
            }
            set
            {
                this.description = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Decimal RequireAmount
        {
            get
            {
                return this.requireamount;
            }
            set
            {
                this.requireamount = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Decimal DiscountAmount
        {
            get
            {
                return this.discountamount;
            }
            set
            {
                this.discountamount = value;
            }
        }

        /// <summary>
        /// 状态 0 ：可用3：取消
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

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public DateTime ValidUntilDate
        {
            get
            {
                return this.validuntildate;
            }
            set
            {
                this.validuntildate = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32 ExpirationDay
        {
            get
            {
                return this.expirationday;
            }
            set
            {
                this.expirationday = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32 ExpirationType
        {
            get
            {
                return this.expirationtype;
            }
            set
            {
                this.expirationtype = value;
            }
        }
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        [System.ComponentModel.Description("营销类型")]
        public Int32 MarketingType { get; set; }
    }
}