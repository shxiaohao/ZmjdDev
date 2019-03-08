using HJD.AccountServices.Entity;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HJDAPI.Models
{
    /// <summary>
    /// 操作结果
    /// </summary>
    [Serializable]
    [DataContract]
    public class OperationResult:BaseParam
    {
        public OperationResult()
        {
            UserID = 0;
            Data = "";
            Email = "";
            Mobile = "";
            Message = "";
            TrueName = "";
            Avatar = "";
            PersonalSignature = "";
            PersonalSignatureMaxLength = 80;
            ThemeCodeSN = "";
            UserPrivileges = new List<UserPrivilegeRel>();
            UserPrivilegeNames = new List<string>();
            CustomerTypeDescribe = "";
            CustomerTypeInterests = "";
            CustomerTypeInterestsUrl = "";
            InvitationCode = "";
            IsTemporaryPWD = false;
            SaveMoneyDesc = "";
            HelloWord = "";
            HelloTip = "";
        }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string HelloTip { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string HelloWord { get; set; }

        /// <summary>
        ///节省金额描述
        /// </summary>
        [DataMember]
        public string SaveMoneyDesc { get; set; }
        /// <summary>
        /// 是否是临时密码
        /// </summary>
        [DataMember]
        public bool IsTemporaryPWD { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        [DataMember]
        public long UserID { get; set; }
        
        /// <summary>
        /// 操作是否成功
        /// </summary>
        [DataMember]
        public bool Success { get; set; }

        /// <summary>
        /// 操作失败时的错误消息
        /// </summary>
        [DataMember]
        public string Message { get; set; }

        /// <summary>
        /// 附加数据
        /// </summary>
        [DataMember]
        public string Data { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string Mobile { get; set; }

        [DataMember]
        public string TrueName { get; set; }

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
        /// 用户在用的主题
        /// </summary>
        [DataMember]
        public string ThemeCodeSN { get; set; }

        /// <summary>
        /// 个性签名最大字数
        /// </summary>
        [DataMember]
        public int PersonalSignatureMaxLength { get; set; }

        /// <summary>
        /// 用户权限列表
        /// </summary>
        [DataMember]
        public List<UserPrivilegeRel> UserPrivileges { get; set; }

        /// <summary>
        /// 用户权限Name列表
        /// </summary>
        [DataMember]
        public List<string> UserPrivilegeNames { get; set; }
        
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
        /// 用户身份权益URL
        /// </summary>
        [DataMember]
        public string CustomerTypeInterestsUrl { get; set; }
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
        public string EndVipTime { get; set; }
        /// <summary>
        ///邀请码
        /// </summary>
        [DataMember]
        public string InvitationCode { get; set; }

    }

    /// <summary>
    /// 品鉴师个人提交数据
    /// </summary>
    [Serializable]
    [DataContract]
    public class InspectorApplyData
    {
        /// <summary>
        /// UserID
        /// </summary>
        [DataMember]
        public long UserID { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [DataMember]
        public string MobilePhone { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        [DataMember]
        public string TrueName { get; set; }

        [DataMember]
        public string Job { get; set; }

        [DataMember]
        public string JobExperience { get; set; }

        [DataMember]
        public string JobSpecialty { get; set; }

        [DataMember]
        public List<UserTagRelEntity> Tags { get; set; }
    }

    /// <summary>
    /// 操作结果
    /// </summary>
    [Serializable]
    [DataContract]
    public class UploadAvatarResult : BasePostResult
    {
        public UploadAvatarResult()
        {
            Success = false;
            Message = "";
            Avatar = "";
        }
        
        /// <summary>
        /// 头像
        /// </summary>
        [DataMember]
        public string Avatar { get; set; }
    }

    [Serializable]
    [DataContract]
    public class ChangeFollowerFollowingResult : BasePostResult
    {
        /// <summary>
        /// 更新后的粉丝数
        /// </summary>
        [DataMember]
        public int FollowersCount { get; set; }

        /// <summary>
        /// 更新后的关注数
        /// </summary>
        [DataMember]
        public int FollowingsCount { get; set; }

        /// <summary>
        /// 他人粉丝数
        /// </summary>
        [DataMember]
        public int OthersFollowersCount { get; set; }

        /// <summary>
        /// 他人关注数
        /// </summary>
        [DataMember]
        public int OthersFollowingsCount { get; set; }

        /// <summary>
        /// 更新后的关注状态 0 未关注 1 已关注 2 相互关注
        /// </summary>
        [DataMember]
        public int FollowState { get; set; }
    }

    /// <summary>
    /// 操作结果
    /// </summary>
    [Serializable]
    [DataContract]
    public class AccountMemberBriefResult : BasePostResult
    {
        /// <summary>
        /// 头像
        /// </summary>
        [DataMember]
        public string PersonalSignature { get; set; }
    }

    /// <summary>
    /// 关注或粉丝列表
    /// </summary>
    [Serializable]
    [DataContract]
    public class FollowerFollowingRelItem
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [DataMember]
        public long UserID { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        [DataMember]
        public string NickName { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        [DataMember]
        public string AvatarUrl { get; set; }

        ///// <summary>
        ///// 是否相互关注
        ///// </summary>
        //[DataMember]
        //public bool IsFollowEachOther { get; set; }

        /// <summary>
        /// 关注状态 0没有关系 1粉丝 2关注 3相互关注
        /// </summary>
        [DataMember]
        public int FollowState
        {
            get;
            set;
        }
    }

    [Serializable]
    [DataContract]
    public class InspectorDescPicSurlResult:BasePostResult
    {
        /// <summary>
        /// 头像
        /// </summary>
        [DataMember]
        public string DescPicSurl { get; set; }
    }

    [Serializable]
    [DataContract]
    public class UserMsgResult
    {
        [DataMember]
        public int unReadCount { get; set; }
    }

    [Serializable]
    [DataContract]
    public class UserMsgListResult
    {
        [DataMember]
        public int totalCount { get; set; }

        [DataMember]
        public List<SysMsgItem> items { get; set; }
    }

    [Serializable]
    [DataContract]
    public class UserMsgSearchParam : BaseParam
    {
        [DataMember]
        public long curUserID { get; set; }
        [DataMember]
        public int start { get; set; }
        [DataMember]
        public int count { get; set; }
    }

    [Serializable]
    [DataContract]
    public class UserMsgUpdateParam : BaseParam
    {
        [DataMember]
        public long curUserID { get; set; }

        [DataMember]
        public List<long> ids { get; set; }

        [DataMember]
        public int state { get; set; }
    }

    [Serializable]
    [DataContract]
    public class SysMsgItem
    {
        /// <summary>
        /// 消息ID
        /// </summary>
        [DataMember]
        public long msgId { get; set; }

        /// <summary>
        /// 发送人昵称 如管理员 某某用户
        /// </summary>
        [DataMember]
        public string senderNickName { get; set; }

        /// <summary>
        /// 发送人UserID
        /// </summary>
        [DataMember]
        public long senderUserID { get; set; }

        /// <summary>
        /// 消息内容提醒,评论或回复
        /// </summary>
        [DataMember]
        public string briefType { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [DataMember]
        public string briefContent { get; set; }

        /// <summary>
        /// 发生时间描述
        /// </summary>
        [DataMember]
        public string timeDesc { get; set; }

        /// <summary>
        /// 头像链接
        /// </summary>
        [DataMember]
        public string avatarUrl { get; set; }

        /// <summary>
        /// 点击消息内容 前往的页面。无需跳转返回空字符串,需要跳转返回相应的whotel协议头
        /// </summary>
        [DataMember]
        public string jumpUrl { get; set; }

        /// <summary>
        /// 消息当前的状态值 0.未读 1.已读
        /// </summary>
        [DataMember]
        public int state { get; set; }

        /// <summary>
        /// 发送时间
        /// </summary>
        [DataMember]
        public DateTime sendDate { get; set; }
    }

    /// <summary>
    /// 基本操作结果信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class BasePostResult
    {
        /// <summary>
        /// 操作是否成功
        /// </summary>
        [DataMember]
        public bool Success { get; set; }

        /// <summary>
        /// 操作失败时的错误消息
        /// </summary>
        [DataMember]
        public string Message { get; set; }
    }

    [Serializable]
    [DataContract]
    public class GenTrackCodeParam
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [DataMember]
        public long UserID { get; set; }
        /// <summary>
        /// 跟踪页面
        /// </summary>
        [DataMember]
        public ZMJDShareTrackBizType BizType { get; set; }
        /// <summary>
        ///酒店ID
        /// </summary>
        [DataMember]
        public int HotelID { get; set; }
        /// <summary>
        /// 点评ID
        /// </summary>
        [DataMember]
        public int CommentID { get; set; }
        /// <summary>
        /// 房券活动ID
        /// </summary>
        [DataMember]
        public int CouponActivityID { get; set; }
        /// <summary>
        /// 地区ID
        /// </summary>
        [DataMember]
        public int DistrictID { get; set; }
        /// <summary>
        /// 主题ID
        /// </summary>
        [DataMember]
        public int Interest { get; set; }
    }

    public enum ZMJDShareTrackBizType
    {
        commentdetail = 1,
        hoteldetail = 2,
        districtinterestlist = 3,
        roomcoupondetail = 4,
        roomgroupcoupondetail = 5,
        festivalroom = 6,
        recommendfriend = 7
    }
     

    [Serializable]
    [DataContract]
    public class TrackCodeData
    {
        /// <summary>
        /// 完整的带跟踪数据的分享链接
        /// </summary>
        [DataMember]
        public string ShareLink { get; set; }

        /// <summary>
        /// 分享链接用于跟踪的加密字符串
        /// </summary>
        [DataMember]
        public string EncodeStr { get; set; }
    }

    [Serializable]
    [DataContract]
    public class CommonRecordParam
    {
        /// <summary>
        /// 点评ID、酒店ID、地区ID
        /// </summary>
        [DataMember]
        public long busniessId { get; set; }

        /// <summary>
        /// 业务记录类型
        /// 1.点评 2.酒店 3.度假地区(App首页选择)  4.度假地区（App发现页选择）
        /// </summary>
        [DataMember]
        public int businessType { get; set; }

        /// <summary>
        /// 操作人UserID
        /// </summary>
        [DataMember]
        public long userID { get; set; }

        /// <summary>
        /// 用户未登录 openID(微信专用凭证)
        /// </summary>
        [DataMember]
        public string openID { get; set; }

        /// <summary>
        /// 用户未登录 记录sessionID UUID
        /// </summary>
        [DataMember]
        public string sessionID { get; set; }

        /// <summary>
        /// 客户端IP
        /// </summary>
        [DataMember]
        public string clientIP { get; set; }

        /// <summary>
        /// 操作发生的终端类型 iOS web weixin android
        /// 0.wap 1.web 2.iOS 3.android 4.weixin
        /// </summary>
        [DataMember]
        public int terminalType { get; set; }

        /// <summary>
        /// app版本号
        /// </summary>
        [DataMember]
        public string appVersion { get; set; }

        /// <summary>
        /// 记录类型 如分享 或者 浏览
        /// 1.分享 2.浏览 3.搜索
        /// </summary>
        [DataMember]
        public int recordType { get; set; }
    }

    public class Comment500Param : CommonRecordParam
    {
        /// <summary>
        /// 点评ID
        /// </summary>
        [DataMember]
        public int commentID { get; set; }
    }

    [Serializable]
    [DataContract]
    public class UpdateUserMsgState : BasePostResult
    {
        /// <summary>
        /// 未读消息数
        /// </summary>
        [DataMember]
        public int unReadCount { get; set; }
    }

    /// <summary>
    /// 基本操作结果信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class BasePostResponse
    {
        /// <summary>
        /// 操作结果是否成功
        /// </summary>
        [DataMember]
        public bool result { get; set; }

        /// <summary>
        /// 返回信息
        /// </summary>
        [DataMember]
        public string data { get; set; }

        /// <summary>
        /// 操作失败时的错误代码
        /// </summary>
        [DataMember]
        public string errorcode { get; set; }

        /// <summary>
        /// 详细的错误说明
        /// </summary>
        [DataMember]
        public string errormsg { get; set; }
    }

    [Serializable]
    [DataContract]
    public class ThirdPartyUserInfoParam : BaseSignParam
    {
        [DataMember]
        public List<BasicUserInfoParam> userinfos { get; set; }
    }

    [Serializable]
    [DataContract]
    public class ThirdPartyRequestProductDataParam : BaseSignParam
    {
        [DataMember]
        public string merchantcode { get; set; }
        [DataMember]
        public string starttime { get; set; }
    }

    [Serializable]
    [DataContract]
    public class ThirdPartyModifyPhoneParam : BaseSignParam
    {
        /// <summary>
        /// 老手机号
        /// </summary>
        [DataMember]
        public string oldphone { get; set; }

        /// <summary>
        /// 新手机号
        /// </summary>
        [DataMember]
        public string newphone { get; set; }

        /// <summary>
        /// 商家代码 铂汇金融 写 bohuijinrong
        /// </summary>
        [DataMember]
        public string merchantcode { get; set; }
    }

    [Serializable]
    [DataContract]
    public class BasicUserInfoParam
    {
        /// <summary>
        /// 手机号数组 英文半角逗号相隔
        /// </summary>
        [DataMember]
        public string mobilephone { get; set; }

        /// <summary>
        /// 真名
        /// </summary>
        [DataMember]
        public string truename { get; set; }
        
        /// <summary>
        /// 身份证号
        /// </summary>
        [DataMember]
        public string identitycard { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [DataMember]
        public string gender { get; set; }
    }

    [Serializable]
    [DataContract]
    public class CommentSharePageData
    {
        /// <summary>
        /// 积分计算结果
        /// </summary>
        [DataMember]
        public CalCommentPointResult pointResult { get; set; }

        /// <summary>
        /// 点评分享数据
        /// </summary>
        [DataMember]
        public CommentShareModel commentShare { get; set; }
    }

    [Serializable]
    [DataContract]
    public class CalCommentPointResult
    {
        /// <summary>
        /// 可获得多少积分
        /// </summary>
        [DataMember]
        public int point { get; set; }

        /// <summary>
        /// 品鉴活动需要的积分数
        /// </summary>
        [DataMember]
        public int needPoint { get; set; }
    }
}