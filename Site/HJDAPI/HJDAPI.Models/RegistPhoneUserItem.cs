using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace HJDAPI.Models
{
    [DataContract]
    public class RegistPhoneUserItem:BaseParam
    {    
        /// <summary>
        /// 手机号
        /// </summary>
        [DataMember]
        public string Phone { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        [DataMember]
        public string ConfirmPassword { get; set; }
 
        /// <summary>
        /// 密码
        /// </summary>
        [DataMember]
        public string Password { get; set; }
        /// <summary>
        /// 邀请码
        /// </summary>
        [DataMember]
        public string InvitationCode { get; set; }
        /// <summary>
        /// 渠道号
        /// </summary>
        [DataMember]
        public long CID { get; set; }
        /// <summary>
        /// 昵称
        /// </summary>
        [DataMember]
        public string NickName { get; set; }
        /// <summary>
        /// 临时密码
        /// </summary>
        [DataMember]
        public bool IsTemporaryPWD { get; set; }
        /// <summary>
        /// 渠道号
        /// </summary>
        [DataMember]
        public string Unionid { get; set; }
      
    }

    [DataContract]
    public class UserPriviledgeInsertParam : BaseParam
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        [DataMember]
        public long UserId { get; set; }

        /// <summary>
        /// 权限Id
        /// 2001 铂涛
        /// 2002 magicall
        /// </summary>
        [DataMember]
        public Int32 PriviledgeId { get; set; }

        /// <summary>
        /// 增加或删除权限
        /// </summary>
        [DataMember]
        public bool IsAdd { get; set; }
    }
}