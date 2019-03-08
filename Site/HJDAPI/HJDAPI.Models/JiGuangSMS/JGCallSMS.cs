using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HJDAPI.Models.JiGuangSMS
{

    /// <summary>
    /// 基础参数
    /// </summary>
   public class JGCallSMS
    {
        public long nonce { get; set; }

        public string signature { get; set; }

        public long timestamp { get; set; }

        public string type { get; set; }

        public string data { get; set; }
    }

    /// <summary>
    /// 下行消息送达状态回调参数
    /// </summary>
    public class DownParam
    {



        public string msgId { get; set; }
        public long status { get; set; }

        public string receiveTime { get; set; }

        public string phone { get; set; }

    }


    /// <summary>
    /// 上行消息内容回调
    /// </summary>
    public class UpParam
    {

        public string phone { get; set; }

        public string replyTime { get; set; }

        public string content { get; set; }


    }



    /// <summary>
    ///模板审核
    /// </summary>
    public class TemplateAudit
    {


        public int tempId { get; set; }


        public int status { get; set; }



        public string refuseReason { get; set; }




    }


    /// <summary>
    /// 签名审核
    /// </summary>
    public class SignAudit
    {


        public int signId { get; set; }


        public int status { get; set; }



        public string refuseReason { get; set; }

    }




}
