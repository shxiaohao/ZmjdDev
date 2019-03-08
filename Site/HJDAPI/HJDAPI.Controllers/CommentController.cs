using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using HJD.DestServices.Contract;
using HJD.HotelServices.Contracts;
using HJDAPI.Models;
using System.Runtime.Serialization;
using HJD.CommentService.Contract;
using System.Net.Http;
using Newtonsoft.Json;
using HJDAPI.Controllers.Common;
using HJDAPI.Common.Security;
using HJD.PhotoServices.Entity;
using HJDAPI.Controllers.Adapter;
using MessageService.Contract;
using HJDAPI.Common.LogAttribute;
using HJDAPI.Common.Helpers;

namespace HJDAPI.Controllers
{
    public class CommentController : BaseApiController
    {
        [HttpGet]
        public CommentShareModel GetCommentShareInfo(int CommentID)
        {
            CommentInfoModel comment = CommentAdapter.GetOneComment(CommentID, AppUserID);
            return comment.shareModel;//DescriptionHelper.GetShareModel(comment, appUserID);
        }

        [HttpGet]
        public CommentInfoModel GetOneComment(int CommentID)
        {
            CommentInfoModel model = CommentAdapter.GetOneComment(CommentID);
            return model;
        }

        [HttpGet]
        public CommentInfoModel GetOneComment40(int CommentID, Int64 TimeStamp, int SourceID, string RequestType, string sign)
        {
            if (Signature.IsRightSignature(TimeStamp, SourceID, RequestType, sign))
            {
                return CommentAdapter.GetOneComment(CommentID);//改成调controller
            }
            else
            {
                return new CommentInfoModel();
            }
        }

        [HttpGet]
        public UserCanWriteCommentResult GetUserCanWriteCommentOrderID(int hotelID, long userID)
        {
            return CommentAdapter.GetUserCanWriteCommentOrderID(hotelID, userID);
        }

        [HttpGet]
        public UserCanWriteCommentResult GetUserCanWriteCommentOrderID40(int hotelID, long userID, Int64 TimeStamp, int SourceID, string RequestType, string sign)
        {
            if (Signature.IsRightSignature(TimeStamp, SourceID, RequestType, sign))
            {
                return CommentAdapter.GetUserCanWriteCommentOrderID(hotelID, userID);
            }
            else
            {
                return new UserCanWriteCommentResult { msg = "签名错误！" };
            }
        }

        [HttpGet]
        public CanWriteCommentResult GetUserCanWriteComment(int hotelID, long userID)
        {
            return CommentAdapter.GetUserCanWriteComment(hotelID, userID);
        }

        [HttpGet]
        public CanWriteCommentResult GetUserCanWriteComment40(int hotelID, long userID, Int64 TimeStamp, int SourceID, string RequestType, string sign)
        {
            if (Signature.IsRightSignature(TimeStamp, SourceID, RequestType, sign))
            {
                return CommentAdapter.GetUserCanWriteComment(hotelID, userID);
            }
            else
            {
                return new CanWriteCommentResult { Message = "签名错误！" };
            }
        }

        [HttpGet]
        public CommentDefaultInfoModel GetCommentDefaultInfo(int hotelid)
        {
            return CommentAdapter.GetCommentDefaultInfo(hotelid);
        }

        [HttpGet]
        public CommentDefaultInfoModel GetCommentDefaultInfoEx(int hotelid)
        {
            return CommentAdapter.GetCommentDefaultInfoEx(hotelid);
        }

        [HttpGet]
        public CommentDefaultInfoModel GetCommentDefaultInfoEx20(long userid, int hotelid)
        {
            CommentDefaultInfoModel cdim = CommentAdapter.GetCommentDefaultInfoEx(hotelid);
            cdim.AlertResult = CommentAdapter.getAlertPointsRuleResult(userid, hotelid);
            return cdim;
        }

        [HttpGet]
        public CommentDefaultInfoModel GetCommentDefaultInfo30(long userid, int hotelid)
        {
            CommentDefaultInfoModel cdim = CommentAdapter.GetCommentDefaultInfo30(hotelid);
            cdim.AlertResult = CommentAdapter.getAlertPointsRuleResult(userid, hotelid);
            return cdim;
        }

        [HttpGet]
        public UserCommentListModel GetUserCommentList(long userID, int start, int count)
        {
            return CommentAdapter.GetUserCommentList(userID, start, count);
        }

        [HttpGet]
        public CommentListModel GetUserCommentList20(long userID, int start, int count)
        {
            return CommentAdapter.GetUserCommentList20(userID, start, count, AppVer);
        }

