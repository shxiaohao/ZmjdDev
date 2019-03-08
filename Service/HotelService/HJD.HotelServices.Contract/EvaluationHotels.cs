using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Runtime.Serialization;
using HJD.Framework.Entity;


namespace HJD.HotelServices.Contracts
{
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public class InspectorHotel
    {
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32 ID
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32 HotelID
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string HotelName
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Boolean IsValid
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Boolean IsExpired
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32 ApplicationCount
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32 MaxCount
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32 LeaveCount
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public DateTime ExpiredDate
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public DateTime CreateTime
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public DateTime UpdateTime
        {
            get;
            set;
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public long Creator
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public long Updator
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public String Description
        {
            get;
            set;
        }

        /// <summary>
        /// 备注
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public String Note
        {
            get;
            set;
        }

        /// <summary>
        /// 需要的积分
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int RequiredPoint
        {
            get;
            set;
        }

        /// <summary>
        /// 排名
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int Rank
        {
            get;
            set;
        }

        /// <summary>
        /// 可用日期
        /// 0.通用 1.周末不可用 2.节假日不可用 3.周末节假日不可用
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int UseDateType { get; set; }

        /// <summary>
        /// 套餐ID
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public int PackageId { get; set; }
    }
}