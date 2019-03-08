using HJD.Framework.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace HJD.HotelServices.Contracts
{

    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public sealed class HotelReviewDelLogEntity
    {

        #region 映射字段
        private Int32 id = 0;

        private String logtype;

        private Int32 deltype = 0;

        private Int32 delgrade = 0;

        private String delreason = "";

        private Int32 ugctype = 0;

        private Int32 ugcid = 0;

        private DateTime datetime = System.DateTime.Parse("1900-1-1");

        private String operatoruid = "";
        #endregion

        /// <summary>
        /// 
        /// </summary>
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
        public String LogType
        {
            get
            {
                return this.logtype;
            }
            set
            {
                this.logtype = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 DelType
        {
            get
            {
                return this.deltype;
            }
            set
            {
                this.deltype = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 DelGrade
        {
            get
            {
                return this.delgrade;
            }
            set
            {
                this.delgrade = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String DelReason
        {
            get
            {
                return this.delreason;
            }
            set
            {
                this.delreason = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 UGCType
        {
            get
            {
                return this.ugctype;
            }
            set
            {
                this.ugctype = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 UGCID
        {
            get
            {
                return this.ugcid;
            }
            set
            {
                this.ugcid = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime DateTime
        {
            get
            {
                return this.datetime;
            }
            set
            {
                this.datetime = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String OperatorUID
        {
            get
            {
                return this.operatoruid;
            }
            set
            {
                this.operatoruid = value;
            }
        }
    }
}