        /// <summary>
        /// 网站 我的点评列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public CommentListModel20 GetUserCommentList30(long userID, int start, int count)
        {
            return CommentAdapter.GetUserCommentList30(userID, start, count, AppVer);
        }

        /// <summary>
        /// ToDo 查询是否对该名称的酒店写过点评 如果有则提醒已经写过 在添加酒店页面 如果酒店名称已存在且该用户则不让写了
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public AddHotelVerifyResult CanWriteAddHotelComment(string hotelName, long userID)
        {
            //ToDo 查询自己添加酒店的点评记录
            int flag = HotelAdapter.HotelService.GetUserAddHotelForComment(hotelName, userID);
            return flag == 0 ? new AddHotelVerifyResult() { CanAdd = true, Message = "" } : new AddHotelVerifyResult() { CanAdd = false, Message = "您已添加过该酒店" };
        }

        [HttpGet]
        public UserCommentListModel GetUserCommentList40(long userID, Int64 TimeStamp, int SourceID, string RequestType, string sign)
        {
            if (Signature.IsRightSignature(TimeStamp, SourceID, RequestType, sign))
            {
                return CommentAdapter.GetUserCommentList(userID, 0, 0);
            }
            else
            {
                return new UserCommentListModel();
            }
        }

        /// <summary>
        /// 用户点评列表 4.0版本开始使用
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="TimeStamp"></param>
        /// <param name="SourceID"></param>
        /// <param name="RequestType"></param>
        /// <param name="sign"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [HttpGet]
        public CommentListModel GetUserCommentList50(long userID, Int64 TimeStamp, int SourceID, string RequestType, string sign, int start = 0, int count = 1000)
        {
            if (Signature.IsRightSignature(TimeStamp, SourceID, RequestType, sign))
            {
                return CommentAdapter.GetUserCommentList20(userID, start, count, AppVer);
            }
            else
            {
                return new CommentListModel() { CommentList = new List<UserCommentListItemEntity>() };
            }
        }

        [HttpPost]
        public SubmitCommentResultEntity SubmitComment(SubmitCommentEntity c)
        {
            c.appVer = AppVer;
            if (c.UserID != 0 && (c.HotelID > 0 || !string.IsNullOrWhiteSpace(c.HotelName)))
            {
                if (c.HotelID > 0)
                {
                    return CommentAdapter.SubmitComment(c);
                }
                else if (c.HotelID == 0 && !string.IsNullOrWhiteSpace(c.HotelName))
                {
                    return CommentAdapter.SubmitComment4NewHotel(c);
                }
                else
                {
                    HJD.AccountServices.Entity.MemberProfileInfo info = AccountAdapter.GetCurrentUserInfo(c.UserID);
                    if (info != null && !string.IsNullOrWhiteSpace(info.MobileAccount) && DateTime.Now.Hour <= 22)
                    {
                        SMServiceController.SendSMS(info.MobileAccount.Trim(), "尊敬的周末酒店App用户，如果您在写点评过程中遇到任何问题，欢迎拨打我们的服务热线4000-021-701进行咨询。");
                    }
                    SMServiceController.SendSMS("18021036971", string.Format("SubmitComment点评提交参数,来源{0},用户ID{1},酒店ID{2},酒店名称{3},订单ID{4},详细参数:{5}", AppType, c.UserID, c.HotelID, c.HotelName, c.OrderID, JsonConvert.SerializeObject(c)));//遇到这种情况 发送报警短信
                    return new SubmitCommentResultEntity { Success = 200, Message = "缺少酒店信息无法提交点评" };
                }
            }
            else
            {
                return new SubmitCommentResultEntity() { CommentID = 0, Success = 1, Message = "参数不完整" };
            }
        }

