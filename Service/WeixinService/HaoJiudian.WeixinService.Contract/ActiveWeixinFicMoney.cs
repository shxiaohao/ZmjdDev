using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using HJD.Framework.Entity;

namespace HJD.WeixinServices.Contracts
{
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    [HJD.Framework.Entity.DefaultColumnAttribute()]
    public class ActiveWeixinFicMoney
    {
        private int id = 0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        private int activeId = 0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public int ActiveId
        {
            get { return activeId; }
            set { activeId = value; }
        }

        private string openid = "";
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public string Openid
        {
            get { return openid; }
            set { openid = value; }
        }

        private int value2 = 0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public int Value
        {
            get { return value2; }
            set { value2 = value; }
        }

        private int partnerId = 0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public int PartnerId
        {
            get { return partnerId; }
            set { partnerId = value; }
        }

        private string remark;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public string Remark
        {
            get { return remark; }
            set { remark = value; }
        }

        private DateTime createTime;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateTime
        {
            get { return createTime; }
            set { createTime = value; }
        }
    }
}
