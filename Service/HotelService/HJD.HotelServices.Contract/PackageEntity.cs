using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJD.HotelServices.Contracts
{  
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public sealed class PackageEntity
    {
        /// <summary>
        /// 分享内容
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string ShareDescription { get; set; }

        /// <summary>
        /// 分享标题
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string ShareTitle { get; set; }

        /// <summary>
        /// 主推套餐
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public bool IsMainPush { get; set; }


        /// <summary>
        /// 套餐是否可分销
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public bool IsDistributable { get; set; }

        /// <summary>
        /// 优势价格
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public bool InBenefitArea { get; set; }  

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public bool IsNotSale { get; set; }  

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public bool CanUseCoupon { get; set; }  

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public bool IsSellOut { get; set; }  

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public bool IsAskPrice { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int WHPackageType { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public bool OnlyForVIP { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public bool ForVIPFirstBuy { get; set; } 
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public  int VIPFirstPayDiscount { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int DateSelectType { get; set; }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string  Brief { get; set; }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string SerialNO { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public bool CanInvoice { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int TimeAdvance { get; set; }//预订时间提前量，分钟。如今天不能预订今天的，吸能预订明天的，但也必需在4点前预订。那么提前时间量为：24*60+（24-16）*60

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int StartNum { get; set; }//初始套餐销售数

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int SoldCountSum { get; set; }//套餐已销售总数

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int DayLimitMin { get; set; }////套餐可选最少天数限止，0：无限止， 1：最少选一天 2：最少选两天 3：。。。。

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int DayLimitMax { get; set; }//套餐可选最多天数限止，0：无限止， 1：最多选一天 2：最多选两天 3：。。。。

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int RoomNightLimitMin { get; set; }////套餐可选最少天数限止，0：无限止， 1：最少选一天 2：最少选两天 3：。。。。

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int RoomNightLimitMax { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int BigBedCount { get; set; }  //可订大床数

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int TwinBedCount { get; set; } //可订双订数

        #region 映射字段
        private Int32 id = 0;
        
        private Int32 hotelid = 0;
        
        private String code = "";
        
        private DateTime startdate = System.DateTime.Parse("1900-1-1");
        
        private DateTime enddate = System.DateTime.Parse("1900-1-1");
        
        private Boolean isvalid = false;
        
        private Int32 roomid = 0;
        
        private Int32? packagecount = 0;

        private Int32 minhotelpeople = 0;

        private Int32 maxhotelpeople = 0;

        private String cardtype = "";

        private String travelpersondescribe = "";

        private int custombuymax = 0;

        private int custombuymin = 0;
        #endregion
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
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
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public Int32 HotelID
        {
            get
            {
                return this.hotelid;
            }
            set
            {
                this.hotelid = value;
            }
        }

        /// <summary>
        /// 房间供应商ID
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32 SupplierID { get; set; }
        
        /// <summary>
        /// 房间供应商类型
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32 SupplierType { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public String Code
        {
            get
            {
                return this.code;
            }
            set
            {
                this.code = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
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
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public DateTime EndDate
        {
            get
            {
                return this.enddate;
            }
            set
            {
                this.enddate = value;
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
        public Int32 RoomID
        {
            get
            {
                return this.roomid;
            }
            set
            {
                this.roomid = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public Int32? PackageCount
        {
            get
            {
                return this.packagecount;
            }
            set
            {
                this.packagecount = value;
            }
        }

        /// <summary>
        /// 最少入住人数
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32 MinHotelPeople
        {
            get
            {
                return this.minhotelpeople;
            }
            set
            {
                this.minhotelpeople = value;
            }
        }

        /// <summary>
        /// 最多入住人数
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32 MaxHotelPeople
        {
            get
            {
                return this.maxhotelpeople;
            }
            set
            {
                this.maxhotelpeople = value;
            }
        }

        /// <summary>
        /// 证件类型，多个用";"分割
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public String CardType
        {
            get
            {
                return this.cardtype;
            }
            set
            {
                this.cardtype = value;
            }
        }
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string[] CardTypeList { get; set; }


        /// <summary>
        /// 出行人描述
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string TravelPersonDescribe
        {
            get { return this.travelpersondescribe; }
            set { this.travelpersondescribe = value; }
        }

        /// <summary>
        /// 最少购买限制
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32 CustomBuyMin
        {
            get
            {
                return this.custombuymin;
            }
            set
            {
                this.custombuymin = value;
            }
        }

        /// <summary>
        /// 最多购买限制
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32 CustomBuyMax
        {
            get
            {
                return this.custombuymax;
            }
            set
            {
                this.custombuymax = value;
            }
        }

    }
}