        [HttpPost]
        public SubmitCommentResultEntity SubmitComment40(SubmitCommentEntity c)
        {
            c.appVer = AppVer;
            if (Signature.IsRightSignature(c.TimeStamp, c.SourceID, c.RequestType, c.Sign))
            {
                c.appVer = AppVer;

                if (c.HotelID == 0 && !string.IsNullOrWhiteSpace(c.HotelName))
                {
                    c.HotelID = HotelAdapter.GetHotelIdByName(c.HotelName);
                }

                if (c.HotelID > 0)
                {
                    return CommentAdapter.SubmitComment(c);
                }
                else if (c.HotelID == 0 && !string.IsNullOrWhiteSpace(c.HotelName))
                {
                    return CommentAdapter.SubmitComment4NewHotel(c);
                }
                else
                {
                    HJD.AccountServices.Entity.MemberProfileInfo info = AccountAdapter.GetCurrentUserInfo(c.UserID);
                    if (info != null && !string.IsNullOrWhiteSpace(info.MobileAccount) && DateTime.Now.Hour <= 22)
                    {
                        SMServiceController.SendSMS(info.MobileAccount.Trim(), "尊敬的周末酒店App用户，如果您在写点评过程中遇到任何问题，欢迎拨打我们的服务热线4000-021-701进行咨询。");
                    }
                    SMServiceController.SendSMS("18021036971", string.Format("SubmitComment40点评提交参数,来源{0},AppVer{6},用户ID{1},酒店ID{2},酒店名称{3},订单ID{4},详细参数:{5}", AppType, AppVer, c.UserID, c.HotelID, c.HotelName, c.OrderID, JsonConvert.SerializeObject(c)));
                    return new SubmitCommentResultEntity { Success = 200, Message = "缺少酒店信息无法提交点评" };
                }
            }
            else
            {
                return new SubmitCommentResultEntity { Success = 100, Message = "签名错误！" };
            }
        }

        /// <summary>
        /// 追加点评内容
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        [HttpPost]
        public SubmitCommentResultEntity AddCommentContent(SubmitCommentEntity c)
        {
            c.appVer = AppVer;
            if (Signature.IsRightSignature(c.TimeStamp, c.SourceID, c.RequestType, c.Sign))
            {
                if (c.CommentID > 0 && c.addInfos != null && c.addInfos.Count != 0)
                {
                    CommentAdapter.commentService.UpdateWHComments(c.CommentID, 0, c.UserID); //将点评状态改为待审核状态 
                    return CommentAdapter.AddCommentContent(c);
                }
                else
                {
                    return new SubmitCommentResultEntity { Success = 200, Message = c.CommentID == 0 ? "缺少点评ID无法提交" : "缺少补充点评信息无法提交" };
                }
            }
            else
            {
                return new SubmitCommentResultEntity { Success = 100, Message = "验证签名错误！" };
            }
        }

        [HttpGet]
        public CommentInfoEntity GenWHHotelcommentDescription(int ID)
        {
            return CommentAdapter.GenWHHotelcommentDescription(ID);
        }

        /// <summary>
        /// 上传照片数据
        /// </summary>
        /// <param name="photo">照片类  photo.AppID：接口定义为区分android\ios\web,  照片系统中APPID 为平台，与TypeID合用做照片尺寸的切割，目前照片系统APPID仅取值为1.  两者定义不同 </param>
        /// <returns></returns>
        [HttpPost]
        public CommentPhotoUploadResultEntity UploadCommnetPhoto(CommentPhotoUploadEntity photo)
        {
            CommentPhotoUploadResultEntity r = new CommentPhotoUploadResultEntity();
            r.Success = 0;
            r.Message = "上传成功！";
            try
            {
                byte[] picdata = Convert.FromBase64String(photo.PhotoData);

                CommentInfoEntity c = CommentAdapter.GetOneCommentNoPhoto(photo.CommentID);

                HotelItem h = HotelAdapter.GetOneHotelInfo(c.Comment.HotelID);

                int phsid = PhotoAdapter.PhotoUpload(picdata, 1, 1, true);
                int type = 100; //酒店点评ID

                PhotoAdapter.InsertPhotoInfo(phsid, type, photo.TagBlockCategory, c.Comment.UserID, h.DistrictId, h.Id, photo.CommentID, "");

                r.phsid = phsid;
            }
            catch (Exception e)
            {
                r.Success = 1;
                r.Message = e.Message + e.StackTrace;
                r.phsid = 0;
            }
            return r;
        }

        /// <summary>
        /// 插入点评照片数据
        /// </summary>
        /// <param name="photo">照片类  photo.AppID：接口定义为区分android\ios\web,  照片系统中APPID 为平台，与TypeID合用做照片尺寸的切割，目前照片系统APPID仅取值为1.  两者定义不同 </param>
        /// <returns></returns>
        [HttpPost]
        public CommentPhotoUploadResultEntity InsertCommnetPhoto40(CommentPhotoInsertEntity photo)
        {
            CommentPhotoUploadResultEntity r = new CommentPhotoUploadResultEntity();
            r.Success = 0;
            r.Message = "添加成功！";

            if (photo.CommentID == 0)
            {
                r.Success = 200;
                r.Message = "缺少点评信息";
            }
            else if (string.IsNullOrWhiteSpace(photo.PhotoSURL))
            {
                r.Success = 300;
                r.Message = "缺少点评照片信息";
            }
            else if (Signature.IsRightSignature(photo.TimeStamp, photo.SourceID, photo.RequestType, photo.Sign))
            {
                photo.AppVer = AppVer;
                r = CommentAdapter.InsertCommentPhotoUpload(photo);
            }
            else
            {
                r.Success = 100;
                r.Message = "签名错误！";
            }
            return r;
        }

