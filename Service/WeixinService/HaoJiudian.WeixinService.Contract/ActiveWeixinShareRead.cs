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
    public class ActiveWeixinShareRead
    {
        private int id = 0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        private int activeID = 0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public int ActiveID
        {
            get { return activeID; }
            set { activeID = value; }
        }

        private string sharerOpenid = "";
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public string SharerOpenid
        {
            get { return sharerOpenid; }
            set { sharerOpenid = value; }
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

        private int luckCode = 0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public int LuckCode
        {
            get { return luckCode; }
            set { luckCode = value; }
        }

        private int readCount = 0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public int ReadCount
        {
            get { return readCount; }
            set { readCount = value; }
        }

        private DateTime lastReadTime;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 
        /// </summary>
        public DateTime LastReadTime
        {
            get { return lastReadTime; }
            set { lastReadTime = value; }
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
