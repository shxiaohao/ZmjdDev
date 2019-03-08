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
    public sealed class TravelPersonEntity
    {
        
        #region 映射字段
        private Int32 id = 0;

        private Int64 userid = 0;

        private String travelpersonname = "";

        private Int32 idtype = 0;

        private String idnumber = "";

        private DateTime createtime = System.DateTime.Parse("1900-1-1");

        private DateTime updatetime = System.DateTime.Parse("1900-1-1");

        private Int32 state = 0;

        private DateTime birthday = System.DateTime.Parse("1900-1-1");
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
        /// 注册人员id
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int64 UserID
        {
            get
            {
                return this.userid;
            }
            set
            {
                this.userid = value;
            }
        }

        /// <summary>
        /// 出行人姓名
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public String TravelPersonName
        {
            get
            {
                return this.travelpersonname;
            }
            set
            {
                this.travelpersonname = value;
            }
        }

        /// <summary>
        /// 证件类型
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32 IDType
        {
            get
            {
                return this.idtype;
            }
            set
            {
                this.idtype = value;
            }
        }

        /// <summary>
        /// 证件号码
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public String IDNumber
        {
            get
            {
                return this.idnumber;
            }
            set
            {
                this.idnumber = value;
            }
        }

        /// <summary>
        /// 创建时间
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
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
        /// 修改时间
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public DateTime UpdateTime
        {
            get
            {
                return this.updatetime;
            }
            set
            {
                this.updatetime = value;
            }
        }

        /// <summary>
        /// 状态。1.正常2.删除
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public Int32 State
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
        /// 生日
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public DateTime Birthday
        {
            get
            {
                return this.birthday;
            }
            set
            {
                this.birthday = value;
            }
        }


        /// <summary>
        /// 证件类型名称
        /// </summary>
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        public string CardTypeName
        {
            get
            {
                return ((HJD.HotelServices.Contracts.HotelServiceEnums.EnumCardType)(this.idtype)).ToString();
            }
            set
            {
              //  cardTypeName = value;
            }
        }

    }
}