        /// <summary>
        /// App4.2版本引入 点评照片上传新格式
        /// </summary>
        /// <param name="photo"></param>
        /// <returns></returns>
        [HttpPost]
        public CommentPhotoUploadResultEntity InsertCommnetPhoto50(CommentPhotoInsertEntity photo)
        {
            CommentPhotoUploadResultEntity r = new CommentPhotoUploadResultEntity();
            r.Success = 0;
            r.Message = "添加成功！";

            if (photo.CommentID == 0)
            {
                r.Success = 200;
                r.Message = "缺少点评信息";
            }
            else if (string.IsNullOrWhiteSpace(photo.PhotoSURL))
            {
                r.Success = 300;
                r.Message = "缺少点评照片信息";
            }
            else if (Signature.IsRightSignature(photo.TimeStamp, photo.SourceID, photo.RequestType, photo.Sign))
            {
                photo.AppVer = AppVer;
                r = CommentAdapter.InsertCommentPhotoUpload(photo);
            }
            else
            {
                r.Success = 100;
                r.Message = "签名错误！";
            }
            return r;
        }

        [HttpGet]
        public CommentInfoModel2 GetOneCommentEx(int CommentID)
        {
            CommentInfoModel2 model = CommentAdapter.GetOneCommentEx(CommentID);
            return model;
        }

        [HttpGet]
        public CommentInfoModel2 GetOneComment40Ex(int CommentID, Int64 TimeStamp, int SourceID, string RequestType, string sign)
        {
            if (Signature.IsRightSignature(TimeStamp, SourceID, RequestType, sign))
            {
                return CommentAdapter.GetOneCommentEx(CommentID);//改成调controller
            }
            else
            {
                return new CommentInfoModel2();
            }
        }

        [HttpGet]
        public CommentInfoModel3 GetOneComment500([FromUri]Comment500Param param)
        {
            try
            {
                if (param.userID == 0 && string.IsNullOrWhiteSpace(param.sessionID) && string.IsNullOrWhiteSpace(param.openID))
                {

                }
                else
                {
                    int terminalType = param.terminalType > 0 ? param.terminalType : CommMethods.GetTerminalId(AppType);
                    string clientIP = string.IsNullOrWhiteSpace(param.clientIP) ? HttpRequestHelper.GetClientIp(this.ControllerContext.Request) : param.clientIP;

                    int id = HotelAdapter.InsertBrowsingRecord(new BrowsingRecordEntity()
                    {
                        BusinessID = param.commentID,
                        BusinessType = 1,
                        Visitor = param.userID,
                        TerminalType = terminalType,
                        ClientIP = clientIP,
                        SessionID = param.sessionID,
                        OpenID = param.openID,
                        AppVer = param.appVersion
                    });
                }
            }
            catch (Exception ex)
            {

            }

            return CommentAdapter.GetOneComment50(param.commentID, param.userID);//改成调controller
        }

        [HttpGet]
        public CommentInfoModel3 GetOneComment50(int CommentID, Int64 TimeStamp, int SourceID, string RequestType, string sign, long userID = 0)
        {
            if (Signature.IsRightSignature(TimeStamp, SourceID, RequestType, sign))
            {
                long appUserID = userID == 0 ? AppUserID : userID;//从APIController获取AppUserID
                try
                {
                    if (CommentID == 0)
                    {

                    }
                    else
                    {
                        int terminalType = CommMethods.GetTerminalId(AppType);
                        string clientIP = HttpRequestHelper.GetClientIp(this.ControllerContext.Request);

                        int id = HotelAdapter.InsertBrowsingRecord(new BrowsingRecordEntity()
                        {
                            BusinessID = CommentID,
                            BusinessType = 1,
                            Visitor = appUserID,
                            TerminalType = terminalType,
                            ClientIP = clientIP,
                            SessionID = "",
                            OpenID = "",
                            AppVer = AppVer
                        });
                    }
                }
                catch (Exception ex)
                {

                }

                var result = CommentAdapter.GetOneComment50(CommentID, userID > 0 ? userID : appUserID);//改成调controller
                var boxDataList = ADAdapter.GetPopBoxConfigEntityList((int)HJDAPI.Common.Helpers.Enums.PopBoxTarget.commentdetail);
                var boxData = boxDataList.FirstOrDefault();
                result.boxData = new PopupBoxData()
                {
                    isShow = boxData != null ? boxData.isShow : false,
                    showUrl = boxData != null ? boxData.showUrl : "",
                    lazyLoadTime = boxData != null ? boxData.lazyLoadTime : 0.2f,
                    widthRatio = boxData != null ? boxData.widthRatio : 0.8f,
                    frequency = boxData != null ? boxData.frequency : 0,
                    boxId = boxData != null ? boxData.boxId : "",
                    widthHeightRatio = boxData != null ? boxData.widthHeightRatio : 0.6f
                };
                return result;
            }
            else
            {
                return new CommentInfoModel3();
            }
        }

