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
    public class ActiveWeixinPartner
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

        private string partnerCode = "";
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public string PartnerCode
        {
            get { return partnerCode; }
            set { partnerCode = value; }
        }

        private string name = "";
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string brief = "";
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public string Brief
        {
            get { return brief; }
            set { brief = value; }
        }

        private string qrCodeUrl = "";
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public string QrCodeUrl
        {
            get { return qrCodeUrl; }
            set { qrCodeUrl = value; }
        }

        private string logoUrl;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public string LogoUrl
        {
            get { return logoUrl; }
            set { logoUrl = value; }
        }

        private int luckCodeCount;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public int LuckCodeCount
        {
            get { return luckCodeCount; }
            set { luckCodeCount = value; }
        }

        private int state;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public int State
        {
            get { return state; }
            set { state = value; }
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

        private decimal activefund;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public decimal ActiveFund
        {
            get { return activefund; }
            set { activefund = value; }
        }

        private int activejoincost;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public int ActiveJoinCost
        {
            get { return activejoincost; }
            set { activejoincost = value; }
        }
        private decimal activeminreward;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public decimal ActiveMinReward
        {
            get { return activeminreward; }
            set { activeminreward = value; }
        }
        private decimal activemaxreward;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public decimal ActiveMaxReward
        {
            get { return activemaxreward; }
            set { activemaxreward = value; }
        }

        private string shortname;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public string ShortName
        {
            get { return shortname; }
            set { shortname = value; }
        }
        
    }
}
