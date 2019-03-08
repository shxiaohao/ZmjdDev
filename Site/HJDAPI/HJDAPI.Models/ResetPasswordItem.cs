using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace HJDAPI.Models
{
    [DataContract]
    public class ResetPasswordItem:BaseParam
    {
        /// <summary>
        /// 验证码
        /// </summary>
        [DataMember]
        public string confirmCode { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [DataMember]
        public string Phone { get; set; }


        /// <summary>
        /// 密码
        /// </summary>
        [DataMember]
        public string newpassword { get; set; }

      
    } 
}