        /// <summary>
        /// 需要用户登录 才可设置 类似点赞 点一下ok 再点一下取消
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultEntity MarkCommentUseful(CommentUsefulParam param)
        {
            if (param.CommentID == 0 || param.UserID == 0)
            {
                return new ResultEntity() { Success = 1, Message = "缺少参数 更新失败" };
            }

            int commentUsefulID = CommentAdapter.InsertOrUpdateCommentUseful(param.CommentID, param.UserID, param.ChannelID == 0 ? HJDAPI.Common.Helpers.StringHelper.TransAppTypeHeaderToAppType(AppType) : param.ChannelID);

            if (commentUsefulID > 0)//新增返回记录ID 更新返回0
            {
                //参照微信做法 同一个人对某条评论点赞的消息仅仅一次 取消赞不发消息
                SysMessageEnitity cue = MessageAdapter.GetOneMessage(new SysMessageEnitity()
                {
                    id = 0,
                    businessID = commentUsefulID,
                    businessType = 3
                });

                //如果没加入消息列 则插入一条
                if (cue == null || cue.id == 0)
                {
                    var commentInfoEntity = CommentAdapter.GetOneCommentInfoEntity(param.CommentID);
                    var commentUserID = commentInfoEntity.Comment.UserID;
                    var hotelName = commentInfoEntity.Comment.HotelName;
                    //点赞成功 插入之后 开始添加消息记录 ToDo把消息发送 挪到队列里处理
                    if (commentUserID != param.UserID)
                    {

                        MessageAdapter.InsertSysMessage(new SysMessageEnitity()
                        {
                            state = 0,
                            businessID = commentUsefulID,
                            businessType = 3,//1代表是评论 2代表是回复 3代表点赞(有帮助)
                            receiver = param.ReceiverUserID == 0 ? commentUserID : param.ReceiverUserID
                        });

                        var curUserInfo = AccountAdapter.GetCurrentUserInfo(param.UserID);
                        MessageAdapter.SendAppNotice(new SendNoticeEntity()
                        {
                            actionUrl = string.Format("whotelapp://www.zmjiudian.com/personal/comments/{0}", param.CommentID),
                            appType = 0,
                            from = curUserInfo.NickName,
                            msg = string.Format("{0}认为你对{1}酒店的点评有用", curUserInfo.NickName, hotelName),
                            noticeType = ZMJDNoticeType.useful,
                            title = "周末酒店",
                            userID = commentUserID
                        });
                    }
                }
            }
            return new ResultEntity() { Success = 0, Message = "成功" };
        }

        /// <summary>
        /// 获得个人主页数据
        /// </summary>
        /// <param name="homeUserID"></param>
        /// <param name="curUserID"></param>
        /// <returns></returns>
        [HttpGet]
        public PersonalHomePageModel GetPersonalPageModel(long homeUserID, long curUserID, int start = 0, int count = 1000)
        {
            /// android 4.2版本 查看其他人的个人主页 涉及小数的点评分数异常
            var result = CommentAdapter.GetPersonalPageModel(homeUserID, curUserID, start, count);

            bool isAndroid4dot2 = AppType.ToLower().Contains("android") && AppVer.StartsWith("4.2");
            if (!isAndroid4dot2 || homeUserID == curUserID) return result;

            foreach (var homePageComemntItem in result.commentData.commentList)
            {
                homePageComemntItem.commentScore = (float)Math.Ceiling(homePageComemntItem.commentScore);//正常的分数不会超过5分
            }
            return result;
        }

