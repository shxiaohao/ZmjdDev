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
    public sealed class JLPackageEntity
    {
        
        #region 映射字段
        private int qtyable = 0;
        
        private int voidabletype = 0;
        
        private int cashscaletype = 0;
        
        private int dayselect = 0;
        
        private string timeselect = "";
        
        private int hotelid = 0;
        
        private string hotelname = "";
        
        private int jlhotelid = 0;
        
        private int roomtypeid = 0;
        
        private int supplierid = 0;
        
        private string namechn = "";
        
        private string acreages = "";
        
        private string floordistribution = "";
        
        private string bedtype = "";
        
        private string bedsize = "";
        
        private int allowaddbed = 0;
        
        private System.DateTime night = System.DateTime.Now;
        
        private int pricingtype = 0;
        
        private int supplierid1 = 0;
        
        private string currency = "";
        
        private double preeprice;
        
        private double price;
        
        private string ratetypename = "";
        
        private string includebreakfastqty2 = "";
        
        private string netcharge = "";
        #endregion
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public int Qtyable
        {
            get
            {
                return this.qtyable;
            }
            set
            {
                this.qtyable = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public int Voidabletype
        {
            get
            {
                return this.voidabletype;
            }
            set
            {
                this.voidabletype = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public int Cashscaletype
        {
            get
            {
                return this.cashscaletype;
            }
            set
            {
                this.cashscaletype = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public int Dayselect
        {
            get
            {
                return this.dayselect;
            }
            set
            {
                this.dayselect = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public string Timeselect
        {
            get
            {
                return this.timeselect;
            }
            set
            {
                this.timeselect = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public int HotelId
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
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public string HotelName
        {
            get
            {
                return this.hotelname;
            }
            set
            {
                this.hotelname = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public int JlHotelID
        {
            get
            {
                return this.jlhotelid;
            }
            set
            {
                this.jlhotelid = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public int RoomtypeId
        {
            get
            {
                return this.roomtypeid;
            }
            set
            {
                this.roomtypeid = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public int Supplierid
        {
            get
            {
                return this.supplierid;
            }
            set
            {
                this.supplierid = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public string Namechn
        {
            get
            {
                return this.namechn;
            }
            set
            {
                this.namechn = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public string Acreages
        {
            get
            {
                return this.acreages;
            }
            set
            {
                this.acreages = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public string Floordistribution
        {
            get
            {
                return this.floordistribution;
            }
            set
            {
                this.floordistribution = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public string Bedtype
        {
            get
            {
                return this.bedtype;
            }
            set
            {
                this.bedtype = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public string Bedsize
        {
            get
            {
                return this.bedsize;
            }
            set
            {
                this.bedsize = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public int Allowaddbed
        {
            get
            {
                return this.allowaddbed;
            }
            set
            {
                this.allowaddbed = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public System.DateTime Night
        {
            get
            {
                return this.night;
            }
            set
            {
                this.night = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public int Pricingtype
        {
            get
            {
                return this.pricingtype;
            }
            set
            {
                this.pricingtype = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public int Supplierid1
        {
            get
            {
                return this.supplierid1;
            }
            set
            {
                this.supplierid1 = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public string Currency
        {
            get
            {
                return this.currency;
            }
            set
            {
                this.currency = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public double Preeprice
        {
            get
            {
                return this.preeprice;
            }
            set
            {
                this.preeprice = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public double Price
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
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public string Ratetypename
        {
            get
            {
                return this.ratetypename;
            }
            set
            {
                this.ratetypename = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public string Includebreakfastqty2
        {
            get
            {
                return this.includebreakfastqty2;
            }
            set
            {
                this.includebreakfastqty2 = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public string Netcharge
        {
            get
            {
                return this.netcharge;
            }
            set
            {
                this.netcharge = value;
            }
        }
    }
}
