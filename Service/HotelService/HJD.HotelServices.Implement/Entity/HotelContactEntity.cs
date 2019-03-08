using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJD.HotelServices.Implement.Entity
{
    
    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public sealed class HotelContactEntity
    {

        #region 映射字段
        private Int32 hotelid = 0;
        
        private String contactname = "";
        
        private String tel = "";
        
        private String fax = "";
        
        private String sms = "";
        
        private String fax2 = "";
        
        private Int32 fax2timestart = 0;
        
        private Int32 fax2timeend = 0;
        
        private String financial = "";
        
        private String financialtel = "";
        
        private String sales = "";
        
        private Int32 deposit = 0;
        
        private Int32 salesdepttimefrom = 0;
        
        private Int32 salesdepttimeto = 0;
        
        private Int32 invoicetype = 0;
        
        private String tag = "";
        
        private String bankinfo = "";
        
        private Int32 hotelpaytype = 0;
        
        private String selfbreakfastprice = "";

        private String buffetsupperprice = "";
        
        private String note = "";
        
        private Int32 salesdeptoffday = 0;
        
        private Int32 withpet = 0;
        
        private Int32 indoorswimmingpool = 0;
        
        private Int32 indoorswimmingpoolishengwen = 0;
        
        private Int32 outdoorswimmingpool = 0;
        
        private Int32 indoorchildrenpool = 0;
        
        private Int32 outdoorchildrenpool = 0;
        
        private Int32 outdoorchildrenpoolishengwen = 0;
        
        private Int32 indoorchildrenplayground = 0;
        
        private Int32 outdoorchildrenplayground = 0;
        
        private Int32 roomwifi = 0;
        
        private String highspeedrailstation = "";
        
        private Int32 hsrailstationdistance = 0;
        
        private Int32 hasbus = 0;
        
        private String busschedule = "";
        
        private Int32 beach = 0;
        
        private Int32 privatebeach = 0;
        
        private Int32 tenniscourt = 0;
        
        private Int32 childcare = 0;
        
        private Int32 childcareisfree = 0;
        
        private Int32 isfestival = 0;
        
        private Int32 priority = 0;
        
        private Int32 monthsettleday = 0;
        
        private Int32 checkinsettleadvanceday = 0;
        
        private String confirmemail = "";

        private String hotelitemnotice = "";
        #endregion

        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public String HotelItemNotice
        {
            get
            {
                return this.hotelitemnotice;
            }
            set
            {
                this.hotelitemnotice = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
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
        /// 
        /// </summary>
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public String ContactName
        {
            get
            {
                return this.contactname;
            }
            set
            {
                this.contactname = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public String Tel
        {
            get
            {
                return this.tel;
            }
            set
            {
                this.tel = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public String Fax
        {
            get
            {
                return this.fax;
            }
            set
            {
                this.fax = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public String Sms
        {
            get
            {
                return this.sms;
            }
            set
            {
                this.sms = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public String Fax2
        {
            get
            {
                return this.fax2;
            }
            set
            {
                this.fax2 = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public Int32 Fax2TimeStart
        {
            get
            {
                return this.fax2timestart;
            }
            set
            {
                this.fax2timestart = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public Int32 Fax2TimeEnd
        {
            get
            {
                return this.fax2timeend;
            }
            set
            {
                this.fax2timeend = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public String Financial
        {
            get
            {
                return this.financial;
            }
            set
            {
                this.financial = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public String FinancialTel
        {
            get
            {
                return this.financialtel;
            }
            set
            {
                this.financialtel = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public String Sales
        {
            get
            {
                return this.sales;
            }
            set
            {
                this.sales = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public Int32 Deposit
        {
            get
            {
                return this.deposit;
            }
            set
            {
                this.deposit = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public Int32 SalesDeptTimeFrom
        {
            get
            {
                return this.salesdepttimefrom;
            }
            set
            {
                this.salesdepttimefrom = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public Int32 SalesDeptTimeTo
        {
            get
            {
                return this.salesdepttimeto;
            }
            set
            {
                this.salesdepttimeto = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public Int32 InvoiceType
        {
            get
            {
                return this.invoicetype;
            }
            set
            {
                this.invoicetype = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public String Tag
        {
            get
            {
                return this.tag;
            }
            set
            {
                this.tag = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public String BankInfo
        {
            get
            {
                return this.bankinfo;
            }
            set
            {
                this.bankinfo = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public Int32 HotelPayType
        {
            get
            {
                return this.hotelpaytype;
            }
            set
            {
                this.hotelpaytype = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public String SelfBreakfastPrice
        {
            get
            {
                return this.selfbreakfastprice;
            }
            set
            {
                this.selfbreakfastprice = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public String BuffetSupperPrice
        {
            get
            {
                return this.buffetsupperprice;
            }
            set
            {
                this.buffetsupperprice = value;
            }
        
        }
        
        /// <summary>
        /// 
        /// </summary>
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public String Note
        {
            get
            {
                return this.note;
            }
            set
            {
                this.note = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public Int32 SalesDeptOffday
        {
            get
            {
                return this.salesdeptoffday;
            }
            set
            {
                this.salesdeptoffday = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public Int32 WithPet
        {
            get
            {
                return this.withpet;
            }
            set
            {
                this.withpet = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public Int32 IndoorSwimmingPool
        {
            get
            {
                return this.indoorswimmingpool;
            }
            set
            {
                this.indoorswimmingpool = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public Int32 IndoorSwimmingPoolIsHengWen
        {
            get
            {
                return this.indoorswimmingpoolishengwen;
            }
            set
            {
                this.indoorswimmingpoolishengwen = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public Int32 OutdoorSwimmingPool
        {
            get
            {
                return this.outdoorswimmingpool;
            }
            set
            {
                this.outdoorswimmingpool = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public Int32 IndoorChildrenPool
        {
            get
            {
                return this.indoorchildrenpool;
            }
            set
            {
                this.indoorchildrenpool = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public Int32 OutdoorChildrenPool
        {
            get
            {
                return this.outdoorchildrenpool;
            }
            set
            {
                this.outdoorchildrenpool = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public Int32 OutdoorChildrenPoolIsHengWen
        {
            get
            {
                return this.outdoorchildrenpoolishengwen;
            }
            set
            {
                this.outdoorchildrenpoolishengwen = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public Int32 IndoorChildrenPlayground
        {
            get
            {
                return this.indoorchildrenplayground;
            }
            set
            {
                this.indoorchildrenplayground = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public Int32 OutdoorChildrenPlayground
        {
            get
            {
                return this.outdoorchildrenplayground;
            }
            set
            {
                this.outdoorchildrenplayground = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public Int32 RoomWIFI
        {
            get
            {
                return this.roomwifi;
            }
            set
            {
                this.roomwifi = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public String HighSpeedRailStation
        {
            get
            {
                return this.highspeedrailstation;
            }
            set
            {
                this.highspeedrailstation = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public Int32 HSRailStationDistance
        {
            get
            {
                return this.hsrailstationdistance;
            }
            set
            {
                this.hsrailstationdistance = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public Int32 HasBus
        {
            get
            {
                return this.hasbus;
            }
            set
            {
                this.hasbus = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public String BusSchedule
        {
            get
            {
                return this.busschedule;
            }
            set
            {
                this.busschedule = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public Int32 Beach
        {
            get
            {
                return this.beach;
            }
            set
            {
                this.beach = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public Int32 PrivateBeach
        {
            get
            {
                return this.privatebeach;
            }
            set
            {
                this.privatebeach = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public Int32 TennisCourt
        {
            get
            {
                return this.tenniscourt;
            }
            set
            {
                this.tenniscourt = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public Int32 ChildCare
        {
            get
            {
                return this.childcare;
            }
            set
            {
                this.childcare = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public Int32 ChildCareIsFree
        {
            get
            {
                return this.childcareisfree;
            }
            set
            {
                this.childcareisfree = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public Int32 IsFestival
        {
            get
            {
                return this.isfestival;
            }
            set
            {
                this.isfestival = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public Int32 Priority
        {
            get
            {
                return this.priority;
            }
            set
            {
                this.priority = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public Int32 MonthSettleDay
        {
            get
            {
                return this.monthsettleday;
            }
            set
            {
                this.monthsettleday = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public Int32 CheckInSettleAdvanceDay
        {
            get
            {
                return this.checkinsettleadvanceday;
            }
            set
            {
                this.checkinsettleadvanceday = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public String ConfirmEmail
        {
            get
            {
                return this.confirmemail;
            }
            set
            {
                this.confirmemail = value;
            }
        }

        /// <summary>
        /// 显示携程价格
        /// </summary>
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public bool ShowCtripPrice { get; set; }
    }
}