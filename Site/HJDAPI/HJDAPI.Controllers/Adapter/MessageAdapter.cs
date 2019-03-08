using HJD.AccountServices.Entity;
using HJD.CommentService.Contract;
using HJD.Framework.WCF;
using HJD.HotelServices.Contracts;
using HJDAPI.Common.Helpers;
using HJDAPI.Common.Security;
using HJDAPI.Controllers.Common;
using HJDAPI.Models;
using MessageService.Contract;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HJDAPI.Controllers.Adapter
{
    public class MessageAdapter
    {
        private readonly static string iOS_SendMessage_appmastersecret = "6il2jalxrx2bkiijvpoopnqp5eawsbml";
        private readonly static string android_SendMessage_appmastersecret = "4zfiyouhwlirztxhnfkkdmaccqy8hheu";

        public readonly static string iOS_SendMessage_appkey = "5296e91656240bade001c381";
        public readonly static string android_SendMessage_appkey = "529545f556240b0a8c15c1aa";


        private readonly static string umengSendHttpMethod = "POST";
        private readonly static string umengSendUrl = "http://msg.umeng.com/api/send";


        private static IMessageService MsgService = ServiceProxyFactory.Create<IMessageService>("basicHttpBinding_IMessageService");

        /// <summary>
        /// 获取用户消息数量
        /// </summary>
        /// <param name="curUserID"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        internal static int GetSysMessageEntityCount(long curUserID, int state)
        {
            return MsgService.GetSysMessageEntityCount(new MessageSearchParam()
            {
                messageType = 0,
                count = 0,
                start = 0,
                state = state,
                receiver = curUserID
            });
        }

        /// <summary>
        /// 获取用户消息列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        internal static IEnumerable<SysMessageEnitity> GetSysMessageEntityList(MessageSearchParam param)
        {
            return MsgService.GetSysMessageEntityList(param);
        }

        internal static int InsertSysNotice(SysNoticeEnitity noticeEntity)
        {
            return MsgService.InsertSysNotice(noticeEntity);
        }

        /// <summary>
        /// 插入新消息内容
        /// </summary>
        /// <param name="sm"></param>
        /// <returns></returns>
        internal static long InsertSysMessage(SysMessageEnitity sm)
        {
            return MsgService.InsertSysMessage(sm);
        }

        /// <summary>
        /// 更新新消息内容
        /// </summary>
        /// <param name="sm"></param>
        /// <returns></returns>
        internal static long UpdateSysMessage(SysMessageEnitity sm)
        {
            return MsgService.UpdateSysMessage(sm);
        }

        /// <summary>
        /// 生成消息列表
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        internal static List<SysMsgItem> GenSysMsgItemListBatch(IEnumerable<SysMessageEnitity> list)
        {
            List<SysMsgItem> msgList = new List<SysMsgItem>();

            if (list != null && list.Count() != 0)
            {
                Dictionary<int, List<long>> dicBizID = new Dictionary<int, List<long>>();
                Dictionary<int, List<SysMessageEnitity>> dicMsg = new Dictionary<int, List<SysMessageEnitity>>();
                foreach (var group in list.GroupBy(_ => _.businessType))
                {
                    dicBizID.Add(group.Key, group.Select(_ => _.businessID).ToList());
                    dicMsg.Add(group.Key, group.ToList());
                }

                foreach (var key in dicBizID.Keys)
                {
                    //根据类型 生成不同的提醒
                    string briefType = "";
                    switch (key)
                    {
                        case 1:
                            briefType = "评论";//"对你的文章发表了评论";
                            break;
                        case 2:
                            briefType = "回复";//"回复了你的评论";
                            break;
                        case 3:
                            briefType = "";//认为您的点评有帮助
                            break;
                        case 4:
                            briefType = "发表了新的点评";
                            break;
                        case 5:
                            briefType = "";//关注了你
                            break;
                        case 6:
                            briefType = "注册成功";//
                            break;
                        case 7:
                            briefType = "评论成功";//
                            break;
                        case 8:
                            briefType = "更换头像";//
                            break;
                        case 9:
                            briefType = "消费积分";//
                            break;
                        case 19:
                            briefType = "邀请注册积分";//
                            break;
                        case 20:
                            briefType = "获邀注册积分";//
                            break;
                        case 21:
                            briefType = "积分过期提醒";//
                            break;
                        case 100:
                            briefType = "官方消息";//周末酒店官方通知
                            break;
                        default:
                            briefType = "通知";
                            break;
                    }

                    if (key == 1 || key == 2)//评论回复
                    {
                        IEnumerable<CommentReviewsEntity> commentReviews = CommentAdapter.GetManyCommentReviewsEntity(dicBizID[key]);
                        msgList.AddRange(commentReviews.Select(_ => new SysMsgItem()
                        {
                            msgId = dicMsg[key].First(j=>j.businessID == _.ID).id,
                            senderNickName = _.SendUserNickName,
                            senderUserID = _.SendUser,
                            avatarUrl = string.IsNullOrWhiteSpace(_.SendUserAvatar) ? DescriptionHelper.defaultAvatar : PhotoAdapter.GenHotelPicUrl(_.SendUserAvatar, Enums.AppPhotoSize.jupiter),
                            briefType = briefType,
                            briefContent = System.Web.HttpUtility.UrlDecode(_.ReviewContent, System.Text.Encoding.UTF8),
                            timeDesc = CommentAdapter.genReviewTimeSpan(_.CreateTime),
                            sendDate = _.CreateTime,
                            jumpUrl = string.Format("whotelapp://www.zmjiudian.com/personal/comments/{0}", _.CommentID),
                            state = dicMsg[key].First(j => j.businessID == _.ID).state
                        }));
                    }
                    else if (key == 3)//点赞
                    {
                        IEnumerable<CommentUsefulEntity> commentUsefuls = CommentAdapter.GetManyCommentUseful(dicBizID[key].Select(_=>(int)_));
                        msgList.AddRange(commentUsefuls.Select(_ => new SysMsgItem()
                        {
                            msgId = dicMsg[key].First(j => j.businessID == _.ID).id,
                            senderNickName = _.UserNickName,
                            senderUserID = _.UserID,
                            avatarUrl = string.IsNullOrWhiteSpace(_.UserAvatar) ? DescriptionHelper.defaultAvatar : PhotoAdapter.GenHotelPicUrl(_.UserAvatar, Enums.AppPhotoSize.jupiter),
                            briefType = briefType,
                            briefContent = "认为您的点评有帮助",
                            timeDesc = CommentAdapter.genReviewTimeSpan(_.UpdateTime),
                            sendDate = _.UpdateTime,
                            jumpUrl = string.Format("whotelapp://www.zmjiudian.com/personal/comments/{0}", _.CommentID),
                            state = dicMsg[key].First(j => j.businessID == _.ID).state
                        }));
                    }
                    else if (key == 4)//新点评
                    {
                        IEnumerable<CommentEntity> comments = CommentAdapter.GetManyCommentBasicInfoEntity(dicBizID[key].Select(_ => (int)_));
                        IEnumerable<long> userIds = comments.Select(_ => _.UserID).Distinct();
                        List<User_Info> userInfos = AccountAdapter.GetUserBasicInfo(userIds).ToList();
                        msgList.AddRange(comments.Select(_ => new SysMsgItem()
                        {
                            msgId = dicMsg[key].First(j => j.businessID == _.ID).id,
                            senderNickName = userInfos.First(j => j.UserId == _.UserID).NickName,
                            senderUserID = userInfos.First(j => j.UserId == _.UserID).UserId,
                            avatarUrl = string.IsNullOrWhiteSpace(userInfos.First(j => j.UserId == _.UserID).AvatarUrl) ? DescriptionHelper.defaultAvatar : PhotoAdapter.GenHotelPicUrl(userInfos.First(j => j.UserId == _.UserID).AvatarUrl, Enums.AppPhotoSize.jupiter),
                            briefType = briefType,
                            briefContent = "",//点评标题或酒店名称
                            timeDesc = CommentAdapter.genReviewTimeSpan(_.EditTime),
                            sendDate = _.EditTime,
                            jumpUrl = string.Format("whotelapp://www.zmjiudian.com/personal/comments/{0}", _.ID),
                            state = dicMsg[key].First(j => j.businessID == _.ID).state
                        }));
                    }
                    else if (key == 5)//新关注 跳转到对方主页
                    {
                        IEnumerable<FollowerFollowingRelEntity> followerfollowings = AccountAdapter.GetManyFollowerFollowings(dicBizID[key]);
                        msgList.AddRange(followerfollowings.Select(_ => new SysMsgItem()
                        {
                            msgId = dicMsg[key].First(j => j.businessID == _.ID).id,
                            senderNickName = _.FollowerNickName,
                            senderUserID = _.Follower,
                            avatarUrl = string.IsNullOrWhiteSpace(_.FollowerAvatarUrl) ? DescriptionHelper.defaultAvatar : PhotoAdapter.GenHotelPicUrl(_.FollowerAvatarUrl, Enums.AppPhotoSize.jupiter),
                            briefType = briefType,
                            briefContent = "关注了你",
                            timeDesc = CommentAdapter.genReviewTimeSpan(dicMsg[key].First(j => j.businessID == _.ID).CreateTime),
                            sendDate = dicMsg[key].First(j => j.businessID == _.ID).CreateTime,
                            jumpUrl = string.Format("whotelapp://www.zmjiudian.com/personal/homepage?userid={0}", _.Follower),
                            state = dicMsg[key].First(j => j.businessID == _.ID).state
                        }));
                    }
                    else if (key == 6)//新注册
                    {
                        try
                        {
                            List<PointsEntity> Points = HotelAdapter.GetPointListByTypeIDAndUserID(12, dicBizID[key]); //12表示注册得到的积分
                            IEnumerable<long> userIds = Points.Select(_ => _.UserID).Distinct();
                            List<User_Info> userInfos = AccountAdapter.GetUserBasicInfo(userIds).ToList();
                          //  Log.WriteLog(string.Format("commentsCount:{0}.userIds:{1}.UserInfosCount:{2},UserInfosCount:{3}", Points.Count(), userIds.Count(), userInfos.Count, dicMsg[key].First().businessID));
                            msgList.AddRange(Points.Select(_ => new SysMsgItem()
                            {
                                msgId = dicMsg[key].First(j => j.businessID == _.UserID).id,
                                senderNickName = userInfos.First(j => j.UserId == _.UserID).NickName,
                                senderUserID = userInfos.First(j => j.UserId == _.UserID).UserId,
                                avatarUrl = string.IsNullOrWhiteSpace(userInfos.First(j => j.UserId == _.UserID).AvatarUrl) ? DescriptionHelper.defaultAvatar : PhotoAdapter.GenHotelPicUrl(userInfos.First(j => j.UserId == _.UserID).AvatarUrl, Enums.AppPhotoSize.jupiter),
                                briefType = briefType,
                                briefContent = "恭喜注册成功，我们已将10奖励积分存放在您的钱包中。",
                                timeDesc = CommentAdapter.genReviewTimeSpan(_.CreateTime),//dicMsg[key].First(j => j.businessID == _.UserID).CreateTime
                                sendDate = _.CreateTime,// dicMsg[key].First(j => j.businessID == _.UserID).CreateTime
                                jumpUrl = "",
                                state = dicMsg[key].First(j => j.businessID == _.UserID).state
                            }));
                        }
                        catch (Exception e)
                        {
                            Log.WriteLog("Exception:新注册" + e);
                        }
                    }
                    else if (key == 7)//点评得到积分
                    {
                        List<PointsEntity> Points = HotelAdapter.GetPointListByTypeIDAndBusinessID(1, dicBizID[key]); //1点评得到的积分
                        IEnumerable<long> userIds = Points.Select(_ => _.UserID).Distinct();
                        List<User_Info> userInfos = AccountAdapter.GetUserBasicInfo(userIds).ToList();
                        msgList.AddRange(Points.Select(_ => new SysMsgItem()
                        {
                            msgId = dicMsg[key].First(j => j.businessID == _.BusinessID).id,
                            senderNickName = userInfos.First(j => j.UserId == _.UserID).NickName,
                            senderUserID = userInfos.First(j => j.UserId == _.UserID).UserId,
                            avatarUrl = string.IsNullOrWhiteSpace(userInfos.First(j => j.UserId == _.UserID).AvatarUrl) ? DescriptionHelper.defaultAvatar : PhotoAdapter.GenHotelPicUrl(userInfos.First(j => j.UserId == _.UserID).AvatarUrl, Enums.AppPhotoSize.jupiter),
                            briefType = briefType,
                            briefContent = string.Format("您的点评已发表。恭喜获得{0}积分。",_.LeavePoint),
                            timeDesc = CommentAdapter.genReviewTimeSpan(_.CreateTime),//dicMsg[key].First(j => j.businessID == _.BusinessID).CreateTime
                            sendDate = _.CreateTime,//dicMsg[key].First(j => j.businessID == _.BusinessID).CreateTime,
                            jumpUrl = "",
                            state = dicMsg[key].First(j => j.businessID == _.BusinessID).state
                        }));
                    }
                    else if (key == 8)//上传头像
                    {
                        List<PointsEntity> Points = HotelAdapter.GetPointListByTypeIDAndUserID(10, dicBizID[key]); //10上传头像得到的积分
                        IEnumerable<long> userIds = Points.Select(_ => _.UserID).Distinct();
                        List<User_Info> userInfos = AccountAdapter.GetUserBasicInfo(userIds).ToList();
                        msgList.AddRange(Points.Select(_ => new SysMsgItem()
                        {
                            msgId = dicMsg[key].First(j => j.businessID == _.UserID).id,
                            senderNickName = userInfos.First(j => j.UserId == _.UserID).NickName,
                            senderUserID = userInfos.First(j => j.UserId == _.UserID).UserId,
                            avatarUrl = string.IsNullOrWhiteSpace(userInfos.First(j => j.UserId == _.UserID).AvatarUrl) ? DescriptionHelper.defaultAvatar : PhotoAdapter.GenHotelPicUrl(userInfos.First(j => j.UserId == _.UserID).AvatarUrl, Enums.AppPhotoSize.jupiter),
                            briefType = briefType,
                            briefContent = "换个头像换个心情，5积分送给美好的你。",
                            timeDesc = CommentAdapter.genReviewTimeSpan(_.CreateTime),//dicMsg[key].First(j => j.businessID == _.UserID).CreateTime
                            sendDate =_.CreateTime,// dicMsg[key].First(j => j.businessID == _.UserID).CreateTime,
                            jumpUrl = "",
                            state = dicMsg[key].First(j => j.businessID == _.UserID).state
                        }));
                    }
                    else if (key == 9)//消费积分
                    {
                        List<PointsEntity> Points = HotelAdapter.GetPointListByTypeIDAndBusinessID(13, dicBizID[key]); //13消费得到的积分
                        IEnumerable<long> userIds = Points.Select(_ => _.UserID).Distinct();
                        List<User_Info> userInfos = AccountAdapter.GetUserBasicInfo(userIds).ToList();
                        msgList.AddRange(Points.Select(_ => new SysMsgItem()
                        {
                            msgId = dicMsg[key].First(j => j.businessID == _.BusinessID).id,
                            senderNickName = userInfos.First(j => j.UserId == _.UserID).NickName,
                            senderUserID = userInfos.First(j => j.UserId == _.UserID).UserId,
                            avatarUrl = string.IsNullOrWhiteSpace(userInfos.First(j => j.UserId == _.UserID).AvatarUrl) ? DescriptionHelper.defaultAvatar : PhotoAdapter.GenHotelPicUrl(userInfos.First(j => j.UserId == _.UserID).AvatarUrl, Enums.AppPhotoSize.jupiter),
                            briefType = briefType,
                            briefContent = string.Format("您的订单已完成。恭喜获得{0}积分。", _.LeavePoint),
                            timeDesc = CommentAdapter.genReviewTimeSpan(_.CreateTime),//dicMsg[key].First(j => j.businessID == _.BusinessID).CreateTime
                            sendDate = _.CreateTime,//dicMsg[key].First(j => j.businessID == _.BusinessID).CreateTime,
                            jumpUrl = "",
                            state = dicMsg[key].First(j => j.businessID == _.BusinessID).state
                        }));
                    }
                    else if (key == 19)//新注册
                    {
                        try
                        {
                            List<PointsEntity> Points = HotelAdapter.GetPointListByTypeIDAndUserID(19, dicBizID[key]); //12表示注册得到的积分
                            IEnumerable<long> userIds = Points.Select(_ => _.UserID).Distinct();
                            List<User_Info> userInfos = AccountAdapter.GetUserBasicInfo(userIds).ToList();
                            //  Log.WriteLog(string.Format("commentsCount:{0}.userIds:{1}.UserInfosCount:{2},UserInfosCount:{3}", Points.Count(), userIds.Count(), userInfos.Count, dicMsg[key].First().businessID));
                            msgList.AddRange(Points.Select(_ => new SysMsgItem()
                            {
                                msgId = dicMsg[key].First(j => j.businessID == _.UserID).id,
                                senderNickName = userInfos.First(j => j.UserId == _.UserID).NickName,
                                senderUserID = userInfos.First(j => j.UserId == _.UserID).UserId,
                                avatarUrl = string.IsNullOrWhiteSpace(userInfos.First(j => j.UserId == _.UserID).AvatarUrl) ? DescriptionHelper.defaultAvatar : PhotoAdapter.GenHotelPicUrl(userInfos.First(j => j.UserId == _.UserID).AvatarUrl, Enums.AppPhotoSize.jupiter),
                                briefType = briefType,
                                briefContent = "恭喜邀请注册成功，我们已将50奖励积分存放在您的钱包中。",
                                timeDesc = CommentAdapter.genReviewTimeSpan(_.CreateTime),//dicMsg[key].First(j => j.businessID == _.UserID).CreateTime
                                sendDate = _.CreateTime,// dicMsg[key].First(j => j.businessID == _.UserID).CreateTime
                                jumpUrl = "",
                                state = dicMsg[key].First(j => j.businessID == _.UserID).state
                            }));
                        }
                        catch (Exception e)
                        {
                            Log.WriteLog("Exception:新注册" + e);
                        }
                    }
                    else if (key == 20)//获邀注册积分
                    {
                        try
                        {
                            List<PointsEntity> Points = HotelAdapter.GetPointListByTypeIDAndUserID(20, dicBizID[key]); //12表示注册得到的积分
                            IEnumerable<long> userIds = Points.Select(_ => _.UserID).Distinct();
                            List<User_Info> userInfos = AccountAdapter.GetUserBasicInfo(userIds).ToList();
                            //  Log.WriteLog(string.Format("commentsCount:{0}.userIds:{1}.UserInfosCount:{2},UserInfosCount:{3}", Points.Count(), userIds.Count(), userInfos.Count, dicMsg[key].First().businessID));
                            msgList.AddRange(Points.Select(_ => new SysMsgItem()
                            {
                                msgId = dicMsg[key].First(j => j.businessID == _.UserID).id,
                                senderNickName = userInfos.First(j => j.UserId == _.UserID).NickName,
                                senderUserID = userInfos.First(j => j.UserId == _.UserID).UserId,
                                avatarUrl = string.IsNullOrWhiteSpace(userInfos.First(j => j.UserId == _.UserID).AvatarUrl) ? DescriptionHelper.defaultAvatar : PhotoAdapter.GenHotelPicUrl(userInfos.First(j => j.UserId == _.UserID).AvatarUrl, Enums.AppPhotoSize.jupiter),
                                briefType = briefType,
                                briefContent = "恭喜注册成功，我们已将50奖励积分存放在您的钱包中。",
                                timeDesc = CommentAdapter.genReviewTimeSpan(_.CreateTime),//dicMsg[key].First(j => j.businessID == _.UserID).CreateTime
                                sendDate = _.CreateTime,// dicMsg[key].First(j => j.businessID == _.UserID).CreateTime
                                jumpUrl = "",
                                state = dicMsg[key].First(j => j.businessID == _.UserID).state
                            }));
                        }
                        catch (Exception e)
                        {
                            Log.WriteLog("Exception:新注册" + e);
                        }
                    }
                    else if (key == 21)//积分过期提醒
                    {
                        try
                        {
                            List<PointsEntity> Points = AccountAdapter.GetExpirePointsEntity(string.Join(",", dicBizID[key]));
                            IEnumerable<long> userIds = Points.Select(_ => _.UserID).Distinct();
                            List<User_Info> userInfos = AccountAdapter.GetUserBasicInfo(userIds).ToList();
                            msgList.AddRange(Points.Select(_ => new SysMsgItem()
                            {
                                msgId = dicMsg[key].First(j => j.businessID == _.UserID).id,
                                senderNickName = userInfos.First(j => j.UserId == _.UserID).NickName,
                                senderUserID = userInfos.First(j => j.UserId == _.UserID).UserId,
                                avatarUrl = string.IsNullOrWhiteSpace(userInfos.First(j => j.UserId == _.UserID).AvatarUrl) ? DescriptionHelper.defaultAvatar : PhotoAdapter.GenHotelPicUrl(userInfos.First(j => j.UserId == _.UserID).AvatarUrl, Enums.AppPhotoSize.jupiter),
                                briefType = briefType,
                                briefContent = "您有" + _.LeavePoint + "积分三个月内到期，请尽快兑换好礼！",
                                timeDesc = CommentAdapter.genReviewTimeSpan(Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-01 10:00:00"))),
                                sendDate = _.CreateTime,
                                jumpUrl = "http://www.zmjiudian.com/Coupon/MoreList/1/0/0?albumId=22",
                                state = dicMsg[key].First(j => j.businessID == _.UserID).state
                            }));
                        }
                        catch (Exception e)
                        {
                            Log.WriteLog("Exception:新注册" + e);
                        }
                    }

                    else if (key == 100)
                    {
                        var noticeIds = dicBizID[key].Select(_ => (int)_);
                        //默认系统通知 系统通知 后台设置->系统通知
                        var sysNotices = MsgService.GetManySysNotice(noticeIds, dicMsg[key].First().receiver);
                        sysNotices = sysNotices.Where(_ => _.Receiver == dicMsg[key].First().receiver).ToList();
                        //IEnumerable<long> userIds = sysNotices.Select(_ => _.Receiver).Distinct();
                        //List<User_Info> userInfos = AccountAdapter.GetUserBasicInfo(userIds).ToList();
                        msgList.AddRange(sysNotices.Select(_ => new SysMsgItem()
                        {
                            msgId = dicMsg[key].First(j => j.businessID == _.ID).id,
                            senderNickName = _.NotifierNickName,
                            senderUserID = _.Notifier,
                            avatarUrl = string.IsNullOrWhiteSpace(_.NotifierAvatarSUrl) ? PhotoAdapter.GenHotelPicUrl(DescriptionHelper.defaultZMJDLogo, HJDAPI.Common.Helpers.Enums.AppPhotoSize.jupiter) : PhotoAdapter.GenHotelPicUrl(_.NotifierAvatarSUrl, Enums.AppPhotoSize.jupiter),
                            briefType = briefType,
                            briefContent = _.NoticeContent,
                            timeDesc = CommentAdapter.genReviewTimeSpan(_.CreateTime),//dicMsg[key].First(j => j.businessID == _.ID).CreateTime
                            sendDate = _.CreateTime,//dicMsg[key].First(j => j.businessID == _.ID).CreateTime,
                            jumpUrl = _.JumpAppUrl,
                            state = dicMsg[key].First(j => j.businessID == _.ID).state
                        }));
                    }
                }
                return msgList.OrderBy(_ => _.state).ThenByDescending(_ => _.sendDate).ToList();
            }
            else
            {
                return msgList;
            }
        }

        public static UmengMessageResult SendMsgByUmeng(string title, string content, long userId, UmengMessageExtra extra, bool? is4Android, Enums.UmengMessagePushType PushType = Enums.UmengMessagePushType.customizedcast)
        {
            var unReadMsgCount = 0;
            try
            {
                unReadMsgCount = GetSysMessageEntityCount(userId, 0);
            }
            catch (Exception ex) { }

            int totalCount = userId == 0 ? 1 : unReadMsgCount == 0 ? 1 : unReadMsgCount;//默认1 未读消息数目
            
            var filter = new UmengMessageFilter();

            //处理推送消息  调用第三方接口 业务需要分离 iOS和Android都发送一次
            UmengAndroidPostParam umengParam = new UmengAndroidPostParam()
            {
                //type = HJDAPI.Common.Helpers.Enums.UmengMessagePushType.customizedcast.ToString(),//HJDAPI.Common.Helpers.Enums.UmengMessagePushType.groupcast.ToString(),
                type = PushType.ToString(),
                payload = new UmengMessageAndroidPayLoad()
                {
                    display_type = HJDAPI.Common.Helpers.Enums.UmengMessageDisplayType.notification.ToString(),//notification-通知，message-消息
                    body = new UmengMessageAndroidBody()
                    {
                        ticker = "您有一条来自周末酒店的新消息",
                        title = title,
                        text = content,
                        icon = "",
                        largeIcon = "",
                        img = "",
                        sound = "",
                        builder_id = "0",
                        play_vibrate = "true",
                        play_lights = "true",
                        play_sound = "true",
                        after_open = HJDAPI.Common.Helpers.Enums.UmengMessageAfterOpen.go_custom.ToString(),
                        url = "",
                        activity = "",
                        custom = extra
                    },
                    extra = extra
                },
                alias_type = "userid",//userid
                alias = userId.ToString(),//4562166 4513463
                policy = new UmengMessagePolicy()
                {
                    start_time = "",//DateTime.Now.AddMinutes(20).ToString("yyyy-MM-dd HH:mm:ss")
                    expire_time = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm:ss"),
                    max_send_num = 1000,
                    out_biz_no = ""
                },
                production_mode = Configs.SendNotice_Production_Mode,
                description = "",
                thirdparty_id = "",
                filter = filter
            };

            //bool isAndroid = true;
            var result = new UmengMessageResult();
            if (is4Android != null && is4Android.HasValue)
            {
                var jsonStr = GenUmengSendMessagePostJsonStr(umengParam, (bool)is4Android, totalCount);
                result = SendUmengMsg(jsonStr, (bool)is4Android);
            }
            else
            {
                var jsonStr = GenUmengSendMessagePostJsonStr(umengParam, true, totalCount);
                result = SendUmengMsg(jsonStr, true);
                jsonStr = GenUmengSendMessagePostJsonStr(umengParam, false, totalCount);
                result = SendUmengMsg(jsonStr, false);
            }
            return result;
        }

        public static string GenUmengSendMessagePostJsonStr(UmengAndroidPostParam param, bool is4Android, int unReadCount = 1)
        {
            string postJsonStr = "";
            string timeStamp = Signature.GenSince1970_01_01_00_00_00Seconds().ToString();

            if(is4Android){
                param.appkey = android_SendMessage_appkey;
                param.timestamp = timeStamp;
                postJsonStr = JsonConvert.SerializeObject(param);
            }
            else
            {
                var iOSMessage = new UmengiOSPostParam()
                {
                    appkey = iOS_SendMessage_appkey,
                    timestamp = timeStamp,
                    type = param.type,
                    device_tokens = param.device_tokens,
                    alias_type = param.alias_type,
                    alias = param.alias,
                    file_id = param.file_id,
                    payload = new UmengMessageiOSPayLoad()
                    {
                        aps = new UmengMessageiOSBody()
                        {
                            alert = param.payload.body.text,
                            badge = unReadCount
                        },
                        type = param.payload.extra.type,
                        action = param.payload.extra.action,
                        bgColor = param.payload.extra.bgColor,
                        btnName = param.payload.extra.btnName,
                        isShowInApp = param.payload.extra.isShowInApp,
                        timeElapse = param.payload.extra.timeElapse,
                        msgType = param.payload.extra.msgType,
                        receiverId = param.payload.extra.receiverId
                    },
                    policy = param.policy,
                    production_mode = param.production_mode,
                    description = param.description,
                    thirdparty_id = param.thirdparty_id,
                    filter = param.filter
                };
                postJsonStr = JsonConvert.SerializeObject(iOSMessage);
            }
            return postJsonStr;
        }

        /// <summary>
        /// 通过友盟给iOS推送信息
        /// </summary>
        /// <returns></returns>
        public static UmengMessageResult SendUmengMsg(string postJsonStr, bool is4Android)
        {
            string apiUrl = umengSendUrl;
            string sign = Signature.GenUmengSendMessageSign(umengSendHttpMethod, apiUrl, postJsonStr, is4Android ? android_SendMessage_appmastersecret : iOS_SendMessage_appmastersecret);
            apiUrl += "?sign=" + sign;

            CookieContainer cc = new CookieContainer();
            string json = "";
            try
            {
                json = HttpRequestHelper.PostJson(apiUrl, postJsonStr, ref cc);//发送消息
            }
            catch (Exception ex)
            {
              //  Log.WriteLog(string.Format("SendUmengMsg->客户端:{0};消息内容:{1};异常信息:{2};异常堆栈:{3}", is4Android ? "android" : "iOS", postJsonStr, ex.Message, ex.StackTrace));
            }

            return string.IsNullOrWhiteSpace(json) ? new UmengMessageResult()
            {
                data = new UmengMessageResultData()
                {
                    error_code = "0000",
                    msg_id = "",
                    task_id = "",
                    thirdparty_id = ""
                },
                ret = string.Format("umeng向{0}发送消息API错误", is4Android ? "android" : "iOS")
            } : JsonConvert.DeserializeObject<UmengMessageResult>(json);//发送成功或失败
        }

        /// <summary>
        /// 获取单条消息
        /// </summary>
        /// <param name="sme">按id 或者 businessType+businessID获取单条消息</param>
        /// <returns></returns>
        internal static SysMessageEnitity GetOneMessage(SysMessageEnitity sme)
        {
            return MsgService.GetOneMessage(sme);
        }

        internal static List<SysMessageEnitity> GetUnReadMessageList(long receiver, int businessType)
        {
            return MsgService.GetUnReadMessageList(receiver, businessType);
        }
        
        internal static int SendAppNotice(SendNoticeEntity txtEntity)
        {
            var userId = txtEntity.userID;
            //var isTest = AccountAdapter.HasUserPriviledge(userId, AccountAdapter.UserPriviledge.InnerTest);//2016_05_31 放开测试人员限制 所有的都可收到
            if (false)
            {
                return -1;
            }
            else
            {
                var title = string.IsNullOrWhiteSpace(txtEntity.title) ? "周末酒店" : txtEntity.title;
                var msg = string.IsNullOrWhiteSpace(txtEntity.msg) ? "周末酒店" : txtEntity.msg;
                var actionUrl = txtEntity.actionUrl;
                var defaultActionUrl = "whotelapp://www.zmjiudian.com/msglist?userid={userid}&realuserid=1";

                if(string.IsNullOrWhiteSpace(actionUrl)){
                    switch((int)txtEntity.noticeType){
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                            actionUrl = defaultActionUrl;
                            break;
                        case 6:
                            var magicallActionUrl = "http://www.zmjiudian.com/MagiCall/MagiCallClient";
                            actionUrl = ADAdapter.FormatActionURL(magicallActionUrl);
                            break;
                        case 7:
                        case 8:
                            var sourceId = 100;
                            var timeStamp = HJDAPI.Common.Security.Signature.GenSince1970_01_01_00_00_00Seconds();
                            var urlWithoutSign = string.Format("http://www.zmjiudian.com/personal/wallet/{0}/fund?TimeStamp={1}&SourceID={2}",txtEntity.userID, timeStamp, sourceId);            
                            var sign = HJDAPI.Common.Security.Signature.GenSignature(timeStamp, sourceId, Configs.MD5Key, urlWithoutSign);
                            
                            var completeUrl = urlWithoutSign + "&sign=" + sign;

                            var encodeCompleteUrl = System.Web.HttpUtility.UrlEncode(completeUrl);
                            actionUrl = "whotelapp://www.zmjiudian.com/gotopage?url=" + encodeCompleteUrl;
                            break;
                        default:
                            actionUrl = defaultActionUrl;
                            break;
                    }
                }

                var umengExtra = new UmengMessageExtra()
                {
                    action = actionUrl,
                    bgColor = "red",
                    btnName = "去看看",
                    isShowInApp = true,
                    timeElapse = 15f,
                    type = "comment",
                    msgType = txtEntity.noticeType.ToString(),
                    receiverId = userId
                };

                switch (txtEntity.appType)
                {
                    case 1:
                        MessageAdapter.SendMsgByUmeng(title, msg, userId, umengExtra, false);
                        break;
                    case 2:
                        MessageAdapter.SendMsgByUmeng(title, msg, userId, umengExtra, true);
                        break;
                    default:
                        MessageAdapter.SendMsgByUmeng(title, msg, userId, umengExtra, false);
                        MessageAdapter.SendMsgByUmeng(title, msg, userId, umengExtra, true);
                        break;
                }

                return 0;
            }
        }


        internal static int SendAllAppNotice(SendNoticeEntity txtEntity)
        {
            var userId = txtEntity.userID;
            //var isTest = AccountAdapter.HasUserPriviledge(userId, AccountAdapter.UserPriviledge.InnerTest);//2016_05_31 放开测试人员限制 所有的都可收到
            if (false)
            {
                return -1;
            }
            else
            {
                var title = string.IsNullOrWhiteSpace(txtEntity.title) ? "周末酒店" : txtEntity.title;
                var msg = string.IsNullOrWhiteSpace(txtEntity.msg) ? "周末酒店" : txtEntity.msg;
                var actionUrl = txtEntity.actionUrl;
                var defaultActionUrl = "whotelapp://www.zmjiudian.com/msglist?userid={userid}&realuserid=1";

                if (string.IsNullOrWhiteSpace(actionUrl))
                {
                    switch ((int)txtEntity.noticeType)
                    {
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                            actionUrl = defaultActionUrl;
                            break;
                        case 6:
                            var magicallActionUrl = "http://www.zmjiudian.com/MagiCall/MagiCallClient";
                            actionUrl = ADAdapter.FormatActionURL(magicallActionUrl);
                            break;
                        case 7:
                        case 8:
                            var sourceId = 100;
                            var timeStamp = HJDAPI.Common.Security.Signature.GenSince1970_01_01_00_00_00Seconds();
                            var urlWithoutSign = string.Format("http://www.zmjiudian.com/personal/wallet/{0}/fund?TimeStamp={1}&SourceID={2}", txtEntity.userID, timeStamp, sourceId);
                            var sign = HJDAPI.Common.Security.Signature.GenSignature(timeStamp, sourceId, Configs.MD5Key, urlWithoutSign);

                            var completeUrl = urlWithoutSign + "&sign=" + sign;

                            var encodeCompleteUrl = System.Web.HttpUtility.UrlEncode(completeUrl);
                            actionUrl = "whotelapp://www.zmjiudian.com/gotopage?url=" + encodeCompleteUrl;
                            break;
                        default:
                            actionUrl = defaultActionUrl;
                            break;
                    }
                }

                var umengExtra = new UmengMessageExtra()
                {
                    action = actionUrl,
                    bgColor = "red",
                    btnName = "去看看",
                    isShowInApp = true,
                    timeElapse = 15f,
                    type = "allUser",
                    msgType = txtEntity.noticeType.ToString(),
                    receiverId = userId
                };

                switch (txtEntity.appType)
                {
                    case 1:
                        MessageAdapter.SendMsgByUmeng(title, msg, userId, umengExtra, false, Enums.UmengMessagePushType.broadcast);
                        break;
                    case 2:
                        MessageAdapter.SendMsgByUmeng(title, msg, userId, umengExtra, true, Enums.UmengMessagePushType.broadcast);
                        break;
                    default:
                        MessageAdapter.SendMsgByUmeng(title, msg, userId, umengExtra, false, Enums.UmengMessagePushType.broadcast);
                        MessageAdapter.SendMsgByUmeng(title, msg, userId, umengExtra, true, Enums.UmengMessagePushType.broadcast);
                        break;
                }

                return 0;
            }
        }
    }
}