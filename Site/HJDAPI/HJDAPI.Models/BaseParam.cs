using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    /// <summary>
    //TimeStamp: int64 时间戳 1970年1月1日零时零分零秒 到当前Utc时间的秒数，
    //SourceID: int 32 来源ID 1：ios 2: android 3:web 4:msite
    //RequestType: string 请求接口服务的名称,
    //Sign: string 签名
    /// </summary>
    [Serializable]
    [DataContract]
    public class BaseParam
    {
        [DataMember]
        public Int64 TimeStamp { get; set; }//：计算从1970年到现在的秒数；
        [DataMember]
        public long  SourceID { get; set; }//：来源ID数值： 1：ios 2: android 3:web 4:msite；
        [DataMember]
        public string RequestType { get; set; }//：请求接口服务的名称， SubmitOrder40；
        [DataMember]
        public string Sign { get; set; }//: 请求签名    
    }

    [Serializable]
    [DataContract]
    public class BaseSignParam
    {
        /// <summary>
        /// 商家代码 铂汇金融 写 bohuijinrong
        /// </summary>
        [DataMember]
        public string merchantcode { get; set; }
        [DataMember]
        public Int64 timestamp { get; set; }//计算从1970年1月1日0时0分到现在(协调世界时)的秒数；
        [DataMember]
        public string noncestr { get; set; }//随机字符串
        [DataMember]
        public string extrefnumber { get; set; }//当次调用标识（guid）用于接口调用核对
        [DataMember]
        public string apikey { get; set; }//接口调用编码，由铂汇提供，（联调环境使用dev）
        [DataMember]
        public string sign { get; set; }//MD5签名串
    }
}