        /// <summary>
        /// 获取个人主页点评列表20版本
        /// App 4.2版本使用
        /// </summary>
        /// <param name="homeUserID"></param>
        /// <param name="curUserID"></param>
        /// <param name="isRecommend"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [HttpGet]
        public PersonalHomePageModel20 GetPersonalPageModel20(long homeUserID, long curUserID, int isRecommend = 1, int start = 0, int count = 1000)
        {
            return CommentAdapter.GetPersonalPageModel20(homeUserID, curUserID, isRecommend, start, count);
        }

        /// <summary>
        /// 添加评论回复 设计到消息通知记录
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        [HttpPost]
        public CommentAddReviewResult AddCommentReview(CommentAddReviewParam c)
        {
            if (Signature.IsRightSignature(c.TimeStamp, c.SourceID, c.RequestType, c.Sign))
            {
                if (string.IsNullOrWhiteSpace(c.ReviewContent))
                {
                    return new CommentAddReviewResult()
                    {
                        Success = false,
                        Message = "内容不能为空！"
                    };
                }
                else
                {
                    int contentLength = c.ReviewContent.Length;
                    if (c.ReviewContent.Length >= 600)
                    {
                        return new CommentAddReviewResult()
                        {
                            Success = false,
                            Message = "内容最长不能超过" + contentLength + "个字符！"
                        };
                    }
                    //禁止站外链接 ToDo
                    else if (false)
                    {

                    }
                }

                string utf8_Count = System.Web.HttpUtility.UrlEncode(c.ReviewContent, System.Text.Encoding.UTF8);//把评论内容转成utf-8编码的字符串
                long reviewID = CommentAdapter.AddReview4Comment(new CommentReviewsEntity()
                {
                    State = 0,
                    CommentID = c.CommentID,
                    Parent = c.ParentReviewID,
                    ReceiveUser = c.ReceiveUser,
                    SendUser = c.SendUser,
                    ReviewContent = utf8_Count,//c.ReviewContent,
                    TerminalType = HJDAPI.Common.Helpers.StringHelper.TransAppTypeHeaderToAppType(AppType)
                });

                //插入之后 开始添加消息记录 下一版4.2把消息发送 挪到队列里处理 
                MessageAdapter.InsertSysMessage(new SysMessageEnitity()
                {
                    state = 0,
                    businessID = reviewID,
                    businessType = c.ParentReviewID == 0 ? 1 : 2,//1代表是评论 2代表是回复
                    receiver = c.ReceiveUser
                });

                //reviewID 大于 0 插入新记录成功
                if (reviewID > 0)
                {
                    var sendNickName = AccountAdapter.GetUserBasicInfo(new List<long>() { c.SendUser }).First().NickName;
                    var content = c.ParentReviewID == 0 ? string.Format("您收到一条来自{0}的评论", sendNickName) : string.Format("您收到一条来自{0}的回复", sendNickName);

                    MessageAdapter.SendAppNotice(new SendNoticeEntity()
                    {
                        actionUrl = string.Format("whotelapp://www.zmjiudian.com/personal/comments/{0}", c.CommentID),
                        appType = 0,
                        from = sendNickName,
                        msg = content,
                        noticeType = c.ParentReviewID == 0 ? ZMJDNoticeType.review : ZMJDNoticeType.reply,
                        title = "周末酒店",
                        userID = c.ReceiveUser
                    });
                }

                return new CommentAddReviewResult()
                {
                    Success = reviewID > 0 ? true : false,
                    Message = reviewID > 0 ? "成功" : "失败"
                };
            }
            else
            {
                return new CommentAddReviewResult()
                {
                    Success = false,
                    Message = "验证签名错误"
                };
            }
        }

        /// <summary>
        /// 获取补充点评的模板内容
        /// </summary>
        /// <param name="commentID"></param>
        /// <returns></returns>
        [HttpGet]
        public CommentDefaultInfoModel GetAddCommentContentDefaultInfo(int commentID = 0)
        {
            return CommentAdapter.GetAddCommentContentDefaultInfo(commentID);
        }

        /// <summary>
        /// 获取补充点评的模板内容 20版本
        /// </summary>
        /// <param name="commentID"></param>
        /// <returns></returns>
        [HttpGet]
        public CommentDefaultInfoModel GetAddCommentContentDefaultInfo20(int commentID = 0)
        {
            return CommentAdapter.GetAddCommentContentDefaultInfo20(commentID);
        }

