using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace HJDAPI.Models
{
    [Serializable]
    [DataContract]
    public class WeixinConfig
    {
        // 开启调试模式,调用的所有api的返回值会在客户端alert出来，若要查看传入的参数，可以在pc端打开，参数信息会通过log打出，仅在pc端时才会打印
        [DataMember]
        public bool debug
        {
            get;set;
        }
        // 必填，公众号的唯一标识
        [DataMember]
        public string appId
        {
            get;set;
        }
        // 必填，生成签名的时间戳
        [DataMember]
        public Int64 timestamp
        {
            get;set;
        }
        // 必填，生成签名的随机串
        [DataMember]
        public string nonceStr
        {
            get;set;
        }
        // 必填，签名，SHA1加密
        [DataMember]
        public string signature
        {
            get;set;
        }
        // 必填，需要使用的JS接口列表
        [DataMember]
        public string jsApiList
        {
            get;set;
        }

        //加密需要的url
        [DataMember]
        public string url
        {
            get;
            set;
        }
    }
}
