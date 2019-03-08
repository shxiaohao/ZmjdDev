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
    public sealed class HotelRestaurantEntity
    {
        
        #region 映射字段
        private Int32 id = 0;
        
        private String name = "";
        
        private Int32? fee = 0;
        
        private Double? taste = 0F;
        
        private Double? environment = 0F;
        
        private Double? service = 0F;
        
        private String tag = "";
        
        private String featrued = "";
        
        private String distance = "";
        
        private Boolean isinner = false;
        
        private String dpurl = "";
        
        private Int32? state = 0;
        
        private Int32? hotelid = 0;
        
        private Int32? shopid = 0;
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
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public Int32? Fee
        {
            get
            {
                return this.fee;
            }
            set
            {
                this.fee = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public Double? Taste
        {
            get
            {
                return this.taste;
            }
            set
            {
                this.taste = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public Double? Environment
        {
            get
            {
                return this.environment;
            }
            set
            {
                this.environment = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public Double? Service
        {
            get
            {
                return this.service;
            }
            set
            {
                this.service = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
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
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public String Featrued
        {
            get
            {
                return this.featrued;
            }
            set
            {
                this.featrued = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public String Distance
        {
            get
            {
                return this.distance;
            }
            set
            {
                this.distance = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public Boolean IsInner
        {
            get
            {
                return this.isinner;
            }
            set
            {
                this.isinner = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public String DPURL
        {
            get
            {
                return this.dpurl;
            }
            set
            {
                this.dpurl = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public Int32? State
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
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public Int32? Hotelid
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
        public Int32? Shopid
        {
            get
            {
                return this.shopid;
            }
            set
            {
                this.shopid = value;
            }
        }
    }
}