        /// <summary>
        /// ToDo 评论和回复 独立接口
        /// 类似百度贴吧（维护楼数）
        /// 根级别评论 分页显示
        /// 次级回复或评论在首页显示一部分 更多部分到新窗口显示 显示完毕跳回上一页第几楼
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpGet]
        public CommentReviewsResult GetCommentReviews(CommentReviewsParam param)
        {
            return new CommentReviewsResult();
        }

        /// <summary>
        /// 4.2版本App 写点评默认的选项
        /// </summary>
        /// <param name="hotelid"></param>
        /// <returns></returns>
        [HttpGet]
        public CommentDefaultInfoModel GetCommentDefaultInfo40(int hotelid, long userid = 0)
        {
            return CommentAdapter.GetCommentDefaultInfo40(hotelid);
        }

        //[HttpGet]
        //public string GetSayHello()
        //{
        //    return CommentAdapter.commentService.ExecuteMessageTask("12","123456");
        //}

        #region 计算积分
        [HttpGet]
        public CommentSharePageData GetCommentSharePageData(int commentID, long commentUserID, long orderID = 0, int hotelID = 0)
        {
            CommentEntity ce = null;
            if ((orderID > 0 && hotelID == 0) || (orderID == 0 && hotelID == 0) || commentUserID == 0)
            {
                ce = CommentAdapter.GetManyCommentBasicInfoEntity(new int[] { commentID }).FirstOrDefault();
                if (ce != null)
                {
                    orderID = ce.OrderID;
                    hotelID = ce.HotelID;
                    commentUserID = ce.UserID;  
                }
            }

            CommentShareModel shareModel = GetCommentShareInfo(commentID);//new CommentShareModel();
            shareModel.returnUrl = string.Format("{0}/Comment/CommentSubCompleted?commentid={1}&userid={2}&share=1", Configs.WWWURL, commentID, commentUserID);

            bool canAcquire = CommentAdapter.CanAcquireCommentPoint(commentID, hotelID, commentUserID, orderID);
            //Log.WriteLog(string.Format("canAcquire:{0},commentID:{1},hotelID:{2},commentUserID:{3},orderID:{4}", canAcquire, commentID, hotelID, commentUserID, orderID));
            if (canAcquire)
            {
                int point = CommentAdapter.CalculateCommentPoints(commentUserID, commentID, orderID);
                return new CommentSharePageData() { pointResult = new CalCommentPointResult() { point = point, needPoint = 30 }, commentShare = shareModel };
            }
            else
            {
                return new CommentSharePageData() { pointResult = new CalCommentPointResult() { point = 0, needPoint = 30 }, commentShare = shareModel };
            }
        }
        #endregion

        #region 记录分享和浏览  搜索记录

        /// <summary>
        /// 插入分享或浏览记录
        /// businessType 区分业务类型 1点评 2酒店
        /// recordType 区分记录类型 1分享 2浏览
        /// </summary>
        /// <param name="cbre"></param>
        /// <returns></returns>
        [HttpPost]
        public BasePostResult InsertUserRecord(CommonRecordParam recordParam)
        {
            recordParam.terminalType = recordParam.terminalType > 0 ? recordParam.terminalType : CommMethods.GetTerminalId(AppType);
            recordParam.clientIP = string.IsNullOrWhiteSpace(recordParam.clientIP) ? HttpRequestHelper.GetClientIp(this.ControllerContext.Request) : recordParam.clientIP;
            recordParam.appVersion = string.IsNullOrWhiteSpace(recordParam.appVersion) ? AppVer : recordParam.appVersion;

            ////分享点评得到积分
            //if (recordParam.userID != 0 && recordParam.recordType == 1)
            //{
            //    try
            //    {
            //        int ShareCount = HotelAdapter.GetPointslistNumByUserIdAndTypeId(recordParam.userID, 11).Select(_ => _.CreateTime.Date == DateTime.Now.Date) != null ? HotelAdapter.GetPointslistNumByUserIdAndTypeId(recordParam.userID, 11).Select(_ => _.CreateTime.Date == DateTime.Now.Date).Count() : 0;
            //        if (ShareCount <= 5)
            //        {
            //            PointsEntity pe = new PointsEntity()
            //            {
            //                BusinessID = 0,
            //                LeavePoint = 1,
            //                TotalPoint = 1,
            //                TypeID = 11,//11分享得到积分
            //                UserID = recordParam.userID,
            //                Approver = 0
            //            };
            //            HotelAdapter.HotelService.InsertOrUpdatePoints(pe);
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        Log.WriteLog("分享插入积分报错：userid" + recordParam.userID + " " + e.Message + "\r\n" + e.StackTrace);
            //    }
            //}

            return HotelAdapter.InsertUserRecord(recordParam);
        }

