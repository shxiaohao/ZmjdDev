using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models.JiGuangSMS
{

    /// <summary>
    /// 短信发送结果
    /// </summary>
    [System.Runtime.Serialization.DataContractAttribute]
    public class TemplateSendResult
    {
        [System.Runtime.Serialization.DataMemberAttribute]
        public string msg_id { get; set; }

        [System.Runtime.Serialization.DataMemberAttribute]
        public ErrorResult error { get; set; }


    }

    /// <summary>
    /// 发送失败错误信息
    /// </summary>
    [System.Runtime.Serialization.DataContractAttribute]
    public class ErrorResult {
        [System.Runtime.Serialization.DataMemberAttribute]
        public string code { get; set; }
        [System.Runtime.Serialization.DataMemberAttribute]
        public string message { get; set; }

    } 
}
