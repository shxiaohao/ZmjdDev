using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    public class YZXSMSEntity
    {
        /// <summary>
        /// 上行标识符
        /// </summary>
        public string moid { get; set; }
        /// <summary>
        /// 短信发送端手机号码
        /// </summary>
        public string mobile { get; set; }
        /// <summary>
        /// 短信内容
        /// </summary>
        public string content { get; set; }
        /// <summary>
        /// 签名字段
        /// </summary>
        public string sign { get; set; }
        /// <summary>
        /// 上行时间
        /// </summary>
        public string reply_time { get; set; }


    }
}
