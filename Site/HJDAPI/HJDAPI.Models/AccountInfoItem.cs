using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace HJDAPI.Models
{
    [DataContract]
    public class AccountInfoItem : BaseParam
    {
        /// <summary>
        /// 验证码
        /// </summary>
        [DataMember]
        public string ConfirmCode { get; set; }
        /// <summary>
        /// 临时密码
        /// </summary>
        [DataMember]
        public bool IsTemporaryPWD { get; set; }
        /// <summary>
        /// 邀请人Userid
        /// </summary>
        [DataMember]
        public long FriendUserId { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        [DataMember]
        public string Email { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [DataMember]
        public string Phone { get; set; }


        /// <summary>
        /// 手机帐号状态  0：末验证 1：已验证 2. 已设置密码，可登录
        /// </summary>
        [DataMember]
        public int MobileState { get; set; }


        /// <summary>
        /// 密码
        /// </summary>
        [DataMember]
        public string Password { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        [DataMember]
        public string NickName { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [DataMember]
        public string RemoteAddress { get; set; }

        /// <summary>
        /// 自动登录
        /// </summary>
        [DataMember]
        public bool AutoLogin { get; set; }

        /// <summary>
        /// 是否是手机登录 1为手机 0为其他
        /// </summary>
        [DataMember]
        public int IsMobile { get; set; }



        /// <summary>
        /// 渠道号
        /// </summary>
        [DataMember]
        public long CID { get; set; }



        //第三方登录相关 开始
        /// <summary>
        /// 第三方的ID
        /// </summary>
        [DataMember]
        public string Uid { get; set; }

        /// <summary>
        /// 第三方类型
        /// </summary>
        
        [DataMember]
        public string BindNickName { get; set; }

        [DataMember]
        public int LoginType { get; set; }

        [DataMember]
        public string AccessToken { get; set; }

        [DataMember]
        public string AccessTokenSecret { get; set; }

        //第三方登录相关 结束
    } 

    [DataContract]
    public class AccountAvatarParam : BaseParam
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [DataMember]
        public long userID { get; set; }

        /// <summary>
        /// 头像链接
        /// </summary>
        [DataMember]
        public string avatarSUrl { get; set; }
    }

    [DataContract]
    public class ChangeFollowerFollowingParam : BaseParam
    {
        /// <summary>
        /// 粉丝userID
        /// </summary>
        [DataMember]
        public long follower { get; set; }

        /// <summary>
        /// 被关注者userID
        /// </summary>
        [DataMember]
        public long following { get; set; }

        /// <summary>
        /// 关注状态 0 取消关注 1 关注
        /// </summary>
        [DataMember]
        public bool isValid { get; set; }
    }

    [DataContract]
    public class AccountMemberBriefParam : BaseParam
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [DataMember]
        public long userID { get; set; }

        /// <summary>
        /// 个性签名
        /// </summary>
        [DataMember]
        public string personalSignature { get; set; }
    }

    [DataContract]
    public class InspectorDescPicSurlParam : BaseParam
    {
        /// <summary>
        /// 品鉴师userID
        /// </summary>
        [DataMember]
        public long userID { get; set; }

        /// <summary>
        /// App首页推荐关注的描述图片 短链接
        /// </summary>
        [DataMember]
        public string desPicUrl { get; set; }
    }

    [DataContract]
    public class PersonalThemeParam : BaseParam
    {
        /// <summary>
        /// 个人主题CodeSN
        /// </summary>
        [DataMember]
        public string themeCodeSN { get; set; }

        /// <summary>
        /// 个人主题名称
        /// </summary>
        [DataMember]
        public string themeName { get; set; }

        /// <summary>
        /// 主题来源 默认还是其他扩展
        /// </summary>
        [DataMember]
        public int type { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [DataMember]
        public int userID { get; set; }
    }
    
    [DataContract]
    public class SysMsgParam : BaseParam
    {
        /// <summary>
        /// 当前用户ID
        /// </summary>
        [DataMember]
        public long curUserID { get; set; }

        /// <summary>
        /// 从第几条开始分页
        /// </summary>
        [DataMember]
        public int start { get; set; }

        /// <summary>
        /// 每页显示多少条消息
        /// </summary>
        [DataMember]
        public int count { get; set; }
    }
}