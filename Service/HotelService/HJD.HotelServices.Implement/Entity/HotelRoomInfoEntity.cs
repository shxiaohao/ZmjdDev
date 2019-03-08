using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJD.HotelServices.Implement.Entity
{
    
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public sealed class HotelRoomInfoEntity
    {
        
        #region 映射字段
        private int id = 0;
        
        private int hotelid = 0;
        
        private string roomcode = "";
        
        private string description = "";
          
        
        private string options = "";
        
        private string defaultoption = "";
        
        private string area = "";
        
        private string floor = "";
        
        private int addbedfee = 0;
        
        private bool addbedfeeincludebreakfast;
        
        private int bigbedcount = 0;
        
        private double bigbedwidth;
        
        private int twinbedcount = 0;
        
        private double twinbedwidth;
        
        private bool twinbedcanmove;
        
        private string specialdes = "";
        
        private int defaultbedtype = 0;
        
        private int hastwinbed = 0;
        
        private int hasbigbed = 0;

        private bool ischeckbedtype = true;
        #endregion
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public int ID
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
        public int HotelID
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
        public string RoomCode
        {
            get
            {
                return this.roomcode;
            }
            set
            {
                this.roomcode = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public string Description
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
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public string Options
        {
            get
            {
                return this.options;
            }
            set
            {
                this.options = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public string DefaultOption
        {
            get
            {
                return this.defaultoption;
            }
            set
            {
                this.defaultoption = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public string Area
        {
            get
            {
                return this.area;
            }
            set
            {
                this.area = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public string Floor
        {
            get
            {
                return this.floor;
            }
            set
            {
                this.floor = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public int AddBedFee
        {
            get
            {
                return this.addbedfee;
            }
            set
            {
                this.addbedfee = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public bool AddBedFeeIncludeBreakfast
        {
            get
            {
                return this.addbedfeeincludebreakfast;
            }
            set
            {
                this.addbedfeeincludebreakfast = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public int BigBedCount
        {
            get
            {
                return this.bigbedcount;
            }
            set
            {
                this.bigbedcount = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public double BigBedWidth
        {
            get
            {
                return this.bigbedwidth;
            }
            set
            {
                this.bigbedwidth = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public int TwinBedCount
        {
            get
            {
                return this.twinbedcount;
            }
            set
            {
                this.twinbedcount = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public double TwinBedWidth
        {
            get
            {
                return this.twinbedwidth;
            }
            set
            {
                this.twinbedwidth = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public bool TwinBedCanMove
        {
            get
            {
                return this.twinbedcanmove;
            }
            set
            {
                this.twinbedcanmove = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public string SpecialDes
        {
            get
            {
                return this.specialdes;
            }
            set
            {
                this.specialdes = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public int DefaultBedType
        {
            get
            {
                return this.defaultbedtype;
            }
            set
            {
                this.defaultbedtype = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public int HasTwinBed
        {
            get
            {
                return this.hastwinbed;
            }
            set
            {
                this.hastwinbed = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public int HasBigBed
        {
            get
            {
                return this.hasbigbed;
            }
            set
            {
                this.hasbigbed = value;
            }
        }

        /// <summary>
        /// 是否确认床型
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public bool IsCheckBedType
        {
            get { return this.ischeckbedtype; }
            set { this.ischeckbedtype = value; }
        
        }
    }
}
