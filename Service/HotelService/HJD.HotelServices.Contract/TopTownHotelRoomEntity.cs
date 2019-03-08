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
    public class TopTownHotelRoomEntity
    {
        #region 映射字段
        private int hotelid = 0;

        private int hoteloriid = 0;

        private int toptownhotelid = 0;

        private int roomid = 0;

        private string roomtypename = "";

        private string hotelcode = "";

        private string bedtypename = "";

        private int maxaddbed = 0;

        private string roomtypecode = "";

        #endregion

        /// <summary>
        /// 床型Code
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string RoomTypeCode
        {
            get { return this.roomtypecode; }
            set { this.roomtypecode = value; }
        }

        /// <summary>
        /// 床型
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string BedTypeName
        {
            get { return this.bedtypename; }
            set { this.bedtypename = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
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
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int HotelOriID
        {
            get { return this.hoteloriid; }
            set { this.hoteloriid = value; }
        }

        /// <summary>
        /// 大都市hotelid
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int TopTownHotelID
        {
            get
            {
                return this.toptownhotelid;
            }
            set
            {
                this.toptownhotelid = value;
            }
        }

        /// <summary>
        /// 房型id
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int RoomId
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
        /// 房型
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string RoomTypeName
        {
            get
            {
                return this.roomtypename;
            }
            set
            {
                this.roomtypename = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string HotelCode
        {
            get { return this.hotelcode; }
            set { this.hotelcode = value; }
        }

        /// <summary>
        /// 最大加床数
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int MaxAddBed
        {
            get { return this.maxaddbed; }
            set { this.maxaddbed = value; }
        }



    }
}
