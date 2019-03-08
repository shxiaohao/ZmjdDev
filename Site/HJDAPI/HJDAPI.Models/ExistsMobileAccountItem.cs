using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
  
    [DataContract]
    public class ExistsMobileAccountItem:BaseParam
    {
         /// <summary>
        /// 手机号
        /// </summary>
        [DataMember]
        public string Phone { get; set; }
    }


    [DataContract]
    public class ExistsCodeItem : BaseParam
    {
        /// <summary>
        /// 手机号
        /// </summary>
        [DataMember]
        public string Phone { get; set; }
        /// <summary>
        /// 邀请码
        /// </summary>
        [DataMember]
        public string InvitationCode { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        [DataMember]
        public string SMSCode { get; set; }
    }

}
