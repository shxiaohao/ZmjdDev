using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Models
{
    /// <summary>
    /// 操作结果
    /// </summary>
    [Serializable]
    [DataContract]
    public class UserInfoResult
    {
        public UserInfoResult()
        {
            UserID = 0;
            Email = "";
            Mobile = "";
            Avatar = "";
            PersonalSignature = "";
            CustomerTypeDescribe = "";
            CustomerTypeInterests = "";
            RealName = "";
            VIPCID = 0;
            NickName = "";
        }
        
        /// <summary>
        /// 用户昵称
        /// </summary>
        [DataMember]
        public string NickName { get; set; }
        /// <summary>
        /// 购买VIP的CID
        /// </summary>
        [DataMember]
        public Int64 VIPCID { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        [DataMember]
        public long UserID { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string Mobile { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        [DataMember]
        public string Avatar { get; set; }
        /// <summary>
        /// 粉丝数
        /// </summary>
        [DataMember]
        public int FollowersCount { get; set; }
        /// <summary>
        /// 关注数
        /// </summary>
        [DataMember]
        public int FollowingsCount { get; set; }
        /// <summary>
        /// 个性签名
        /// </summary>
        [DataMember]
        public string PersonalSignature { get; set; }
        /// <summary>
        /// 用户身份
        /// </summary>
        [DataMember]
        public int CustomerType { get; set; }
        /// <summary>
        /// 用户身份描述
        /// </summary>
        [DataMember]
        public string CustomerTypeDescribe { get; set; }
        /// <summary>
        /// 用户身份权益
        /// </summary>
        [DataMember]
        public string CustomerTypeInterests { get; set; }
        /// <summary>
        /// 真实姓名
        /// </summary>
        [DataMember]
        public string RealName { get; set; }
        /// <summary>
        /// 购买vip时间
        /// </summary>
        [DataMember]
        public DateTime StartVipTime { get; set; }
        /// <summary>
        /// vip到期时间
        /// </summary>
        [DataMember]
        public DateTime EndVipTime { get; set; }

        /// <summary>
        /// 邀请码
        /// </summary>
        [DataMember]
        public string InvitationCode { get; set; }

        /// <summary>
        /// 节省金额
        /// </summary>
        [DataMember]
        public decimal SaveMoney { get; set; }

        /// <summary>
        /// 当前积分
        /// </summary>
        [DataMember]
        public int CanUsePoints { get; set; }

    }
}
