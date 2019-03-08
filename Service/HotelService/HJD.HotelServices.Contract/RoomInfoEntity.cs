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
    public sealed class RoomInfoEntity
    {

        #region 映射字段
        private Int32 channelid = 0;

        private Int32 hotelid = 0;

        private Int32 roomid = 0;

        private String roomname = "";

        private String area = "";

        private String floor = "";

        private Int32? broadband = 0;

        private Int32? standardoccupancy = 0;

        private Int32? bedtype = 0;

        private Decimal? bedsize = 0m;

        private Boolean canaddbed = false;

        private String others = "";

        private String pics = "";
        #endregion

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32 ChannelID
        {
            get
            {
                return this.channelid;
            }
            set
            {
                this.channelid = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
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
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
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
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public String RoomName
        {
            get
            {
                return this.roomname;
            }
            set
            {
                this.roomname = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public String Area
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
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public String Floor
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
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32? Broadband
        {
            get
            {
                return this.broadband;
            }
            set
            {
                this.broadband = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32? StandardOccupancy
        {
            get
            {
                return this.standardoccupancy;
            }
            set
            {
                this.standardoccupancy = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32? BedType
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
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Decimal? BedSize
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
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Boolean CanAddBed
        {
            get
            {
                return this.canaddbed;
            }
            set
            {
                this.canaddbed = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public String Others
        {
            get
            {
                return this.others;
            }
            set
            {
                this.others = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public String Pics
        {
            get
            {
                return this.pics;
            }
            set
            {
                this.pics = value;
            }
        }
    }
}
