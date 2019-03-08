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
    public sealed class HotelOTAEntity
    {
        
        #region 映射字段
        private Int32 hotelid = 0;
        
        private Int32 hoteloriid = 0;
        
        private Boolean isvalid = true;
        
        private Int16 channelid = 0;
        
        private Boolean cansyncprice = true;
        
        private DateTime createtime = System.DateTime.Now;



        private String channelcode = "";

        private String channelname = "";

        private String url = "";

        #endregion
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional=true)]
        public Int32 HotelId
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
        public Int32 HotelOriID
        {
            get
            {
                return this.hoteloriid;
            }
            set
            {
                this.hoteloriid = value;
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
        public Int16 ChannelID
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
        [HJD.Framework.Entity.DBColumnAttribute(DefaultValue="((1))", IsOptional=true)]
        public Boolean CanSyncPrice
        {
            get
            {
                return this.cansyncprice;
            }
            set
            {
                this.cansyncprice = value;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(DefaultValue="(getdate())", IsOptional=true)]
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
        public String Url
        {
            get
            {
                return this.url;
            }
            set
            {
                this.url = value;
            }
        }
    }
}