        /// <summary>
        /// 返回分享跟踪的加密字符串
        /// 解密后得到的是数据json字符串
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public TrackCodeData GenTrackCodeResult4Share(GenTrackCodeParam param)
        {
            return DescriptionHelper.GenTrackCodeResult4Share(param);
        }

        #endregion

        #region 首页酒店推荐点评

        /// <summary>
        /// 首页推荐点评列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public RecommendCommentListModel GetRecommendCommentListModel([FromUri]RecommendCommentListQueryParam param)
        {
            return CommentAdapter.GetWonderfulCommentListModel(param);
        }

        #endregion

        #region 酒店详情页推荐点评
        /// <summary>
        /// 查看关注人写的某个酒店的点评
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="hotelId"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [HttpGet]
        public ReviewResult40 GetCommentInfosByFollowing(long userId, int hotelId, int start = 0, int count = 4)
        {
            userId = userId == 0 ? AppUserID : userId;
            var reviews = new ReviewResult40()
            {
                HotelID = hotelId,
                Start = start
            };

            if (userId > 0 && hotelId > 0)
            {
                int totalCount = 0;
                List<CommentInfoEntity> commentList = CommentAdapter.GetCommentInfosByFollowing(userId, hotelId,
                    new int[] { 1 }, out totalCount, start, count);
                if (commentList != null && commentList.Count != 0)
                {
                    reviews.TotalCount = totalCount;
                    reviews.AllReviewCount = totalCount;
                    reviews.GroupTotalCount = totalCount;
                    reviews.Start = start;

                    reviews.Result = CommentAdapter.GenCommentItems(commentList);

                    var likeCommentIds = CommentAdapter.commentService.GetUserClickUsefulComment(userId);
                    if (likeCommentIds != null && likeCommentIds.Count > 0)
                    {
                        foreach (CommentItem item in reviews.Result)
                        {
                            item.HasClickUseful = likeCommentIds.Contains(item.Id) ? true : false;
                        }
                    }
                }
            }
            return reviews;
        }
        /// <summary>
        /// 查看品鉴师写的某个酒店的点评
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="hotelId"></param>
        /// <param name="start"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [HttpGet]
        public ReviewResult40 GetCommentInfosByInspector(long userId, int hotelId, int start = 0, int count = 4)
        {
            var reviews = new ReviewResult40()
            {
                HotelID = hotelId,
                Start = start
            };
            if (hotelId > 0)
            {
                int totalCount = 0;
                List<CommentInfoEntity> commentList = CommentAdapter.GetCommentInfosByInspector(hotelId,
                    new int[] { 1 }, out totalCount, start, count);
                if (commentList != null && commentList.Count != 0)
                {
                    reviews.TotalCount = totalCount;
                    reviews.AllReviewCount = totalCount;
                    reviews.GroupTotalCount = totalCount;
                    reviews.Start = start;

                    reviews.Result = CommentAdapter.GenCommentItems(commentList);

                    var likeCommentIds = CommentAdapter.commentService.GetUserClickUsefulComment(userId);
                    if (likeCommentIds != null && likeCommentIds.Count > 0)
                    {
                        foreach (CommentItem item in reviews.Result)
                        {
                            item.HasClickUseful = likeCommentIds.Contains(item.Id) ? true : false;
                        }
                    }
                }
            }
            return reviews;
        }
        #endregion

        #region 优惠套餐或者房券活动的点评
        [HttpGet]
        public ReviewResult40 GetCommentInfosByActivityId(int activityId, int start = 0, int count = 4)
        {
            var reviews = new ReviewResult40()
            {
                HotelID = 0,
                Start = start
            };


            return reviews;
        }

        [HttpGet]
        public ReviewResult40 GetCommentInfosByPId(int pid, int start = 0, int count = 3)
        {
            return CommentAdapter.GetCommentInfosByPId(pid, start, count);
        }
        #endregion

        #region 获取热点点评
        /// <summary>
        /// 4.6版App 发现页获取热点点评
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpGet]
        public RecommendCommentListModel GetPublishedCommentList([FromUri]RecommendCommentParam param)
        {
            return CommentAdapter.GetPublishedCommentList(param);
        }
        #endregion


        #region 推荐
        /// <summary>
        /// 4.6版App 发现页获取热点点评
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpGet]
        public RecommendCommentListModel GetPublishedCommentsList([FromUri]RecommendCommentParam param)
        {
            return CommentAdapter.GetPublishedCommentsList(param);
        }
        #endregion
    }
}