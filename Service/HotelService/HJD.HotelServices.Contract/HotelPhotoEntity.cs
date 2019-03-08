using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HJD.HotelServices.Contracts
{

    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public sealed class HotelPhotoEntity
    {
        #region 映射字段
        private Int32 id = 0;

        private String title = "";

        private Int32 channelid = 0;

        private Int32 otaid = 0;

        private Int32 hotelid = 0;

        private Int32 type = 0;

        private Boolean isupdate = false;

        private Boolean deleted = false;

        private DateTime createdate = System.DateTime.Parse("1900-1-1");

        private Boolean iscover = false;

        private String surl = "";
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
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public String Title
        {
            get
            {
                return this.title;
            }
            set
            {
                this.title = value;
            }
        }

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
        public Int32 OTAID
        {
            get
            {
                return this.otaid;
            }
            set
            {
                this.otaid = value;
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
        public Boolean IsUpdate
        {
            get
            {
                return this.isupdate;
            }
            set
            {
                this.isupdate = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Boolean Deleted
        {
            get
            {
                return this.deleted;
            }
            set
            {
                this.deleted = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public DateTime CreateDate
        {
            get
            {
                return this.createdate;
            }
            set
            {
                this.createdate = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Boolean IsCover
        {
            get
            {
                return this.iscover;
            }
            set
            {
                this.iscover = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public String SURL
        {
            get
            {
                return this.surl;
            }
            set
            {
                this.surl = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public String URL
        {
            get;
            set;
        }
    }
}

