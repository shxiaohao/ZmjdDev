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
    public class ResponseEntity
    {
        private int responseType = 0;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 返回类型
        /// </summary>
        public int ResponseType
        {
            get { return responseType; }
            set { responseType = value; }
        }

        private string responseContent = "";
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 返回内容
        /// </summary>
        public string ResponseContent
        {
            get { return responseContent; }
            set { responseContent = value; }
        }

        private string responseEvent = "";
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 返回事件
        /// </summary>
        public string ResponseEvent
        {
            get { return responseEvent; }
            set { responseEvent = value; }
        }

        private RequestEntity requestEntity;
        [System.Runtime.Serialization.DataMemberAttribute()]
        [HJD.Framework.Entity.DBColumnAttribute(IsOptional = true)]
        /// <summary>
        /// 返回请求对象
        /// </summary>
        public RequestEntity RequestEntity
        {
            get { return requestEntity; }
            set { requestEntity = value; }
        }
    }
}
